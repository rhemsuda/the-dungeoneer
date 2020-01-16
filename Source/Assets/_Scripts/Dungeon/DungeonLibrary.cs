using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DungeonLibrary : MonoBehaviour
{
    private List<GameObject> startRoomPrefabs = new List<GameObject>();
    private List<GameObject> dungeonRoomPrefabs = new List<GameObject>();
    private List<GameObject> hallRoomPrefabs = new List<GameObject>();
    private List<GameObject> bossRoomPrefabs = new List<GameObject>();

    public GameObject DoorPrefab;
    public GameObject LockedDoorPrefab;
    public GameObject BossDoorPrefab;
    public GameObject KeyPrefab;

    public GameObject[] OrbPrefabs;
    public Material[] KeyMaterials;
    public Sprite[] KeySprites;

    private static DungeonLibrary _instance;
	public static DungeonLibrary Instance { get { return _instance; } }

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

		LoadRoomObjects(Resources.LoadAll<GameObject>("Dungeon/Rooms/Start"), startRoomPrefabs);
		LoadRoomObjects(Resources.LoadAll<GameObject>("Dungeon/Rooms/Dungeon"), dungeonRoomPrefabs);
		LoadRoomObjects(Resources.LoadAll<GameObject>("Dungeon/Rooms/Hall"), hallRoomPrefabs);
		LoadRoomObjects(Resources.LoadAll<GameObject>("Dungeon/Rooms/Boss"), bossRoomPrefabs);

		this.DoorPrefab = Resources.Load<GameObject>("Dungeon/Doors/Door");
		this.LockedDoorPrefab = Resources.Load<GameObject>("Dungeon/Doors/LockDoor");
        this.BossDoorPrefab = Resources.Load<GameObject>("Dungeon/Doors/BossDoor");
        this.KeyPrefab = Resources.Load<GameObject>("Dungeon/Keys/Key");

		this.OrbPrefabs = Resources.LoadAll<GameObject>("Dungeon/Doors/ColorOrbs");
		this.KeyMaterials = Resources.LoadAll<Material>("Dungeon/Keys/KeyMaterials");
        this.KeySprites = Resources.LoadAll<Sprite>("Dungeon/Keys/KeySprites");
    }


    public GameObject GetRandomRoom(RoomType roomType)
    {
        GameObject[] rooms = GetRoomsList(roomType);
        int roomIndex = new System.Random().Next(rooms.Length);
        return rooms[roomIndex];
    }

    public GameObject[] GetRoomsList(RoomType roomType)
    {
        GameObject[] rooms = (roomType.Equals(RoomType.Start)) ? startRoomPrefabs.ToArray()
            : (roomType.Equals(RoomType.Dungeon)) ? dungeonRoomPrefabs.ToArray()
            : (roomType.Equals(RoomType.Hall)) ? hallRoomPrefabs.ToArray()
            : bossRoomPrefabs.ToArray();

        return rooms;
    }

    void LoadRoomObjects(GameObject[] roomObjects, List<GameObject> rooms)
    {
        foreach (GameObject obj in roomObjects)
        {
            Room r = obj.GetComponent<Room>();
            if (r != null)
            {
                r.CombinedMesh = CreateCombinedMesh(obj);
                rooms.Add(obj);
            }
        }
    }

    Mesh CreateCombinedMesh(GameObject room)
    {
        MeshFilter[] meshFilters = room.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        return combinedMesh;
    }
}
