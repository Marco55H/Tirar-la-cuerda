namespace Maui.Views 
{
    public partial class MainPage : ContentPage
    {
        public bool listo;
        public MainPage()
        {
            InitializeComponent();
            bool listo = false;
        }

        // Método que cambia el color del botón cuando se presiona
        private void OnButtonClicked(object sender, EventArgs e)
        {
            listo= !listo;
            var button = sender as Button;
            if (button != null)
            {
                // Cambia el color basado en el valor de 'Jugador.Listo'
                if (listo)
                {
                    button.BackgroundColor = Colors.Green;
                }
                else
                {
                    button.BackgroundColor = Colors.Red;
                }
                
            }
        }
    }
}
