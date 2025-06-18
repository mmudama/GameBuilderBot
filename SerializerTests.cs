using System;
using System.Collections.Generic;
using GameBuilderBot.Common;
using Xunit;

namespace GameBuilderBotTests.Common
{
    public class SerializerTests
    {
        private readonly Serializer _serializer = new();

        [Fact]
        public void SerializeToString_And_DeserializeFromString_JSON_RoundTrip()
        {
            var obj = new TestClass { Id = 42, Name = "Test" };
            string json = _serializer.SerializeToString(obj, FileType.JSON);

            var result = _serializer.DeserializeFromString<TestClass>(json, FileType.JSON);

            Assert.Equal(obj.Id, result.Id);
            Assert.Equal(obj.Name, result.Name);
        }

        [Fact]
        public void TryDeepCopy_ReturnsDeepCopy()
        {
            var original = new TestClass { Id = 1, Name = "Original" };
            var copy = _serializer.TryDeepCopy(original);

            Assert.NotSame(original, copy);
            Assert.Equal(original.Id, copy.Id);
            Assert.Equal(original.Name, copy.Name);
        }

        private class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
