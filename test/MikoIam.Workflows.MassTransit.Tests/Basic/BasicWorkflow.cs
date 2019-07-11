using System.Text;
using MikoIam.Workflows.Engine;
using Xunit.Sdk;

namespace MikoIam.Workflows.MassTransit.Tests.Basic
{
    public class BasicWorkflow : Workflow
    {
        private readonly StringBuilder _testResults;

        public BasicWorkflow(StringBuilder testResults)
        {
            _testResults = testResults;
        }

        public WorkflowTask A => new WorkflowTask("A", () => _testResults.Append("A"));

        public void Setup()
        {
            StartOn<StartWorkflowMessage>();
            Initially().Do(A);
            After(A).Finish();
        }
    }
}