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
            //Comprobar que los nombres no esten reetidos
            if(nombre1 == nombre || nombre2 == nombre)
            {
                await Clients.Caller.SendAsync("NombreRepetido");
            }

            //Para comprobar que los nombres se inicialicen
            if (nombre1 == "")
            {
                nombre1 = nombre;
            }
            else
            {
                nombre2 = nombre;
            }

            //Si los dos nombres están llenos, no se puede unir nadie más
            if (nombre2 !="" && nombre1 !="")
            {
                await Clients.Caller.SendAsync("GrupoLleno");
            }
            // Si no, se unen a un grupo
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, grupo);
                //Para que aparezcan en la  vista los jugadores
                await Clients.Group(grupo).SendAsync("añadeJugador", nombre1, nombre2);
            }


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
