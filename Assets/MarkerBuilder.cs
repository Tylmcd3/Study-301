using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Elixir]
public class MarkerBuilder : MonoBehaviour
{
    public float beamLength = 0.1f;
    public float markerMaxSize = 4.0f;
    public float markerMinSize = 2.5f;
    public int markerWandEndScale = 5;
    public int numberOfSegments = 16;
    public float PenBaseSize = 0.018f;
    public float PenTipSize = 0.1f;

    public Shader shader;
    public Material Transparent;

    public void GeneratePenTip(Transform tip, Color colour)
    {
        MeshRenderer TipRenderer = tip.GetComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        tip.GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[3 * (numberOfSegments + 1)];
        int[] triangles = new int[3 * 5 * (numberOfSegments + 1)];

        // Generate vertices
        for (int i = 0; i < numberOfSegments; i++)
        {

            float angle = 2 * 3.14159265f * i / numberOfSegments;
            float x1 = (markerMaxSize / 200) * Mathf.Cos(angle);
            float z1 = (markerMaxSize / 200) * Mathf.Sin(angle);

            float x2 = (markerMinSize / 200) * Mathf.Cos(angle);
            float z2 = (markerMinSize / 200) * Mathf.Sin(angle);

            vertices[i] = new Vector3(x1, 0, z1);
            vertices[i + numberOfSegments] = new Vector3(x1, PenBaseSize, z1);
            vertices[i + (numberOfSegments * 2)] = new Vector3(x2, PenTipSize, z2);
        }
        vertices[numberOfSegments * 3] = new Vector3(0, PenTipSize, 0);

        // Generate triangles
        int vertIndex = 0;
        int triIndex = 0;
        for (int i = 0; i < numberOfSegments; i++)
        {
            //Base Ring
            triangles[triIndex] = i;
            triangles[triIndex + 1] = numberOfSegments + (i + 1) % numberOfSegments;
            triangles[triIndex + 2] = (i + 1) % numberOfSegments;

            triangles[triIndex + 3] = i;
            triangles[triIndex + 4] = numberOfSegments + i;
            triangles[triIndex + 5] = numberOfSegments + ((i + 1) % numberOfSegments);

            //Ring to Tip
            triangles[triIndex + 6] = numberOfSegments + i;
            triangles[triIndex + 7] = (numberOfSegments * 2) + (i + 1) % numberOfSegments;
            triangles[triIndex + 8] = numberOfSegments + (i + 1) % numberOfSegments;

            triangles[triIndex + 9] = numberOfSegments + i;
            triangles[triIndex + 10] = (numberOfSegments * 2) + i;
            triangles[triIndex + 11] = (numberOfSegments * 2) + ((i + 1) % numberOfSegments);

            //Tip
            triangles[triIndex + 12] = (numberOfSegments * 2) + i;
            triangles[triIndex + 13] = numberOfSegments * 3;
            triangles[triIndex + 14] = (numberOfSegments * 2) + ((i + 1) % numberOfSegments);

            vertIndex++;
            triIndex += 15;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Create a new material and apply it to the mesh renderer
        Material material = new Material(shader);
        material.color = colour;
        TipRenderer.material = material;

        TipRenderer.GetComponent<MeshCollider>().sharedMesh = mesh;

    }
    public void GenerateDepthWand(Transform tip, Transform wand, Color colour)
    {
        MeshRenderer WandRenderer = wand.GetComponent<MeshRenderer>();

        wand.localPosition = tip.localPosition + new Vector3(0, PenTipSize, 0);
        Mesh mesh = new Mesh();
        wand.gameObject.GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[2 * (numberOfSegments + 1)];
        int[] triangles = new int[3 * 3 * numberOfSegments];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];

        // Generate vertices
        for (int i = 0; i < numberOfSegments; i++)
        {
            float angle = 2 * 3.14159265f * i / numberOfSegments;
            float x1 = (markerMinSize / 200) * Mathf.Cos(angle);
            float z1 = (markerMinSize / 200) * Mathf.Sin(angle);

            float x2 = ((markerMinSize / 200) / markerWandEndScale) * Mathf.Cos(angle);
            float z2 = ((markerMinSize / 200) / markerWandEndScale) * Mathf.Sin(angle);
            vertices[i] = new Vector3(x1, 0, z1);
            vertices[i + numberOfSegments] = new Vector3(x2, beamLength, z2);
        }

        vertices[numberOfSegments * 2] = new Vector3(0, beamLength, 0);

        // Generate triangles
        int vertIndex = 0;
        int triIndex = 0;
        for (int i = 0; i < numberOfSegments; i++)
        {
            //Base Ring
            triangles[triIndex] = i;
            triangles[triIndex + 1] = numberOfSegments + (i + 1) % numberOfSegments;
            triangles[triIndex + 2] = (i + 1) % numberOfSegments;

            triangles[triIndex + 3] = i;
            triangles[triIndex + 4] = numberOfSegments + i;
            triangles[triIndex + 5] = numberOfSegments + ((i + 1) % numberOfSegments);

            //Tip
            triangles[triIndex + 6] = numberOfSegments + i;
            triangles[triIndex + 7] = numberOfSegments * 2;
            triangles[triIndex + 8] = numberOfSegments + ((i + 1) % numberOfSegments);


            vertIndex++;
            triIndex += 9;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Create a new material and apply it to the mesh renderer
        Material mat = new Material(Transparent);
        mat.color = colour;
        WandRenderer.material = mat;
    }
}
