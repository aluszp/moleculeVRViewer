using UnityEngine;
using System.Collections.Generic;

public class HelixPointsMaker : MonoBehaviour
{
    //arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
    public static Vector3[] MakeHelixCurve(float r, float slope, Vector3 startingPoint, Vector3 endingPoint)
    {
        
        float circumference = Mathf.PI * 2 * r;
        float arcLength = Mathf.Sqrt(Mathf.Pow(endingPoint.z-startingPoint.z, 2) + Mathf.Pow(circumference, 2));
        float T = arcLength / (Mathf.Sqrt(r*r + slope*slope));


        Vector3 directionVector = endingPoint - startingPoint;

        List<Vector3> helixPoints = new List<Vector3>();
        List<Vector3> newHelixPoints = new List<Vector3>();
        

        
        for (float t=0; t<=T; t += 0.1f)
        {
            if (t == 0)
            { helixPoints.Add(Vector3.zero); }

            else
            {
                Vector3 helixPoint = new Vector3(r * Mathf.Cos(2 * Mathf.PI * t), r * Mathf.Sin(2 * Mathf.PI * t), slope * t);
                helixPoints.Add(helixPoint);
            }            
        }

        helixPoints.Add(new Vector3(0, 0, directionVector.magnitude));
       
        Vector3 difference = (endingPoint - helixPoints[helixPoints.Count - 1]);
        foreach (Vector3 helixPoint in helixPoints)
        {
            newHelixPoints.Add(helixPoint + (difference * ((helixPoint - startingPoint).magnitude) / (helixPoints[helixPoints.Count - 1] - startingPoint).magnitude));
        }
        
        return (helixPoints.ToArray());
    }
}