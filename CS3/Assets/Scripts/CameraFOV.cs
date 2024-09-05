using System.Collections.Generic;
using UnityEngine;

namespace DeadFusion.GameCore
{
    public class CameraFOV : MonoBehaviour
    {
        [System.Serializable]
        public class FOV_Override
        {
            public string label;
            public int priority;
            public float fov;

            public FOV_Override(string label, int priority, float fov)
            {
                this.label = label;
                this.priority = priority;
                this.fov = fov;
            }
        }

        [SerializeField] float defaultFov;
        [SerializeField] float interpolationSpeed;
        [SerializeField]
        List<FOV_Override> overrides = new List<FOV_Override>();
        Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, overrides.Count > 0 ? overrides[0].fov : defaultFov, Time.deltaTime * interpolationSpeed);
        }

        public FOV_Override AddOverride(float fov, int priority, string label)
        {
            FOV_Override newOverride = new FOV_Override(label, priority, fov);
            int i = 0;
            for (; i < overrides.Count; i++)
            {
                if (overrides[i].priority > newOverride.priority)
                    break;
            }
            overrides.Insert(i, newOverride);
            return newOverride;
        }

        public void RemoveOverride(FOV_Override fovOverride)
        {
            overrides.Remove(fovOverride);
        }
    }
}