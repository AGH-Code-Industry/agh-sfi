using DeadFusion.GameCore.Weapons;
using System.IO;
using System.Text;
using UnityEngine;

namespace DeadFusion.GameCore.Items
{
    [CreateAssetMenu(fileName = "WeaponItem", menuName = "ItemDefinitions/WeaponItem", order = 0)]
    public class WeaponItemDefinition : SelectableItemDefinition
    {
        public WeaponConfig weaponConfig;
        public int defaultAmmoReserve;

        public override Item GetItem()
        {
            return new WeaponItem(key, name, description, icon, weaponConfig, defaultAmmoReserve);
        }
    }

    public class WeaponItem : SelectableItem
    {
        public readonly WeaponConfig weaponConfig;
        public int loadedAmmo;
        public int ammoReserve;

        public WeaponItem(string key, string name, string description, Sprite icon, WeaponConfig weaponConfig, int defaultAmmoReserve) : base(key, name, description, icon)
        {
            this.weaponConfig = weaponConfig;
            ammoReserve = defaultAmmoReserve;
            loadedAmmo = weaponConfig.magazineSize;
        }

        public override void OnSelect(GameObject player)
        {
            player.GetComponent<WeaponSystem>().EquipWeapon(this);
        }

        public override void OnDeselect(GameObject player)
        {
            player.GetComponent<WeaponSystem>().DeEquipWeapon();
        }

        public override bool CanDeselect(GameObject player)
        {
            return true;
        }

        public override byte[] SerializeData()
        {
            MemoryStream memStream = new MemoryStream();
            using (var writer = new BinaryWriter(memStream, Encoding.UTF8, false))
            {
                writer.Write(loadedAmmo);
                writer.Write(ammoReserve);
            }
            return memStream.ToArray();
        }

        public override void DeserializeData(byte[] data)
        {
            MemoryStream memStream = new MemoryStream(data);
            using (var reader = new BinaryReader(memStream, Encoding.UTF8, false))
            {
                loadedAmmo = reader.ReadInt32();
                ammoReserve = reader.ReadInt32();
            }
        }
    }
}