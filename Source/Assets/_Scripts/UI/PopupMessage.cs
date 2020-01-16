using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PopupMessage : MonoBehaviour
{
    private string message = string.Empty;
    private float displayTime = 2.0f;
    private Text messageText = null;

    void Start()
    {
        messageText = this.GetComponent<Text>();
    }

    public void SetMessage(string message)
    {
        this.message = message;
    }

    public void SetDisplayTime(float timeSeconds)
    {
        this.displayTime = timeSeconds;
    }

    public void Display()
    {
        StartCoroutine("DisplayAndFade");
    }

    IEnumerator DisplayAndFade()
    {
        Text messageText = this.GetComponent<Text>();
        messageText.text = message;
        yield return new WaitForSeconds(displayTime);

        while(messageText.color.a > 0.0f)
        {
            Color newColor = messageText.color;
            newColor.a -= 0.05f;
            messageText.color = newColor;

            yield return new WaitForSeconds(0.1f);
        }

        Destroy(this.gameObject);
    }
}
