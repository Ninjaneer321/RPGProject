using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.UI;
using RPG.Control;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class OtherInventory : MonoBehaviour, IRaycastable
{
    [SerializeField] ChestPickupManager chestPickupManager = null;

    public CursorType GetCursorType()
    {
        PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        if (Vector3.Distance(this.transform.position, playerManager.transform.position) <= 2.5f)
        {
            return CursorType.Pickup;
        }

        return CursorType.None;
    }

    public bool HandleRaycast(PlayerManager callingController)
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (Vector3.Distance(player.transform.position, this.transform.position) <= 2.5f)
            {
                Debug.Log("Here");
                GameObject otherInventory = GameObject.FindWithTag("OtherInventory");
                otherInventory.GetComponent<ShowHideUI>().ShowOtherInventory(gameObject);
                player.transform.LookAt(this.transform, Vector3.up);
                if (gameObject.CompareTag("ChestPickup") && chestPickupManager.isLooted == false)
                {
                    GetComponent<OtherInventorySpawner>().AddItemsToOtherInventory();
                    chestPickupManager.isLooted = true;
                }
                //FindObjectOfType<ShowHideUI>().ShowOtherInventory(gameObject);
                Debug.Log(gameObject.name);
            }
        }
        return true;
    }

}