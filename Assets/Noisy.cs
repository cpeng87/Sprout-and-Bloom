// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Noisy : PlantSpawner
// {
//     protected override void GrowPlant()
//     {
//         List<Node> currNodes = new List<Node>(nodes);
//         for (int i = 1; i < currNodes.Count; i++)   // no branching at base
//         {
//             Node node = currNodes[i];
//             List<Bud> currBuds = new List<Bud>(node.buds);
//             foreach(Bud bud in currBuds)
//             {
//                 if (bud.isDead)
//                 {
//                     continue;
//                 }
//                 bud.CalculateDieProbability();
//                 if (Random.value < bud.dieProbability)
//                 {
//                     bud.SetDead();  //bud is dead
//                 }
//                 bud.IncrementAge();
//                 if (Random.value < growthProbability)
//                 {
//                     Vector3 dir = (bud.direction).normalized;
                    
//                     float gLength = Mathf.Pow(growthLengthFactor, bud.order) * growthLength;
//                     float noiseX = (Random.value) - 0.5f;
//                     float noiseY = (Random.value) - 0.5f;
//                     float noiseZ = (Random.value) - 0.5f;
//                     Vector3 newLocation = new Vector3(node.position.x + (gLength * (dir.x + noiseX)), 
//                                                         node.position.y + (gLength * (dir.y + noiseY)), 
//                                                         node.position.z + (gLength * (dir.z + noiseZ)));
//                     Node newNode = new Node(newLocation, bud.order);
//                     nodes.Add(newNode);

//                     Internode newInternode = new Internode(node, newNode, numQuads, woodColor, this.transform);
//                     newInternode.CreateInternode();
//                     internodes.Add(newInternode);

//                     Bud newBud = new Bud(bud.growthProbability, bud.direction, bud.order);
//                     newNode.buds.Add(newBud);
//                     node.buds.Remove(bud);
//                 }
//             }

//             float rand = Random.value;
//             if (isFlowering && rand < 0.1f)
//             {
//                 Instantiate(flower, node.position, Quaternion.identity);
//             }
//             else if (Random.value < branchProbability)
//             {
//                 float noiseX = (Random.value * 2) - 1;
//                 float noiseY = (Random.value * 2) - 1;
//                 float noiseZ = (Random.value * 2) - 1;
//                 Vector3 newDirection = new Vector3(noiseX, noiseY, noiseZ).normalized;
//                 Bud newSideBud = new Bud(growthProbability, newDirection, node.order + 1);
//                 node.buds.Add(newSideBud);
//             }
//         }
//     }
// }
