using Microsoft.AspNetCore.SignalR;
namespace TMB.Challenge.API.Hubs;

public class OrdersHub : Hub
{

    public async Task SubscribeToOrder(string orderUuid)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderUuid);
    }

    public async Task UnsubscribeFromOrder(string orderUuid)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderUuid);
    }
}