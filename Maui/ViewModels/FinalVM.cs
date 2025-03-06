using Ent;
using Maui.ViewModels.Utility;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Maui.ViewModels
{
    [QueryProperty(nameof(Jugador), "jugador")]
    class FinalVM:INotifyPropertyChanged
    {
        #region Atributos
        private ClsJugador jugador;
        private string mensajeGanador;
        private readonly HubConnection _connection;
        private DelegateCommand cmdVolver;
        private DelegateCommand cmdRevancha;
        private string partidasJugadas;
        private int puntosJugador;
        private int puntosEnemigo;
        #endregion

        #region Propiedades 
        public ClsJugador Jugador
        {
            get { return jugador; }
            set
            {
                jugador = value;
                NotifyPropertyChanged("Jugador");
                //Cuando pilla el nombre del jugador, tambien buscaré el nombre del ganador
                buscarGanador();
            }
        }

        public int PuntosJugador
        {
            get { return puntosJugador; }
            set
            {
                puntosJugador = value;
                NotifyPropertyChanged("PuntosJugador");
            }
        }

        public int PuntosEnemigo
        {
            get { return puntosEnemigo; }
            set
            {
                puntosEnemigo = value;
                NotifyPropertyChanged("PuntosEnemigo");
            }
        }

        public string MensajeGanador
        {
            get { return mensajeGanador; }
            set
            {

                mensajeGanador = value;
                NotifyPropertyChanged("MensajeGanador");

            }
        }

        public string PartidasJugadas
        {
            get
            {
                return partidasJugadas;
            }
            set
            {
                partidasJugadas = value;
                NotifyPropertyChanged("PartidasJugadas");
            }
        }
        public DelegateCommand CmdVolver
        {
            get { return cmdVolver; }
        }

        public DelegateCommand CmdRevancha
        {
            get { return cmdRevancha; }
        }
        #endregion

        #region Contructor
        public FinalVM()
        {
            _connection = new HubConnectionBuilder().WithUrl("https://localhost:7163/hubCuerda").Build();

            _connection.On<string, int ,int>("nombreGanador", nombreGanadorEncontrado);
            _connection.On("IniciarJuego", empezar);
            _connection.On<int>("partidasJugadas", partidasjugadas);

            // Conectarse y suscribirse
            esperarConexion();


            partidasJugadas = "Partida numero "+0;
            cmdVolver = new DelegateCommand(cmdVolver_Execute, true);
            cmdRevancha = new DelegateCommand(cmdRevancha_Execute, true);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Metodo que me mandará a la ventana de inicio
        /// </summary>
        private async void cmdVolver_Execute()
        {
            await _connection.InvokeCoreAsync("LeaveGroup", args:
            new[]
                {
                jugador.Grupo,
                jugador.Nombre
                }
            );
            
            await Shell.Current.GoToAsync($"//MainPage?jugador={jugador.Nombre}");
        }

        /// <summary>
        /// Metodo que inicia la partida, me mandará a la ventana de la partida en forma de revancha
        /// </summary>
        private async void cmdRevancha_Execute()
        {
            await _connection.InvokeCoreAsync("Revancha", args:
            new[]
                 {
                    jugador.Grupo,
                    jugador.Nombre
                 }
            );
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Metodo asincrono que busca el ganador del grupo
        /// </summary>
        /// <returns></returns>
        private async Task buscarGanador()
            {
                await _connection.InvokeCoreAsync("nombreGanador", args:
                new[]
                    {
                        jugador.Grupo,
                    }
                );
            }

        /// <summary>
        /// El Hub devuelve el nombre del ganador para ponerlo en al UI, ademas de la puntuación de cada uno, si pierde se modificaran los puntos del enemigo, 
        /// si gana los tuyos, pero envio los puntos de los dos jugadores ya que si envio uno tengo que ver cual es y demas
        /// </summary>
        private void nombreGanadorEncontrado(string nombre, int puntos1, int puntos2)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if(nombre == jugador.Nombre)
                {
                    mensajeGanador = "Enhorabuena, ganaste la partida!!!";                    
                    puntosJugador = puntos2 / 2 ;
                    puntosEnemigo = puntos1 / 2 ;
                }
                else
                {
                    mensajeGanador = "Te ganó " + nombre + " más suerte la próxima vez";
                    puntosJugador = puntos1 / 2 ;
                    puntosEnemigo = puntos2 / 2 ;
                }

                //Si el jugador es igual al nombre devuelto, los puntos del jugador serán los del jugador 1 del HUB, los puntos del enemigo serán los del jugador 2 del HUB,
                //entre dos , ya que cuando son dos jugadores, llamo dos veces a sumar puntuacion en el HUB
                

                NotifyPropertyChanged("PuntosJugador");
                NotifyPropertyChanged("PuntosEnemigo");
                NotifyPropertyChanged("MensajeGanador");
            });
        }

        /// <summary>
        /// Metodo que me mostrará las partidas jugadas
        /// </summary>
        /// <param name="_partidasJugadas"></param>
        private void partidasjugadas(int _partidasJugadas)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                partidasJugadas="Partida numero : "+_partidasJugadas;
                NotifyPropertyChanged("PartidasJugadas");
            });
        }

        /// <summary>
        /// Metodo que inicia la partida, me mandará a la ventana de la partida
        /// </summary>
        private async void empezar()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var diccionario = new Dictionary<string, object>
                {
                {"jugador", jugador}
                };

                await Shell.Current.GoToAsync("///JuegoView", diccionario);
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
