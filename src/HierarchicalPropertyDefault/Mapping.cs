using System;
using System.Collections.Generic;
using System.Linq;
using HierarchicalPropertyDefault.Internal;

namespace HierarchicalPropertyDefault
{
    public class Mapping<TSource>
    {
        public static Mapping<TSource> CreateMap<TTarget>() => CreateMap<TTarget>(x => x.AutoMap());

        public static Mapping<TSource> CreateMap<TTarget>(Action<MappingConfiguration<TSource, TTarget>>? configuration)
        {
            var config = new MappingConfiguration<TSource, TTarget>();

            configuration?.Invoke(config);

            return new Mapping<TSource>(typeof(TTarget), config.Map.Values);
        }

        private Mapping(Type target, IEnumerable<MappingInfo<TSource>> mappings)
        {
            this.TargetType = target ?? throw new ArgumentNullException(nameof(target));

            if (mappings is null)
                throw new ArgumentNullException(nameof(mappings));

            this.TargetLookupDictionary = mappings.ToDictionary(x => x.TargetProperty.Name);
        }

        internal readonly IReadOnlyDictionary<string, MappingInfo<TSource>> TargetLookupDictionary;

        public Type TargetType { get; private set; }

        internal T? GetValue<T>(TSource? parent, string propertyName)
            => parent is null || propertyName is null
                ? default
                : this.TargetLookupDictionary.TryGetValue(propertyName, out var v)
                    && v is MappingInfo<TSource, T> mi
                        ? mi.GetValue(parent)
                        : default;

        internal IEnumerable<string> GetDependents(string propertyName)
            => this.TargetLookupDictionary.TryGetValue(propertyName, out var v)
                ? v.Dependents
                : Enumerable.Empty<string>();
    }
}
