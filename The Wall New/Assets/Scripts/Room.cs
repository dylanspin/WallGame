using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private Transform wallPoint;
    [SerializeField] private Transform roofPoint;
    [SerializeField] private Transform buildingGrid;
    
    [Header("SpawnObject")]
    [SerializeField] public GameObject[] Roofs;
    [SerializeField] public GameObject[] Walls;
    [SerializeField] public GameObject[] WallDecorations;

    [Header("Data")]
    [SerializeField] public Material[] biomeMaterials;

    [Header("Private values")]
    private int biome = 0;
    private int specialId = 0;
    private int xPos = 0;
    private int yPos = 0;
    private GameObject spawnedWall;
    private GameObject spawnedRoof;
    private GameObject wallDecorations;
    private WallGen genScript;

    public void generate(int newBiome,int wall,int roof,int special,WallGen newScript,int x,int y)
    {
        xPos = x;
        yPos = y;
        
        if(wall > -1)//if not removed
        {   
            spawnedWall = Instantiate(Walls[wall],wallPoint.position,Quaternion.Euler(0,0,0),wallPoint) as GameObject;
        }

        if(roof > -1)//if not removed
        {   
            spawnedRoof = Instantiate(Roofs[roof],roofPoint.position,Quaternion.Euler(0,0,0),roofPoint) as GameObject;
        }

        setbiome(newBiome);
        specialId = special;
        genScript = newScript;
    }

    private void setbiome(int newBiome)
    {
        biome = newBiome;
        if(spawnedWall)
        {
            spawnedWall.transform.GetChild(0).GetComponent<MeshRenderer>().material = biomeMaterials[biome];
            spawnedWall.transform.GetChild(1).GetComponent<MeshRenderer>().material = biomeMaterials[biome];
        }

        if(spawnedRoof)
        {
            spawnedRoof.transform.GetChild(0).GetComponent<MeshRenderer>().material = biomeMaterials[biome];
        }
    }

    public FloorInfo getFloor()
    {
        if(spawnedRoof)
        {
            return spawnedRoof.GetComponent<FloorInfo>();
        }
        else
        {
            return null;
        }
    }

    public Transform getBuildGrid()
    {
        return buildingGrid;
    }

    public int[] getRoomCoords()//for saving the new data for the grid
    {
        int[] coords = {xPos,yPos};

        return coords;
    }
}
