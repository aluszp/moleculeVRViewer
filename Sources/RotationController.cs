using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Code.Scripts;

namespace Leap.Unity
{
    class RotationController: MonoBehaviour
    {
        public PinchDetector PinchDetectorA;
        public GameObject rightHand;
        
        bool flag = true;
        Vector3 oldPosition;


      void Update()
        {
            
            if (PinchDetectorA.IsPinching)
            {
                if (flag)
                {
                    oldPosition = rightHand.transform.localPosition;
                    flag = false;
                }
                transform.RotateAround(MoleculeViewer.target, transform.TransformDirection(Vector3.up), (rightHand.transform.localPosition.z - oldPosition.z)*50);

                if (Mathf.Abs(rightHand.transform.localPosition.x - oldPosition.x) < 0.005)
                {
                    transform.RotateAround(MoleculeViewer.target, transform.TransformDirection(Vector3.left), (rightHand.transform.localPosition.y - oldPosition.y) * 70);
                }

                if (Mathf.Abs(Vector3.Distance(transform.localPosition, MoleculeViewer.target)) < 100)
                {
                    transform.Translate(Vector3.forward * (rightHand.transform.localPosition.x - oldPosition.x) * 30);
                }
            }

          
            else
            {
                flag = true;
            }
        }

    }
}
