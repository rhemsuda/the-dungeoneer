using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class Hitmarker : MonoBehaviour
{
    public void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }

    public void ShowHit(int hitAmount, Color hitColor, bool critical = false)
    {
        TextMesh textMesh = this.GetComponent<TextMesh>();
        textMesh.text = hitAmount.ToString();
        textMesh.color = hitColor;
        textMesh.fontSize = (critical) ? textMesh.fontSize * 2 : textMesh.fontSize;
        this.GetComponent<Animation>().Play();
        Destroy(this.transform.parent.gameObject, 3);
    }
}
