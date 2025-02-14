using Maui.ViewModels.Utility;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.ViewModels
{
    public class EntrarPartidaVM : INotifyPropertyChanged
    {
        #region Atributos
        private string grupo;
        private string nombre;
        private List<string> jugadores;
        private readonly HubConnection _connection;
        private DelegateCommand cmdUnirGrupo;
        #endregion


        #region Propiedades 
        public string Grupo
        {

            get { return grupo; }

            set
            {
                grupo = value;
                cmdUnirGrupo.RaiseCanExecuteChanged();
            }

        }

        public string Nombre
        {

            get { return nombre; }

            set
            {
                nombre = value;
                cmdUnirGrupo.RaiseCanExecuteChanged();
            }

        }

        public List<string> Jugadores
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

            _connection.On<string, string>("añadeJugador", verNombres);

            // Esperar a que se conecte
            esperarConexion();

            cmdUnirGrupo = new DelegateCommand(cmdUnirGrupo_Execute, cmdUnirGrupo_CanExecute);

            nombre = "";
            grupo = "";
            jugadores = new List<string>();
        }
        #endregion


        #region Métodos

        private void verNombres(String nombre1, String nombre2)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                jugadores.Add(nombre1);
                jugadores.Add(nombre2);
                NotifyPropertyChanged("Jugadores");
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

        //Comprobar si se puede ejecutar el comando, si no hay nada vacio, se puede ejecutar
        private bool cmdUnirGrupo_CanExecute()
        {
            bool sePuedeEjecutar = true;

            if(string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(grupo))
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
                grupo,
                nombre
                }
            );
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
