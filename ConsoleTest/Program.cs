// See https://aka.ms/new-console-template for more information

using ConsoleTest;
using ConsoleTest.Db;
using ConsoleTest.Events;
using ConsoleTest.Models;
using DrMadWill.Event.Synchronization.Service;
using DrMadWill.Event.Synchronization.Service.Abstractions;
using DrMW.EventBus.Core.Abstractions;
using DrMW.EventBus.RabbitMq;
using DrMW.Repositories;
using DrMW.Repositories.Abstractions.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();
    
services.AddDbContext<TestDb>(options => options.UseSqlServer( "Server=localhost,1433;Database=TestDb;User Id=SA;Password=StrongPassword123;"));
services.AddAutoMapper(typeof(Program));
services.AddSynchronizationServices();
services.AddLogging();
services.AddScoped<TestInfoHandler>();
services.LayerRepositoriesRegister<TestUnitOfWork, TestQueryRepositories, TestServiceManager, TestDb, TestDb>();
services.AddRabbitMq("Test.Ala","amqp://guest:guest@localhost:5672");
var app = services.BuildServiceProvider();
var eventBus = app.GetService<IEventBus>();
await eventBus.Subscribe<TestInfoIntegrationEvent, TestInfoHandler>();
using var scop = app.CreateScope();
var unitOfWork = scop.ServiceProvider.GetService<IUnitOfWork>();
var sync = scop.ServiceProvider.GetService<ISynchronizationService>();
var s =  await unitOfWork.Repository<DummyInfo, Guid>()
    .AddAsync(new DummyInfo
    {
        Name = "DDest"
    });
await unitOfWork.CommitAsync();
while (true)
{
    await sync.SendSyc<TestInfoIntegrationEvent, DummyInfo, Guid>(s => s.Id == s.Id,s.Id.ToString());
    Console.ReadKey();
}

//


