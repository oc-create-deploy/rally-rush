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
    private Material courseAccent;
    private void OnDestroy(){if(courseAccent!=null)Destroy(courseAccent);}
    public List<GameObject> levels = new List<GameObject>();
    public void Awake()
    {
        if (levels.Count == 0) { Debug.LogError("Rally Rush has no configured levels."); return; }
        currentLevel = Mathf.Max(0,PlayerPrefs.GetInt("level",0));
        InitMap();
    }

    private void InitMap()
    {
        int stage=currentLevel%7;
        var course=Instantiate(levels[Mathf.Min(stage%3,levels.Count-1)]);
        var gates=course.GetComponentsInChildren<Corridor>().OrderBy(g=>g.transform.position.z).ThenBy(g=>g.transform.position.x).ToArray();
        // Three authored arithmetic routes on the retained physical course.
        // Stage0: build then multiply;1: compare add/multiply;2: manage division.
        string[][] plans={new[]{"+20","-10","/3","x4"},new[]{"+12","x2","-15","x3"},new[]{"x3","+8","/2","+15"},new[]{"+20","-10","/3","x4"},new[]{"+16","x2","x3","+24"},new[]{"x2","+30","/2","x3"},new[]{"+25","x3","x4","-20"}};
        for(int i=0;i<gates.Length && i<4;i++)
        {
            var g=gates[i];string op=plans[stage][i];int n=int.Parse(op.Substring(1));
            g._corridorType=op[0]=='+' ? Constants.CorridorTypes.Increase : op[0]=='-' ? Constants.CorridorTypes.Decrease : op[0]=='x' ? Constants.CorridorTypes.Multiply : Constants.CorridorTypes.Divide;
            g.increaseAmount=g.decreaseAmount=g.multiplyAmount=g.divideAmount=n;
        }
        // Later levels change decision spacing as well as arithmetic; the opening
        // remains generous, then the player must switch lanes before the clash.
        if(stage>=4)for(int i=0;i<gates.Length && i<4;i++)
        { var position=gates[i].transform.position;position.z=(i<2 ? 26f+(stage-4)*3 : 54f+(stage-4)*2);gates[i].transform.position=position; }
        Constants.SPEED_COEFFICIENT=3.5f+Mathf.Min(stage,4)*.25f;
        var rivals=new GameObject("Rival crowd encounter");
        rivals.transform.SetParent(course.transform,false);rivals.transform.position=new Vector3(.45f,2f,82f);
        var trigger=rivals.AddComponent<BoxCollider>();trigger.isTrigger=true;trigger.size=new Vector3(7,3,.6f);
        rivals.AddComponent<_Scripts.Controllers.MiniBattleController>().corridorEnemyCount=stage==3 ? 6 : 6+stage*6;
        var hazard=new GameObject("Single-use breakaway post");hazard.transform.SetParent(course.transform,false);
        hazard.transform.position=new Vector3(1.6f,2.1f,69);
        var hazardCollider=hazard.AddComponent<BoxCollider>();hazardCollider.isTrigger=true;hazardCollider.size=new Vector3(1.1f,2,.6f);
        hazard.AddComponent<RallyCourseHazard>();
        course.AddComponent<RallyLogoWorld>().Build(gates,stage);
        courseAccent=new Material(FindObjectOfType<_Scripts.Controllers.ArmyController>().RunnerMaterial);
        courseAccent.color=stage==0 ? new Color(.05f,.7f,.8f) : stage==1 ? new Color(1f,.65f,.1f) : new Color(.65f,.3f,.85f);
        for(int i=0;i<10;i++)for(int side=0;side<2;side++)
        {
            var marker=GameObject.CreatePrimitive(PrimitiveType.Cube);marker.name="Rally course marker";
            marker.transform.SetParent(course.transform,false);marker.transform.position=new Vector3(side==0 ? -3.1f : 4f,2.5f,8+i*12);
            marker.transform.localScale=new Vector3(.15f,1.3f,.3f);
            marker.GetComponent<Collider>().enabled=false;Destroy(marker.GetComponent<Collider>());
            marker.GetComponent<Renderer>().sharedMaterial=courseAccent;
        }
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
