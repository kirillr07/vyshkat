using Microsoft.Extensions.DependencyInjection;
using VyshKat.Domain.Abstractions;
using VyshKat.Domain.Services;
using VyshKat.Application;
using VyshKat.Infrastructure;
using VyshKat.Presentation;

var services = new ServiceCollection();

services.AddSingleton<IServiceCenter, ServiceCenter>();
services.AddSingleton<IFleetRepository, FleetRepository>();
services.AddTransient<FleetService>();
services.AddTransient<FleetReportService>();
services.AddTransient<ConsoleAppMenu>();

var serviceProvider = services.BuildServiceProvider();

var menu = serviceProvider.GetRequiredService<ConsoleAppMenu>();
menu.Run();