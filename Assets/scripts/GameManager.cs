using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    public static GameManager Instance { get; private set; }

    public int PlayerChoiceIndex { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Game Manager instantiated");
        }
        else
        {
            Destroy(gameObject);
            
        }
    }

    public void SaveChoice(int choiceIndex)
    {
        PlayerChoiceIndex = choiceIndex;
        Debug.Log($"[GameManager] Choice saved: {choiceIndex}");
        
        PlayerPrefs.SetInt("LastChoice", choiceIndex);
        PlayerPrefs.Save();
    }


    public void LoadChoice()
    {
        if (PlayerPrefs.HasKey("LastChoice"))
        {
            PlayerChoiceIndex = PlayerPrefs.GetInt("LastChoice");
            Debug.Log($"[GameManager] Choice loaded from PlayerPrefs: {PlayerChoiceIndex}");
        }
    }

    public void RestData()
    {
        PlayerChoiceIndex = -1;
        PlayerPrefs.DeleteKey("LastChoice");
    }
    
    public void LoadScene(string sceneName)
    {
        Debug.Log($"[GameManager] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    private void OnEnable()
    {
        ChoiceHandler.OnChoiceMade += HandleChoiceMade;
    }

    private void OnDisable()
    {
        ChoiceHandler.OnChoiceMade -= HandleChoiceMade;
    }

    private void HandleChoiceMade(int index)
    {
        SaveChoice(index);
    }
}
