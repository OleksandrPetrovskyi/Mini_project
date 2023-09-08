using Calculator.Solution_methods.Check;
using System.Text.RegularExpressions;

namespace Calculator.Solution_methods
{
    public class StandardSolution : ISolution
    {
        protected ICheck _check;
        protected readonly char[] mathSymbols = new char[] { '+', '-', '*', '/' };

        private const string regexNumbers = "digits";
        private readonly Regex digitSearchRule = new Regex(@$"(?<{regexNumbers}>\d*[.,]*\d*)");

        private int _decimalNumber;
        public int DecimalNumber
        {
            get => _decimalNumber;
            set
            {
                if (value > 0 && value < 15)
                {
                    _decimalNumber = value;
                }
            }
        }
        public StandardSolution(ICheck check)
        {
            _check = check;
        }
        
        public async Task<SolutionResponse> ResultAsync(string input)
        {
            const char _NUMBER_SEPARATOR = '.';
            const string divideByZeroError = "Cannot divide by zero";
            const int skipOpenBracket = 1;
            const int addCloseBracket = 1;
            var numbers = new List<double>();

            var mathExpression = input.Replace(" ", "").Replace(',',_NUMBER_SEPARATOR).Replace('.', _NUMBER_SEPARATOR).ToLower();

            var resultByValidation = await _check.Check(mathExpression);
            if (resultByValidation.Success == false)
                return new SolutionResponse(resultByValidation.Success, input, resultByValidation.ErrorMessage);
            
            mathExpression = RemovingExtraBrackets(mathExpression);

            for (var index = mathExpression.LastIndexOf('('); index >= 0; index = mathExpression.LastIndexOf('('))
            {
                if (mathExpression[index] == '(')
                {
                    var indexCloseBracket = mathExpression.IndexOf(')', index) - index;
                    var expression = mathExpression.Substring(index + skipOpenBracket, indexCloseBracket);

                    FindingNumbersInExpression(ref expression, index + skipOpenBracket, numbers);
                    double? resultInBrakes = await Calculation(expression, 0, numbers);
                    numbers.Clear();

                    if (resultInBrakes == null)
                        return new SolutionResponse(false, input, divideByZeroError);

                    var bracketLength = mathExpression.IndexOf(')', index) - index + addCloseBracket;
                    mathExpression = ReplaceCharacters(mathExpression, index, bracketLength, resultInBrakes.ToString()!);
                }
            }

            FindingNumbersInExpression(ref mathExpression, 0, numbers);
            var result = await Calculation(mathExpression, 0, numbers);
            numbers.Clear();

            return result switch
            {
                null => new SolutionResponse(false, input, divideByZeroError),
                _ => new SolutionResponse(true, input, Math.Round(result.Value, DecimalNumber).ToString())
            };
        }

        virtual protected void FindingNumbersInExpression(ref string mathExpression, int index, List<double> numbers)
        {
            var foundNumbersInString = digitSearchRule.Matches(mathExpression);
            var numberOfCharsRemoved = 0;

            foreach (Match res in foundNumbersInString)
            {
                GroupCollection groups = res.Groups;

                int numberIndex;
                if (groups[regexNumbers].Length > 1 && groups[regexNumbers].Value[0] == '-')
                {
                    if (groups[regexNumbers].Index == 0
                        || mathSymbols.Contains(mathExpression[groups[regexNumbers].Index - 1 - numberOfCharsRemoved]))
                    {
                        numbers.Add(double.Parse(groups[regexNumbers].Value));

                        numberIndex = groups[regexNumbers].Index - numberOfCharsRemoved;
                        mathExpression = mathExpression.Remove(numberIndex, 1);
                        numberOfCharsRemoved++;
                    }

                    else if (char.IsDigit(mathExpression[groups[regexNumbers].Index - 1 - numberOfCharsRemoved]))
                    {
                        var tempValue = double.Parse(groups[regexNumbers].Value) * -1;
                        numbers.Add(tempValue);
                    }
                }

                else if (groups[regexNumbers].Length == 1 && groups[regexNumbers].Value[0] == '-')
                {
                    numbers.Add(-1);

                    numberIndex = groups[regexNumbers].Index - numberOfCharsRemoved;
                    mathExpression = ReplaceCharacters(mathExpression, numberIndex, 1, "1*");
                    numberOfCharsRemoved--;
                }

                else if (groups[regexNumbers].Length > 0 && groups[regexNumbers].Value[0] != '-')
                {
                    numbers.Add(double.Parse(groups[regexNumbers].Value));
                }
            }
        }
        virtual protected async Task<double?> Calculation(string mathExpression, int startIndex, List<double> numbers)
        {
            if (await _check.IsDivisionByZero(mathExpression) == true)
                return null;

            double outputResult = numbers[0];
            var secondNumberIndex = 1;

            for (int index = startIndex; index < mathExpression.Length - 1; index++)
            {
                if (mathExpression[index] == ')' || numbers.Count == 1)
                {
                    return outputResult;
                }

                if (index == startIndex)
                {
                    continue;
                }
                switch (mathExpression[index])
                {
                    case '+':
                        outputResult += numbers[secondNumberIndex];
                        secondNumberIndex++;
                        break;
                    case '-':
                        outputResult -= numbers[secondNumberIndex];
                        secondNumberIndex++;
                        break;
                    case '*':
                        outputResult *= numbers[secondNumberIndex];
                        secondNumberIndex++;
                        break;
                    case '/':
                        outputResult /= numbers[secondNumberIndex];
                        secondNumberIndex++;
                        break;
                }
            }
            return outputResult;
        }
        protected string ReplaceCharacters(string expression, int index, int amtChars, string replacement)
        {
            expression = expression.Remove(index, amtChars);
            return expression.Insert(index, replacement);
        }
        protected string RemovingExtraBrackets(string mathExpression)
        {
            if (!mathExpression.Contains('('))
            {
                return mathExpression;
            }

            var mathematicalExpressionValidation = new Regex(@"[()]*(?<function>(?<![sna]|\d)-?(cos|sin|tan|abc)\()*(?<numbers>-?\d*([.,]\d+)?)(?<symbols>(?<=[\d)])[*/+=](?![)+*/]))*");
            var foundMathSymbolsInString = mathematicalExpressionValidation.Matches(mathExpression);

            foreach (Match res in foundMathSymbolsInString)
            {
                GroupCollection groups = res.Groups;

                if (groups[0].Length > 0 && groups[0].Value[0] == '('
                    && mathExpression[groups["numbers"].Index + groups["numbers"].Length] == ')')
                {
                    int twoBrackets = 2;
                    int indexOpenBracket = groups["numbers"].Index - 1;
                    mathExpression = ReplaceCharacters(mathExpression, indexOpenBracket, groups["numbers"].Length + twoBrackets, groups["numbers"].Value);
                }
            }
            return mathExpression;
        }
    }
}