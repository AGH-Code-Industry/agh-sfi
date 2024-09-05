using UnityEngine;

namespace DeadFusion.GameCore.Items
{
    public abstract class SelectableItemDefinition : ItemDefinition
    {

    }

    public abstract class SelectableItem : Item
    {

        public SelectableItem(string key, string name, string description, Sprite icon) : base(key, name, description, icon) { }

        public abstract void OnSelect(GameObject player);

        public abstract void OnDeselect(GameObject player);

        public abstract bool CanDeselect(GameObject player);
    }
}