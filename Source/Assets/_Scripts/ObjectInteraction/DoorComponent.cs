using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorComponent : MonoBehaviour, Interactable
{
    private Animator anim;
    private Passage passage;

    Dictionary<string, System.Action<object[]>> actions;

    void Start()
    {
        anim = this.GetComponent<Animator>();

        this.actions = new Dictionary<string, System.Action<object[]>>();
        this.actions.Add("OpenDoor", OpenDoor);
    }

    void OpenDoor(object[] param)
    {
        if (param[0].GetType() != typeof(Player))
        {
            throw new UnityException("Wrong parameter passed to \"OpenDoor\"");
        }

        if (passage.Type == PassageType.LockedDoor)
        {
            if (passage.Locked)
            {
                Player player = param[0] as Player;
                if (player.CollectedKeys.Contains(passage.KeyType))
                {                
                    passage.Locked = false;
                    passage.DestroyOrb();
                    player.CollectedKeys.Remove(passage.KeyType);
                }
                else
                {
                    HUD.Instance.SpawnInteractError("Requires Key");
                }
                return;
            }
        }

        if(passage.Locked)
        {
            if (passage.Type == PassageType.LockedDoor)
            {
                Player player = param[0] as Player;
                if (player.CollectedKeys.Contains(passage.KeyType))
                {
                    passage.Locked = false;
                    passage.DestroyOrb();
                    player.CollectedKeys.Remove(passage.KeyType);
                }
                else
                {
                    HUD.Instance.SpawnInteractError("Requires Key");
                }
            }

            if (passage.Type == PassageType.BossDoor)
            {
                var lockedDoors = new List<Passage>();
                Dungeon.Instance.Rooms.ForEach(r => lockedDoors.AddRange(r.GetDoors(true).Where(d => d.Locked)));

                int lockedDoorCount = lockedDoors.Count / 2;
                if(lockedDoorCount > 0)
                {
                    HUD.Instance.SpawnInteractError("You have not unlocked all doors. (" + lockedDoorCount + " remaining)");
                }
                else
                {
                    passage.Locked = false;
                }
            }
            return;
        }

        anim.SetTrigger("OpenDoor");
        passage.EnableNavigation();
    }  

    public void SetPassage(Passage p)
    {
        if (p.Type != PassageType.Open)
        {
            passage = p;
        }
    }

    public Dictionary<string, System.Action<object[]>> GetActions()
    {
        return actions;
    }

    public string GetObjectName()
    {
        return "Door";
    }

    public string GetInteractText()
    {
        return (passage.Type == PassageType.BossDoor && passage.Locked) ? "Press F to unlock Boss Door" 
            : (passage.Type == PassageType.BossDoor) ? "Press F to open Boss Door" 
            : (passage.Locked) ? "Press F to unlock" 
            : "Press F to open";
    }
    
    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
