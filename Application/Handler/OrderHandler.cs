using TMB.Challenge.Application.DTOs.Order;
using TMB.Challenge.Application.Interfaces;
using TMB.Challenge.Domain.Entities;
using Microsoft.Extensions.Logging;
using TMB.Challenge.Application.Common;
using TMB.Challenge.Domain.Enum;


namespace TMB.Challenge.Application.Handler;

/// <summary>
/// Classe responsavel para lidar com as requisições relacionadas a pedidos dentro da aplicação.
/// </summary>
public class OrderHandler(IOrderRepository repository, ILogger<OrderHandler> logger, IOrderStatusHistoryRepository repositoryStatusHistory, IOutboxMessage outboxRepository, IMessageBusService messageBusService)
{
    private readonly IOrderRepository _repository = repository;
    private readonly IOrderStatusHistoryRepository _repositoryStatusHistory = repositoryStatusHistory;
    private readonly IOutboxMessage _outboxRepository = outboxRepository;
    private readonly ILogger<OrderHandler> _logger = logger;
    private readonly IMessageBusService _messageBusService = messageBusService;
    public async Task<Result<OrderDTO>> PostOrder(OrderCreateDTO orderDto)
    {
        try
        {
            var order = new Order
            {
                Cliente = orderDto.Cliente,
                Produto = orderDto.Produto,
                Valor = orderDto.Valor
            };
            Order createdOrder = await _repository.AddAsync(order);
            var messageToPublish = new OrderMessageDTO { Uuid = createdOrder.Uuid };
            await _outboxRepository.AddAsync(messageToPublish);
            var resultDto = new OrderDTO
            {
                DataCriacao = createdOrder.DataCriacao,
                Cliente = createdOrder.Cliente ?? string.Empty,
                Produto = createdOrder.Produto ?? string.Empty,
                Valor = createdOrder.Valor,
                Uuid = createdOrder.Uuid,
                Status = createdOrder.Status
            };
            return Result<OrderDTO>.Success(resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro ao criar o pedido para o cliente {Cliente}", orderDto.Cliente);
            return Result<OrderDTO>.Failure("Ocorreu um erro interno ao processar o pedido.");
        }
    }
    public async Task<PaginatedResult<OrderDTO>> GetAllOrdersAsync(int pageNumber, int pageSize, OrderStatus? status = null)
    {
        var paginatedOrders = await _repository.GetAllAsync(pageNumber, pageSize, status);
        var orderDtos = paginatedOrders.Items.Select(o => new OrderDTO
        {
            Uuid = o.Uuid,
            Cliente = o.Cliente ?? string.Empty,
            Produto = o.Produto ?? string.Empty,
            Valor = o.Valor,
            Status = o.Status,
            DataCriacao = o.DataCriacao,
        }).ToList();

        return new PaginatedResult<OrderDTO>(orderDtos, paginatedOrders.TotalCount, paginatedOrders.PageNumber, paginatedOrders.PageSize);
    }
    public async Task<Result<OrderDTO>> GetOrderByUuidAsync(string uuid)
    {
        var order = await _repository.GetByUuidAsync(uuid);
        if (order is null) return Result<OrderDTO>.Failure("Não encontrado");

        return Result<OrderDTO>.Success(new OrderDTO
        {
            Uuid = order.Uuid,
            Cliente = order.Cliente ?? string.Empty,
            Produto = order.Produto ?? string.Empty,
            Valor = order.Valor,
            Status = order.Status,
            DataCriacao = order.DataCriacao,
            StatusHistories = order.StatusHistories
        });
    }

    /// <summary>
    /// Processa um pedido recebido da fila.
    /// </summary>
    public async Task ProcessOrderFromQueue(OrderMessageDTO message)
    {
        var order = await _repository.GetByUuidAsync(message.Uuid);
        if (order is null)
        {
            _logger.LogError("Pedido com Uuid {OrderUuid} não encontrado.", message.Uuid);
            return;
        }

        if (order.Status != OrderStatus.Pendente)
        {
            _logger.LogWarning("Pedido {OrderUuid} já está sendo ou foi processado. Status: {Status}. Mensagem ignorada.", order.Uuid, order.Status);
            return;
        }

        try
        {
            // --- ATUALIZAÇÃO PARA "PROCESSANDO" ---
            order.Status = OrderStatus.Processando;
            await _repository.UpdateAsync(order);
            _logger.LogInformation("Status do pedido {Uuid} atualizado para 'Processando' no banco.", order.Uuid);

            // Busca o pedido ATUALIZADO com o histórico mais recente
            var updatedOrderForProcessing = await _repository.GetByUuidAsync(message.Uuid);

            // CORREÇÃO: Cria um payload anônimo formatando a data corretamente
            var processingPayload = new
            {
                Uuid = updatedOrderForProcessing?.Uuid,
                Status = updatedOrderForProcessing?.Status.ToString(),
                // Usamos .Select() para criar um novo objeto com a data no formato ISO 8601
                StatusHistories = updatedOrderForProcessing?.StatusHistories.Select(h => new
                {
                    h.Id,
                    h.PreviousStatus,
                    h.NewStatus,
                    // A formatação "o" garante compatibilidade com JavaScript
                    ChangedAt = h.ChangedAt.ToString("o")
                })
            };
            await _messageBusService.PublishMessageAsync(processingPayload, "order-status-updates");
            _logger.LogInformation("Notificação de status 'Processando' enviada para {Uuid}.", order.Uuid);

            await Task.Delay(5000);

            // --- ATUALIZAÇÃO PARA "FINALIZADO" ---
            order.Status = OrderStatus.Finalizado;
            await _repository.UpdateAsync(order);
            _logger.LogInformation("Status do pedido {Uuid} atualizado para 'Finalizado' no banco.", order.Uuid);

            // Busca o pedido FINAL com o histórico completo
            var finalOrder = await _repository.GetByUuidAsync(message.Uuid);

            // CORREÇÃO: Formata o payload final da mesma forma
            var finalPayload = new
            {
                Uuid = finalOrder?.Uuid,
                Status = finalOrder?.Status.ToString(),
                StatusHistories = finalOrder?.StatusHistories.Select(h => new
                {
                    h.Id,
                    h.PreviousStatus,
                    h.NewStatus,
                    ChangedAt = h.ChangedAt.ToString("o")
                })
            };
            await _messageBusService.PublishMessageAsync(finalPayload, "order-status-updates");
            _logger.LogInformation("Notificação de status 'Finalizado' enviada para {Uuid}.", order.Uuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar o pedido {OrderUuid}.", order.Uuid);
        }
    }
}