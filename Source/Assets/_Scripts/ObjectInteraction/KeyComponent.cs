using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyComponent : MonoBehaviour, Interactable
{
    public KeyType KeyType { get; set; }
    Dictionary<string, System.Action<object[]>> actions;

	void Start ()
    {
        this.actions = new Dictionary<string, System.Action<object[]>>();
        this.actions.Add("Pickup", Pickup);
	}

    void Pickup(object[] param)
    {
        if(param[0].GetType() != typeof(Player))
        {
            throw new UnityException("Wrong parameter passed to \"Pickup\"");
        }

        Player player = param[0] as Player;
        player.CollectedKeys.Add(this.KeyType);

        Destroy(this.gameObject);
    }

    public Dictionary<string, System.Action<object[]>> GetActions()
    {
        return actions;
    }

    public string GetObjectName()
    {
        return "Key";
    }

    public string GetInteractText()
    {
        return "Press F to pick up key";
    }

}
