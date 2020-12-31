using System.Collections.Generic;

namespace HierarchicalPropertyDefault
{
    public readonly struct Optional<T>
    {
        private readonly bool hasValue;
        private readonly T? value;

        private Optional(T? value)
        {
            this.value = value;
            this.hasValue = true;
        }

        public Optional(bool hasValue, T? value)
        {
            this.hasValue = hasValue;
            this.value = this.hasValue ? value : default;
        }

        public bool HasValue => this.hasValue;

        public T? Value => this.hasValue ? this.value : default;

        public override bool Equals(object? other)
        {
            // if the other object is the same type as this
            if (other is Optional<T> h)
            {
                // if both object doesn't have value,
                // or their values are equal.
                return !(this.hasValue || h.hasValue) || EqualityComparer<T>.Default.Equals(this.value!, h.value!);
            }
            else
            {
                return this.hasValue && this.value != null
                    ? this.value.Equals(other)
                    : other == null;
            }
        }

        public override int GetHashCode() => this.hasValue ? (this.value?.GetHashCode() ?? 0) : 0;

        public static implicit operator Optional<T>(T? value) => new Optional<T>(value);
        public static explicit operator T?(Optional<T> value) => value!.Value;
    }
}
