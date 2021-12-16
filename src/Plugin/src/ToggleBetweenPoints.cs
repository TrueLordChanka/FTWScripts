using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;


namespace Plugin.src
{ 
    public class ToggleBetweenPoints : MonoBehaviour
    {
		
        public GameObject objectToWatch;
		public GameObject affectedObject;
		public bool enableInRange;
		public enum Axis
		{
			X,
			Y,
			Z
		}
		public float upperLimit;
		public float lowerLimit;
		public ToggleBetweenPoints.Axis ObservedAxis;

		
		[Header("Debug Option")]
		public bool debug = true;
//#if !(UNITY_EDITOR || UNITY_5)

		public void FixedUpdate()
    {
			ToggleBetweenPoints.Axis observedAxis = this.ObservedAxis;
			if (observedAxis != ToggleBetweenPoints.Axis.X)
            {
				if (observedAxis != ToggleBetweenPoints.Axis.Y)
				{
					if (observedAxis == ToggleBetweenPoints.Axis.Z)
					{
                        if (objectToWatch.transform.localPosition.z >= lowerLimit && objectToWatch.transform.localPosition.z <= upperLimit)
						{
                            if (enableInRange)
                            {
								affectedObject.SetActive(true);
								if (debug) Debug.Log("Set Active with Z");
                            }
                            else
                            {
								affectedObject.SetActive(false);
								if (debug) Debug.Log("Set Inactive with Z");
							}
                        }
                        else
                        {
							affectedObject.SetActive(true);
							if (debug) Debug.Log("Set Active failed Z Test");
						}
					}
                }
                else
                {
					if (objectToWatch.transform.localPosition.y >= lowerLimit && objectToWatch.transform.localPosition.y <= upperLimit)
					{
						if (enableInRange)
						{
							affectedObject.SetActive(true);
							if (debug) Debug.Log("Set Active with Y");
						}
						else
						{
							affectedObject.SetActive(false);
							if (debug) Debug.Log("Set Inactive with Y");
						}
					}
					else
					{
						affectedObject.SetActive(true);
						if (debug) Debug.Log("Set Active failed Y Test");
					}
				}
            }
            else
            {
				if (objectToWatch.transform.localPosition.x >= lowerLimit && objectToWatch.transform.localPosition.x <= upperLimit)
				{
					if (enableInRange)
					{
						affectedObject.SetActive(true);
						if (debug) Debug.Log("Set Active with X");
					}
					else
					{
						affectedObject.SetActive(false);
						if (debug) Debug.Log("Set Inactive with X");
					}
				}
				else
				{
					affectedObject.SetActive(true);
					if (debug) Debug.Log("Set Active failed X Test");
				}
			}



    }







//#endif









	}
}



