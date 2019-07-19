namespace MikoIam.Workflows.MassTransit.Tests.MultipleRuns
{
    public class CompleteTaskBMessage
    {
        public string Correlation { get; }

        public CompleteTaskBMessage(string correlation)
        {
            Correlation = correlation;
        }
    }
}