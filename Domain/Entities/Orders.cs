using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMB.Challenge.Domain.Enum;


namespace TMB.Challenge.Domain.Entities;


public class Order
{
    // Chave primária com geração automática de identidade
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    // Gera um UUID único para cada pedido
    public string Uuid { get; set; } = Guid.NewGuid().ToString();
    public string? Produto { get; set; }
    // Cliente que fez o pedido
    public string? Cliente { get; set; }
    // Define o valor como decimal para suportar valores monetários
    public decimal Valor { get; set; }
    // Define a data de criação como a data e hora atual em UTC
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    // Define o status padrão como "Pendente"
    public OrderStatus Status { get; set; } = OrderStatus.Pendente;

    // Relação um-para-muitos com o histórico de status do pedido
    public virtual ICollection<OrderStatusHistory> StatusHistories { get; set; } = [];


    // Ele cria uma conversão implícita para OrderStatus. Isso significa que você pode usar um objeto Order
    // diretamente onde um OrderStatus é esperado, e ele automaticamente usará a propriedade Status.
    public static implicit operator OrderStatus(Order order)
    {
        return order.Status;
    }
}
