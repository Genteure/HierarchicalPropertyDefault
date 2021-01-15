using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HierarchicalPropertyDefault.Internal
{
    internal abstract class MappingInfo<TSource>
    {
        protected MappingInfo(PropertyInfo targetProperty, IReadOnlyList<PropertyInfo> sourceProperties, IReadOnlyList<string> dependents)
        {
            this.TargetProperty = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
            this.SourceProperties = sourceProperties ?? throw new ArgumentNullException(nameof(sourceProperties));
            this.Dependents = dependents ?? throw new ArgumentNullException(nameof(dependents));
        }

        public PropertyInfo TargetProperty { get; protected set; }

        public IReadOnlyList<PropertyInfo> SourceProperties { get; protected set; }

        public IReadOnlyList<string> Dependents { get; }
    }

    internal class MappingInfo<TSource, TValue> : MappingInfo<TSource>
    {
        public MappingInfo(PropertyInfo targetProperty, IReadOnlyList<PropertyInfo> sourceProperties, IReadOnlyList<string> dependents) : base(targetProperty, sourceProperties, dependents)
        {
            if (sourceProperties.Count == 0)
                throw new ArgumentException($"sourceProperties must have 1 or more properties.", nameof(sourceProperties));

            for (int i = 0; i < sourceProperties.Count - 1; i++)
            {
                var a = sourceProperties[i];
                var b = sourceProperties[i + 1];

                if (!a.CanRead)
                    throw new ArgumentException($"Property \"{a.ReflectedType.Name}.{a.Name}\" is write-only.", nameof(sourceProperties));

                if (!a.ReflectedType.IsAssignableFrom(b.PropertyType))
                    throw new ArgumentException($"Property of \"{b.ReflectedType.Name}.{a.Name}\" does not match \"{b.PropertyType.Name}\".", nameof(sourceProperties));
            }

            {
                var last = sourceProperties[sourceProperties.Count - 1];
                if (!last.CanRead)
                    throw new ArgumentException($"Property \"{last.ReflectedType.Name}.{last.Name}\" is write-only.", nameof(sourceProperties));
            }

            var arg = Expression.Parameter(typeof(TSource), "s");
            var def = Expression.Default(typeof(TValue));
            var body = sourceProperties.Reverse().Aggregate(arg as Expression, (e, p) => Expression.Property(e, p));
            for (int i = sourceProperties.Count - 1; i >= 0; i--)
                body = Expression.Condition(Expression.Equal(sourceProperties.Reverse().Take(i).Aggregate(arg as Expression, (e, p) => Expression.Property(e, p)), NullExpression.Null), def, body);
            var exp = Expression.Lambda<Func<TSource, TValue>>(body, arg);
            this.GetValue = exp.Compile();
        }

        public MappingInfo(PropertyInfo targetProperty, Func<TSource, TValue> getValueFunc) : base(targetProperty, Array.Empty<PropertyInfo>(), Array.Empty<string>())
            => this.GetValue = getValueFunc ?? throw new ArgumentNullException(nameof(getValueFunc));

        public Func<TSource, TValue> GetValue { get; }
    }
}
