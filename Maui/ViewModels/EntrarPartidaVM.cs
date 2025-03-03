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
        private DelegateCommand cmdSalirGrupo;
        private DelegateCommand cmdPreparado;
        private string llenoORepetido;
        private List<ClsGrupo> grupos;
        private bool estaEnGrupo;
        #endregion

        #region Propiedades 

        public ClsJugador Jugador
        {
            get { return jugador; }
            set
            {
                jugador = value;
                NotifyPropertyChanged("Jugador");
            }
        }

        public bool EstaEnGrupo
        {
            get { return estaEnGrupo; }
            set
            {
                estaEnGrupo = value;
                NotifyPropertyChanged("EstaEnGrupo");
                cmdSalirGrupo.RaiseCanExecuteChanged(); // Actualiza el estado del botón de salir
                cmdPreparado.RaiseCanExecuteChanged(); // Actualiza el estado del botón de estar Preparado
            }
        }

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
        public DelegateCommand CmdSalirGrupo
        {

            get { return cmdSalirGrupo; }

        }
        public DelegateCommand CmdPreparado

        {
            get { return cmdPreparado; }
        }
        #endregion

        #region Constructores
        public EntrarPartidaVM()
        {
            //Conectar con URL del servidor
            _connection = new HubConnectionBuilder().WithUrl("https://localhost:7163/hubCuerda").Build();

            _connection.On<ClsJugador, ClsJugador>("jugadoresDelGrupo", verNombres);
            _connection.On("GrupoLleno", grupoLleno);
            _connection.On("NombreRepetido", nombreRepetido);
            _connection.On("IniciarJuego", empezar);

            // Esperar a que se conecte
            esperarConexion();

            estaEnGrupo = false;
            cmdUnirGrupo = new DelegateCommand(cmdUnirGrupo_Execute, cmdUnirGrupo_CanExecute);
            cmdSalirGrupo = new DelegateCommand(cmdSalirGrupo_Execute, () => EstaEnGrupo);
            cmdPreparado = new DelegateCommand(cmdPreparado_Execute, () => EstaEnGrupo);

            grupos = new List<ClsGrupo>();
            jugador = new ClsJugador();
            jugadores = new ObservableCollection<ClsJugador>();
        }

        #endregion

        #region Commands
        //Ejecutar el comando, si se puede ejecutar, se envia al servidor que el jugador esta preparado
        private async void cmdPreparado_Execute()
        {
           await _connection.InvokeCoreAsync("Preparado", args:
           new[]
                {
                jugador.Grupo,
                jugador.Nombre
                }
            );
        }

        //Comprobar si se puede ejecutar el comando, si no hay nada vacio, se puede ejecutar
        private bool cmdUnirGrupo_CanExecute()
        {
            bool sePuedeEjecutar = false;
            if ( !string.IsNullOrEmpty(jugador.Nombre) && !string.IsNullOrEmpty(jugador.Grupo))
            {

                sePuedeEjecutar = true;
                
            }
            return sePuedeEjecutar;
        }

        //Ejecutar el comando, Salimos del grupo
        private async void cmdSalirGrupo_Execute()
        {
            await _connection.InvokeCoreAsync("LeaveGroup", args:
            new[]
                {
                jugador.Grupo,
                jugador.Nombre
                }
            );
            EstaEnGrupo = false;
        }

        //Ejecutar el comando, añadimos al grupoa
        private async void cmdUnirGrupo_Execute()
        {
            await _connection.InvokeCoreAsync("JoinGroup", args:
            new[]
                {
                jugador.Grupo,
                jugador.Nombre
                }
            );
            EstaEnGrupo = true;
        }
        #endregion

        #region Métodos

        /// <summary>
        /// El Hub avisa de que el grupo esta lleno, cuando los dos nombres estan llenos, se muestra el mensaje
        /// </summary>
        private void grupoLleno()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                llenoORepetido = "Grupo lleno";
                NotifyPropertyChanged("LlenoORepetido");
            });
        }

        /// <summary>
        /// El Hub avisa de que el grupo esta lleno, cuando se quiere introducir un nombre identico a otro, se muestra el mensaje
        /// </summary>
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
