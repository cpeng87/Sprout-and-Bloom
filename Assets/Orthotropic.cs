using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orthotropic : PlantSpawner
{
    public float curveFactor = 0.2f;
    protected override void GrowPlant()
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
                // GameObject newLeaf = Instantiate(leafCluster, node.position, Quaternion.identity);
                float noiseScale = (Random.value * 0.5f) - 0.25f;
                // newLeaf.transform.localScale = new Vector3(2f + noiseScale, 2f + noiseScale, 2f + noiseScale);
                // newLeaf.transform.rotation = Quaternion.LookRotation(Vector3.forward, bud.direction);
                // newLeaf.transform.SetParent(this.transform);
                node.buds.Remove(bud);
                if (isFlowering && Random.value < 0.04f)
                {
                    GameObject newFlower = Instantiate(flower, node.position, Quaternion.identity);
                    noiseScale = (Random.value * 0.5f) - 0.25f;
                    newFlower.transform.localScale = new Vector3(2f + noiseScale, 2f + noiseScale, 2f + noiseScale);
                    newFlower.transform.rotation = Quaternion.LookRotation(Vector3.forward, bud.direction);
                    newFlower.transform.SetParent(this.transform);
                    node.buds.Remove(bud);
                }
                else if (Random.value < growthProbability)
                {
                    Vector3 dir = (bud.direction).normalized;
                    dir = (dir + (new Vector3(0,1,0) * curveFactor)).normalized;
                    bud.direction = dir;
                    
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

            if (Random.value < branchProbability)
            {
                float noiseX = (Random.value * 2) - 1;
                float noiseY = (Random.value * 0.8f) + 0.2f;
                float noiseZ = (Random.value * 2) - 1;
                Vector3 newDirection = new Vector3(noiseX, noiseY, noiseZ).normalized;
                newDirection = (newDirection + (new Vector3(0,1,0) * 0.1f)).normalized;
                Bud newSideBud = new Bud(growthProbability, newDirection, node.order + 1);
                node.buds.Add(newSideBud);
            }
        }
    }
}
