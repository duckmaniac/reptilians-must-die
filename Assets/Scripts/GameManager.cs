using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int currentLevelNumber;
    public int CurrentLevelNumber {
        get { return currentLevelNumber; }
        set {
            currentLevelNumber = value;
            Utility.SaveToMemory(Constants.SAVEFILE_CURRENT_LEVEL, currentLevelNumber);
        }
    }

    public List<CardData> PlayerDeckData { get; private set; }

    // all possible player cards
    public List<CardData> PlayerCardsData { get; private set; }

    // all possible reptilian cards
    public List<CardData> ReptiliansCardsData { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(InitGame());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator InitGame()
    {
        // load all available cards
        LoadCardsData();

        // load current level
        CurrentLevelNumber = Utility.LoadFromMemory<int>(Constants.SAVEFILE_CURRENT_LEVEL);
        if (Equals(CurrentLevelNumber, default(int))) CurrentLevelNumber = 1;

        // load player's deck
        PlayerDeckData = Utility.LoadFromMemory<List<CardData>>(Constants.SAVEFILE_DECK_DATA) ?? new List<CardData>();

        yield return null;
    }

    private void LoadCardsData()
    {
        PlayerCardsData = Utility.LoadJsonFromResources<List<CardData>>(Constants.RESOURCE_ORDER_CARDS);
        ReptiliansCardsData = Utility.LoadJsonFromResources<List<CardData>>(Constants.RESOURCE_REPTILIANS_CARDS);
    }

    public void SavePlayerDeck()
    {
        Utility.SaveToMemory(Constants.SAVEFILE_DECK_DATA, PlayerDeckData);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Level");
    }

    public void NextNews()
    {
        if (CurrentLevelNumber < 5)
        {
            CurrentLevelNumber += 1;
            SceneManager.LoadScene("News");
        } else
        {
            // грузим финал
        }

    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Level");
    }
}
