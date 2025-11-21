using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        private Dictionary<Vector2Int, GameObject> _gridObjects = new Dictionary<Vector2Int, GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public IEnumerable<Vector2Int> GetAllPositions() => _gridObjects.Keys;

        // Для регистрации объектов (стены, корзины, курицы и пр.)
        public void RegisterObject(Vector2Int pos, GameObject obj)
        {
            _gridObjects[pos] = obj;
        }

        public void UnregisterObject(Vector2Int pos)
        {
            if (_gridObjects.ContainsKey(pos))
                _gridObjects.Remove(pos);
        }

        public void UpdateGrid(GameObject obj, Vector2Int from, Vector2Int to)
        {
            if (_gridObjects.ContainsKey(from))
                _gridObjects.Remove(from);
            _gridObjects[to] = obj;
        }

        // Подписка на событие курицы
        public void SubscribeChicken(GameCore.Player.ChickenController chicken)
        {
            chicken.OnMoveRequest += HandleMoveRequest;
        }
        public void UnsubscribeChicken(GameCore.Player.ChickenController chicken)
        {
            chicken.OnMoveRequest -= HandleMoveRequest;
        }

        // Логика столкновений и толкания
        public void HandleMoveRequest(GameObject sender, Vector2Int from, Vector2Int dir)
        {
            var chicken = sender.GetComponent<GameCore.Player.ChickenController>();
            if (chicken == null) return;

            Vector2Int target = from + dir;
            if (!_gridObjects.TryGetValue(target, out var blocker))
            {
                // Нет препятствий — перемещаем курицу
                chicken.MoveTo(target, dir);
                UpdateGrid(sender, from, target);
                return;
            }

            if (blocker.CompareTag("Wall"))
            {
                // Стена — нельзя идти
                return;
            }
            if (blocker.CompareTag("PWall"))
            {
                // Стена — нельзя идти
                return;
            }
            else if (blocker.CompareTag("Basket"))
            {
                Vector2Int afterBasket = target + dir;
                if (!_gridObjects.ContainsKey(afterBasket))
                {
                    // Можно сдвинуть корзину
                    var basket = blocker.GetComponent<GameCore.Level.BasketController>();
                    if (basket != null)
                    {
                        basket.MoveTo(afterBasket);
                        UpdateGrid(blocker, target, afterBasket);

                        // Теперь двигаем курицу
                        chicken.MoveTo(target, dir);
                        UpdateGrid(sender, from, target);
                    }
                }
            }
        }
    }
}