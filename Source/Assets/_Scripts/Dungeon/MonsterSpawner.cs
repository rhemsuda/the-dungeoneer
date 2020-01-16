using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    private GameObject[] monsterList;
    private GameObject boss;

	// Use this for initialization
	void Start ()
    {
        this.monsterList = Resources.LoadAll<GameObject>("Enemies");
        this.boss = Resources.Load<GameObject>("Boss/Dragon");
    }
	
    public void FillDungeon(Dungeon dungeon)
    {
        foreach (Room room in dungeon.Rooms.Skip(1))
        {
            GameObject[] spawnPoints = room.transform.FindGameObjectsByChildTag("MonsterSpawn").ToArray();

            int difficultyMaxSpawns = (dungeon.Size == DungeonSize.Small) ? 2 : (dungeon.Size == DungeonSize.Medium) ? 3 : 4;
            int maxSpawns = (room.RoomType == RoomType.Dungeon) ? difficultyMaxSpawns 
                : (room.RoomType == RoomType.Hall) ? 1  
                : (room.RoomType == RoomType.Boss) ? 1 
                : 0;
            int minSpawns = (room.RoomType == RoomType.Boss || room.RoomType == RoomType.Hall) ? 1 
                : (room.RoomType == RoomType.Dungeon) ? 2 
                : 0;

            int spawnCount = Random.Range(minSpawns, maxSpawns);
            for (int i = 0; i < spawnCount && i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] != null)
                {
                    GameObject monsterToSpawn;
                    if(room.RoomType == RoomType.Boss)
                    {
                        monsterToSpawn = boss;
                    }
                    else
                    {
                        int randomMonsterIndex = Random.Range(0, monsterList.Length);
                        monsterToSpawn = monsterList[randomMonsterIndex];
                    }
                    
                    GameObject go = Instantiate(monsterToSpawn, spawnPoints[i].transform.position, Quaternion.identity, dungeon.transform);
                    go.GetComponent<NavMeshAgent>().enabled = true;
                }
            }
        }
    }
}
