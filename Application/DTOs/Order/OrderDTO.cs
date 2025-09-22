using System.ComponentModel.DataAnnotations;
using TMB.Challenge.Domain.Entities;
using TMB.Challenge.Domain.Enum;

namespace TMB.Challenge.Application.DTOs.Order;


public class OrderDTO
{
    public required string Uuid { get; set; }
    public required string Produto { get; set; }
    public required string Cliente { get; set; }
    public OrderStatus Status { get; set; }
    public required decimal Valor { get; set; }
    public required DateTime DataCriacao { get; set; }
    public ICollection<OrderStatusHistory>? StatusHistories { get; set; }
}