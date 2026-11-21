using System;
using System.Collections;
using _Scripts.Core;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace _Scripts.Controllers
{
    public class MiniBattleController : MonoBehaviour
    {
        public int corridorEnemyCount ;
        private WaitForSeconds _getWait;
        public ParticleSystem bossFightParticle;
        private GameObject createdParticleSystem;
        private bool engaged;
        private readonly List<GameObject> enemyRunners=new List<GameObject>();
        private TextMeshPro enemyLabel;
        private Material enemyKit;
        private Transform MakePart(Transform parent,string name,Vector3 at,Vector3 size,PrimitiveType type)
        {
            var part=GameObject.CreatePrimitive(type);part.name=name;part.transform.SetParent(parent,false);
            part.transform.localPosition=at;part.transform.localScale=size;
            part.GetComponent<Collider>().enabled=false;Destroy(part.GetComponent<Collider>());
            part.GetComponent<Renderer>().sharedMaterial=enemyKit;return part.transform;
        }
        private void Update()
        {
            while(enemyRunners.Count>Mathf.Max(0,corridorEnemyCount))
            {var unit=enemyRunners[enemyRunners.Count-1];enemyRunners.RemoveAt(enemyRunners.Count-1);Destroy(unit);}
            for(int i=0;i<enemyRunners.Count;i++)
                enemyRunners[i].transform.localRotation=Quaternion.Euler(Mathf.Sin(Time.time*9+i)*8,180,0);
            if(enemyLabel!=null && Camera.main!=null)
            {enemyLabel.text="RIVALS " + Mathf.Max(0,corridorEnemyCount);enemyLabel.transform.rotation=Camera.main.transform.rotation;}
        }
        
        private void Start()
        {
            _getWait = new WaitForSeconds(0.6F);
            var army=FindObjectOfType<ArmyController>();
            enemyKit=army!=null && army.RunnerMaterial!=null ? new Material(army.RunnerMaterial) : new Material(Shader.Find("Standard"));
            enemyKit.color=new Color(.93f,.16f,.22f);
            for(int i=0;i<corridorEnemyCount;i++)
            {
                var unit=new GameObject("Original rival runner "+i);unit.transform.SetParent(transform,false);
                unit.transform.localPosition=new Vector3((i%5-2)*.55f,0,2+(i/5)*.6f);
                MakePart(unit.transform,"Vest",new Vector3(0,.55f,0),new Vector3(.36f,.45f,.25f),PrimitiveType.Cube);
                MakePart(unit.transform,"Head",new Vector3(0,.93f,0),Vector3.one*.27f,PrimitiveType.Sphere);
                MakePart(unit.transform,"Left leg",new Vector3(-.1f,.17f,0),new Vector3(.12f,.4f,.15f),PrimitiveType.Cube);
                MakePart(unit.transform,"Right leg",new Vector3(.1f,.17f,0),new Vector3(.12f,.4f,.15f),PrimitiveType.Cube);
                MakePart(unit.transform,"Left arm",new Vector3(-.26f,.55f,0),new Vector3(.12f,.4f,.15f),PrimitiveType.Cube);
                MakePart(unit.transform,"Right arm",new Vector3(.26f,.55f,0),new Vector3(.12f,.4f,.15f),PrimitiveType.Cube);
                enemyRunners.Add(unit);
            }
            var label=new GameObject("Rival count");label.transform.SetParent(transform,false);label.transform.localPosition=new Vector3(0,1.7f,2);
            enemyLabel=label.AddComponent<TextMeshPro>();enemyLabel.font=Resources.Load<TMP_FontAsset>("RallyFont");
            enemyLabel.fontSize=5;enemyLabel.alignment=TextAlignmentOptions.Center;enemyLabel.color=Color.white;
            enemyLabel.rectTransform.sizeDelta=new Vector2(4,1);
        }

        private void OnDestroy(){if(enemyKit!=null)Destroy(enemyKit);}

        private void OnTriggerEnter(Collider other)
        {
            if(engaged || !other.CompareTag("Cat") || GameFlowManager.Instance.state!=GameState.Game)return;
            var formation=other.GetComponentInParent<RadialFormation>();
            if(formation==null)return;
            engaged=true;
            AudioManager.Instance.PlayFightSound(AudioManager.Instance.cartoonFightSound );
            this.GetComponent<Collider>().enabled = false;
            GameFlowManager.Instance.UpdateGameState(GameState.MiniBattle);
            if(bossFightParticle!=null)
            {
                createdParticleSystem = Instantiate(bossFightParticle).gameObject;
                createdParticleSystem.transform.localScale = new Vector3(3, 3, 3);
                createdParticleSystem.transform.position = this.transform.position;
            }
            StartCoroutine(DecreaseArmyOverTime(formation));
        }

        private IEnumerator DecreaseArmyOverTime(RadialFormation otherRadialFormation)
        {
            while (otherRadialFormation.amount > 0 && corridorEnemyCount > 0)
            {
                corridorEnemyCount -= 1;
                otherRadialFormation.amount  -= 1;
                print("enemy count is "  + corridorEnemyCount +"and player amount is " + otherRadialFormation.amount );

                yield return _getWait;
                while(GameFlowManager.Instance.state==GameState.Pause)yield return null;
                

            }
            // Includes a zero-enemy configuration without leaving the run stuck.
            AudioManager.Instance.StopFightSound();
            GameFlowManager.Instance.UpdateGameState(otherRadialFormation.amount>0 ? GameState.Game : GameState.Lose);
            Destroy(createdParticleSystem);
            gameObject.SetActive(false);
        }
    }
}
