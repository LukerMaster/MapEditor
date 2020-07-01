using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace MapEditor
{
    class Editor
    {

        public Editor()
        {
            ResizeMap(20, 20);
        }

        private int _selectedTile;
        public int selectedTile { get => _selectedTile; set { if (value >= 0)  { _selectedTile = value; } } }

        public BitmapImage baseTileset;
        public BitmapImage topTileset;

        public int baseTilesetColumns { get => baseTileset.PixelWidth / TileSize; }
        public int baseTilesetRows { get => baseTileset.PixelHeight / TileSize; }

        public int topTilesetColumns { get => topTileset.PixelWidth / TileSize; }
        public int topTilesetRows { get => topTileset.PixelHeight / TileSize; }

        public List<int> baseTiles = new List<int>();
        public List<int> topTiles = new List<int>();
        public List<int> hitboxes = new List<int>();

        private int _tileSize = 32;
        private int _mapWidth = 20;
        private int _mapHeight = 20;

        public int TileSize { get => _tileSize; set { if (value > 0) _tileSize = value; } }
        public int MapWidth { get => _mapWidth; }
        public int MapHeight { get => _mapHeight; }

        public void ResizeMap(int x, int y)
        {
            baseTiles.Clear();
            for (int i = 0; i < x * y; i++)
                baseTiles.Add(0);
            topTiles.Clear();
            for (int i = 0; i < x * y; i++)
                topTiles.Add(0);
            hitboxes.Clear();
            for (int i = 0; i < x * y; i++)
                hitboxes.Add(0);

            _mapWidth = x;
            _mapHeight = y;
        }
    }
}
