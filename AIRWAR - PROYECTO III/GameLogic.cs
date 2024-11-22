using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AIRWAR___PROYECTO_III
{
    public class GameLogic
    {
        private DispatcherTimer timer;
        private Canvas gameCanvas;
        private Player player;

        public GameLogic(Canvas canvas, Player player)
        {
            gameCanvas = canvas;
            this.player = player;

            // Inicializar el temporizador
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += GameTimerEvent;
        }
        public void StartGame()
        {
            timer.Start();
        }
        public void HandleKeyPress(Key key)
        {
            return;
        }
        private void GameTimerEvent(object sender, EventArgs e)
        {
            player.Move(gameCanvas);
        }
    }
}
