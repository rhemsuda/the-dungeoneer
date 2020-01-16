using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshSurface))]
public class Room : MonoBehaviour 
{
	public RoomType RoomType;
	public Mesh CombinedMesh;
	public int RoomWeight;
	public int RoomNumber;

    public bool Buildable = true;
    public KeyType HeldKey;

    //public Vector3[] SpawnLocations;
	public List<RoomConnector> AvailableConnectors = new List<RoomConnector>();
	public List<Passage> PassageWays = new List<Passage> ();

    public NavMeshSurface NavSurface { get; private set; }

	// Use this for initialization
	void Awake () 
	{
		AvailableConnectors = this.GetConnectors().ToList();
        NavSurface = this.GetComponent<NavMeshSurface>();
    }

	public Passage[] GetAllDoors()
	{
		Passage[] doorPassages = PassageWays.Where(p => p.Type != PassageType.Open).ToArray();
		return doorPassages;
	}

	public Passage[] GetDoors(bool lockedFlag)
	{
		Passage[] doorPassages;
        PassageType passageType = (lockedFlag) ? PassageType.LockedDoor : PassageType.Door;
        doorPassages = PassageWays.Where(p => p.Type == passageType).ToArray();
        return doorPassages;
	}
		
	public RoomConnector[] GetConnectors()
	{
		List<RoomConnector> connectors = new List<RoomConnector> ();

		GameObject[] cons = transform.FindGameObjectsByChildTag ("Connector").ToArray ();
		GameObject[] openings = transform.FindGameObjectsByChildTag ("Opening").ToArray ();
        GameObject[] walls = transform.FindGameObjectsByChildTag("Wall").ToArray();

		for (int i = 0; i < cons.Length; i++)
		{
			connectors.Add (new RoomConnector (cons [i], openings [i], walls[i], i));          
		}
			
		return connectors.ToArray ();
	}

}
