using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Passage
{
	public bool Locked { get; set; }
	public PassageType Type { get; set; }
	public KeyType KeyType { get; set; }
	public Room[] Rooms = new Room[2];

	public Vector3 ConnectionPosition { get; set; }
	public Quaternion ConnectionRotation { get; set; }
	public GameObject DoorModel { get; private set; }
    public NavMeshLink NavPassage { get; set; }

	public Passage(Room room1, Room room2, NavMeshLink navPassage, PassageType type = PassageType.Open, KeyType keyType = KeyType.None)
	{
		this.Rooms [0] = room1;
		this.Rooms [1] = room2;
        this.NavPassage = navPassage;
		this.Type = type;
		this.KeyType = keyType;
		this.Locked = false;
	}

	public Room NotCurrent(Room currentRoom)
	{
		return (Rooms [0] == currentRoom) ? Rooms [1] : Rooms [0];
	}

	public void BuildDoor()
	{
		if (DoorModel == null) 
		{
			if (Type == PassageType.Door) 
			{
				//Create normal door prefab
				this.DoorModel = MonoBehaviour.Instantiate(DungeonLibrary.Instance.DoorPrefab, ConnectionPosition, ConnectionRotation, Rooms[0].transform);
			} 
			else if (Type == PassageType.LockedDoor) 
			{
				//Create locked door prefab
				this.DoorModel = MonoBehaviour.Instantiate(DungeonLibrary.Instance.LockedDoorPrefab, ConnectionPosition, ConnectionRotation, Rooms[0].transform);
				GameObject colorOrb = DungeonLibrary.Instance.OrbPrefabs.Where(o => o.name == Enum.GetName(typeof(KeyType), KeyType) + "Orb").First();
				MonoBehaviour.Instantiate(colorOrb, DoorModel.transform.Find("OrbHolder").position, Quaternion.identity, DoorModel.transform.Find("OrbHolder"));
			}
            else if(Type == PassageType.BossDoor)
            {
                this.DoorModel = MonoBehaviour.Instantiate(DungeonLibrary.Instance.BossDoorPrefab, ConnectionPosition, ConnectionRotation, Rooms[0].transform);
            }

            DoorModel.GetComponent<DoorComponent>().SetPassage(this);
		}
	}

    public void EnableNavigation()
    {
        this.NavPassage.enabled = true;
    }

    public void DestroyOrb()
    {
        if(Type == PassageType.LockedDoor)
        {
            if(DoorModel != null)
            {
                GameObject orbHolder = DoorModel.transform.Find("OrbHolder").gameObject;
                if (orbHolder != null)
                {
                    MonoBehaviour.Destroy(orbHolder);
                }
            }
        }
    }

}

public enum PassageType
{
	Open,
	Door,
	LockedDoor,
    BossDoor
}

public enum KeyType
{
	None,
	LightRed,
	LightGreen,
	LightBlue,
	LightOrange,
	LightPurple,
	Yellow,
	Red,
	Green,
	Blue,
	Orange,
	Purple
}
