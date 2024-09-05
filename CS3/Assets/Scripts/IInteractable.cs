using DeadFusion.GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string GetDescription(GameObject player);

    void OnInteract(GameObject player);
}
