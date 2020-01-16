using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Dont know where else to put this currently
    public List<KeyType> CollectedKeys = new List<KeyType>();

    private const int MAX_POTIONS = 7;
    public int numPotions = 2;

    void Start()
    {
        this.UpdatePotions();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            this.UsePotion();
        }
    }

    public void AddPotion()
    {
        if(numPotions < MAX_POTIONS)
        {
            numPotions++;
            UpdatePotions();
        }
    }

    public void UsePotion()
    {
        if(numPotions > 0)
        {
            int heal = this.GetComponent<CombatStats>().MaxHealth / 2;
            this.GetComponent<CombatStats>().IncreaseHealth(heal);
            numPotions--;
            UpdatePotions();
        }     
    }

    private void UpdatePotions()
    {
        for(int i = 0; i < MAX_POTIONS; i++)
        {
            if(i < numPotions)
            {
                HUD.Instance.PotionHolders[i].sprite = HUD.Instance.FullPotionSprite;
            }
            else
            {
                HUD.Instance.PotionHolders[i].sprite = HUD.Instance.EmptyPotionSprite;
            }
        }
    }

}
