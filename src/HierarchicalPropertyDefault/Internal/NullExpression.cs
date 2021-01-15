using System.Linq.Expressions;

namespace HierarchicalPropertyDefault.Internal
{
    internal static class NullExpression
    {
        internal static Expression Null = Expression.Constant(null);
    }
}
