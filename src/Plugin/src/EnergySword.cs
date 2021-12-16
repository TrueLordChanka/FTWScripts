using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;


namespace Plugin.src
{ 
    public class EnergySword : FVRMeleeWeapon
    {


        [SerializeField] private Material bladeMat;
        public GameObject bladePhys;
        
        public AudioEvent igniteSound;
        public AudioEvent retractSound;
        [Header("Lerp Speed >1")]
        public float lerpSpeed = 0.5F;

        private bool isdebug = true;
        public bool isLit = false;
        private bool isLerping = false;
        private float startTime;
        private float journeyLength;
        private bool stable= true;


#if !(UNITY_EDITOR || UNITY_5)

        private void Start() //defaults the blade to off
        {
            Color color = bladeMat.color;
            color.a = 0;
            bladeMat.color = color;
            bladePhys.SetActive(false);
        }

        
        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);
            if (base.IsHeld && this.m_hand.Input.TriggerDown && this.m_hasTriggeredUpSinceBegin)
            {
                //TODO TOGGLE BLADE STATE
                if (!isLit)
                {
                    if (isdebug) Debug.Log("turn on bitch");
                    isLit = true;
                    stable = false;
                }
                else
                {
                    if (isdebug) Debug.Log("turn off bitch");
                    isLit = false;
                    stable = false;
                }
            }
        }
        

        private void Update()
        {
            if (!stable)
            {
                if (isdebug) Debug.Log("now unstable");
                if (isdebug) Debug.Log("islerping"+ isLerping);
                if (isdebug) Debug.Log("islit" + isLit);
                if (isLit)
                {
                    if (!isLerping)  //lerping "open"
                    {
                        Color color = bladeMat.color;
                        color.a = 0;
                        bladeMat.color = color;
                        if (isdebug) Debug.Log("roatethinggoooo");
                        startTime = Time.time;
                        journeyLength = 1 - bladeMat.color.a ;
                        isLerping = true;
                        if (retractSound != null)
                        {
                            SM.PlayGenericSound(igniteSound, transform.position);
                        }
                    }
                    if (isLerping)
                    {
                        float distCovered = (Time.time - startTime) * lerpSpeed;
                        float fractionOfJourney = distCovered / journeyLength;
                        Color color = bladeMat.color;
                        color.a = Mathf.Lerp(bladeMat.color.a, 1, fractionOfJourney);
                        bladeMat.color = color;
                        if(fractionOfJourney >= .5)
                        {
                            bladePhys.SetActive(true);
                        }
                        if (fractionOfJourney >= 1)
                        {
                            isLerping = false;
                            isLit = true;
                            stable = true;
                        }
                    }
                }
                else
                {
                    if (!isLerping)  //lerping "closed"
                    {
                        Color color = bladeMat.color;
                        color.a = 1;
                        bladeMat.color = color;
                        if (isdebug) Debug.Log("roatethinggoooo");
                        startTime = Time.time;
                        journeyLength =  bladeMat.color.a;
                        isLerping = true;
                        if(retractSound != null)
                        {
                            SM.PlayGenericSound(retractSound, transform.position);
                        }
                    }
                    if (isLerping)
                    {
                        float distCovered = (Time.time - startTime) * lerpSpeed;
                        float fractionOfJourney = distCovered / journeyLength;
                        Color color = bladeMat.color;
                        color.a = Mathf.Lerp(bladeMat.color.a, 0, fractionOfJourney);
                        bladeMat.color = color;
                        if (fractionOfJourney >= .5)
                        {
                            bladePhys.SetActive(false);
                        }
                        if (fractionOfJourney >= 1)
                        {
                            isLerping = false;
                            isLit = false;
                            stable = true;
                        }
                    }
                }
            }

        }










#endif

    }
}



