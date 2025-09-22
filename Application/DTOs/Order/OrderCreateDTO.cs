using System.ComponentModel.DataAnnotations;

namespace TMB.Challenge.Application.DTOs.Order;

public class OrderCreateDTO
{
    [Required]
    public required string Produto { get; set; }
    [Required]
    public required string Cliente { get; set; }
    [Required]
    public required decimal Valor { get; set; }
}