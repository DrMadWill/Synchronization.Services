using DrMW.Core.Models.Abstractions;

namespace ConsoleTest.Models;

public class DummyInfo : IBaseEntity<Guid>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}