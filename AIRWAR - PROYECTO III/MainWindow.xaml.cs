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
        
        public MainWindow()
        {
            InitializeComponent();

            Player player = new Player(Player); // Inicia Jugador
            gameLogic = new GameLogic(MyCanvas, player);  // Inicia Juego
            gameLogic.StartGame();

            MyCanvas.Focus();

            ImageBrush playerImage = new ImageBrush(); // Dibuja la imagen del jugador
            playerImage.ImageSource = new BitmapImage(new Uri("C:\\Users\\sofia\\source\\repos\\AIRWAR - PROYECTO III\\AIRWAR - PROYECTO III\\Imagen\\AntiAirCratf.png"));
            Player.Fill = playerImage;

        }
        private void Shoot(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(Player) + Player.Width/2);
                Canvas.SetTop(newBullet, Canvas.GetTop(Player) - newBullet.Height);

                MyCanvas.Children.Add(newBullet);
            }
        }
    }
}