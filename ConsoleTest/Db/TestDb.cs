using ConsoleTest.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleTest.Db;

public class TestDb : DbContext
{
    
    public TestDb(DbContextOptions<TestDb> options) : base(options)
    {
        
    }

    public DbSet<DummyInfo> DummyInfos { get; set; }
    
    
}