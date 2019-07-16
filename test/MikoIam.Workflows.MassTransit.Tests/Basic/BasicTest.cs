using System;
using System.Text;
using System.Threading;
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
            var workflow = new BasicWorkflow();
            var observer = new SingleWorkflowObserver(workflow);

            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("basic_workflow", ep => { ep.Workflow(workflow); });
            });

            // Act
            bus.Start();

            await bus.Publish(new StartWorkflowMessage());
            observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));
            
            bus.Stop();

            // Assert
            Assert.Equal("AB", observer.TaskOrder);
        }
    }
}