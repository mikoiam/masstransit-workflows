using System;
using System.Threading.Tasks;
using MassTransit;
using Xunit;

namespace MikoIam.Workflows.MassTransit.Tests.Empty
{
    public class EmptyWorkflowTests
    {
        [Fact]
        public async Task ShouldExecuteWorkflow()
        {
            // Arrange
            var workflow = new EmptyWorkflow();
            var observer = new SingleWorkflowObserver(workflow);

            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("empty_workflow", ep => { ep.Workflow(workflow); });
            });

            // Act
            bus.Start();

            await bus.Publish(new StartWorkflowMessage());
            var workflowFinished = observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            bus.Stop();

            // Assert
            Assert.True(workflowFinished);
        }
        
        [Fact]
        public async Task ShouldNotExecuteWorkflowOnNonRegisteredMessage()
        {
            // Arrange
            var workflow = new EmptyWorkflow();
            var observer = new SingleWorkflowObserver(workflow);

            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("empty_workflow", ep => { ep.Workflow(workflow); });
            });

            // Act
            bus.Start();

            await bus.Publish(new DoNotStartWorkflowMessage());
            var workflowFinished = observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            bus.Stop();

            // Assert
            Assert.False(workflowFinished);
            Assert.Equal("", observer.EventSequence);
        }
    }
}