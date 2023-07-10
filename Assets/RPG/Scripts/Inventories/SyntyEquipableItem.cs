using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "Equipment/SyntyEquipableItem", fileName = "SyntyEquipableItem", order = 0)]
    public class SyntyEquipableItem : StatsEquipableItem
    {
        [System.Serializable]
        public class ItemPair
        {
            public string category = "";
            public int index;
        }

        [System.Serializable]
        public class ItemColor
        {
            public string category = "";
            public Color color = Color.magenta;
        }

        [Header("The name of the object in the Modular Characters Prefab representing this item.")]
        [SerializeField]
        List<ItemPair> objectsToActivateMale = new List<ItemPair>();
        [SerializeField]
        List<ItemPair> objectsToActivateFemale = new List<ItemPair>();

        [SerializeField] List<ItemColor> colorChanges = new List<ItemColor>();

        [Header("Slot Categories to deactivate when this item is activated.")]
        [SerializeField]
        List<string> slotsToDeactivate = new List<string>();

        public List<string> SlotsToDeactivate => slotsToDeactivate;
        public List<ItemPair> ObjectsToActivateM => objectsToActivateMale;
        public List<ItemPair> ObjectsToActivateF => objectsToActivateFemale;
        public List<ItemColor> ColorChangers => colorChanges;
    }
}

