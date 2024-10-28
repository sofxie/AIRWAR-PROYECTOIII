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

namespace AIRWAR___PROYECTO_III
{
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int speed = 7; // Velocidad del Movimiento del Jugador

        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            
            timer.Tick += GameTimerEvent; // Timer del Juego
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
        }

        private void GameTimerEvent(object sender, EventArgs e) 
        {
            Canvas.SetLeft(Player, Canvas.GetLeft(Player) - speed);
            if (Canvas.GetLeft(Player) < 5 || Canvas.GetLeft(Player) + (Player.Width * 2) > Application.Current.MainWindow.Width)
            {
                speed = -speed;
            }
        }
    }
}