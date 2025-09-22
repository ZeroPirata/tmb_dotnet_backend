using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMB.Challenge.Domain.Entities;

public class OutboxMessage
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string? EventType { get; set; }
    public string? Payload { get; set; }
    public string? Status { get; set; } = "Pendente";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}