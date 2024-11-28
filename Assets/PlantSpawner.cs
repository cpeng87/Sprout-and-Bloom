using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 position;
    public List<Bud> buds;
    public int order;
    public Vector3[] nodeVertices;
    public Node prevNode;
    public List<Node> nextNode;

    public Node(Vector3 position, int order, Node prevNode)
    {
        this.position = position;
        buds = new List<Bud>();
        this.order = order;
        this.prevNode = prevNode;
    }
    public void AddNextNode(Node nodeToAdd)
    {
        if (nextNode == null)
        {
            nextNode = new List<Node>();
            nextNode.Add(nodeToAdd);
        }
        else
        {
            nextNode.Add(nodeToAdd);
        }
    }
    public List<Node> GetConnectedNodes()
    {
        List<Node> nodes = nextNode;
        List<Node> rtn = new List<Node>();
        rtn.Add(this);

        if (nodes == null)
        {
            return rtn;
        }

        while (nodes.Count > 0)
        {
            Node currNode = nodes[0];
            nodes.RemoveAt(0);

            rtn.Add(currNode);

            if (currNode.nextNode != null)
            {
                foreach (Node next in currNode.nextNode)
                {
                    nodes.Add(next);
                }
            }
        }
        return rtn;
    }
    public void CalculateNodeVertices(Vector3 dir, int numQuads, Vector3 pos)
    {
        float radius = (Mathf.Pow(0.5f, order))/ 2f;
        float degreeIncrement = Mathf.Deg2Rad * (360.0f / numQuads);

        nodeVertices = new Vector3[numQuads];

        Vector3 xaxis = Vector3.Cross(dir, Vector3.right).normalized;
        Vector3 yaxis = Vector3.Cross(dir, xaxis).normalized;
        int vertexIndex = 0;

        for (int i = 0; i < numQuads; i++)
        {
            //Debug.Log("Node vertex:" + (this.position + (((xaxis * Mathf.Cos(i * degreeIncrement)) + (yaxis * Mathf.Sin(i * degreeIncrement))) * radius)));
            nodeVertices[vertexIndex] = 
            pos + (((xaxis * Mathf.Cos(i * degreeIncrement)) + (yaxis * Mathf.Sin(i * degreeIncrement))) * radius);
            vertexIndex++;
        }
    }
}

public class Bud
{
    public float dieProbability;
    public float growthProbability;
    public int age;
    public int order;
    public bool isDead;
    public Vector3 direction;  //normalized vector

    public Bud(float growthProbability, Vector3 direction)
    {
        this.growthProbability = growthProbability;
        isDead = false;
        this.direction = direction;
    }
    public Bud(float growthProbability, Vector3 direction, int order)
        : this(growthProbability, direction)
    {
        this.order = order;
    }
    public void SetDead()
    {
        isDead = true;
    }
    public void CalculateDieProbability()
    {
        // dieProbability = (dieProbability) + (order * 0.03f) + (age * 0.02f);
        dieProbability = (order * 0.05f) + (age * 0.05f);
    }
    public void IncrementAge()
    {
        age++;
    }
}

public class Internode
{
    public Node start;
    public Node end;
    public int numQuads;
    public Vector3[] vertices;
    public int[] triangles;
    private int numTris;
    public GameObject internodeObj;
    public Color woodColor;
    public Transform parentTransform;
    public Vector2[] uvs;

    public Internode(Node start, Node end, int numQuads, Color woodColor, Transform parentTransform)
    {
        this.start = start;
        this.end = end;
        this.numQuads = numQuads;
        this.woodColor = woodColor;
        this.parentTransform = parentTransform;
    }

    public void CreateInternode()
    {
        SetupMesh();
        
        // Create the GameObject for the internode
        internodeObj = new GameObject("Internode");
        internodeObj.transform.SetParent(parentTransform);
        
        // Add necessary components
        internodeObj.AddComponent<MeshFilter>();
        internodeObj.AddComponent<MeshRenderer>();
        internodeObj.AddComponent<Cuttable>();
        internodeObj.GetComponent<Cuttable>().plant = parentTransform.gameObject.GetComponent<PlantSpawner>();
        internodeObj.GetComponent<Cuttable>().endNode = end;
        internodeObj.GetComponent<Cuttable>().startNode = start;

        // Create the mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        
        // Assign the mesh to the MeshFilter
        internodeObj.GetComponent<MeshFilter>().mesh = mesh;

        // Set up texture
        Texture2D texture = SetupTexture();
        internodeObj.GetComponent<MeshRenderer>().material.mainTexture = texture;

        // Assign color to the material
        internodeObj.GetComponent<MeshRenderer>().material.color = woodColor;

        // Add a MeshCollider and assign the mesh to it
        MeshCollider meshCollider = internodeObj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = false;  // Set to true if using in physics and require convex shape for interactions
    }

