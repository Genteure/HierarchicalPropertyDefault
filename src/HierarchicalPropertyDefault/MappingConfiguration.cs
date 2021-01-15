using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HierarchicalPropertyDefault.Internal;

namespace HierarchicalPropertyDefault
{
    public class MappingConfiguration<TSource, TTarget>
    {
        internal readonly Dictionary<PropertyInfo, MappingInfo<TSource>> Map = new();

        internal MappingConfiguration() { }

        public MappingConfiguration<TSource, TTarget> Clear()
        {
            this.Map.Clear();
            return this;
        }

        public MappingConfiguration<TSource, TTarget> AutoMap() => this.AutoMap(_ => Array.Empty<string>());

        public MappingConfiguration<TSource, TTarget> AutoMap(Func<PropertyInfo, IReadOnlyList<string>> getDependents)
        {
            var tproperties = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var sproperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in tproperties)
            {
                var source = sproperties.FirstOrDefault(x => x.Name.Equals(prop.Name));
                if (source is not null && source.CanRead && source.PropertyType == prop.PropertyType)
                {
                    var valueType = source.PropertyType;
                    var miType = typeof(MappingInfo<,>).MakeGenericType(typeof(TSource), valueType);
                    var mi = (MappingInfo<TSource>)Activator.CreateInstance(miType, prop, new[] { source }, getDependents(prop));
                    this.Map[prop] = mi;
                }
            }

            return this;
        }

        public MappingConfiguration<TSource, TTarget> ForProperty<TProperty>(Expression<Func<TTarget, TProperty>> targetExpression, Expression<Func<TSource, TProperty>> sourceExpression)
            => this.ForProperty(targetExpression, sourceExpression, Array.Empty<string>() as IReadOnlyList<string>);

        public MappingConfiguration<TSource, TTarget> ForProperty<TProperty>(Expression<Func<TTarget, TProperty>> targetExpression, Expression<Func<TSource, TProperty>> sourceExpression, params string[] dependents)
           => this.ForProperty(targetExpression, sourceExpression, dependents as IReadOnlyList<string>);

        public MappingConfiguration<TSource, TTarget> ForProperty<TProperty>(Expression<Func<TTarget, TProperty>> targetExpression, Expression<Func<TSource, TProperty>> sourceExpression, IReadOnlyList<string> dependents)
        {
            var targetProperty = EnsureProperty(targetExpression);

            var sourceList = new List<PropertyInfo>();
            var body = sourceExpression.Body;
            while (true)
            {
                if (body is MemberExpression mb)
                {
                    var member = mb.Member;
                    if (member is not PropertyInfo sourceProperty) throw new ArgumentException("Only properties is allowed.", nameof(sourceExpression));
                    if (sourceProperty.GetGetMethod(true)?.IsStatic ?? sourceProperty.GetSetMethod(true).IsStatic)
                        throw new ArgumentException("Static property is not supported.", nameof(sourceExpression));

                    sourceList.Add(sourceProperty);

                    if (mb.Expression is ParameterExpression)
                        break;
                    else
                        body = mb.Expression;
                }
                else
                    throw new ArgumentException("Unsupported expression", nameof(sourceExpression));
            }

            var mi = new MappingInfo<TSource, TProperty>(targetProperty, sourceList, dependents);
            this.Map[targetProperty] = mi;

            return this;
        }

        public MappingConfiguration<TSource, TTarget> ForPropertyWithFunc<TProperty>(Expression<Func<TTarget, TProperty>> targetExpression, Func<TSource, TProperty> sourceFunc)
        {
            var targetProperty = EnsureProperty(targetExpression);

            var mi = new MappingInfo<TSource, TProperty>(targetProperty, sourceFunc);
            this.Map[targetProperty] = mi;

            return this;
        }

        public MappingConfiguration<TSource, TTarget> IgnoreProperty<TProperty>(Expression<Func<TTarget, TProperty>> targetExpression)
        {
            var targetProperty = EnsureProperty(targetExpression);

            this.Map.Remove(targetProperty);

            return this;
        }

        private static PropertyInfo EnsureProperty<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            var targetMember = (expression.Body as MemberExpression)?.Member;
            if (targetMember is null) throw new ArgumentException("Expression not supported.", nameof(expression));
            if (targetMember.ReflectedType != typeof(T1)) throw new ArgumentException("Must use a property from the target type.", nameof(expression));
            if (targetMember is not PropertyInfo targetProperty) throw new ArgumentException("Must use a property as target", nameof(expression));
            if (targetProperty.GetGetMethod(true)?.IsStatic ?? targetProperty.GetSetMethod(true).IsStatic)
                throw new ArgumentException("Static property is not supported", nameof(expression));

            return targetProperty;
        }
    }
}
