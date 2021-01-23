using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MatchThree
{
    [System.Serializable]
    public class TileInfo
    {
        public int Id;
        public Sprite Sprite;
        public Color Color;
    }
    
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Data/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private TileInfo[] _tileInfos;
        [SerializeField] private int _tileSize = 16;
        [SerializeField] private float _tileMoveSpeed = 32f;
        [SerializeField] private int _tileFieldWidth = 6;
        [SerializeField] private int _tileFieldHeight = 6;
        [SerializeField] private int _tileMatchCount = 3;

        public TileInfo[] TileInfos => _tileInfos;
        public int TileSize => _tileSize;
        public float TileMoveSpeed => _tileMoveSpeed;
        public int TileFieldWidth => _tileFieldWidth;
        public int TileFieldHeight => _tileFieldHeight;
        public int TileMatchCount => _tileMatchCount;

        public TileInfo GetRandomTileInfo()
        {
            return _tileInfos[Random.Range(0, _tileInfos.Length)];
        }
        
        public TileInfo GetNextTileInfo(TileInfo tileInfo)
        {
            return _tileInfos[(Array.IndexOf(_tileInfos, tileInfo) + 1) % _tileInfos.Length];
        }
    }
}
