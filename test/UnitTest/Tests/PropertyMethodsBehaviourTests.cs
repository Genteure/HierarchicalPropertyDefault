using HierarchicalPropertyDefault;
using Xunit;

namespace UnitTest.Tests
{
    public class PropertyMethodsBehaviourTests : HierarchicalObject<object, PropertyMethodsBehaviourTests>
    {
        private const string IntPropertyName = "IntProperty";
        private const string StringPropertyName = "StringProperty";

        public PropertyMethodsBehaviourTests()
        {
            this.SetPropertyValue(5, IntPropertyName);
            this.SetPropertyValue("test", StringPropertyName);
        }

        [Fact]
        public void GetPropertyHasValueTest()
        {
            Assert.True(this.GetPropertyHasValue(IntPropertyName));
            Assert.True(this.GetPropertyHasValue(StringPropertyName));

            Assert.False(this.GetPropertyHasValue("name"));
        }

        [Fact]
        public void GetPropertyValueTest()
        {
            Assert.Equal(5, this.GetPropertyValue<int>(IntPropertyName));
            Assert.Equal("test", this.GetPropertyValue<string>(StringPropertyName));

            Assert.Null(this.GetPropertyValue<string>("name"));
        }

        [Fact]
        public void SetPropertyHasValueTest()
        {
            const string PropertyName = "SetTest";

            Assert.False(this.GetPropertyHasValue(PropertyName));
            this.SetPropertyHasValue<string>(true, PropertyName);
            Assert.True(this.GetPropertyHasValue(PropertyName));
            this.SetPropertyHasValue<int>(false, PropertyName);
            Assert.False(this.GetPropertyHasValue(PropertyName));
        }
    }
}
