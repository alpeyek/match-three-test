using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree
{
    [System.Serializable]
    public class TileInfo
    {
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

        public TileInfo[] TileInfos => _tileInfos;
        public int TileSize => _tileSize;
        public float TileMoveSpeed => _tileMoveSpeed;
        public int TileFieldWidth => _tileFieldWidth;
        public int TileFieldHeight => _tileFieldHeight;
    }
}
