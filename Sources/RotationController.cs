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
        public PinchDetector PinchDetectorB;
        public GameObject leftHand;
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
                    transform.RotateAround(MoleculeViewer.target, transform.TransformDirection(Vector3.left), (rightHand.transform.localPosition.y - oldPosition.y) * 100);
                }
                transform.Translate(Vector3.back* (rightHand.transform.localPosition.x - oldPosition.x) * 30);
                print("pozycja ręki: " + rightHand.transform.localPosition+" old: " + oldPosition + " delta: " + (rightHand.transform.localPosition.x-oldPosition.x));
            }

            else if (PinchDetectorB.IsPinching)
            {
                if (flag)
                {
                    oldPosition = leftHand.transform.position;
                    flag = false;
                }
                transform.RotateAround(MoleculeViewer.target, transform.TransformDirection(Vector3.up), (leftHand.transform.localPosition.z - oldPosition.z) * 50);
                transform.RotateAround(MoleculeViewer.target, transform.TransformDirection(Vector3.left), (leftHand.transform.localPosition.y - oldPosition.y) * 100);
                print("pozycja ręki: " + leftHand.transform.position);
                print("old: " + oldPosition);
            }

            else
            {
                flag = true;
            }
        }

    }
}
