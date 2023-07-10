using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectHairColor : MonoBehaviour
{
    [Header("Color Values")]
    public float redAmount;
    public float greenAmount;
    public float blueAmount;
    public float alphaAmount;

    public Color currentHairColor;

    [Header("Color Sliders")]
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    //We grab the material from the skinmesh renderer, and change the color properties of the material.
    public List<SkinnedMeshRenderer> rendererList = new List<SkinnedMeshRenderer>();

    public void UpdateSliders()
    {
        redAmount = redSlider.value;
        greenAmount = greenSlider.value;
        blueAmount = blueSlider.value;
        SetHairColor();
    }
    public void SetHairColor()
    {
        currentHairColor = new Color(redAmount, greenAmount, blueAmount, alphaAmount);

        for (int i = 0; i < rendererList.Count; i++)
        {
            rendererList[i].material.SetColor("_Color_Hair", currentHairColor);
            rendererList[i].material.SetColor("_Color_Stubble", currentHairColor);
        }
    }
}
