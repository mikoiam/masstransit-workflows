using System;
using System.Text;
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
            var result = new StringBuilder();
            var workflow = new BasicWorkflow(result);

            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("basic_workflow", ep => { ep.Workflow(workflow); });
            });

            // Act
            bus.Start();

            await bus.Publish(new StartWorkflowMessage());

            await Task.Delay(3000);
            bus.Stop();

            // Assert
            Assert.Equal("A", result.ToString());
        }
    }
}