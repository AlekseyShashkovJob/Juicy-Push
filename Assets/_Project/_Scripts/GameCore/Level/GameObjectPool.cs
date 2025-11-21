using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Level
{
    public class GameObjectPool
    {
        public GameObject Prefab { get; private set; }
        private readonly Queue<GameObject> Pool = new();
        private readonly Transform Parent;

        public GameObjectPool(GameObject prefab, int initialSize, Transform parent = null)
        {
            Prefab = prefab;
            Parent = parent;
            for (int i = 0; i < initialSize; i++)
            {
                var obj = Object.Instantiate(prefab, parent);
                obj.SetActive(false);
                Pool.Enqueue(obj);
            }
        }

        public GameObject Get()
        {
            if (Pool.Count > 0)
            {
                var obj = Pool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            var newObj = Object.Instantiate(Prefab, Parent);
            newObj.SetActive(true);
            return newObj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(Parent);
            if (!Pool.Contains(obj))
                Pool.Enqueue(obj);
        }
    }
}