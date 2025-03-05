namespace Ent
{
    [Serializable]
    public class ClsJugador
    {
        #region atributos
        private string nombre;

        private string grupo;

        private int puntuacion;

        private bool listo;

        private int victorias;
        #endregion

        #region Propiedades
        public string Nombre {  get { return nombre; } set { nombre = value; } }
        public string Grupo { get { return grupo; } set { grupo = value; } }
        public int Puntuacion { get { return puntuacion; } set { puntuacion = value; } }
        public bool Listo { get { return listo; } set { listo = value; } }
        public int Victorias { get { return victorias; } set { victorias = value; } }

        #endregion

        #region Constructores
        public ClsJugador() 
        {
            this.nombre = "";
            this.grupo = "";
            this.puntuacion = 0;
            this.listo = false;
            this.victorias = 0;
        }
        public ClsJugador(string _nombre, string _grupo, int _puntuacion, bool _listo, int _victorias)
        {
            this.nombre = _nombre;
            this.grupo = _grupo;
            this.puntuacion = _puntuacion;
            this.listo = _listo;
            this.victorias = _victorias;
        }
        #endregion
    }
}
