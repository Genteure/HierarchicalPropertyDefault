using HierarchicalPropertyDefault;
using System;
using UnitTest.Models;
using Xunit;

namespace UnitTest.Tests
{
    public class MappingTest
    {
        [Fact]
        public void Test1()
        {
            var map = Mapping<Basic1>.CreateMap<Basic2>(config
                => config
                .Clear()
                .AutoMap()
                .ForProperty(x => x.Property1, x => x.Property1)
                .ForProperty(x => x.Property2, x => x.Property3)
                .IgnoreProperty(x => x.Property3));
        }
    }
}
