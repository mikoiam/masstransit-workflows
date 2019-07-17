using System;
using System.Threading.Tasks;
using MassTransit;
using Xunit;

namespace MikoIam.Workflows.MassTransit.Tests.Basic
{
    public class BasicTest
    {
        [Fact]
        public async Task ShouldExecuteWorkflow()
        {
            // Arrange
            var aCalled = false;
            var bCalled = false;
            var workflow = new BasicWorkflow(() => { aCalled = true; }, () => { bCalled = true; });
            var observer = new SingleWorkflowObserver(workflow);

            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("basic_workflow", ep => { ep.Workflow(workflow); });
            });

            // Act
            bus.Start();

            await bus.Publish(new StartWorkflowMessage());
            var workflowFinished = observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            bus.Stop();

            // Assert
            Assert.True(workflowFinished);
            Assert.Equal("AB", observer.TaskOrder);
            Assert.True(aCalled);
            Assert.True(bCalled);
        }

        [Fact]
        public async Task ShouldReportWorkflowEventsInCorrectOrder()
        {
            // Arrange
            var workflow = new BasicWorkflow(() => { }, () => { });
            var observer = new SingleWorkflowObserver(workflow);

            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("basic_workflow", ep => { ep.Workflow(workflow); });
            });

            // Act
            bus.Start();

            await bus.Publish(new StartWorkflowMessage());
            var workflowFinished = observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            bus.Stop();

            // Assert
            Assert.True(workflowFinished);
            Assert.Equal("@WF@A$A@B$B$WF", observer.EventOrder);
        }
        
        [Fact]
        public async Task ShouldNotExecuteWorkflowOnNonRegisteredMessage()
        {
            // Arrange
            var workflow = new BasicWorkflow(() => { }, () => { });
            var observer = new SingleWorkflowObserver(workflow);

            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("basic_workflow", ep => { ep.Workflow(workflow); });
            });

            // Act
            bus.Start();

            await bus.Publish(new DoNotStartWorkflowMessage());
            var workflowFinished = observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            bus.Stop();

            // Assert
            Assert.False(workflowFinished);
            Assert.Equal("", observer.EventOrder);
        }
    }
}