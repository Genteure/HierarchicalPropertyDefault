using System;
using HierarchicalPropertyDefault.Internal;
using UnitTest.Models;
using Xunit;

namespace UnitTest.Tests
{
    public class MappingInfoTest
    {
        [Fact]
        public void Test1()
        {
            const string? testvalue = "testvalue";
            var mi = new MappingInfo<Basic2, string>(typeof(Basic1).GetProperty(nameof(Basic1.Property1))!,
                new[] {
                    typeof(Basic2).GetProperty(nameof(Basic2.Property2))!
                }, Array.Empty<string>());
            var obj = new Basic2 { Property2 = testvalue };
            var result = mi.GetValue(obj);
            Assert.Equal(testvalue, result);
        }

        [Fact]
        public void Test2()
        {
            var data = new Basic1();
            var mi = new MappingInfo<Basic2, Basic1>(typeof(Basic1).GetProperty(nameof(Basic1.Object))!,
                new[] {
                    typeof(Basic2).GetProperty(nameof(Basic2.Object))!
                }, Array.Empty<string>());

            var obj = new Basic2 { Object = data };
            var result = mi.GetValue(obj);
            Assert.Same(data, result);
        }

        [Fact]
        public void Test3()
        {
            var data = new Basic1() { Property1 = "hello world" };
            var mi = new MappingInfo<Basic2, string>(typeof(Basic1).GetProperty(nameof(Basic1.Object))!, new[] {
                typeof(Basic1).GetProperty(nameof(Basic1.Property1))!,
                typeof(Basic1).GetProperty(nameof(Basic1.Object))!,
                typeof(Basic1).GetProperty(nameof(Basic1.Object))!,
                typeof(Basic2).GetProperty(nameof(Basic2.Object))!,
            }, Array.Empty<string>());

            var obj = new Basic2
            {
                Object = new Basic1
                {
                    Object = new Basic1
                    {
                        Object = data
                    }
                }
            };
            var result = mi.GetValue(obj);
            Assert.Equal(data.Property1, result);

            obj.Object.Object = null;
            result = mi.GetValue(obj);
            Assert.Equal(default, result);
        }


        [Fact]
        public void Test4()
        {
            var data = new Basic1() { Property1 = "hello world" };
            var mi = new MappingInfo<Basic2, int>(typeof(Basic1).GetProperty(nameof(Basic1.Object))!, new[] {
                typeof(string).GetProperty(nameof(string.Length))!,
                typeof(Basic1).GetProperty(nameof(Basic1.Property1))!,
                typeof(Basic1).GetProperty(nameof(Basic1.Object))!,
                typeof(Basic1).GetProperty(nameof(Basic1.Object))!,
                typeof(Basic2).GetProperty(nameof(Basic2.Object))!,
            }, Array.Empty<string>());

            var obj = new Basic2
            {
                Object = new Basic1
                {
                    Object = new Basic1
                    {
                        Object = data
                    }
                }
            };
            var result = mi.GetValue(obj);
            Assert.Equal(data.Property1.Length, result);

            obj.Object.Object = null;
            result = mi.GetValue(obj);
            Assert.Equal(default, result);
        }

        [Fact]
        public void TestThrow()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var mi = new MappingInfo<Basic1, string>(typeof(Basic1).GetProperty(nameof(Basic1.Property1))!,
                    new[] {
                        typeof(Basic2).GetProperty(nameof(Basic2.Property2))!
                    }, Array.Empty<string>());
            });
        }
    }
}
