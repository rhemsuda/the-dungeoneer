using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public GameObject NowLoadingPanel { get; private set; }
    public GameObject PlayingPanel { get; private set; }

    public Text InteractText { get; private set; }
    public Image[] KeySlots = new Image[10];
    public List<Image> PotionHolders = new List<Image>();

    public Sprite EmptyPotionSprite { get; private set; }
    public Sprite FullPotionSprite { get; private set; }

  
    private static HUD _instance;
    public static HUD Instance { get { return _instance; } }

    void Start()
    {
        this.NowLoadingPanel = GameObject.Find("NowLoadingPanel");
        this.PlayingPanel = GameObject.Find("PlayingPanel");
        this.InteractText = GameObject.Find("InteractText").GetComponent<Text>();

        for(int i = 0; i < 10; i++)
        {
            string slotName = "KeySlot" + (i + 1);
            KeySlots[i] = PlayingPanel.transform.Find("KeyPanel/KeySlots/" + slotName).GetComponent<Image>();
        }

        foreach(GameObject go in GameObject.Find("PotionHolders").transform.FindGameObjectsByChildName("PotionHolder"))
        {
            Image i = go.GetComponent<Image>();
            PotionHolders.Add(i);
        }

        this.EmptyPotionSprite = Resources.Load<Sprite>("UI/potion_empty");
        this.FullPotionSprite = Resources.Load<Sprite>("UI/potion_full");

        this.PlayingPanel.SetActive(false);
        this.UpdateKeySlots();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void SpawnInteractError(string message, float displayTime = 2.0f)
    {
        if (PlayingPanel.transform.FindChildInChildren("InteractError") == null)
        {
            GameObject messageObject = Instantiate(Resources.Load<GameObject>("InteractError"), PlayingPanel.transform.Find("InteractErrorHolder"));
            messageObject.transform.localPosition = Vector3.zero;
            messageObject.name = "InteractError";

            PopupMessage popup = messageObject.GetComponent<PopupMessage>();
            popup.SetMessage(message);
            popup.SetDisplayTime(displayTime);
            popup.Display();
        }
    }

    public void UpdateKeySlots(KeyType[] keys = null)
    {
        for(int i = 0; i < KeySlots.Length; i++)
        {
            Image slot = KeySlots[i];
            slot.sprite = null;
            slot.enabled = false;

            if(keys != null && i < keys.Length && keys[i] != KeyType.None)
            {
                KeyType key = keys[i];
                string colorName = Enum.GetName(typeof(KeyType), key);
                Sprite keySprite = DungeonLibrary.Instance.KeySprites.Where(s => s.name == "Key" + colorName).First();
                slot.sprite = keySprite;
                slot.enabled = true;
            }
        }
    }
}
