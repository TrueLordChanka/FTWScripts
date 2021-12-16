using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;


namespace Plugin.src
{ 

    [RequireComponent (typeof(PMat))]
    public class BreakableArmour : MonoBehaviour, IFVRDamageable
    {

        public MatDef defaultMatdef;
        public MatDef brokenMatdef;
        public float durability;
        public AudioEvent shatter;

        private bool playedaudio = false;
        private float damageTaken;

#if !(UNITY_EDITOR || UNITY_5)
        public void Start()
        {
            GetComponent<PMat>().MatDef = defaultMatdef;
        }

        public void Update()
        {
            
            if(damageTaken >= durability && !playedaudio)
            {
                GetComponent<PMat>().MatDef = brokenMatdef;
                SM.PlayGenericSound(shatter,this.transform.position);
                playedaudio = true;
            }

        }
		
        public void Damage(Damage d)
        {
            damageTaken = damageTaken + d.Dam_TotalKinetic; 
        }
#endif
    }
}



