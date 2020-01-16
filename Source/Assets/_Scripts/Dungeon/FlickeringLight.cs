using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    public bool flickering = true;
    public float minThreshold = 0.4f;
    public float maxThreshold = 1f;

    private new Light light;
    private float initialIntensity;

	void Start ()
    {
        light = this.GetComponent<Light>();
        initialIntensity = light.intensity;
        StartCoroutine(Flicker());
	}
	
    IEnumerator Flicker()
    {
        while(flickering)
        {
            if (light != null)
            {
                float randomIntensity = Random.Range(minThreshold, maxThreshold);
                randomIntensity = randomIntensity * initialIntensity;
                light.intensity = randomIntensity;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

}
