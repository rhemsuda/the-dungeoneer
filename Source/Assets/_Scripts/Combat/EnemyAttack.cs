using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CombatStats))]
[RequireComponent(typeof(Animator))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private AttackType type;
    [SerializeField]
    private GameObject attackObject;

    private GameObject playerObject;
    private CombatStats combatStats;
    private Animator anim;
    private float lastAttackTime;

    // Use this for initialization
    void Start ()
    {
        combatStats = this.GetComponent<CombatStats>();
        anim = this.GetComponent<Animator>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
	}

    public void Attack()
    {
        if ((Time.time - lastAttackTime) >= combatStats.AttackSpeed)
        {
            lastAttackTime = Time.time;
            switch (type)
            {
                case AttackType.Melee:
                    this.MeleeAttack();
                    break;
                case AttackType.Projectile:
                    this.ProjectileAttack();
                    break;
            }
        }
    }

    private void MeleeAttack()
    {
        //Set attack trigger
        anim.SetTrigger("Attack");

        //Deal damage on attack trigger
        playerObject.GetComponent<PlayerCombat>().TakeDamage(CalculatedDamage());
    }

    private void ProjectileAttack()
    {
        anim.SetTrigger("Attack");
    }

    private void ShootProjectile()
    {
        //Instantiate projectile and set damage values
        if (attackObject != null)
        {
            Transform spellSpawn = transform.FindGameObjectsByChildName("SpellSpawn")[0].transform;
            GameObject go = Instantiate(attackObject, spellSpawn.position, Quaternion.identity);

            Vector3 targetPosition = playerObject.transform.position;
            Vector3 projectileDirection = targetPosition - spellSpawn.position;
            projectileDirection.y = 0;

            Projectile p = go.GetComponent<Projectile>();            
            p.Fire(1000f, projectileDirection.normalized, CalculatedDamage());
        }
    }

    private int CalculatedDamage()
    {
        int minDamage = combatStats.CurrentDamage - 15;
        minDamage = (minDamage < 0) ? minDamage = 0 : minDamage;
        return Random.Range(minDamage, combatStats.CurrentDamage);
    }

    public void PlayMagicSound()
    {
        SoundManager.Instance.SoundEffectsAudioSource.clip = SoundManager.Instance.MagicShotClip;
        SoundManager.Instance.SoundEffectsAudioSource.Play();
    }

    public void PlayFireSound(int on)
    {
        if(on == 0)
        {
            SoundManager.Instance.SoundEffectsAudioSource.loop = false;
            SoundManager.Instance.SoundEffectsAudioSource.Stop();
        }
        else
        {
            SoundManager.Instance.SoundEffectsAudioSource.clip = SoundManager.Instance.FireClip;
            SoundManager.Instance.SoundEffectsAudioSource.loop = true;
            SoundManager.Instance.SoundEffectsAudioSource.Play();
        }
    }

    private enum AttackType
    {
        Melee,
        Projectile
    }
}
