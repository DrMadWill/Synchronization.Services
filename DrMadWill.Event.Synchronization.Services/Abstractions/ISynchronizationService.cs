using System.Linq.Expressions;
using DrMW.Core.Models.Abstractions;
using DrMW.EventBus.Core.BaseModels;

namespace DrMadWill.Event.Synchronization.Service.Abstractions;

public interface ISynchronizationService : IDisposable
{
    Task SendSyc<TEvent, TEntity, TPrimary>(Expression<Func<TEntity, bool>> predicate, string logKey, 
        params Expression<Func<TEntity, object>>[]? including)
        where TEntity : class, IBaseEntity<TPrimary>
        where TEvent : IntegrationEvent;

    Task SendSyc<TEvent, TEntity>
    (Expression<Func<TEntity, bool>> predicate, string logKey,
        params Expression<Func<TEntity, object>>[]? including)
        where TEntity : class
        where TEvent : IntegrationEvent;
    
    Task SendSyc<TEvent, TEntity, TPrimary>
    (Func<IQueryable<TEntity>, IQueryable<TEntity>> func, string logKey, int second,
        params Expression<Func<TEntity, object>>[]? including)
        where TEntity : class, IBaseEntity<TPrimary>
        where TEvent : IntegrationEvent;

    /// <summary>
    /// Write database
    /// </summary>
    /// <param name="event"></param>
    /// <param name="predicate"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    /// <returns></returns>
    Task SyncData<TEvent, TEntity, TPrimary>(TEvent @event,Expression<Func<TEntity,bool>> predicate)
        where TEvent : IntegrationEvent, IHasDelete
        where TEntity : class, IOriginEntity<TPrimary>;

    Task RepairEvent(string id, string repairElement);
    Task RepairEvent<TEntity>(string id);
    Task RepairListing(Dictionary<string, Func<string, Task>> repairs);

    Task<bool> DefaultRepair<TEntity, TPrimary>(string id, string repairElement = "")
        where TEntity : class, IOriginEntity<TPrimary>;

    Task<bool> DefaultRepairIntPrimary<TEntity>(int id, string repairElement = "")
        where TEntity : class, IOriginEntity<int>;

    Task<bool> DefaultRepairStringPrimary<TEntity>(string id, string repairElement = "")
        where TEntity : class, IOriginEntity<string>;

    Task<bool> DefaultRepairGuidPrimary<TEntity>(Guid id, string repairElement = "")
        where TEntity : class, IOriginEntity<Guid>;
}