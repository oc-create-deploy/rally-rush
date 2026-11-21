using System.Collections;
using System.Linq;
using _Scripts.Models;
using _Scripts.Utilities;
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
        currentLevel = Mathf.Max(0,PlayerPrefs.GetInt("level",0));
        InitMap();
    }

    private void InitMap()
    {
        int stage=currentLevel%3;
        var course=Instantiate(levels[Mathf.Min(stage,levels.Count-1)]);
        var gates=course.GetComponentsInChildren<Corridor>().OrderBy(g=>g.transform.position.z).ThenBy(g=>g.transform.position.x).ToArray();
        // Three authored arithmetic routes on the retained physical course.
        // Stage0: build then multiply;1: compare add/multiply;2: manage division.
        string[][] plans={new[]{"+20","-10","/3","x4"},new[]{"+12","x2","-15","x3"},new[]{"x3","+8","/2","+15"}};
        for(int i=0;i<gates.Length && i<4;i++)
        {
            var g=gates[i];string op=plans[stage][i];int n=int.Parse(op.Substring(1));
            g._corridorType=op[0]=='+' ? Constants.CorridorTypes.Increase : op[0]=='-' ? Constants.CorridorTypes.Decrease : op[0]=='x' ? Constants.CorridorTypes.Multiply : Constants.CorridorTypes.Divide;
            g.increaseAmount=g.decreaseAmount=g.multiplyAmount=g.divideAmount=n;
        }
        Constants.SPEED_COEFFICIENT=3.5f+stage*.35f;
    }
    public void NextLevel()
    {
        currentLevel++;
        // Advance the earned level; course variant cycles, not the saved progress.
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
