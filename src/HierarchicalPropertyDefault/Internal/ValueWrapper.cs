namespace HierarchicalPropertyDefault.Internal
{
    internal class ValueWrapper<T> : ValueWrapper
    {
        public T? Value { get; set; }
    }
    internal class ValueWrapper
    {
        internal static readonly ValueWrapper Default = new ValueWrapper();
    }
}
