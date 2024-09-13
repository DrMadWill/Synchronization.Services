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
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    /// <returns></returns>
    Task SyncData<TEvent, TEntity, TPrimary>(TEvent @event)
        where TEvent : IntegrationEvent, IHasDelete
        where TEntity : class, IOriginEntity<TPrimary>;

    /// <summary>
    /// Write Database
    /// </summary>
    /// <param name="event"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    Task SyncData<TEvent, TEntity>(TEvent @event)
        where TEvent : IntegrationEvent, IHasDelete
        where TEntity : class;

}