using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log(gameObject.transform.parent);
        //DontDestroyOnLoad(gameObject.transform.root);
    }
}
