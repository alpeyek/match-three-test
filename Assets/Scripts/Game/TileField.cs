using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
            {
                if (_tiles[i][row] != null &&_tiles[i][row].TypeId == typeId)
                    sameTypeTilesRow++;
                else
                    break;
            }
            
            for (int i = col + 1; i < _tiles.Count; i++)
            {
                if (_tiles[i][row] != null && _tiles[i][row].TypeId == typeId)
                    sameTypeTilesRow++;
                else
                    break;
            }
            
            for (int i = row - 1; i >= 0; i--)
            {
                if (_tiles[col][i] != null && _tiles[col][i].TypeId == typeId)
                    sameTypeTilesCol++;
                else
                    break;
            }
            
            for (int i = row + 1; i < _tiles[col].Count; i++)
            {
                if (_tiles[col][i] != null && _tiles[col][i].TypeId == typeId)
                    sameTypeTilesCol++;
                else
                    break;
            }
            
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

        private bool CheckMatch(Tile tile)
        {
            if (tile == null || !tile.gameObject.activeSelf)
                return false;
            
            var matchedHorizontal = IsHorizontalMatch(tile, out var matchedTilesHor);
            var matchedVertical = IsVerticalMatch(tile, out var matchedTilesVert);

            if (matchedHorizontal)
            {
                var tilesShiftedCount = 0;
                foreach (var matchedTile in matchedTilesHor)
                {
                    matchedTile.Match();
                    
                    var shiftedTiles = new List<Tile>();
                    ShiftTileColumn(matchedTile.Col, matchedTile.Row, 1, in shiftedTiles,
                        () =>
                        {
                            if (++tilesShiftedCount >= matchedTilesHor.Count)
                            {
                                CheckMatchForTiles(shiftedTiles);
                            }
                        });
                }
                    
            }
            
            if (matchedVertical)
            {
                foreach (var matchedTile in matchedTilesVert)
                    matchedTile.Match();

                var firstTile = matchedTilesVert.OrderBy(t => t.Rect.anchoredPosition.y).Last();
                var shiftedTiles = new List<Tile>();
                ShiftTileColumn(firstTile.Col, firstTile.Row, matchedTilesVert.Count, in shiftedTiles,
                    () => CheckMatchForTiles(shiftedTiles));
            }

            return matchedHorizontal || matchedVertical;
        }

        private void CheckMatchForTiles(List<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                CheckMatch(tile);
            }
        }

        private bool IsHorizontalMatch(Tile tile, out List<Tile> matchedTiles)
        {
            matchedTiles = new List<Tile> {tile};
            var row = tile.Row;

            for (int i = tile.Col + 1; i < _tiles.Count; i++)
            {
                if (_tiles[i][row].TypeId == tile.TypeId)
                    matchedTiles.Add(_tiles[i][row]);
                else
                    break;
            }
            
            for (int i = tile.Col - 1; i >= 0; i--)
            {
                if (_tiles[i][row].TypeId == tile.TypeId)
                    matchedTiles.Add(_tiles[i][row]);
                else
                    break;
            }

            return matchedTiles.Count >= GameController.Instance.Config.TileMatchCount;
        }
        
        private bool IsVerticalMatch(Tile tile, out List<Tile> matchedTiles)
        {
            matchedTiles = new List<Tile> {tile};
            var col = tile.Col;

            for (int i = tile.Row + 1; i < _tiles[col].Count; i++)
            {
                if (_tiles[col][i].TypeId == tile.TypeId)
                    matchedTiles.Add(_tiles[col][i]);
                else
                    break;
            }
            
            for (int i = tile.Row - 1; i >= 0; i--)
            {
                if (_tiles[col][i].TypeId == tile.TypeId)
                    matchedTiles.Add(_tiles[col][i]);
                else
                    break;
            }

            return matchedTiles.Count >= GameController.Instance.Config.TileMatchCount;
        }

        private void SetTile(Tile tile, int row, int col)
        {
            _tiles[col][row] = tile;
        }
        
        private void SwapTiles(Tile tile1, Tile tile2)
        {
            Tile tTile = tile1;
            int tRow = tTile.Row;
            int tCol = tTile.Col;
            
            SetTile(tile2, tile1.Row, tile1.Col);
            SetTile(tTile, tile2.Row, tile2.Col);

            tile1.SetRowCol(tile2.Row, tile2.Col);
            tile2.SetRowCol(tRow, tCol);
        }

        private void OnTileClick(Tile tile)
        {
            if (tile == _selectedTile)
                return;

            if (_selectedTile == null)
            {
                _selectedTile = tile;
                _selectedTile.SetSelected(true);
                return;
            }

            if (AreTilesAdjacent(_selectedTile, tile))
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_selectedTile.MoveTo(tile));
                sequence.Join(tile.MoveTo(_selectedTile));
                sequence.AppendCallback(() =>
                {
                    SwapTiles(_selectedTile, tile);

                    CheckMatch(_selectedTile);
                    CheckMatch(tile);
                    
                    _selectedTile = null;
                });
                sequence.Play();
                
                _selectedTile.SetSelected(false);
                tile.SetSelected(false);
            }
            else
            {
                _selectedTile.SetSelected(false);
                _selectedTile = tile;
                _selectedTile.SetSelected(true);
            }
        }

        private void ShiftTileColumn(int col, int startIndex, int tilesCount, in List<Tile> shiftedTiles, Action callback)
        {
            if (startIndex + tilesCount >= _tiles[col].Count)
                return;
            
            var config = GameController.Instance.Config;
            var sequence = DOTween.Sequence();
            var moveToPos = Vector2.zero;

            for (int i = 0; i < _tiles[col].Count; i++)
            {
                if (_tiles[col][i] != null && _tiles[col][i].gameObject.activeSelf)
                {
                    moveToPos = _tiles[col][i].Rect.anchoredPosition - new Vector2(0f, config.TileSize);
                }
            }

            for (int i = startIndex + tilesCount, j = 0; i < _tiles[col].Count; i++, j++)
            {
                var tile = _tiles[col][i];
                tile.SetRowCol(startIndex + j, col);
                _tiles[col][startIndex + j] = tile;
                
                sequence.Join(tile.MoveTo(moveToPos));
                moveToPos.y -= config.TileSize;

                shiftedTiles.Add(tile);
            }
            
            sequence.AppendCallback(() => callback?.Invoke());

            sequence.Play();
        }
    }
}