    //creates branch mesh
    public void SetupMesh()
    {
        numTris = 0;
        vertices = new Vector3[numQuads * 2];
        uvs = new Vector2[numQuads * 2];

        // T- tangent to curve (direction)
        // B- binormal (perpendicular to both T and N)
        // N- normal (perpendicular to dir T)

        Vector3 difference = (end.position - start.position).normalized;   //T
        Vector3 xaxis = Vector3.Cross(difference, Vector3.right).normalized;  //N
        Vector3 yaxis = Vector3.Cross(difference, xaxis).normalized;  //B
        int vertexIndex = 0;

        if (start.nodeVertices == null)
        {
            start.CalculateNodeVertices(difference, numQuads, start.position);
        }
        if (end.nodeVertices == null)
        {
            end.CalculateNodeVertices(difference, numQuads, end.position);
        }

        for (int i = 0; i < numQuads; i++)
        {
            vertices[vertexIndex] = start.nodeVertices[i];
            uvs[vertexIndex] = new Vector2((float)i / numQuads, 0);
            vertexIndex++;

            vertices[vertexIndex] = end.nodeVertices[i];
            uvs[vertexIndex] = new Vector2((float)i / numQuads, 0);
            vertexIndex++;
        }

        triangles = new int[(numQuads + vertices.Length) * 2 * 3];
        for (int i = 0; i < numQuads * 2; i++)
        {
            MakeQuad(i, (i + 3) % vertices.Length, (i + 1) % vertices.Length, (i + 2) % vertices.Length);
            //Debug.Log("Making quad with: " + vertices[i] + " , " + vertices[(i + 1) % vertices.Length] + " , " + vertices[(i + 2) % vertices.Length] + " , " + vertices[(i + 3) % vertices.Length]);
            i++;
        }

        //ends
        for (int i = 2; i < vertices.Length - 3; i++)
        {
            MakeTri(0, i, i + 2);
            MakeTri(1, i + 1, i + 3);
            i++;
        }
    }

    private Texture2D SetupTexture() 
    {
        int texture_width = 100;
        int texture_height = 100;
        int scale = 20;
        Texture2D texture = new Texture2D (texture_width, texture_height);
        Color[] colors = new Color[texture_width * texture_height];
        // create the Perlin noise pattern in "colors"
        for (int i = 0; i < texture_width; i++)
        for (int j = 0; j < texture_height; j++) {
        float x = scale * i / (float) texture_width;
        float y = scale * j / (float) texture_height;
        float t = Mathf.PerlinNoise (x, y);
        // Perlin noise!
        colors [j * texture_width + i] = new Color (t, t, t, 1.0f);
        // gray scale values (r = g = b)
        }
        // copy the colors into the texture
        texture.SetPixels(colors);
        // do texture specific stuff, probably including making the mipmap levels
        texture.Apply();
        // return the texture
        return (texture);
    }

    //fix ordering???
    private void MakeQuad(int p1, int p2, int p3, int p4)
    {
		MakeTri (p1, p4, p3);
		MakeTri (p2, p3, p4);
    }

    private void MakeTri(int p1, int p2, int p3)
    {
        if (vertices == null || triangles == null)
        {
            Debug.Log("Vertices or triangles are null :<");
        }
        
		int index = numTris * 3;
		numTris++;

        if (p1 >= vertices.Length || p3 >= vertices.Length || p3 >= vertices.Length)
        {
            Debug.Log("one is longer");
        }

		triangles[index] = p1;
		triangles[index + 1] = p2;
		triangles[index + 2] = p3;
	}
}

public class PlantSpawner : MonoBehaviour
{
    public int seed;
    public float growthProbability;
    public float branchProbability;
    public int iterations;
    public List<Internode> internodes;
    public List<Node> nodes;
    public float growthLength;
    public int numQuads;
    // public GameObject sphere;
    public bool noisyGrowth;
    public Color woodColor;
    public bool isFlowering;
    public GameObject flower;
    public float growthLengthFactor;
    public List<GameObject> nodeObjs;
    public GameObject leafCluster;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(seed);
        nodes = new List<Node>();
        internodes = new List<Internode>();

