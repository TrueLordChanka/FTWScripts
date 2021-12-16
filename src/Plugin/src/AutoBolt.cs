using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;


namespace Plugin.src
{ 
    public class AutoBolt : FVRFireArm
    {
		private bool debug = false;

		[Header("BoltActionRifle Config")]
		public FVRFireArmChamber Chamber;


		public bool HasMagEjectionButton = true;


		public bool HasFireSelectorButton = true;


		public AutoBoltHandle BoltHandle;

		public float BoltLerp;

		public bool BoltMovingForward;

		public AutoBoltHandle.BoltActionHandleState CurBoltHandleState;

		public AutoBoltHandle.BoltActionHandleState LastBoltHandleState;

		[Header("Hammer Config")]
		public bool HasVisualHammer;


		public Transform Hammer;


		public float HammerUncocked;

		public float HammerCocked;

		private bool m_isHammerCocked;


		public BoltActionRifle.HammerCockType CockType;


		private FVRFirearmMovingProxyRound m_proxy;

		[Header("Round Positions Config")]
		public Transform Extraction_MagazinePos;

		public Transform Extraction_ChamberPos;

		public Transform Extraction_Ejecting;

		public Transform EjectionPos;

		public float UpwardEjectionForce;

		public float RightwardEjectionForce = 2f;

		public float XSpinEjectionTorque = 80f;

		public Transform Muzzle;

		public GameObject ReloadTriggerWell;

		[Header("Control Config")]
		public float TriggerResetThreshold = 0.1f;

		public float TriggerFiringThreshold = 0.8f;

		private float m_triggerFloat;

		private bool m_hasTriggerCycled;

		private bool m_isMagReleasePressed;

		public Transform Trigger_Display;

		public float Trigger_ForwardValue;

		public float Trigger_RearwardValue;

		public FVRPhysicalObject.InterpStyle TriggerInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;

		public Transform Trigger_Display2;

		public float Trigger_ForwardValue2;

		public float Trigger_RearwardValue2;

		public FVRPhysicalObject.InterpStyle TriggerInterpStyle2 = FVRPhysicalObject.InterpStyle.Rotation;

		public Transform MagReleaseButton_Display;

		public FVRPhysicalObject.Axis MagReleaseAxis;

		public FVRPhysicalObject.InterpStyle MagReleaseInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;

		public float MagReleasePressedValue;

		public float MagReleaseUnpressedValue;

		private float m_magReleaseCurValue;

		private float m_magReleaseTarValue;

		private Vector2 TouchPadAxes = Vector2.zero;

		public Transform FireSelector_Display;

		public FVRPhysicalObject.Axis FireSelector_Axis;

		public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle = FVRPhysicalObject.InterpStyle.Rotation;

		public BoltActionRifle.FireSelectorMode[] FireSelector_Modes;

		private int m_fireSelectorMode;

		public bool RequiresHammerUncockedToToggleFireSelector;

		public bool UsesSecondFireSelectorChange;

		public Transform FireSelector_Display_Secondary;

		public FVRPhysicalObject.Axis FireSelector_Axis_Secondary;

		public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle_Secondary = FVRPhysicalObject.InterpStyle.Rotation;

		public BoltActionRifle.FireSelectorMode[] FireSelector_Modes_Secondary;

		[Header("Special Features")]
		public bool EjectsMagazineOnEmpty;

		public bool PlaysExtraTailOnShot;

		public FVRTailSoundClass ExtraTail = FVRTailSoundClass.Explosion;

		private bool justFired = false;
		private bool isLerping1 = false;
		private bool isLerping2 = false;
		private bool doneLerp1 = false;
		private float startTime;
		private float journeyLengthBolt1;
		private float journeyLengthStock1;
		private float journeyLengthBolt2;
		private float journeyLengthStock2;
		private bool obj1Complete;
		private bool obj2Complete;
		private bool obj1Complete2;
		private bool obj2Complete2;
		private bool beginLerp2 = false;
		[Header("Lerp Speed >1")]
		public float lerpSpeedboltUp = 0.5F;
		public float lerpSpeedboltBack = 0.5F;
		public float lerpSpeedStockF = 0.5F;
		public float lerpSpeedStockR = 0.5F;
		[Header("Things to Lerp")]
		public GameObject boltObject;
		public GameObject stockObject;
		[Header("BoltPoints")]
		public Transform boltPt1;
		public Transform boltPt2;
		public Transform boltPt3;
		[Header("Stock Points")]
		public Transform stockPt1;
		public Transform stockPt2;


