using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarFixer : MonoBehaviour
{
    [SerializeField] Scrollbar scrollBar;
    // Start is called before the first frame update
    void Awake()
    {
        scrollBar.value = 1;
    }
}
