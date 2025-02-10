using Maui.ViewModels.Utility;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.ViewModels
{
    internal class EntrarPartidaVM : INotifyPropertyChanged
    {
        #region Atributos
        private string grupo;
        private string nombre;
        private readonly HubConnection _connection;
        private DelegateCommand cmdConfirmarParticipacion;
        #endregion


        #region Propiedades 
        public string Grupo
        {

            get { return grupo; }

            set
            {
                grupo = value;
                NotifyPropertyChanged("Grupo");
            }

        }

        public string Nombre
        {

            get { return nombre; }

            set
            {
                nombre = value;
                NotifyPropertyChanged("Nombre");
            }

        }

        public DelegateCommand CmdConfirmarParticipacion
        {

            get { return cmdConfirmarParticipacion; }

        }
        #endregion


        #region Constructores
        EntrarPartidaVM()
        {
            _connection = new HubConnectionBuilder().WithUrl(new Uri("https://localhost:7135/chathub")).Build();

            esperarConexion();

            cmdConfirmarParticipacion = new DelegateCommand(cmd_Execute, cmd_CanExecute);
        }
        #endregion


        #region Métodos
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
