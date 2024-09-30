using DrMW.EventBus.Core.Abstractions;

namespace ConsoleTest.Events;

public class HelloHandler : IIntegrationEventHandler<HelloInfoIntegrationEvent>
{
    private readonly IEventBus _eventBus;
    public HelloHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public async Task Handle(HelloInfoIntegrationEvent @event)
    {
        Console.WriteLine("Hello World!");
    }
}