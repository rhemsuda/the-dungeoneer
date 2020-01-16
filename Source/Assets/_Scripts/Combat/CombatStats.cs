using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStats : MonoBehaviour 
{
	public int MaxHealth;
	public int MaxDamage;
	public float AttackDistance;
	public float AttackSpeed;

	public int CurrentHealth { get; private set; }
	public int CurrentDamage { get; private set; }

    public void Awake()
    {
        this.CurrentHealth = MaxHealth;
        this.CurrentDamage = MaxDamage;
    }

	public void DecreaseHealth(int value)
	{
		if (CurrentHealth - value <= 0) 
		{
			CurrentHealth = 0;
		} 
		else
		{
			CurrentHealth -= value;
		}
    }

	public void IncreaseHealth(int value)
	{
		if (CurrentHealth + value > MaxHealth) 
		{
			CurrentHealth = MaxHealth;
		}
		else 
		{
			CurrentHealth += value;
		}
	}
}
