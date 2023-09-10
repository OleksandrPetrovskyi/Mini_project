using LasticShopAdo;
using LasticShopAdo.Interfaces;
using LasticShopAdo.Repository;
using Microsoft.Extensions.DependencyInjection;
using LasticShopAdo.Validation;
using System.ComponentModel;


var serviseProvider = new ServiceCollection()
    .AddSingleton<IUserInterface, UserInterface>()
    .AddSingleton<IShopRepository, ShopRepository>()
.AddSingleton<Startup>()
    .AddSingleton<StringСonverter>()
    .AddSingleton<IDataValidation, DataValidation>()
    .BuildServiceProvider();

await serviseProvider.GetRequiredService<Startup>().Run();
