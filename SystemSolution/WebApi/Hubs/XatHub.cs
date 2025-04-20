using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    public class XatHub : Hub
    {
        public async Task SendMessage(string usuari, string missatge)
        {
            await Clients.All.SendAsync("ReceiveMessage", usuari, missatge);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Nou client: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            Console.WriteLine($"Client desconnectat: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(ex);
        }
    }
}
