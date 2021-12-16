using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;


namespace Plugin.src
{ //mathf.lerp
    public class proxGrenade : FVRPhysicalObject
    {

        public bool DoesUseHandedPose = true;

        private bool m_isInRightHandMode = true;

        private List<proxGrenadeRing> m_rings = new List<proxGrenadeRing>();


        [Header("Payload")]
        public List<GameObject> SpawnOnSplode;

        public SmokeSolidEmitter SmokeEmitter;

        [Header("Audio")]

        public AudioEvent AudEvent_Pinpull;

        public AudioEvent AudEvent_Armed;

        public ParticleSystem FusePSystem;

        public Transform FuseCylinder;

        private int m_fuseCylinderSetting = 2;

        private bool m_isPinPulled;

        private bool m_hasSploded;

        public int IFF;



        private bool is_Armed = false;
        private bool is_Set = false;


        private bool is_Stuck;


        private FixedJoint m_j;
        private bool m_hasJoint;

        [Header("Safe/Armed Renders")]
        public GameObject ArmedRender;
        public GameObject SafeRender;

        private bool beginCountdown;

        [Header("Misc Params")]
        public bool is_Sticky;
        public bool armedToStick;
        public float arm_After_Impact_Time;
        public float speed_to_Stick;
        public float ProxRange;
        public LayerMask LM_Prox;


        private float cooldown = 0;
        private bool onCooldown = false;
        private bool coolingDown = true;

        [Header("Single Point Rotation")]
        private bool hasExpanded;
        public Transform pointInterp1;
        public Transform pointInterp2;
        public float lerpSpeed = 1.0F;
        public GameObject lerpObject;
        private bool isLerping = false;
        private float startTime;
        private float journeyLength;


        float timer = 0;
        bool timerReached = false;
        bool iscounting = false;

#if!(UNITY_EDITOR||UNITY_5)
        public override void Awake()
        {
            base.Awake();
        }

        public void RegisterRing(proxGrenadeRing r)
        {
            this.m_rings.Add(r);

        }

        public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
        {
            GameObject gameObject = base.DuplicateFromSpawnLock(hand);
            proxGrenade component = gameObject.GetComponent<proxGrenade>();
            return gameObject;
        }

        public void ArmingSequence()
        {
            if (is_Armed == true)
            {
                iscounting = true;
                beginCountdown = true;
                timerReached = false;
                timer = 0;
            }
        }

        void Update()
        {
            if (beginCountdown)
            {
                if (!timerReached)
                {
                    timer += Time.deltaTime;
                    if (timer > arm_After_Impact_Time)
                    {
                        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Armed, base.transform.position);
                        is_Set = true;
                        timer = 0;
                        beginCountdown = false;
                        iscounting = false;
                        timerReached = true;
                    }
                }


            }
            /*
            if (onCooldown)
            {
                if (coolingDown)
                {
                    cooldown += Time.deltaTime;
                    if (cooldown > 0.5f)
                    {
                        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Armed, base.transform.position);
                        Debug.Log("Cooled Down");
                        cooldown = 0;
                        onCooldown = false;
                        coolingDown = false;

                    }
                }

            }
            */
            if (this.m_isPinPulled && !hasExpanded && !isLerping)
            {
                //Debug.Log("roatethinggoooo");
                startTime = Time.time;
                journeyLength = Vector3.Distance(pointInterp1.localPosition, pointInterp2.localPosition);
                isLerping = true;
                
            }

