using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;


public class MultiEnableLaserPointer : FVRFireArmAttachmentInterface
{
    

	private bool IsOn;

	public GameObject BeamEffect;

	public GameObject BeamEffect2;

	public GameObject BeamHitPoint;

	public Transform Aperture;

	public LayerMask LM;

	private RaycastHit m_hit;

	public AudioEvent AudEvent_LaserOnClip;

	public AudioEvent AudEvent_LaserOffClip;

#if !(UNITY_EDITOR || UNITY_5)

	public override void Awake()
	{
		base.Awake();
		this.BeamHitPoint.transform.SetParent(null);
	}

	public override void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.BeamHitPoint);
	}


	public override void UpdateInteraction(FVRViveHand hand)
	{
		Vector2 touchpadAxes = hand.Input.TouchpadAxes;
		if (hand.IsInStreamlinedMode)
		{
			if (hand.Input.BYButtonDown)
			{
				this.ToggleOn();
			}
		}
		else if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.25f && Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
		{
			this.ToggleOn();
		}
		base.UpdateInteraction(hand);
	}


	public override void FVRUpdate()
	{
		base.FVRUpdate();
		if (this.IsOn)
		{
			Vector3 position = this.Aperture.position + this.Aperture.forward * 1000f;
			float num = 1000f;
			if (Physics.Raycast(this.Aperture.position, this.Aperture.forward, out this.m_hit, 1000f, this.LM, QueryTriggerInteraction.Ignore))
			{
				position = this.m_hit.point;
				num = this.m_hit.distance;
			}
			float t = num * 0.01f;
			float num2 = Mathf.Lerp(0.01f, 0.2f, t);
			this.BeamHitPoint.transform.position = position;
			this.BeamHitPoint.transform.localScale = new Vector3(num2, num2, num2);
		}
	}

	public override void OnDetach()
	{
		base.OnDetach();
		this.IsOn = false;
		this.BeamHitPoint.SetActive(this.IsOn);
		this.BeamEffect.SetActive(this.IsOn);
		this.BeamEffect2.SetActive(this.IsOn);
	}


	private void ToggleOn()
	{
		this.IsOn = !this.IsOn;
		this.BeamHitPoint.SetActive(this.IsOn);
		this.BeamEffect.SetActive(this.IsOn);
		this.BeamEffect2.SetActive(this.IsOn);
		if (this.IsOn)
		{
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LaserOnClip, base.transform.position);
		}
		else
		{
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_LaserOffClip, base.transform.position);
		}
	}

	
#endif
}
