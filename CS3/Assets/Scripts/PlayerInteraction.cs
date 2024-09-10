using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeadFusion.GameCore
{
    public class PlayerInteraction : MonoBehaviour
    {
        public static PlayerInteraction Instance;

        public string Description { get; private set; }
        public bool blockInteraction;

        [SerializeField] Transform raycastOrigin;
        [SerializeField] LayerMask interactionLayer;
        [SerializeField] float interactionDistance;

        GameObject cachedObject;
        IInteractable cachedInteractable;

        Keyboard kb;

        private void Awake()
        {
            kb = InputSystem.GetDevice<Keyboard>();
            Instance = this;
        }

        void Update()
        {
            if (blockInteraction)
            {
                Description = string.Empty;
                return;
            }

            UpdateInteractable();

            if (cachedInteractable == null)
            {
                Description = string.Empty;
                return;
            }

            Description = cachedInteractable.GetDescription(gameObject);

            if (kb.eKey.wasPressedThisFrame)
                Interact();
        }

        void UpdateInteractable()
        {
            Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
            {
                if (cachedObject == hit.transform.gameObject)
                    return;

                cachedObject = hit.transform.gameObject;
                cachedInteractable = cachedObject.GetComponentInParent<IInteractable>();
            }
            else
            {
                cachedObject = null;
                cachedInteractable = null;
            }
        }

        void Interact()
        {
            cachedInteractable.OnInteract(gameObject);
        }
    }
}