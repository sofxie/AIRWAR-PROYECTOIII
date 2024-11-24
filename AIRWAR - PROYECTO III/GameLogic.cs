using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace AIRWAR___PROYECTO_III
{
    public class GameLogic
    {
        private DispatcherTimer timer;
        private Canvas gameCanvas;
        private double PSpeed = 5;
        public Player player;
        private int score = 0;
        private int time = 60;
        private Random rand = new Random();
        private int xAvion = 50;
        private int yAvion = 50;



        public event Action<int> Score; 
        public event Action<int> Time;   
        public event Action GameOver;
        public GameLogic(Canvas canvas, Player player)
        {
            gameCanvas = canvas;
            this.player = player;
        }
        public void StartGame()
        {
            SMovimiento();
            STimer();
        }
        private void SMovimiento()
        {
            var movementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // Aproximadamente 60 FPS
            };

            movementTimer.Tick += (s, e) =>
            {
                player.Move();
            };

            movementTimer.Start();
        }
        private void STimer()
        {
            var gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            gameTimer.Tick += (s, e) =>
            {
                if (time > 0)
                {
                    time--;
                    Time?.Invoke(time); // Notificar cambio en el tiempo
                    EnemigosSpawn(this.xAvion, this.yAvion);
                    this.xAvion += 1;
                    this.yAvion += 1;

                }
                else
                {
                    gameTimer.Stop();
                    GameOver?.Invoke(); // Notificar fin del juego
                }
            };


            gameTimer.Start();
        }
        public void HandleKeyPress(Key key, DateTime pressStartTime, DateTime pressEndTime)
        {
            if (key == Key.Space)
            {
                // Calcular duración del clic
                double pressDuration = (pressEndTime - pressStartTime).TotalMilliseconds;

                // Crear bala con velocidad basada en la duración del clic
                player.Shoot(pressDuration);
            }
        }
        private void EnemigosSpawn(int x, int y)
        {
            ImageBrush enemyBrush = new ImageBrush();
            enemyBrush.ImageSource = new BitmapImage(new Uri("C:\\Users\\ariel\\Source\\AIRWAR-PROYECTOIII\\AIRWAR - PROYECTO III\\Imagen\\Plane.png"));

            Rectangle newEnemy = new Rectangle
            {
                Tag = "Avion",
                Height = 50,
                Width = 56,
                Fill = enemyBrush
            };

            Canvas.SetTop(newEnemy, y);
            Canvas.SetLeft(newEnemy, x);
            gameCanvas.Children.Add(newEnemy);

        }
    }
}
