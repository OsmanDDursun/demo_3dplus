using UnityEngine;

namespace App.Scripts.Helpers
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<T>(true);
                    if (!_instance)
                    {
                        Debug.LogError($"Instance of {nameof(T)} not found in scene");
                    }
                    else
                    {
                        var instance = _instance as SingletonBehaviour<T>;
                        instance.OnAwake();
                    }
                }
                
                return _instance;
            }
            
            private set => _instance = value;
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<T>(true);
                if (!Instance)
                {
                    Debug.LogError($"Instance of {nameof(T)} not found in scene");
                }
                else
                {
                    OnAwake();
                }
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected abstract void OnAwake();
    }
}