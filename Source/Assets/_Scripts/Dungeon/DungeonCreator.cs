using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(DungeonLibrary))]
public class DungeonCreator : MonoBehaviour
{
    private DungeonLibrary dungeonLibrary;
    private System.Random random;

    public Dungeon Dungeon { get; private set; }

    public bool Created { get; private set; }

    // Use this for initialization
    void Start()
    {
        dungeonLibrary = this.GetComponent<DungeonLibrary>();
        this.random = new System.Random();
    }

    public IEnumerator CreateDungeon(DungeonSize size, Action<Dungeon> result)
    {
        this.Dungeon = new GameObject("Dungeon").AddComponent(typeof(Dungeon)) as Dungeon;
        this.Dungeon.gameObject.layer = LayerMask.NameToLayer("Dungeon");
        this.Dungeon.Size = size;

        Transform skeletonParent = new GameObject("Skeletons").transform;
        
        //Choose a random starting room and add it to dungeon
        GameObject startRoomObject = Instantiate(dungeonLibrary.GetRandomRoom(RoomType.Start), Dungeon.gameObject.transform);
        Room startRoom = startRoomObject.GetComponent<Room>();
        Dungeon.AddRoom(startRoom);
        Dungeon.SetSpawnPoint();

        GameObject startRoomSkeleton = CreateSkeleton(startRoom.name, startRoom.CombinedMesh);
        startRoomSkeleton.transform.parent = skeletonParent;

        int maxWeight = (int)Dungeon.Size;

        bool dungeonFinished = false;
        while (!dungeonFinished)
        {
            //Get buildable rooms
            Room[] buildableRooms = Dungeon.GetBuildableRooms();
            if(buildableRooms.Length < 1)
            {
                Room newBuildableRoom = Dungeon.GetRoomWithOpenConnections();
                if(newBuildableRoom != null)
                {
                    newBuildableRoom.Buildable = true;
                    continue;
                }
                else
                {
                    //TODO: Should also look out for getting rooms with open connections but no rooms to fit.
                    //could cause a freeze
                    Debug.LogError("NO ROOMS WITH OPEN CONNECTIONS");

                    //TODO: Start over? Assert?
                }
            }

            //Choose a random buildable room
            Room currentRoom = buildableRooms[RandomNumber.Between(0, buildableRooms.Length - 1)];

			RoomConnector[] currentRoomConnectors = currentRoom.AvailableConnectors.ToArray();
            random.Shuffle(currentRoomConnectors);

            //We dont want medium or large rooms to spawn with one path to start
            int minRoomsToCreate = (Dungeon.Size == DungeonSize.Small && currentRoom.RoomType == RoomType.Start) ? 2
                : ((Dungeon.Size == DungeonSize.Medium || Dungeon.Size == DungeonSize.Large) && currentRoom.RoomType == RoomType.Start) ? 3
                : 1;
         
            //TODO: Weigh the results of this based on the difficulty of the dungeon
            int numberOfRooms = RandomNumber.Between(minRoomsToCreate, currentRoomConnectors.Length);

            for (int connectorIndex = 0; connectorIndex < numberOfRooms; connectorIndex++)
            {
                //Choose a random connector on the starting room
                RoomConnector currentConnector = currentRoomConnectors[connectorIndex];

                //Choose a random room type
                double typeRand = random.NextDouble();
                RoomType roomType = (Dungeon.GetDungeonWeight() > maxWeight) ? RoomType.Boss : (typeRand * 100 < 50) ? RoomType.Hall : RoomType.Dungeon;

                if (Dungeon.GetDungeonWeight() > maxWeight)
                {
                    roomType = RoomType.Boss;  
                }

                GameObject[] roomPrefabs = dungeonLibrary.GetRoomsList(roomType);
                random.Shuffle(roomPrefabs);

                //Shuffle through the rooms of the type we want and try to find a room that fits
                foreach (GameObject roomPrefab in roomPrefabs)
                {
                    Room tempRoom = roomPrefab.GetComponent<Room>();

                    //Get the available connectors and shuffle them
					RoomConnector[] tempRoomConnectors = tempRoom.GetConnectors();
                    random.Shuffle(tempRoomConnectors);

                    //Get information about current connector transform
                    Quaternion currentConnectorRotation = currentConnector.Connector.transform.rotation;
                    Vector3 currentConnectorPosition = currentConnector.Connector.transform.position;

                    //For each room, cycle through the randomly shuffled list of connectors
                    for (int i = 0; i < tempRoomConnectors.Length; i++)
                    {
                        RoomConnector tempRoomConnector = tempRoomConnectors[i];

                        GameObject skeleton = CreateSkeleton(tempRoom.name, tempRoom.CombinedMesh);
                        skeleton.transform.parent = skeletonParent;

                        Quaternion tempConnectorRotation = tempRoomConnector.Connector.transform.rotation;
                        Vector3 tempConnectorPosition = tempRoomConnector.Connector.transform.position;

                        //Get the connection position of the current connector and new connector and
                        //set the skeletons position so the connection points line up
                        Vector3 connectPosition = currentConnectorPosition + (tempRoom.transform.position - tempConnectorPosition);
                        skeleton.transform.position = connectPosition;

                        //Calculate the angles of the connector vectors in order to determine the difference
                        var currentForward = currentConnectorRotation * Vector3.forward;
                        var tempForward = tempConnectorRotation * Vector3.forward;
                        var angleA = Mathf.Atan2(currentForward.x, currentForward.z) * Mathf.Rad2Deg;
                        var angleB = Mathf.Atan2(tempForward.x, tempForward.z) * Mathf.Rad2Deg;

                        //Get rotation difference and rotate skeleton around connection point
                        float angleDifference = Mathf.DeltaAngle(angleA, angleB);
                        skeleton.transform.RotateAround(currentConnectorPosition, Vector3.up, 180 - angleDifference);

                        //Move the skeleton a hair forward so it does not collide with the connector. Only detect collision on rooms.
                        Vector3 oldSkeletonPosition = skeleton.transform.position;
                        skeleton.transform.position += 0.1f * currentConnector.Connector.transform.forward;

                        //Wait for a physics cycle to complete so we can determine if a collision is occuring
                        yield return new WaitForFixedUpdate();

                        //Check if the skeleton has collided with another room.
                        if (!skeleton.GetComponent<MeshSkeleton>().IsColliding)
                        {
                            //Move the position back when no collision was detected
                            skeleton.transform.position = oldSkeletonPosition;

                            //Once room validity has been confirmed and there are no collisions, we instantiate
                            //the new room object and add it to the dungeon
                            GameObject newRoomObject = Instantiate(roomPrefab, skeleton.transform.position, skeleton.transform.rotation);
                            newRoomObject.transform.parent = Dungeon.transform;

                            Room newRoom = newRoomObject.GetComponent<Room>();

                            //Choose a connection type depending on the connecting rooms
                            PassageType passageType = (newRoom.RoomType == RoomType.Dungeon || currentRoom.RoomType == RoomType.Dungeon) ? PassageType.Door : PassageType.Open;

                            //Get connector of newly created room.
                            RoomConnector newRoomConnector = newRoom.GetConnectors().Where(r => r.ConnectorNumber == tempRoomConnector.ConnectorNumber).FirstOrDefault();

                            //Create passageway between current room and new room
                            Passage newRoomPassage = new Passage(currentRoom, newRoom, newRoomConnector.Connector.GetComponent<NavMeshLink>(), passageType);

                            //Enable the nav mesh link this connector holds if the passage is an open passage
                            if (passageType == PassageType.Open)
                            {
                                newRoomPassage.EnableNavigation();
                            }

                            Vector3 newConnectionPosition = (currentConnector.Connector.transform.position + newRoomConnector.Connector.transform.position) / 2;
                            newConnectionPosition.y += 0.25f;
                            newRoomPassage.ConnectionPosition = newConnectionPosition;
                            newRoomPassage.ConnectionRotation = currentConnector.Connector.transform.rotation;

                            if (newRoom.RoomType == RoomType.Boss)
                            {
                                newRoomPassage.Type = PassageType.BossDoor;
                                newRoomPassage.Locked = true;
                                dungeonFinished = true;
                            }

                            currentRoom.PassageWays.Add(newRoomPassage);
                            newRoom.PassageWays.Add(newRoomPassage);
                            currentConnector.Wall.SetActive(false);
                            newRoomConnector.Wall.SetActive(false);

                            currentRoom.AvailableConnectors.RemoveAll(r => r.ConnectorNumber == currentConnector.ConnectorNumber);
                            newRoom.AvailableConnectors.RemoveAll(r => r.ConnectorNumber == newRoomConnector.ConnectorNumber);          

                            Dungeon.AddRoom(newRoom);
                            break;
                        }

                        if (skeleton)
                            Destroy(skeleton);
                    }
                }
            }
          
            //When we have added all the rooms we want, we set the current room as non-buildable
            //and continue looping, choosing a random buildable room to add to.
            currentRoom.Buildable = false;
        }

        //TODO: Choose best spot for boss room and place it.

        //Create the dungeon collider to maximize performance of collision detection throughout dungeon
        this.CreateDungeonCollider();

        //Build procedural navigation mesh. Navigation links are put in place and will link together when instantiated
        this.BuildNavMesh();

        //Place keys and doors so they are always accessible
        this.PlaceKeysAndDoors();

        //Build the doors after creating the dungeon collider so doorways arent affected
        this.BuildDoors();

        //Set the results for this coroutine to be the dungeon
        result(Dungeon);

        //Get rid of our list of skeletons used for placement
        Destroy(skeletonParent.gameObject);
    }

