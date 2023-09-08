namespace Calculator.Logger
{
    internal interface ILogger
    {
        Task Record(string message);
        Task ErrorRecord(string message);
    }
}
