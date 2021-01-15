using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HierarchicalPropertyDefault.Internal;

namespace HierarchicalPropertyDefault
{
    public abstract class HierarchicalObject<TParent, TSelf> : INotifyPropertyChanged
    {
        private TParent? parent;
        private readonly Mapping<TParent> mapping;
        private readonly ConcurrentDictionary<string, ValueWrapper> propertyValues = new();
        private readonly ChangeListener? changeListener;

        #region Constructors

        protected HierarchicalObject() : this(x => x.AutoMap()) { }

        protected HierarchicalObject(Action<MappingConfiguration<TParent, TSelf>>? configuration)
        {
            if (typeof(TSelf) != this.GetType())
                throw new TypeMismatchException($"Current type ({this.GetType().FullName}) does not match type parameter \"{nameof(TSelf)}\" ({typeof(TSelf).FullName})");

            this.mapping = Mapping<TParent>.CreateMap(configuration);

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(TParent)))
            {
                this.changeListener = new ChangeListener(typeof(TParent));
                this.changeListener.Configure(this.mapping);
                this.changeListener.ChangeTriggered += this.ChangeListener_ChangeTriggered;
            }
        }

        protected HierarchicalObject(Mapping<TParent> mapping)
        {
            if (typeof(TSelf) != this.GetType())
                throw new TypeMismatchException($"Current type ({this.GetType().FullName}) does not match type parameter \"{nameof(TSelf)}\" ({typeof(TSelf).FullName})");

            if (mapping.TargetType != typeof(TSelf))
                throw new ArgumentException($"Mapping target ({this.GetType().FullName}) does not match type parameter \"{nameof(TSelf)}\" ({typeof(TSelf).FullName})", nameof(mapping));

            this.mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(TParent)))
            {
                this.changeListener = new ChangeListener(typeof(TParent));
                this.changeListener.Configure(this.mapping);
                this.changeListener.ChangeTriggered += this.ChangeListener_ChangeTriggered;
            }
        }

        #endregion

        protected internal TParent? Parent
        {
            get => this.parent;
            set
            {
                if (ReferenceEquals(this, value))
                    return;

                this.parent = value;

                this.changeListener?.UpdateValue(this.parent as INotifyPropertyChanged);
            }
        }

        #region INotifyPropertyChanged Handler & Implementation

        private void ChangeListener_ChangeTriggered(object sender, string e)
        {
            if (this.propertyValues.ContainsKey(e))
                return;

            this.OnPropertyChangedWithDependents(e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChangedWithDependents(string propertyName)
        {
            this.OnPropertyChanged(propertyName);
            foreach (var dep in this.mapping.GetDependents(propertyName))
                this.OnPropertyChanged(dep);
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region Protected Property Methods

        protected internal T? GetPropertyValue<T>([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return default;

            return this.propertyValues.TryGetValue(propertyName, out var value)
                && value is ValueWrapper<T> wrapper
                ? wrapper.Value
                : this.mapping.GetValue<T>(this.parent, propertyName);
        }

        protected internal void SetPropertyValue<T>(T? value, [CallerMemberName] string? propertyName = null, bool clearOnNull = false)
        {
            if (propertyName is null) return;

            if (clearOnNull && value is null)
                this.ClearPropertyValue(propertyName);
            else
            {
                this.propertyValues.AddOrUpdate(propertyName,
                    _ => new ValueWrapper<T> { Value = value },
                    (_, old) =>
                    {
                        if (old is ValueWrapper<T> reuse)
                        {
                            reuse.Value = value;
                            return reuse;
                        }
                        return new ValueWrapper<T> { Value = value };
                    });

                this.OnPropertyChangedWithDependents(propertyName);
            }
        }

        protected internal void ClearPropertyValue([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return;
            this.propertyValues.TryRemove(propertyName, out var _);
            this.OnPropertyChangedWithDependents(propertyName);
        }

        protected internal bool GetPropertyHasValue([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return false;
            return this.propertyValues.ContainsKey(propertyName);
        }

        protected internal void SetPropertyHasValue<T>(bool hasValue, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return;
            if (hasValue)
            {
                if (this.propertyValues.ContainsKey(propertyName)) return;

                this.propertyValues[propertyName] = new ValueWrapper<T> { Value = this.mapping.GetValue<T>(this.parent, propertyName) };

                this.OnPropertyChangedWithDependents(propertyName);
            }
            else
            {
                this.ClearPropertyValue(propertyName);
            }
        }

        protected internal Optional<T?> GetPropertyValueOptional<T>([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return default;

            return this.propertyValues.TryGetValue(propertyName, out var value)
                && value is ValueWrapper<T> wrapper
                ? wrapper.Value
                : new Optional<T?>();
        }

        protected internal void SetPropertyValueOptional<T>(Optional<T?> value, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return;

            if (value.HasValue)
                this.SetPropertyValue(value: value.Value, propertyName: propertyName, clearOnNull: false);
            else
                this.ClearPropertyValue(propertyName: propertyName);
        }

        #endregion
    }
}
