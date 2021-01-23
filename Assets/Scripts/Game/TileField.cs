using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MathThree;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MatchThree
{
    public class TileField : MonoBehaviour
    {
        [SerializeField] private GameObjectPool _tilePool;

        private List<List<Tile>> _tiles = new List<List<Tile>>();
        private Tile _selectedTile = null;

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

            for (int col = 0; col < config.TileFieldHeight; col++)
            {
                List<Tile> tileCol = new List<Tile>();
                _tiles.Add(tileCol);
                
                for (int row = 0; row < config.TileFieldWidth; row++)
                {
                    var tile = _tilePool.Get<Tile>(transform);

                    var tileInfo = config.GetRandomTileInfo();
                    while (!CanPlaceTile(tileInfo.Id, row, col))
                    {
                        tileInfo = config.GetNextTileInfo(tileInfo);
                    }
                    
                    tile.Bind(tileInfo, tilePos, config.TileSize, row, col, OnTileClick);
                    tilePos.y -= config.TileSize;
                    
                    tileCol.Add(tile);
                }

                tilePos.x += config.TileSize;
                tilePos.y = config.TileSize * config.TileFieldHeight * 0.5f;
            }
        }

        private bool CanPlaceTile(int typeId, int row, int col)
        {
            var sameTypeTilesRow = 1;
            var sameTypeTilesCol = 1;
            
            for (int i = col - 1; i >= 0; i--)
                if (_tiles[i][row] != null &&_tiles[i][row].TypeId == typeId)
                    sameTypeTilesRow++;
                else
                    break;
            
            for (int i = col + 1; i < _tiles.Count; i++)
                if (_tiles[i][row] != null && _tiles[i][row].TypeId == typeId)
                    sameTypeTilesRow++;
                else
                    break;
            
            for (int i = row - 1; i >= 0; i--)
                if (_tiles[col][i] != null && _tiles[col][i].TypeId == typeId)
                    sameTypeTilesCol++;
                else
                    break;
            
            for (int i = row + 1; i < _tiles[col].Count; i++)
                if (_tiles[col][i] != null && _tiles[col][i].TypeId == typeId)
                    sameTypeTilesCol++;
                else
                    break;
            
            return sameTypeTilesRow < GameController.Instance.Config.TileMatchCount &&
                   sameTypeTilesCol < GameController.Instance.Config.TileMatchCount;
        }

        private bool AreTilesAdjacent(Tile tile1, Tile tile2)
        {
            if (tile1.Row == tile2.Row)
            {
                if (tile1.Col == tile2.Col - 1 || tile1.Col == tile2.Col + 1)
                    return true;
            }

            if (tile1.Col == tile2.Col)
            {
                if (tile1.Row == tile2.Row - 1 || tile1.Row == tile2.Row + 1)
                    return true;
            }

            return false;
        }

        private bool IsMatch(Tile tile)
        {
            var matchedHorizontal = IsHorizontalMatch(tile, out _);
            var matchedVertical = IsVerticalMatch(tile, out _);

            return matchedHorizontal || matchedVertical;
        }

        private bool IsHorizontalMatch(Tile tile, out List<Tile> matchedTiles)
        {
            matchedTiles = new List<Tile> {tile};
            var row = tile.Row;

            for (int i = tile.Col + 1; i < _tiles.Count; i++)
                if (_tiles[i][row].TypeId == tile.TypeId)
                    matchedTiles.Add(_tiles[i][row]);
                else
                    break;
            
            for (int i = tile.Col - 1; i >= 0; i--)
                if (_tiles[i][row].TypeId == tile.TypeId)
                    matchedTiles.Add(_tiles[i][row]);
                else
                    break;

            return matchedTiles.Count >= GameController.Instance.Config.TileMatchCount;
        }
        
        private bool IsVerticalMatch(Tile tile, out List<Tile> matchedTiles)
        {
            matchedTiles = new List<Tile> {tile};
            var col = tile.Col;

            for (int i = tile.Row + 1; i < _tiles[col].Count; i++)
                if (_tiles[col][i].TypeId == tile.TypeId)
                    matchedTiles.Add(_tiles[col][i]);
                else
                    break;
            
            for (int i = tile.Row - 1; i >= 0; i--)
                if (_tiles[col][i].TypeId == tile.TypeId)
                    matchedTiles.Add(_tiles[col][i]);
                else
                    break;

            return matchedTiles.Count >= GameController.Instance.Config.TileMatchCount;
        }

        private void OnTileClick(Tile tile)
        {
            if (tile == _selectedTile)
            {
                return;
            }

            if (_selectedTile == null)
            {
                _selectedTile = tile;
                _selectedTile.SetSelected(true);
                return;
            }

            if (AreTilesAdjacent(_selectedTile, tile))
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_selectedTile.Move(tile));
                sequence.Join(tile.Move(_selectedTile));
                sequence.AppendCallback(() =>
                {
                    int tRow = _selectedTile.Row;
                    int tCol = _selectedTile.Col;

                    _selectedTile.Row = tile.Row;
                    _selectedTile.Col = tile.Row;
                    tile.Row = tRow;
                    tile.Col = tCol;

                    bool match = IsMatch(_selectedTile);
                    Debug.Log($"Match on {_selectedTile.Row}, {_selectedTile.Col}");

                    match = IsMatch(tile);
                    Debug.Log($"Match on {tile.Row}, {tile.Col}");
                });
                sequence.Play();
                
                _selectedTile.SetSelected(false);
                tile.SetSelected(false);

                _selectedTile = null;
            }
            else
            {
                _selectedTile.SetSelected(false);
                _selectedTile = tile;
                _selectedTile.SetSelected(true);
            }
        }
    }
}
