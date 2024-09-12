using System.Linq.Expressions;
using AutoMapper;
using DrMadWill.Event.Synchronization.Service.Abstractions;
using DrMW.Core.Models.Abstractions;
using DrMW.EventBus.Core.Abstractions;
using DrMW.EventBus.Core.BaseModels;
using DrMW.Repositories.Abstractions.Works;
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
            _eventBus.Publish(model);
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
            TEntity? entity = await  (including != null ? repo.FindByIncludingQueryable(predicate,including) : repo.Queryable().Where(predicate))
                .FirstOrDefaultAsync();
            
            if (entity is null)
            {
                _logger.LogError(typeof(TEvent).Name + $" not syc | data not found. logKey: {logKey} ");
                return;
            }

            var model = _mapper.Map<TEvent>(entity);
            _eventBus.Publish(model);
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
            var all = await func(repo.Queryable()).ToListAsync();
            
            
            if (!all.Any())
                return;
            

            var models = all.Select( _mapper.Map<TEvent>).ToList();
            foreach (var model in models)
            {
                Thread.Sleep(TimeSpan.FromSeconds(second));
                _eventBus.Publish(model);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(typeof(TEvent).Name + $" all not syc | error occur . logKey:  {logKey} | Error {e}");
        }
    }
    
    public async Task SendSyc<TEvent, TEntity>
        (Func<IQueryable<TEntity>,IQueryable<TEntity>> func,string logKey,int second,params Expression<Func<TEntity,object>>[]? including)
        where TEntity : class
        where TEvent : IntegrationEvent
    {
        try
        {
            var repo = _unitOfWork.AnonymousRepository<TEntity>();
            var all = await func(repo.Queryable()).ToListAsync();
            
            
            if (!all.Any())
                return;
            

            var models = all.Select( _mapper.Map<TEvent>).ToList();
            foreach (var model in models)
            {
                Thread.Sleep(TimeSpan.FromSeconds(second));
                _eventBus.Publish(model);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(typeof(TEvent).Name + $" all not syc | error occur . logKey:  {logKey} | Error {e}");
        }
    }
    
    public virtual async Task SyncData<TEvent, TEntity, TPrimary>(TEvent @event) 
        where TEvent : IntegrationEvent, IHasDelete 
        where TEntity : class, IOriginEntity<TPrimary>
    {
        var repo = _unitOfWork. OriginRepository<TEntity, TPrimary>();
        var dict = _mapper.Map<TEntity>(@event);
        
        if (@event.IsDeleted == true)
        {
            await repo.RemoveAsync(dict);
            await _unitOfWork.CommitAsync();
        }
        else
        {
            if (await repo.Table.AnyAsync(s => s.Id.Equals(dict.Id))) await repo.UpdateAsync(dict);
            else await repo.AddAsync(dict);
            await _unitOfWork.CommitAsync();
        }
       
    }
    
    public virtual async Task SyncData<TEvent, TEntity>(TEvent @event) 
        where TEvent : IntegrationEvent, IHasDelete 
        where TEntity : class
    {
        var repo = _unitOfWork.AnonymousRepository<TEntity>();
        var dict = _mapper.Map<TEntity>(@event);
        
        if (@event.IsDeleted == true)
        {
            await repo.RemoveAsync(dict);
            await _unitOfWork.CommitAsync();
        }
        else
        {
            await repo.UpdateAsync(dict);
            await _unitOfWork.CommitAsync();
        }
       
    }
}