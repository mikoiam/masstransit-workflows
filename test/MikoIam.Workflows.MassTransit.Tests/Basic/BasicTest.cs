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
            var workflow = new BasicWorkflow(() => { aCalled = true;}, () => { bCalled = true;});
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
            Assert.True(aCalled);
            Assert.True(bCalled);
        }
    }
}