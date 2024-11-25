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
        private List<Line> routeLines = new List<Line>(); // Almacena las líneas de ruta

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
            // Crear enemigos para cada tupla en airportPositions solo si no se ha alcanzado el límite de 2 aviones
            foreach (var posicion in airportPositions)
            {
                if (airportAircraftCount[posicion] < 2)
                {
                    EnemigosSpawn(posicion.X, posicion.Y, "Aeropuerto"); // Pasar el origen como Aeropuerto
                    airportAircraftCount[posicion]++; // Incrementar el contador de aviones del aeropuerto
                }
            }

            // Crear enemigos para cada tupla en carrierPositions solo si no se ha alcanzado el límite de 2 aviones
            foreach (var posicion in carrierPositions)
            {
                if (carrierAircraftCount[posicion] < 2)
                {
                    EnemigosSpawn(posicion.X, posicion.Y, "Portavión"); // Pasar el origen como Portavión
                    carrierAircraftCount[posicion]++; // Incrementar el contador de aviones del portaavión
                }
            }
        }

        private void EnemigosSpawn(double x, double y, string origen)
        {
            ImageBrush enemyBrush = new ImageBrush();
            enemyBrush.ImageSource = new BitmapImage(new Uri("C:\\Users\\sofia\\source\\repos\\AIRWAR - PROYECTO III\\AIRWAR - PROYECTO III\\Imagen\\Plane.png"));

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
            enemigos.Add(new Enemy(newEnemy, x, y, origen, gameCanvas));  // Pasamos el origen aquí
        }

        private void MoverAviones()
        {
            var movimientoAvionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };

            movimientoAvionTimer.Tick += (s, e) =>
            {
                foreach (var enemigo in enemigos)
                {
                    MoverAvion(enemigo);

                    // Recargar combustible si el avión está en un aeropuerto
                    if (enemigo.Origen == "Aeropuerto" && enemigo.X == enemigo.DestinoX && enemigo.Y == enemigo.DestinoY)
                    {
                        enemigo.Refuel();
                    }
                }

                // Eliminar enemigos con combustible agotado
                EliminarEnemigosMarcados();

                // Después de mover los aviones, actualizamos las rutas
                DrawRoutes(); // Actualizar las rutas
            };

            movimientoAvionTimer.Start();
        }

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

                // Consumir combustible más rápido (ajustado el valor a 0.5 para un consumo más rápido)
                enemigo.ConsumeFuel(0.4);  // Ajusta este valor según la velocidad de consumo del avión

                // Si el avión se queda sin combustible, marcarlo para eliminarlo
                if (enemigo.CurrentFuel == 0)
                {
                    enemigo.MarkForDeletion();
                }

                // Actualizar la posición del enemigo en el canvas
                Canvas.SetLeft(enemigo.Rectangulo, enemigo.X + 10);
                Canvas.SetTop(enemigo.Rectangulo, enemigo.Y + 10);

                // Mover la barra de combustible junto con el avión
                Canvas.SetLeft(enemigo.FuelBar, enemigo.X + 10);  // Mover la barra de combustible a la misma X que el avión
                Canvas.SetTop(enemigo.FuelBar, enemigo.Y + 40);   // Mover la barra de combustible debajo del avión
            }
        }

        private void EliminarEnemigosMarcados()
        {
            var enemigosAEliminar = enemigos.Where(e => e.IsMarkedForDeletion).ToList();

            foreach (var enemigo in enemigosAEliminar)
            {
                enemigo.Destruir(gameCanvas, enemigos);
            }
        }

        // Método para dibujar las rutas con líneas punteadas entre los aviones y los aeropuertos/portaviones
        private void DrawRoutes()
        {
            // Limpiar las líneas existentes
            foreach (var line in routeLines)
            {
                gameCanvas.Children.Remove(line);
            }

            routeLines.Clear(); // Limpiar la lista de rutas

            // Dibujar las rutas para cada avión
            foreach (var enemigo in enemigos)
            {
                // Determinar el color de la línea según el origen del avión
                var origen = new Point(enemigo.X, enemigo.Y);
                var destino = new Point(enemigo.DestinoX, enemigo.DestinoY);

                // Determinar si el avión proviene de un aeropuerto o de un portavión
                Brush lineColor = Brushes.Red; // Default to red (for airport)
                if (enemigo.Origen == "Portavión")
                {
                    // Si el avión proviene de un portavión, usar azul
                    lineColor = Brushes.Blue;
                }

                // Crear la línea punteada con el color adecuado
                Line routeLine = new Line
                {
                    X1 = origen.X + 45,
                    Y1 = origen.Y + 45,
                    X2 = destino.X + 45,
                    Y2 = destino.Y + 45,
                    Stroke = lineColor, // Asignar el color según el origen
                    StrokeThickness = 2, // Grosor de la línea
                    StrokeDashArray = new DoubleCollection { 2, 2 } // Patrón punteado
                };

                // Añadir la línea al Canvas
                gameCanvas.Children.Add(routeLine);
                routeLines.Add(routeLine); // Añadir la línea a la lista de rutas
            }
        }

        public List<Enemy> GetEnemigos()
        {
            return enemigos; // Devuelve la lista de enemigos
        }
    }

    public class Enemy
    {
        public Rectangle Rectangulo { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public double DestinoX { get; set; }
        public double DestinoY { get; set; }

        private Canvas gameCanvas2;

        // Barra de combustible
        public Rectangle FuelBar { get; }
        private double maxFuel = 100; // Máximo de combustible
        public double CurrentFuel { get; set; } // Combustible actual

        public bool IsInvincible { get; private set; } = false;  // Indica si el enemigo es invencible
        private DispatcherTimer stopTimer;  // Temporizador para detener al enemigo
        private DispatcherTimer refuelTimer; // Temporizador para recargar combustible
        public string Origen { get; set; }

        public bool IsMarkedForDeletion { get; private set; } = false;  // Si está marcado para eliminación

        public Enemy(Rectangle rectangulo, double x, double y, string origen, Canvas gameCanvas)
        {
            Rectangulo = rectangulo;
            X = x;
            Y = y;
            DestinoX = x;
            DestinoY = y;
            Origen = origen;
            gameCanvas2 = gameCanvas;

            // Configuración de la barra de combustible
            FuelBar = new Rectangle
            {
                Fill = Brushes.Orange,  // Barra de combustible color naranja
                Height = 3,
                Width = 50 // Ancho inicial de la barra
            };

            Canvas.SetTop(FuelBar, y + 40); // Colocar la barra debajo del avión
            Canvas.SetLeft(FuelBar, x + 10); // Ajustar la posición de la barra en el Canvas
            gameCanvas2.Children.Add(FuelBar); // Asegúrate de añadirla al Canvas

            // Inicializar el combustible
            CurrentFuel = maxFuel;

            // Configurar el temporizador para detener el enemigo por un tiempo aleatorio
            stopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)  // Controla la actualización del temporizador
            };

            stopTimer.Tick += (s, e) => UpdateDetentionTimer();

            // Temporizador para recargar el combustible
            refuelTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)  // Recargar cada 500 ms (ajustable)
            };
            refuelTimer.Tick += (s, e) => RefuelIncrement(); // Llamar a la recarga

            this.gameCanvas2 = gameCanvas2;
        }

        // Iniciar el tiempo de detención en el aeropuerto o portaavión
        public void StartDetentionTime()
        {
            int randomDetentionTime = new Random().Next(4000, 10000); // Tiempo aleatorio entre 1 y 5 segundos
            IsInvincible = true;  // Hacer al enemigo invencible durante la detención
            stopTimer.Interval = TimeSpan.FromMilliseconds(randomDetentionTime);
            stopTimer.Start();  // Comienza el temporizador

            // Comenzar el temporizador de recarga de combustible
            refuelTimer.Start();
        }

        // Actualizar el temporizador de detención
        private void UpdateDetentionTimer()
        {
            // Cuando el temporizador llega a su fin, se puede mover de nuevo y ya no será invencible
            IsInvincible = false;
            stopTimer.Stop();  // Detener el temporizador

            // Detener el temporizador de recarga cuando termine la detención
            refuelTimer.Stop();
        }

        // Incrementar el combustible durante el tiempo de detención
        private void RefuelIncrement()
        {
            if (CurrentFuel < maxFuel)
            {
                CurrentFuel += 10;  // Recargar 2 unidades de combustible por cada "tic" del temporizador
                if (CurrentFuel > maxFuel)
                    CurrentFuel = maxFuel;  // Asegurarse de no exceder el máximo

                // Actualizar la barra de combustible
                FuelBar.Width = (CurrentFuel / maxFuel) * 50;  // Ajustar el ancho de la barra según el combustible restante
            }
        }

        // Consumir combustible mientras el avión se mueve
        public void ConsumeFuel(double amount)
        {
            CurrentFuel -= amount;
            if (CurrentFuel <= 0)
            {
                CurrentFuel = 0;
                // Si el combustible se agota, destruir el avión
                MarkForDeletion();
            }

            // Actualizar la barra de combustible
            FuelBar.Width = (CurrentFuel / maxFuel) * 50;  // Ajustar el ancho de la barra según el combustible restante
        }

        // Marcar el enemigo para eliminación
        public void MarkForDeletion()
        {
            IsMarkedForDeletion = true;
        }

        // Recargar combustible cuando el avión llega a un aeropuerto
        public void Refuel()
        {
            CurrentFuel = maxFuel;
            FuelBar.Width = 50;  // Recargar completamente la barra de combustible
        }

        public void Destruir(Canvas gameCanvas, List<Enemy> enemigos)
        {
            // Si el enemigo no es invencible, se destruye
            if (!IsInvincible)
            {
                // Eliminar el rectángulo del Canvas
                gameCanvas.Children.Remove(Rectangulo);
                gameCanvas.Children.Remove(FuelBar);  // Eliminar la barra de combustible

                // Eliminar el enemigo de la lista
                enemigos.Remove(this);
            }
        }
    }

}

