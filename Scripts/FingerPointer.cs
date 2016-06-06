using UnityEngine;
using System.Collections;

public class FingerPointer : MonoBehaviour
{

    Ray ray;
    float lengthOfHit;
    RaycastHit hit;

    void Update()
    {
        print("zaczynam");
        lengthOfHit = 200;
        ray = new Ray(transform.position, transform.rotation * Vector3.forward * lengthOfHit);

        if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "atom"))
        {
            print("wzkazales na atom!!!");
        }
    }
}
