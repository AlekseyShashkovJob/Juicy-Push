using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Player
{
    public class ChickenController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveTime = 0.15f;
        [SerializeField] private int _tileSize = 100;
        [Header("Sprites")]
        [SerializeField] private Sprite _idleSprite;
        [SerializeField] private Sprite _moveSprite;
        [Header("Components")]
        [SerializeField] private Image _spriteRenderer;

        private enum State { Idle, Moving }
        private State _state = State.Idle;

        private Vector2Int _gridPos;
        private Vector2 _targetPosition;
        private Vector2Int _lastMoveDir;
        private RectTransform _rectTransform;
        private bool _isMoving = false;

        private IInputHandler _inputHandler;
        private Vector3 _bottomLeftOffset = Vector3.zero;

        public event Action<GameObject, Vector2Int, Vector2Int> OnMoveRequest;

        private void Awake()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<Image>();

            _inputHandler = Application.isMobilePlatform ? new TouchInputHandler() : new MouseInputHandler();
        }

        private void Update()
        {
            if (_isMoving)
            {
                _rectTransform.anchoredPosition = Vector2.MoveTowards(
                    _rectTransform.anchoredPosition, _targetPosition, (_tileSize / _moveTime) * Time.deltaTime);

                if (Vector2.Distance(_rectTransform.anchoredPosition, _targetPosition) < 0.1f)
                {
                    _rectTransform.anchoredPosition = _targetPosition;
                    _isMoving = false;
                    SetState(State.Idle);
                }
                return;
            }

            if (Time.timeScale == 0f)
            {
                PauseInputReset();
                return;
            }

            Vector2 moveDir = Vector2.zero;
            _inputHandler.HandleInput(ref moveDir);

            Vector2Int dir = Vector2Int.zero;
            if (moveDir.sqrMagnitude > 0.1f)
            {
                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                    dir = moveDir.x > 0 ? Vector2Int.right : Vector2Int.left;
                else
                    dir = moveDir.y > 0 ? Vector2Int.up : Vector2Int.down;
            }

            if (dir != Vector2Int.zero)
            {
                OnMoveRequest?.Invoke(gameObject, _gridPos, dir);
            }
        }

        public void Init(Vector2Int startGridPos, int tileSize, Vector3 bottomLeftOffset)
        {
            _tileSize = tileSize;
            _gridPos = startGridPos;
            _rectTransform = GetComponent<RectTransform>();
            _bottomLeftOffset = bottomLeftOffset;
            _targetPosition = GridToWorld(_gridPos);
            _rectTransform.anchoredPosition = _targetPosition;
            SetState(State.Idle);
        }

        public void MoveTo(Vector2Int newPos, Vector2Int moveDir)
        {
            _gridPos = newPos;
            _lastMoveDir = moveDir;
            _targetPosition = GridToWorld(_gridPos);

            if (_lastMoveDir.x != 0)
            {
                Vector3 scale = _rectTransform.localScale;
                scale.x = Mathf.Sign(_lastMoveDir.x);
                _rectTransform.localScale = scale;
            }

            SetState(State.Moving);
            _isMoving = true;
        }

        private void SetState(State state)
        {
            _state = state;
            if (_spriteRenderer != null)
                _spriteRenderer.sprite = (state == State.Idle) ? _idleSprite : _moveSprite;
        }

        private Vector2 GridToWorld(Vector2Int gridPos)
        {
            return new Vector2(_bottomLeftOffset.x + gridPos.x * _tileSize, _bottomLeftOffset.y + gridPos.y * _tileSize);
        }

        private void PauseInputReset()
        {
            if (_inputHandler is MouseInputHandler mouse)
                mouse.Reset();
            else if (_inputHandler is TouchInputHandler touch)
                touch.Reset();
        }
    }
}