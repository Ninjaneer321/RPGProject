using System.Collections;
using System.Collections.Generic;
using GameDevTV.UI;
using UnityEngine;

public class CloseAllUI : MonoBehaviour
{
    [SerializeField] List<GameObject> listOfGameObjects = new List<GameObject>();
    [SerializeField] KeyCode toggleKey = KeyCode.Escape;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            CloseAllUIElements();
        }
    }

    void CloseAllUIElements()
    {
        foreach (var gameObject in listOfGameObjects)
        {
            //if (!gameObject.GetComponent<ShowHideUI>().isOpened)
            //{
            //    Debug.Log(gameObject + " isnt opened");
            //}
            if (gameObject.GetComponent<ShowHideUI>().isOpened)
            {
                gameObject.GetComponent<ShowHideUI>().Toggle();
            }
        }
    }
}
