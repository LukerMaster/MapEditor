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
using System.Windows.Shapes;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO;
using SFML.Graphics;
using SFML.Window;

namespace MapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateRenderWindows();
            UpdateMapPreview();
        }

        RenderWindow mapWindow;
        RenderWindow tilesetWindow;
        Editor edt = new Editor();

        private void CreateRenderWindows()
        {
            {
                
                if (MapSurfaceUI == null)
                    MapSurfaceUI = new WpfSfmlHost.SfmlDrawingSurface();

                var context = new ContextSettings { DepthBits = 24 };
                this.mapWindow = new RenderWindow(this.MapSurfaceUI.Handle, context);
                this.mapWindow.SetActive(true);
            }

            {
                
                if (TilesetSurfaceUI == null)
                    TilesetSurfaceUI = new WpfSfmlHost.SfmlDrawingSurface();

                var context = new ContextSettings { DepthBits = 24 };
                this.tilesetWindow = new RenderWindow(this.TilesetSurfaceUI.Handle, context);
                this.tilesetWindow.SetActive(true);
            }
            
        }



        private void UpdateMapPreview()
        {
            RectangleShape shape = new RectangleShape() { Position = new SFML.System.Vector2f(10, 10) ,Size = new SFML.System.Vector2f(30, 30) };
            shape.FillColor = new Color(255, 0, 0, 255);
            mapWindow.Clear();
            mapWindow.Draw(shape);
            
            if (edt.baseTileset != null && ShowBaseCheckBox.IsChecked == true)
            {
                for (int y = 0; y < edt.MapHeight; y++)
                {
                    for (int x = 0; x < edt.MapWidth; x++)
                    {
                        int selectedX = edt.TileSize * (edt.baseTiles[x + y * edt.MapWidth] % edt.baseTilesetColumns);
                        int selectedY = edt.TileSize * (edt.baseTiles[x + y * edt.MapWidth] / edt.baseTilesetColumns);
                    }
                }
            }
            if (edt.topTileset != null && ShowTopCheckBox.IsChecked == true)
            {
                for (int y = 0; y < edt.MapHeight; y++)
                {
                    for (int x = 0; x < edt.MapWidth; x++)
                    {
                        int selectedX = edt.TileSize * (edt.baseTiles[x + y * edt.MapWidth] % edt.baseTilesetColumns);
                        int selectedY = edt.TileSize * (edt.baseTiles[x + y * edt.MapWidth] / edt.baseTilesetColumns);

                    }
                }
            }
            mapWindow.Display();
        }

        private void ButtonLoadBaseTileset_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Tilemaps (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                Uri imageUri = new Uri(openFileDialog.FileName);
                MessageBox.Show(imageUri.LocalPath);
                edt.baseTileset = new Texture(imageUri.LocalPath);

                if (edt.baseTileset.Size.X % edt.TileSize != 0 || edt.baseTileset.Size.Y % edt.TileSize != 0 || edt.TileSize <= 0)
                {
                    MessageBox.Show("Invalid tile size for this image. Tileset size not divisible by tile size", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    edt.baseTileset = null;
                }
            }
            UpdateMapPreview();
        }

        private void ButtonLoadTopTileset_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Tilemaps (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp";
            Nullable<bool> result = openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                Uri imageUri = new Uri(openFileDialog.FileName);
                edt.baseTileset = new Texture(imageUri.AbsoluteUri);

                if (edt.topTileset.Size.X % edt.TileSize != 0 || edt.topTileset.Size.Y % edt.TileSize != 0 || edt.TileSize <= 0)
                {
                    MessageBox.Show("Invalid tile size for this image. Tileset size not divisible by tile size", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    edt.topTileset = null;
                }
            }
            UpdateMapPreview();
        }

        private int MousePos2SelectedTile(Point mousePos, float TilesetWidth, float TilesetHeight)
        {
            return Convert.ToInt32(Math.Floor((mousePos.X / TilesetWidth) * edt.baseTilesetColumns))
             + Convert.ToInt32(Math.Floor((mousePos.Y / TilesetHeight) * edt.baseTilesetRows)) * edt.baseTilesetColumns;
        }
        private void SelectTile_Click(object sender, RoutedEventArgs e)
        {
            edt.selectedTile = MousePos2SelectedTile(System.Windows.Input.Mouse.GetPosition(this), 1, 1);
            selectedTileText.Text = "Selected Tile: " + edt.selectedTile.ToString();
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

            TileSizeLabel.Text = "Tile Size: " + edt.TileSize;
            MapSizeLabel.Text = "Map Size: " + edt.MapWidth + "x" + edt.MapHeight;
            UpdateMapPreview();
        }

        private void ClickedOnMap(object sender, RoutedEventArgs e)
        {
            
            UpdateMapPreview();
        }

        private void UpdateCheckBoxes(object sender, RoutedEventArgs e) => UpdateMapPreview();

        private void DrawSurface_SizeChanged(object sender, EventArgs e)
        {

        }
    }
}
