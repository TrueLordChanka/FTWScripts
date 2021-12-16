using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;


namespace Plugin.src
{ 
    public class AutoBoltHandle : FVRInteractiveObject
    {
		// Token: 0x040033C8 RID: 13256
		public AutoBolt Rifle;

		// Token: 0x040033C9 RID: 13257
		public bool UsesQuickRelease;

		// Token: 0x040033CA RID: 13258
		public Transform BoltActionHandleRoot;

		// Token: 0x040033CB RID: 13259
		public Transform BoltActionHandle;

		// Token: 0x040033CC RID: 13260
		public float BaseRotOffset;

		// Token: 0x040033CD RID: 13261
		private float rotAngle;

		// Token: 0x040033CE RID: 13262
		public float MinRot;

		// Token: 0x040033CF RID: 13263
		public float MaxRot;

		// Token: 0x040033D0 RID: 13264
		public float UnlockThreshold = 70f;

		// Token: 0x040033D1 RID: 13265
		public Transform Point_Forward;

		// Token: 0x040033D2 RID: 13266
		public Transform Point_Rearward;

		// Token: 0x040033D3 RID: 13267
		public Vector3 HandPosOffset = new Vector3(0f, 0f, 0f);

		// Token: 0x040033D4 RID: 13268
		private bool m_wasTPInitiated;

		// Token: 0x040033D5 RID: 13269
		public bool UsesExtraRotationPiece;

		// Token: 0x040033D6 RID: 13270
		public Transform ExtraRotationPiece;

		// Token: 0x040033D7 RID: 13271
		public AutoBoltHandle.BoltActionHandleState HandleState;

		// Token: 0x040033D8 RID: 13272
		public AutoBoltHandle.BoltActionHandleState LastHandleState;

		// Token: 0x040033D9 RID: 13273
		public AutoBoltHandle.BoltActionHandleRot HandleRot = AutoBoltHandle.BoltActionHandleRot.Down;

		// Token: 0x040033DA RID: 13274
		public AutoBoltHandle.BoltActionHandleRot LastHandleRot = AutoBoltHandle.BoltActionHandleRot.Down;

		// Token: 0x040033DB RID: 13275
		private Vector3 m_localHandPos_BoltDown;

		// Token: 0x040033DC RID: 13276
		private Vector3 m_localHandPos_BoltUp;

		// Token: 0x040033DD RID: 13277
		private Vector3 m_localHandPos_BoltBack;

		// Token: 0x040033DE RID: 13278
		private float fakeBoltDrive;

		// Token: 0x02000505 RID: 1285
		public enum BoltActionHandleState
		{
			// Token: 0x040033E0 RID: 13280
			Forward,
			// Token: 0x040033E1 RID: 13281
			Mid,
			// Token: 0x040033E2 RID: 13282
			Rear
		}

		// Token: 0x02000506 RID: 1286
		public enum BoltActionHandleRot
		{
			// Token: 0x040033E4 RID: 13284
			Up,
			// Token: 0x040033E5 RID: 13285
			Mid,
			// Token: 0x040033E6 RID: 13286
			Down
		}

#if !(UNITY_EDITOR || UNITY_5)


