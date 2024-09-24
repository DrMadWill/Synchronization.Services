using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConsoleTest.Db;

public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDb>
{
    public TestDb CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestDb>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=TestDb;User Id=SA;Password=StrongPassword123;");

        return new TestDb(optionsBuilder.Options);
    }
}
