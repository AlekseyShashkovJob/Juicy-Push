using UnityEngine;
using System.Collections.Generic;

namespace GameCore.Level
{
    public class LevelGenerator : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _perimeterWallPrefab;
        [SerializeField] private GameObject[] _wallVariants;
        [SerializeField] private GameObject _basketPrefab;
        [SerializeField] private GameObject _nestPrefab;
        [SerializeField] private GameObject _chickenPrefab;
        [SerializeField] private GameObject _tilePrefab;

        [SerializeField] private Transform _gameFieldParent;

        private readonly List<GameObject> SpawnedObjects = new();
        private readonly int TileSize = 100;

        private GameObjectPool _perimeterWallPool;
        private GameObjectPool[] _wallPools;
        private GameObjectPool _basketPool;
        private GameObjectPool _nestPool;
        private GameObjectPool _chickenPool;
        private GameObjectPool _tilePool;

        private Player.ChickenController _chickenController;

        private void Awake()
        {
            _perimeterWallPool = new GameObjectPool(_perimeterWallPrefab, 50, _gameFieldParent);

            _wallPools = new GameObjectPool[_wallVariants.Length];
            for (int i = 0; i < _wallVariants.Length; i++)
                _wallPools[i] = new GameObjectPool(_wallVariants[i], 5, _gameFieldParent);

            _basketPool = new GameObjectPool(_basketPrefab, 6, _gameFieldParent);
            _nestPool = new GameObjectPool(_nestPrefab, 6, _gameFieldParent);
            _chickenPool = new GameObjectPool(_chickenPrefab, 1, _gameFieldParent);
            _tilePool = new GameObjectPool(_tilePrefab, 150, _gameFieldParent);
        }

        public void BuildLevel(LevelData data)
        {
            ClearLevel();

            if (GridManager.Instance != null)
            {
                // Получаем все ключи, чтобы не изменять коллекцию во время обхода
                var keys = new List<Vector2Int>(GridManager.Instance.GetAllPositions());
                foreach (var pos in keys)
                    GridManager.Instance.UnregisterObject(pos);
            }

            var grid = data.gridSize;

            Vector3 bottomLeft = GetCenteredBottomLeft(grid, TileSize);

            // --- Тайлы ---
            for (int x = 0; x < grid.x; x++)
                for (int y = 0; y < grid.y; y++)
                    SpawnAt(_tilePool, new Vector2Int(x, y), bottomLeft, TileSize, isTile: true);

            // --- Генерируем стены по периметру ---
            HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
            for (int x = 0; x < grid.x; x++)
            {
                wallPositions.Add(new Vector2Int(x, 0));
                wallPositions.Add(new Vector2Int(x, grid.y - 1));
            }
            for (int y = 1; y < grid.y - 1; y++)
            {
                wallPositions.Add(new Vector2Int(0, y));
                wallPositions.Add(new Vector2Int(grid.x - 1, y));
            }

            // --- Периметр ---
            foreach (var pos in wallPositions)
            {
                var wall = SpawnAt(_perimeterWallPool, pos, bottomLeft, TileSize);
                GameCore.GridManager.Instance.RegisterObject(pos, wall);
            }

            // --- Внутренние стены ---
            if (data.wallPositions != null)
            {
                foreach (var pos in data.wallPositions)
                {
                    if (!wallPositions.Contains(pos))
                    {
                        int wallVariant = Random.Range(0, _wallPools.Length);
                        var wall = SpawnAt(_wallPools[wallVariant], pos, bottomLeft, TileSize);
                        GridManager.Instance.RegisterObject(pos, wall);
                    }
                }
            }

            // --- Корзины ---
            foreach (var pos in data.basketPositions)
                if (!wallPositions.Contains(pos))
                {
                    var basket = SpawnAt(_basketPool, pos, bottomLeft, TileSize);
                    if (basket.TryGetComponent<BasketController>(out var basketCtrl))
                        basketCtrl.Init(pos, TileSize, bottomLeft);
                    GridManager.Instance.RegisterObject(pos, basket);
                }

            // --- Гнёзда ---
            foreach (var pos in data.nestPositions)
                if (!wallPositions.Contains(pos))
                    SpawnAt(_nestPool, pos, bottomLeft, TileSize);

            // --- Курица ---
            if (!wallPositions.Contains(data.chickenPosition))
            {
                GameObject chickenGO = SpawnAt(_chickenPool, data.chickenPosition, bottomLeft, TileSize);
                _chickenController = chickenGO.GetComponent<Player.ChickenController>();
                if (_chickenController != null)
                {
                    _chickenController.Init(data.chickenPosition, TileSize, bottomLeft);
                    GridManager.Instance.RegisterObject(data.chickenPosition, chickenGO);
                    GridManager.Instance.SubscribeChicken(_chickenController);
                }
            }
        }

        private Vector3 GetCenteredBottomLeft(Vector2Int grid, int tileSize)
        {
            float x = -((grid.x * tileSize) / 2f) + tileSize / 2f;
            float y = -((grid.y * tileSize) / 2f) + tileSize / 2f;
            return new Vector3(x, y, 0f);
        }

        private GameObject SpawnAt(GameObjectPool pool, Vector2Int gridPos, Vector3 bottomLeft, int tileSize, bool isTile = false)
        {
            GameObject obj = pool.Get();
            obj.transform.SetParent(_gameFieldParent);

            if (obj.TryGetComponent<RectTransform>(out var rect))
                rect.anchoredPosition = new Vector2(bottomLeft.x + gridPos.x * tileSize, bottomLeft.y + gridPos.y * tileSize);
            else
                obj.transform.localPosition = bottomLeft + new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0);

            if (isTile)
                obj.transform.SetAsFirstSibling();
            else
                obj.transform.SetAsLastSibling();

            SpawnedObjects.Add(obj);
            return obj;
        }

        private void ClearLevel()
        {
            if (_chickenController != null)
            {
                GridManager.Instance.UnsubscribeChicken(_chickenController);
                _chickenController = null;
            }
            foreach (var obj in SpawnedObjects)
            {
                if (obj == null) continue;
                switch (obj.tag)
                {
                    case "PWall":
                        _perimeterWallPool.Return(obj);
                        break;
                    case "Wall":
                        foreach (var pool in _wallPools)
                        {
                            if (pool.Prefab.name == obj.name.Replace("(Clone)", "").Trim())
                            {
                                pool.Return(obj);
                                break;
                            }
                        }
                        break;
                    case "Basket":
                        _basketPool.Return(obj);
                        break;
                    case "Nest":
                        _nestPool.Return(obj);
                        break;
                    case "Chicken":
                        _chickenPool.Return(obj);
                        break;
                    case "Tile":
                        _tilePool.Return(obj);
                        break;
                    default:
                        obj.SetActive(false);
                        break;
                }
            }
            SpawnedObjects.Clear();
        }
    }
}