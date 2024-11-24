using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace AIRWAR___PROYECTO_III
{
    public partial class MainWindow : Window
    {
        private GameLogic gameLogic; // Clase aparte para estructurar la logica
        private DateTime SpaceTime; // Tiempo de inicio al presionar clic
        private bool ClickSpace = false; // El click esta presionado

        public MainWindow()
        {
            InitializeComponent();

            Player player = new Player(Player, MyCanvas); // Inicia Jugador
            gameLogic = new GameLogic(MyCanvas, player);  // Inicia Juego
            gameLogic.StartGame();

            MyCanvas.Focus();

            ImageBrush playerImage = new ImageBrush(); // Dibuja la imagen del jugador
            playerImage.ImageSource = new BitmapImage(new Uri("C:\\Users\\ariel\\Source\\AIRWAR-PROYECTOIII\\AIRWAR - PROYECTO III\\Imagen\\AntiAirCratf.png"));// Hay que cambiar la ruta
            Player.Fill = playerImage;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Detectar inicio de disparo
            if (e.Key == Key.Space && !ClickSpace)
            {
                SpaceTime = DateTime.Now;
                ClickSpace = true;
            }
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // Detectar disparo al soltar Space
            if (e.Key == Key.Space && ClickSpace)
            {
                TimeSpan pressDuration = DateTime.Now - SpaceTime;

                gameLogic.player.Shoot(pressDuration.TotalMilliseconds);
                ClickSpace = false;
            } 
        }
    }
}
