using System.ComponentModel;
using Moq;
using UnitTest.Models;
using Xunit;

namespace UnitTest.Tests
{
    public class NotifyChildTests
    {
        [Fact]
        public void WillNotifyChild()
        {
            const string T1 = "test1";
            const string T2 = "test2";
            const string T3 = "test3";

            var mock = new Mock<PropertyChangedEventHandler>();
            var p = new NotifyParent1();
            var c = new NotifyChild { Parent = p };
            c.PropertyChanged += mock.Object;

            Assert.Null(c.ChildProperty1);
            Assert.Null(c.ChildProperty2);
            Assert.Null(c.ChildProperty3);

            p.Property1 = T1;
            mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty1))), Times.Once());
            mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty4))), Times.Once());
            mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty5))), Times.Once());
            mock.VerifyNoOtherCalls();
            Assert.Equal(T1, c.ChildProperty1);

            p.Object = new NotifyParent2();
            mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty2))), Times.Once());
            mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty3))), Times.Once());
            mock.VerifyNoOtherCalls();
            Assert.Null(c.ChildProperty2);
            Assert.Null(c.ChildProperty3);

            p.Object.Property2 = T2;
            mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty2))), Times.Exactly(2));
            mock.VerifyNoOtherCalls();
            Assert.Equal(T2, c.ChildProperty2);
            Assert.Null(c.ChildProperty3);

            p.Object.Object = new NotifyParent3();
            mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty3))), Times.Exactly(2));
            mock.VerifyNoOtherCalls();
            Assert.Null(c.ChildProperty3);

            p.Object.Object.Property3 = T3;
            mock.Verify(x => x(It.IsAny<object>(), It.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(NotifyChild.ChildProperty3))), Times.Exactly(3));
            mock.VerifyNoOtherCalls();
            Assert.Equal(T3, c.ChildProperty3);
        }
    }
}
