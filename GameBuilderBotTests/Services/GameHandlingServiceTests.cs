using System.Collections.Generic;
using GameBuilderBot.Common;
using GameBuilderBot.Models;
using GameBuilderBot.Services;
using Xunit;
using System.Reflection;

namespace GameBuilderBotTests.Services
{
    public class GameHandlingServiceTests
    {
        [Fact]
        public void ApplyDefaultValuesToGameState_AddsMissingFields()
        {
            var service = new GameHandlingService(new Serializer());

            var source = new Dictionary<string, Field>
            {
                { "A", new Field("exprA", "1") },
                { "B", new Field("exprB", "2") }
            };
            var destination = new Dictionary<string, Field>
            {
                { "A", new Field("exprA", "1") }
            };

            MethodInfo method = typeof(GameHandlingService).GetMethod("ApplyDefaultValuesToGameState", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(service, [source, destination]);

            Assert.True(destination.ContainsKey("A"));
            Assert.True(destination.ContainsKey("B"));
            Assert.Equal("2", destination["B"].Value.ToString());
            Assert.NotSame(source["B"], destination["B"]); // Ensure a new instance was created
        }
    }
}
