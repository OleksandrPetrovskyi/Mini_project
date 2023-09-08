using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi_Client.Converters;
using WebApi_Client.Logger;
using WebApi_Client.Repository;

namespace WebApi_Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<HttpClient>()
                .AddSingleton<Startup>()
                .AddSingleton<StringConverter>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<ILogger, ConsoleLogger>()
                .BuildServiceProvider();

            await serviceProvider.GetRequiredService<Startup>().Run();
        }
    }
}
