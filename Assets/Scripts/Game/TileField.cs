using System;
using System.Collections;
using System.Collections.Generic;
using MathThree;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MatchThree
{
    public class TileField : MonoBehaviour
    {
        [SerializeField] private GameObjectPool _tilePool;

        private List<List<Tile>> _tiles = new List<List<Tile>>();

        public void Initialize()
        {
            GenerateField();
        }

        public void CleanUp()
        {
            foreach (var tileCol in _tiles)
                tileCol.Clear();
            _tiles.Clear();
            
            _tilePool.ReleaseAll();
        }

        private void GenerateField()
        {
            var config = GameController.Instance.Config;
            
            var tilePos = new Vector2(-config.TileSize * config.TileFieldWidth * 0.5f,
                config.TileSize * config.TileFieldHeight * 0.5f);

            for (int col = 0; col < config.TileFieldWidth; col++)
            {
                List<Tile> tileCol = new List<Tile>();
                
                for (int row = 0; row < config.TileFieldHeight; row++)
                {
                    Tile tile = _tilePool.Get<Tile>(transform);

                    TileInfo tileInfo = config.GetRandomTileInfo();
                    
                    tile.Bind(tileInfo, tilePos, config.TileSize, row, col);
                    tilePos.y -= config.TileSize;
                    
                    tileCol.Add(tile);
                }
                
                _tiles.Add(tileCol);
                
                tilePos.x += config.TileSize;
                tilePos.y = config.TileSize * config.TileFieldHeight * 0.5f;
            }
        }
    }
}
