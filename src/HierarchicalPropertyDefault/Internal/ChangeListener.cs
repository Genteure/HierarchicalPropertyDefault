using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HierarchicalPropertyDefault.Internal
{
    /**     Glossary
     * 
     * value:
     *      The object instance this ChangeListener is targeting.
     * property:
     *      Same as "source property".
     * source property:
     *      A property of value. 
     *      This ChangeListener is tracking changes on source properties.
     * source name:
     *      The name of source property.
     * property value:
     *      The value of a source property.
     * target (target name):
     *      The name of the binding target.
     *      Does not has any function in the context of ChangeListener, treat as an arbitrary value.
     * 
     * */

    internal class ChangeListener : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// Type of <see cref="Value"/>
        /// </summary>
        private Type Type { get; }

        /// <summary>
        /// Current value of this <see cref="ChangeListener"/>
        /// </summary>
        private INotifyPropertyChanged? Value { get; set; }

        /// <summary>
        /// Getter function of source properties
        /// </summary>
        /// <remarks>
        /// Key: Name of the source property.
        /// Value: Getter function for the property. Null if property value does not impl INotifyPropertyChanged.
        /// </remarks>
        private Dictionary<string, Func<INotifyPropertyChanged, INotifyPropertyChanged?>?> PropertyValueGetters { get; } = new();

        /// <summary>
        /// Children of this <see cref="ChangeListener"/>
        /// </summary>
        /// <remarks>
        /// Key: Name of the source property.
        /// Value: ChangeListener for the corresponding property.
        /// </remarks>
        private Dictionary<string, ChangeListener> Children { get; } = new();

        /// <summary>
        /// Targets of this <see cref="ChangeListener"/>
        /// </summary>
        /// <remarks>
        /// Key: Name of the source property.
        /// Value: List of target names.
        /// </remarks>
        private Dictionary<string, List<string>> Targets { get; } = new();

        /// <summary>
        /// Constructor for <see cref="ChangeListener"/>
        /// </summary>
        /// <param name="type">Allowed value type</param>
        public ChangeListener(Type type)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
                throw new ArgumentException("Type must implement INotifyPropertyChanged.", nameof(type));
        }

        public event EventHandler<string>? ChangeTriggered;

        internal void Configure<T>(Mapping<T> mapping)
        {
            foreach (var item in mapping.TargetLookupDictionary.Values)
                if (item.SourceProperties.Count > 0)
                    this.AddSubscription(item.TargetProperty.Name, new Queue<PropertyInfo>(item.SourceProperties.Reverse()));
        }

        internal void AddSubscription(string targetName, Queue<PropertyInfo> sourcePath)
        {
            if (this.disposedValue) throw new ObjectDisposedException(nameof(ChangeListener));

            if (sourcePath.Count == 0)
                throw new ArgumentException("PropertyInfo Queue must not be empty.", nameof(sourcePath));

            var p = sourcePath.Dequeue();
            var sourceName = p.Name;

            if (!this.Type.IsAssignableFrom(p.ReflectedType))
                throw new ArgumentException($"Type mismatch: \"{this.Type.FullName}\" is not assignable from \"{p.ReflectedType.FullName}\"", nameof(sourcePath));

            if (sourcePath.Count == 0)
            {
                // last node of the tree
                AddTarget(targetName, sourceName);
                return;
            }

            // Child ChangeListener should handle this
            if (this.PropertyValueGetters.TryGetValue(sourceName, out var getter))
            {
                if (getter is null)
                {
                    // type of source property does not impl INotifyPropertyChanged.
                    // notifying tree terminates here.
                    AddTarget(targetName, sourceName);
                }
                else
                {
                    // pass info to child ChangeListener
                    if (this.Children.TryGetValue(sourceName, out var child))
                        child.AddSubscription(targetName, sourcePath);
                    else
                        throw new InvalidOperationException("No child listener found.");
                }
            }
            else
            {
                // check and create new a child ChangeListener
                if (typeof(INotifyPropertyChanged).IsAssignableFrom(p.PropertyType))
                {
                    // build getter function
                    var arg = Expression.Parameter(typeof(INotifyPropertyChanged), "s");
                    var body = Expression.Convert(Expression.Property(Expression.Convert(arg, p.DeclaringType), p), typeof(INotifyPropertyChanged));
                    var exp = Expression.Lambda<Func<INotifyPropertyChanged, INotifyPropertyChanged?>>(body, arg);
                    var func = exp.Compile();
                    this.PropertyValueGetters[sourceName] = func;

                    // create child
                    var child = new ChangeListener(p.PropertyType);
                    child.ChangeTriggered += this.Child_ChangeTriggered;
                    this.Children[sourceName] = child;
                    this.UpdateChildValue(sourceName);

                    // pass info to child ChangeListener
                    child.AddSubscription(targetName, sourcePath);
                }
                else
                {
                    // type of source property does not impl INotifyPropertyChanged.
                    this.PropertyValueGetters[sourceName] = null;

                    // notifying tree terminates here.
                    AddTarget(targetName, sourceName);
                }
            }
            return;
            void AddTarget(string targetName, string sourceName)
            {
                List<string> list;
                if (this.Targets.ContainsKey(sourceName))
                    list = this.Targets[sourceName];
                else
                {
                    list = new();
                    this.Targets[sourceName] = list;
                }

                if (!list.Contains(targetName))
                    list.Add(targetName);
            }
        }

        internal void UpdateValue(INotifyPropertyChanged? value)
        {
            if (this.disposedValue) throw new ObjectDisposedException(nameof(ChangeListener));

            if (this.Value is not null)
                this.Value.PropertyChanged -= this.Value_PropertyChanged;

            this.Value = value;

            if (this.Value is not null)
                this.Value.PropertyChanged += this.Value_PropertyChanged;

            foreach (var list in this.Targets.Values)
                list.ForEach(t => ChangeTriggered?.Invoke(this, t));

            foreach (var sourceName in this.Children.Keys)
                this.UpdateChildValue(sourceName);
        }

        private void UpdateChildValue(string sourceName)
        {
            if (!this.PropertyValueGetters.TryGetValue(sourceName, out var getter)
                || getter is null
                || !this.Children.TryGetValue(sourceName, out var child))
                return;

            if (this.Value is null)
                child.UpdateValue(null);
            else
            {
                var childValue = getter(this.Value);
                child.UpdateValue(childValue);
            }
        }

        private void Child_ChangeTriggered(object sender, string e)
        {
            // pass event up to root
            if (!this.disposedValue)
                ChangeTriggered?.Invoke(sender, e);
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.disposedValue) return;

            if (this.Targets.TryGetValue(e.PropertyName, out var list))
                list.ForEach(t => ChangeTriggered?.Invoke(this, t));

            this.UpdateChildValue(e.PropertyName);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                this.disposedValue = true;

                if (disposing)
                {
                    // dispose managed state (managed objects)
                    if (this.Value is not null)
                        this.Value.PropertyChanged -= this.Value_PropertyChanged;
                    this.Value = null;

                    ChangeTriggered = null;

                    foreach (var item in this.Children)
                        item.Value.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                this.Targets.Clear();
                this.PropertyValueGetters.Clear();
                this.Children.Clear();
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ChangeListener()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
