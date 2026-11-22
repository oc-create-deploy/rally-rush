using System;
using System.Collections;
using _Scripts.Core;
using UnityEngine;

namespace _Scripts.Models
{
    public class Cat : MonoBehaviour
    {
        private Animator _catAnimator;
        public Transform rotationTarget;
        private Transform leftArm, rightArm, leftLeg, rightLeg, body;
        private float stridePhase;
        private static Material kit, skin, dark;
        private static Material[] rainbow;
        private Material runnerPaint;

        private Transform Part(string label, Transform parent, Vector3 at, Vector3 size, Material material, PrimitiveType shape=PrimitiveType.Cube)
        {
            var go=GameObject.CreatePrimitive(shape);go.name=label;
            go.transform.SetParent(parent,false);go.transform.localPosition=at;go.transform.localScale=size;
            var collider=go.GetComponent<Collider>();collider.enabled=false;Destroy(collider);
            go.GetComponent<Renderer>().sharedMaterial=material;return go.transform;
        }
        private Transform Limb(string label, Vector3 at, Vector3 size)
        {
            var pivot=new GameObject(label+" joint").transform;pivot.SetParent(body,false);pivot.localPosition=at;
            Part(label,pivot,new Vector3(0,-size.y/2,0),size,runnerPaint,PrimitiveType.Capsule);return pivot;
        }
        private void BuildRunner()
        {
            // Original runtime humanoid replaces the shipped single primitive.
            // Preserve the existing root collider/tag used by gates and battles.
            var rootRenderer=GetComponent<Renderer>();
            if(kit==null)
            {
                Material basis=rootRenderer!=null ? rootRenderer.sharedMaterial : null;
                kit=basis!=null ? new Material(basis) : new Material(Shader.Find("Standard"));
                kit.color=new Color(.03f,.72f,.78f);
                skin=new Material(kit){color=new Color(.95f,.68f,.42f)};
                dark=new Material(kit){color=new Color(.12f,.19f,.29f)};
            }
            foreach(var renderer in GetComponentsInChildren<Renderer>()) renderer.enabled=false;
            body=new GameObject("Rally original runner").transform;body.SetParent(transform,false);
            if(rainbow==null || rainbow[0]==null)
            {
                Color[] colors={new Color(1f,.42f,.025f),new Color(.05f,.65f,1f),new Color(.35f,.9f,.06f),new Color(.65f,.12f,.95f),new Color(1f,.78f,.03f)};
                rainbow=new Material[colors.Length];
                for(int i=0;i<colors.Length;i++) { rainbow[i]=new Material(kit){color=colors[i]}; if(rainbow[i].HasProperty("_Glossiness"))rainbow[i].SetFloat("_Glossiness",.55f); }
            }
            runnerPaint=rainbow[UnityEngine.Random.Range(0,rainbow.Length)];
            Part("Rounded toy body",body,new Vector3(0,.32f,0),new Vector3(.68f,.9f,.46f),runnerPaint,PrimitiveType.Capsule);
            Part("Round toy head",body,new Vector3(0,1.04f,0),Vector3.one*.65f,runnerPaint,PrimitiveType.Sphere);
            leftArm=Limb("Left arm",new Vector3(-.43f,.64f,0),new Vector3(.24f,.72f,.26f));
            rightArm=Limb("Right arm",new Vector3(.43f,.64f,0),new Vector3(.24f,.72f,.26f));
            leftLeg=Limb("Left leg",new Vector3(-.2f,-.06f,0),new Vector3(.28f,.84f,.30f));
            rightLeg=Limb("Right leg",new Vector3(.2f,-.06f,0),new Vector3(.28f,.84f,.30f));
            rotationTarget=body;stridePhase=UnityEngine.Random.value*Mathf.PI*2;
        }
        private void Update()
        {
            if(body==null || GameFlowManager.Instance==null)return;
            var state=GameFlowManager.Instance.state;
            bool running=state==GameState.Game || state==GameState.MiniBattle || state==GameState.Battle;
            if(!running)return; // Freeze pose during the actual pause/menu.
            float swing=Mathf.Sin(Time.time*12+stridePhase)*34;
            leftArm.localRotation=Quaternion.Euler(-swing,0,0);rightArm.localRotation=Quaternion.Euler(swing,0,0);
            leftLeg.localRotation=Quaternion.Euler(swing,0,0);rightLeg.localRotation=Quaternion.Euler(-swing,0,0);
        }

        private void Awake()
        {
            BuildRunner();
            _catAnimator = GetComponent<Animator>();
            RandomizeIdle();
            if (GameFlowManager.Instance.state == GameState.Start)
            {
                ControlAnimationState(0);
            }
        }

        public void ControlAnimationState(int state)
        {
            if (_catAnimator == null || _catAnimator.runtimeAnimatorController == null) return;
            if (state == 0)
            {
                _catAnimator.SetTrigger("stopRunning");
            }

            if (state == 1)
            {
                _catAnimator.SetTrigger("startRunning");
            }
        }
        
        private void RandomizeIdle()
        {
            if (_catAnimator == null || _catAnimator.runtimeAnimatorController == null) return;
            _catAnimator.enabled = false;
            float waitToAnimate = UnityEngine.Random.Range(0, 0.2F);
            StartCoroutine(WaitForAnimate(waitToAnimate));
        }
        IEnumerator WaitForAnimate(float second)
        {
            yield return new WaitForSeconds(second);
            _catAnimator.enabled = true;
        }
    }
}
