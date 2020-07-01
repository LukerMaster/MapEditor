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
using System.Drawing;
using System.IO;

namespace MapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateMapPreview();
        }


        Editor edt = new Editor();

        private void UpdateMapPreview()
        {
            if (true)
            {
                Bitmap mapPreview = new Bitmap(edt.TileSize * edt.MapWidth, edt.TileSize * edt.MapHeight);
                if (edt.baseTileset != null && ShowBaseCheckBox.IsChecked == true)
                {
                    Bitmap tilemapBitmap = BitmapImage2Bitmap(edt.baseTileset);

                    for (int y = 0; y < edt.MapHeight; y++)
                    {
                        for (int x = 0; x < edt.MapWidth; x++)
                        {
                            int selectedX = edt.TileSize * (edt.baseTiles[x + y * edt.MapWidth] % edt.baseTilesetColumns);
                            int selectedY = edt.TileSize * (edt.baseTiles[x + y * edt.MapWidth] / edt.baseTilesetColumns);
                            CopyRegionIntoImage(tilemapBitmap, new Rectangle(selectedX, selectedY, edt.TileSize, edt.TileSize), ref mapPreview, new Rectangle(x * edt.TileSize, y * edt.TileSize, edt.TileSize, edt.TileSize));
                        }
                    }
                }
                if (edt.topTileset != null && ShowTopCheckBox.IsChecked == true)
                {
                    Bitmap tilemapBitmap = BitmapImage2Bitmap(edt.topTileset);

                    for (int y = 0; y < edt.MapHeight; y++)
                    {
                        for (int x = 0; x < edt.MapWidth; x++)
                        {
                            int selectedX = edt.TileSize * (edt.topTiles[x + y * edt.MapWidth] % edt.topTilesetColumns);
                            int selectedY = edt.TileSize * (edt.topTiles[x + y * edt.MapWidth] / edt.topTilesetColumns);
                            CopyRegionIntoImage(tilemapBitmap, new Rectangle(selectedX, selectedY, edt.TileSize, edt.TileSize), ref mapPreview, new Rectangle(x * edt.TileSize, y * edt.TileSize, edt.TileSize, edt.TileSize));
                        }
                    }
                }

                if (MapImage != null)
                {
                    MapImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                       mapPreview.GetHbitmap(),
                                        IntPtr.Zero,
                                        Int32Rect.Empty,
                                        BitmapSizeOptions.FromWidthAndHeight(edt.TileSize * edt.MapWidth, edt.TileSize * edt.MapHeight));
                }
            }
            MessageBox.Show("XD");
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
        {
           using (Graphics grD = Graphics.FromImage(destBitmap))
           {
                grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
           }
        }

        private void ButtonLoadBaseTileset_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Tilemaps (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp";
            Nullable<bool> result = openFileDialog.ShowDialog();
            
            if (openFileDialog.FileName != "")
            {
                Uri imageUri = new Uri(openFileDialog.FileName);
                edt.baseTileset = new BitmapImage(new Uri(openFileDialog.FileName));

                if (edt.baseTileset.PixelWidth % edt.TileSize == 0 && edt.baseTileset.PixelHeight % edt.TileSize == 0 && edt.TileSize > 0)
                {
                    TilesetImage.Source = edt.baseTileset;
                }
                else
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
                edt.topTileset = new BitmapImage(new Uri(openFileDialog.FileName));

                if (edt.topTileset.PixelWidth % edt.TileSize == 0 && edt.topTileset.PixelHeight % edt.TileSize == 0 && edt.TileSize > 0)
                {
                    TilesetImage.Source = edt.topTileset;
                }
                else
                {
                    MessageBox.Show("Invalid tile size for this image. Tileset size not divisible by tile size", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    edt.topTileset = null;
                }
            }
            UpdateMapPreview();
        }

        private void SelectTile_Click(object sender, RoutedEventArgs e)
        {
            edt.selectedTile = Convert.ToInt32(Math.Floor((Mouse.GetPosition(TilesetImage).X / TilesetImage.ActualWidth) * edt.baseTilesetColumns))
             + Convert.ToInt32(Math.Floor((Mouse.GetPosition(TilesetImage).Y / TilesetImage.ActualHeight) * edt.baseTilesetRows)) * edt.baseTilesetColumns;
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
            edt.baseTiles[Convert.ToInt32(Math.Floor((Mouse.GetPosition(MapImage).X / MapImage.ActualWidth) * edt.MapWidth))
             + Convert.ToInt32(Math.Floor((Mouse.GetPosition(MapImage).Y / MapImage.ActualHeight) * edt.MapHeight)) * edt.MapWidth] = edt.selectedTile;
            UpdateMapPreview();
        }

        private void UpdateCheckBoxes(object sender, RoutedEventArgs e) => UpdateMapPreview();
    }
}
