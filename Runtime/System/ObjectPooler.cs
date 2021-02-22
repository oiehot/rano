using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rano
{   
    [System.Serializable]
    public class ObjectPoolItem
    {
        public string name;
        public GameObject gameObject;
        public int amount;
    }

    public class ObjectPooler : MonoBehaviour
    {
        private int count = 0;
        private Dictionary<string, List<GameObject>> pools;
        public List<ObjectPoolItem> items;
        public HideFlags defaultHideFlags = HideFlags.HideInHierarchy;

        void Awake()
        {
            this.pools = new Dictionary<string, List<GameObject>>();
        }

        public void Init()
        {
            foreach (ObjectPoolItem item in items)
            {
                GameObject srcGameObject = item.gameObject;
                string srcName = item.name;
                if (srcName == null)
                {
                    throw new Exception("Item must have a name");
                }
                var pool = new List<GameObject>();
                this.pools[srcName] = pool;

                for (int i=0; i<item.amount; i++)
                {
                    GameObject obj = (GameObject)Instantiate(srcGameObject);
                    obj.name = $"{srcName}_{i}";
                    obj.hideFlags = defaultHideFlags;
                    obj.SetActive(false);
                    pool.Add(obj);
                }
            }
        }

        public GameObject Get(string name)
        {
            if (!this.pools.ContainsKey(name)) return null;
            List<GameObject> pool = this.pools[name];

            for (int i=0; i<pool.Count; i++)
            {
                if (!pool[i].activeInHierarchy)
                {
                    return pool[i];
                }
            }
            return null;
        }
    }
}