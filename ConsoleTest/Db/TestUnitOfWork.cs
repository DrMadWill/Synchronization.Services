using System.Reflection;
using AutoMapper;
using DrMW.Repositories.Concretes.Works;

namespace ConsoleTest.Db;

public class TestUnitOfWork : UnitOfWork<TestDb>
{
    public TestUnitOfWork(TestDb context, IMapper mapper, IServiceProvider serviceProvider) : 
        base(context, typeof(TestDb).Assembly, mapper, serviceProvider)
    {
    }
}
