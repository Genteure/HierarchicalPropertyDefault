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
                /**
                 * this.hasValue h.hasValue this.value h.value should_return
                 *    true         true        true     true     true   // all equal
                 *    true         true        false    false    true   // all equal
                 *    true         true        true     false    false  // different value
                 *    true         true        false    true     false  // different value
                 *    false        true        N/A       ANY     false  // one don't have value
                 *    true         false       ANY       N/A     false  // one don't have value
                 *    false        false       N/A       N/A     true   // both don't have value
                 */

                return this.hasValue == h.hasValue && (!this.hasValue || EqualityComparer<T>.Default.Equals(this.value!, h.value!));
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
