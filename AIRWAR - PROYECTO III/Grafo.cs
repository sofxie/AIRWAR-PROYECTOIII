using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using System.Windows.Shapes;
using System.Windows;

namespace AIRWAR___PROYECTO_III
{
    public class Grafo
    {
        private Random random = new Random();
        public Dictionary<Nodo, List<Nodo>> AdjacencyList { get; private set; } // Lista de adyacencia

        public Grafo()
        {
            AdjacencyList = new Dictionary<Nodo, List<Nodo>>();
        }
        public void PlaceNodesManually(List<(double X, double Y)> Aeropuerto, List<(double X, double Y)> Portaaviones, Canvas canvas)
        {
            string airportImagePath = "C:\\Users\\sofia\\source\\repos\\AIRWAR - PROYECTO III\\AIRWAR - PROYECTO III\\Imagen\\PortaAviones.png"; // Cambia a la ruta real de tu imagen
            string carrierImagePath = "C:\\Users\\sofia\\source\\repos\\AIRWAR - PROYECTO III\\AIRWAR - PROYECTO III\\Imagen\\PortaAviones.png";
            // Crear y colocar
            foreach (var position in Aeropuerto)
            {
                var AeroImage = CrearImagenEstructura(airportImagePath);
                Nodo airport = new Nodo($"Aeropuerto {position}", "Aeropuerto", position, AeroImage);
                AddNode(airport);
                AddToCanvas(airport, canvas);
            }
            foreach (var position in Portaaviones)
            {
                var PortaImage = CrearImagenEstructura(carrierImagePath);
                Nodo carrier = new Nodo($"Portaavión {position}", "Portaavión", position, PortaImage);
                AddNode(carrier);
                AddToCanvas(carrier, canvas);
            }
        }
        // Agregar nodo al Canvas
        private void AddToCanvas(Nodo node, Canvas canvas)
        {
            if (node.Shape is UIElement element)
            {
                Canvas.SetLeft(element, node.Position.X);
                Canvas.SetTop(element, node.Position.Y);
                canvas.Children.Add(element);
            }
        }

        // Agregar nodo
        public void AddNode(Nodo node)
        {
            if (!AdjacencyList.ContainsKey(node))
            {
                AdjacencyList[node] = new List<Nodo>();
            }
        }

        // Agregar ruta entre nodos
        public void AddEdge(Nodo from, Nodo to)
        {
            if (AdjacencyList.ContainsKey(from) && AdjacencyList.ContainsKey(to))
            {
                AdjacencyList[from].Add(to);
                AdjacencyList[to].Add(from); // Grafo no dirigido
            }
        }
        private Image CrearImagenEstructura(string imagePath, double width = 100, double height = 100)
        {
            var image = new Image
            {
                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)),
                Width = width,
                Height = height
            };
            return image;
        }

        // Generar rutas aleatorias (probabilidad de conexión)
        public void GenerateRandomRoutes(double connectionProbability)
        {
            var nodes = new List<Nodo>(AdjacencyList.Keys);

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    if (random.NextDouble() <= connectionProbability) // Probabilidad de conexión
                    {
                        AddEdge(nodes[i], nodes[j]);
                    }
                }
            }
        }   
    }
}
