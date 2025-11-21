using UnityEngine;

namespace GameCore.Player
{
    public class MouseInputHandler : IInputHandler
    {
        private Vector2 _mouseStartPos;
        private bool _isMouseDown = false;

        public void HandleInput(ref Vector2 moveDirection)
        {
            moveDirection = Vector2.zero;

            // Блокируем инпут во время паузы!
            if (Time.timeScale == 0f)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                _mouseStartPos = Input.mousePosition;
                _isMouseDown = true;
            }

            if (_isMouseDown && Input.GetMouseButtonUp(0))
            {
                Vector2 mouseEndPos = Input.mousePosition;
                Vector2 swipeDelta = mouseEndPos - _mouseStartPos;

                if (swipeDelta.magnitude > 50f)
                {
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        moveDirection = swipeDelta.x > 0 ? Vector2.right : Vector2.left;
                    else
                        moveDirection = swipeDelta.y > 0 ? Vector2.up : Vector2.down;
                }

                _isMouseDown = false;
            }
        }

        // Сброс состояния при паузе
        public void Reset()
        {
            _isMouseDown = false;
        }
    }
}