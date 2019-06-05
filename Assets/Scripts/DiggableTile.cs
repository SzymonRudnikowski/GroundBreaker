using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// Base class for all diggable tiles
    /// </summary>
    [CreateAssetMenu(fileName = "New Diggable Tile", menuName = "Tiles/Diggable Tile")]
    public class DiggableTile : TileBase, IInventoryItem, ITradeable
    {
        #region ITradable Implementation

        public bool Tradeable { get; set; }

        private int m_sellPrice = 1;
        public int SellPrice
        {
            get { return m_sellPrice; }
            set
            {
                m_sellPrice = Math.Max(1, value);
            }
        }

        private int m_customBuyPrice = -1;
        public int BuyPrice
        {
            get
            {
                if (m_customBuyPrice > 0) return m_customBuyPrice;
                return Mathf.CeilToInt(SellPrice * 1.5f);
            }
            set
            {
                m_customBuyPrice = value;
            }
        }

        #endregion



        #region IInventoryItem Implementation

        private Sprite m_inventorySprite = null;
        public Sprite InventoryIcon
        {
            get
            {
                return m_inventorySprite != null ? m_inventorySprite : GetSprite(Vector3Int.zero);
            }
            set
            {
                m_inventorySprite = value;
            }
        }

        private bool m_stackable = true;
        public bool Stackable
        {
            get { return m_stackable; }
            set { m_stackable = value; }
        }

        private int m_stackSize = 32;
        public int StackSize
        {
            get { return m_stackable ? m_stackSize : 1; }
            set { m_stackSize = Math.Max(1, value); }
        }

        #endregion



        #region Tile Position Specific Information

        /// <summary>
        /// Name of the current diggable type
        /// </summary>
        new public string name = "Tile";

        /// <summary>
        /// How many hits tile can take before being destroyed
        /// </summary>
        public int health = 1;

        #endregion




        #region Rendering Information

        /// <summary>
        /// If set, effect spawned when player digs this tile type
        /// </summary>
        public GameObject hitEffect;

        /// <summary>
        /// List of images to consider when calculating image to display
        /// </summary>
        public Sprite[] sprites;

        /// <summary>
        /// Returns pseudo-random image basing on position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Sprite GetSprite(Vector3Int position)
        {
            if ((sprites != null) && (sprites.Length > 0))
            {
                long hash = position.x;
                hash = (hash + 0xabcd1234) + (hash << 15);
                hash = (hash + 0x0987efab) ^ (hash >> 11);
                hash ^= position.y;
                hash = (hash + 0x46ac12fd) + (hash << 7);
                hash = (hash + 0xbe9730af) ^ (hash << 11);
                Random.InitState((int)hash);
                return sprites[(int)(sprites.Length * Random.value)];
            }
            return null;
        }

        #endregion

        #region TileBase Overrides

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            // Call base method first
            base.StartUp(position, tilemap, go);

            // Get per-position storage
            var tilemapInfo = tilemap.GetComponent<TilemapInformation>();

            // No TilemapInformation = fail
            if (tilemapInfo == null)
            {
                return false;
            }

            // Update postion property
            return tilemapInfo.Set(position, "health", health);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = GetSprite(position);
            tileData.colliderType = Tile.ColliderType.Grid;
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            base.RefreshTile(position, tilemap);
        }

        #endregion

    }

    /// <summary>
    /// Editor for DiggableTile class
    /// </summary>
#if UNITY_EDITOR
    [CustomEditor(typeof(DiggableTile))]
    public class DiggableTileEditor : Editor
    {
        /// <summary>
        /// Returns target tile as Diggable Tile
        /// </summary>
        private DiggableTile Tile
        {
            get
            {
                return (target as DiggableTile);
            }
        }

        #region Editor Overrides 

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Tradeable, Sell, Buy Prices
            Tile.Tradeable = EditorGUILayout.Toggle("Tradeable?", Tile.Tradeable);
            if (Tile.Tradeable)
            {
                Tile.SellPrice = EditorGUILayout.IntField("Sell Price", Tile.SellPrice);
                Tile.BuyPrice = EditorGUILayout.IntField("Buy Price", Tile.BuyPrice);
            }

            EditorGUILayout.Separator();

            // Inventory Icon, Stack 
            Tile.InventoryIcon = (Sprite)EditorGUILayout.ObjectField("Inventory Icon", Tile.InventoryIcon, typeof(Sprite), false);
            Tile.Stackable = EditorGUILayout.Toggle("Stackable?", Tile.Stackable);
            if(Tile.Stackable)
            {
                Tile.StackSize = EditorGUILayout.IntField("Stack Size", Tile.StackSize);
            }

            EditorGUILayout.Separator();


            // Name
            Tile.name = EditorGUILayout.TextField("Name", Tile.name);

            // Health
            Tile.health = EditorGUILayout.IntField("Hitpoints", Tile.health);

            // Hit effect
            Tile.hitEffect = (GameObject)EditorGUILayout.ObjectField("Dig effect", Tile.hitEffect, typeof(GameObject), false);

            EditorGUILayout.Space();

            // Images
            int numOfSprites = Mathf.Max(0, EditorGUILayout.DelayedIntField("Number of Sprites", Tile.sprites != null ? Tile.sprites.Length : 0));
            if (Tile.sprites == null || Tile.sprites.Length != numOfSprites)
            {
                Array.Resize<Sprite>(ref Tile.sprites, numOfSprites);
            }

            int cols = 3;
            int rows = Mathf.CeilToInt(1.0f * numOfSprites / cols);

            EditorGUILayout.Space();

            for (int row = 0; row < rows; row++)
            {
                GUILayout.BeginHorizontal();

                for (int col = 0; col < cols; col++)
                {
                    int idx = row * cols + col;
                    if (idx >= numOfSprites) break;
                    Tile.sprites[idx] = (Sprite)EditorGUILayout.ObjectField(Tile.sprites[idx], typeof(Sprite), false);
                }

                GUILayout.EndHorizontal();
            }


            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(Tile);
        }

        #endregion
    }

#endif
}
