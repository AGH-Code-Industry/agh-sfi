using UnityEngine;

namespace DeadFusion.GameCore
{
    public class ViewmodelFov : MonoBehaviour
    {
        [SerializeField] float fieldOfView;
        [SerializeField] Renderer[] renderers;

        void Update()
        {
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                foreach (Material mat in renderer.materials)
                    mat.SetFloat("_FpsModeFov", fieldOfView);
            }
        }

        public void UpdateRenderers()
        {
            renderers = new Renderer[0];
            renderers = GetComponentsInChildren<Renderer>(true);
        }

        public void SetFOV(float fov)
        {
            fieldOfView = fov;
        }
    }
}