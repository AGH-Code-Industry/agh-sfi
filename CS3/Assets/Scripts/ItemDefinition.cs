using DeadFusion.Utility;
using Unity.Collections;
using UnityEngine;

namespace DeadFusion.GameCore.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "ItemDefinitions/Item", order = 0)]
    public class ItemDefinition : ScriptableObject
    {
        public string key;
        public new string name;
        [TextArea()]
        public string description;
        public Sprite icon;

        public virtual Item GetItem()
        {
            return new Item(key, name, description, icon);
        }
    }

    public class Item
    {
        public readonly string key;

        public readonly string name;

        public readonly Sprite icon;

        internal readonly string description;

        public virtual string Description
        {
            get { return description; }
        }

        public virtual byte[] SerializeData() { return new byte[0]; }

        public virtual void DeserializeData(byte[] data) { }

        public Item(string key, string name, string description, Sprite icon)
        {
            this.key = key;
            this.name = name;
            this.description = description;
            this.icon = icon;
        }
    }
}