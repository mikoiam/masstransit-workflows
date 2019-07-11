using System;
using Xunit;
using MassTransit;
using System.Threading.Tasks;

namespace MikoIam.Workflows.MassTransit.Tests.Basic
{
    public class BasicTest
    {
        [Fact]
        public async Task Test()
        {
            // Arrange
            var result = string.Empty;

            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
                    {
                        sbc.ReceiveEndpoint("test_queue", ep =>
                        {
                            ep.Handler<BasicMessage>((context) =>
                            {
                                result = $"Received: {context.Message.Text}";
                                return Task.CompletedTask;
                            });
                        });
                    });

            // Act
            bus.Start();

            await bus.Publish(new BasicMessage { Text = "Hi" });

            await Task.Delay(3000);
            bus.Stop();

            // Assert
            Assert.Equal("Received: Hi", result);
        }
    }
}