    public void BuildNavMesh()
    {
        Dungeon.Rooms.ForEach(r => r.GetComponent<NavMeshSurface>().BuildNavMesh());
    }

    public void PlaceKeysAndDoors()
    {
        GameObject keyParent = new GameObject("Keys");
        keyParent.transform.parent = Dungeon.transform;
        int totalKeyTypes = Enum.GetValues(typeof(KeyType)).Length - 1;

        //Set the number of doors to create to be the total amount of rooms divided by the distance we want between each key - 1 for safety.
        int keyDistance = 2;
        int numberOfDoors = Dungeon.Rooms.Count / 4;
        numberOfDoors = (numberOfDoors > totalKeyTypes) ? totalKeyTypes : numberOfDoors;

        int doorsCreated = 0;
        while (doorsCreated < numberOfDoors)
        {
            //Choose a random passageway
			List<Passage> passageWays = new List<Passage>();
            Dungeon.Rooms.ForEach (r => passageWays.AddRange (r.GetDoors (false)));

            //If we have run out of passageways that have available doors, we break
            if (passageWays.Count < 1)
                break;

			int index = RandomNumber.Between(0, passageWays.Count - 1);
            Passage randomPassage = passageWays[index];

            //Choose a random room for key
            index = RandomNumber.Between(0, Dungeon.Rooms.Count - 1);
            Room keyRoom = Dungeon.Rooms[index];

            //If key is within distance of other keys OR if the key cant find the door, restart loop
            if(keyRoom.RoomType == RoomType.Boss 
                || FindKey(keyRoom, keyDistance) 
                || !RoomAccessible(keyRoom, randomPassage, randomPassage) 
                || !KeyFindStart(keyRoom, randomPassage))
            {
                continue;
            }

            //Pick random key color and assign it to the key room
            KeyType[] availableKeys = this.GetAvailableKeys();
            index = RandomNumber.Between(0, availableKeys.Length - 1);
            KeyType newKey = availableKeys[index];
            keyRoom.HeldKey = newKey;

			//Set lock information for new random passage.
			randomPassage.Type = PassageType.LockedDoor;
			randomPassage.Locked = true;
			randomPassage.KeyType = newKey;

            //Choose a spawn location for the key
            Vector3[] spawnLocations = keyRoom.transform.FindGameObjectsByChildTag("SpawnLocation").Select(s => s.gameObject.transform.position).ToArray();
            index = RandomNumber.Between(0, spawnLocations.Length - 1);
            Vector3 spawnLocation = spawnLocations[index];      

            //Instantiate a key at the location and set the material to the key colour
            Quaternion randomRotY = Quaternion.Euler(0.0f, UnityEngine.Random.rotation.y, 0.0f);
            string colorName = Enum.GetName(typeof(KeyType), newKey);
            GameObject key = Instantiate(dungeonLibrary.KeyPrefab, spawnLocation, randomRotY, keyParent.transform);
            key.GetComponentInChildren<MeshRenderer>().material = dungeonLibrary.KeyMaterials.Where(m => m.name == "Key" + colorName).First();
            key.GetComponentInChildren<KeyComponent>().KeyType = newKey;

            doorsCreated++;
        }
    }


