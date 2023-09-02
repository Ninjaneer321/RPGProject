using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;

public class PersonalInformation : MonoBehaviour, ISaveable
{
    [SerializeField] string characterName = "Player2";


    public string GetCharacterName()
    {
        return characterName;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public object CaptureState()
    {
        return characterName;
    }

    public void RestoreState(object state)
    {
        characterName = (string)state;
    }
}
