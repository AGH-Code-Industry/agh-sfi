using System.Collections;
using UnityEngine;

namespace DeadFusion.GameCore.Weapons
{
    public class WeaponViewmodel : Viewmodel
    {
        [SerializeField] GameObject muzzleFlash;
        [SerializeField] float flashDuration = 0.01f;

        [HideInInspector]
        public WeaponSystem weaponSystem;
        Coroutine currentFlashCoroutine;

        public void OnLoad()
        {
            weaponSystem.LoadAmmo();
        }

        public void ShowMuzzleFlash()
        {
            if (currentFlashCoroutine != null)
                StopCoroutine(currentFlashCoroutine);
            currentFlashCoroutine = StartCoroutine(ShowMuzzleFlashCoroutine());
        }

        IEnumerator ShowMuzzleFlashCoroutine()
        {
            muzzleFlash.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            muzzleFlash.SetActive(false);
        }
    }
}