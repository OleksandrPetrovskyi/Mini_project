namespace Calculator.Solution_methods
{
    public interface ISolution
    {
        int DecimalNumber { get; set; }
        Task<SolutionResponse> ResultAsync(string input);
    }
}
