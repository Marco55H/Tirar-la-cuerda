namespace Maui.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Método que cambia el color del botón cuando se presiona
        private void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Alterna entre rojo y verde
                button.BackgroundColor = button.BackgroundColor == Colors.Red ? Colors.Green : Colors.Red;
            }
        }
    }
}

