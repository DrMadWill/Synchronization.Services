using System.ComponentModel.DataAnnotations.Schema;
using DrMW.Core.Models.Abstractions;

namespace ConsoleTest.Models;

public class DommyDbS : IOriginEntity<Guid>
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public string Name { get; set; }
}