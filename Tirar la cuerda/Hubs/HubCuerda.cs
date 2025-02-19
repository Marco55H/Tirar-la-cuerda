using Ent;
using Microsoft.AspNetCore.SignalR;


namespace Tirar_la_cuerda.Hubs
{
    public class HubCuerda : Hub
    {
        private static ClsJugador jugador1 = new ClsJugador();
        private static ClsJugador jugador2 = new ClsJugador();

        // Unirse a un grupo
        public async Task JoinGroup(string grupo, string nombre)
        {
            //Si los dos nombres están llenos, no se puede unir nadie más
            if (jugador2.Nombre !="")
            {
                await Clients.Caller.SendAsync("GrupoLleno");
            }
            // Si no, se comprueba que no esten repetidos
            else
            {

                //Comprobar que los nombres no esten repetidos
                if (jugador1.Nombre == nombre)
                {
                    await Clients.Caller.SendAsync("NombreRepetido");
                }
                //Si no estan repetidos 
                else
                {
                    //Para comprobar que los nombres se inicialicen
                    if (jugador1.Nombre == "")
                    {
                        jugador1.Nombre = nombre;
                    }
                    else if (jugador2.Nombre == "")
                    {
                        jugador2.Nombre = nombre;
                    }

                    await Groups.AddToGroupAsync(Context.ConnectionId, grupo);
                    //Para que aparezcan en la  vista los jugadores
                    await Clients.Group(grupo).SendAsync("añadeJugador", jugador1, jugador2);

                }
            }
        }

        //Envia a los jugadores del grupo para que vayan a la vista de la partida
        public async Task empezarJuego(string grupo, string nombre)
        {
            //Si los nombres no están empieza el juego para los del grupo solo me hace falta mirar el dos, ya que se llenarán en orden
            if (jugador2.Listo && jugador1.Listo)
            {
                await Clients.Group(grupo).SendAsync("EmpiezaJuego", nombre);
            }
        }
    }   
}