		[Header("Reciprocating Barrel")]
		public bool HasReciprocatingBarrel;

		public G11RecoilingSystem RecoilSystem;

		private bool m_isQuickboltTouching;

		private Vector2 lastTPTouchPoint = Vector2.zero;

		public enum ZPos
		{
			Forward,
			Middle,
			Rear
		}


		public enum HammerCockType
		{
			OnBack,
			OnUp,
			OnClose,
			OnForward
		}

		public enum FireSelectorModeType
		{
			Safe,
			Single
		}

		[Serializable]
		public class FireSelectorMode
		{
			public float SelectorPosition;

			public BoltActionRifle.FireSelectorModeType ModeType;

			public bool IsBoltLocked;
		}

#if !(UNITY_EDITOR || UNITY_5)
		public bool IsHammerCocked
		{
			get
			{
				return this.m_isHammerCocked;
			}
		}

		public bool HasExtractedRound()
		{
			return this.m_proxy.IsFull;
		}

		public override void Awake()
		{
			base.Awake();
			if (this.UsesClips && this.ClipTrigger != null)
			{
				if (this.CurBoltHandleState == AutoBoltHandle.BoltActionHandleState.Rear)
				{
					if (!this.ClipTrigger.activeSelf)
					{
						this.ClipTrigger.SetActive(true);
					}
				}
				else if (this.ClipTrigger.activeSelf)
				{
					this.ClipTrigger.SetActive(false);
				}
			}
			GameObject gameObject = new GameObject("m_proxyRound");
			this.m_proxy = gameObject.AddComponent<FVRFirearmMovingProxyRound>();
			this.m_proxy.Init(base.transform);
		}

		public bool CanBoltMove()
		{
			return this.FireSelector_Modes.Length < 1 || !this.FireSelector_Modes[this.m_fireSelectorMode].IsBoltLocked;
		}

