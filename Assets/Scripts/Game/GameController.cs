using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MatchThree
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;
        [SerializeField] private TileField _tileField;
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        public static GameController Instance { get; private set; }
        
        public GameConfig Config => _config;
        
        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _tileField.Initialize();
        }

        private void OnMatch()
        {
            
        }
    }
}
