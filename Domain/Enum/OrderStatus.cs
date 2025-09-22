namespace TMB.Challenge.Domain.Enum;

// Enum para representar os diferentes status de um pedido
// Quando um pedido é criado, ele começa como "Pendente" e pode mudar para "Processando" ou "Finalizado"
public enum OrderStatus
{
    Pendente,
    Processando,
    Finalizado
}