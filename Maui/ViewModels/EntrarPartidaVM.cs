using Ent;
using Maui.ViewModels.Utility;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.ViewModels
{
    public class EntrarPartidaVM : INotifyPropertyChanged
    {
        #region Atributos
        private ClsJugador jugador;
        private ObservableCollection<ClsJugador> jugadores;
        private readonly HubConnection _connection;
        private DelegateCommand cmdUnirGrupo;
        private DelegateCommand cmdPreparado;
        private string llenoORepetido;
        #endregion

        #region Propiedades 
        public string Grupo
        {

            get { return jugador.Grupo; }

            set
            {
                jugador.Grupo = value;
                cmdUnirGrupo.RaiseCanExecuteChanged();
            }

        }

        public string LlenoORepetido
        {
            get { return llenoORepetido; }
        }

        public string Nombre
        {

            get { return jugador.Nombre; }

            set
            {
                jugador.Nombre = value;
                cmdUnirGrupo.RaiseCanExecuteChanged();
            }

        }

        public ObservableCollection<ClsJugador> Jugadores
        {
            get { return jugadores; }
        }

        public DelegateCommand CmdUnirGrupo
        {

            get { return cmdUnirGrupo; }

        }
        #endregion

        #region Constructores
        public EntrarPartidaVM()
        {
            //Conectar con URL del servidor
            _connection = new HubConnectionBuilder().WithUrl("https://localhost:7163/hubCuerda").Build();

            _connection.On<ClsJugador, ClsJugador>("añadeJugador", verNombres);
            _connection.On("GrupoLleno", grupoLleno);
            _connection.On("NombreRepetido", nombreRepetido);

            // Esperar a que se conecte
            esperarConexion();

            cmdUnirGrupo = new DelegateCommand(cmdUnirGrupo_Execute, cmdUnirGrupo_CanExecute);

            jugador = new ClsJugador();
            jugadores = new ObservableCollection<ClsJugador>();
        }
        #endregion

        #region Commands
        //Comprobar si se puede ejecutar el comando, si no hay nada vacio, se puede ejecutar
        private bool cmdUnirGrupo_CanExecute()
        {
            bool sePuedeEjecutar = true;

            if (string.IsNullOrEmpty(jugador.Nombre) || string.IsNullOrEmpty(jugador.Grupo))
            {
                sePuedeEjecutar = false;
            }

            return sePuedeEjecutar;
        }

        //Ejecutar el comando, añadimos al grupo y comienza a esperar a que empiece la partida
        private async void cmdUnirGrupo_Execute()
        {
            
            await _connection.InvokeCoreAsync("JoinGroup", args:
            new[]
                {
                jugador.Grupo,
                jugador.Nombre
                }
            );
        }
        #endregion

        #region Métodos
        private void grupoLleno()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                llenoORepetido = "Grupo lleno";
                NotifyPropertyChanged("LlenoORepetido");
            });
        }

        private void nombreRepetido()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                llenoORepetido = "Nombre Repetido";
                NotifyPropertyChanged("LlenoORepetido");
            });
        }

        /// <summary>
        /// Metodo que añade los jugadores a la lista de jugadores
        /// </summary>
        /// <param name="jugador1">1 jugador</param>
        /// <param name="jugador2">2 jugador</param>
        private void verNombres(ClsJugador jugador1, ClsJugador jugador2)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                jugadores.Clear();
                jugadores.Add(jugador1);
                jugadores.Add(jugador2);
                llenoORepetido = "";
                NotifyPropertyChanged("LlenoORepetido");
            });
        }

        /// <summary>
        /// Metodo asincrono que inicia la conexion con el hub del servidor
        /// </summary>
        /// <returns></returns>
        private async Task esperarConexion()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _connection.StartAsync();
            });
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
