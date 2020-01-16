using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public DungeonSize DungeonSize { get; set; }
    public bool HideRoofs { get; set; }

    private static GameSettings _instance;
    public static GameSettings Instance { get { return _instance; } }

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

        DontDestroyOnLoad(this);
    }
}
