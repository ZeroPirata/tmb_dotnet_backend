using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TMB.Challenge.Domain.Enum;

namespace TMB.Challenge.Domain.Entities;

public class OrderStatusHistory
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    [Required]
    public OrderStatus PreviousStatus { get; set; }
    [Required]
    public OrderStatus NewStatus { get; set; }
    [Required]
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    [ForeignKey("OrderId")]
    [JsonIgnore]
    public Order Order { get; set; } = null!;
}