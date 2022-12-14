using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Rooms/Biome")]
public class Biome : ScriptableObject
{
    [Header("Biome Data")]
    public string Name = "Building";
    public int rarity = 1;

    [Header("Biome Settings")]
    public bool bothBackDecor = false;
    public bool canBeUnderWater = true;

    [Header("Building objects")]
    public GameObject[] WallStains;
    public GameObject[] specialWallDecor;
    public build[] buildings;
    


    [Header("Materials")]
    public Material stainMaterial;
    public Material roomMat;

}