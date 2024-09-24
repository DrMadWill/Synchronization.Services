using DrMW.Core.Models.Abstractions;
using DrMW.EventBus.Core.BaseModels;

namespace ConsoleTest.Events;

public class TestInfoIntegrationEvent : IntegrationEvent,IHasDelete
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool? IsDeleted { get; set; }
}