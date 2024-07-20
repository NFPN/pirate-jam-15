using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ObjectPooler : MonoBehaviour
    {
        [Serializable]
        public class Pool
        {
            public string name;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolsDict;

        private void Awake()
        {
            poolsDict = new Dictionary<string, Queue<GameObject>>();

            foreach (var pool in pools)
            {
                var objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                    AddNewObj(pool, objectPool);

                poolsDict.Add(pool.name, objectPool);
            }
        }

        private static void AddNewObj(Pool pool, Queue<GameObject> objectPool)
        {
            var obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }

        public GameObject GetObj(string name)
        {
            if (!poolsDict.ContainsKey(name))
                return null;

            var objectToSpawn = poolsDict[name].Dequeue();

            if (objectToSpawn != null)
            {
                objectToSpawn.SetActive(true);
                poolsDict[name].Enqueue(objectToSpawn);
            }

            return objectToSpawn;
        }
    }
}
