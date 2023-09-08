using Calculator.Logger;
using Calculator.Reader;
using Calculator.Solution_methods;
using Calculator.UserInterface;

namespace Calculator.Types_of_calculators
{
    internal class ConsoleCalculator : ICalculator
    {
        protected const char _ENTER = (char)13;
        protected const char _BACKSPACE = (char)8;
        protected const char _ESCAPE = (char)27;
        protected const char _SPACE = (char)32;
        protected const char _NUMBER_SEPARATOR = '.';

        protected char _inputChar = ' ';
        protected string _mathExpression = string.Empty;

        protected ISolution _solution;
        protected IUserInterface _userInterface;
        protected ILogger _logger;
        protected IReader _reader;
        
        protected delegate Task Logging(string message);
        protected event Logging LoggerEvent;
        protected event Logging ErrorLoggerEvent;

        public ConsoleCalculator(ISolution solution, IUserInterface userInterface, ILogger logger, IReader reader)
        {
            _solution = solution;
            _userInterface = userInterface;
            _logger = logger;
            _reader = reader;

            LoggerEvent += _userInterface.Message;
            LoggerEvent += _logger.Record;

            ErrorLoggerEvent += _logger.ErrorRecord;
            ErrorLoggerEvent += _userInterface.ImportantMessage;
        }
       
        public async Task StartCalculator()
        {
            await _userInterface.ImportantMessage(@$"Greetings User!
You can used: 
1) Digitals - 0-9
2) Operators -   + - * / ( ) . , =
4) ESCAPE - Exit Calculator.
5) ENTER - Save the current expression and start a new expression on a new line.
6) BACKSPACE - Remove last element.

Beginning of work
Specify the number of decimal places (from 0 to 9):    ");

            int.TryParse(_userInterface.GetChar.ToString(), out int decimalNumber);
            _solution.DecimalNumber = decimalNumber;
            await _userInterface.Message(Environment.NewLine);

            var CalculationFromFile = CalculationsFromFile("Formulas for testing.txt");

            do
            {
                _inputChar = _userInterface.GetChar;

                if (_inputChar != '\0')
                {
                    _mathExpression = _inputChar switch
                    {
                        _ESCAPE => History(),
                        _ENTER => History(),
                        _BACKSPACE => Backspace(_mathExpression),
                        '=' => await Calculation(_mathExpression),
                        _ => AddSymbolToMathExpression(_inputChar, _mathExpression)
                    };
                }
            }
            while (_inputChar != _ESCAPE);

            Task.WaitAny(CalculationFromFile);
        }

        public async Task CalculationsFromFile(string path)
        {
            var lineNumber = 0;
            var numberOfLines = 50;
            var message = string.Empty;
            int numberOfAssignedTasks;
            
            var tasks = new Task<SolutionResponse>[10];
            List<string>? Formulas;
            
            do
            {
                Formulas = (await _reader.Read(path, lineNumber, numberOfLines))!.ToList();
                lineNumber += numberOfLines;

                if (Formulas is null || Formulas.Count < 1)
                    continue;

                for (int j = 0; j < Formulas.Count;)
                {
                    numberOfAssignedTasks = 0;

                    for (int i = 0; i < tasks.Length && j < Formulas.Count; i++)
                    {
                        tasks[i] = _solution.ResultAsync(Formulas[j++]);
                        numberOfAssignedTasks++;
                    }

                    for (int i = 0; i < tasks.Length && i < numberOfAssignedTasks; i++)
                    {
                        var result = await tasks[i];
                        if(result.Success == true)
                            message += ($"{result.Request}{result.Response}{Environment.NewLine}");
                        else
                            message += ($"Error: {result.Request}{result.Response}{Environment.NewLine}");
                    }
                }

                _ = _logger.Record(message);
                message = string.Empty;
            }
            while (Formulas != null && Formulas.Count == numberOfLines);
        }

        private async Task<string> Calculation(string mathExpression)
        {
            var answer = await _solution.ResultAsync($"{mathExpression}=");
            var result = $"{answer.Request}{answer.Response}";

            if (answer.Success == true
                && LoggerEvent != null && result.Length != 0)
            {
                await LoggerEvent(result);
            }
            else if (answer.Success == false
                && ErrorLoggerEvent != null && result.Length != 0)
            {
                await ErrorLoggerEvent(result);
            }

            return result;
        }
        private string AddSymbolToMathExpression(char symbol, string mathExpression)
        {
            if (symbol == ',' || symbol == '.')
                mathExpression += _NUMBER_SEPARATOR;
            else
                mathExpression += symbol;

            return mathExpression;
        }
        private string Backspace(string mathExpression)
        {
            if (mathExpression.Length > 0)
            {
                int index;

                if (mathExpression.Contains('='))
                    index = mathExpression.IndexOf('=');
                else
                    index = mathExpression.Length - 1;
                
                var count = mathExpression.Length - index;
                var deleteMessage = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    deleteMessage += ' ';
                }

                _userInterface.DeleteCharacters(index, deleteMessage);
                mathExpression = mathExpression.Remove(index);
            }

            return mathExpression;
        }
        private string History()
        {
            _userInterface.Message(Environment.NewLine);
            return string.Empty;
        }
    }
}