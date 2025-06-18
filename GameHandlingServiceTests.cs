using System;
using System.Collections.Generic;
using GameBuilderBot.Common;
using GameBuilderBot.Models;
using GameBuilderBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GameBuilderBotTests.Services
{
    public class GameHandlingServiceTests
    {
        [Fact]
        public void ApplyDefaultValuesToGameState_AddsMissingFields()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<Serializer>();
            var provider = services.BuildServiceProvider();

            var service = new GameHandlingService(provider);

            var source = new Dictionary<string, Field>
            {
                { "A", new Field("1", "1") },
                { "B", new Field("2", "2") }
            };
            var destination = new Dictionary<string, Field>
            {
                { "A", new Field("1", "1") }
            };

            // Use reflection to access the private method
            var method = typeof(GameHandlingService).GetMethod("ApplyDefaultValuesToGameState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            method.Invoke(service, new object[] { source, destination });

            // Assert
            Assert.True(destination.ContainsKey("A"));
            Assert.True(destination.ContainsKey("B"));
            Assert.Equal("2", destination["B"].Value.ToString());
        }
    }
}
