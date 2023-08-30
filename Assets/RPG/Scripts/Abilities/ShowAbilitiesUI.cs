using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAbilitiesUI : MonoBehaviour
{
    [SerializeField] AbilityUI abilityUI = null;

    private void Awake()
    {

    }

    private void Start()
    {
        abilityUI.SetupAbilities(abilityUI.GetAbilitiesBank());
    }

}
