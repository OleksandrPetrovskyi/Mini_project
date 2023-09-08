using System.Text.RegularExpressions;

namespace Calculator.Solution_methods.Check
{
    public class CheckMathExpressionForExtendedCalculator : CheckMathExpressionForStandartCalculator
    {
        private const string numbers = "numbers";
        private const string symbols = "symbols";
        private const string function = "function";

        public CheckMathExpressionForExtendedCalculator() : base()
        {
            mathematicalExpressionValidation = new Regex(@$"([()]*(?<{function}>(?<![sna]|\d)-?(cos|sin|tan|abc)(?=[\((-?\d)]))*(?<{numbers}>-?\d*([.,]\d+)?)(?<{symbols}>(?<=[\d)])[*/+](?![)+*/]))*)+=");
        }
    }
}
