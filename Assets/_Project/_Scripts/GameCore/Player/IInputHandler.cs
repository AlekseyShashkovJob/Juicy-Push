using UnityEngine;

namespace GameCore.Player
{
    public interface IInputHandler
    {
        void HandleInput(ref Vector2 moveDirection);
    }
}