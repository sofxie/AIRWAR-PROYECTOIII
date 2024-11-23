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

        public Player(UIElement element,Canvas canvas)
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
        public void Shoot(double pressDuration)
        {
            double bulletSpeed = Math.Max(200, Math.Min(800, pressDuration * 0.1)); // Velocidad limitada

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
                gameCanvas.Children.Remove(bullet); // Remover bala cuando salga del canvas
            };

            bullet.BeginAnimation(Canvas.TopProperty, bulletAnimation);
        }
        public UIElement GetElement()
        {
            return playerElement;
        }
    }
}
