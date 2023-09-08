using System.Configuration;

namespace Calculator.Logger
{
    internal class FileLogger : ILogger
    {
        private readonly string _path;

        public FileLogger()
        {
            _path = ConfigurationManager.AppSettings.Get("history")!;
        }

        public async Task Record(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            using (var writer = new StreamWriter(_path, true))
            {
                await writer.WriteLineAsync(message);
            }
        }

        public async Task ErrorRecord(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            using (var writer = new StreamWriter(_path, true))
            {
                await writer.WriteLineAsync($"Error: {message}");
            }
        }

    }
}