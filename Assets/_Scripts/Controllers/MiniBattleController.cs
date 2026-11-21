using System;
using System.Collections;
using _Scripts.Core;
using UnityEngine;

namespace _Scripts.Controllers
{
    public class MiniBattleController : MonoBehaviour
    {
        public int corridorEnemyCount ;
        private WaitForSeconds _getWait;
        public ParticleSystem bossFightParticle;
        private GameObject createdParticleSystem;
        private bool engaged;
        
        private void Start()
        {
            _getWait = new WaitForSeconds(0.2F);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(engaged || !other.CompareTag("Cat") || GameFlowManager.Instance.state!=GameState.Game)return;
            var formation=other.GetComponentInParent<RadialFormation>();
            if(formation==null)return;
            engaged=true;
            AudioManager.Instance.PlayFightSound(AudioManager.Instance.cartoonFightSound );
            this.GetComponent<Collider>().enabled = false;
            GameFlowManager.Instance.UpdateGameState(GameState.MiniBattle);
            createdParticleSystem = Instantiate(bossFightParticle).gameObject;
            createdParticleSystem.transform.localScale = new Vector3(3, 3, 3);
            createdParticleSystem.transform.position = this.transform.position;
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
