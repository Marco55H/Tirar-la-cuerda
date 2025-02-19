namespace Ent
{
    public class ClsJugador
    {
        #region atributos
        private string nombre="";

        private string grupo ="";

        private int puntuacion = 0;

        private bool listo=false;
        #endregion

        #region Propiedades
        public string Nombre {  get { return nombre; } set { nombre = value; } }
        public string Grupo { get { return grupo; } set { grupo = value; } }
        public int Puntuacion { get { return puntuacion; } set { puntuacion = value; } }
        public bool Listo { get { return listo; } set { listo = value; } }

        #endregion

        #region Constructores
        public ClsJugador() { }
        public ClsJugador(string _nombre, string _grupo, int _puntuacion, bool _listo)
        {
            this.nombre = _nombre;
            this.grupo = _grupo;
            this.puntuacion = _puntuacion;
            this.listo = _listo;
        }
        #endregion
    }
}
