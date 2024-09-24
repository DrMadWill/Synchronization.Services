using System.Reflection;
using AutoMapper;
using DrMW.Repositories.Concretes.Works;

namespace ConsoleTest.Db;

public class TestQueryRepositories : QueryRepositories<TestDb>
{
    public TestQueryRepositories(TestDb dbContext, IMapper mapper,IServiceProvider serviceProvider) : base(dbContext, mapper, typeof(Program).Assembly,serviceProvider)
    {
    }
}