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
        private GameLogic gameLogic;
        private Grafo graph;
        private DateTime SpaceTime;
        private bool ClickSpace = false;
        private Label scoreLabel;  // Add a reference to the score label

        public MainWindow()
        {
            InitializeComponent();

            var airportPositions = new List<(double X, double Y)> { (130, 50), (350, 150), (170, 450) };
            var carrierPositions = new List<(double X, double Y)> { (450, 70), (30, 330), (400, 350) };

            Player player = new Player(Player, MyCanvas);
            graph = new Grafo();
            gameLogic = new GameLogic(MyCanvas, player, airportPositions, carrierPositions, lblTimer);
            gameLogic.StartGame();

            player.ScoreUpdated += UpdateScore;  // Subscribe to the ScoreUpdated event

            MyCanvas.Focus();

            graph.PlaceNodesManually(airportPositions, carrierPositions, MyCanvas);
            graph.GenerateRandomRoutes(0.5);

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("C:\\Users\\ariel\\Source\\AIRWAR-PROYECTOIII\\AIRWAR - PROYECTO III\\Imagen\\AntiAirCratf.png"));
            Player.Fill = playerImage;

            scoreLabel = new Label();  // Initialize the score label
            scoreLabel.Content = "Score: 0";  // Initial score display
            scoreLabel.HorizontalAlignment = HorizontalAlignment.Left;
            scoreLabel.VerticalAlignment = VerticalAlignment.Top;
            scoreLabel.Margin = new Thickness(10, 10, 0, 0);
            MyCanvas.Children.Add(scoreLabel);
        }

        private void UpdateScore(int newScore)
        {
            scoreLabel.Content = "Score: " + newScore.ToString();  // Update the label with the new score
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !ClickSpace)
            {
                SpaceTime = DateTime.Now;
                ClickSpace = true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && ClickSpace)
            {
                TimeSpan pressDuration = DateTime.Now - SpaceTime;

                gameLogic.player.Shoot(pressDuration.TotalMilliseconds, gameLogic.GetEnemigos());
                ClickSpace = false;
            }
        }
    }

}