	private void BuildDoors()
	{
        List<Passage> passageWays = new List<Passage>();
        Dungeon.Rooms.ForEach(r => passageWays.AddRange(r.GetAllDoors()));
        passageWays = passageWays.Distinct().ToList();

        Debug.Log(passageWays.Where(p => p.Type == PassageType.LockedDoor).Count() + " Locked Doors");
        foreach (Passage passage in passageWays)
        {
            passage.BuildDoor();
        }
	}

    private bool FindKey(Room startRoom, int depth)
    {
        //Return false if we make it to zero depth and still havent found key
        if (depth == 0) return false;
        if (startRoom.HeldKey != KeyType.None) return true;

		Room[] adjacentRooms = startRoom.PassageWays.Where (p => p.NotCurrent (startRoom)).Select(p => p.NotCurrent(startRoom)).ToArray();
		for (int i = 0; i < adjacentRooms.Length; i++)
        {
            if (FindKey(adjacentRooms[i], depth - 1))
            { 
                return true;
            }
        }
        
        return false;
    }
		
	private bool RoomAccessible(Room keyRoom, Passage doorPassage, Passage initialPassage, Passage[] unlocked = null)
    {
        Queue<Room> queue = new Queue<Room>();
        queue.Enqueue(keyRoom);

		List<Room> visitedRooms = new List<Room> ();
        visitedRooms.Add(keyRoom);

        List<Passage> accessibleLockedDoors = new List<Passage>();
        if (unlocked != null) accessibleLockedDoors.AddRange(unlocked);
        accessibleLockedDoors.Add(doorPassage);

        Room destination = doorPassage.Rooms.First();
        while(queue.Count > 0)
        {
            Room currentRoom = queue.Dequeue();
            if (currentRoom == null)
                continue;

            if(currentRoom.Equals(destination))
                return true;

            foreach (Passage passage in currentRoom.PassageWays)
            {
                if (!visitedRooms.Contains(passage.NotCurrent(currentRoom)))
                {
                    if (passage.Equals(initialPassage))
                        continue;

                    //If previously passed this door,
                    if (passage.Type != PassageType.LockedDoor 
                        || accessibleLockedDoors.Contains(passage) 
                        || RoomAccessible(Dungeon.GetRoomWithKey(passage.KeyType), passage, initialPassage, accessibleLockedDoors.ToArray()))
                    {
                        queue.Enqueue(passage.NotCurrent(currentRoom));
                        visitedRooms.Add(passage.NotCurrent(currentRoom));
                    }
                } 
			}     

        }

        return false;
    }


