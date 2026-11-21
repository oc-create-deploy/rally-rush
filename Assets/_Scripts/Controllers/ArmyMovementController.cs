using System;
using _Scripts.Models;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.Controllers
{
    public class ArmyMovementController : MonoBehaviour
    {
        public bool isGamePaused;
        private float _lastFrameFingerPositionX;
        private Vector3 _lastFrameFingerPosition;
        private float _moveFactorX;
        private float MoveFactorX => _moveFactorX;
        private Vector3 movementVector;


        void Update()
        {
            if (isGamePaused != false) return;

            ControlCharacter();
            //CLAMP X POS
            var pos = transform.position;
            pos.x =  Mathf.Clamp(transform.position.x, -Constants.CLAMP_MODIFIER, Constants.CLAMP_MODIFIER);
            transform.position = pos;
            
            Move();
        }
        
        private void ControlCharacter()
        {
            _moveFactorX=0;
            if(Input.GetMouseButtonDown(0))
            {
                _lastFrameFingerPositionX=Input.mousePosition.x;
                _lastFrameFingerPosition=Input.mousePosition;
            }
            else if(Input.GetMouseButton(0))
            {
                // Map a screen-width drag to the track width, independent of FPS.
                _moveFactorX=(Input.mousePosition.x-_lastFrameFingerPositionX)/Mathf.Max(1,Screen.width);
                _lastFrameFingerPositionX=Input.mousePosition.x;
            }
        }
        private void Move()
        {
            var position=transform.position;
            position.x=Mathf.Clamp(position.x+_moveFactorX*Constants.CLAMP_MODIFIER*4,-Constants.CLAMP_MODIFIER,Constants.CLAMP_MODIFIER);
            position+=transform.forward*Time.deltaTime*Constants.SPEED_COEFFICIENT;
            transform.position=position;
            RotateAnimatedRig(_moveFactorX*500);
        }

        private Vector3 CalculateDirection()
        {
            Vector3 temp = ( Input.mousePosition - _lastFrameFingerPosition);
            temp.y = 0;
            temp.z = 0;
            return temp;
        }

        
        private void RotateAnimatedRig(float dir)
        {
            for (int i = 1; i < this.transform.childCount; i++)
            {
                Cat cat = transform.GetChild(i).GetComponent<Cat>();
                // Primitive crowd units have no animated child rig. Rotate their root instead.
                if (cat == null) continue;
                Transform rig = cat.rotationTarget != null ? cat.rotationTarget : cat.transform;
                rig.rotation = Quaternion.Euler(0, Mathf.Clamp(dir, -100F, 100F), 0);

            }
        }


    }
}
