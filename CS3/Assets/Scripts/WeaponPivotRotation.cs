using UnityEngine;
using UnityEngine.InputSystem;

namespace DeadFusion.GameCore
{
    public class WeaponPivotRotation : MonoBehaviour
    {
        [System.Serializable]
        public struct PivotRotationSettings
        {
            public float regressionSpeed;
            public float maxDelta;
            public float deltaScale;
            public float adsMultiplier;
        }

        public bool ads = false;
        [SerializeField] PivotRotationSettings settings;
        Mouse ms;

        private void Awake()
        {
            ms = InputSystem.GetDevice<Mouse>();
        }

        private void Update()
        {
            Vector2 mouseRot = Vector2.ClampMagnitude(ms.delta.value, settings.maxDelta * Time.deltaTime) * settings.deltaScale * (ads ? settings.adsMultiplier : 1f);
            transform.Rotate(-mouseRot.y, mouseRot.x, 0f, Space.Self);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * settings.regressionSpeed);
        }

        public void SetRotationSettings(PivotRotationSettings settings)
        {
            this.settings = settings;
        }
    }
}