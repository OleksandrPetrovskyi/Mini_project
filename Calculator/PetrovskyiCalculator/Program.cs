using Microsoft.Extensions.DependencyInjection;
using Calculator;
using Calculator.Logger;
using Calculator.Reader;
using Calculator.Solution_methods;
using Calculator.Solution_methods.Check;
using Calculator.Types_of_calculators;
using Calculator.UserInterface;

var serviceProvider = new ServiceCollection()
    .AddSingleton<IUserInterface, ConsoleInterface>()
    .AddSingleton<ILogger, FileLogger>()
        .AddSingleton<IReader, FileReader>()
    .AddSingleton<ICalculator, ConsoleCalculator>()
    .AddSingleton<ISolution, ExtendedSolution>()
    .AddSingleton<ICheck, CheckMathExpressionForExtendedCalculator>()
    .AddSingleton<CustomThreadPool>()
    .BuildServiceProvider();

var calculator = serviceProvider.GetRequiredService<ICalculator>();

try
{
    await calculator.StartCalculator();
}
catch (Exception ex) { }