using Ent;
using Microsoft.AspNetCore.SignalR;


namespace Tirar_la_cuerda.Hubs
{
    public class HubCuerda : Hub
    {
        //Lista de grupos que existen
        private static List<ClsGrupo> grupos = new List<ClsGrupo>();


        // Unirse a un grupo
        public async Task JoinGroup(string grupo, string nombre)
        {
            //Esta variable se usa para ver el grupo al que queremos unirnos
            ClsGrupo grupoActual = grupos.FirstOrDefault(g => g.Nombre == grupo);

            //Si el grupo no existe, se crea
            if (grupoActual == null)
            {
                grupoActual = new ClsGrupo();
                grupoActual.Nombre = grupo;
                grupos.Add(grupoActual);
                ClsJugador jugador1 = new ClsJugador();

                //Inicializo el primer jugador para meterlo en el grupo
                jugador1 = new ClsJugador(nombre, grupo, 0, false);

                ClsJugador jugador2 = new ClsJugador();
                grupoActual.AddJugador(jugador1);
                grupoActual.AddJugador(jugador2);


                //Añadir al grupo
                await Groups.AddToGroupAsync(Context.ConnectionId, grupo);

                //Añadir a los jugadores al grupo
                await Clients.Group(grupo).SendAsync("jugadoresDelGrupo", grupoActual.Jugadores[0], grupoActual.Jugadores[1]);
            }
            else
            {
                //Si el grupo tiene dos jugadores, esta lleno
                if (!String.IsNullOrEmpty(grupoActual.Jugadores[1].Nombre))
                {
                    await Clients.Caller.SendAsync("GrupoLleno");
                }
                // Si no, se comprueba que no esten repetidos
                else
                {
                    //Comprobar que los nombres no esten repetidos
                    //Si el nombre que quieres añadir es igual al nombre del jugador que ya esta añadido, son iguales
                    if (grupoActual.Jugadores[0].Nombre == nombre)
                    {
                        await Clients.Caller.SendAsync("NombreRepetido");
                    }
                    //Si no estan repetidos 
                    else
                    {
                        //Para comprobar que los nombres se inicialicen y añada al grupo a los jugadores
                        //Se añade se añade el jugador 2, ya que el jugador 1 se creo cuando se creo el grupo
                        grupoActual.Jugadores[1] = new ClsJugador();
                        grupoActual.Jugadores[1].Nombre = nombre;
                        grupoActual.Jugadores[1].Grupo = grupo;
                        grupoActual.Jugadores[1].Puntuacion = 0;
                        grupoActual.Jugadores[1].Listo = false;

                        //Añadir al grupo
                        await Groups.AddToGroupAsync(Context.ConnectionId, grupo);

                        //Para que aparezcan en la  vista los jugadores que hay en el grupo
                        await Clients.Group(grupo).SendAsync("jugadoresDelGrupo", grupoActual.Jugadores[0], grupoActual.Jugadores[1]);

                    }
                }
            }
        }

        // Salir de un grupo
        public async Task LeaveGroup(string grupo, string nombre)
        {
            //Esta variable se usa para ver el grupo que estamos usando
            ClsGrupo grupoActual = grupos.FirstOrDefault(g => g.Nombre == grupo);

            //Si el grupo existe habrá que borrarlo de la lista de grupos y tendremos que borrar los datos para la UI
            if (grupoActual != null)
            {


                //Si el nombre es igual al nombre del jugador 2 , se borra el jugador 2 de la lista de jugadores del grupo,
                //si es el creador del grupo el que se cambia, es decir, el jugador 1, eliminamos aese jugador
                if (grupoActual.Jugadores[1].Nombre == nombre)
                {
                    grupoActual.Jugadores[1] = new ClsJugador();
                }
                else
                {
                    grupoActual.Jugadores[0] = new ClsJugador();
                }

                grupoActual.Jugadores[0].Listo = false;
                grupoActual.Jugadores[1].Listo = false;


                //Se envia a la vista los jugadores que hay en el grupo
                await Clients.Group(grupo).SendAsync("jugadoresDelGrupo", grupoActual.Jugadores[0], grupoActual.Jugadores[1]);

                //Si los dos jugadores estan vacios, se elimina el grupo
                if (grupoActual.Jugadores[0].Nombre == "" && grupoActual.Jugadores[1].Nombre == "")
                {
                    grupos.Remove(grupoActual);
                }
                //Borrar del grupo
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupo);
            }
        }

