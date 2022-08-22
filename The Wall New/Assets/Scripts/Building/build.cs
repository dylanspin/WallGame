using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Rooms/Building")]
public class build : ScriptableObject
{
    public string Name = "Building";
    public GameObject spawnObj;
    public int costBuilding = 0;
    public int[] size = {1,1};
    public float health = 1000;
    public int[] gainItems;//list of item ids that are gained when broken?
    public int[] gainResources;//amounts of resources gained
}
