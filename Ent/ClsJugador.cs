namespace Ent
{
    public class ClsJugador
    {
        #region atributos
        private string nombre="";

        private string grupo ="";

        private int puntuacion = 0;
        #endregion

        #region Propiedades
        public string Nombre {  get { return nombre; } set { nombre = value; } }
        public string Grupo { get { return grupo; } set { grupo = value; } }
        public int Puntuacion { get { return puntuacion; } set { puntuacion = value; } }
        #endregion

        #region Constructores
        public ClsJugador() { }
        public ClsJugador(string _nombre, string _grupo, int _puntuacion)
        {
            this.nombre = _nombre;
            this.grupo = _grupo;
            this.puntuacion = _puntuacion;
        }
        #endregion
    }
}
