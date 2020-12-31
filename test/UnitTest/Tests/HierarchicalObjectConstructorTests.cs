using System;
using HierarchicalPropertyDefault;
using UnitTest.Models;
using Xunit;

namespace UnitTest.Tests
{
    public class HierarchicalObjectConstructorTests
    {
        [Fact]
        public void Test()
        {
            new TestSubjectClass();

            new TestSubjectClass(c => c.Clear().AutoMap());
            new TestSubjectClass(c => c.Clear().ForProperty(x => x.Property1, x => x.Property1).ForProperty(x => x.Property3, x => x.Property3));

            var mapping = Mapping<Tree>.CreateMap<TestSubjectClass>();

            new TestSubjectClass(mapping);
        }

        public class TestSubjectClass : HierarchicalObject<Tree, TestSubjectClass>
        {
            public TestSubjectClass()
            {
            }

            public TestSubjectClass(Action<MappingConfiguration<Tree, TestSubjectClass>>? configuration) : base(configuration)
            {
            }

            public TestSubjectClass(Mapping<Tree> mapping) : base(mapping)
            {
            }

            public string? Property1 { get => this.GetPropertyValue<string?>(); set => this.SetPropertyValue(value); }
            public int Property2 { get => this.GetPropertyValue<int>(); set => this.SetPropertyValue(value); }
            public int? Property3 { get => this.GetPropertyValue<int?>(); set => this.SetPropertyValue(value); }
            public bool Property4 { get => this.GetPropertyValue<bool>(); set => this.SetPropertyValue(value); }
            public bool? Property5 { get => this.GetPropertyValue<bool?>(); set => this.SetPropertyValue(value); }
        }
    }
}
