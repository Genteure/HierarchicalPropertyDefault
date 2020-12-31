using HierarchicalPropertyDefault;
using UnitTest.Models;
using Xunit;

namespace UnitTest.Tests
{
    public class BasicTests
    {
        [Fact]
        public void ParentCanNotBeThis()
        {
            var obj = new Tree();
            obj.Parent = obj;
            _ = obj.Property1;
        }

        [Fact]
        public void ClosestParentWin()
        {
            var t1 = new Tree { Property1 = "t1", Property2 = 1 };
            var t2 = new Tree { Parent = t1, Property1 = "t2" };
            var t3 = new Tree { Parent = t2, Property3 = 3 };

            Assert.Equal(1, t3.Property2);
            Assert.Equal("t2", t3.Property1);
            Assert.Equal(3, t3.Property3);
        }

        [Fact]
        public void WrongTSelf()
        {
            Assert.Throws<TypeMismatchException>(() => new WrongTSelf());
        }

        [Theory]
        [InlineData("test", 4, null, true, null)]
        [InlineData("hello", 2, 3, false, true)]
        [InlineData(null, -5, 7, true, false)]
        public void FullTree(string p1, int p2, int? p3, bool p4, bool? p5)
        {
            var root = new Tree
            {
                Property1 = p1,
                Property2 = p2,
                Property3 = p3,
                Property4 = p4,
                Property5 = p5
            };

            var t0 = new Tree0 { Parent = root };
            var t1 = new Tree1 { Parent = root };
            var t00 = new Tree00 { Parent = t0 };
            var t01 = new Tree01 { Parent = t0 };
            var t10 = new Tree10 { Parent = t1 };
            var t11 = new Tree11 { Parent = t1 };

            Assert.Equal(p1, t0.Property1);
            Assert.Equal(p2, t0.Property2);
            Assert.Equal(p3, t0.Property3);
            Assert.Equal(p4, t0.Property4);
            Assert.Equal(p5, t0.Property5);

            Assert.Equal(p1, t1.Property1);
            Assert.Equal(p2, t1.Property2);
            Assert.Equal(p3, t1.Property3);
            Assert.Equal(p4, t1.Property4);
            Assert.Equal(p5, t1.Property5);

            Assert.Equal(p1, t00.Property1);
            Assert.Equal(p2, t00.Property2);
            Assert.Equal(p3, t00.Property3);
            Assert.Equal(p4, t00.Property4);
            Assert.Equal(p5, t00.Property5);

            Assert.Equal(p1, t01.Property1);
            Assert.Equal(p2, t01.Property2);
            Assert.Equal(p3, t01.Property3);
            Assert.Equal(p4, t01.Property4);
            Assert.Equal(p5, t01.Property5);

            Assert.Equal(p1, t10.Property1);
            Assert.Equal(p2, t10.Property2);
            Assert.Equal(p3, t10.Property3);
            Assert.Equal(p4, t10.Property4);
            Assert.Equal(p5, t10.Property5);

            Assert.Equal(p1, t11.Property1);
            Assert.Equal(p2, t11.Property2);
            Assert.Equal(p3, t11.Property3);
            Assert.Equal(p4, t11.Property4);
            Assert.Equal(p5, t11.Property5);
        }
    }
}
