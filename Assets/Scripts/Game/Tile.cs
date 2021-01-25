using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThree
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _selection;
        [SerializeField] private TextMeshProUGUI _text;

        private event Action<Tile> _onClicked = delegate { };

        private RectTransform _rectTransform;
        private bool _isSelected;

        public int TypeId { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public RectTransform Rect => _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Bind(TileInfo tileInfo, Vector2 position, float size,
            int row, int col, Action<Tile> clickCallback = null)
        {
            _image.sprite = tileInfo.Sprite;

            Rect.anchoredPosition = position;
            Rect.sizeDelta = new Vector2(size, size);

            _isSelected = false;
            _onClicked = clickCallback;

            SetRowCol(row, col);

            TypeId = tileInfo.Id;
            
            _button.onClick.RemoveListener(OnClick);
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            Debug.Log($"{Row}, {Col}");
            SetSelected(!_isSelected);
            
            _onClicked?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            _selection.SetActive(selected);
        }

        public void SetRowCol(int row, int col)
        {
            Row = row;
            Col = col;
            _text.text = $"{Row}, {Col}";
        }

        public Tweener MoveTo(Tile otherTile)
        {
            return MoveTo(otherTile.Rect.anchoredPosition);
        }
        
        public Tweener MoveTo(Vector2 pos)
        {
            return Rect.DOAnchorPos(pos, 0.5f).SetEase(Ease.InOutSine);
        }

        public void Match()
        {
            gameObject.SetActive(false);
        }
    }
}