using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gazeus.DesafioMatch3
{
    public class ParticlePool : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string name;
            public GameObject prefab;
            public int size;
        }

        public static ParticlePool Instance;
        [SerializeField]
        private List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if (Instance == null) 
            {
                Instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.name, objectPool);
            }
        }

        public GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(name))
            {
                Debug.LogWarning("Pool com tag " + name + " não existe.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[name].Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            poolDictionary[name].Enqueue(objectToSpawn);

            return objectToSpawn;
        }
    }
}
