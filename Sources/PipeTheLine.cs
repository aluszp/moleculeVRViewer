using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PipeTheLine : MonoBehaviour
{

    //public GameObject pipePrefab;
    public Vector3[] points;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    float radiusX;
    float radiusY;

    public void DrawThePipe(Vector3[] pointsOfLine, GameObject pipePrefab, float givenRadiusX, float givenRadiusY)
    {
        GameObject pipe = (GameObject)Instantiate(pipePrefab, Vector3.zero, Quaternion.identity);
        points = pointsOfLine;
        pipe.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
        radiusX = givenRadiusX;
        radiusY = givenRadiusY;
        SetVertices();
        SetTriangles();
        mesh.RecalculateNormals();
        
    }

    private void SetVertices()
    {

        vertices = new Vector3[(10 * 4 * (points.Length)) + 22];
        CreateFirstQuadRing();
        int iDelta = 10 * 4;
        for (int pointIndex = 2, i = iDelta; pointIndex < points.Length; pointIndex++, i += iDelta)
        {
            CreateQuadRing(pointIndex, i);
        }
        vertices[10 * 4 * (points.Length)] = points[0];
        vertices[10 * 4 * (points.Length) + 1] = points[points.Length - 1];

        float angleStep = (2f * Mathf.PI) / 10;
        for (int angle = 0; angle < 10; angle++)
        {
            Vector3 beginningPoint = GetPointOfVertix(points[0], angle * angleStep);
            vertices[(10 * 4 * (points.Length)) + 2 + angle] = beginningPoint;
            Vector3 endingPoint = GetPointOfVertix(points[points.Length - 1], angle * angleStep);
            vertices[(10 * 4 * (points.Length)) + 12 + angle] = endingPoint;
        }


        foreach (Vector3 vertix in vertices)
        { print(vertix); }
        mesh.vertices = vertices;
    }

    private void CreateFirstQuadRing()
    {
        float angleStep = (2f * Mathf.PI) / 10;


        Vector3 vertexA = GetPointOfVertix(points[1], 0);
        Vector3 vertexB = GetPointOfVertix(points[0], 0);

        for (int angle = 1, i = 0; angle <= 10; angle++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOfVertix(points[1], angle * angleStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOfVertix(points[0], angle * angleStep);
        }

    }

    //private void CreateSecondQuadRing()
    //{
    //    float angleStep = (2f * Mathf.PI) / 10;
    //    Vector3[] points = { new Vector3(0, 0, 0), new Vector3(0, 0, 2), new Vector3(0, 0, 4), new Vector3(0, 0, 6) };

    //    Vector3 vertexC = GetPointOfVertix(points[2], 0);
    //    Vector3 vertexD = GetPointOfVertix(points[1], 0);

    //    for (int angle = 1, i = 40; angle <= 10; angle++, i += 4)
    //    {
    //        vertices[i] = vertexC;
    //        vertices[i + 1] = vertexC = GetPointOfVertix(points[2], angle * angleStep);
    //        vertices[i + 2] = vertexD;
    //        vertices[i + 3] = vertexD = GetPointOfVertix(points[1], angle * angleStep);
    //    }
    //    foreach (Vector3 vertix in vertices)
    //    { print(vertix); }
    //}

    private void CreateQuadRing(int pointIndex, int i)
    {

        float angleStep = (2f * Mathf.PI) / 10;
        int ringOffset = 10 * 4;


        for (int angle = 0; angle <= 10; angle++, i += 4)
        {
            vertices[i] = GetPointOfVertix(points[pointIndex], angle * angleStep);
            vertices[i + 1] = GetPointOfVertix(points[pointIndex], (angle + 1) * angleStep);
            vertices[i + 2] = vertices[i - ringOffset];
            vertices[i + 3] = vertices[i - ringOffset + 1];
        }
    }

    private void SetTriangles()
    {

        triangles = new int[(10 * 6 * (points.Length)) + 60];
        for (int t = 0, i = 0; t < triangles.Length - 60; t += 6, i += 4)
        {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 1;
            triangles[t + 2] = triangles[t + 3] = i + 2;
            triangles[t + 5] = i + 3;
        }

        for (int ti = 0, vi = 2; ti < 30; ti += 3, vi++)
        {
            if (ti != 27)
            {
                triangles[(10 * 6 * (points.Length)) + ti] = (10 * 4 * (points.Length)) + vi;
                triangles[(10 * 6 * (points.Length)) + ti + 1] = (10 * 4 * (points.Length)) + vi + 1;
                triangles[(10 * 6 * (points.Length)) + ti + 2] = 10 * 4 * (points.Length);
            }
            else
            {
                triangles[(10 * 6 * (points.Length)) + ti] = (10 * 4 * (points.Length)) + vi;
                triangles[(10 * 6 * (points.Length)) + ti + 1] = (10 * 4 * (points.Length)) + 2;
                triangles[(10 * 6 * (points.Length)) + ti + 2] = 10 * 4 * (points.Length);
            }
        }

        for (int ti = 0, vi = 12; ti < 30; ti += 3, vi++)
        {
            if (ti != 27)
            {
                triangles[(10 * 6 * (points.Length)) + 30 + ti + 1] = (10 * 4 * (points.Length)) + vi;
                triangles[(10 * 6 * (points.Length)) + 30 + ti] = (10 * 4 * (points.Length)) + vi + 1;
                triangles[(10 * 6 * (points.Length)) + 30 + ti + 2] = (10 * 4 * (points.Length)) + 1;
            }
            else
            {
                triangles[(10 * 6 * (points.Length)) + 30 + ti + 1] = (10 * 4 * (points.Length)) + vi;
                triangles[(10 * 6 * (points.Length)) + 30 + ti] = (10 * 4 * (points.Length)) + 12;
                triangles[(10 * 6 * (points.Length)) + 30 + ti + 2] = (10 * 4 * (points.Length)) + 1;
            }
        }





        mesh.triangles = triangles;
    }

    private Vector3 GetPointOfVertix(Vector3 point, float angle)
    {
        Vector3 p;

        p.x = point.x + radiusX * Mathf.Sin(angle);
        p.y = point.y + radiusY * Mathf.Cos(angle);
        p.z = point.z;
        return p;
    }

}

