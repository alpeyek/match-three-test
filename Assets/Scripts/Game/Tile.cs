using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThree
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _selection;
        
        [HideInInspector] public int Row;
        [HideInInspector] public int Col;

        private event Action<Tile> _onClicked = delegate { };

        private RectTransform _rectTransform;
        private bool _isSelected;

        public int TypeId { get; private set; }

        public RectTransform Rect => _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Bind(TileInfo tileInfo, Vector2 position, float size,
            int row, int col, Action<Tile> clickCallback = null)
        {
            _image.sprite = tileInfo.Sprite;
            _image.color = tileInfo.Color;

            Rect.anchoredPosition = position;
            Rect.sizeDelta = new Vector2(size, size);

            _isSelected = false;
            _onClicked = clickCallback;

            Row = row;
            Col = col;

            TypeId = tileInfo.Id;
            
            _button.onClick.RemoveListener(OnClick);
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            SetSelected(!_isSelected);
            
            _onClicked?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            _selection.SetActive(selected);
        }

        public Tweener Move(Tile otherTile)
        {
            return Rect.DOAnchorPos(otherTile.Rect.anchoredPosition, 0.5f)
                .SetEase(Ease.InOutSine);
        }

        public void Match()
        {
            gameObject.SetActive(false);
        }
    }
}