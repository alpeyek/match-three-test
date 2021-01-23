using System;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThree
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _selection;

        private event Action _onClicked = delegate { };

        private RectTransform _rectTransform;
        private bool _isSelected;
        public int Row { get; private set; }
        public int Col { get; private set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Bind(TileInfo tileInfo, Vector2 position, float size,
            int row, int col, Action clickCallback = null)
        {
            _image.sprite = tileInfo.Sprite;
            _image.color = tileInfo.Color;

            _rectTransform.anchoredPosition = position;
            _rectTransform.sizeDelta = new Vector2(size, size);

            _isSelected = false;
            _onClicked = clickCallback;

            Row = row;
            Col = col;
            
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _isSelected = !_isSelected;
            _selection.SetActive(_isSelected);
            
            _onClicked?.Invoke();
        }
    }
}