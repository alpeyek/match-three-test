using System;
using System.Collections.Generic;
using UnityEngine;

namespace MathThree
{
    public class GameObjectPool : MonoBehaviour
    {
        public enum PoolLayerType
        {
            None = 0,
            Prefab = 1,
            Root = 2
        }

        public Transform Root => _root;
        public GameObject Prefab => _prefab;

        [Header("Object Links")]
        [SerializeField]
        private Transform _root;
        [SerializeField]
        private GameObject _prefab;

        [Header("Pool Config")]
        [SerializeField]
        private int _cacheCount;
        [SerializeField]
        protected PoolLayerType _layer = PoolLayerType.Root;
        [SerializeField]
        private bool _prefabScale = false;
        [SerializeField]
        private bool _prefabRotate = false;

        [Header("Precache items")]
        [SerializeField]
        protected List<GameObject> _items = new List<GameObject>();
        protected bool _initialized;

        public virtual void ReleaseAll()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i]?.SetActive(false);
                if (_root != null)
                    _items[i].transform.SetParent(_root);
            }
        }

        public virtual GameObject Get()
        {
            if (!_initialized) Start();

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] == null)
                {
                    _items[i] = Create();
                    return _items[i];
                }
                if (!_items[i].activeSelf)
                {
                    _items[i].SetActive(true);
                    return _items[i];
                }
            }

            var item = Create();
            _items.Add(item);
            return item;
        }

        public virtual GameObject Get(Transform parent)
        {
            var item = Get();
            item.transform.SetParent(parent);
            return item;
        }

        public T Get<T>()
        {
            return Get().GetComponent<T>();
        }

        public T Get<T>(Transform parent)
        {
            return Get(parent).GetComponent<T>();
        }

        public virtual void Start()
        {
            if (_initialized) return;
            _initialized = true;

            var count = Math.Max(_items.Count, _cacheCount);
            for (int i = 0; i < count; i++)
            {
                if (i < _items.Count)
                {
                    if (_items[i] == null) _items[i] = Create();
                }
                else
                {
                    _items.Add(Create());
                }
            }

            ReleaseAll();
        }

        public virtual GameObject Create()
        {
            GameObject go = Instantiate(_prefab) as GameObject;

            if (go != null)
            {
                Transform t = go.transform;
                if (_root != null)
                {
                    t.SetParent(_root);
                    if (_layer == PoolLayerType.Root)
                        go.layer = _root.gameObject.layer;
                }

                if (_layer == PoolLayerType.Prefab)
                    go.layer = _prefab.layer;

                t.localPosition = Vector3.zero;
                t.localRotation = _prefabRotate ? _prefab.transform.localRotation : Quaternion.identity;
                t.localScale = _prefabScale ? _prefab.transform.localScale : Vector3.one;
                go.SetActive(true);
            }
            return go;
        }
    }
}
