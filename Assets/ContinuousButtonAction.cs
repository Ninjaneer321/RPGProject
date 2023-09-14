using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContinuousButtonAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RotateObject rotateObject;
    private bool isPointerDown = false;
    private float repeatInterval = 0.025f; // The interval between function invocations while the pointer is held down

    [SerializeField] bool directionBool = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        StartCoroutine(RepeatFunction());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    private IEnumerator RepeatFunction()
    {
        // Call your repeating function here
        while (isPointerDown)
        {
            YourRepeatingFunction();
            yield return new WaitForSeconds(repeatInterval);
        }
    }

    private void YourRepeatingFunction()
    {
        if (!directionBool)
        {
            rotateObject.RotateButtonInputA();
        }
        else if (directionBool)
        {
            rotateObject.RotateButtonInputD();
        }
    }
}