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
            gateLabel.fontSize = 8; gateLabel.alignment = TextAlignmentOptions.Center;
            gateLabel.color = Color.white; gateLabel.outlineWidth = .2f;
            gateLabel.rectTransform.sizeDelta = new Vector2(3, 1.5f);
        }
        private void LateUpdate()
        {
            if (gateLabel == null || Camera.main == null) return;
            // Consumed gates disable their collider; disabled Collider.bounds is empty.
            // Anchor to the still-visible mesh, so labels do not collapse to origin.
            var bounds = meshRenderer.bounds;
            gateLabel.transform.position = new Vector3(bounds.center.x, bounds.max.y + 1.5f, bounds.min.z - 1f);
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
            // The player's steering selects a side, not an outer follower's
            // earlier exit when a wide crowd straddles both alternatives.
            var bounds=GetComponent<Collider>().bounds;
            if(formation.transform.position.x<bounds.min.x || formation.transform.position.x>bounds.max.x)return;
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
