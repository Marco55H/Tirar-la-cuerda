using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ent
{
    /// <summary>
    /// Clase grupo para representar los grupos de jugadores que existen
    /// </summary>
    public class ClsGrupo
    {
        private List<ClsJugador> jugadores = new List<ClsJugador>();
        private string nombre;
        private int numeroJuegos;

        public List<ClsJugador> Jugadores 
        { get => jugadores; set => jugadores = value; }

        public string Nombre
        { get => nombre; set => nombre = value; }

        public int NumeroJuegos
        { get => numeroJuegos; set => numeroJuegos = value; }

        public ClsGrupo() { numeroJuegos = 1; }


        public ClsGrupo(string nombre)
        {
            this.nombre = nombre;
            numeroJuegos = 1;
        }

        /// <summary>
        /// Añade un jugador al grupo
        /// </summary>
        /// <param name="jugador"> Jugador que se añade</param>
        public void AddJugador(ClsJugador jugador)
        {
            jugadores.Add(jugador);
        }
    }
}
