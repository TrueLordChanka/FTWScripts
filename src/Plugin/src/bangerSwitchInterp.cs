using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;

namespace Plugin.src
{

	public class bangerSwitchInterp
	{

		private bool m_isLargeAperture = true;

		// Token: 0x04006067 RID: 24679
		public Transform Flipsight;

		// Token: 0x04006068 RID: 24680
		public float m_flipsightStartRotX;

		// Token: 0x04006069 RID: 24681
		public float m_flipsightEndRotX = -90f;

		// Token: 0x0400606A RID: 24682
		private float m_flipsightCurRotX;

		// Token: 0x0400606B RID: 24683
		public AR15HandleSightFlipper.Axis RotAxis;

		// Token: 0x0400606C RID: 24684
		private float m_curFlipLerp;

		// Token: 0x0400606D RID: 24685
		private float m_tarFlipLerp;

		// Token: 0x0400606E RID: 24686
		private float m_lastFlipLerp;

		// Token: 0x02000A8D RID: 2701
		public enum Axis
		{
			// Token: 0x04006070 RID: 24688
			X,
			// Token: 0x04006071 RID: 24689
			Y,
			// Token: 0x04006072 RID: 24690
			Z
		}
		public BangerSwitch switchToObserve;



        // Token: 0x06003971 RID: 14705 RVA: 0x0019C2A8 File Offset: 0x0019A6A8
        //public void Update()
        //{
        //    if (switchToObserve.m_isOn == true)
        //    {
        //        if (this.m_isLargeAperture)
        //        {
        //            this.m_tarFlipLerp = 0f;
        //        }
        //        else
        //        {
        //            this.m_tarFlipLerp = 1f;
        //        }
        //        this.m_curFlipLerp = Mathf.MoveTowards(this.m_curFlipLerp, this.m_tarFlipLerp, Time.deltaTime * 4f);
        //        if (Mathf.Abs(this.m_curFlipLerp - this.m_lastFlipLerp) > 0.01f)
        //        {
        //            this.m_flipsightCurRotX = Mathf.Lerp(this.m_flipsightStartRotX, this.m_flipsightEndRotX, this.m_curFlipLerp);
        //            AR15HandleSightFlipper.Axis rotAxis = this.RotAxis;
        //            if (rotAxis != AR15HandleSightFlipper.Axis.X)
        //            {
        //                if (rotAxis != AR15HandleSightFlipper.Axis.Y)
        //                {
        //                    if (rotAxis == AR15HandleSightFlipper.Axis.Z)
        //                    {
        //                        this.Flipsight.localEulerAngles = new Vector3(0f, 0f, this.m_flipsightCurRotX);
        //                    }
        //                }
        //                else
        //                {
        //                    this.Flipsight.localEulerAngles = new Vector3(0f, this.m_flipsightCurRotX, 0f);
        //                }
        //            }
        //            else
        //            {
        //                this.Flipsight.localEulerAngles = new Vector3(this.m_flipsightCurRotX, 0f, 0f);
        //            }
        //        }
        //        this.m_lastFlipLerp = this.m_curFlipLerp;
        //    }
        //}

    }
}
