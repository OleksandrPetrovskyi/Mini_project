using System.Text.RegularExpressions;

namespace Calculator.Solution_methods.Check
{
    public class CheckMathExpressionForStandartCalculator : ICheck
    {
        private const string numbers = "numbers";
        private const string symbols = "symbols";
        protected Regex mathematicalExpressionValidation;

        public CheckMathExpressionForStandartCalculator()
        {
            mathematicalExpressionValidation = new Regex(@$"[()]*(?<{numbers}>-?\d*([.,]\d+)?)(?<{symbols}>(?<=[\d)])[*/+=](?![)+*/]))*");
        }
        public async Task<(bool Success, string ErrorMessage)> Check(string message)
        {
            if (message == string.Empty || message[0] == '=')
                return (false, "");

            var parenthesesCheck = new Stack<char>();

            foreach (char c in message)
            {
                if (c == '(')
                {
                    parenthesesCheck.Push(c);
                }
                else if (c == ')')
                {
                    if (parenthesesCheck.Count == 0)
                        return (false, "Incorrect use of parentheses in a formula.");

                    parenthesesCheck.Pop();
                }
            }

            if (parenthesesCheck.Count != 0)
                return (false, "Incorrect use of parentheses in a formula.");
            else if (message.Where(equal => equal == '=').Count() > 1)
                return (false, "Formula can have 0 or 1 equal sign.");

            for (int i = message.IndexOf('-') + 1; i != 0; i = message.IndexOf('-', i) + 1)
            {
                if (!char.IsNumber(message[i]) && !char.IsLetter(message[i]) && message[i] != '(')
                    return (false, "After the minus there should be a mathematical expression");
            }

            for (int i = message.IndexOf('(') + 1; i != 0; i = message.IndexOf('(', i) + 1)
            {
                if (message[i] == ')')
                    return (false, "Parentheses must contain a mathematical expression.");
            }

            MatchCollection foundMathSymbolsInString = await Task.Run(() => mathematicalExpressionValidation.Matches(message));

            if (foundMathSymbolsInString[0].Groups[0].Length != message.Length)
                return (false, $"Error in formula before expression: {message.Substring(foundMathSymbolsInString[0].Groups[0].Index)}");
            else if (await IsDivisionByZero(message) == true)
                return (false, "Cannot divide by zero");

            return (true, "");
        }
        public async Task<bool> IsDivisionByZero(string mathExpression)
        {
            var checkingForDivisionByZero = new Regex(@"(\d*([.,]\d*)?\/-?0)");

            if (checkingForDivisionByZero.IsMatch(mathExpression))
                return true;
            else
                return false;
        }
    }
}