    private bool KeyFindStart(Room keyRoom, Passage restrictedPassage)
    {
        Room startRoom = Dungeon.Rooms[0];

        Queue<Room> queue = new Queue<Room>();
        queue.Enqueue(keyRoom);

        List<Room> visitedRooms = new List<Room>();
        visitedRooms.Add(keyRoom);

        while (queue.Count > 0)
        {
            Room currentRoom = queue.Dequeue();
            if (currentRoom == null)
                continue;

            if (currentRoom.Equals(startRoom))
                return true;

            foreach (Passage passage in currentRoom.PassageWays)
            {
                if (!visitedRooms.Contains(passage.NotCurrent(currentRoom)))
                {
                    if (!passage.Equals(restrictedPassage))
                    {
                        queue.Enqueue(passage.NotCurrent(currentRoom));
                        visitedRooms.Add(passage.NotCurrent(currentRoom));
                    }
                }
            }
        }

        return false;
    }	
    private KeyType[] GetAvailableKeys()
    {
        List<KeyType> keysTaken = new List<KeyType>();
		keysTaken.AddRange (Dungeon.Rooms.Where (r => r.HeldKey != KeyType.None).Select(r => r.HeldKey));

        List<KeyType> availableKeys = new List<KeyType>();
        availableKeys.AddRange(Enum.GetValues(typeof(KeyType)).Cast<KeyType>());

        keysTaken.ForEach(k => availableKeys.Remove(k));
        availableKeys.Remove(KeyType.None);

        return availableKeys.ToArray();
    }

