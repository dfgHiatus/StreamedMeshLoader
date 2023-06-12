using UnityEngine;
using UdonSharp;

public class OBJParser : UdonSharpBehaviour
{
    /// <summary>
    /// Singleton helper class that provides a static OBJParser instance
    /// </summary>
    /// <param name="objData"></param>
    /// <returns></returns>
    public Mesh ImportOBJ(string objData)
    {
        string[] objLines = objData.Split('\n');

        int vertexCount = CountVertices(objLines);
        int uvCount = CountUVs(objLines);
        int normalCount = CountNormals(objLines);
        int triangleCount = CountTriangles(objLines);        
        int max = Mathf.Max(vertexCount, uvCount, normalCount, triangleCount);

        Vector3[] vertices = new Vector3[max];
        Vector2[] uvs = new Vector2[max];
        Vector3[] normals = new Vector3[max];
        int[] triangles = new int[max];

        int vertexIndex = 0;
        int uvIndex = 0;
        int normalIndex = 0;
        int triangleIndex = 0;

        foreach (string line in objLines)
        {
            string[] tokens = line.Split(' ');

            if (tokens[0] == "v")
            {
                ParseVertex(tokens, ref vertexIndex, vertices);
            }
            else if (tokens[0] == "vt")
            {
                ParseUV(tokens, ref uvIndex, uvs);
            }
            else if (tokens[0] == "vn")
            {
                ParseNormal(tokens, ref normalIndex, normals);
            }
            else if (tokens[0] == "f")
            {
                ParseFace(tokens, ref triangleIndex, uvs, normals, triangles);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }
 
    private int CountVertices(string[] objLines)
    {
        int count = 0;

        foreach (string line in objLines)
        {
            if (line.StartsWith("v "))
                count++;
        }

        Debug.Log($"Parsed mesh with {count} vertices");

        return count;
    }

    private int CountUVs(string[] objLines)
    {
        int count = 0;

        foreach (string line in objLines)
        {
            if (line.StartsWith("vt "))
                count++;
        }

        Debug.Log($"Parsed mesh with {count} UVs");

        return count;
    }

    private int CountNormals(string[] objLines)
    {
        int count = 0;

        foreach (string line in objLines)
        {
            if (line.StartsWith("vn "))
                count++;
        }

        Debug.Log($"Parsed mesh with {count} normals");
        return count;
    }

    private int CountTriangles(string[] objLines)
    {
        int count = 0;

        foreach (string line in objLines)
        {
            if (line.StartsWith("f "))
            {
                string[] tokens = line.Split(' ');
                count += tokens.Length - 1;
            }
        }

        Debug.Log($"Parsed mesh with {count} triangles");

        return count;
    }

    private void ParseVertex(string[] tokens, ref int index, Vector3[] vertices)
    {
        float x = float.Parse(tokens[1]);
        float y = float.Parse(tokens[2]);
        float z = float.Parse(tokens[3]);
        vertices[index] = new Vector3(x, y, z);
        index++;
    }

    private void ParseUV(string[] tokens, ref int index, Vector2[] uvs)
    {
        float u = float.Parse(tokens[1]);
        float v = float.Parse(tokens[2]);
        uvs[index] = new Vector2(u, v);
        index++;
    }

    private void ParseNormal(string[] tokens, ref int index, Vector3[] normals)
    {
        float nx = float.Parse(tokens[1]);
        float ny = float.Parse(tokens[2]);
        float nz = float.Parse(tokens[3]);
        normals[index] = new Vector3(nx, ny, nz);
        index++;
    }

    private void ParseFace(string[] tokens, ref int index, Vector2[] uvs, Vector3[] normals, int[] triangles)
    {
        for (int i = 1; i < tokens.Length; i++)
        {
            string[] faceData = tokens[i].Split('/');

            int vIndex = int.Parse(faceData[0]) - 1;
            int uvIndex = int.Parse(faceData[1]) - 1;
            int nIndex = int.Parse(faceData[2]) - 1;

            triangles[index] = vIndex;
            index++;

            uvs[vIndex] = uvs[uvIndex];
            normals[vIndex] = normals[nIndex];
        }
    }
}
