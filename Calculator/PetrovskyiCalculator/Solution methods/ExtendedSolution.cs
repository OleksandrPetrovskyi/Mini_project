using Calculator.Solution_methods.Check;
using System.Text.RegularExpressions;

namespace Calculator.Solution_methods
{
    public class ExtendedSolution : StandardSolution
    {
        private const string regexFunction = "function";
        private const string regexNumbers = "digits";
        private const string regexSymbols = "symbols";
        private readonly Regex digitSearchRule = new Regex(@$"[()]*(?<{regexFunction}>(?<![sna]|\d)-?(cos|sin|tan|abc))*(?<{regexNumbers}>-?\d*([.,]\d*)?)(?<{regexSymbols}>[*/+=]*)");

        public ExtendedSolution(ICheck check) : base(check)
        {
            _check = check;
        }

        protected override void FindingNumbersInExpression(ref string mathExpression, int index, List<double> numbers)
        {
            var foundNumbersInString = digitSearchRule.Matches(mathExpression);

            var numberOfCharsRemoved = 0;
            foreach (Match res in foundNumbersInString)
            {
                GroupCollection groups = res.Groups;

                int numberIndex;

                if (groups[regexFunction].Length > 0)
                {
                    if (groups[regexFunction].Value[0] == '-'
                        && (groups[regexFunction].Index == 0
                           || mathSymbols.Contains(mathExpression[groups[regexFunction].Index - 1 - numberOfCharsRemoved])))
                    {
                        numbers.Add(-1);

                        numberIndex = groups[regexFunction].Index - numberOfCharsRemoved;
                        mathExpression = ReplaceCharacters(mathExpression, numberIndex, 1, "1*");
                        numberOfCharsRemoved--;

                        var number = double.Parse(groups[regexNumbers].Value);

                        if (number < 0)
                        {
                            numbers.Add(number);

                            numberIndex = groups[regexNumbers].Index - numberOfCharsRemoved;
                            mathExpression = mathExpression.Remove(numberIndex, 1);
                            numberOfCharsRemoved++;
                        }
                        else
                        {
                            numbers.Add(number);
                        }
                    }
                    else
                    {
                        if (double.Parse(groups[regexNumbers].Value) < 0)
                        {
                            var tempValue = double.Parse(groups[regexNumbers].Value) * -1;
                            numbers.Add(tempValue);

                            numberIndex = groups[regexNumbers].Index - numberOfCharsRemoved;
                            mathExpression = mathExpression.Remove(numberIndex, 1);
                            numberOfCharsRemoved++;
                        }
                        else
                        {
                            numbers.Add(double.Parse(groups[regexNumbers].Value));
                        }
                    }
                }

                else if (groups[regexNumbers].Length > 1 && groups[regexNumbers].Value[0] == '-')
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

                else if (groups[regexNumbers].Length == 1 && groups[regexNumbers].Value[0] == '-'
                    && groups[regexNumbers].Index - 1 >= 0 && mathSymbols.Contains(mathExpression[groups[regexNumbers].Index - 1]))
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
        protected override async Task<double?> Calculation(string mathExpression, int startIndex, List<double> numbers)
        {
            if (await _check.IsDivisionByZero(mathExpression) == true)
            {
                return null;
            }

            PrioritySetting(ref mathExpression, startIndex, numbers);
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

                else if (mathExpression[index] == '+')
                {
                    outputResult = outputResult + numbers[secondNumberIndex];
                    secondNumberIndex++;
                }

                else if (mathExpression[index] == '-')
                {
                    outputResult = outputResult - numbers[secondNumberIndex];
                    secondNumberIndex++;
                }
            }

            return outputResult;
        }
        protected void PrioritySetting(ref string mathExpression, int startIndex, List<double> numbers)
        {
            const int ANY_OPERATOR = 1;
            int indexOfNumber = 0;
            var previousOperatorIndex = startIndex;

            for (int index = startIndex;
                index < mathExpression.Length && mathExpression[index] != ')' && mathExpression[index] != '=';
                index++)
            {
                if (char.IsDigit(mathExpression, index))
                    continue;

                mathExpression = FunctionEvaluation(mathExpression, index, indexOfNumber, numbers);

                if (mathExpression[index] == '*')
                {
                    mathExpression = FunctionEvaluation(mathExpression, index + ANY_OPERATOR, indexOfNumber + 1, numbers);

                    var secondNumberLength = numbers[indexOfNumber + 1].ToString().Length;
                    int dell;
                    if (previousOperatorIndex == 0)
                        dell = mathExpression.Substring(previousOperatorIndex, index + 1 + secondNumberLength).Length;
                    else
                        dell = mathExpression.Substring(previousOperatorIndex + 1, index - previousOperatorIndex + secondNumberLength).Length;

                    numbers[indexOfNumber] *= numbers[indexOfNumber + 1];
                    numbers.RemoveAt(indexOfNumber + 1);

                    var insert = numbers[indexOfNumber].ToString();
                    if(numbers[indexOfNumber] < 0)
                        insert = insert.Substring(1);
                        
                    mathExpression = previousOperatorIndex != 0
                        ? ReplaceCharacters(mathExpression, previousOperatorIndex + 1, dell, insert)
                        : ReplaceCharacters(mathExpression, previousOperatorIndex, dell, insert);

                    index = previousOperatorIndex + numbers[indexOfNumber].ToString().Length - 1;
                }

                else if (mathExpression[index] == '/')
                {
                    mathExpression = FunctionEvaluation(mathExpression, index + ANY_OPERATOR, indexOfNumber + 1, numbers);

                    var secondNumberLength = numbers[indexOfNumber + 1].ToString().Length;

                    int dell;
                    if (previousOperatorIndex == 0)
                        dell = mathExpression.Substring(previousOperatorIndex, index + 1 + secondNumberLength).Length;
                    else
                        dell = mathExpression.Substring(previousOperatorIndex + 1, index - previousOperatorIndex + secondNumberLength).Length;

                    numbers[indexOfNumber] /= numbers[indexOfNumber + 1];
                    numbers.RemoveAt(indexOfNumber + 1);
                    var insert = numbers[indexOfNumber].ToString();
                    if (numbers[indexOfNumber] < 0)
                        insert = insert.Substring(1);

                    mathExpression = previousOperatorIndex != 0
                        ? ReplaceCharacters(mathExpression, previousOperatorIndex + 1, dell, insert)
                        : ReplaceCharacters(mathExpression, previousOperatorIndex, dell, insert);

                    index = previousOperatorIndex + numbers[indexOfNumber].ToString().Length - 1;
                }

                else if (mathSymbols.Contains(mathExpression[index]))
                {
                    previousOperatorIndex = index;
                    indexOfNumber++;
                }
            }
        }

        private string FunctionEvaluation(string mathExpression, int index, int indexOfNumber, List<double> numbers)
        {
            var digitSearchRule = new Regex(@"(?<function>(?<![sna]|\d)(cos(ine)?|sin(us)?|tan(gent)?|abc))+");
            var foundNumbersInString = digitSearchRule.Matches(mathExpression);
            var numberOfCharactersToDelete = 0;

            foreach (Match res in foundNumbersInString)
            {
                GroupCollection groups = res.Groups;

                if (groups["function"].Index == index)
                {
                    numberOfCharactersToDelete = groups["function"].Length - 1 + numbers[indexOfNumber].ToString().Length;
                }
            }

            if (mathExpression[index] == 'c' && mathExpression[index + 1] == 'o')
            {
                return Cosine(mathExpression, index, indexOfNumber, numberOfCharactersToDelete, numbers);
            }

            else if (mathExpression[index] == 's' && mathExpression[index + 1] == 'i')
            {
                return Sinus(mathExpression, index, indexOfNumber, numberOfCharactersToDelete, numbers);
            }

            else if (mathExpression[index] == 't' && mathExpression[index + 1] == 'a')
            {
                return Tangent(mathExpression, index, indexOfNumber, numberOfCharactersToDelete, numbers);
            }

            else if (mathExpression[index] == 'a' && mathExpression[index + 1] == 'b')
            {
                return Module(mathExpression, index, indexOfNumber, numberOfCharactersToDelete, numbers);
            }
            return mathExpression;
        }
        private string Cosine(string mathExpression, int index, int indexOfNumber, int numberOfCharactersToDelete, List<double> numbers)
        {
            if (numbers[indexOfNumber] < 0)
            {
                numberOfCharactersToDelete--;
            }

            numbers[indexOfNumber] = Math.Cos(numbers[indexOfNumber]);

            string number = numbers[indexOfNumber].ToString();
            if (numbers[indexOfNumber] < 0)
            {
                number = number.Substring(1, number.Length - 1);
            }

            return ReplaceCharacters(mathExpression, index, numberOfCharactersToDelete, number);
        }
        private string Sinus(string mathExpression, int index, int indexOfNumber, int numberOfCharactersToDelete, List<double> numbers)
        {
            numbers[indexOfNumber] = Math.Sin(numbers[indexOfNumber]);

            string number = numbers[indexOfNumber].ToString();
            if (numbers[indexOfNumber] < 0)
            {
                number = number.Substring(1, number.Length - 1);
            }

            return ReplaceCharacters(mathExpression, index, numberOfCharactersToDelete, number);
        }
        private string Tangent(string mathExpression, int index, int indexOfNumber, int numberOfCharactersToDelete, List<double> numbers)
        {
            numbers[indexOfNumber] = Math.Tan(numbers[indexOfNumber]);

            string number = numbers[indexOfNumber].ToString();
            if (numbers[indexOfNumber] < 0)
            {
                number = number.Substring(1, number.Length - 1);
            }

            return ReplaceCharacters(mathExpression, index, numberOfCharactersToDelete, number);
        }
        private string Module(string mathExpression, int index, int indexOfNumber, int numberOfCharactersToDelete, List<double> numbers)
        {
            numbers[indexOfNumber] = Math.Abs(numbers[indexOfNumber]);

            string number = numbers[indexOfNumber].ToString();
            if (numbers[indexOfNumber] < 0)
            {
                number = number.Substring(1, number.Length - 1);
            }

            return ReplaceCharacters(mathExpression, index, numberOfCharactersToDelete, number);
        }
    }
}