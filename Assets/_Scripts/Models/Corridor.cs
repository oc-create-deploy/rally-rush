using System.Collections;
using _Scripts.Controllers;
using _Scripts.Core;
using _Scripts.Utilities;
using DG.Tweening;
using UnityEngine;
using TMPro;

namespace _Scripts.Models
{
    public class Corridor : MonoBehaviour
    {
        public int increaseAmount;
        public int decreaseAmount;
        public int multiplyAmount;
        public int divideAmount;

        public Constants.CorridorTypes _corridorType;
        [HideInInspector] public MeshRenderer meshRenderer;
        public BoxCollider neighbourCorridorCollider;
        public bool consumed;
        public int appliedBefore, appliedAfter;

        private void Awake()
        {
            meshRenderer = this.GetComponent<MeshRenderer>();
        }
        
        
        private TextMeshPro gateLabel;
        public string OperationLabel => _corridorType switch
        {
            Constants.CorridorTypes.Increase => "+" + increaseAmount,
            Constants.CorridorTypes.Decrease => "-" + decreaseAmount,
            Constants.CorridorTypes.Multiply => "x" + multiplyAmount,
            Constants.CorridorTypes.Divide => "/" + divideAmount,
            _ => "?"
        };
        private void Start()
        {
            var go = new GameObject("Gate operation");
            go.transform.SetParent(transform, false);
            gateLabel = go.AddComponent<TextMeshPro>();
            gateLabel.font = Resources.Load<TMP_FontAsset>("RallyFont");
            gateLabel.text = OperationLabel;
            gateLabel.fontSize = 12; gateLabel.alignment = TextAlignmentOptions.Center;
            gateLabel.color = Color.white; gateLabel.outlineWidth = .2f;
            gateLabel.rectTransform.sizeDelta = new Vector2(3, 1.5f);
        }
        private void LateUpdate()
        {
            if (gateLabel == null || Camera.main == null) return;
            gateLabel.transform.position = GetComponent<Collider>().bounds.center + new Vector3(0, .7f, -.6f);
            gateLabel.transform.rotation = Camera.main.transform.rotation;
            // Parent gates are scaled differently; labels have a consistent world size.
            var scale = transform.lossyScale;
            gateLabel.transform.localScale = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);
        }

        public Constants.CorridorTypes GetCorridorType()
        {
            return _corridorType;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (consumed || !other.CompareTag("Cat") || GameFlowManager.Instance.state != GameState.Game) return;
            var formation=other.GetComponentInParent<RadialFormation>();
            if(formation==null)return;
            // Consume both choices BEFORE callbacks can process another crowd unit.
            consumed=true;
            if(neighbourCorridorCollider!=null)
            {
                var paired=neighbourCorridorCollider.GetComponent<Corridor>();
                if(paired!=null)paired.consumed=true;
                neighbourCorridorCollider.enabled=false;
            }
            GetComponent<BoxCollider>().enabled=false;
            appliedBefore=formation.amount;

            CorridorController.Instance.CorridorEffect(this, other.gameObject);
            appliedAfter=formation.amount;
            
        }

    }
    
}
