using ConsoleTest.Models;
using DrMadWill.Event.Synchronization.Service.Abstractions;
using DrMW.EventBus.Core.Abstractions;

namespace ConsoleTest.Events;

public class TestInfoHandler : IIntegrationEventHandler<TestInfoIntegrationEvent>
{
    private readonly ISynchronizationService _synchronizationService;
    private readonly IEventBus _eventBus;

    public TestInfoHandler(ISynchronizationService synchronizationService, IEventBus eventBus)
    {
        _synchronizationService = synchronizationService;
        _eventBus = eventBus;
    }

    public async Task Handle(TestInfoIntegrationEvent @event)
    {
        await _synchronizationService.SyncData<TestInfoIntegrationEvent, DommyDbS, Guid>(@event,s => s.Id == @event.Id);
        await _eventBus.Publish(new HelloInfoIntegrationEvent { });
    }
}