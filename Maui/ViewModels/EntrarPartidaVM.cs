using Ent;
using Maui.ViewModels.Utility;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.ViewModels
{
    [QueryProperty(nameof(Nombre), "jugador")]
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
        private bool estaEnGrupo;
        private bool listo;
        private bool repetidoOlleno;
        #endregion

        #region Propiedades 
        public bool Listo
        {
            get { return listo; }
            set
            {
                listo = value;
                NotifyPropertyChanged("Listo");
            }
        }

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
                if (estaEnGrupo != value)
                {
                    estaEnGrupo = value;
                    NotifyPropertyChanged("EstaEnGrupo");

                    cmdSalirGrupo.RaiseCanExecuteChanged();
                    cmdPreparado.RaiseCanExecuteChanged();
                    cmdUnirGrupo.RaiseCanExecuteChanged();
                }
            }
        }
        public bool RepetidoOlleno
        {
            get { return repetidoOlleno; }
            set
            {
                repetidoOlleno = value;
                NotifyPropertyChanged("RepetidoOlleno");
            }
        }
        public string Grupo
        {

            get { return jugador.Grupo; }

            set
            {
                jugador.Grupo = value;
                repetidoOlleno = false;
                NotifyPropertyChanged("RepetidoOlleno"); 
                estaEnGrupo = false;
                NotifyPropertyChanged("EstaEnGrupo");
                NotifyPropertyChanged("Jugador");
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
                repetidoOlleno = false;
                NotifyPropertyChanged("RepetidoOlleno");
                estaEnGrupo = false;
                NotifyPropertyChanged("EstaEnGrupo");
                NotifyPropertyChanged("Jugador");
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
            _connection = new HubConnectionBuilder().WithUrl("https://tirarlacuerda.azurewebsites.net/hubCuerda").Build();

            _connection.On<ClsJugador, ClsJugador>("jugadoresDelGrupo", verNombres);
            _connection.On("GrupoLleno", grupoLleno);
            _connection.On("NombreRepetido", nombreRepetido);
            _connection.On("IniciarJuego", empezar);

            // Esperar a que se conecte
            esperarConexion();

            EstaEnGrupo = false;
            cmdUnirGrupo = new DelegateCommand(cmdUnirGrupo_Execute, cmdUnirGrupo_CanExecute);
            cmdSalirGrupo = new DelegateCommand(cmdSalirGrupo_Execute,()=> EstaEnGrupo && !RepetidoOlleno && !cmdUnirGrupo_CanExecute());
            cmdPreparado = new DelegateCommand(cmdPreparado_Execute, () => EstaEnGrupo && !RepetidoOlleno && !cmdUnirGrupo_CanExecute());

            listo=false;
            jugador = new ClsJugador();
            jugadores = new ObservableCollection<ClsJugador>();
        }

        #endregion

        #region Commands
        // Comprobar si se puede ejecutar el comando para unirse al grupo
        private bool cmdUnirGrupo_CanExecute()
        {
            bool sePuedeEjecutar = false;

            if (!string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(Grupo)) // Si el nombre y el grupo no están vacíos
            {
                // Permitir unirse si no está en el grupo y el grupo no está lleno
                if (!EstaEnGrupo && !RepetidoOlleno)
                {
                    sePuedeEjecutar = true;
                }
                // Si el nombre está repetido, el botón sigue habilitado para poder cambiar el nombre y unirse nuevamente
                else if (!EstaEnGrupo && RepetidoOlleno)
                {
                    sePuedeEjecutar = true; // Permitir unirse aunque esté repetido para poder cambiar el nombre
                }
            }

            return sePuedeEjecutar;
        }

        // Ejecutar el comando para unirse al grupo
        private async void cmdUnirGrupo_Execute()
        {
            if (!EstaEnGrupo && !RepetidoOlleno) // Solo ejecutar si no está en el grupo y el grupo no está lleno
            {
                await _connection.InvokeCoreAsync("JoinGroup", args:
                new[]
                {
            jugador.Grupo,
            jugador.Nombre
                });

                EstaEnGrupo = true;
                NotifyPropertyChanged("EstaEnGrupo");

                RepetidoOlleno = false; // Resetear el estado de lleno/repetido al unirse
                NotifyPropertyChanged("RepetidoOlleno");
            }
        }

        // Ejecutar el comando, si se puede ejecutar, se envía al servidor que el jugador está preparado
        private async void cmdPreparado_Execute()
        {
            if (!RepetidoOlleno && EstaEnGrupo) // Solo si el grupo no está repetido o lleno y el jugador está en un grupo
            {
                await _connection.InvokeCoreAsync("Preparado", args:
                new[]
                {
            jugador.Grupo,
            jugador.Nombre
                });

                listo = !listo; // Cambiar el estado de 'listo'
                NotifyPropertyChanged("Listo");
            }
        }

        // Ejecutar el comando para salir del grupo
        private async void cmdSalirGrupo_Execute()
        {
            if (EstaEnGrupo && !RepetidoOlleno) // Solo si está en un grupo y el grupo no está repetido o lleno
            {
                await _connection.InvokeCoreAsync("LeaveGroup", args:
                new[]
                {
            jugador.Grupo,
            jugador.Nombre
                });

                EstaEnGrupo = false;
                NotifyPropertyChanged("EstaEnGrupo");

                // Resetear estado
                listo = false;
                Jugador.Listo = false;

                NotifyPropertyChanged("Listo");
                NotifyPropertyChanged("LlenoORepetido");
            }
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
                llenoORepetido = "Grupo lleno, Prueba otro o Espera";
                NotifyPropertyChanged("LlenoORepetido");

                repetidoOlleno = true;
                NotifyPropertyChanged("RepetidoOlleno");
            });
        }

        /// <summary>
        /// El Hub avisa de que el grupo esta lleno, cuando se quiere introducir un nombre identico a otro, se muestra el mensaje
        /// </summary>
        private void nombreRepetido()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                llenoORepetido = "Nombre Repetido, Modificalo";
                NotifyPropertyChanged("LlenoORepetido");

                repetidoOlleno = true;
                NotifyPropertyChanged("RepetidoOlleno");
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
                EstaEnGrupo = false;
                listo = false;
                NotifyPropertyChanged("Listo");
                NotifyPropertyChanged("EstaEnGrupo");
                cmdUnirGrupo.RaiseCanExecuteChanged();

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
