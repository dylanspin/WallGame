using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Rooms/Building")]
public class build : ScriptableObject
{
    public string Name = "Building";

    [Header("Object that spawned")]
    public GameObject spawnObj;

    [Header("Object spawned if hasRoof")]
    public GameObject spawnObjRoof;

    [Header("Cost of buying building")]
    public int costBuilding = 0;

    [Header("Building grid size: 0=height -- 1=width")]
    public int[] size = {1,1};

    [Header("Health Building has")]
    public float health = 1000;

    [Header("Stuff gained when destroyed")]
    public int[] gainItems;//list of item ids that are gained when broken?

    [Header("Not sure yet")]
    public int[] gainResources;//amounts of resources gained

    [Header("If stain material")]
    public bool useStain = true;
}
