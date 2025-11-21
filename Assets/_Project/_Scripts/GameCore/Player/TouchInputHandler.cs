using UnityEngine;

namespace GameCore.Player
{
    public class TouchInputHandler : IInputHandler
    {
        private Vector2 _touchStartPos;
        private bool _isTouching = false;

        public void HandleInput(ref Vector2 moveDirection)
        {
            moveDirection = Vector2.zero;

            // Блокируем инпут во время паузы!
            if (Time.timeScale == 0f)
                return;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _touchStartPos = touch.position;
                        _isTouching = true;
                        break;

                    case TouchPhase.Moved:
                        if (_isTouching)
                        {
                            Vector2 swipeDelta = touch.position - _touchStartPos;
                            if (swipeDelta.magnitude > 50f)
                            {
                                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                                    moveDirection = swipeDelta.x > 0 ? Vector2.right : Vector2.left;
                                else
                                    moveDirection = swipeDelta.y > 0 ? Vector2.up : Vector2.down;
                                _isTouching = false;
                            }
                        }
                        break;

                    case TouchPhase.Ended:
                        _isTouching = false;
                        break;
                }
            }
        }

        // Сброс состояния при паузе
        public void Reset()
        {
            _isTouching = false;
        }
    }
}