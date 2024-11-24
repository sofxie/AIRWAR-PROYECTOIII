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
        private double PSpeed = 2; // Velocidad de movimiento de los enemigos
        public Player player;
        private int score = 0;
        private int time = 60;
        private Random rand = new Random();
        private int xAvion = 50;
        private int yAvion = 50;

        private List<(double X, double Y)> airportPositions;
        private List<(double X, double Y)> carrierPositions;
        private List<Enemy> enemigos = new List<Enemy>(); // Almacenar los enemigos

        // Limitar el número de aviones por aeropuerto y portaavión
        private Dictionary<(double X, double Y), int> airportAircraftCount = new Dictionary<(double X, double Y), int>();
        private Dictionary<(double X, double Y), int> carrierAircraftCount = new Dictionary<(double X, double Y), int>();

        public event Action<int> Score;
        public event Action<int> Time;
        public event Action GameOver;

        public GameLogic(Canvas canvas, Player player, List<(double X, double Y)> airportPositions, List<(double X, double Y)> carrierPositions)
        {
            gameCanvas = canvas;
            this.player = player;
            this.airportPositions = airportPositions;
            this.carrierPositions = carrierPositions;

            // Inicializamos los contadores de aviones para aeropuertos y portaaviones
            foreach (var pos in airportPositions)
            {
                airportAircraftCount[pos] = 0; // No hay aviones iniciales
            }

            foreach (var pos in carrierPositions)
            {
                carrierAircraftCount[pos] = 0; // No hay aviones iniciales
            }
        }

        public void StartGame()
        {
            SMovimiento();
            STimer();
            MoverAviones(); // Iniciar el movimiento de los aviones
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
                    CrearEnemigosDesdePosiciones();
                }
                else
                {
                    gameTimer.Stop();
                    GameOver?.Invoke(); // Notificar fin del juego
                }
            };

            gameTimer.Start();
        }

        // Método en GameLogic para iniciar el disparo
        public void HandleKeyPress(Key key, DateTime pressStartTime, DateTime pressEndTime)
        {
            if (key == Key.Space)
            {
                // Calcular duración del clic
                double pressDuration = (pressEndTime - pressStartTime).TotalMilliseconds;

                // Crear bala y detectar colisión, pasando la lista de enemigos
                player.Shoot(pressDuration, enemigos); // Pasamos la lista de enemigos al método Shoot
            }
        }

        private void CrearEnemigosDesdePosiciones()
        {
            // Crear enemigos para cada tupla en airportPositions solo si no se ha alcanzado el límite de 4 aviones
            foreach (var posicion in airportPositions)
            {
                if (airportAircraftCount[posicion] < 4)
                {
                    EnemigosSpawn(posicion.X, posicion.Y);
                    airportAircraftCount[posicion]++; // Incrementar el contador de aviones del aeropuerto
                }
            }

            // Crear enemigos para cada tupla en carrierPositions solo si no se ha alcanzado el límite de 2 aviones
            foreach (var posicion in carrierPositions)
            {
                if (carrierAircraftCount[posicion] < 2)
                {
                    EnemigosSpawn(posicion.X, posicion.Y);
                    carrierAircraftCount[posicion]++; // Incrementar el contador de aviones del portaavión
                }
            }
        }

        private void EnemigosSpawn(double x, double y)
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

            Canvas.SetTop(newEnemy, y + 10);
            Canvas.SetLeft(newEnemy, x + 10);
            gameCanvas.Children.Add(newEnemy);

            // Crear objeto Enemy y agregarlo a la lista de enemigos
            enemigos.Add(new Enemy(newEnemy, x, y));
        }

        // Función que hace que los aviones se muevan de forma continua entre los aeropuertos y portaviones
        private void MoverAviones()
        {
            var movimientoAvionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // El intervalo para actualizar las posiciones de los aviones
            };

            movimientoAvionTimer.Tick += (s, e) =>
            {
                foreach (var enemigo in enemigos)
                {
                    MoverAvion(enemigo); // Mover cada avión
                }
            };

            movimientoAvionTimer.Start();
        }

        // Mover un avión hacia su destino
        private void MoverAvion(Enemy enemigo)
        {
            // Calcular la distancia total entre la posición actual y el destino
            double distancia = Math.Sqrt(Math.Pow(enemigo.DestinoX - enemigo.X, 2) + Math.Pow(enemigo.DestinoY - enemigo.Y, 2));

            // Si la distancia es muy pequeña (el avión ha llegado), asignamos un nuevo destino
            if (distancia < 2)
            {
                // Elegir un nuevo destino de manera aleatoria (aeropuerto o portaavión)
                var destino = rand.Next(0, 2) == 0 ? airportPositions : carrierPositions;
                var nuevaPosicion = destino[rand.Next(0, destino.Count)];

                enemigo.DestinoX = nuevaPosicion.X;
                enemigo.DestinoY = nuevaPosicion.Y;

                // Volver a calcular la distancia al nuevo destino
                distancia = Math.Sqrt(Math.Pow(enemigo.DestinoX - enemigo.X, 2) + Math.Pow(enemigo.DestinoY - enemigo.Y, 2));

                // Si el enemigo llega a su nuevo destino, empieza el tiempo de detención
                enemigo.StartDetentionTime();  // Invoca el tiempo de detención
            }

            // Si el enemigo no está invencible, mueve el enemigo hacia su destino
            if (!enemigo.IsInvincible)
            {
                double stepX = (enemigo.DestinoX - enemigo.X) * PSpeed / distancia;
                double stepY = (enemigo.DestinoY - enemigo.Y) * PSpeed / distancia;

                enemigo.X += stepX;
                enemigo.Y += stepY;

                // Actualizar la posición del enemigo en el canvas
                Canvas.SetLeft(enemigo.Rectangulo, enemigo.X + 10);
                Canvas.SetTop(enemigo.Rectangulo, enemigo.Y + 10);
            }
        }

        public List<Enemy> GetEnemigos()
        {
            return enemigos; // Devuelve la lista de enemigos
        }

    }


    // Clase auxiliar para almacenar la información de los enemigos
    public class Enemy
    {
        public Rectangle Rectangulo { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public double DestinoX { get; set; }
        public double DestinoY { get; set; }

        // Propiedades para manejar la invencibilidad y el tiempo de detención
        public bool IsInvincible { get; private set; } = false;  // Indica si el enemigo es invencible
        private DispatcherTimer stopTimer;  // Temporizador para detener al enemigo

        public Enemy(Rectangle rectangulo, double x, double y)
        {
            Rectangulo = rectangulo;
            X = x;
            Y = y;
            DestinoX = x;
            DestinoY = y;

            // Configurar el temporizador para detener el enemigo por un tiempo aleatorio
            stopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)  // Controla la actualización del temporizador
            };

            stopTimer.Tick += (s, e) => UpdateDetentionTimer();
        }

        // Iniciar el tiempo de detención en el aeropuerto o portaavión
        public void StartDetentionTime()
        {
            int randomDetentionTime = new Random().Next(1000, 5000); // Tiempo aleatorio entre 1 y 5 segundos
            IsInvincible = true;  // Hacer al enemigo invencible durante la detención
            stopTimer.Interval = TimeSpan.FromMilliseconds(randomDetentionTime);
            stopTimer.Start();  // Comienza el temporizador
        }

        // Actualizar el temporizador de detención
        private void UpdateDetentionTimer()
        {
            // Cuando el temporizador llega a su fin, se puede mover de nuevo y ya no será invencible
            IsInvincible = false;
            stopTimer.Stop();  // Detener el temporizador
        }

        public void Destruir(Canvas gameCanvas, List<Enemy> enemigos)
        {
            // Si el enemigo no es invencible, se destruye
            if (!IsInvincible)
            {
                // Eliminar el rectángulo del Canvas
                gameCanvas.Children.Remove(Rectangulo);

                // Eliminar el enemigo de la lista
                enemigos.Remove(this);
            }
        }
    }




}
