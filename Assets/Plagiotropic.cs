// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Plagiotropic : PlantSpawner
// {
//     protected override void GrowPlant()
//     {
//         List<Node> currNodes = new List<Node>(nodes);
//         for (int i = 1; i < currNodes.Count; i++)
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

//                     if (noisyGrowth)
//                     {
//                         float noiseX = (Random.value * 0.5f) - 0.25f;
//                         float noiseY = (Random.value * 0.5f) - 0.25f;
//                         float noiseZ = (Random.value * 0.5f) - 0.25f;
//                         dir = dir + (new Vector3(noiseX, noiseY, noiseZ)).normalized;
//                     }
//                     //changing growth length based on order, maybe can store this information than having to calculate it out all the time
//                     float gLength = Mathf.Pow(growthLengthFactor, bud.order) * growthLength;
//                     Vector3 newLocation = new Vector3(node.position.x + (gLength * dir.x), 
//                                                         node.position.y + (gLength * dir.y), 
//                                                         node.position.z + (gLength * dir.z));
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
//             if (Random.value < branchProbability)
//             {
//                 float noiseX = (Random.value * 2) - 1; //[-1,1]
//                 float noiseY = ((Random.value/5f) - 0.1f)/2;  // [-0.1,0.1]
//                 float noiseZ = (Random.value * 2) - 1;  //[-1,1]
//                 Vector3 newDirection = new Vector3(noiseX, noiseY, noiseZ).normalized;
//                 // Vector3 newDirection = new Vector3(0.333f, 0.333f, 0.333f);
//                 Bud newSideBud = new Bud(growthProbability, newDirection, node.order + 1);
//                 node.buds.Add(newSideBud);
//             }
//         }
//     }
// }
