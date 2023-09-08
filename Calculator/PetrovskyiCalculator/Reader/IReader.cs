namespace Calculator.Reader
{
    internal interface IReader
    {
        Task<IEnumerable<string>?> Read(string path, int lineNumber, int count);
    }
}
