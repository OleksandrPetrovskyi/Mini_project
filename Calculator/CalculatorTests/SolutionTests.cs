using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Calculator.Solution_methods;
using Calculator.Solution_methods.Check;
using System;
using System.Data;
using System.Threading.Tasks;

namespace CalculatorTests
{
    [TestClass]
    public class SolutionTests
    {
        [DataTestMethod]
        [DataRow("1=", 1)]
        [DataRow("-1+2=", 1)]
        [DataRow("2*2=", 4)]
        [DataRow("-2*-2=", 4)]
        [DataRow("2/-1=", -2)]
        [DataRow("2+2*2=", 6)]
        [DataRow("2*(5+(6*2)/(2-1)+1)*2=", 72)]
        public async Task Result_WithValidFormulas_SolutionOfEquations(string formula, double answerStr)
        {
            // Arrange
            var answer = Convert.ToDouble(answerStr);

            var check = new Mock<ICheck>();
            check.Setup(c => c.Check(formula).Result).Returns((true, ""));
            var solution = new ExtendedSolution(check.Object);
            solution.DecimalNumber = 3;

            //act
            double.TryParse((await solution.ResultAsync(formula)).Response, out var result);
            var a = await solution.ResultAsync(formula);
            
            //assert
            Assert.AreEqual(result, answer, 0.01);
        }

        [DataTestMethod]
        [DataRow("-cos-+5=", "After the minus there should be a mathematical expression")]
        [DataRow("2/-=", "After the minus there should be a mathematical expression")]
        [DataRow("++3=", "Error in formula before expression:")]
        [DataRow("-tansin(5)=", "Error in formula before expression:")]
        [DataRow("2/0=", "Cannot divide by zero")]
        [DataRow("()=", "Parentheses must contain a mathematical expression.")]
        [DataRow(")(=", "Incorrect use of parentheses in a formula.")]
        [DataRow("())=", "Incorrect use of parentheses in a formula.")]
        [DataRow("2+2==", "Formula can have 0 or 1 equal sign.")]
        public async Task Check_WithoutValidFormulas_CheckingEquations(string formula, string errorMessage)
        {
            // Arrange
            var check = new CheckMathExpressionForExtendedCalculator();

            //act
            var result = (await check.Check(formula)).ErrorMessage;

            //assert
            Assert.IsTrue(result.Contains(errorMessage));
        }
    }
}