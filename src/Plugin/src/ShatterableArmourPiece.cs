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
    public class ShatterableArmourPiece : MonoBehaviour, IFVRDamageable
    {

        public MatDef defaultMatdef;
        public MatDef brokenMatdef;
        public float durability;
        public AudioEvent shatter;
        public ShatterableArmour ParentArmour;

        public float damageTaken;

#if !(UNITY_EDITOR || UNITY_5)
        public void Start()
        {
            GetComponent<PMat>().MatDef = defaultMatdef;
        }

        public void Damage(Damage d)
        {
            ParentArmour.damageTaken += d.Dam_TotalKinetic;
        }

        public void Shatter()
        {
            GetComponent<PMat>().MatDef = brokenMatdef;
        }
#endif
    }
}



