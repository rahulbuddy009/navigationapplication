using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class OrderHub : Hub
{
    public async Task JoinDeliveryGroup(int deliveryPersonId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"delivery-{deliveryPersonId}");
    }
}
