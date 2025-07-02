using System.Collections.Generic;
using GameBuilderBot.Models;
using GameBuilderBot.Runners;
using Xunit;

namespace GameBuilderBotTests.Runners
{
    // Minimal concrete implementation for testing
    public class TestAssignmentRunner : AssignmentRunner
    {
        public TestAssignmentRunner() : base(null, null) { }
        public override object CalculateValue(GameState state, string fieldName, string expression) => null;
        public override string OneLinerHelp() => null;

        // Expose PopulateField for testing
        public bool TestPopulateField(GameState state, string fieldName, in object newValue, string expression, out object previousValue)
            => PopulateField(state, fieldName, in newValue, expression, out previousValue);
    }

    public class AssignmentRunnerTests
    {
        [Fact]
        public void PopulateField_FieldDoesNotExist_AddsFieldAndReturnsFalse()
        {
            // Arrange
            var runner = new TestAssignmentRunner();
            var state = new GameState
            {
                Fields = new Dictionary<string, Field>()
            };
            string fieldName = "score";
            object newValue = 42;
            string expression = "42";

            // Act
            var result = runner.TestPopulateField(state, fieldName, in newValue, expression, out var previousValue);

            // Assert
            Assert.False(result);
            Assert.Null(previousValue);
            Assert.True(state.Fields.ContainsKey(fieldName));
            Assert.Equal("42", state.Fields[fieldName].Value.ToString());
            Assert.Equal(expression, state.Fields[fieldName].Expression);
        }

        [Fact]
        public void PopulateField_FieldExists_UpdatesValueAndReturnsTrue()
        {
            // Arrange
            var runner = new TestAssignmentRunner();
            var state = new GameState
            {
                Fields = new Dictionary<string, Field>
                {
                    { "score", new Field("oldExpr", 10) }
                }
            };
            string fieldName = "score";
            object newValue = 99;
            string expression = "99";

            // Act
            var result = runner.TestPopulateField(state, fieldName, in newValue, expression, out var previousValue);

            // Assert
            Assert.True(result);
            Assert.Equal(10, previousValue);
            Assert.Equal(99, state.Fields[fieldName].Value);
            Assert.Equal(expression, state.Fields[fieldName].Expression);
        }

        [Fact]
        public void PopulateField_ReturnsFalseIfPreviousValueIsNull()
        {
            // Arrange
            var runner = new TestAssignmentRunner();
            var state = new GameState
            {
                Fields = new Dictionary<string, Field>
                {
                    { "test", new Field(null, 24) }
                }

            };
            string fieldName = "test";
            string expression = "42";
            object newValue = 42;
            // Act
            var result = runner.TestPopulateField(state, fieldName, in newValue, expression, out var previousValue);
            // Assert
            Assert.False(result);
            Assert.Null(previousValue);
        }

        [Fact]
        public void PopulateField_ReturnsTrueIfPreviousValueExists()
        {
            // Arrange
            var runner = new TestAssignmentRunner();
            var state = new GameState
            {
                Fields = new Dictionary<string, Field>
                {{ "test", new Field("I was ", 24) } }

            };
            string fieldName = "test";
            string expression = "I am ";
            object newValue = 42;
            // Act
            var result = runner.TestPopulateField(state, fieldName, in newValue, expression, out var previousValue);
            // Assert
            Assert.True(result);
            Assert.NotNull(previousValue);
        }
    }
}
