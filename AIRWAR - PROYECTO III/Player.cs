using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Numerics;
using Microsoft.VisualBasic;
using System.Windows.Shapes;

namespace AIRWAR___PROYECTO_III
{
    public class Player
    {
        public UIElement playerElement;
        public int speed { get; set; } = 8;
        private Canvas gameCanvas;
        private int score;

        public event Action<int> Score;

        public Player(UIElement element, Canvas canvas)
        {
            playerElement = element;
            gameCanvas = canvas;
        }

        public void Move()
        {
            // Mueve al jugador y cambia la dirección si alcanza los límites del canvas
            double left = Canvas.GetLeft(playerElement);
            left += speed;

            // Detectar bordes y cambiar dirección
            if (left < 0 || left + ((FrameworkElement)playerElement).Width > gameCanvas.ActualWidth)
            {
                speed = -speed;
            }

            Canvas.SetLeft(playerElement, left);
        }

        public void Shoot(double pressDuration, List<Enemy> enemigos)  // Recibimos la lista de enemigos
        {
            double bulletSpeed = Math.Max(200, Math.Min(800, pressDuration * 0.5)); // Velocidad limitada

            Rectangle bullet = new Rectangle
            {
                Width = 5,
                Height = 20,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };

            Canvas.SetLeft(bullet, Canvas.GetLeft(playerElement) + ((FrameworkElement)playerElement).Width / 2 - bullet.Width / 2);
            Canvas.SetTop(bullet, Canvas.GetTop(playerElement) - bullet.Height);

            gameCanvas.Children.Add(bullet);

            double targetY = -bullet.Height; // Posición final (fuera del canvas por arriba)
            double initialY = Canvas.GetTop(bullet);
            double duration = Math.Abs(initialY - targetY) / bulletSpeed;

            var bulletAnimation = new DoubleAnimation
            {
                From = initialY,
                To = targetY,
                Duration = TimeSpan.FromSeconds(duration)
            };

            bulletAnimation.Completed += (s, e) =>
            {
                gameCanvas.Children.Remove(bullet); // Remover la bala cuando salga del canvas
            };

            bullet.BeginAnimation(Canvas.TopProperty, bulletAnimation);

            // Detectar la colisión entre la bala y los enemigos
            DispatcherTimer collisionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // Aproximadamente 60 FPS
            };

            collisionTimer.Tick += (s, e) =>
            {
                foreach (var enemigo in enemigos.ToList()) // Usamos `ToList` para evitar modificar la lista mientras la recorremos
                {
                    if (IsColliding(bullet, enemigo))
                    {
                        // Verificar si el enemigo es invencible
                        if (!enemigo.IsInvincible)
                        {
                            // Destruir la bala y el enemigo si hay colisión y no es invencible
                            enemigo.Destruir(gameCanvas, enemigos); // Eliminar enemigo
                            gameCanvas.Children.Remove(bullet);   // Eliminar bala
                        }
                        // Si el enemigo es invencible, no eliminamos la bala, solo detenemos la comprobación de colisiones
                        collisionTimer.Stop();
                        break;
                    }
                }
            };

            collisionTimer.Start();
        }

        private bool IsColliding(Rectangle bullet, Enemy enemigo)
        {
            // Detectar colisión entre la bala y el enemigo
            Rect bulletRect = new Rect(Canvas.GetLeft(bullet), Canvas.GetTop(bullet), bullet.Width, bullet.Height);
            Rect enemyRect = new Rect(Canvas.GetLeft(enemigo.Rectangulo), Canvas.GetTop(enemigo.Rectangulo), enemigo.Rectangulo.Width, enemigo.Rectangulo.Height);

            return bulletRect.IntersectsWith(enemyRect);  // Si se intersectan, hay colisión
        }

        public UIElement GetElement()
        {
            return playerElement;
        }
    }



}
