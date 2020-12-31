using System.ComponentModel;
using Moq;
using UnitTest.Models;
using Xunit;

namespace UnitTest.Tests
{
    public class NotifySelfTests
    {
        private readonly Mock<PropertyChangedEventHandler> mock = new Mock<PropertyChangedEventHandler>();
        private readonly NotifyChild obj = new NotifyChild();

        public NotifySelfTests() => this.obj.PropertyChanged += this.mock.Object;

        [Fact]
        public void WillNotifyWhenSetValue()
        {
            this.obj.ChildProperty1 = "test";

            this.mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty1))), Times.Once());
            this.mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty4))), Times.Once());
            this.mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty5))), Times.Once());
            this.mock.VerifyNoOtherCalls();
        }

        [Fact]
        public void WillNotifyWhenSetHasValue()
        {
            this.obj.HasChildProperty1 = true;

            this.mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty1))), Times.Once());
            this.mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty4))), Times.Once());
            this.mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty5))), Times.Once());
            this.mock.VerifyNoOtherCalls();
        }

        [Fact]
        public void WillNotifyWhenSetValueOptional()
        {
            this.obj.OptionalChildProperty1 = new HierarchicalPropertyDefault.Optional<string?>(true, "test");

            this.mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty1))), Times.Once());
            this.mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty4))), Times.Once());
            this.mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty5))), Times.Once());
            this.mock.VerifyNoOtherCalls();
        }
    }
}
