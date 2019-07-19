using System;
using System.Threading.Tasks;
using MassTransit;
using Xunit;

namespace MikoIam.Workflows.MassTransit.Tests.MultipleRuns
{
    public class MultipleRunsWorkflowTests
    {
        private readonly IBusControl _bus;
        private readonly SingleWorkflowObserver<MultipleRunsWorkflowContext> _observerX;
        private readonly SingleWorkflowObserver<MultipleRunsWorkflowContext> _observerY;

        public MultipleRunsWorkflowTests()
        {
            var workflow = new MultipleRunsWorkflow();
            _observerX = new SingleWorkflowObserver<MultipleRunsWorkflowContext>(workflow,
                ctx => ctx.SomeProp == "X"); 
            _observerY = new SingleWorkflowObserver<MultipleRunsWorkflowContext>(workflow,
                ctx => ctx.SomeProp == "Y");
            
            _bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("multiple_runs_workflow", ep => { ep.Workflow(workflow); });
            });
        }

        [Fact]
        public async Task ShouldExecuteWorkflowRunsIndependently()
        {
            // Act
            _bus.Start();

            await _bus.Publish(new StartWorkflowMessage("X"));
            await _bus.Publish(new StartWorkflowMessage("Y"));

            var taskAXStarted = _observerX.TaskStartedHandle.WaitOne(TimeSpan.FromSeconds(5));
            await _bus.Publish(new CompleteTaskAMessage("X"));
            var taskAYStarted = _observerY.TaskStartedHandle.WaitOne(TimeSpan.FromSeconds(5));
            await _bus.Publish(new CompleteTaskAMessage("Y"));
            
            var taskBXStarted = _observerX.TaskStartedHandle.WaitOne(TimeSpan.FromSeconds(5));
            await _bus.Publish(new CompleteTaskBMessage("X"));
            var taskBYStarted = _observerY.TaskStartedHandle.WaitOne(TimeSpan.FromSeconds(5));
            await _bus.Publish(new CompleteTaskBMessage("Y"));
            
            var workflowXFinished = _observerX.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));
            var workflowYFinished = _observerY.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            _bus.Stop();
            
            Assert.True(workflowXFinished);
            Assert.True(workflowYFinished);
            Assert.Equal("@WF-@A-$A-@B-$B-$WF", _observerX.EventSequence);
            Assert.Equal("@WF-@A-$A-@B-$B-$WF", _observerY.EventSequence);
        }
    }
}