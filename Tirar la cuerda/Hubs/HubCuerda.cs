using Microsoft.AspNetCore.SignalR;


namespace Tirar_la_cuerda.Hubs
{
    public class HubCuerda : Hub
    {
        private static string nombre1 ="";
        private static string nombre2 = "";

        // Unirse a un grupo
        public async Task JoinGroup(string grupo, string nombre)
        {
            //Para comprobar que los nombres se inicialicen
            if (nombre1 == "")
            {
                nombre1 = nombre;
            }
            else
            {
                nombre2 = nombre;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, grupo);

        }

        //Envia a los jugadores del grupo para que vayan a la vista de la partida
        public async Task empezarJuego(string grupo, string nombre)
        {
            //Si los nombres no están empieza el juego para los del grupo solo me hace falta mirar el dos, ya que se llenarán en orden
            if (nombre2 != "")
            {
                await Clients.Group(grupo).SendAsync("EmpiezaJuego", nombre);
            }
        }
    }
    
}
