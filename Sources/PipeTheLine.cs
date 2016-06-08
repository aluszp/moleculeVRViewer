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
    Vector2[] uvs;
    float radiusX;
    float radiusY;
    int pointsOfRing;
    const int quadVertices = 4;

    public void DrawThePipe(Vector3[] pointsOfLine, GameObject pipe, float givenRadiusX, float givenRadiusY)
    {
        pointsOfRing = 10;
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

        vertices = new Vector3[(pointsOfRing * quadVertices * points.Length)]; 
        CreateFirstQuadRing();
        int iDelta = pointsOfRing * quadVertices;
        for (int pointIndex = 2, i = iDelta; pointIndex < points.Length; pointIndex++, i += iDelta)
        {
            CreateQuadRing(pointIndex, i);
        }

        
        mesh.vertices = vertices;
    }

    private void CreateFirstQuadRing()
    {
        float angleStep = (2f * Mathf.PI) / pointsOfRing;


        Vector3 vertexA = GetPointOfVertix(points[1], 0);
        Vector3 vertexB = GetPointOfVertix(points[0], 0);

        for (int angle = 1, i = 0; angle <= pointsOfRing; angle++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOfVertix(points[1], angle * angleStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOfVertix(points[0], angle * angleStep);
        }

    }

    private void CreateQuadRing(int pointIndex, int i)
    {

        float angleStep = (2f * Mathf.PI) / pointsOfRing;
        int ringOffset = pointsOfRing * quadVertices;


        for (int angle = 0; angle <= pointsOfRing; angle++, i += quadVertices)
        {
            vertices[i] = GetPointOfVertix(points[pointIndex], angle * angleStep);
            vertices[i + 1] = GetPointOfVertix(points[pointIndex], (angle + 1) * angleStep);
            vertices[i + 2] = vertices[i - ringOffset];
            vertices[i + 3] = vertices[i - ringOffset + 1];
        }
    }

  

    private void SetTriangles()
    {

        triangles = new int[(pointsOfRing * 6 * (points.Length)) ];
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 1;
            triangles[t + 2] = triangles[t + 3] = i + 2;
            triangles[t + 5] = i + 3;
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

