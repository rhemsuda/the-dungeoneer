using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreathe : MonoBehaviour
{
    ParticleSystem particleSystem;
    GameObject playerObject;

    void Start()
    {
        particleSystem = transform.FindGameObjectsByChildName("FireBreathe")[0].GetComponent<ParticleSystem>();

    }

    void Update()
    {
        if(playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
            return;
        }

        Vector3 pos = playerObject.transform.position - particleSystem.transform.position;
        var newRot = Quaternion.LookRotation(pos);
        particleSystem.transform.rotation = Quaternion.Lerp(particleSystem.transform.rotation, newRot, 0.05f);
    }

    public void BreatheFire(int breatheFire)
    {
        if (breatheFire == 1)
        {
            particleSystem.Play();
        }
        else
        {
            particleSystem.Stop();
        }
    }
}