    private MeshFilter[] GetBaseMeshFilters(Transform parent)
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.gameObject.layer == LayerMask.NameToLayer("Dungeon"))
            {
                meshFilters.AddRange(child.GetComponentsInChildren<MeshFilter>());
            }
            if (child.childCount > 0)
            {
                meshFilters.AddRange(GetBaseMeshFilters(child));
            }
        }

        return meshFilters.ToArray();
    }

    private void CreateDungeonCollider()
    {
        //Disable Useless Walls. All remaining connectors will have an opening left behind which will cause problems in the collider
        Dungeon.Rooms.ForEach(r => r.AvailableConnectors.ForEach(c => c.Opening.SetActive(false)));

        MeshFilter[] meshFilters = GetBaseMeshFilters(Dungeon.transform);
        List<CombineInstance> combine = new List<CombineInstance>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            if(meshFilters[i].gameObject.activeSelf)
            {
                CombineInstance c = new CombineInstance();
                c.mesh = meshFilters[i].sharedMesh;
                c.transform = meshFilters[i].transform.localToWorldMatrix;
                combine.Add(c);
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine.ToArray());

        MeshCollider dungeonCollider = Dungeon.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        dungeonCollider.sharedMesh = combinedMesh;     
    }

    private GameObject CreateSkeleton(string roomName, Mesh combinedMesh)
    {
        GameObject skeleton = new GameObject(roomName + "Skeleton");
        skeleton.AddComponent(typeof(MeshSkeleton));
        MeshCollider skeletonCollider = skeleton.AddComponent(typeof(MeshCollider)) as MeshCollider;
        skeletonCollider.sharedMesh = combinedMesh;
        skeletonCollider.convex = true;
        skeletonCollider.isTrigger = true;
        Rigidbody rb = skeleton.AddComponent(typeof(Rigidbody)) as Rigidbody;
        rb.useGravity = false;
        rb.isKinematic = true;    

        return skeleton;
    }


    void OnDrawGizmos()
    {
        List<Passage> lockedPassages = new List<Passage>();
        Dungeon.Rooms.ForEach(r => lockedPassages.AddRange(r.GetDoors(true)));

        foreach (Passage passage in lockedPassages)
        {
            string colorName = Enum.GetName(typeof(KeyType), passage.KeyType);
            Material keyMat = dungeonLibrary.KeyMaterials.Where(m => m.name == "Key" + colorName).First();
            Gizmos.color = keyMat.color;
            Gizmos.DrawCube(passage.ConnectionPosition, new Vector3(4.0f, 4.0f, 4.0f));
            Vector3 keyRoomPosition = Dungeon.GetRoomWithKey(passage.KeyType).transform.position;
            Gizmos.DrawCube(keyRoomPosition, new Vector3(4.0f, 4.0f, 4.0f));
        }
    }

}
