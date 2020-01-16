using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHealthbar : MonoBehaviour 
{
    [SerializeField]
	private RectTransform healthbarOverlay;

	[SerializeField]
	private CombatStats combatStats;

	void Start () 
	{
		healthbarOverlay = transform.Find("Overlay").GetComponent<RectTransform> ();
	}

	void Update () 
	{
        if (combatStats == null)
            return;

		float newWidth = ((float)combatStats.CurrentHealth / (float)combatStats.MaxHealth) * 100;
        healthbarOverlay.sizeDelta = new Vector2 (newWidth, healthbarOverlay.sizeDelta.y);

		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
			Camera.main.transform.rotation * Vector3.up);
	}
}
