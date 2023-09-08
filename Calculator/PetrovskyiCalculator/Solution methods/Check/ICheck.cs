namespace Calculator.Solution_methods.Check
{
    public interface ICheck
    {
        Task <(bool Success, string ErrorMessage)> Check(string message);
        Task<bool> IsDivisionByZero(string mathExpression);
    }
}