		// Token: 0x06001A8F RID: 6799 RVA: 0x000C6F02 File Offset: 0x000C5302
		public override void Awake()
		{
			base.Awake();
			this.CalculateHandPoses();
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x000C6F10 File Offset: 0x000C5310
		public void TPInitiate()
		{
			this.m_wasTPInitiated = true;
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x000C6F19 File Offset: 0x000C5319
		public override void EndInteraction(FVRViveHand hand)
		{
			this.m_wasTPInitiated = false;
			base.EndInteraction(hand);
		}

		// Token: 0x06001A92 RID: 6802 RVA: 0x000C6F29 File Offset: 0x000C5329
		public override bool IsInteractable()
		{
			return this.Rifle.CanBoltMove() && base.IsInteractable();
		}

		// Token: 0x06001A93 RID: 6803 RVA: 0x000C6F44 File Offset: 0x000C5344
		private void CalculateHandPoses()
		{
			this.m_localHandPos_BoltDown = this.Rifle.transform.InverseTransformPoint(base.transform.position);
			Vector3 vector = base.transform.position - this.BoltActionHandleRoot.position;
			vector = Quaternion.AngleAxis(Mathf.Abs(this.MaxRot - this.MinRot) + 10f, this.BoltActionHandleRoot.forward) * vector;
			vector += this.BoltActionHandleRoot.position;
			this.m_localHandPos_BoltUp = this.Rifle.transform.InverseTransformPoint(vector);
			Vector3 position = vector + -this.BoltActionHandleRoot.forward * (0.005f + Vector3.Distance(this.Point_Forward.position, this.Point_Rearward.position));
			this.m_localHandPos_BoltBack = this.Rifle.transform.InverseTransformPoint(position);
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x000C703C File Offset: 0x000C543C
		public override void FVRUpdate()
		{
			base.FVRUpdate();
			Debug.DrawLine(this.Rifle.transform.TransformPoint(this.m_localHandPos_BoltDown), this.Rifle.transform.TransformPoint(this.m_localHandPos_BoltUp), Color.red);
			Debug.DrawLine(this.Rifle.transform.TransformPoint(this.m_localHandPos_BoltUp), this.Rifle.transform.TransformPoint(this.m_localHandPos_BoltBack), Color.blue);
			
			//Debug.Log("RotAndle: " + this.rotAngle + "  UnlockThresh: " + this.UnlockThreshold);
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x000C70BC File Offset: 0x000C54BC
		public void DriveBolt(float amount)
		{
			if (this.Rifle.Clip != null)
			{
				this.Rifle.EjectClip();
				return;
			}
			this.fakeBoltDrive += amount;
			this.fakeBoltDrive = Mathf.Clamp(this.fakeBoltDrive, 0f, 1f);
			Vector3 vector;
			if (this.fakeBoltDrive < 0.5f)
			{
				vector = Vector3.Slerp(this.m_localHandPos_BoltDown, this.m_localHandPos_BoltUp, this.fakeBoltDrive * 2f);
			}
			else
			{
				vector = Vector3.Lerp(this.m_localHandPos_BoltUp, this.m_localHandPos_BoltBack, (this.fakeBoltDrive - 0.5f) * 2f);
			}
			vector = this.Rifle.transform.TransformPoint(vector);
			this.ManipulateBoltUsingPosition(vector, true);
			float num = Mathf.InverseLerp(this.Point_Forward.localPosition.z, this.Point_Rearward.localPosition.z, this.BoltActionHandleRoot.localPosition.z);
			if (num < 0.05f)
			{
				this.HandleState = AutoBoltHandle.BoltActionHandleState.Forward;
			}
			else if (num > 0.95f)
			{
				this.HandleState = AutoBoltHandle.BoltActionHandleState.Rear;
			}
			else
			{
				this.HandleState = AutoBoltHandle.BoltActionHandleState.Mid;
			}
			this.Rifle.UpdateBolt(this.HandleState, num);
			this.LastHandleState = this.HandleState;
		}

	
		public bool ManipulateBoltUsingPosition(Vector3 pos, bool touchpadDrive)
		{
			bool result = false;
			if (this.HandleState == AutoBoltHandle.BoltActionHandleState.Forward)
			{
				Vector3 vector = pos - this.BoltActionHandle.position;
				vector = Vector3.ProjectOnPlane(vector, this.BoltActionHandleRoot.transform.forward).normalized;
				Vector3 right = this.BoltActionHandleRoot.transform.right;
				this.rotAngle = Mathf.Atan2(Vector3.Dot(this.BoltActionHandleRoot.forward, Vector3.Cross(right, vector)), Vector3.Dot(right, vector)) * 57.29578f;
				this.rotAngle += this.BaseRotOffset;
				this.rotAngle = Mathf.Clamp(this.rotAngle, this.MinRot, this.MaxRot);
				this.BoltActionHandle.localEulerAngles = new Vector3(0f, 0f, this.rotAngle);
				if (this.UsesExtraRotationPiece)
				{
					this.ExtraRotationPiece.localEulerAngles = new Vector3(0f, 0f, this.rotAngle);
				}
				if (this.rotAngle >= this.UnlockThreshold )
				{
					this.HandleRot = AutoBoltHandle.BoltActionHandleRot.Up;
				}
				else if (Mathf.Abs(this.rotAngle - this.MinRot) < 2f  )
				{
					this.HandleRot = AutoBoltHandle.BoltActionHandleRot.Down;
				}
				else
				{
					this.HandleRot = AutoBoltHandle.BoltActionHandleRot.Mid;
				}
				if (this.HandleRot == AutoBoltHandle.BoltActionHandleRot.Up && this.LastHandleRot != AutoBoltHandle.BoltActionHandleRot.Up)
				{
					this.Rifle.PlayAudioEvent(FirearmAudioEventType.HandleUp, 1f);
					if (this.Rifle.CockType == BoltActionRifle.HammerCockType.OnUp)
					{
						this.Rifle.CockHammer();
					}
				}
				else if (this.HandleRot == AutoBoltHandle.BoltActionHandleRot.Down && this.LastHandleRot != AutoBoltHandle.BoltActionHandleRot.Down)
				{
					this.Rifle.PlayAudioEvent(FirearmAudioEventType.HandleDown, 1f);
					if (this.Rifle.CockType == BoltActionRifle.HammerCockType.OnClose)
					{
						this.Rifle.CockHammer();
					}
					result = true;
				}
				this.LastHandleRot = this.HandleRot;
			}
			if (this.rotAngle >= this.UnlockThreshold)
			{
				Vector3 b = this.HandPosOffset.x * this.BoltActionHandleRoot.right + this.HandPosOffset.y * this.BoltActionHandleRoot.up + this.HandPosOffset.z * this.BoltActionHandleRoot.forward;
				Vector3 closestValidPoint = base.GetClosestValidPoint(this.Point_Forward.position, this.Point_Rearward.position, pos - b);
				this.BoltActionHandleRoot.position = closestValidPoint;
			}
			return result;
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x000C74AC File Offset: 0x000C58AC
		public override void UpdateInteraction(FVRViveHand hand)
		{
			if (this.Rifle.Clip != null)
			{
				this.Rifle.EjectClip();
				return;
			}
			bool flag = this.ManipulateBoltUsingPosition(hand.Input.Pos, false);
			float num = Mathf.InverseLerp(this.Point_Forward.localPosition.z, this.Point_Rearward.localPosition.z, this.BoltActionHandleRoot.localPosition.z);
			if (num < 0.05f)
			{
				this.HandleState = AutoBoltHandle.BoltActionHandleState.Forward;
			}
			else if (num > 0.95f)
			{
				this.HandleState = AutoBoltHandle.BoltActionHandleState.Rear;
			}
			else
			{
				this.HandleState = AutoBoltHandle.BoltActionHandleState.Mid;
			}
			if (this.HandleState != AutoBoltHandle.BoltActionHandleState.Forward || this.LastHandleState == AutoBoltHandle.BoltActionHandleState.Forward)
			{
				if (this.HandleState != AutoBoltHandle.BoltActionHandleState.Rear || this.LastHandleState != AutoBoltHandle.BoltActionHandleState.Rear)
				{
				}
			}
			this.Rifle.UpdateBolt(this.HandleState, num);
			this.LastHandleState = this.HandleState;
			base.UpdateInteraction(hand);
			if (flag && this.UsesQuickRelease && this.m_wasTPInitiated && (this.Rifle.IsAltHeld || !this.Rifle.IsHeld))
			{
				hand.Buzz(hand.Buzzer.Buzz_BeginInteraction);
				this.EndInteraction(hand);
				this.Rifle.BeginInteraction(hand);
				hand.ForceSetInteractable(this.Rifle);
				if (!hand.Input.TriggerPressed)
				{
					this.Rifle.SetHasTriggeredUp();
				}
			}
		}


#endif
	}
	}



