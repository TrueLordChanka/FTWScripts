using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;


namespace Plugin.src
{ 

    
    public class ShatterableArmour : MonoBehaviour
    {

        public MatDef defaultMatdef;
        public MatDef brokenMatdef;
        public float durability;
        public AudioEvent shatter;
        public bool doesNotDestroyOnShatter;
        public System.Collections.Generic.List<ShatterableArmourPiece> ArmourParts;
        public GameObject shatterEffect;
        public float damageTaken;
        private bool playedaudio = false;
#if !(UNITY_EDITOR || UNITY_5)

        public void Start()
        {
            GM.CurrentSceneSettings.SosigKillEvent += KillCounter;
        }

        private void KillCounter(Sosig s)
        {

        }


        public void Update()
        {

            if(damageTaken >= durability && !playedaudio)
            {
                GetComponent<PMat>().MatDef = brokenMatdef;
                SM.PlayGenericSound(shatter,this.transform.position);
                playedaudio = true;
                if (doesNotDestroyOnShatter)
                {
                    foreach (var ShatterableArmourPiece in ArmourParts)
                    {
                        if (shatterEffect != null)
                        {
                            GameObject.Instantiate(shatterEffect, transform.position, transform.rotation);
                        }
                        ShatterableArmourPiece.Shatter();
                    }
                }
                else
                {
                    if (shatterEffect != null)
                    {
                        GameObject.Instantiate(shatterEffect, transform.position, transform.rotation);
                    }
                    DestroyObject(gameObject);
                }
            }

        }

#endif
    }
}



