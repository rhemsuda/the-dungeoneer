using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnchor : MonoBehaviour
{
	public Transform followObject;
	public Vector3 localOffset;
	public Vector3 screenOffset;

	private RectTransform myCanvas;

	void Start () 
	{
		myCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();	
	}

	void LateUpdate () 
	{
		Vector3 worldPoint = followObject.TransformPoint (localOffset);
		Vector3 viewportPoint = Camera.main.WorldToViewportPoint (worldPoint);

		viewportPoint -= 0.5f * Vector3.one;
		viewportPoint.z = 0;

		Rect rec = myCanvas.rect;
		viewportPoint.x *= rec.width;
		viewportPoint.y *= rec.height;

		transform.localPosition = viewportPoint + screenOffset;
	}
}
