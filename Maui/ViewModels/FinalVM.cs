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

        public string MensajeGanador
        {
            get { return mensajeGanador; }
            set
            {

                mensajeGanador = value;
                NotifyPropertyChanged("MensajeGanador");

            }
        }

        public DelegateCommand CmdVolver
        {
            get { return cmdVolver; }
        }
        #endregion

        #region Contructor
        public FinalVM()
        {
            _connection = new HubConnectionBuilder().WithUrl("https://localhost:7163/hubCuerda").Build();

            _connection.On<string>("nombreGanador", nombreGanadorEncontrado);

            // Conectarse y suscribirse
            esperarConexion();

            cmdVolver = new DelegateCommand(cmdVolver_Execute, true);
        }
        #endregion

        #region Commands
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
        /// El Hub devuelve el nombre del ganador para ponerlo en al UI
        /// </summary>
        private void nombreGanadorEncontrado(string nombre)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if(nombre == jugador.Nombre)
                {
                    mensajeGanador = "Enhorabuena, ganaste la partida!!!";
                }
                else
                {
                    mensajeGanador = "Te ganó " + nombre + " más suerte la próxima vez";
                }
                NotifyPropertyChanged("MensajeGanador");
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
