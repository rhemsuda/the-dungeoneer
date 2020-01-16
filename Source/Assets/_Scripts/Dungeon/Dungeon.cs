using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public List<Room> Rooms = new List<Room>();
    public DungeonSize Size { get; set; }
    public Vector3 SpawnPoint { get; private set; }

    private static Dungeon _instance;
    public static Dungeon Instance { get { return _instance; } }

    private void Awake()
    {
        //Create singleton instance on awake. We want to use these values freely throughout our application.
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void SetSpawnPoint()
    {
        this.SpawnPoint = Rooms[0].transform.Find("PlayerSpawn").position;
    }

    public int GetDungeonWeight()
	{
		int dungeonWeight = 0;
        Rooms.ForEach (d => dungeonWeight += d.RoomWeight);
		return dungeonWeight;
	}

	public void AddRoom(Room room)
	{
		room.RoomNumber = Rooms.Count;
        Rooms.Add (room);
	}

    public Room GetRoomWithKey(KeyType key)
    {
        Room room = Rooms.Where(r => r.HeldKey == key).FirstOrDefault();
        return room;
    }

	public Room[] GetBuildableRooms()
	{
		return Rooms.Where (r => r.Buildable == true).ToArray();
	}

    public Room GetRoomWithOpenConnections()
    {
        Room openRoom = null;
        while(openRoom == null)
        {
            int index = RandomNumber.Between(0, Rooms.Count - 1);
            openRoom = (Rooms[index].AvailableConnectors.Count > 0) ? Rooms[index] : null;
        }
        return openRoom;
    }
}

public enum DungeonSize
{
	Small = 100,
	Medium = 200,
	Large = 300
}