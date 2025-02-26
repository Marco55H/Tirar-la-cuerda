using Ent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.ViewModels
{
    [QueryProperty(nameof(Jugador), "jugador")]
    internal class JuegoVM
    {
        #region Atributos
        private ClsJugador jugador;
        #endregion


        #region Propiedades        
        public ClsJugador Jugador
        {
            get { return jugador; }
            set
            {
                jugador = value;
            }
        }
        #endregion

        #region Contructor
        public JuegoVM()
        {
            jugador = new ClsJugador();
        }
        #endregion
    }
}
