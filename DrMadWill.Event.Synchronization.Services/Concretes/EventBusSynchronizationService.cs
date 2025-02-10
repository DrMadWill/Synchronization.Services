using System.Linq.Expressions;
using AutoMapper;
using DrMadWill.Event.Synchronization.Service.Abstractions;
using DrMW.Core.Models.Abstractions;
using DrMW.EventBus.Core.Abstractions;
using DrMW.EventBus.Core.BaseModels;
using DrMW.Repositories.Abstractions.Works;
using DrMW.Repositories.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrMadWill.Event.Synchronization.Service.Concretes;

public class EventBusSynchronizationService : ISynchronizationService
{
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly ILogger<EventBusSynchronizationService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public EventBusSynchronizationService(IEventBus eventBus, IMapper mapper,
        ILogger<EventBusSynchronizationService> logger, IUnitOfWork unitOfWork)
    {
        _eventBus = eventBus;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task SendSyc<TEvent, TEntity, TPrimary>
        (Expression<Func<TEntity,bool>> predicate,string logKey,params Expression<Func<TEntity,object>>[]? including)
        where TEntity : class, IBaseEntity<TPrimary>
        where TEvent : IntegrationEvent
    {
        try
        {
            var repo = _unitOfWork.AnonymousRepository<TEntity>();
            TEntity? entity = await  (including != null ? repo.FindByIncludingQueryable(predicate,including) : repo.Queryable().Where(predicate))
                .FirstOrDefaultAsync();
            
            if (entity is null)
            {
                _logger.LogError(typeof(TEvent).Name + $" not syc | data not found. logKey: {logKey} ");
                return;
            }

            var model = _mapper.Map<TEvent>(entity);
            await _eventBus.Publish(model);
        }
        catch (Exception e)
        {
            _logger.LogError(typeof(TEvent).Name + $" not syc | error occur . logKey:  {logKey} | Error {e}");
        }
    }
    
    public async Task SendSyc<TEvent, TEntity>
        (Expression<Func<TEntity,bool>> predicate,string logKey,params Expression<Func<TEntity,object>>[]? including)
        where TEntity : class
        where TEvent : IntegrationEvent
    {
        try
        {
            var repo = _unitOfWork.AnonymousRepository<TEntity>();
            TEntity? entity = await  (including != null ? repo.FindByIncludingQueryable(predicate,including) : repo.Queryable().Where(predicate)).AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (entity is null)
            {
                _logger.LogError(typeof(TEvent).Name + $" not syc | data not found. logKey: {logKey} ");
                return;
            }

            var model = _mapper.Map<TEvent>(entity);
            await _eventBus.Publish(model);
        }
        catch (Exception e)
        {
            _logger.LogError(typeof(TEvent).Name + $" not syc | error occur . logKey:  {logKey} | Error {e}");
        }
    }
    
    public async Task SendSyc<TEvent, TEntity, TPrimary>
        (Func<IQueryable<TEntity>,IQueryable<TEntity>> func,string logKey,int second,params Expression<Func<TEntity,object>>[]? including)
        where TEntity : class,IBaseEntity<TPrimary>
        where TEvent : IntegrationEvent
    {
        try
        {
            var repo = _unitOfWork.Repository<TEntity, TPrimary>();
            var all = await func(repo.Queryable()).AsNoTracking().ToListAsync();
            
            
            if (!all.Any())
                return;
            

            var models = all.Select( _mapper.Map<TEvent>).ToList();
            foreach (var model in models)
            {
                Thread.Sleep(TimeSpan.FromSeconds(second));
                await _eventBus.Publish(model);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(typeof(TEvent).Name + $" all not syc | error occur . logKey:  {logKey} | Error {e}");
        }
    }

    public async Task RepairEvent(string id, string repairElement)
        => await _eventBus.BasicPublishAsync(id, GenerateEventName(repairElement));

    public async Task RepairEvent<TEntity>(string id)
        => await _eventBus.BasicPublishAsync(id, GenerateEventName(typeof(TEntity).Name));

    public async Task RepairListing(Dictionary<string,Func<string,Task>> repairs)
    {
        foreach (var repair in repairs)
        {
            var repairElement = GenerateEventName(repair.Key);
            await _eventBus.StartBasicConsumeAsync(repairElement, async (message, eventName) =>
            {
                await repair.Value(message);
            });
        }
    }
    
    private string GenerateEventName(string eventName)
    {
        return eventName + "Repair";
    }

    public async Task SendSyc<TEvent, TEntity>
        (Func<IQueryable<TEntity>,IQueryable<TEntity>> func,string logKey,int second,params Expression<Func<TEntity,object>>[]? including)
        where TEntity : class
        where TEvent : IntegrationEvent
    {
        try
        {
            var repo = _unitOfWork.AnonymousRepository<TEntity>();
            var all = await func(repo.Queryable()).AsNoTracking().ToListAsync();
            
            
            if (!all.Any())
                return;
            

            var models = all.Select( _mapper.Map<TEvent>).ToList();
            foreach (var model in models)
            {
                Thread.Sleep(TimeSpan.FromSeconds(second));
                await _eventBus.Publish(model);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(typeof(TEvent).Name + $" all not syc | error occur . logKey:  {logKey} | Error {e}");
        }
    }
    
    public virtual async Task SyncData<TEvent, TEntity, TPrimary>(TEvent @event,Expression<Func<TEntity,bool>> predicate) 
        where TEvent : IntegrationEvent, IHasDelete 
        where TEntity : class, IOriginEntity<TPrimary>
    {
        var repo = _unitOfWork. OriginRepository<TEntity, TPrimary>();
        if (@event.IsDeleted == true)
        {
            var dict = await  repo.Table.FirstOrDefaultAsync(predicate);
            if (dict != null)
            {
                await repo.RemoveAsync(dict);
            }
        }
        else
        {
            if (await repo.Queryable().AsNoTracking().AnyAsync(predicate))
            {
                Console.WriteLine("Sync Service Update =>>>>> : {0} | {1} ", typeof(TEvent).Name,@event.JsonString());
                var dict = await  repo.Table.FirstOrDefaultAsync(predicate);
                _mapper.Map(@event, dict); 
                await repo.UpdateAsync(dict);
            }
            else
            {
                Console.WriteLine("Sync Service Add =>>>>> : {0} | {1} ", typeof(TEvent).Name,@event.JsonString());
                var newEntity = _mapper.Map<TEntity>(@event);
                
                await repo.AddAsync(newEntity);
            }
        }

        await _unitOfWork.CommitAsync();
    }
    
 
     
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected implementation of Dispose pattern.
    /// </summary>
    /// <param name="disposing">Indicates whether the method call comes from a Dispose method (its value is true) or from a finalizer (its value is false).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Free any other managed objects here.
        }
    }

    /// <summary>
    /// Destructor for ReadRepository.
    /// </summary>
    ~EventBusSynchronizationService()
    {
        Dispose(false);
    }
    
    
}