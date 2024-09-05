using UnityEngine;

namespace DeadFusion.GameCore
{
    public class ViewmodelHolder : MonoBehaviour
    {
        [field: SerializeField] public Animator handAnimator { get; private set; }
        [SerializeField] Transform pivot;
        [SerializeField] ViewmodelFov viewmodelFov;

        Viewmodel currentViewmodel;

        [SerializeField] GameObject defaultViewmodel;

        private void Start()
        {
            handAnimator.fireEvents = false;
        }

        public GameObject UseViewmodel(GameObject viewmodelPrefab)
        {
            DestroyViewmodel();
            Viewmodel newViewmodel = Instantiate(viewmodelPrefab, transform).GetComponent<Viewmodel>();
            if (newViewmodel == null)
            {
                Debug.LogError("Missing Viewmodel component in Viewmodel Prefab.");
                return null;
            }

            handAnimator.avatar = newViewmodel.animator.avatar;
            handAnimator.runtimeAnimatorController = newViewmodel.animator.runtimeAnimatorController;

            pivot.localPosition = newViewmodel.pivotPosition;
            pivot.localRotation = Quaternion.identity;
            handAnimator.transform.localRotation = Quaternion.identity;

            handAnimator.transform.parent = pivot.transform;
            newViewmodel.transform.parent = pivot.transform;

            handAnimator.gameObject.SetActive(true);
            viewmodelFov.SetFOV(newViewmodel.fov);
            viewmodelFov.UpdateRenderers();
            currentViewmodel = newViewmodel;

            return newViewmodel.gameObject;
        }

        [ContextMenu("Destroy Viewmodel")]
        public void DestroyViewmodel()
        {
            if (currentViewmodel == null)
                return;
            Destroy(currentViewmodel.gameObject);
            handAnimator.avatar = null;
            handAnimator.runtimeAnimatorController = null;
            handAnimator.transform.parent = transform;
            handAnimator.transform.localPosition = Vector3.zero;
            handAnimator.gameObject.SetActive(true);

            viewmodelFov.UpdateRenderers();
        }

        [ContextMenu("Test Viewmodel")]
        void TestViewmodel()
        {
            UseViewmodel(defaultViewmodel);
        }
    }
}