using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private int damage;

	void Awake ()
    {
        rigidbody = this.GetComponent<Rigidbody>();
	}

    public void Fire(float speed, Vector3 direction, int damage)
    {
        this.damage = damage;
        this.transform.LookAt(direction);
        this.rigidbody.AddForce(speed * direction * Time.deltaTime, ForceMode.Impulse);

        Destroy(this.gameObject, 5f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.transform.CompareTag("Player"))
        {
            col.transform.GetComponent<PlayerCombat>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
