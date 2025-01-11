using UnityEngine;
using UdonSharp;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using System;

public class OBJParser : BaseUdonCoroutine
{
    public Mesh Mesh { get; private set; }

    private Vector3[] vertices;
    private Vector2[] uvs;
    private Vector3[] normals;
    private int[] triangles;

    private int vertexIndex = 0;
    private int uvIndex = 0;
    private int normalIndex = 0;
    private int triangleIndex = 0;

    private string[] objLines;
    private int counter = 0;

    /// <summary>
    /// Singleton helper class that provides a static OBJParser instance
    /// </summary>
    /// <param name="objData"></param>
    /// <returns></returns>
    public void ImportOBJ(string objData)
    {
        vertexIndex = 0;
        uvIndex = 0;
        normalIndex = 0;
        triangleIndex = 0;
        counter = 0;
        objLines = objData.Split('\n');

        int max = CountVertices(objLines);
        vertices = new Vector3[max];
        uvs = new Vector2[max];
        normals = new Vector3[max];
        triangles = new int[max];
    }

    protected override void Setup()
    {
        base.Setup(); 
        Debug.Log("Started OBJ parse coroutine...");
    }

    protected override bool Tick()
    {
        if (counter < objLines.Length)
        {
            string[] tokens = objLines[counter].Split(' ');

            if (tokens[0] == "v")
            {
                ParseVertex(tokens);
            }
            else if (tokens[0] == "vt")
            {
                ParseUV(tokens);
            }
            else if (tokens[0] == "vn")
            {
                ParseNormal(tokens);
            }
            else if (tokens[0] == "f")
            {
                ParseFace(tokens);
            }

            Debug.Log($"Progress: {counter}/{objLines.Length}");
            counter++;

            return false;
        }

        Mesh = new Mesh();
        Mesh.vertices = vertices;
        Mesh.uv = uvs;
        Mesh.normals = normals;
        Mesh.triangles = triangles;
        
        Mesh.RecalculateBounds();
        Mesh.RecalculateNormals();
        Mesh.RecalculateTangents();

        return true;
    }

    protected override void OnCompletion()
    {
        base.OnCompletion();
        Debug.Log("Finished OBJ parse coroutine!");
    }

    private int CountVertices(string[] objLines)
    {
        int vertexCount = 0;
        int uvCount = 0;
        int normalCount = 0;
        int triangleCount = 0;

        foreach (string line in objLines)
        {
            if (line.StartsWith("v "))
                vertexCount++;
            if (line.StartsWith("vt "))
                uvCount++;
            if (line.StartsWith("vn "))
                normalCount++;
            if (line.StartsWith("f "))
            {
                string[] tokens = line.Split(' ');
                triangleCount += tokens.Length - 1;
            }
        }

#if UNITY_EDITOR
        Debug.Log($"Parsed mesh with {vertexCount} vertices");
        Debug.Log($"Parsed mesh with {uvCount} UVs");
        Debug.Log($"Parsed mesh with {normalCount} normals");
        Debug.Log($"Parsed mesh with {triangleCount} triangles");
#endif

        return Mathf.Max(vertexCount, uvCount, normalCount, triangleCount);
    }


    private void ParseVertex(string[] tokens)
    {
        float x = float.Parse(tokens[1]);
        float y = float.Parse(tokens[2]);
        float z = float.Parse(tokens[3]);
        vertices[vertexIndex] = new Vector3(x, y, z);
        vertexIndex++;
    }

    private void ParseUV(string[] tokens)
    {
        float u = float.Parse(tokens[1]);
        float v = float.Parse(tokens[2]);
        uvs[uvIndex] = new Vector2(u, v);
        uvIndex++;
    }

    private void ParseNormal(string[] tokens)
    {
        float nx = float.Parse(tokens[1]);
        float ny = float.Parse(tokens[2]);
        float nz = float.Parse(tokens[3]);
        normals[normalIndex] = new Vector3(nx, ny, nz);
        normalIndex++;
    }

    private void ParseFace(string[] tokens)
    {
        for (int i = 1; i < tokens.Length; i++)
        {
            string[] faceData = tokens[i].Split('/');

            int vIndex = int.Parse(faceData[0]) - 1;
            int uvIndex = int.Parse(faceData[1]) - 1;
            int nIndex = int.Parse(faceData[2]) - 1;

            triangles[triangleIndex] = vIndex;
            triangleIndex++;

            uvs[vIndex] = uvs[uvIndex];
            normals[vIndex] = normals[nIndex];
        }
    }
}