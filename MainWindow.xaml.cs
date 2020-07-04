using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Shapes;

namespace MapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Editor edt = new Editor();
        List<Rectangle> rectsBase = new List<Rectangle>();
        List<Rectangle> rectsTop = new List<Rectangle>();
        List<Rectangle> rectsHitbox = new List<Rectangle>();

        public MainWindow()
        {
            InitializeComponent();
            CreateHitboxPreview();
        }



        private void UpdateTilePreview(int x, int y)
        {

            if (edt.baseTileset != null && x + y * edt.MapWidth < rectsBase.Count)
            {

                int placedX = (edt.baseTiles[x + y * edt.MapWidth] % edt.baseTilesetColumns);
                int placedY = (edt.baseTiles[x + y * edt.MapWidth] / edt.baseTilesetColumns);

                Rectangle rect = new Rectangle();
                ImageBrush b = new ImageBrush();
                b.ImageSource = edt.baseTileset;
                b.Viewbox = new Rect(placedX / (float)edt.baseTilesetColumns, placedY / (float)edt.baseTilesetRows, edt.TileSize / (float)edt.baseTileset.PixelWidth, edt.TileSize / (float)edt.baseTileset.PixelHeight);
                rect.Fill = b;
                rect.Height = edt.TileSize;
                rect.Width = edt.TileSize;
                rectsBase[x + y * edt.MapWidth].Fill = b;
            }
            if (edt.topTileset != null && x + y * edt.MapWidth < rectsTop.Count)
            {

                int placedX = (edt.topTiles[x + y * edt.MapWidth] % edt.topTilesetColumns);
                int placedY = (edt.topTiles[x + y * edt.MapWidth] / edt.topTilesetColumns);

                Rectangle rect = new Rectangle();
                ImageBrush b = new ImageBrush();
                b.ImageSource = edt.topTileset;
                b.Viewbox = new Rect(placedX / (float)edt.topTilesetColumns, placedY / (float)edt.topTilesetRows, edt.TileSize / (float)edt.topTileset.PixelWidth, edt.TileSize / (float)edt.topTileset.PixelHeight);
                rect.Fill = b;
                rect.Height = edt.TileSize;
                rect.Width = edt.TileSize;
                rectsTop[x + y * edt.MapWidth].Fill = b;
            }
            if (x + y * edt.MapWidth < rectsHitbox.Count)
            {
                Rectangle rect = new Rectangle();
                SolidColorBrush b = new SolidColorBrush();
                if (edt.hitboxes[x + y * edt.MapWidth] != 0)
                    b.Color = new Color() { R = 255, G = 255, B = 0, A = 128 };
                else
                    b.Color = new Color() { R = 0, G = 0, B = 0, A = 0 };
                rect.Fill = b;
                rect.Height = edt.TileSize;
                rect.Width = edt.TileSize;
                rectsHitbox[x + y * edt.MapWidth].Fill = b;
            }

        }

        private void CreateMapButton()
        {
            if (UIMapButton == null)
                UIMapButton = new Button();
            UIMapButton.Width = edt.TileSize * edt.MapWidth;
            UIMapButton.Height = edt.TileSize * edt.MapHeight;
        }

        private void CreateBasePreview()
        {
            if (UIBaseMapCanvas == null)
                UIBaseMapCanvas = new Canvas();
            UIBaseMapCanvas.Width = edt.TileSize * edt.MapWidth;
            UIBaseMapCanvas.Height = edt.TileSize * edt.MapHeight;
            UIBaseMapCanvas.RenderSize = new Size(edt.TileSize * edt.MapWidth, edt.TileSize * edt.MapHeight);

            CreateMapButton();

            UIBaseMapCanvas.Children.Clear();
            rectsBase.Clear();


            if (edt.baseTileset != null)
            {
                for (int y = 0; y < edt.MapHeight; y++)
                {
                    for (int x = 0; x < edt.MapWidth; x++)
                    {
                        int placedX = (edt.baseTiles[x + y * edt.MapWidth] % edt.baseTilesetColumns);
                        int placedY = (edt.baseTiles[x + y * edt.MapWidth] / edt.baseTilesetColumns);

                        rectsBase.Add(new Rectangle());
                        ImageBrush b = new ImageBrush();
                        b.ImageSource = edt.baseTileset;
                        b.Viewbox = new Rect(placedX / (float)edt.baseTilesetColumns, placedY / (float)edt.baseTilesetRows, edt.TileSize / (float)edt.baseTileset.PixelWidth, edt.TileSize / (float)edt.baseTileset.PixelHeight);
                        rectsBase[x + y * edt.MapWidth].Fill = b;
                        rectsBase[x + y * edt.MapWidth].Height = edt.TileSize;
                        rectsBase[x + y * edt.MapWidth].Width = edt.TileSize;
                        UIBaseMapCanvas.Children.Add(rectsBase[x + y * edt.MapWidth]);
                        Canvas.SetTop(UIBaseMapCanvas.Children[x + y * edt.MapWidth], y * edt.TileSize);
                        Canvas.SetLeft(UIBaseMapCanvas.Children[x + y * edt.MapWidth], x * edt.TileSize);
                    }
                }
            }
        }
        private void CreateTopPreview()
        {
            if (UITopMapCanvas == null)
                UITopMapCanvas = new Canvas();
            UITopMapCanvas.Width = edt.TileSize * edt.MapWidth;
            UITopMapCanvas.Height = edt.TileSize * edt.MapHeight;
            UITopMapCanvas.RenderSize = new Size(edt.TileSize * edt.MapWidth, edt.TileSize * edt.MapHeight);

            CreateMapButton();

            UITopMapCanvas.Children.Clear();
            rectsTop.Clear();

            for (int y = 0; y < edt.MapHeight; y++)
            {
                for (int x = 0; x < edt.MapWidth; x++)
                {
                    if (edt.topTileset != null)
                    {
                        int placedX = (edt.topTiles[x + y * edt.MapWidth] % edt.topTilesetColumns);
                        int placedY = (edt.topTiles[x + y * edt.MapWidth] / edt.topTilesetColumns);

                        rectsTop.Add(new Rectangle());
                        ImageBrush b = new ImageBrush();
                        b.ImageSource = edt.topTileset;
                        b.Viewbox = new Rect(placedX / (float)edt.topTilesetColumns, placedY / (float)edt.topTilesetRows, edt.TileSize / (float)edt.topTileset.PixelWidth, edt.TileSize / (float)edt.topTileset.PixelHeight);
                        rectsTop[x + y * edt.MapWidth].Fill = b;
                        rectsTop[x + y * edt.MapWidth].Height = edt.TileSize;
                        rectsTop[x + y * edt.MapWidth].Width = edt.TileSize;
                        UITopMapCanvas.Children.Add(rectsTop[x + y * edt.MapWidth]);
                        Canvas.SetTop(UITopMapCanvas.Children[x + y * edt.MapWidth], y * edt.TileSize);
                        Canvas.SetLeft(UITopMapCanvas.Children[x + y * edt.MapWidth], x * edt.TileSize);
                    }
                }
            }
        }
        private void CreateHitboxPreview()
        {
            if (true)
            {
                if (UIHitboxMapCanvas == null)
                    UIHitboxMapCanvas = new Canvas();
                UIHitboxMapCanvas.Width = edt.TileSize * edt.MapWidth;
                UIHitboxMapCanvas.Height = edt.TileSize * edt.MapHeight;
                UIHitboxMapCanvas.RenderSize = new Size(edt.TileSize * edt.MapWidth, edt.TileSize * edt.MapHeight);

                CreateMapButton();

                UIHitboxMapCanvas.Children.Clear();
                rectsHitbox.Clear();

                for (int y = 0; y < edt.MapHeight; y++)
                {
                    for (int x = 0; x < edt.MapWidth; x++)
                    {
                        rectsHitbox.Add(new Rectangle());
                        SolidColorBrush b = new SolidColorBrush();
                        if (edt.hitboxes[x + y * edt.MapWidth] != 0)
                            b.Color = new Color() { R = 255, G = 255, B = 0, A = 128 };
                        else
                            b.Color = new Color() { R = 0, G = 0, B = 0, A = 0 };
                        rectsHitbox[x + y * edt.MapWidth].Fill = b;
                        rectsHitbox[x + y * edt.MapWidth].Height = edt.TileSize;
                        rectsHitbox[x + y * edt.MapWidth].Width = edt.TileSize;
                        UIHitboxMapCanvas.Children.Add(rectsHitbox[x + y * edt.MapWidth]);
                        Canvas.SetTop(UIHitboxMapCanvas.Children[x + y * edt.MapWidth], y * edt.TileSize);
                        Canvas.SetLeft(UIHitboxMapCanvas.Children[x + y * edt.MapWidth], x * edt.TileSize);
                    }
                }
            }
        }

        private void ButtonLoadBaseTileset_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Tilemaps (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp"
            };
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                Uri imageUri = new Uri(openFileDialog.FileName);
                edt.baseTileset = new BitmapImage(new Uri(openFileDialog.FileName));

                if (edt.baseTileset.PixelWidth % edt.TileSize != 0 || edt.baseTileset.PixelHeight % edt.TileSize != 0 || edt.TileSize <= 0)
                {
                    MessageBox.Show("Invalid tile size for this image. Tileset size not divisible by tile size", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    edt.baseTileset = null;
                }
                else
                    UseBase();
            }
            CreateBasePreview();
        }

        private void ButtonLoadTopTileset_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Tilemaps (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp"
            };
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                Uri imageUri = new Uri(openFileDialog.FileName);
                edt.topTileset = new BitmapImage(new Uri(openFileDialog.FileName));

                if (edt.topTileset.PixelWidth % edt.TileSize != 0 || edt.topTileset.PixelHeight % edt.TileSize != 0 || edt.TileSize <= 0)
                {
                    MessageBox.Show("Invalid tile size for this image. Tileset size not divisible by tile size", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    edt.topTileset = null;
                }
                else
                    UseTop();
            }
            CreateTopPreview();
        }

        private void SelectTile_Click(object sender, RoutedEventArgs e)
        {
            if (edt.SelectedLayer == 0)
            {
                edt.selectedTile = Convert.ToInt32(Math.Floor((Mouse.GetPosition(TilesetImage).X / TilesetImage.ActualWidth) * edt.baseTilesetColumns))
             + Convert.ToInt32(Math.Floor((Mouse.GetPosition(TilesetImage).Y / TilesetImage.ActualHeight) * edt.baseTilesetRows)) * edt.baseTilesetColumns;
            }
            if (edt.SelectedLayer == 1)
            {
                edt.selectedTile = Convert.ToInt32(Math.Floor((Mouse.GetPosition(TilesetImage).X / TilesetImage.ActualWidth) * edt.topTilesetColumns))
             + Convert.ToInt32(Math.Floor((Mouse.GetPosition(TilesetImage).Y / TilesetImage.ActualHeight) * edt.topTilesetRows)) * edt.topTilesetColumns;
            }
            UpdateSelectedTileText();
        }

        private void UpdateSelectedTileText()
        {
            if (edt.SelectedLayer == 2)
                selectedTileText.Text = "HITBOX";
            else
            {
                string layer = "";
                if (edt.SelectedLayer == 0)
                    layer = "BASE - ";
                if (edt.SelectedLayer == 1)
                    layer = "TOP - ";

                selectedTileText.Text = layer + "Selected Tile: " + edt.selectedTile.ToString();
            }
            
        }

        private void UpdateMapSizeText()
        {
            TileSizeLabel.Text = "Tile Size: " + edt.TileSize;
            MapSizeLabel.Text = "Map Size: " + edt.MapWidth + "x" + edt.MapHeight;
        }
        private void ApplySizes_Click(object sender, RoutedEventArgs e)
        {
            int mapSizeX = 0;
            int mapSizeY = 0;
            int tileSize = 0;
            if (!int.TryParse(MapSizeX.Text, out mapSizeX))
            {
                MessageBox.Show("Wrong map width.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(MapSizeY.Text, out mapSizeY))
            {
                MessageBox.Show("Wrong map height.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(TileSize.Text, out tileSize))
            {
                MessageBox.Show("Wrong tile size.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            edt.ResizeMap(mapSizeX, mapSizeY);
            edt.TileSize = tileSize;

            UpdateMapSizeText();
            CreateBasePreview();
            CreateTopPreview();
            CreateHitboxPreview();
        }

        private void UpdateCheckBoxes(object sender, RoutedEventArgs e)
        {
            if (UIBaseMapCanvas != null)
            {
                if (UIShowBaseCheckBox.IsChecked == true)
                    UIBaseMapCanvas.Visibility = Visibility.Visible;
                else
                    UIBaseMapCanvas.Visibility = Visibility.Hidden;
            }
            if (UITopMapCanvas != null)
            {
                if (UIShowTopCheckBox.IsChecked == true)
                    UITopMapCanvas.Visibility = Visibility.Visible;
                else
                    UITopMapCanvas.Visibility = Visibility.Hidden;
            }
            if (UIHitboxMapCanvas != null)
            {
                if (UIShowHitboxesCheckBox.IsChecked == true)
                    UIHitboxMapCanvas.Visibility = Visibility.Visible;
                else
                    UIHitboxMapCanvas.Visibility = Visibility.Hidden;
            }
        }


        private void PlaceTile_Click(object sender, RoutedEventArgs e)
        {
            int posX = Convert.ToInt32(Math.Floor((Mouse.GetPosition(UIMapButton).X / UIBaseMapCanvas.ActualWidth) * edt.MapWidth));
            int posY = Convert.ToInt32(Math.Floor((Mouse.GetPosition(UIMapButton).Y / UIBaseMapCanvas.ActualHeight) * edt.MapHeight));
            if (posX + posY * edt.MapWidth < edt.baseTiles.Count && edt.SelectedLayer == 0)
            {
                if (PlacerBrush.IsChecked == true)
                    edt.baseTiles[posX + posY * edt.MapWidth] = edt.selectedTile;
                else if (PlacerFill.IsChecked == true)
                {
                    for (int i = 0; i < edt.MapWidth * edt.MapHeight; i++)
                        edt.baseTiles[i] = edt.selectedTile;
                    PlacerFill.IsChecked = false;
                    PlacerBrush.IsChecked = true;
                    CreateBasePreview();
                }
            }

            if (posX + posY * edt.MapWidth < edt.topTiles.Count && edt.SelectedLayer == 1)
            {
                if (PlacerBrush.IsChecked == true)
                    edt.topTiles[posX + posY * edt.MapWidth] = edt.selectedTile;
                else if (PlacerFill.IsChecked == true)
                {
                    for (int i = 0; i < edt.MapWidth * edt.MapHeight; i++)
                        edt.topTiles[i] = edt.selectedTile;
                    PlacerFill.IsChecked = false;
                    PlacerBrush.IsChecked = true;
                    CreateTopPreview();
                }
            }
                
                
            if (posX + posY * edt.MapWidth < edt.hitboxes.Count && edt.SelectedLayer == 2)
            {
                if (edt.hitboxes[posX + posY * edt.MapWidth] == 0)
                    edt.hitboxes[posX + posY * edt.MapWidth] = 1;
                else
                    edt.hitboxes[posX + posY * edt.MapWidth] = 0;
            }
            UpdateTilePreview(posX, posY);
        }

        private void UseBase()
        {
            if (edt.baseTileset != null)
            {
                TilesetImage.Source = edt.baseTileset;
                edt.selectedTile = 0;
                edt.SelectedLayer = 0;

                UpdateSelectedTileText();
            }
        }
        private void UseBaseClick(object sender, RoutedEventArgs e) => UseBase();
        private void UseTop()
        {
            if (edt.topTileset != null)
            {
                TilesetImage.Source = edt.topTileset;
                edt.selectedTile = 0;
                edt.SelectedLayer = 1;
                UpdateSelectedTileText();
            }
        }
        private void UseTopClick(object sender, RoutedEventArgs e) => UseTop();
        private void UseHitbox()
        {
            edt.selectedTile = 0;
            edt.SelectedLayer = 2;
            UpdateSelectedTileText();
        }
        private void UseHitboxClick(object sender, RoutedEventArgs e) => UseHitbox();

        private void LoadBaseMapFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text map files (*.txt, *.dat)|*.txt;*.dat"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                string map = File.ReadAllText(openFileDialog.FileName);
                string[] rows = map.Split('\n');
                List<string> tiles = new List<string>();
                foreach (string row in rows)
                {
                    row.Trim(' ');
                    string[] rowTiles = row.Split(' ');
                    foreach (string tile in rowTiles)
                        tiles.Add(tile);
                }
                
                edt.ResizeMap(tiles.Count / rows.Length, rows.Length);
                UpdateMapSizeText();
                for (int i = 0; i < tiles.Count; i++)
                    edt.baseTiles[i] = Int32.Parse(tiles[i]);
                CreateBasePreview();
            }
        }

        private void LoadTopMapFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text map files (*.txt, *.dat)|*.txt;*.dat"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                string map = File.ReadAllText(openFileDialog.FileName);
                string[] rows = map.Split('\n');
                List<string> tiles = new List<string>();
                foreach (string row in rows)
                {
                    row.Trim(' ');
                    string[] rowTiles = row.Split(' ');
                    foreach (string tile in rowTiles)
                        tiles.Add(tile);
                }
                if (edt.MapWidth == tiles.Count / rows.Length && edt.MapHeight == rows.Length)
                {
                    for (int i = 0; i < tiles.Count; i++)
                        edt.topTiles[i] = Int32.Parse(tiles[i]);
                    CreateTopPreview();
                }
                else
                {
                    WPFCustomMessageBox.CustomMessageBox.ShowOK("Top map size not equal base map size.", "Error", "RIP", MessageBoxImage.Error);
                }
            }
        }

        private void LoadHitboxMapFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text map files (*.txt, *.dat)|*.txt;*.dat"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                string map = File.ReadAllText(openFileDialog.FileName);
                string[] rows = map.Split('\n');
                List<string> tiles = new List<string>();
                foreach (string row in rows)
                {
                    row.Trim(' ');
                    string[] rowTiles = row.Split(' ');
                    foreach (string tile in rowTiles)
                        tiles.Add(tile);
                }
                if (edt.MapWidth == tiles.Count / rows.Length && edt.MapHeight == rows.Length)
                {
                    for (int i = 0; i < tiles.Count; i++)
                        edt.hitboxes[i] = Int32.Parse(tiles[i]);
                    CreateHitboxPreview();
                }
                else
                {
                    WPFCustomMessageBox.CustomMessageBox.ShowOK("Hitbox map size not equal base map size.", "Error", "RIP", MessageBoxImage.Error);
                }
            }
        }

        private void SaveMap(object sender, RoutedEventArgs e)
        {
            string WriteToString(List<int> tiles)
            {
                string target = "";
                for (int y = 0; y < edt.MapHeight; y++)
                {
                    for (int x = 0; x < edt.MapWidth; x++)
                    {
                        target += tiles[x + y * edt.MapWidth];
                        if (x != edt.MapWidth - 1)
                            target += ' ';
                    }
                    if (y != edt.MapHeight - 1)
                        target += '\n';
                }
                return target;
            }
            string baseText = WriteToString(edt.baseTiles);
            File.WriteAllText("Maps\\" + MapName.Text + "_base.dat", baseText);
            string topText = WriteToString(edt.topTiles);
            File.WriteAllText("Maps\\" + MapName.Text + "_top.dat", topText);
            string hitboxesText = WriteToString(edt.hitboxes);
            File.WriteAllText("Maps\\" + MapName.Text + "_hitboxes.dat", hitboxesText);

        }
    }
}
