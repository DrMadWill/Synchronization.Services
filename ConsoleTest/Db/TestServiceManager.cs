using System.Reflection;
using AutoMapper;
using DrMW.Repositories.Abstractions.Works;
using DrMW.Repositories.Services.Concretes;

namespace ConsoleTest.Db;

public class TestServiceManager : ServiceManager
{
    public TestServiceManager(IUnitOfWork unitOfWork, IQueryRepositories queryRepositories, IMapper mapper, IServiceProvider serviceProvider) 
        : base(unitOfWork, queryRepositories, mapper, typeof(Program).Assembly, serviceProvider)
    {
    }
}