using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonDoorComponent : MonoBehaviour, Interactable
{
    private Animator anim;
    private bool picking = false;
    private bool locked = true;
    private bool opened = false;

    Dictionary<string, System.Action<object[]>> actions;

    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();

        this.actions = new Dictionary<string, System.Action<object[]>>();
        this.actions.Add("OpenDoor", OpenDoor);
    }

    void OpenDoor(object[] param)
    {
        if(locked && !picking)
        {
            StartCoroutine("PickLock");
        }
        else if(!locked)
        {
            anim.SetTrigger("OpenDoor");
            this.enabled = false;
            this.opened = true;
        }
    }

    IEnumerator PickLock()
    {
        this.picking = true;
        yield return new WaitForSeconds(2f);

        float roll = Random.value;
        if(roll > 0.5f)
        {
            this.locked = false;
        }
        this.picking = false;
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
        return (opened) ? string.Empty : (picking) ? "Picking Lock..." : (locked) ? "Press F to pick lock" : "Press F to open";
    }

}
