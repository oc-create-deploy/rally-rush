using UnityEngine;
using TMPro;
using _Scripts.Core;
using _Scripts.Controllers;

// A readable, single-use breakaway post. Contact costs exactly one real runner.
public class RallyCourseHazard : MonoBehaviour
{
    public bool spent;
    public int hazardBefore,hazardAfter;
    private Material paint;
    private TextMeshPro label;
    private Transform post;
    private void Start()
    {
        paint=new Material(FindObjectOfType<ArmyController>().RunnerMaterial);
        paint.color=new Color(1f,.35f,.08f);
        var body=GameObject.CreatePrimitive(PrimitiveType.Capsule);body.name="Candy bumper -1";
        post=body.transform;post.SetParent(transform,false);post.localPosition=new Vector3(0,.45f,0);post.localScale=new Vector3(.65f,1.4f,.65f);
        body.GetComponent<Collider>().enabled=false;Destroy(body.GetComponent<Collider>());
        body.GetComponent<Renderer>().sharedMaterial=paint;
        // Soft white studs echo the icon's toy obstacles, within trigger width.
        for(int i=0;i<3;i++)
        {
            var stud=GameObject.CreatePrimitive(PrimitiveType.Sphere);stud.name="Bumper stud";
            stud.transform.SetParent(post,false);stud.transform.localPosition=new Vector3(0,-.3f+i*.3f,-.45f);
            stud.transform.localScale=Vector3.one*.22f;
            stud.GetComponent<Collider>().enabled=false;Destroy(stud.GetComponent<Collider>());
        }
        var textObject=new GameObject("Hazard cost");textObject.transform.SetParent(transform,false);textObject.transform.localPosition=new Vector3(0,1.5f,0);
        label=textObject.AddComponent<TextMeshPro>();label.font=Resources.Load<TMP_FontAsset>("RallyFont");label.fontSize=3.5f;
        label.rectTransform.sizeDelta=new Vector2(3,1);label.alignment=TextAlignmentOptions.Center;label.text="BUMPER -1";label.color=Color.white;
    }
    private void Update()
    {
        if(label!=null && Camera.main!=null)label.transform.rotation=Camera.main.transform.rotation;
        if(spent && post!=null)post.localRotation=Quaternion.RotateTowards(post.localRotation,Quaternion.Euler(0,0,80),Time.deltaTime*180);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(spent || !other.CompareTag("Cat") || GameFlowManager.Instance.state!=GameState.Game)return;
        var formation=other.GetComponentInParent<RadialFormation>();if(formation==null)return;
        spent=true;GetComponent<Collider>().enabled=false;
        hazardBefore=formation.amount;hazardAfter=Mathf.Max(0,hazardBefore-1);formation.amount=hazardAfter;
        GameFlowManager.Instance.SetPlayerCount(hazardAfter);UIManager.Instance.SetPlayerCountText(hazardAfter);
        label.text="-1 RUNNER";paint.color=new Color(.4f,.45f,.5f);
        if(hazardAfter==0)GameFlowManager.Instance.UpdateGameState(GameState.Lose);
    }
    private void OnDestroy(){if(paint!=null)Destroy(paint);}
}
