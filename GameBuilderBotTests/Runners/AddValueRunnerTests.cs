using System.Collections.Generic;
using GameBuilderBot.Models;
using GameBuilderBot.Runners;
using Xunit;

namespace GameBuilderBotTests.Runners
{
    public class AddValueRunnerTests
    {
        private AddValueRunner CreateRunner() => new AddValueRunner(null, null);

        [Fact]
        public void CalculateValue_FieldDoesNotExist_AddsValueToZero()
        {
            // Arrange
            var runner = CreateRunner();
            var state = new GameState { Fields = new Dictionary<string, Field>() };
            string fieldName = "score";
            string expression = "5";

            // Act
            var result = runner.CalculateValue(state, fieldName, expression);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void CalculateValue_FieldExists_AddsToExistingValue()
        {
            // Arrange
            var runner = CreateRunner();
            var state = new GameState
            {
                Fields = new Dictionary<string, Field>
                {
                    { "score", new Field(null, 10) }
                }
            };
            string fieldName = "score";
            string expression = "7";

            // Act
            var result = runner.CalculateValue(state, fieldName, expression);

            // Assert
            Assert.Equal(17, result);
        }

        [Fact]
        public void CalculateValue_ExpressionIsMath_AddsEvaluatedResult()
        {
            // Arrange
            var runner = CreateRunner();
            var state = new GameState
            {
                Fields = new Dictionary<string, Field>
                {
                    { "score", new Field(null, 3) }
                }
            };
            string fieldName = "score";
            string expression = "2+5";

            // Act
            var result = runner.CalculateValue(state, fieldName, expression);

            // Assert
            Assert.Equal(10, result);
        }

        [Fact]
        public void CalculateValue_FieldValueIsString_ParsesAndAdds()
        {
            // Arrange
            var runner = CreateRunner();
            var state = new GameState
            {
                Fields = new Dictionary<string, Field>
                {
                    { "score", new Field(null, "8") }
                }
            };
            string fieldName = "score";
            string expression = "4";

            // Act
            var result = runner.CalculateValue(state, fieldName, expression);

            // Assert
            Assert.Equal(12, result);
        }

        [Fact]
        public void CalculateValue_RecognizesDiceRoll()
        {
            // Arrange
            var runner = CreateRunner();
            int originalValue = 5;

            var state = new GameState
            {
                Fields = new Dictionary<string, Field>
                {
                    { "score", new Field(null, originalValue) }
                }
            };
            string fieldName = "score";
            string expression = "2d6"; // Example dice roll
            // Act
            var result = runner.CalculateValue(state, fieldName, expression);
            // Assert
            Assert.IsType<int>(result); // Assuming the dice roll evaluates to an integer
            Assert.InRange((int)result, originalValue + 2, originalValue + 12); // 2d6 can roll between 2 and 12 
        }
    }
}