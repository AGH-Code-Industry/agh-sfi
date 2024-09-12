using DeadFusion.GameCore.Items;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    SelectableItem currentItem;

    [SerializeField] SelectableItemDefinition _item1;
    [SerializeField] SelectableItemDefinition _item2;

    SelectableItem item1;
    SelectableItem item2;

    Keyboard kb;

    private void Awake()
    {
        kb = InputSystem.GetDevice<Keyboard>();
        item1 = _item1.GetItem() as SelectableItem;
        item2 = _item2.GetItem() as SelectableItem;
    }

    void Update()
    {
        if (kb.digit1Key.wasPressedThisFrame)
        {
            SelectItem(item1);
        }
        else if (kb.digit2Key.wasPressedThisFrame)
        {
            SelectItem(item2);
        }
    }

    void SelectItem(SelectableItem item)
    {
        if (item == currentItem)
            return;

        TryDeselect();
        currentItem = item;
        item.OnSelect(gameObject);
    }

    void TryDeselect()
    {
        if (currentItem != null)
            currentItem.OnDeselect(gameObject);
    }
}
