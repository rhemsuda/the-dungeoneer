using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CombatStats))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private GameObject hitmarkerPrefab;

    private CombatStats playerStats;
	private Animator anim;

    //TODO: Because of time constraints i need to make multiple variables here, this should be abstracted
    private float cooldown;
    private float abilityCooldown2;
    private float abilityCooldown3;

    private bool basicAttackSwitch;

    private bool alive = true;

	void Start () 
	{
		this.playerStats = GetComponent<CombatStats> ();
        this.anim = GetComponent<Animator>();
    }
		
	void Update () 
	{
		if (!alive)
			return;

        //TODO: Separate this into a more generic healthbar script
        float width = (((playerStats.CurrentHealth - 0) * (250 - 95)) / (playerStats.MaxHealth - 0)) + 95;
        RectTransform r = GameObject.Find("HealthbarMask").GetComponent<RectTransform>();
        r.sizeDelta = new Vector2(width, r.sizeDelta.y);

		cooldown -= Time.deltaTime;
        abilityCooldown2 -= Time.deltaTime;
        abilityCooldown3 -= Time.deltaTime;
		if (cooldown <= 0) 
		{
			if (Input.GetButtonDown ("Attack1")) 
			{
                string trigger = (basicAttackSwitch == false) ? "Attack1" : "Attack2";           
                anim.SetTrigger (trigger);
                cooldown = playerStats.AttackSpeed;
                basicAttackSwitch = !basicAttackSwitch;
            }
            else if (Input.GetButtonDown("Attack2"))
            {
                if (abilityCooldown2 <= 0)
                {
                    anim.SetTrigger("Ability1");
                    cooldown = playerStats.AttackSpeed;
                    abilityCooldown2 = 10f;
                }
                    
            }
            else if (Input.GetButtonDown("Attack3"))
            {
                if (abilityCooldown3 <= 0)
                {
                    anim.SetTrigger("Ability2");
                    cooldown = playerStats.AttackSpeed;
                    abilityCooldown3 = 20f;
                }
            }
        }

		if (playerStats.CurrentHealth <= 0)
			Die ();
	}

    public void HitEnemies(int skillId)
    {
        List<GameObject> enemiesToHit = new List<GameObject>();
        if (skillId == 2)
        {
            enemiesToHit.AddRange(Physics.OverlapSphere(transform.position, playerStats.AttackDistance * 2, 1 << 10).Select(c => c.gameObject));
            enemiesToHit.Remove(this.gameObject);
        }
        else
        {
            enemiesToHit.Add(this.GetClosestEnemy());
        }

        foreach(GameObject enemy in enemiesToHit)
        {
            if (enemy != null)
            {
                float damageModifier = (skillId == 1) ? 2f : (skillId == 2) ? 1.5f : 1f;

                //Get the combat stats of the closest enemy and apply damage
                CombatStats cb = enemy.GetComponent<CombatStats>();
                int calculatedDamage = CalculatedDamage();
                bool critical = Random.value > 0.9f;
                calculatedDamage = critical ? calculatedDamage * 2 : calculatedDamage;
                calculatedDamage = (int)(calculatedDamage * damageModifier);
                cb.DecreaseHealth(calculatedDamage);

                //Display hit marker above enemies head
                //TODO: This is tightly coupled. Fix it
                Vector3 hitmarkerPosition = enemy.transform.Find("HitmarkerSpawn").transform.position;
                GameObject go = Instantiate(hitmarkerPrefab, hitmarkerPosition, Quaternion.identity);
                go.GetComponentInChildren<Hitmarker>().ShowHit(calculatedDamage, Color.white, critical);
            }
        } 
    }

    public void TakeDamage(int damage)
    {
        //Display hit marker above enemies head
        Vector3 hitmarkerPosition = this.transform.Find("HitmarkerSpawn").transform.position;
        GameObject go = Instantiate(hitmarkerPrefab, hitmarkerPosition, Quaternion.identity);
        go.GetComponentInChildren<Hitmarker>().ShowHit(damage, Color.red);

        playerStats.DecreaseHealth(damage);
    }

    private int CalculatedDamage()
    {
        int minDamage = playerStats.CurrentDamage - 15;
        minDamage = (minDamage < 0) ? minDamage = 0 : minDamage;
        return Random.Range(minDamage, playerStats.CurrentDamage);
    }

    private GameObject GetClosestEnemy()
	{
		GameObject closestEnemy = null;

		foreach (Collider col in Physics.OverlapSphere(transform.position, playerStats.AttackDistance, 1 << 10)) 
		{
            //Check if player is facing the object, if not skip this collider
            float dot = Vector3.Dot(transform.forward, (col.transform.position - transform.position).normalized);

            if (Vector3.Dot (transform.forward, (col.transform.position - transform.position).normalized) >= 0.6f) 
			{
				if (closestEnemy != null) 
				{
					float currentClosestDistance = Vector3.Distance (transform.position, closestEnemy.transform.position);
					float newDistance = Vector3.Distance (transform.position, col.transform.position);
					if (newDistance < currentClosestDistance) 
					{
						closestEnemy = col.gameObject;
					}
				}
				else 
				{
					closestEnemy = col.gameObject;	
				}
			}
		}

		return closestEnemy;
	}

    float damageRate = 0.1f;
    float lastHit = 0f;
    void OnParticleCollision(GameObject other)
    {
        if (Time.time - lastHit > damageRate)
        {
            lastHit = Time.time;
            int levelModifier = (Dungeon.Instance.Size == DungeonSize.Small) ? 1 : (Dungeon.Instance.Size == DungeonSize.Medium) ? 2 : 3;
            playerStats.DecreaseHealth(4 * levelModifier);
        }
    }

    private void Die()
	{
        if(alive)
        {
            this.GetComponent<PlayerController>().enabled = false;
            anim.SetTrigger("Die");
            alive = false;
        }
		
	}

    public void PlaySwordSlashSound()
    {
        SoundManager.Instance.PlayerAudioSource.clip = SoundManager.Instance.SwordSwooshClip;
        SoundManager.Instance.PlayerAudioSource.Play();
    }
}
