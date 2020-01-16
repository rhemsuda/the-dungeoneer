using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.MusicAudioSource.clip = SoundManager.Instance.MenuMusic;
        SoundManager.Instance.MusicAudioSource.Play();
    }

    public void Load()
    {
        StartCoroutine("LoadGameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetDungeonSize(int size)
    {
        GameSettings.Instance.DungeonSize = (size == 0) ? DungeonSize.Small : (size == 1) ? DungeonSize.Medium : DungeonSize.Large;
    }

    public void ToggleHideRoofs()
    {
        Toggle t = GameObject.Find("HideRoofs").GetComponent<Toggle>();
        GameSettings.Instance.HideRoofs = t.isOn;
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
            yield return null;

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
            yield return null;
    }

    public void PlayMenuClickSound()
    {
        SoundManager.Instance.SoundEffectsAudioSource.clip = SoundManager.Instance.ButtonClickClip;
        SoundManager.Instance.SoundEffectsAudioSource.Play();
    }
}
