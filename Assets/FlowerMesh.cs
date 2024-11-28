using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerMesh : MonoBehaviour
{
    public Vector3[] vertices;
    public int[] triangles;
    public int numTris;
    // Start is called before the first frame update
    void Start()
    {
        MakeFlower();
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        this.gameObject.GetComponent<MeshFilter>().mesh = mesh;
        this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0.95f,0.95f,0.95f,1f);
    }

    private void MakeFlower()
    {
        numTris = 0;
        vertices = new Vector3[16];

        //top
        vertices[0] = new Vector3(-0.167f, 0.09f, -0.167f);
        vertices[1] = new Vector3(0f, 0.22f, -0.5f);
        vertices[2] = new Vector3(0.167f, 0.09f, -0.167f);
        vertices[3] = new Vector3(0.5f, 0.22f, 0f);
        vertices[4] = new Vector3(0.167f, 0.09f, 0.167f);
        vertices[5] = new Vector3(0f, 0.22f, 0.5f);
        vertices[6] = new Vector3(-0.167f, 0.09f, 0.167f);
        vertices[7] = new Vector3(-0.5f, 0.22f, 0f);

        //bottom
        vertices[8] = new Vector3(-0.167f, 0.05f, -0.167f);
        vertices[9] = new Vector3(0f, 0.2f, -0.5f);
        vertices[10] = new Vector3(0.167f, 0.05f, -0.167f);
        vertices[11] = new Vector3(0.5f, 0.2f, 0f);
        vertices[12] = new Vector3(0.167f, 0.05f, 0.167f);
        vertices[13] = new Vector3(0f, 0.2f, 0.5f);
        vertices[14] = new Vector3(-0.167f, 0.05f, 0.167f);
        vertices[15] = new Vector3(-0.5f, 0.2f, 0f);

        triangles = new int[((6 * 2) + (8 * 2)) * 3];

        // top
        MakeTri(0, 2, 1);
        MakeTri(2, 4, 3);
        MakeTri(4, 6, 5);
        MakeTri(6, 0, 7);
        MakeQuad(0, 6, 4, 2);

        // bottom
        MakeTri(8, 9, 10);
        MakeTri(10, 11, 12);
        MakeTri(12, 13, 14);
        MakeTri(14, 15, 8);
        MakeQuad(8, 10, 12, 14);

        // sides
        MakeQuad(0, 1, 9, 8);
        MakeQuad(1, 2, 10, 9);
        MakeQuad(2, 3, 11, 10);
        MakeQuad(3, 4, 12, 11);
        MakeQuad(4, 5, 13, 12);
        MakeQuad(5, 6, 14, 13);
        MakeQuad(6, 7, 15, 14);
        MakeQuad(7, 0, 8, 15);
    }

    private void MakeQuad(int p1, int p2, int p3, int p4)
    {
		MakeTri (p1, p2, p3);
		MakeTri (p1, p3, p4);
    }
    private void MakeTri(int p1, int p2, int p3)
    {   
		int index = numTris * 3;
		numTris++;

		triangles[index] = p1;
		triangles[index + 1] = p2;
		triangles[index + 2] = p3;
	}
}