            if (isLerping)
            {
                float distCovered = (Time.time - startTime) * lerpSpeed;

                float fractionOfJourney = distCovered / journeyLength;
                //Debug.Log("fraction of journety" + fractionOfJourney + "dist covered" + distCovered + "objloc" + lerpObject.transform.localPosition) ;
                lerpObject.transform.localPosition = Vector3.Lerp(pointInterp1.localPosition, pointInterp2.localPosition, fractionOfJourney);
                if(fractionOfJourney >= 1)
                {
                    isLerping = false;
                    hasExpanded = true;
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
           //Debug.Log("Collided with somthing");
            foreach (ContactPoint contact in collision.contacts)
            {
                //ArmTickDown();
                if (armedToStick && is_Sticky )
                {
                    //Debug.Log("ArmedToStick has passed");
                    if (collision.relativeVelocity.magnitude > speed_to_Stick && this.is_Armed == true)
                    {
                        //Antons code
                        if (collision.collider.attachedRigidbody != null)
                        {

                            /*
                            this.m_j = base.gameObject.AddComponent<FixedJoint>();
                            this.m_j.connectedBody = collision.collider.attachedRigidbody;
                            this.m_j.enableCollision = false;
                            this.m_hasJoint = true;
                            is_Stuck = true;
                            ArmingSequence();
                            */
                        }
                        else 
                        {
                            this.RootRigidbody.isKinematic = true;
                            ArmingSequence();
                        }//end Antons code

                    }
                    else if (collision.relativeVelocity.magnitude > speed_to_Stick && this.is_Armed == false)
                    {
                        //Debug.Log("Hit Hard & NOT Armed");
                    }
                    else
                    {
                       // Debug.Log("Didnt hit hard");
                    }
                }
                else if (is_Sticky)
                {
                    //Debug.Log("ArmedToStick is false");
                    if (collision.relativeVelocity.magnitude > 3)
                    {
                        //Antons code
                        if (collision.collider.attachedRigidbody != null)
                        {
                            /*
                            this.m_j = base.gameObject.AddComponent<FixedJoint>();
                            this.m_j.connectedBody = collision.collider.attachedRigidbody;
                            this.m_j.enableCollision = false;
                            this.m_hasJoint = true;
                            is_Stuck = true;
                            if (!iscounting)
                            {
                                ArmingSequence();
                            }
                            */
                        }
                        else
                        {
                            this.RootRigidbody.isKinematic = true;
                            ArmingSequence();
                        } //end Antons code
                    }
                    else
                    {
                       // Debug.Log("Didnt hit hard");
                    }
                }
                //Debug.Log("the item isnt sticky");
                if (!armedToStick)
                {
                    ArmingSequence();
                }
            }
        }

       

        public override void FVRUpdate()
        {
            base.FVRUpdate();
            if (this.m_rings.Count > 0)
            {
                this.m_isPinPulled = true;
                for (int i = 0; i < this.m_rings.Count; i++)
                {
                    if (!this.m_rings[i].HasPinDetached())
                    {
                        this.m_isPinPulled = false;
                    }
                }
            }
            if (this.m_isPinPulled)
            {
                this.ArmedRender.SetActive(true);
                this.SafeRender.SetActive(false);
            }
            if (this.m_isPinPulled && !base.IsHeld && base.QuickbeltSlot == null)
            {
                is_Armed = true;
            }
            if (this.m_hand != null)
            {
                //Debug.Log("log1");
                base.RootRigidbody.isKinematic = false;
                if (this.m_j != null)
                {
                    Debug.Log("deleted m_j"); 
                   
                    UnityEngine.Object.Destroy(this.m_j);
                    onCooldown = true;
                }
                this.m_hasJoint = false;
                this.is_Stuck = false;
                this.is_Set = false;
                //if (is_Armed)
                //{
                //    Debug.Log("log2");
                //    if (this.m_j == null)
                //    {
                //        Debug.Log("log3");
                //        this.m_hasJoint = false;
                //    }
                //    if (this.m_hasJoint && this.m_j.connectedBody == null)
                //    {
                //        Debug.Log("log4");
                //        UnityEngine.Object.Destroy(this.m_j);
                //        is_Stuck = false;
                //    }
                //}

            }

            

            if (is_Armed && is_Set && Physics.CheckSphere(base.transform.position, this.ProxRange, this.LM_Prox, QueryTriggerInteraction.Ignore))
            {
                if (!this.m_hasSploded)
                {
                    this.m_hasSploded = true;
                    for (int j = 0; j < this.SpawnOnSplode.Count; j++)
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.SpawnOnSplode[j], base.transform.position, Quaternion.identity);
                        Explosion component = gameObject.GetComponent<Explosion>();
                        if (component != null)
                        {
                            component.IFF = this.IFF;
                        }
                        ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
                        if (component2 != null)
                        {
                            component2.IFF = this.IFF;
                        }
                        GrenadeExplosion component3 = gameObject.GetComponent<GrenadeExplosion>();
                        if (component3 != null)
                        {
                            component3.IFF = this.IFF;
                        }
                    }
                }

                if (base.IsHeld)
                {
                    FVRViveHand hand = this.m_hand;
                    this.m_hand.ForceSetInteractable(null);
                    this.EndInteraction(hand);
                }
                UnityEngine.Object.Destroy(base.gameObject);

            }
        }

        public override void BeginInteraction(FVRViveHand hand)
        {
            if (this.DoesUseHandedPose)
            {
                if (!hand.IsThisTheRightHand && this.m_isInRightHandMode)
                {
                    Vector3 vector = this.PoseOverride.up;
                    Vector3 vector2 = this.PoseOverride.forward;
                    vector = Vector3.Reflect(vector, base.transform.right);
                    vector2 = Vector3.Reflect(vector2, base.transform.right);
                    this.PoseOverride.rotation = Quaternion.LookRotation(vector2, vector);
                    vector = this.PoseOverride_Touch.up;
                    vector2 = this.PoseOverride_Touch.forward;
                    vector = Vector3.Reflect(vector, base.transform.right);
                    vector2 = Vector3.Reflect(vector2, base.transform.right);
                    this.PoseOverride_Touch.rotation = Quaternion.LookRotation(vector2, vector);
                    this.m_isInRightHandMode = false;
                }
                else if (hand.IsThisTheRightHand && !this.m_isInRightHandMode)
                {
                    Vector3 vector3 = this.PoseOverride.up;
                    Vector3 vector4 = this.PoseOverride.forward;
                    vector3 = Vector3.Reflect(vector3, base.transform.right);
                    vector4 = Vector3.Reflect(vector4, base.transform.right);
                    this.PoseOverride.rotation = Quaternion.LookRotation(vector4, vector3);
                    vector3 = this.PoseOverride_Touch.up;
                    vector4 = this.PoseOverride_Touch.forward;
                    vector3 = Vector3.Reflect(vector3, base.transform.right);
                    vector4 = Vector3.Reflect(vector4, base.transform.right);
                    this.PoseOverride_Touch.rotation = Quaternion.LookRotation(vector4, vector3);
                    this.m_isInRightHandMode = true;
                }
            }
            base.BeginInteraction(hand);
        }

        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);
            this.IFF = GM.CurrentPlayerBody.GetPlayerIFF();

        }
#endif

    }
	}



