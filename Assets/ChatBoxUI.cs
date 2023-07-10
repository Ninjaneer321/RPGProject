using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatBoxUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chatText;
    ChatBox chatBox;

    public bool isOpened = true;
    void Awake()
    {
        gameObject.SetActive(true);

        chatBox = GameObject.FindGameObjectWithTag("Player").GetComponent<ChatBox>();

        if (chatBox != null)
        {
            chatBox.onChange += RefreshUI;
        }

        RefreshUI();
    }

    public void Toggle()
    {
        if (isOpened)
        {
            isOpened = false;
        }
        else
        {
            isOpened = true;
        }
        gameObject.SetActive(!gameObject.activeSelf);
    } 
    private void RefreshUI()
    {
        chatText.text = chatBox.GetText();

    }

    public void ClearText()
    {
        chatText.SetText("");
    }


}
