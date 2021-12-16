using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;

namespace Plugin.src
{ //mathf.lerp
	public class proxGrenadeRing : FVRInteractiveObject
	{


        public bool HasPinDetached()
        {
            return this.m_hasPinDetached;
        }


        public override void Start()
        {
            this.G.RegisterRing(this);
            
            GameObject gameObject = new GameObject();
            this.m_pointIn = gameObject.transform;
            this.m_pointIn.transform.SetParent(this.G.transform);
            this.m_pointIn.transform.localPosition = this.Pin.transform.localPosition;
            this.m_pointIn.transform.localRotation = this.Pin.transform.localRotation;
            this.m_posMin = this.m_pointIn.transform.localPosition.z;
            GameObject gameObject2 = new GameObject();
            this.m_pointOut = gameObject2.transform;
            this.m_pointOut.transform.SetParent(this.G.transform);
            this.m_pointOut.transform.position = base.transform.position + this.Pin.transform.forward * this.PinLength;
            this.m_pointOut.transform.localRotation = this.Pin.transform.localRotation;
            this.m_posMax = this.m_pointOut.transform.localPosition.z;
            GameObject gameObject3 = new GameObject();
            this.m_pointMaxGrab = gameObject3.transform;
            this.m_pointMaxGrab.transform.SetParent(this.G.transform);
            this.m_pointMaxGrab.transform.position = base.transform.position + this.Pin.transform.forward * this.PinLength * 3f;
            this.m_pointMaxGrab.transform.localRotation = this.Pin.transform.localRotation;
            this.m_posMaxGrab = this.m_pointMaxGrab.transform.localPosition.z;
            this.m_posCurrent = this.Pin.transform.localPosition.z;
            base.Start();
        }

        public void PopOutRoutine()
        {
            base.Invoke("ForcePopOut", 0.05f);
        }

        private void ForcePopOut()
        {
            this.DetachPin();
        }


        public override bool IsInteractable()
        {
            return (!(this.G.QuickbeltSlot != null) || !this.G.m_isSpawnLock) && !this.m_hasPinDetached && base.IsInteractable();
        }


        public override void BeginInteraction(FVRViveHand hand)
        {
            base.BeginInteraction(hand);
            this.m_handZOffset = this.Pin.transform.InverseTransformPoint(hand.Input.Pos).z;
        }


        public override void EndInteraction(FVRViveHand hand)
        {
            this.Waggle.ResetParticlePos();
            base.EndInteraction(hand);
        }


        private void UpdatePinPos()
        {
            if (base.IsHeld)
            {
                Vector3 closestValidPoint = base.GetClosestValidPoint(this.m_pointIn.position, this.m_pointMaxGrab.position, this.m_hand.Input.Pos + -this.Pin.transform.forward * this.m_handZOffset);
                this.m_heldTarget = this.G.transform.InverseTransformPoint(closestValidPoint).z;
            }
            float num = this.m_posCurrent;
            float num2 = this.m_posCurrent;
            if (base.IsHeld)
            {
                num2 = this.m_heldTarget;
                float num3 = Mathf.InverseLerp(this.m_posMin, this.m_posMaxGrab, num2);
                float num4 = Mathf.Lerp(0.02f, 0.6f, num3 * num3 * num3 * num3);
                num = Mathf.MoveTowards(this.m_posCurrent, num2, num4 * Time.deltaTime);
            }
            if (Mathf.Abs(num - this.m_posCurrent) > Mathf.Epsilon)
            {
                this.m_posCurrent = num;
                base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, this.m_posCurrent);
                this.Pin.transform.localPosition = new Vector3(this.Pin.transform.localPosition.x, this.Pin.transform.localPosition.y, this.m_posCurrent);
                if (this.UsesSwapPin && !this.m_hasSwapped)
                {
                    this.m_hasSwapped = true;
                    this.Pin.gameObject.GetComponent<MeshFilter>().mesh = this.SwapPin;
                }
            }
            if (this.m_posCurrent > this.m_posMax && base.IsHeld)
            {
                this.DetachPin();
            }
            if (base.IsHeld)
            {
                this.Waggle.hingeGraphic.localEulerAngles = this.GrabEuler;
            }
            else
            {
                this.Waggle.Execute();
            }
        }


        public void DetachPin()
        {
            if (this.m_hasPinDetached)
            {
                return;
            }
            this.m_hasPinDetached = true;
            this.Pin.RootRigidbody = this.Pin.gameObject.AddComponent<Rigidbody>();
            this.Pin.RootRigidbody.mass = 0.02f;
            FVRViveHand hand = this.m_hand;
            this.ForceBreakInteraction();
            base.transform.SetParent(this.Pin.transform);
            this.Pin.enabled = true;
            hand.ForceSetInteractable(this.Pin);
            if (Vector3.Distance(this.Pin.transform.position, hand.Input.Pos) > 0.04f)
            {
                this.Pin.transform.position = hand.Input.Pos;
            }
            this.Pin.BeginInteraction(hand);
            SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.G.AudEvent_Pinpull, base.transform.position);
            base.GetComponent<Collider>().enabled = false;
            base.enabled = false;
        }


        public override void FVRUpdate()
        {
            this.UpdatePinPos();
            base.FVRUpdate();
        }


        public float PinLength = 0.01f;

			
			public proxGrenade G;

			
			public FVRPhysicalObject Pin;

			
			private Transform m_pointIn;

			
			private Transform m_pointOut;

			
			private Transform m_pointMaxGrab;

			
			private float m_posMin;

			
			private float m_posMax;

			private float m_posMaxGrab;

			
			private float m_posCurrent;

			
			private float m_heldTarget;

			private float m_handZOffset;

			private bool m_hasPinDetached;

			public WaggleJoint Waggle;

			public Vector3 GrabEuler;

			public bool UsesSwapPin;

			public Mesh SwapPin;

			private bool m_hasSwapped;

		}
	}



