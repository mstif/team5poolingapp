using Microsoft.AspNetCore.SignalR;

namespace api.Hubs
{
    public class EventsExchange:Hub
    {
        public async Task SendMessage( string operation, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", operation, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        public async Task SendMessageToGroup(string groupName, string operation, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {groupName}", message);
        }
    }
}


