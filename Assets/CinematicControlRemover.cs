using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
public class CinematicControlRemover : MonoBehaviour
{

    GameObject player;
    [SerializeField] GameObject followCamera;



    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayableDirector>().played += DisableControl;
        GetComponent<PlayableDirector>().stopped += EnableControl;
        player = GameObject.FindGameObjectWithTag("Player");

    }

    private void EnableControl(PlayableDirector obj)
    {
        Debug.Log("Open Up Intro Window!");
        player.GetComponent<InputManager>().enabled = true;
    }

    private void DisableControl(PlayableDirector obj)
    {
        player.GetComponent<InputManager>().enabled = false;

    }

}
