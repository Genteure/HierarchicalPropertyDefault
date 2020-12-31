using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HierarchicalPropertyDefault;

namespace UnitTest.Models
{
    public class NotifyParent1 : INotifyPropertyChanged
    {
        private string? property1;
        private NotifyParent2? @object;

        public string? Property1 { get => this.property1; set => this.SetField(ref this.property1, value); }
        public NotifyParent2? Object { get => this.@object; set => this.SetField(ref this.@object, value); }

        private void SetField<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class NotifyParent2 : INotifyPropertyChanged
    {
        private string? property2;
        private NotifyParent3? @object;

        public string? Property2 { get => this.property2; set => this.SetField(ref this.property2, value); }
        public NotifyParent3? Object { get => this.@object; set => this.SetField(ref this.@object, value); }

        private void SetField<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class NotifyParent3 : INotifyPropertyChanged
    {
        private string? property3;

        public string? Property3 { get => this.property3; set => this.SetField(ref this.property3, value); }

        private void SetField<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class NotifyChild : HierarchicalObject<NotifyParent1, NotifyChild>
    {
        public NotifyChild() : base(c => c
            .ForProperty(x => x.ChildProperty1, x => x.Property1, nameof(ChildProperty4), nameof(ChildProperty5))
            .ForProperty(x => x.ChildProperty2, x => x.Object!.Property2)
            .ForProperty(x => x.ChildProperty3, x => x.Object!.Object!.Property3))
        { }

        public string? ChildProperty1 { get => this.GetPropertyValue<string>(); set => this.SetPropertyValue(value); }
        public bool HasChildProperty1
        {
            get => this.GetPropertyHasValue(nameof(this.ChildProperty1));
            set => this.SetPropertyHasValue<string>(value, nameof(this.ChildProperty1));
        }
        public Optional<string?> OptionalChildProperty1
        {
            get => this.GetPropertyValueOptional<string>(nameof(this.ChildProperty1));
            set => this.SetPropertyValueOptional(value, nameof(this.ChildProperty1));
        }

        public string? ChildProperty2 { get => this.GetPropertyValue<string>(); set => this.SetPropertyValue(value); }
        public string? ChildProperty3 { get => this.GetPropertyValue<string>(); set => this.SetPropertyValue(value); }
        public string? ChildProperty4 { get => this.GetPropertyValue<string>(); set => this.SetPropertyValue(value); }
        public string? ChildProperty5 { get => this.GetPropertyValue<string>(); set => this.SetPropertyValue(value); }
    }
}
