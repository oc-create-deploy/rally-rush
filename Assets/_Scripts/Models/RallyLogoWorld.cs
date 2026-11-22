using System.Collections.Generic;
using UnityEngine;
using _Scripts.Models;
using _Scripts.Controllers;

// Original logo-inspired set dressing. Never changes collision or gate arithmetic.
public class RallyLogoWorld : MonoBehaviour
{
    readonly List<Material> paints=new List<Material>();
    Material violet,white,gold,cyan,green;
    readonly List<Renderer> finishRenderers=new List<Renderer>();
    ArmyController finishArmy;
    float finishZ;
    Material Paint(Color color)
    {
        var material=new Material(FindObjectOfType<ArmyController>().RunnerMaterial){color=color};
        if(material.HasProperty("_Glossiness"))material.SetFloat("_Glossiness",.4f);
        paints.Add(material);return material;
    }
    Transform Shape(string name,Vector3 at,Vector3 size,Material paint,PrimitiveType type=PrimitiveType.Cube)
    {
        var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(transform,false);
        go.transform.position=at;go.transform.localScale=size;
        var collider=go.GetComponent<Collider>();collider.enabled=false;Destroy(collider);
        go.GetComponent<Renderer>().sharedMaterial=paint;return go.transform;
    }
    public void Build(Corridor[] gates,int stage)
    {
        violet=Paint(new Color(.49f,.18f,.94f));white=Paint(new Color(.96f,.97f,1));
        gold=Paint(new Color(1,.72f,.035f));cyan=Paint(new Color(.04f,.64f,1));green=Paint(new Color(.3f,.94f,.06f));
        // Keep physical authored deck and walls; repaint instead of masking colliders.
        foreach(var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            if(renderer.name=="Ground" || renderer.name=="LevelEnd1to100")renderer.sharedMaterial=white;
            else if(renderer.name.StartsWith("Wall"))renderer.sharedMaterial=violet;
        }
        var camera=Camera.main;
        if(camera!=null){camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.24f,.73f,1);}
        RenderSettings.ambientLight=new Color(.72f,.8f,1);
        RenderSettings.fog=true;RenderSettings.fogColor=new Color(.35f,.75f,1);
        RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogStartDistance=65;RenderSettings.fogEndDistance=180;
        // Chunky portal frames are aligned to the ACTUAL gate trigger bounds.
        foreach(var gate in gates)
        {
            var bounds=gate.GetComponent<Collider>().bounds;
            var paint=gate.transform.position.x<.45f ? green : cyan;
            if(gate.meshRenderer!=null)gate.meshRenderer.sharedMaterial=paint;
            float y=bounds.center.y;
            foreach(float x in new[]{bounds.min.x,bounds.max.x})
                Shape("Candy gate upright",new Vector3(x,y,bounds.center.z),new Vector3(.14f,bounds.size.y+.3f,.2f),paint);
            // No opaque crossbar: native capture showed it cutting through math labels.
        }
        // Outside the steering envelope: stepped island silhouettes and waterfalls.
        for(int i=0;i<14;i++)for(int side=0;side<2;side++)
        {
            float x=side==0 ? -15-(i%3)*5 : 16+(i%3)*5;
            float z=10+i*14;
            float height=5+(i*7%9);
            Shape("Floating violet island",new Vector3(x,-height*.5f,z),new Vector3(5,height,7),i%2==0?violet:cyan);
            Shape("Island snow cap",new Vector3(x,.08f,z),new Vector3(5.2f,.3f,7.2f),white);
            Shape("Waterfall ribbon",new Vector3(x+2.6f,-6,z),new Vector3(.12f,12,2),cyan);
            if(i%3==0)for(int c=0;c<3;c++)Shape("Cloud puff",new Vector3(x+c*1.5f,8+(i%4),z+5),new Vector3(3,1.4f,2),white,PrimitiveType.Sphere);
        }
        var army=FindObjectOfType<ArmyController>();
        float finish=army.boss!=null ? army.boss.transform.position.z-5 : 115;
        finishArmy=army;finishZ=finish;
        int firstFinishChild=transform.childCount;
        // Wide crowned arch and checkered lintel: an unmistakable finish landmark.
        foreach(float x in new[]{-3.35f,4.25f})Shape("Golden finish pillar",new Vector3(x,4.2f,finish),new Vector3(.45f,4.5f,.55f),gold);
        for(int row=0;row<2;row++)for(int col=0;col<12;col++)
            Shape("Finish checker",new Vector3(-3.05f+col*.64f,6.2f+row*.4f,finish),new Vector3(.64f,.4f,.3f),(row+col)%2==0?white:violet);
        Shape("Crown band",new Vector3(.45f,7.1f,finish),new Vector3(1.7f,.4f,.6f),gold);
        for(int i=0;i<3;i++)
        {
            var point=Shape("Crown point",new Vector3(-.1f+i*.55f,7.55f,finish),new Vector3(.42f,.9f,.42f),gold);
            point.rotation=Quaternion.Euler(0,0,(i-1)*-18);
            Shape("Crown jewel",new Vector3(-.1f+i*.55f,8,finish),Vector3.one*.24f,gold,PrimitiveType.Sphere);
        }
        for(int i=firstFinishChild;i<transform.childCount;i++)
            finishRenderers.Add(transform.GetChild(i).GetComponent<Renderer>());
    }
    void LateUpdate()
    {
        // The high arch projects over the relay crowd at close range. Keep the
        // approach landmark, then remove only decoration before it masks play.
        if(finishArmy==null)return;
        bool visible=finishArmy.transform.position.z < finishZ-18f;
        foreach(var renderer in finishRenderers)if(renderer!=null)renderer.enabled=visible;
    }
    void OnDestroy(){foreach(var paint in paints)if(paint!=null)Destroy(paint);}
}