		public override int GetTutorialState()
		{
			if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == BoltActionRifle.FireSelectorModeType.Safe)
			{
				return 4;
			}
			if (this.Chamber.IsFull)
			{
				if (this.Chamber.IsSpent)
				{
					return 0;
				}
				if (base.AltGrip != null)
				{
					return 6;
				}
				return 5;
			}
			else
			{
				if (!(this.Magazine != null) || this.Magazine.HasARound())
				{
					return 3;
				}
				if (this.CurBoltHandleState == AutoBoltHandle.BoltActionHandleState.Forward || this.CurBoltHandleState == AutoBoltHandle.BoltActionHandleState.Mid)
				{
					return 0;
				}
				if (this.Clip != null)
				{
					return 2;
				}
				if (!this.Magazine.IsFull())
				{
					return 1;
				}
				return 3;
			}
		}

	
		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			this.TouchPadAxes = hand.Input.TouchpadAxes;
			if (this.m_hasTriggeredUpSinceBegin)
			{
				this.m_triggerFloat = hand.Input.TriggerFloat;
			}
			if (!this.m_hasTriggerCycled)
			{
				if (this.m_triggerFloat >= this.TriggerFiringThreshold)
				{
					this.m_hasTriggerCycled = true;
				}
			}
			else if (this.m_triggerFloat <= this.TriggerResetThreshold)
			{
				this.m_hasTriggerCycled = false;
			}
			this.m_isMagReleasePressed = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (hand.IsInStreamlinedMode)
			{
				if (this.HasFireSelectorButton && hand.Input.BYButtonDown)
				{
					this.ToggleFireSelector();
				}
				if (this.HasMagEjectionButton && hand.Input.AXButtonPressed)
				{
					this.m_isMagReleasePressed = true;
				}
				flag3 = true;
				if (base.Bipod != null && base.Bipod.IsBipodActive)
				{
					flag3 = false;
				}
				if (!this.CanBoltMove())
				{
					flag2 = false;
					flag3 = false;
				}
				if (flag3 && !this.IsAltHeld && base.AltGrip != null && base.AltGrip.m_hasTriggeredUpSinceBegin && base.AltGrip.IsHeld && base.AltGrip.m_hand.Input.BYButtonDown && this.BoltHandle.UsesQuickRelease && this.BoltHandle.HandleState == AutoBoltHandle.BoltActionHandleState.Forward)
				{
					flag = true;
				}
			}
			else
			{
				if (this.HasMagEjectionButton && hand.Input.TouchpadPressed && this.m_hasTriggeredUpSinceBegin && this.TouchPadAxes.magnitude > 0.3f && Vector2.Angle(this.TouchPadAxes, Vector2.down) <= 45f)
				{
					this.m_isMagReleasePressed = true;
				}
				if (GM.Options.QuickbeltOptions.BoltActionModeSetting == QuickbeltOptions.BoltActionMode.Quickbolting)
				{
					flag3 = true;
				}
				if (GM.Options.QuickbeltOptions.BoltActionModeSetting == QuickbeltOptions.BoltActionMode.Slidebolting)
				{
					flag2 = true;
				}
				if (GM.Options.ControlOptions.UseGunRigMode2)
				{
					flag2 = true;
					flag3 = true;
				}
				if (base.Bipod != null && base.Bipod.IsBipodActive)
				{
					flag2 = true;
					flag3 = false;
				}
				if (!this.CanBoltMove())
				{
					flag2 = false;
					flag3 = false;
				}
				if (this.IsHammerCocked && this.BoltHandle.HandleState == AutoBoltHandle.BoltActionHandleState.Forward && this.BoltHandle.HandleRot == AutoBoltHandle.BoltActionHandleRot.Down)
				{
					flag2 = false;
				}
				if (hand.Input.TouchpadDown && this.TouchPadAxes.magnitude > 0.1f)
				{
					if (flag3 && Vector2.Angle(this.TouchPadAxes, Vector2.right) <= 45f && this.BoltHandle.UsesQuickRelease && this.BoltHandle.HandleState == AutoBoltHandle.BoltActionHandleState.Forward)
					{
						flag = true;
					}
					else if (Vector2.Angle(this.TouchPadAxes, Vector2.left) <= 45f && this.HasFireSelectorButton)
					{
						this.ToggleFireSelector();
					}
				}
			}
			if (this.m_isMagReleasePressed)
			{
				this.ReleaseMag();
				if (this.ReloadTriggerWell != null)
				{
					this.ReloadTriggerWell.SetActive(false);
				}
			}
			else if (this.ReloadTriggerWell != null)
			{
				this.ReloadTriggerWell.SetActive(true);
			}
			if (this.m_hasTriggeredUpSinceBegin && !flag && flag2)
			{
				if ((base.AltGrip != null && !this.IsAltHeld) || GM.Options.ControlOptions.UseGunRigMode2 || (base.Bipod != null && base.Bipod.IsBipodActive))
				{
					if (hand.Input.TouchpadTouched)
					{
						Vector2 touchpadAxes = hand.Input.TouchpadAxes;
						if (touchpadAxes.magnitude > 0.1f)
						{
							bool isQuickboltTouching = this.m_isQuickboltTouching;
							if (Vector2.Angle(touchpadAxes, Vector2.right + Vector2.up) < 90f && !this.m_isQuickboltTouching)
							{
								this.m_isQuickboltTouching = true;
							}
							if (this.m_isQuickboltTouching && isQuickboltTouching)
							{
								float sangle = this.GetSAngle(touchpadAxes, this.lastTPTouchPoint, hand.CMode);
								this.BoltHandle.DriveBolt(-sangle / 90f);
							}
							this.lastTPTouchPoint = touchpadAxes;
						}
						else
						{
							this.lastTPTouchPoint = Vector2.zero;
						}
					}
					else
					{
						this.lastTPTouchPoint = Vector2.zero;
					}
				}
				if (this.m_isQuickboltTouching)
				{
					Debug.DrawLine(this.BoltHandle.BoltActionHandleRoot.transform.position, this.BoltHandle.BoltActionHandleRoot.transform.position + 0.1f * new Vector3(this.lastTPTouchPoint.x, this.lastTPTouchPoint.y, 0f), Color.blue);
				}
			}
			if (hand.Input.TouchpadTouchUp)
			{
				this.m_isQuickboltTouching = false;
				this.lastTPTouchPoint = Vector2.zero;
			}
			this.FiringSystem();
			base.UpdateInteraction(hand);
			if (flag && !this.IsAltHeld && base.AltGrip != null)
			{
				this.m_isQuickboltTouching = false;
				this.lastTPTouchPoint = Vector2.zero;
				hand.Buzz(hand.Buzzer.Buzz_BeginInteraction);
				hand.HandMadeGrabReleaseSound();
				hand.EndInteractionIfHeld(this);
				this.EndInteraction(hand);
				this.BoltHandle.BeginInteraction(hand);
				hand.ForceSetInteractable(this.BoltHandle);
				this.BoltHandle.TPInitiate();
			}
		}

		
		public float GetSignedAngle(Vector2 from, Vector2 to)
		{
			Vector2 vector = new Vector2(from.y, -from.x);
			Vector2 normalized = vector.normalized;
			float num = Mathf.Sign(Vector2.Dot(from, normalized));
			float num2 = Vector2.Angle(from, to);
			return num2 * num;
		}

	
		private float GetSAngle(Vector2 v1, Vector2 v2, ControlMode m)
		{
			if (m == ControlMode.Index)
			{
				return (v1.y - v2.y) * 130f;
			}
			float num = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
			return Vector2.Angle(v1, v2) * num;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			this.m_triggerFloat = 0f;
			this.m_hasTriggerCycled = false;
			this.m_isMagReleasePressed = false;
			this.m_isQuickboltTouching = false;
			this.lastTPTouchPoint = Vector2.zero;
			base.EndInteraction(hand);
		}


		public void SetHasTriggeredUp()
		{
			this.m_hasTriggeredUpSinceBegin = true;
		}

		public void CockHammer()
		{
			if (!this.m_isHammerCocked)
			{
				this.m_isHammerCocked = true;
				if (this.HasVisualHammer)
				{
					base.SetAnimatedComponent(this.Hammer, this.HammerCocked, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
				}
			}
		}


		public void DropHammer()
		{
			if (this.IsHammerCocked)
			{
				this.m_isHammerCocked = false;
				base.PlayAudioEvent(FirearmAudioEventType.HammerHit, 1f);
				this.Fire();
				if (this.HasVisualHammer)
				{
					base.SetAnimatedComponent(this.Hammer, this.HammerUncocked, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
				}
			}
		}

		protected virtual void ToggleFireSelector()
		{
			if (this.RequiresHammerUncockedToToggleFireSelector && this.IsHammerCocked)
			{
				return;
			}
			if (this.BoltHandle.IsHeld)
			{
				return;
			}
			if (this.CurBoltHandleState != AutoBoltHandle.BoltActionHandleState.Forward)
			{
				return;
			}
			if (this.BoltHandle.HandleRot != AutoBoltHandle.BoltActionHandleRot.Down)
			{
				return;
			}
			if (this.FireSelector_Modes.Length > 1)
			{
				this.m_fireSelectorMode++;
				if (this.m_fireSelectorMode >= this.FireSelector_Modes.Length)
				{
					this.m_fireSelectorMode -= this.FireSelector_Modes.Length;
				}
				base.PlayAudioEvent(FirearmAudioEventType.FireSelector, 1f);
				if (this.FireSelector_Display != null)
				{
					base.SetAnimatedComponent(this.FireSelector_Display, this.FireSelector_Modes[this.m_fireSelectorMode].SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
					if (this.UsesSecondFireSelectorChange)
					{
						base.SetAnimatedComponent(this.FireSelector_Display_Secondary, this.FireSelector_Modes_Secondary[this.m_fireSelectorMode].SelectorPosition, this.FireSelector_InterpStyle_Secondary, this.FireSelector_Axis_Secondary);
					}
				}
			}
		}

		public void ReleaseMag()
		{
			if (this.Magazine != null)
			{
				this.m_magReleaseCurValue = this.MagReleasePressedValue;
				base.EjectMag(false);
			}
		}

		public BoltActionRifle.FireSelectorMode GetFiringMode()
		{
			return this.FireSelector_Modes[this.m_fireSelectorMode];
		}

		protected virtual void FiringSystem()
		{
			BoltActionRifle.FireSelectorModeType modeType = this.FireSelector_Modes[this.m_fireSelectorMode].ModeType;
			if (modeType != BoltActionRifle.FireSelectorModeType.Safe && !this.IsAltHeld && this.BoltHandle.HandleState == AutoBoltHandle.BoltActionHandleState.Forward && this.BoltHandle.HandleRot != AutoBoltHandle.BoltActionHandleRot.Up && this.m_hasTriggerCycled)
			{
				this.DropHammer();
			}
		}


		public bool Fire()
		{
			BoltActionRifle.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
			if (!this.Chamber.Fire())
			{
				return false;
			}
			base.Fire(this.Chamber, this.GetMuzzle(), true, 1f);
			this.FireMuzzleSmoke();
			bool twoHandStabilized = this.IsTwoHandStabilized();
			bool foregripStabilized = this.IsForegripStabilized();
			bool shoulderStabilized = this.IsShoulderStabilized();

			/*
			 Trying to yeet the gun out of your hand, dont wokr :(
			 
			 */
			/*
			if (base.IsHeld && base.AltGrip == null && !base.Bipod.IsBipodActive)
			{
				if (this.m_hand != null)
				{
					if (debug) Debug.Log("YEET");
					this.m_hand.EndInteractionIfHeld(this);
					this.ForceBreakInteraction();
				}
				base.RootRigidbody.AddForceAtPosition((-base.transform.forward + base.transform.up + UnityEngine.Random.onUnitSphere * 0.25f) * 30f, this.MuzzlePos.position, ForceMode.Impulse);
				base.RootRigidbody.AddRelativeTorque(Vector3.right * 20f, ForceMode.Impulse);
			}
			*/
			this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, null, 1f);
			FVRSoundEnvironment currentSoundEnvironment = GM.CurrentPlayerBody.GetCurrentSoundEnvironment();
			base.PlayAudioGunShot(this.Chamber.GetRound(), currentSoundEnvironment, 1f);
			justFired = true; //My Code
			if (this.PlaysExtraTailOnShot)
			{
				AudioEvent tailSet = SM.GetTailSet(this.ExtraTail, currentSoundEnvironment);
				this.m_pool_tail.PlayClipVolumePitchOverride(tailSet, base.transform.position, tailSet.VolumeRange * 1f, this.AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x, null);
			}
			








			if (this.HasReciprocatingBarrel)
			{
				this.RecoilSystem.Recoil(false);
			}
			Chamber.IsAccessible = true;

			return true;
		}

		//Heres my Code
		public void Update()
        {
			
			//if ((BoltHandle.HandleState == AutoBoltHandle.BoltActionHandleState.Rear || BoltHandle.HandleState == AutoBoltHandle.BoltActionHandleState.Mid) && BoltHandle.HandleRot == AutoBoltHandle.BoltActionHandleRot.Down)
			if(BoltHandle.BoltActionHandleRoot.localPosition.z <= Vector3.Lerp(BoltHandle.Point_Forward.localPosition, BoltHandle.Point_Rearward.localPosition, 0.5f).z && BoltHandle.BoltActionHandle.localEulerAngles.z < BoltHandle.UnlockThreshold )
			{
				//BoltHandle.BoltActionHandle.transform.localEulerAngles = new Vector3(0, BoltHandle.MaxRot-1, 0);
				if (debug) Debug.Log("This is working?");

				//BoltHandle.DriveBolt(1f);
				/*
				BoltHandle.BoltActionHandleRoot.localPosition = BoltHandle.Point_Rearward.localPosition;
				BoltHandle.BoltActionHandle.localEulerAngles = new Vector3(0, 0, BoltHandle.UnlockThreshold);
				BoltHandle.HandleRot = AutoBoltHandle.BoltActionHandleRot.Up;
				this.UpdateBolt(AutoBoltHandle.BoltActionHandleState.Rear, 1);
				*/
			}
			
			if (justFired) { 
				if (!isLerping1 && !doneLerp1) //Lerp 1 initialization
				{
					obj1Complete2 = false;
					obj2Complete2 = false;
					if (debug) Debug.Log("Begin Lerp 1");
					startTime = Time.time;
					journeyLengthBolt1 = Vector3.Distance(boltPt1.localPosition, boltPt2.localPosition);
					if (debug) Debug.Log("journeylengthbolt"+ journeyLengthBolt1);
					journeyLengthStock1 = Vector3.Distance(stockPt1.localPosition, stockPt2.localPosition);
					isLerping1 = true;
					stockObject.transform.localPosition = stockPt1.transform.localPosition;
					boltObject.transform.localPosition = boltPt1.transform.localPosition;
				}
				if (isLerping1)
				{
					float distCoveredBolt = (Time.time - startTime) * lerpSpeedboltUp;
					float distCoveredStock = (Time.time - startTime) * lerpSpeedStockF;

					float fractionOfJourneyBolt = distCoveredBolt / journeyLengthBolt1;
					float fractionOfJourneyStock = distCoveredStock / journeyLengthStock1;
					//Debug.Log("fraction of journety" + fractionOfJourney + "dist covered" + distCovered + "objloc" + lerpObject.transform.localPosition);
					if(debug) Debug.Log("these values dont help have a nice egg instead ()");
					if (!obj1Complete)
					{

						if (debug) Debug.Log("lerping bolt1");

						boltObject.transform.localPosition = Vector3.Lerp(boltPt1.localPosition, boltPt2.localPosition, fractionOfJourneyBolt);
						BoltHandle.ManipulateBoltUsingPosition(boltObject.transform.position, true );
					}
					if (!obj2Complete)
					{
						if (debug) Debug.Log("Lerping Stock 1");
						stockObject.transform.localPosition = Vector3.Lerp(stockPt1.localPosition, stockPt2.localPosition, fractionOfJourneyStock);
					}
				
					if (fractionOfJourneyBolt >= 1)
					{
						obj1Complete = true;
						if (debug) Debug.Log("Bolts done pt1");
					}
					if( fractionOfJourneyStock >= 1)
					{
						obj2Complete = true;
						if (debug) Debug.Log("Stocks done pt1");
					}
					if(obj1Complete && obj2Complete)
					{
						if (debug) Debug.Log("LERP1 IS DONE IT SHOULDNT BE BEING CALLED ANYMORE");
						isLerping1 = false;
						beginLerp2 = true;
						doneLerp1 = true;
					}
				}

				if (!isLerping2 && beginLerp2) //lerp 2 initialization
				{
					if (debug) Debug.Log("Begin Lerp 2");
					startTime = Time.time;
					journeyLengthBolt2 = Vector3.Distance(boltPt2.localPosition, boltPt3.localPosition);
					journeyLengthStock2 = Vector3.Distance(stockPt2.localPosition, stockPt1.localPosition);
					boltObject.transform.localPosition = boltPt2.transform.localPosition;
					stockObject.transform.localPosition = stockPt2.transform.localPosition;

					isLerping2 = true;
					beginLerp2 = false;
					obj1Complete = false;
					obj2Complete = false;
				}
				if (isLerping2)
				{
					float distCoveredBolt = (Time.time - startTime) * lerpSpeedboltBack;
					float distCoveredStock = (Time.time - startTime) * lerpSpeedStockR;

					float fractionOfJourneyBolt = distCoveredBolt / journeyLengthBolt2;
					float fractionOfJourneyStock = distCoveredStock / journeyLengthStock2;
					//Debug.Log("fraction of journety" + fractionOfJourney + "dist covered" + distCovered + "objloc" + lerpObject.transform.localPosition);
					if (debug) Debug.Log("Lerp: the flamethrower! The kids love this one");
					if (!obj1Complete2)
					{
						if (debug) Debug.Log("lerping bolt2");
						boltObject.transform.localPosition = Vector3.Lerp(boltPt2.localPosition, boltPt3.localPosition, fractionOfJourneyBolt);
						BoltHandle.ManipulateBoltUsingPosition(boltObject.transform.position, true);
					
					}
					if (!obj2Complete2)
					{
						if (debug) Debug.Log("Lerping Stock 2");
						stockObject.transform.localPosition = Vector3.Lerp(stockPt2.localPosition, stockPt1.localPosition, fractionOfJourneyStock);
					}

					if (fractionOfJourneyBolt >= 1)
					{
						obj1Complete2 = true;
						if (debug) Debug.Log("Bolts done pt2");
						boltObject.transform.localPosition = boltPt3.transform.localPosition;
						
						this.UpdateBolt(AutoBoltHandle.BoltActionHandleState.Rear, 1);
						this.Chamber.IsAccessible = true;
						/*
						this.Chamber.EjectRound(this.EjectionPos.position, base.transform.right * this.RightwardEjectionForce + base.transform.up * this.UpwardEjectionForce, base.transform.up * this.XSpinEjectionTorque, false);
						
						this.Chamber.IsFull = false;
						if (debug) Debug.Log("chamber status " + this.Chamber.IsFull);
						*/
					}
					if (fractionOfJourneyStock >= 1)
					{
						stockObject.transform.localPosition = stockPt1.transform.localPosition;
						obj2Complete2 = true;
						if (debug) Debug.Log("Stocks done pt2");
					}
					if (obj1Complete2 && obj2Complete2)
					{
						boltObject.transform.localPosition = boltPt3.transform.localPosition;
						stockObject.transform.localPosition = stockPt1.transform.localPosition;
						if (debug) Debug.Log("All done the autobolt shit");
						BoltHandle.DriveBolt(1f);
						isLerping2 = false;
						justFired = false;
						doneLerp1 = false;
						
					}

				}

			}



		}
	
		public override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			this.UpdateComponentDisplay();		
		}

		private void UpdateComponentDisplay()
		{
			if (this.Trigger_Display != null)
			{
				if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Translate)
				{
					this.Trigger_Display.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat));
				}
				else if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Rotation)
				{
					this.Trigger_Display.localEulerAngles = new Vector3(Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat), 0f, 0f);
				}
			}
			if (this.Trigger_Display2 != null)
			{
				if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Translate)
				{
					this.Trigger_Display2.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this.Trigger_ForwardValue2, this.Trigger_RearwardValue2, this.m_triggerFloat));
				}
				else if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Rotation)
				{
					this.Trigger_Display2.localEulerAngles = new Vector3(Mathf.Lerp(this.Trigger_ForwardValue2, this.Trigger_RearwardValue2, this.m_triggerFloat), 0f, 0f);
				}
			}
			if (this.MagReleaseButton_Display != null)
			{
				Vector3 zero = Vector3.zero;
				if (this.m_isMagReleasePressed)
				{
					this.m_magReleaseTarValue = this.MagReleasePressedValue;
				}
				else
				{
					this.m_magReleaseTarValue = this.MagReleaseUnpressedValue;
				}
				this.m_magReleaseCurValue = Mathf.Lerp(this.m_magReleaseCurValue, this.m_magReleaseTarValue, Time.deltaTime * 4f);
				float magReleaseCurValue = this.m_magReleaseCurValue;
				FVRPhysicalObject.Axis magReleaseAxis = this.MagReleaseAxis;
				if (magReleaseAxis != FVRPhysicalObject.Axis.X)
				{
					if (magReleaseAxis != FVRPhysicalObject.Axis.Y)
					{
						if (magReleaseAxis == FVRPhysicalObject.Axis.Z)
						{
							zero.z = magReleaseCurValue;
						}
					}
					else
					{
						zero.y = magReleaseCurValue;
					}
				}
				else
				{
					zero.x = magReleaseCurValue;
				}
				FVRPhysicalObject.InterpStyle magReleaseInterpStyle = this.MagReleaseInterpStyle;
				if (magReleaseInterpStyle != FVRPhysicalObject.InterpStyle.Translate)
				{
					if (magReleaseInterpStyle == FVRPhysicalObject.InterpStyle.Rotation)
					{
						this.MagReleaseButton_Display.localEulerAngles = zero;
					}
				}
				else
				{
					this.MagReleaseButton_Display.localPosition = zero;
				}
			}
			if (this.CurBoltHandleState == AutoBoltHandle.BoltActionHandleState.Forward)
			{
				this.IsBreachOpenForGasOut = false;
			}
			else
			{
				this.IsBreachOpenForGasOut = true;
			}
		}

		public void UpdateBolt(AutoBoltHandle.BoltActionHandleState State, float lerp)
		{
			this.CurBoltHandleState = State;
			this.BoltLerp = lerp;
			if (this.CurBoltHandleState != AutoBoltHandle.BoltActionHandleState.Forward && !this.m_proxy.IsFull && !this.Chamber.IsFull)
			{
				this.Chamber.IsAccessible = true;
			}
			else
			{
				this.Chamber.IsAccessible = false;
			}
			if (this.UsesClips && this.ClipTrigger != null)
			{
				if (this.CurBoltHandleState == AutoBoltHandle.BoltActionHandleState.Rear)
				{
					if (!this.ClipTrigger.activeSelf)
					{
						this.ClipTrigger.SetActive(true);
					}
				}
				else if (this.ClipTrigger.activeSelf)
				{
					this.ClipTrigger.SetActive(false);
				}
			}
			if (this.CurBoltHandleState == AutoBoltHandle.BoltActionHandleState.Rear && this.LastBoltHandleState != AutoBoltHandle.BoltActionHandleState.Rear)
			{
				if (this.CockType == BoltActionRifle.HammerCockType.OnBack)
				{
					this.CockHammer();
				}
				if (this.Chamber.IsFull)
				{
					if (debug) Debug.Log("This should only be happing if it was manually cycled"); 
					this.Chamber.EjectRound(this.EjectionPos.position, base.transform.right * this.RightwardEjectionForce + base.transform.up * this.UpwardEjectionForce, base.transform.right * this.XSpinEjectionTorque, false);
					base.PlayAudioEvent(FirearmAudioEventType.HandleBack, 1f);
				}
				else
				{
					base.PlayAudioEvent(FirearmAudioEventType.HandleBackEmpty, 1f);
				}
				this.BoltMovingForward = true;
			}
			else if (this.CurBoltHandleState == AutoBoltHandle.BoltActionHandleState.Forward && this.LastBoltHandleState != AutoBoltHandle.BoltActionHandleState.Forward)
			{
				if (this.CockType == BoltActionRifle.HammerCockType.OnForward)
				{
					this.CockHammer();
				}
				if (this.m_proxy.IsFull && !this.Chamber.IsFull)
				{
					this.Chamber.SetRound(this.m_proxy.Round);
					this.m_proxy.ClearProxy();
					base.PlayAudioEvent(FirearmAudioEventType.HandleForward, 1f);
				}
				else
				{
					base.PlayAudioEvent(FirearmAudioEventType.HandleForwardEmpty, 1f);
				}
				this.BoltMovingForward = false;
			}
			else if (this.CurBoltHandleState == AutoBoltHandle.BoltActionHandleState.Mid && this.LastBoltHandleState == AutoBoltHandle.BoltActionHandleState.Rear && this.Magazine != null)
			{
				if (!this.m_proxy.IsFull && this.Magazine.HasARound() && !this.Chamber.IsFull)
				{
					GameObject fromPrefabReference = this.Magazine.RemoveRound(false);
					this.m_proxy.SetFromPrefabReference(fromPrefabReference);
				}
				if (this.EjectsMagazineOnEmpty && !this.Magazine.HasARound())
				{
					this.EjectMag();
				}
			}
			if (this.m_proxy.IsFull)
			{
				this.m_proxy.ProxyRound.position = Vector3.Lerp(this.Extraction_ChamberPos.position, this.Extraction_MagazinePos.position, this.BoltLerp);
				this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.Extraction_ChamberPos.rotation, this.Extraction_MagazinePos.rotation, this.BoltLerp);
			}
			if (this.Chamber.IsFull)
			{
				this.Chamber.ProxyRound.position = Vector3.Lerp(this.Extraction_ChamberPos.position, this.Extraction_Ejecting.position, this.BoltLerp);
				this.Chamber.ProxyRound.rotation = Quaternion.Slerp(this.Extraction_ChamberPos.rotation, this.Extraction_Ejecting.rotation, this.BoltLerp);
			}
			this.LastBoltHandleState = this.CurBoltHandleState;
		}

		public override List<FireArmRoundClass> GetChamberRoundList()
		{
			if (this.Chamber.IsFull && !this.Chamber.IsSpent)
			{
				return new List<FireArmRoundClass>
				{
					this.Chamber.GetRound().RoundClass
				};
			}
			return null;
		}

		public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
			if (rounds.Count > 0)
			{
				this.Chamber.Autochamber(rounds[0]);
			}
		}

		public override List<string> GetFlagList()
		{
			return null;
		}

		public override void SetFromFlagList(List<string> flags)
		{
		}

		public override void ConfigureFromFlagDic(Dictionary<string, string> f)
		{
			string key = string.Empty;
			string text = string.Empty;
			key = "HammerState";
			if (f.ContainsKey(key))
			{
				text = f[key];
				if (text == "Cocked")
				{
					this.m_isHammerCocked = true;
				}
				if (this.HasVisualHammer)
				{
					base.SetAnimatedComponent(this.Hammer, this.HammerCocked, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
				}
			}
			if (this.FireSelector_Modes.Length > 1)
			{
				key = "FireSelectorState";
				if (f.ContainsKey(key))
				{
					text = f[key];
					int.TryParse(text, out this.m_fireSelectorMode);
				}
				if (this.FireSelector_Display != null)
				{
					BoltActionRifle.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
					base.SetAnimatedComponent(this.FireSelector_Display, fireSelectorMode.SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
				}
			}
		}

		public override Dictionary<string, string> GetFlagDic()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string key = "HammerState";
			string value = "Uncocked";
			if (this.m_isHammerCocked)
			{
				value = "Cocked";
			}
			dictionary.Add(key, value);
			if (this.FireSelector_Modes.Length > 1)
			{
				key = "FireSelectorState";
				value = this.m_fireSelectorMode.ToString();
				dictionary.Add(key, value);
			}
			return dictionary;
		}


#endif









	}
	}



