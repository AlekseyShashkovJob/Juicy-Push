using Misc.Services;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Level
{
    public class BasketController : MonoBehaviour
    {
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _onNestSprite;
        [SerializeField] private Image _spriteRenderer;

        private Vector2Int _gridPos;
        private RectTransform _rectTransform;
        private Vector3 _bottomLeftOffset;
        private int _tileSize;

        private bool _isOnNest = false;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Init(Vector2Int startGridPos, int tileSize, Vector3 bottomLeftOffset)
        {
            _gridPos = startGridPos;
            _tileSize = tileSize;
            _bottomLeftOffset = bottomLeftOffset;
            UpdateVisual();
        }

        public void MoveTo(Vector2Int newPos)
        {
            _gridPos = newPos;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (_rectTransform != null)
                _rectTransform.anchoredPosition = new Vector2(_bottomLeftOffset.x + _gridPos.x * _tileSize, _bottomLeftOffset.y + _gridPos.y * _tileSize);
            else
                transform.localPosition = _bottomLeftOffset + new Vector3(_gridPos.x * _tileSize, _gridPos.y * _tileSize, 0);

            transform.SetAsLastSibling();
            SetOnNest(_isOnNest);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Nest") && !_isOnNest)
            {
                _isOnNest = true;
                SetOnNest(true);
                VibroManager.Vibrate();
                GameCore.GameManager.Instance.BasketPlacedInNest();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == null || !other.gameObject.activeInHierarchy || GameCore.GameManager.Instance == null)
                return;

            if (other.CompareTag("Nest") && _isOnNest)
            {
                _isOnNest = false;
                SetOnNest(false);
                GameCore.GameManager.Instance.BasketRemovedFromNest();
            }
        }

        private void SetOnNest(bool onNest)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.sprite = onNest ? _onNestSprite : _normalSprite;
        }
    }
}