using DrMadWill.Event.Synchronization.Service.Abstractions;
using DrMadWill.Event.Synchronization.Service.Concretes;
using Microsoft.Extensions.DependencyInjection;

namespace DrMadWill.Event.Synchronization.Service;

public static class ServiceRegistration
{
    public static IServiceCollection AddSynchronizationServices(this IServiceCollection services)
    {

        services.AddScoped<ISynchronizationService, EventBusSynchronizationService>();
        
        return services;
    }
    
}