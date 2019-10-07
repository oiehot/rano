using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rano.Object
{
    // ObjectPoolItem 클래스는 Inspector에서 수정이 가능하다.
    [System.Serializable]
    public class ObjectPoolItem
    {
        public GameObject gameObject;
        public int amount;
        public bool expandable;
    }

    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler instance;
        public List<ObjectPoolItem> items;
        public List<GameObject> pool;

        private void Awake()
        {
            instance = this;
        }

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

        public GameObject Get(string tag)
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
    }
}