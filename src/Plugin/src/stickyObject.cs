using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;

namespace Plugin.src
{
    class stickyObject : MonoBehaviour
    {

        //public proxGrenade banger;
        //public bool armedToStick;
        //public Rigidbody stickyBody;


        //void Start()
        //{

        //    //Debug.Log("Starting Sticky Object");
        //}

        //void Update()
        //{

            //if (banger.m_hand != null)
            //{
            //    if (banger.m_isArmed)
            //    {
            //        if (banger.m_j == null)
            //        {
            //            banger.m_hasJoint = false;
            //        }
            //        if (banger.m_hasJoint && banger.m_j.connectedBody == null)
            //        {
            //            UnityEngine.Object.Destroy(banger.m_j);
            //            banger.m_isStuck = false;
            //        }
            //    }

            //}


        //    void OnCollisionEnter(Collision collision)
        //    {
        //        //Debug.Log("Collided with somthing");
        //        foreach (ContactPoint contact in collision.contacts)
        //        {

        //            if (armedToStick == true)
        //            {
        //                //Debug.Log("ArmedToStick has passed");
        //                if (collision.relativeVelocity.magnitude > 1 && banger.m_isArmed == true)
        //                {
        //                    //Antons code
        //                    stickyBody.isKinematic = true;
        //                    if (collision.collider.attachedRigidbody != null)
        //                    {
        //                        banger.m_j = base.gameObject.AddComponent<FixedJoint>();
        //                        banger.m_j.connectedBody = collision.collider.attachedRigidbody;
        //                        banger.m_j.enableCollision = false;
        //                        banger.m_hasJoint = true;
        //                    }
        //                    else
        //                    {
        //                        banger.RootRigidbody.isKinematic = true;
        //                    }//end Antons code

        //                }
        //                else if (collision.relativeVelocity.magnitude > 3 && banger.m_isArmed == false)
        //                {
        //                    //Debug.Log("Hit Hard & NOT Armed");
        //                }
        //                else
        //                {
        //                    //Debug.Log("Didnt hit hard");
        //                }
        //            }
        //            else
        //            {
        //                //Debug.Log("ArmedToStick has failed");
        //                if (collision.relativeVelocity.magnitude > 3)
        //                {
        //                    //Antons code
        //                    stickyBody.isKinematic = true;
        //                    if (collision.collider.attachedRigidbody != null)
        //                    {
        //                        banger.m_j = base.gameObject.AddComponent<FixedJoint>();
        //                        banger.m_j.connectedBody = collision.collider.attachedRigidbody;
        //                        banger.m_j.enableCollision = false;
        //                        banger.m_hasJoint = true;
        //                    }
        //                    else
        //                    {
        //                        banger.RootRigidbody.isKinematic = true;
        //                    } //end Antons code
        //                }
        //                else
        //                {
        //                    // Debug.Log("Didnt hit hard");
        //                }
        //            }


        //        }
        //    }

        //}
    }
}