        // Listo para jugar
        public async Task Preparado(string grupo, string nombre)
        {
            //Esta variable se usa para ver el grupo que estamos usando
            ClsGrupo grupoActual = grupos.FirstOrDefault(g => g.Nombre == grupo);
            //Si el grupo existe, se cambia el estado de listo del jugador
            if (grupoActual != null)
            {
                //Si el nombre es igual al nombre del jugador 2 , se cambia el estado de listo del jugador 2
                if (grupoActual.Jugadores[1].Nombre == nombre)
                {
                    //Para cambiar entre true y false si es true cambio a false, si es false a true
                    if (grupoActual.Jugadores[1].Listo == false)
                    {
                        grupoActual.Jugadores[1].Listo = true;
                    }
                    else
                    {
                        grupoActual.Jugadores[1].Listo = false;
                    }
                }
                //Si el nombre es igual al nombre del jugador 1 , se cambia el estado de listo del jugador 1
                else
                {
                    //Para cambiar entre true y false si es true cambio a false, si es false a true
                    if (grupoActual.Jugadores[0].Listo == false)
                    {
                        grupoActual.Jugadores[0].Listo = true;
                    }
                    else
                    {
                        grupoActual.Jugadores[0].Listo = false;
                    }
                }
                //Se envia a la vista los jugadores que hay en el grupo
                await Clients.Group(grupo).SendAsync("jugadoresDelGrupo", grupoActual.Jugadores[0], grupoActual.Jugadores[1]);
                //Si los dos jugadores estan listos, se inicia el juego
                if (grupoActual.Jugadores[0].Listo && grupoActual.Jugadores[1].Listo)
                {
                    await Clients.Group(grupo).SendAsync("IniciarJuego");
                }
            }
        }

        // Pillar el otro jugador
        public async Task nombreEnemigo(string grupo, string nombre)
        {
            //Esta variable se usa para ver el grupo que estamos usando
            ClsGrupo grupoActual = grupos.FirstOrDefault(g => g.Nombre == grupo);
            //Si el grupo existe, se envia el nombre del otro jugador
            if (grupoActual != null)
            {
                //Si el nombre es igual al nombre del jugador 2 , se envia el nombre del jugador 1
                if (grupoActual.Jugadores[1].Nombre == nombre)
                {
                    await Clients.Caller.SendAsync("nombreEnemigo", grupoActual.Jugadores[0].Nombre);
                }
                //Si el nombre es igual al nombre del jugador 1 , se envia el nombre del jugador 2
                else
                {
                    await Clients.Caller.SendAsync("nombreEnemigo", grupoActual.Jugadores[1].Nombre);
                }
            }
        }

        //Tirar de la cuerda
        public async Task tirarCuerda(string grupo, string nombre)
        {
            //Esta variable se usa para ver el grupo que estamos usando
            ClsGrupo grupoActual = grupos.FirstOrDefault(g => g.Nombre == grupo);

            //Si el grupo existe, se envia el nombre del otro jugador
            if (grupoActual != null)
            {
                //Si el jugador 2 es el que ha pulsado, se le restan puntos al jugador 2 y se le suman al jugador 1 se hace asi por interfaz
                if (grupoActual.Jugadores[1].Nombre == nombre)
                {
                    grupoActual.Jugadores[0].Puntuacion+=8;
                    grupoActual.Jugadores[1].Puntuacion-=8;
                }
                //Si el jugador 2 no ha sido el que ha pulsado, se le restan puntos al jugador 1 y se le suman al jugador 2
                else
                {
                    grupoActual.Jugadores[0].Puntuacion-=8;
                    grupoActual.Jugadores[1].Puntuacion+=8;
                }
                //Enviamos a los jugadores del grupo los dos jugadores con sus puntuaciones modificadas
                await Clients.All.SendAsync("tirarCuerda", grupoActual.Jugadores[0], grupoActual.Jugadores[1]);
            }
        }
    }
}