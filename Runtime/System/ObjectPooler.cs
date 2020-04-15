namespace Rano.Core
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    
    [System.Serializable]
    public class ObjectPoolItem
    {
        public GameObject gameObject;
        public int amount;
        public bool expandable;
    }

    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance;
        public List<ObjectPoolItem> items;
        public List<GameObject> pool;

        private void Start()
        {
            pool = new List<GameObject>();
            foreach (ObjectPoolItem item in items)
            {
                for (int i = 0; i < item.amount; i++)
                {
                    GameObject obj = (GameObject)Instantiate(item.gameObject);
                    obj.SetActive(false);
                    pool.Add(obj);
                }
            }
        }

        public GameObject GetByTag(string tag)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeInHierarchy && pool[i].tag == tag)
                {
                    return pool[i];
                }
            }
            foreach (ObjectPoolItem item in items)
            {
                if (item.gameObject.tag == tag)
                {
                    if (item.expandable)
                    {
                        GameObject obj = (GameObject)Instantiate(item.gameObject);
                        obj.SetActive(false);
                        pool.Add(obj);
                        return obj;
                    }
                }
            }
            return null;
        }

        public GameObject GetByNamePrefix(string prefix)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeInHierarchy && pool[i].name.StartsWith(prefix))
                {
                    return pool[i];
                }
            }
            foreach (ObjectPoolItem item in items)
            {
                if (item.gameObject.name.StartsWith(prefix))
                {
                    if (item.expandable) // TODO: 이게 앞에 있는게 더 최적화가 좋을듯함.
                    {
                        GameObject obj = (GameObject)Instantiate(item.gameObject);
                        obj.SetActive(false);
                        pool.Add(obj);
                        return obj;
                    }
                }
            }
            return null;
        }
    }
}