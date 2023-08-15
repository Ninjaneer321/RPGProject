using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

public class EnemyAbilityPopulate : MonoBehaviour
{
    [SerializeField] ActionInventoryItem[] abilities;

    ActionStore actionStore;
    // Start is called before the first frame update

    private void Awake()
    {
        actionStore = GetComponent<ActionStore>();
    }
    void Start()
    {
        PopulateAbilities();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void PopulateAbilities()
    {
        foreach (var ability in abilities)
        {
            actionStore.AddAction(ability, 0, 1);
        }
    }
}
