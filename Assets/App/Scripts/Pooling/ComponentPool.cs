using System.Collections.Generic;
using UnityEngine;

namespace App.Scripts.Pooling
{
    public class ComponentPool<T> where T : Component
    {
        public readonly List<T> ActiveObjects = new List<T>();
        public readonly List<T> InactiveObjects = new List<T>();

        private readonly Transform _parent;
        private readonly T _objectToCreate;
        
        public ComponentPool(T objectPrefab, int poolSize = 10, Transform parent = null)
        {
            _parent = parent;
            _objectToCreate = objectPrefab;
            CreatePool(poolSize);
        }

        private void CreatePool(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                CreateObject();
            }
        }

        public T Get()
        {
            if (InactiveObjects.Count == 0)
            {
                CreatePool(1);
                return Get();
            }
            
            var obj = InactiveObjects[0];
            InactiveObjects.Remove(obj);
            ActiveObjects.Add(obj);
            obj.gameObject.SetActive(true);

            return obj;
        }

        public void Release(T obj)
        {
            if(!InactiveObjects.Contains(obj))
                InactiveObjects.Add(obj);
            ActiveObjects.Remove(obj);
            
            obj.gameObject.SetActive(false);
        }
        
        public void ReleaseAll()
        {
            foreach (var activeObject in ActiveObjects)
            {
                if(!InactiveObjects.Contains(activeObject))
                    InactiveObjects.Add(activeObject);
                activeObject.gameObject.SetActive(false);
            }
            
            ActiveObjects.Clear();
        }

        private T CreateObject()
        {
            var newObj = Object.Instantiate(_objectToCreate, _parent);
            InactiveObjects.Add(newObj);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }
}
