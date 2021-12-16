using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;

namespace Plugin.src
{
    class betterOverheat : MonoBehaviour
    {
#if !(UNITY_EDITOR || UNITY_5)

		private void OnShotFired(FVRFireArm firearm)
		{
			if (firearm == this.Hangun)
			{
				this.AddHeat();
				this.PSystem_Overheat.Emit(5);
				this.PSystem_Overheat2.Emit(5);
			}
		}

		private void AddHeat()
		{
			this.m_heat += heatPerShot;
			this.m_timeSinceLastShot = 0f;
			if (this.m_heat >= 1f && !this.m_isOverheating)
			{
				this.Overheat();
			}
			this.m_heat = Mathf.Clamp(this.m_heat, 0f, 1f);
		}

		private void Overheat()
		{
			this.enableWhenHot.SetActive(true);
			this.m_isOverheating = true;
			this.Hangun.Magazine.ForceEmpty();
			this.m_coolTime = coolDown;
			FVRPooledAudioSource fvrpooledAudioSource = this.Hangun.PlayAudioAsHandling(this.AudEvent_Overheat, base.transform.position);
			fvrpooledAudioSource.FollowThisTransform(base.transform);
		}

		private void Reset()
		{
			this.enableWhenHot.SetActive(false);
			this.m_isOverheating = false;
			this.Hangun.Magazine.ForceFull();
			this.Hangun.DropSlideRelease();
			this.m_heat = 0f;
			FVRPooledAudioSource fvrpooledAudioSource = this.Hangun.PlayAudioAsHandling(this.AudEvent_AllSet, base.transform.position);
			fvrpooledAudioSource.FollowThisTransform(base.transform);
		}

		private void Update()
		{
			this.Hangun.IsSlideLockExternalHeldDown = false;
			if (!this.m_isOverheating)
			{
				if (HasLerp)
				{
					//Lerping Closed
					if (!isLerping)
					{
						Debug.Log("roatethinggoooo");
						startTime = Time.time;
						journeyLength = Vector3.Distance(pointInterp1.localPosition, pointInterp2.localPosition);
						isLerping = true;
					}
					if (isLerping && isOpen)
					{
						float distCovered = (Time.time - startTime) * lerpSpeed;

						float fractionOfJourney = distCovered / journeyLength;
						Debug.Log("fraction of journety" + fractionOfJourney + "dist covered" + distCovered + "objloc" + lerpObject.transform.localPosition);
						lerpObject.transform.localPosition = Vector3.Lerp(pointInterp1.localPosition, pointInterp2.localPosition, fractionOfJourney);
						if (fractionOfJourney >= 1)
						{
							isLerping = false;
							isOpen = false;
						}
					}
				}

				if (this.m_timeSinceLastShot < 0.3f)
				{
					this.m_timeSinceLastShot += Time.deltaTime;
				}
				else if (this.m_heat > 0f)
				{
					this.m_heat -= Time.deltaTime;
				}
			}
			else
			{
				if (HasLerp)
				{
					//Lerping Open
					if (!isLerping)
					{
						Debug.Log("roatethinggoooo");
						startTime = Time.time;
						journeyLength = Vector3.Distance(pointInterp2.localPosition, pointInterp1.localPosition);
						isLerping = true;
					}
					if (isLerping && !isOpen)
					{
						float distCovered = (Time.time - startTime) * lerpSpeed;

						float fractionOfJourney = distCovered / journeyLength;
						Debug.Log("fraction of journety" + fractionOfJourney + "dist covered" + distCovered + "objloc" + lerpObject.transform.localPosition);
						lerpObject.transform.localPosition = Vector3.Lerp(pointInterp2.localPosition, pointInterp1.localPosition, fractionOfJourney);
						if (fractionOfJourney >= 1)
						{
							isLerping = false;
							isOpen = true;

						}
					}
				}
				this.PSystem_Overheat.Emit(1);
				if (this.m_coolTime > 0f)
				{
					this.m_coolTime -= Time.deltaTime;
				}
				else
				{
					this.Reset();
				}
			}
			float y = Mathf.Lerp(0.5f, -0.5f, this.m_heat);
			this.Rend.material.SetColor("_EmissionColor", this.colorGrad.Evaluate(this.m_heat));
			this.Rend.material.SetTextureOffset("_IncandescenceMap", new Vector2(0f, y));
		}

		private void Start()
		{
			GM.CurrentSceneSettings.ShotFiredEvent += this.OnShotFired;
		}

		private void OnDisable()
		{
			GM.CurrentSceneSettings.ShotFiredEvent -= this.OnShotFired;
		}


#endif

		[GradientHDR]
		public Gradient colorGrad;

		public Renderer Rend;

		public Handgun Hangun;

		public AudioEvent AudEvent_Overheat;

		public AudioEvent AudEvent_AllSet;

		public ParticleSystem PSystem_Overheat;

		public ParticleSystem PSystem_Overheat2;

		private float m_heat;

		public float m_timeSinceLastShot = 1f;

		private bool m_isOverheating;

		private float m_coolTime = 3.5f;

		public GameObject enableWhenHot;
		[Header("Heat Per Shot (max 1)")]
		public float heatPerShot = 0.1f;
		[Header("Cooldown time")]
		public float coolDown = 3.5f;
		[Header("To Lerp or not to Lerp")]
		public bool HasLerp;
		[Header("Open Position")]
		public Transform pointInterp1;
		[Header("Closed Position")]
		public Transform pointInterp2;
		[Header("Lerp Speed >1")]
		public float lerpSpeed = 0.5F;
		[Header("Thing to Lerp")]
		public GameObject lerpObject;

		private bool isLerping = false;
		private float startTime;
		private float journeyLength;
		private bool isOpen;
	}
}
