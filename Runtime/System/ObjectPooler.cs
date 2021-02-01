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
        private int count = 0;
        public List<ObjectPoolItem> items;
        public List<GameObject> pool;
        
        public HideFlags defaultHideFlags = HideFlags.HideInHierarchy;

        void Awake()
        {
            this.pool = new List<GameObject>();
        }

        public void Init()
        {
            foreach (ObjectPoolItem item in items)
            {
                for (int i = 0; i < item.amount; i++)
                {
                    Create(item.gameObject);
                }
            }
        }

        private GameObject Create(GameObject src)
        {
            GameObject obj = (GameObject)Instantiate(src);
            obj.name = $"{src.name}_{count}";
            obj.hideFlags = defaultHideFlags;
            obj.SetActive(false);
            pool.Add(obj);
            count++;
            return obj;
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
                        return Create(item.gameObject);
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
                        return Create(item.gameObject);
                    }
                }
            }
            return null;
        }
    }
}