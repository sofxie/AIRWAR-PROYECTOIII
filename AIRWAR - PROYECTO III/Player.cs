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

namespace AIRWAR___PROYECTO_III
{
    public class Player
    {
        private UIElement playerElement;
        private int speed;

        public Player(UIElement element)
        {
            playerElement = element;
            speed = 8;
        }
        public void Move(Canvas gameCanvas)
        {
            // Mueve al jugador y cambia la dirección si alcanza los límites del canvas
            Canvas.SetLeft(playerElement, Canvas.GetLeft(playerElement) - speed);

            if (Canvas.GetLeft(playerElement) < 5 || Canvas.GetLeft(playerElement) + (playerElement.RenderSize.Width * 2) > gameCanvas.ActualWidth)
            {
                speed = -speed;
            }
        }
        public UIElement GetElement()
        {
            return playerElement;
        }
    }
}
