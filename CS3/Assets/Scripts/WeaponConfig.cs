using UnityEngine;

namespace DeadFusion.GameCore.Weapons
{
    [System.Serializable]
    public struct WeaponConfig
    {
        public GameObject viewModel;
        public float adsFov;

        [Header("Damage")]
        public float damage;
        public float damageFalloffStart;
        public float damageFalloffEnd;

        [Header("Firing")]
        public FireMode fireMode;
        public float fireRate;
        public int bulletCount;

        [Header("Ammunition")]
        public LoadMode loadMode;
        public int magazineSize;
        public int ammoReserveCapacity;
        public int clipLoadAmount;

        [Space()]
        public RecoilSettings recoilSettings;

        [Space()]
        public SpreadSettings spreadSettings;

        [Space()]
        public WeaponPivotRotation.PivotRotationSettings pivotRotationSettings;
    }

    [System.Serializable]
    public struct RecoilSettings
    {
        public float interpolation;
        public float verticalRecoil;
        public float horizontalRecoilMin;
        public float horizontalRecoilMax;
        public float adsMultiplier;
    }

    [System.Serializable]
    public struct SpreadSettings
    {
        public float spreadAngle;
        public float adsSpreadAngle;
    }

    public enum FireMode
    {
        Single,
        Automatic
    }

    public enum LoadMode
    {
        Magazine,
        Clip
    }
}