using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace AIRWAR___PROYECTO_III
{
    public class Nodo
    {
        public string Name { get; set; } // Aeropuerto o Portaavión
        public (double X, double Y) Position { get; set; } // Coordenadas para ubicación
        public UIElement Shape { get; set; }

        public Nodo(string name, string type, (double X, double Y) position, UIElement shape)
        {
            Name = name;
            Position = position;
            Shape = shape;
        }
    }
}
