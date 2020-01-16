using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DungeonCreator))]
public class GameManager : MonoBehaviour
{
    private bool isLoaded = false;

    Dungeon dungeon = null;

    IEnumerator Start()
    {
        DungeonCreator creator = this.GetComponent<DungeonCreator>();
        yield return StartCoroutine(creator.CreateDungeon(GameSettings.Instance.DungeonSize, d => dungeon = d));

        this.LoadGame();  
    }

    void LoadGame()
    {
        //Load player
        GameObject playerPrefab = Resources.Load("Player") as GameObject;
        Instantiate(playerPrefab, dungeon.SpawnPoint, Quaternion.identity);

        //Fill the dungeon with random monsters
        this.GetComponent<MonsterSpawner>().FillDungeon(dungeon);

        //Hide roofs if we choose to
        if (GameSettings.Instance.HideRoofs)
        {
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("Roof"))
            {
                go.SetActive(false);
            }
        }

        CameraController cameraController = Camera.main.GetComponent<CameraController>();

        SoundManager.Instance.MusicAudioSource.clip = SoundManager.Instance.GameMusic;
        SoundManager.Instance.MusicAudioSource.Play();

        HUD.Instance.PlayingPanel.SetActive(true);
        HUD.Instance.NowLoadingPanel.SetActive(false);
    }
}