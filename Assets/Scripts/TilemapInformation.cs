using System;
using System.Collections.Generic;

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// Describes types allowed to be stored in TilemapInformation
    /// </summary>
    internal enum TilemapInformationType
    {
        Integer,
        UnityObject
    }

    /// <summary>
    /// Component providing means for tile/position specific information storage
    /// Should be added on Tilemap object
    /// </summary>
    [RequireComponent(typeof(Tilemap))]
    public class TilemapInformation : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Stores position and property name
        /// </summary>
        [Serializable]
        internal struct Key
        {
            public Vector3Int position;
            public string name;
        }

        /// <summary>
        /// Stores type of data store and data itself,
        /// allowed types are listed by <see cref="TilemapInformationType"/>
        /// </summary>
        internal struct Value
        {
            public TilemapInformationType type;
            public object data;
        }

        private Dictionary<Key, Value> m_storage = new Dictionary<Key, Value>();

        #region Set

        private bool Set(Vector3Int position, string name, TilemapInformationType type, object data)
        {
            if (data == null)
            {
                Debug.LogError("Data is null!");
                return false;
            }

            Key key;
            key.name = name;
            key.position = position;

            Value value;
            value.type = type;
            value.data = data;

            m_storage[key] = value;

            return true;
        }

        public bool Set(Vector3Int position, string name, int data)
        {
            return Set(position, name, TilemapInformationType.Integer, data);
        }

        public bool Set(Vector3Int position, string name, UnityEngine.Object data)
        {
            return Set(position, name, TilemapInformationType.UnityObject, data);
        }

        #endregion

        #region Get

        private object Get(Vector3Int position, string name, TilemapInformationType type, object defaultValue)
        {
            Key key;
            key.name = name;
            key.position = position;

            try
            {
                var value = m_storage[key];
                if (value.type != type)
                {
                    Debug.LogWarning("No such property exists!");
                    return defaultValue;
                }
                return value.data;
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
        }

        public int Get(Vector3Int position, string name, int defaultValue = 0)
        {
            return (int)Get(position, name, TilemapInformationType.Integer, defaultValue);
        }

        public UnityEngine.Object Get(Vector3Int position, string name, UnityEngine.Object defaultValue = null)
        {
            return (UnityEngine.Object)Get(position, name, TilemapInformationType.UnityObject, defaultValue);
        }

        #endregion

        public bool Remove(Vector3Int position, string name)
        {
            Key key;
            key.name = name;
            key.position = position;

            return m_storage.Remove(key);
        }

        #region ISerializationCallbackReceiver Implementation 

        [SerializeField]
        [HideInInspector]
        private List<Key> m_intKeys = new List<Key>();

        [SerializeField]
        [HideInInspector]
        private List<Key> m_objectKeys = new List<Key>();

        [SerializeField]
        private List<int> m_intValues = new List<int>();

        [SerializeField]
        private List<UnityEngine.Object> m_objectValues = new List<UnityEngine.Object>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_intKeys.Clear();
            m_objectKeys.Clear();
            m_intValues.Clear();
            m_objectValues.Clear();

            foreach (var kvp in m_storage)
            {
                var value = kvp.Value;
                var key = kvp.Key;

                switch (value.type)
                {
                    case TilemapInformationType.Integer:
                        m_intKeys.Add(key);
                        m_intValues.Add((int)value.data);
                        break;
                    case TilemapInformationType.UnityObject:
                        m_objectKeys.Add(key);
                        m_objectValues.Add((UnityEngine.Object)value.data);
                        break;
                }

            }

        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_storage.Clear();

            for (int i = 0; i != Math.Min(m_intKeys.Count, m_intValues.Count); i++)
            {
                Key key = m_intKeys[i];

                Value value;
                value.type = TilemapInformationType.Integer;
                value.data = m_intValues[i];

                m_storage[key] = value;

            }
            for (int i = 0; i != Math.Min(m_objectKeys.Count, m_objectValues.Count); i++)
            {
                Key key = m_objectKeys[i];

                Value value;
                value.type = TilemapInformationType.UnityObject;
                value.data = m_objectValues[i];

                m_storage[key] = value;
            }
        }

        #endregion

    }
}

