using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PauseManager PauseManager { get; private set; }
    public UIController UIController { get; private set; }

    [Header("Game Objects")]
    [SerializeField] private GameObject walkingArea;
    [SerializeField] private Player playerObject;

    [Header("Item Prefabs")]
    [SerializeField] private GameObject boosterPrefab;
    [SerializeField] private GameObject damagePrefab;
    [SerializeField] private GameObject notePrefab;

    [Header("Item Settings")]
    [SerializeField, Range(1, 10)] private int boosterMaxAmount = 9;                // max booster items amount
    [SerializeField, Range(1f, 10f)] private float boosterRespawnTime = 3f;         // booster item time respawning
    [Header("")]
    [SerializeField, Range(1, 10)] private int damageMaxAmount = 9;                 // max damage items amount
    [SerializeField, Range(1f, 10f)] private float damageRespawnTime = 4f;          // damage item time respawning
    [Header("")]
    [SerializeField, Range(1, 10)] private int noteMaxAmount = 4;                   // max note items amount
    [SerializeField, Range(1f, 10f)] private float noteRespawnTime = 5f;            // note item time respawnin

    private List<GameObject> boosterItems = new List<GameObject>();
    private List<GameObject> damageItems = new List<GameObject>();
    private List<GameObject> noteItems = new List<GameObject>();

    public Dictionary<string, float> WalkingAreaBounds {  get; private set; }       // bounds of walking area for player

    private bool isNoteShowing = false;


    private void Awake()
    {
        Instance = this;

        WalkingAreaBounds = new Dictionary<string, float>() {
            { "min_x", walkingArea.transform.position.x - walkingArea.transform.localScale.x / 2f * 10f },
            { "max_x", walkingArea.transform.position.x + walkingArea.transform.localScale.x / 2f * 10f },
            { "min_z", walkingArea.transform.position.z - walkingArea.transform.localScale.z / 2f * 10f },
            { "max_z", walkingArea.transform.position.z + walkingArea.transform.localScale.z / 2f * 10f }
        };

        // Place player's feet on ground
        Transform playerFeet = playerObject.transform.GetChild(0).transform;
        playerObject.transform.position = new Vector3(
            walkingArea.transform.position.x,
            walkingArea.transform.position.y - playerFeet.localPosition.y,
            walkingArea.transform.position.z
        );

        PauseManager = GetComponent<PauseManager>();
        UIController = GetComponent<UIController>();
    }


    private void Start()
    {
        InitItems();
    }


    private void Update()
    {
        UIController.UpdateTextBooster(playerObject.BoosterTime);

        // Pause/Unpause game on 'esq' button pressed if note isn't showing
        if (!isNoteShowing && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PauseManager.IsPaused)
            {
                OnPauseButtonClick();
            }
            else
            {
                OnGameUnpaused();
            }
        }
    }


    // Called when pause button is clicked
    public void OnPauseButtonClick()
    {
        UIController.ShowMenuPanel(true);
        OnGamePaused();
    }

    // Called when cross button on note is clicked
    public void OnCrossButtonClick()
    {
        isNoteShowing = false;
        UIController.ShowNoteWithText(false, "");
        OnGameUnpaused();
    }

    // Called when history button in menu panel is clicked
    public void OnHistoryButtonClick()
    {
        OnGameUnpaused();
        OnToShowNote(MyLogger.Get());
    }


    public void OnGamePaused()
    {
        UIController.SwitchSticksEnabled(false);
        PauseManager.Pause();
    }

    public void OnGameUnpaused()
    {
        UIController.SwitchSticksEnabled(true);
        UIController.ShowMenuPanel(false);
        PauseManager.UnPause();
    }


    // Items initialization on game (or scene) started
    private void InitItems()
    {
        // Initialization of booster items
        for (int i = 0; i < boosterMaxAmount; i++)
        {
            InstatiateItem(boosterPrefab, boosterItems);
        }

        // Initialization of damage items
        for (int i = 0; i < damageMaxAmount; i++)
        {
            InstatiateItem(damagePrefab, damageItems);
        }

        // Initialization of note items
        for (int i = 0; i < noteMaxAmount; i++)
        {
            InstatiateItem(notePrefab, noteItems);
        }
    }

    // Instantiation of given item prefab
    private void InstatiateItem(GameObject prefab, List<GameObject> itemList)
    {
        float minX = WalkingAreaBounds["min_x"];
        float maxX = WalkingAreaBounds["max_x"];
        float minZ = WalkingAreaBounds["min_z"];
        float maxZ = WalkingAreaBounds["max_z"];
        float x, z;

        // Randoming item coords
        do
        {
            x = UnityEngine.Random.Range(minX, maxX);
            z = UnityEngine.Random.Range(minZ, maxZ);

        } while (!CanPlaceItem(x, z, 5f, 3f));

        // Creating item and adding it to its list
        Vector3 pos = new Vector3(x, prefab.transform.position.y, z);
        GameObject item = Instantiate(prefab);
        item.transform.position = pos;
        itemList.Add(item);
    }

    // Checking if can place item on given coords
    private bool CanPlaceItem(float x, float z, float toPlayerDist, float toItemDist)
    {
        // Checking distance to player
        if (Vector2.Distance(new Vector2(x, z), new Vector2(playerObject.transform.position.x, playerObject.transform.position.z)) < toPlayerDist)
        {
            return false;
        }

        // Checking distance to booster items
        foreach (var item in boosterItems)
        {
            if (Vector2.Distance(new Vector2(x, z), new Vector2(item.transform.position.x, item.transform.position.z)) < toItemDist)
            {
                return false;
            }
        }

        // Checking distance to damage items
        foreach (var item in damageItems)
        {
            if (Vector2.Distance(new Vector2(x, z), new Vector2(item.transform.position.x, item.transform.position.z)) < toItemDist)
            {
                return false;
            }
        }

        // Checking distance to note items
        foreach (var item in noteItems)
        {
            if (Vector2.Distance(new Vector2(x, z), new Vector2(item.transform.position.x, item.transform.position.z)) < toItemDist)
            {
                return false;
            }
        }

        return true;
    }

    // Removing given item from its list
    // called when item is picked up
    public void PopItemFromList(GameObject obj)
    {
        // Coroutine for spawning item after its respawn time
        IEnumerator Wrapper(GameObject prefab, List<GameObject> itemList)
        {
            float time = 0f;
            if (prefab == boosterPrefab)
                time = boosterRespawnTime;
            if (prefab == damagePrefab)
                time = damageRespawnTime;
            if (prefab == notePrefab)
                time = noteRespawnTime;

            yield return new WaitForSeconds(time);
            InstatiateItem(prefab, itemList);
        }

        // Removing from boosters
        if (boosterItems.Remove(obj))
        {
            StartCoroutine(Wrapper(boosterPrefab, boosterItems));
            return;
        }

        // Removing from damages
        if (damageItems.Remove(obj))
        {
            StartCoroutine(Wrapper(damagePrefab, damageItems));
            return;
        }

        // Removing from notes
        if (noteItems.Remove(obj))
        {
            StartCoroutine(Wrapper(notePrefab, noteItems));
            return;
        }
    }


    // Called when player HP is <= 0
    public IEnumerator OnPlayerDeath()
    {
        MyLogger.Log("Player died");

        playerObject.enabled = false;
        yield return UIController.TextDeathEffect();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Called when player took note item or clicked on history button
    public void OnToShowNote(string noteText)
    {
        isNoteShowing = true;
        UIController.ShowNoteWithText(true, noteText);
        OnGamePaused();
    }
}
