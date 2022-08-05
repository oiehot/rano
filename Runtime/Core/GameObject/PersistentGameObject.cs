using UnityEngine;

namespace Rano
{
    public class PersistentGameObject : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }   
    }
}