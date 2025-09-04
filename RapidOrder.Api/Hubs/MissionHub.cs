using Microsoft.AspNetCore.SignalR;

namespace RapidOrder.Api.Hubs
{
    public class MissionHub : Hub<IMissionClient>
    {
        // Optional: grouping by waiter or place-group later
        public Task JoinWaiterGroup(string waiterId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, $"waiter-{waiterId}");
    }
}
