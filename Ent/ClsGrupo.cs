﻿using System;
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

        public List<ClsJugador> Jugadores 
        { get => jugadores; set => jugadores = value; }

        public string Nombre
        { get => nombre; set => nombre = value; }

        public ClsGrupo() { }


        public ClsGrupo(string nombre)
        {
            this.nombre = nombre;
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
