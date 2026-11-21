using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Core;
using DG.Tweening;
using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Scripts.Controllers
{
    public class BattleController : MonoBehaviour
    {
        private bool didLost = false;
        private bool didWon = false;
        public bool canStart = false;
        private bool canEject = false;

        private float playerBarAmount = 0.5f;
        private float enemyBarAmount = 1F;
       
        public Image enemyBar;
        public Image playerBar;
        public Image playerBar2;
        public RectTransform middleBar;
        
        public GameObject battleUI;
        private Boss bossComponent;
        
        public float increaseAmount;

        public Camera mainCamera;
        private CameraController camController;
        private Transform cameraTransform;
        
        private void Awake()
        {
            camController = mainCamera.GetComponent<CameraController>();
            cameraTransform = mainCamera.GetComponent<Transform>();
        }

        private void Start()
        {
            bossComponent = FindObjectOfType<Boss>();
        }

        private readonly List<Material> relayMaterials=new List<Material>();
        private void OnDestroy(){foreach(var material in relayMaterials)if(material!=null)Destroy(material);}
        public int finaleCrowd,finaleMultiplier,finaleReward;
        public string finaleStage="NotStarted";
        public void Battle()
        {
            if(finaleStage!="NotStarted")return;
            StartCoroutine(CrowdRelayFinale());
        }
        private IEnumerator CrowdRelayFinale()
        {
            var army=FindObjectOfType<ArmyController>();
            var formation=army.GetComponent<RadialFormation>();
            finaleCrowd=formation.amount;
            finaleMultiplier=Mathf.Clamp(1+finaleCrowd/20,1,5);
            finaleReward=finaleCrowd*finaleMultiplier;
            finaleStage="CrowdRelay";
            canStart=false;canEject=false;
            if(battleUI!=null)battleUI.SetActive(false);
            var boss=FindObjectOfType<Boss>();
            if(boss!=null)
            {
                foreach(var r in boss.GetComponentsInChildren<Renderer>())r.enabled=false;
                foreach(var c in boss.GetComponentsInChildren<Collider>())c.enabled=false;
            }
            camController.lockCamera=false;camController.target=army.transform;
            mainCamera.transform.DORotate(new Vector3(40,0,0),.5f);
            float startZ=army.transform.position.z;
            var basis=army.RunnerMaterial;
            for(int i=1;i<=5;i++)
            {
                var step=GameObject.CreatePrimitive(PrimitiveType.Cube);step.name="Relay reward step x"+i;
                step.transform.position=new Vector3(.45f,.5f+i*.10f,startZ+3+i*1.5f);
                step.transform.localScale=new Vector3(6,.2f+i*.2f,1.4f);
                step.GetComponent<Collider>().enabled=false;
                var material=basis!=null ? new Material(basis) : new Material(Shader.Find("Standard"));
                relayMaterials.Add(material);
                material.color=i<=finaleMultiplier ? new Color(.12f,.75f,.58f) : new Color(.25f,.33f,.45f);
                step.GetComponent<Renderer>().sharedMaterial=material;
                var label=new GameObject("Relay multiplier").AddComponent<TextMeshPro>();
                label.transform.SetParent(step.transform,false);
                label.transform.position=step.transform.position+Vector3.up*(.5f+i*.1f);
                label.transform.rotation=mainCamera.transform.rotation;
                label.transform.localScale=new Vector3(1/6f,1/(.2f+i*.2f),1/1.4f);
                label.font=Resources.Load<TMP_FontAsset>("RallyFont");label.fontSize=5;label.alignment=TextAlignmentOptions.Center;
                label.text="x"+i;label.color=Color.white;
            }
            var banner=new GameObject("Crowd relay reward").AddComponent<TextMeshPro>();
            banner.font=Resources.Load<TMP_FontAsset>("RallyFont");banner.fontSize=4;banner.alignment=TextAlignmentOptions.Center;
            banner.rectTransform.sizeDelta=new Vector2(7,2);
            banner.transform.position=new Vector3(.45f,4.5f,startZ+7);
            banner.transform.rotation=mainCamera.transform.rotation;
            banner.text=finaleCrowd+" RUNNERS x"+finaleMultiplier+" = "+finaleReward;
            army.transform.DOMove(new Vector3(.45f,army.transform.position.y,startZ+3+finaleMultiplier*1.5f),4);
            yield return new WaitForSeconds(5);
            finaleStage="Complete";
            AudioManager.Instance.StopFightSound();
            ScoreManager.Instance.EndCrowdRelayScore(finaleReward);
            GameFlowManager.Instance.UpdateGameState(GameState.Win);
        }

        private void Update()
        {
            if (canEject)
            {
                var ball = bossComponent.catBall;
                var body = ball.GetComponent<Rigidbody>();
                // Crossing the final score marker completes the roll even if friction
                // never produces an exactly-zero velocity on the current platform.
                if (body.velocity.sqrMagnitude < .0025f || ball.transform.position.z >= ScoreManager.Instance.point100Location.position.z)
                {
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                    ScoreManager.Instance.EndGamePopupScore(ball.gameObject);
                    GameFlowManager.Instance.UpdateGameState(GameState.Win);
                    canEject = false;
                }
            }
            
            if (!canStart) return;
            if (didLost) return;
            if (didWon) return;

            playerBarAmount -= Time.deltaTime * increaseAmount;
            enemyBarAmount += Time.deltaTime * increaseAmount * 2F;

            middleBar.anchoredPosition = new Vector2((playerBar2.fillAmount - 0.5F) * 820F, middleBar.anchoredPosition.y); 
            
            playerBar.fillAmount = playerBarAmount * 2F;
            playerBar2.fillAmount = playerBarAmount;
            enemyBar.fillAmount = enemyBarAmount;
            
            if (playerBarAmount  <= 0)
            {
                didLost = true;
                print("PLAYER LOST THE BATTLE");
                UIManager.Instance.ClosePopup(battleUI);
                GameFlowManager.Instance.UpdateGameState(GameState.Lose);
            }
            if (playerBarAmount >= 1)
            {
                CloseBattleScene();
            }
            
        }
        
        public void Decrease()
        {
            playerBarAmount += increaseAmount;
            enemyBarAmount -= increaseAmount * 2F;
            middleBar.anchoredPosition = new Vector2((playerBar2.fillAmount - 0.5F) * 820F, middleBar.anchoredPosition.y); 
            
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
        }

        private void OpenBattleScene()
        {

            camController.lockCamera = true;
            canStart = true;
            UIManager.Instance.ActivatePopup(battleUI);
            print("OPEN BATTLE SCENE");
        }

        private void CloseBattleScene()
        {
            AudioManager.Instance.StopFightSound();
            didWon = true;
            canStart = false;
            UIManager.Instance.ClosePopup(battleUI);
             bossComponent.EjectCatBallToLevelEnd();
             StartCoroutine(UpdateCanEjectBool());

        }
        private IEnumerator UpdateCanEjectBool()
        {
            yield return new WaitForSeconds(2);
            canEject = true;
        }
    }
}
