using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMB.Challenge.Domain.Enum;


namespace TMB.Challenge.Domain.Entities;


public class Order
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Uuid { get; set; } = Guid.NewGuid().ToString();
    public string? Produto { get; set; }
    public string? Cliente { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pendente;
    public virtual ICollection<OrderStatusHistory> StatusHistories { get; set; } = [];
    public static implicit operator OrderStatus(Order order)
    {
        return order.Status;
    }
}
