using Microsoft.AspNetCore.SignalR;


namespace Tirar_la_cuerda.Hubs
{
    public class HubCuerda : Hub
    {
        // Unirse a un grupo
        public async Task JoinGroup(string grupo)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, grupo);
        }
    }
    
}
