using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Sources
{
    class AtomInteraction: MonoBehaviour
    {
       
        void OnTriggerEnter(Collider c)
        {
            if (c.gameObject.tag.Equals("atom"))
            {
                print("TOUCHED");
                return;
            }
        }

        void OnTriggerStay(Collider c)
        {
            if (c.gameObject.tag.Equals("atom"))
            {
                print("TOUCHED");
                return;
            }
        }
    }
}
