using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public int currentLevel;
    public List<GameObject> levels = new List<GameObject>();
    public void Awake()
    {
        if (levels.Count == 0) { Debug.LogError("Rally Rush has no configured levels."); return; }
        currentLevel = Mathf.Clamp(PlayerPrefs.GetInt("level"), 0, levels.Count - 1);
        InitMap();
    }

    private void InitMap()
    {
        Instantiate(levels[currentLevel]);
    }
    public void NextLevel()
    {
        currentLevel++;
        currentLevel = Mathf.Clamp(currentLevel, 0, levels.Count-1);
        PlayerPrefs.SetInt("level", currentLevel);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("LOAD NEXT LEVEL");
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}
