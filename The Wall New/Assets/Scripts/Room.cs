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
    [SerializeField] private Transform wallDecor;
    
    [Header("SpawnObject")]
    [SerializeField] public GameObject[] Roofs;
    [SerializeField] public GameObject[] Walls;
    [SerializeField] public GameObject[] WallStains;
    [SerializeField] public GameObject[] specialWallDecor;

    [Header("Data")]
    [SerializeField] public Material[] biomeMaterials;
    [SerializeField] public Material[] stainMaterials;

    [Header("Private values")]
    private int biome = 0;
    private int specialId = 0;
    private int xPos = 0;
    private int yPos = 0;
    private GameObject spawnedWall;
    private GameObject spawnedRoof;
    private GameObject[] spawnedWDecor = new GameObject[5];//wall stains and plants spawn ids
    private WallGen genScript;

    public void generate(int newBiome,int wall,int[] wDecorId,int roof,int[,] buildInfo,int special,WallGen newScript,int x,int y)
    {
        xPos = x;
        yPos = y;
        specialId = special;
        genScript = newScript;
        biome = newBiome;
        
        if(wall > -1)//if not removed
        {   
            spawnedWall = Instantiate(Walls[wall],wallPoint.position,Quaternion.Euler(0,0,0),wallPoint) as GameObject;
        }

        setStains(wDecorId);

        if(roof > -1)//if not removed
        {   
            spawnedRoof = Instantiate(Roofs[roof],roofPoint.position,Quaternion.Euler(0,0,0),roofPoint) as GameObject;
            spawnedRoof.GetComponent<FloorInfo>().setFloorData(buildInfo);
            // spawnedRoof.GetComponent<createNav>().bake();
        }   

        setbiome(newBiome);
    }

    private void setStains(int[] wDecorId)
    {
        // stainMaterials
        int decorOffset = -15;
        for(int i=0; i<wDecorId.Length; i++)
        {
            Vector3 spawnPos = wallDecor.position;
            spawnPos.x += decorOffset;
            if(wDecorId[i] > 0)
            {
                spawnedWDecor[i] = Instantiate(WallStains[wDecorId[i]],spawnPos,Quaternion.Euler(0,0,0),wallPoint) as GameObject;
                spawnedWDecor[i].transform.GetChild(0).GetComponent<MeshRenderer>().material = stainMaterials[biome];
            }
            decorOffset += 15;
        }
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
