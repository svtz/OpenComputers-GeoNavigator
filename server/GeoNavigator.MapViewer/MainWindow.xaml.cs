using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GeoNavigator.Hardness;
using Color = System.Drawing.Color;
using Path = System.IO.Path;

namespace GeoNavigator.MapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemWatcher _watcher;

        public MainWindow()
        {
            InitializeComponent();
            _watcher = new FileSystemWatcher(Environment.CurrentDirectory, Path.GetFileName(HardnessRepository.FileName));
            _watcher.Created += WatcherTriggered;
            _watcher.Renamed += WatcherTriggered;
            _watcher.Changed += WatcherTriggered;
            _watcher.EnableRaisingEvents = true;
        }

        private void WatcherTriggered(object sender, FileSystemEventArgs e)
        {
            Refresh(sender, null);
        }


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Refresh(sender, e);
        }

        private Bitmap _map;
        private int _minX;
        private int _maxX;
        private int _minZ;
        private int _maxZ;

        private void Refresh(object sender, RoutedEventArgs e)
        {
            var repo = new HardnessRepository();
            if (!repo.Any())
                return;

            var minX = Convert.ToInt32(repo.MinX());
            var maxX = Convert.ToInt32(repo.MaxX());
            var minY = Convert.ToInt32(repo.MinY());
            var maxY = Convert.ToInt32(repo.MaxY());
            var minZ = Convert.ToInt32(repo.MinZ());
            var maxZ = Convert.ToInt32(repo.MaxZ());

            var mapSizeX = maxX - minX;
            var mapSizeZ = maxZ - minZ;

            //var hardnessMin = repo.HardnessMin();
            //var hardnessMax = repo.HardnessMax();
            //var hardnessAverage = repo.HardnessAverage();
            //var hardness34 = (hardnessMax - hardnessAverage) / 2 + hardnessAverage;

            var colorsPalette = new Dictionary<double, Color>
            {
                {double.NegativeInfinity, Color.Transparent},
                {0, Color.DarkSlateGray},
                {1, Color.Green},
                {2, Color.Yellow},
                {3, Color.Orange},
                {4, Color.Red},
                {10, Color.DarkMagenta},
                {double.PositiveInfinity, Color.White},
            };

            _map?.Dispose();
            _map = new Bitmap(mapSizeX, mapSizeZ);

            for (var x = minX; x < maxX; ++x)
            for (var z = minZ; z < maxZ; ++z)
            {
                var tranparent = true;
                var maxH = 0.0;
                for (var y = minY; y < maxY; ++y)
                {
                    var h = repo.Get(x, y, z);
                    if (!h.HasValue) continue;

                    tranparent = false;
                    if (h > maxH)
                        maxH = h.Value;
                }
                if (tranparent)
                    _map.SetPixel(x - minX, z - minZ, Color.Transparent);
                else
                {
                    var prevColor = colorsPalette.First().Value;
                    var prevH = colorsPalette.First().Key;
                    foreach (var colorValue in colorsPalette)
                    {
                        var nextColor = colorValue.Value;
                        var nextH = colorValue.Key;
                        if (maxH <= colorValue.Key)
                        {
                            Color color;
                            if (prevColor == nextColor)
                                color = nextColor;
                            else
                            {
                                var a = prevColor.A + (nextColor.A - prevColor.A) / (nextH - prevH) * (maxH - prevH);
                                var r = prevColor.R + (nextColor.R - prevColor.R) / (nextH - prevH) * (maxH - prevH);
                                var g = prevColor.G + (nextColor.G - prevColor.G) / (nextH - prevH) * (maxH - prevH);
                                var b = prevColor.B + (nextColor.B - prevColor.B) / (nextH - prevH) * (maxH - prevH);
                                color = Color.FromArgb((int)a, (int)r, (int)g, (int)b);
                            }

                            _map.SetPixel(x - minX, z - minZ, color);
                            break;
                        }
                        prevColor = colorValue.Value;
                        prevH = colorValue.Key;
                    }
                }
            }

            Dispatcher.Invoke(() =>
            {

                var pointer = _map.GetHbitmap();
                try
                {
                    MapContainer.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        pointer,
                        IntPtr.Zero,
                        System.Windows.Int32Rect.Empty,
                        BitmapSizeOptions.FromWidthAndHeight(mapSizeX, mapSizeZ));
                    _minX = minX;
                    _minZ = minZ;
                    _maxX = maxX;
                    _maxZ = maxZ;
                }
                finally
                {
                    DeleteObject(pointer);
                }
            });
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        private Tuple<long, long> ToMinecraftCoords(MouseEventArgs e)
        {
            var pos = e.GetPosition(MapContainer);
            var xPct = pos.X / MapContainer.ActualWidth;
            var zPct = pos.Y / MapContainer.ActualHeight;

            var x = (_maxX - _minX) * xPct + _minX;
            var z = (_maxZ - _minZ) * zPct + _minZ;

            return Tuple.Create((long)x, (long)z);
        }

        private void MapContainer_OnMouseMove(object sender, MouseEventArgs e)
        {
            var mineCoords = ToMinecraftCoords(e);
            CoordsDisplay.Text = $"X:{mineCoords.Item1:N0} Z:{mineCoords.Item2:N0}";
        }

        private void MapContainer_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var mineCoords = ToMinecraftCoords(e);
            var repo = new HardnessRepository();

            var hardnessRating = new List<KeyValuePair<long, double>>();
            for (int y = 0; y < 256; y++)
            {
                var h = repo.Get(mineCoords.Item1, y, mineCoords.Item2);
                if (!h.HasValue)
                    continue;

                hardnessRating.Add(new KeyValuePair<long, double>(y, h.Value));
            }

            var maxH = hardnessRating.OrderByDescending(r => r.Value).Take(5).OrderByDescending(h => h.Key).ToArray();
            if (maxH.Any())
                MessageBox.Show(this, $"Максимальная концентрация на глубинах:{Environment.NewLine}{string.Join(Environment.NewLine, maxH.Select(h => $"{h.Key} ({h.Value})"))}",
                    $"X:{mineCoords.Item1:N0} Z:{mineCoords.Item2:N0}", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