        // determines color of bark
        float noiseR = Random.value/2;
        float noiseG = Random.value/2;
        float noiseB = Random.value/2;
        woodColor = new Color(0.72f + noiseR, 0.47f + noiseG, 0.38f + noiseG, 1f);

        //create first internode and bud before growing
        Node baseNode = new Node(this.gameObject.transform.position, 0, null);
        nodes.Add(baseNode);

        //adding second node since we do not want branches at base of the trunk
        Node secondNode = new Node(new Vector3(baseNode.position.x, baseNode.position.y + 5, baseNode.position.z), 0, baseNode);
        nodes.Add(secondNode);

        baseNode.AddNextNode(secondNode);

        Internode baseInternode = new Internode(baseNode, secondNode, numQuads, woodColor, this.transform);
        baseInternode.CreateInternode();
        internodes.Add(baseInternode);
        secondNode.buds.Add(new Bud(growthProbability, new Vector3(0,1,0))); // straight up
        
        for (int i = 0; i < iterations; i++)
        {
            GrowPlant();
        }

        CheckNodes();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GrowPlant();
            CheckNodes();
        }
    }
    private void CheckNodes()
    {
        // foreach (GameObject node in nodeObjs)
        // {
        //     Destroy(node);
        // }
        // foreach (Node node in nodes)
        // {
        //     nodeObjs.Add(Instantiate(sphere, node.position, Quaternion.identity));
        // }
    }

    protected virtual void GrowPlant()
    {
        List<Node> currNodes = new List<Node>(nodes);
        for (int i = 1; i < currNodes.Count; i++)   // no branching at base
        {
            Node node = currNodes[i];
            List<Bud> currBuds = new List<Bud>(node.buds);
            foreach(Bud bud in currBuds)
            {
                if (bud.isDead)
                {
                    continue;
                }
                bud.CalculateDieProbability();
                if (Random.value < bud.dieProbability)
                {
                    bud.SetDead();  //bud is dead
                }
                bud.IncrementAge();
                if (Random.value < growthProbability)
                {
                    Vector3 dir = (bud.direction).normalized;
                    
                    float gLength = Mathf.Pow(growthLengthFactor, bud.order) * growthLength;
                    Vector3 newLocation = new Vector3(node.position.x + (gLength * dir.x), 
                                                        node.position.y + (gLength * dir.y), 
                                                        node.position.z + (gLength * dir.z));
                    Node newNode = new Node(newLocation, bud.order, node);
                    nodes.Add(newNode);
                    node.AddNextNode(newNode);

                    Internode newInternode = new Internode(node, newNode, numQuads, woodColor, this.transform);
                    newInternode.CreateInternode();
                    internodes.Add(newInternode);

                    Bud newBud = new Bud(bud.growthProbability, bud.direction, bud.order);
                    newNode.buds.Add(newBud);
                    node.buds.Remove(bud);
                }
            }

            float rand = Random.value;
            if (isFlowering && rand < 0.1f)
            {
                Instantiate(flower, node.position, Quaternion.identity);
            }
            else if (Random.value < branchProbability)
            {
                float noiseX = (Random.value * 2) - 1;
                float noiseY = (Random.value * 2) - 1;
                float noiseZ = (Random.value * 2) - 1;
                Vector3 newDirection = new Vector3(noiseX, noiseY, noiseZ).normalized;
                Bud newSideBud = new Bud(growthProbability, newDirection, node.order + 1);
                node.buds.Add(newSideBud);
            }
        }
    }

    public void DeleteBranch(List<Node> branchNodes, Node startNode)
    {
        foreach (Node node in branchNodes)
        {
            Debug.Log("removed a node");
            nodes.Remove(node);
        }
        List<Internode> currInternodes = new List<Internode>();
        foreach (Internode internode in internodes)
        {
            currInternodes.Add(internode);
        }
        foreach (Internode internode in currInternodes)
        {
            if ((branchNodes.Contains(internode.start) || internode.start == startNode) && branchNodes.Contains(internode.end))
            {
                Destroy(internode.internodeObj);
                internodes.Remove(internode);
            }
        }
        CheckNodes();
    }
}
