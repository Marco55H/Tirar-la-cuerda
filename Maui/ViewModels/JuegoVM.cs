using Ent;
using Maui.ViewModels.Utility;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Maui.ViewModels
{
    [QueryProperty(nameof(Jugador), "jugador")]
    internal class JuegoVM:INotifyPropertyChanged
    {
        #region Atributos
        private ClsJugador jugador;
        private readonly HubConnection _connection;
        private string nombreEnemigo;
        private DelegateCommand cmdTirarCuerda;
        private int puntuacionMaxima;
        #endregion

        #region Propiedades  
        
        public int PuntuacionMaxima
        {
            get { return puntuacionMaxima; }
        }

        public ClsJugador Jugador
        {
            get { return jugador; }
            set
            {
                jugador = value;
                NotifyPropertyChanged("Jugador");
                //Cuando pilla el nombre del jugador, tambien buscaré el nombre del enemigo
                buscarNombreEnemigo();
            }
        }

        public string NombreEnemigo
        {
            get { return nombreEnemigo; }
        }

        public DelegateCommand CmdTirarCuerda
        {
            get { return cmdTirarCuerda; }
        }

        #endregion

        #region Contructor
        public JuegoVM()
        {
            _connection = new HubConnectionBuilder().WithUrl("https://tirarlacuerda.azurewebsites.net/hubCuerda").Build();

            // Conectarse y suscribirse
            esperarConexion();

            _connection.On<string>("nombreEnemigo", nombreEnemigoEncontrado);
            _connection.On<ClsJugador, ClsJugador>("tirarCuerda", calculaPuntos);

            cmdTirarCuerda = new DelegateCommand(cmdTirarCuerda_Execute, true);

            puntuacionMaxima = 136;
        }

        #endregion

        #region Commands
        /// <summary>
        /// Ver cuando se puede tirar la cuerda
        /// </summary>
        /// <returns></returns>
        private bool cmdTirarCuerda_CanExecute()
        {
            bool sePuede = false;
            return true;
        }

        /// <summary>
        /// Cuando pulso en el boton le digo a el Hub que jugador de  que grupo está tirando la cuerda
        /// </summary>
        private async void cmdTirarCuerda_Execute()
        {
            await _connection.InvokeCoreAsync("tirarCuerda", args:
            new[]
                {
                    jugador.Grupo,
                    jugador.Nombre
                }
            );
        }

        #endregion

        #region Metodos
        /// <summary>
        /// Metodo asincrono que busca el nombre del enemigo
        /// </summary>
        /// <returns></returns>
        private async Task buscarNombreEnemigo()
        {
            await _connection.InvokeCoreAsync("nombreEnemigo", args:
            new[]
                {
                    jugador.Grupo,
                    jugador.Nombre
                }
            );
        }

        /// <summary>
        /// El Hub devuelve el nombre del enemigo para ponerlo en al UI
        /// </summary>
        private void nombreEnemigoEncontrado(string nombre)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                nombreEnemigo = nombre;
                NotifyPropertyChanged("NombreEnemigo");
            });
        }

        /// <summary>
        /// Metodo que calcula los puntos de los jugadores
        /// </summary>
        /// <param name="Jugador1">El jugador 1 que nos devuelve el HUB</param>
        /// <param name="Jugador2">E jugador 2 que nos devuelve el HUB</param>
        private void calculaPuntos(ClsJugador Jugador1, ClsJugador Jugador2)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //Si el nombre del jugador acutal es igual al nombre del jugador 1 del HUB, los puntos del jugador actual serán los del jugador 1 del HUB,
                //los puntos del jugador 2 serán los del enemigo
                if (Jugador1.Nombre == jugador.Nombre)
                {
                    jugador.Puntuacion = Jugador1.Puntuacion;
                    NotifyPropertyChanged("Jugador");
                }
                //Si el nombre del jugador acutal es igual al nombre del jugador 2 del HUB, los puntos del jugador actual serán los del jugador 2 del HUB,
                //los puntos del jugador 1 serán los del enemigo
                else
                {
                    jugador.Puntuacion = Jugador2.Puntuacion;
                    NotifyPropertyChanged("Jugador");
                }

                //Comprobar para ver que un jugador no ha ganado o perdido, es decir que si la partida ha terminado
                if (jugador.Puntuacion >= puntuacionMaxima || jugador.Puntuacion <= -puntuacionMaxima)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        jugador.Puntuacion = 0;
                        await Shell.Current.GoToAsync("///FinalView", true, new Dictionary<string, object>
                        {
                            { "jugador", jugador }
                        });
                    });
                }
            });
        }

        /// <summary>
        /// Metodo asincrono que inicia la conexion con el hub del servidor
        /// </summary>
        /// <returns></returns>
        private async Task esperarConexion()
        {         
            await _connection.StartAsync();
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
