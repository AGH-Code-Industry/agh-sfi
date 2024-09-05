using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DeadFusion.GameCore.Items;
using Unity.Collections;

namespace DeadFusion.GameCore.Weapons
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] LayerMask shootingMask;
        [SerializeField] ViewmodelHolder viewmodelHolder;
        [SerializeField] CameraMovement cameraMovement;
        [SerializeField] CharacterMovement characterMovement;
        [SerializeField] WeaponPivotRotation pivotRotation;
        [SerializeField] CameraFOV cameraFov;
        [SerializeField] AudioSource ammoRestoreSound;
        [SerializeField] float reloadCrossfade = 0.5f;
        CameraFOV.FOV_Override cameraFovOverride;
        Keyboard kb;
        Mouse mouse;

        public WeaponItem currentWeapon { get; private set; }
        WeaponViewmodel viewmodel;
        List<Animator> animators = new List<Animator>();

        float fireTimer;
        bool reloading = false;

        [Header("Debug")]
        [SerializeField] bool _debugHitTracers;
        [SerializeField] GameObject _debugTracerPrefab;

        public System.Action<HitType> OnHit;

        void Awake()
        {
            kb = InputSystem.GetDevice<Keyboard>();
            mouse = InputSystem.GetDevice<Mouse>();
        }

        private void Start()
        {
            animators.Add(viewmodelHolder.handAnimator);
        }

        void Update()
        {
            if (currentWeapon == null)
                return;

            fireTimer -= Time.deltaTime;
            if ((!mouse.leftButton.isPressed && fireTimer < 0f) || !IsAnimationShootReady())
                fireTimer = 0f;

            bool wantToAds = mouse.rightButton.isPressed;
            foreach (Animator animator in animators)
            {
                animator.SetBool("Ads", wantToAds);
                if (currentWeapon.weaponConfig.loadMode == LoadMode.Clip)
                    animator.SetBool("EndReload", currentWeapon.ammoReserve == 0 || currentWeapon.loadedAmmo >= currentWeapon.weaponConfig.magazineSize || wantToAds);
            }
            UpdateFovOverride();


            bool animationShootRead = IsAnimationShootReady();
            characterMovement.blockSprint.SetBool("Weapon System", IsAiming());
            pivotRotation.ads = IsAiming();

            if (kb.rKey.wasPressedThisFrame && animationShootRead && CanReload())
            {
                StartReload();
            }

            if (CanShoot())
            {
                ShootWeapon();
            }
        }

        public void EquipWeapon(WeaponItem weapon)
        {
            if (currentWeapon != null)
                UnityEngine.Debug.LogWarning("Equipping weapon when another is still equipped!");
            currentWeapon = weapon;
            cameraMovement.recoil.ChangeSettings(weapon.weaponConfig.recoilSettings);
            viewmodel = viewmodelHolder.UseViewmodel(weapon.weaponConfig.viewModel).GetComponent<WeaponViewmodel>();
            viewmodel.weaponSystem = this;
            animators.Add(viewmodel.animator);
            pivotRotation.SetRotationSettings(weapon.weaponConfig.pivotRotationSettings);
            fireTimer = 0f;
            reloading = false;

            foreach (Animator a in animators)
                a.Play("Draw", -1, 0f);
        }

        public void DeEquipWeapon()
        {
            animators.Remove(viewmodel.animator);
            viewmodelHolder.DestroyViewmodel();
            currentWeapon = null;
            viewmodel = null;
        }

        public bool CanDeselect()
        {
            return IsAnimationShootReady() && !IsAiming();
        }

        public void RestoreAmmoRpc()
        {
            if (currentWeapon == null)
                return;

            if (currentWeapon.ammoReserve != currentWeapon.weaponConfig.ammoReserveCapacity)
            {
                currentWeapon.ammoReserve = currentWeapon.weaponConfig.ammoReserveCapacity;
                ammoRestoreSound?.Play();
            }
        }

        public void LoadAmmo()
        {
            int diff = currentWeapon.weaponConfig.magazineSize - currentWeapon.loadedAmmo;
            reloading = false;
            switch (currentWeapon.weaponConfig.loadMode)
            {
                case LoadMode.Clip:
                    int clipLoading = Mathf.Min(diff, currentWeapon.ammoReserve, currentWeapon.weaponConfig.clipLoadAmount);
                    currentWeapon.loadedAmmo += clipLoading;
                    currentWeapon.ammoReserve -= clipLoading;
                    break;
                case LoadMode.Magazine:
                    int loading = Mathf.Min(diff, currentWeapon.ammoReserve);
                    currentWeapon.loadedAmmo += loading;
                    currentWeapon.ammoReserve -= loading;
                    break;
                default:
                    break;
            }
        }

        bool IsAnimationShootReady()
        {
            return animators[0].GetCurrentAnimatorStateInfo(0).IsTag("canShoot")
                || animators[0].GetCurrentAnimatorStateInfo(0).IsTag("canShootAds");
        }

        bool IsAiming()
        {
            return animators[0].GetCurrentAnimatorStateInfo(0).IsTag("canShootAds") 
                || animators[0].GetCurrentAnimatorStateInfo(0).IsTag("ads");
        }

        bool CanReload()
        {
            return currentWeapon.loadedAmmo < currentWeapon.weaponConfig.magazineSize
                && currentWeapon.ammoReserve > 0
                && !reloading;
        }

        void UpdateFovOverride()
        {
            if (cameraFovOverride != null && !IsAiming())
            {
                cameraFov.RemoveOverride(cameraFovOverride);
                cameraFovOverride = null;
            }
            else if (cameraFovOverride == null && IsAiming())
            {
                cameraFovOverride = cameraFov.AddOverride(currentWeapon.weaponConfig.adsFov, 1, "WeaponSystem");
            }
        }

        void StartReload()
        {
            foreach (Animator anim in animators)
                anim.CrossFade("Reload", reloadCrossfade);
            reloading = true;
        }

        bool CanShoot()
        {
            if (currentWeapon.loadedAmmo <= 0)
                return false;
            if (currentWeapon.weaponConfig.fireMode == FireMode.Automatic)
                return mouse.leftButton.isPressed && fireTimer <= 0 && IsAnimationShootReady();
            else if (currentWeapon.weaponConfig.fireMode == FireMode.Single)
                return mouse.leftButton.wasPressedThisFrame && fireTimer <= 0 && IsAnimationShootReady();
            else
                return false;
        }

        void ShootWeapon()
        {
            List<HitData> hitData = new List<HitData>();
            Transform origin = cameraFov.transform;
            for (int i = 0; i < currentWeapon.weaponConfig.bulletCount; i++)
            {
                Ray ray = new Ray(origin.position, origin.forward);
                Vector3 rotationDirection = Quaternion.AngleAxis(Random.Range(0f, 360f), ray.direction) * Vector3.Cross(ray.direction, Vector3.up);
                float spreadAngle = Random.Range(0f, IsAiming() ? currentWeapon.weaponConfig.spreadSettings.adsSpreadAngle : currentWeapon.weaponConfig.spreadSettings.spreadAngle);
                ray.direction = Quaternion.AngleAxis(spreadAngle, rotationDirection) * ray.direction;
                if (Physics.Raycast(ray, out RaycastHit hit, 500f, shootingMask))
                {
                    HitData data = new HitData() 
                    { 
                        hitObject = null, 
                        colliderId = -1, 
                        point = hit.point, 
                        normal = hit.normal,
                        direction = ray.direction.normalized
                    };
                    float damageLerp = (hit.distance - currentWeapon.weaponConfig.damageFalloffStart) / (currentWeapon.weaponConfig.damageFalloffEnd - currentWeapon.weaponConfig.damageFalloffStart);
                    data.damage = Mathf.Lerp(currentWeapon.weaponConfig.damage, 0f, damageLerp);

                    GameObject hitObject = hit.transform.gameObject;

                    while (hitObject.transform.parent != null)
                        hitObject = hitObject.transform.parent.gameObject;


                    if (hitObject != null)
                    {
                        data.hitObject = hitObject;
                        if(hitObject.TryGetComponent(out DamagableColliders colliders)) 
                        {
                            if (colliders.colliders.Contains(hit.transform.gameObject))
                            {
                                data.colliderId = colliders.colliders.IndexOf(hit.transform.gameObject);
                            }
                        }
                    }

                    hitData.Add(data);

                    if (_debugHitTracers)
                        Instantiate(_debugTracerPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
            currentWeapon.loadedAmmo -= 1;
            fireTimer += 60 / currentWeapon.weaponConfig.fireRate;

            ShootServerRpc(hitData.ToArray(), currentWeapon.key);
            PlayShootEffects();
        }

        void ShootServerRpc(HitData[] hitData, FixedString32Bytes weaponKey)
        {
            HitType hitType = HitType.None;
            foreach (HitData hit in hitData)
            {
                if (true)
                {
                    DamageInfo info = new DamageInfo()
                    {
                        amount = hit.damage,
                        colliderIndex = hit.colliderId,
                        normal = hit.normal, type = DamageInfo.DamageType.Bullet,
                        direction = hit.direction,
                        weapon = weaponKey
                    };
                    IDamagable damagable = hit.hitObject.GetComponent<IDamagable>();
                    if(damagable != null)
                    {
                        HitType onDamage = damagable.OnDamage(info);
                        if (onDamage != HitType.None && hitType != HitType.Critical)
                            hitType = onDamage;
                    }
                }
            }
            if (hitType != HitType.None)
                OnHitRpc(hitType);
        }

        void OnHitRpc(HitType hitType)
        {
            OnHit?.Invoke(hitType);
        }

        void PlayShootEffects()
        {
            foreach (Animator anim in animators)
            {
                anim.Play(IsAiming() ? "ShootAds" : "Shoot", -1, 0f);
            }
            //if(currentWeapon.weaponConfig.shootAudioAsset != null) PLAY AUDIO
            //    shootAudioSources.PlayAudio(currentWeapon.weaponConfig.shootAudioAsset.name, false);
            cameraMovement.recoil.AddRecoil(IsAiming());
            viewmodel.ShowMuzzleFlash();
        }
    }

    public struct HitData
    {
        public GameObject hitObject;
        public int colliderId;
        public Vector3 point;
        public Vector3 direction;
        public Vector3 normal;
        public float damage;
    }
}