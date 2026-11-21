using System;
using _Scripts.Core;
using _Scripts.Models;
using _Scripts.Utilities;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace _Scripts.Controllers
{
    public class CorridorController : Singleton<CorridorController>
    {
        public int score  = 0 ;
        public void CorridorEffect(Corridor corridor, GameObject other)
        {
            var formation=other.GetComponentInParent<RadialFormation>();
            if(formation==null)return;
            if(corridor.GetCorridorType()==Constants.CorridorTypes.Divide && corridor.divideAmount<=0)
            { Debug.LogError("Rally gate has an invalid division operand.");return; }
            switch (corridor.GetCorridorType())
            {
                case Constants.CorridorTypes.Increase:
                    other.transform.parent.GetComponent<RadialFormation>().amount += corridor.increaseAmount;
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorIncreaseSound);
                    print("INCREASING ARMY AMOUNT");
                    break; 
                case Constants.CorridorTypes.Decrease:
                    other.transform.parent.GetComponent<RadialFormation>().amount -=  corridor.decreaseAmount;
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorMinusSound);

                    print("DECREASE ARMY AMOUNT");
                    break;
                case Constants.CorridorTypes.Multiply:
                    other.transform.parent.GetComponent<RadialFormation>().amount *= corridor. multiplyAmount;
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorIncreaseSound);
                    print("Multiply ARMY AMOUNT");
                    break;
                case Constants.CorridorTypes.Divide:
                    other.transform.parent.GetComponent<RadialFormation>().amount /= corridor. divideAmount;
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorMinusSound);
                    print("Divide ARMY AMOUNT");
                    break;
                default:
                    Debug.Log("TRIGGER EXCEPTION");
                    break;
            }
           
            formation.amount=Mathf.Max(0,formation.amount);
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

            score += other.transform.parent.GetComponent<RadialFormation>().amount;
            ScoreManager.Instance.upperScoreText.text = score.ToString();
            
            GameFlowManager.Instance.SetPlayerCount(other.transform.parent.GetComponent<RadialFormation>().amount);
            // Update UI
            UIManager.Instance.SetPlayerCountText(other.transform.parent.GetComponent<RadialFormation>().amount);
            // Game Over check
            if (other.transform.parent.GetComponent<RadialFormation>().amount <= 0)
            {
                GameFlowManager.Instance.UpdateGameState(GameState.Lose);
            }

        }

        
    }
}
