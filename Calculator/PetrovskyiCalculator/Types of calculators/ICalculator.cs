namespace Calculator.Types_of_calculators
{
    internal interface ICalculator
    {
        Task StartCalculator();
        Task CalculationsFromFile(string path);
    }
}
