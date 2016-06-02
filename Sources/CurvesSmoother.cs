using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurvesSmoother : MonoBehaviour
{
    //arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
    public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
    {
        
        List<Vector3> curvedPoints = new List<Vector3>();
        
        Vector3 p0, p1, m0, m1;
        
        for (int j = 0; j < arrayToCurve.Length - 1; j++)
   {
            
            // determine control points of segment
            p0 = arrayToCurve[j];
            p1 = arrayToCurve[j+1];
            curvedPoints.Add(p0);
            if (j > 0)
            {
                m0 = (arrayToCurve[j + 1] - arrayToCurve[j - 1])/2;
            }
            else
            {
                m0 = arrayToCurve[j + 1]- arrayToCurve[j];
            }
            if (j < arrayToCurve.Length - 2)
            {
                m1 = (arrayToCurve[j + 2] - arrayToCurve[j])/2;
            }
            else
            {
                m1 = arrayToCurve[j + 1]- arrayToCurve[j];
            }

            // set points of Hermite curve
            Vector3 position;
            float t;
            float pointStep = 1 / smoothness;

            if (j == arrayToCurve.Length - 2)
            {
                pointStep = 1 / (smoothness - 1);
                // last point of last segment should reach p1
            }
            for (int i = 0; i < smoothness; i++) 
      {
                t = i * pointStep;
                position = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0
                   + (t * t * t - 2.0f * t * t + t) * m0
                   + (-2.0f * t * t * t + 3.0f * t * t) * p1
                   + (t * t * t - t * t) * m1;
                curvedPoints.Add(position);
            }
            curvedPoints.Add(p1);
        }
        
    


        return (curvedPoints.ToArray());
    }
}