namespace Calculator.Reader
{
    internal class FileReader : IReader
    {
        public async Task<IEnumerable<string>?> Read(string path, int lineNumber, int count)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (File.ReadLines(path).Count() - lineNumber < count)
                        count = File.ReadLines(path).Count() - lineNumber;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            });

            return File.ReadLines(path).Skip(lineNumber).Take(count);
        }
    }
}
