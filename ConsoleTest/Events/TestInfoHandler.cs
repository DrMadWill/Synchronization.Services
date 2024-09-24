using ConsoleTest.Models;
using DrMadWill.Event.Synchronization.Service.Abstractions;
using DrMW.EventBus.Core.Abstractions;

namespace ConsoleTest.Events;

public class TestInfoHandler : IIntegrationEventHandler<TestInfoIntegrationEvent>
{
    private readonly ISynchronizationService _synchronizationService;

    public TestInfoHandler(ISynchronizationService synchronizationService)
    {
        _synchronizationService = synchronizationService;
    }

    public async Task Handle(TestInfoIntegrationEvent @event)
    {
        await _synchronizationService.SyncData<TestInfoIntegrationEvent, DummyInfo, Guid>(@event,s => s.Id == @event.Id);
    }
}