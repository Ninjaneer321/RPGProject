using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;

public class ChatBox : MonoBehaviour, ISaveable
{
    public event Action onChange;
    string chatBoxText = "";

    public object CaptureState()
    {
        chatBoxText = "";
        return chatBoxText;
    }

    public void RestoreState(object state)
    {
        chatBoxText = (string)state;
    }

    public string GetText()
    {
        return chatBoxText;
    }

    public void UpdateText(string text)
    {

        chatBoxText += text;
        if (onChange != null)
        {
            onChange();
        }

    }
}
