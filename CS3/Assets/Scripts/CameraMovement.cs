using DeadFusion.GameCore.Weapons;
using DeadFusion.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeadFusion.GameCore
{
    public class CameraMovement : MonoBehaviour
    {
        public class Recoil
        {
            RecoilSettings settings;

            float recoilX = 0f;
            float recoilY = 0f;

            public void ChangeSettings(RecoilSettings settings)
            {
                this.settings = settings;
            }

            public void AddRecoil(bool ads)
            {
                float sign = (Random.value > 0.5f ? -1f : 1f);
                recoilX += settings.verticalRecoil * (ads ? settings.adsMultiplier : 1f);
                recoilY += sign * Random.Range(settings.horizontalRecoilMin, settings.horizontalRecoilMax) * (ads ? settings.adsMultiplier : 1f);
            }

            public Vector2 UseRecoil(float deltaTime)
            {
                float usedRecoilX = Mathf.Lerp(0f, recoilX, deltaTime * settings.interpolation);
                float usedRecoilY = Mathf.Lerp(0f, recoilY, deltaTime * settings.interpolation);

                recoilX -= usedRecoilX;
                recoilY -= usedRecoilY;

                return new Vector2(-usedRecoilX, usedRecoilY);
            }
        }

        [SerializeField] InputActionAsset inputActions;
        [SerializeField] Transform playerTransform;
        InputAction mouseAction;
        Keyboard keyboard;

        private Vector2 mouseDelta;
        public float sensitivity;
        float xRotation = 0f;
        float yRotation = 0f;

        [Header("Freezing")]
        public bool freezeX;
        public bool freezeY;
        public BoolOverrideList freeze;

        [Space()]
        public Vector2 freezePosition;
        public Vector2 Rotation
        {
            get
            {
                return new Vector2(xRotation, yRotation);
            }
        }

        [SerializeField] float xOffset = 0f;
        [SerializeField] float xMin = -90f;
        [SerializeField] float xMax = 90f;

        [Space()]
        public Recoil recoil = new Recoil();

        void Awake()
        {
            SetRotation(0f, playerTransform.eulerAngles.y);
            mouseAction = inputActions.FindAction("Mouse");
            mouseAction.Enable();
            keyboard = InputSystem.GetDevice<Keyboard>();
        }

        void Update()
        {
            AddRotation(recoil.UseRecoil(Time.deltaTime));


            if (keyboard.commaKey.wasPressedThisFrame) { sensitivity -= 0.1f; }
            if (keyboard.periodKey.wasPressedThisFrame) { sensitivity += 0.1f; }
            RotateCam();
        }

        void RotateCam()
        {
            mouseDelta = mouseAction.ReadValue<Vector2>();

            float mouseX = mouseDelta.x * sensitivity * Time.timeScale;
            float mouseY = mouseDelta.y * sensitivity * Time.timeScale;

            xRotation -= mouseY;
            xRotation = (freezeX || freeze.isAnyActive) ? freezePosition.x : Mathf.Clamp(xRotation, xMin, xMax);

            transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation + xOffset, xMin, xMax), 0f, 0f);
            yRotation += mouseX;
            yRotation = (freezeY || freeze.isAnyActive) ? freezePosition.y : yRotation;

            playerTransform.localRotation = Quaternion.Euler(new Vector3(0, yRotation, 0));
        }

        public void SetRotation(float xRot, float yRot)
        {
            xRotation = xRot;
            yRotation = yRot;

            transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation, -90f, 90f), 0f, 0f);
            playerTransform.rotation = Quaternion.Euler(new Vector3(0, yRotation, 0));
        }

        public void SetCurrentRotationAsFreeze()
        {
            freezePosition = new Vector2(xRotation, yRotation);
        }

        public void AddRotation(Vector2 rotation)
        {
            xRotation += rotation.x;
            yRotation += rotation.y;

            transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation, -90f, 90f), 0f, 0f);
            playerTransform.rotation = Quaternion.Euler(new Vector3(0, yRotation, 0));
        }
    }
}