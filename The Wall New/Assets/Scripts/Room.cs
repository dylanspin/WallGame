using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private Transform wallPoint;
    [SerializeField] private Transform roofPoint;
    [SerializeField] private Transform buildingGrid;
    [SerializeField] private Transform wallDecor;
    [SerializeField] private Transform gridStart;

    [Header("Set Data")]
    [SerializeField] private Material defaultMaterial;

    [Header("Private values")]
    private int biome = 0;
    private int specialId = 0;
    private int xPos = 0;
    private int yPos = 0;
    private GameObject spawnedWall;
    private GameObject spawnedRoof;
    private GameObject[] spawnedWDecor = new GameObject[5];//wall stains and plants spawn ids
    private List<GameObject> spawnedBuildings = new List<GameObject>();//spawned buildings

    [Header("Scripts")]
    private WallGen genScript;
    private Building buildScript;
    private FloorInfo floorScript;
    private Biome currentBiome;
    private NavMeshSurface[] surfaces = new NavMeshSurface[2];

    public void generate(int newBiome,int wall,int[] wDecorId,int roof,int[,] buildInfo,int special,Building newBuild,WallGen newScript,int x,int y)
    {
        xPos = x;
        yPos = y;
        specialId = special;
        genScript = newScript;
        biome = newBiome;
        buildScript = newBuild;
        if(biome > buildScript.biomes.Length)
        {
            Debug.Log("Error : " + biome);
            return;
        }
        currentBiome = buildScript.biomes[biome];

        if(wall > -1)//if not removed
        {   
            spawnedWall = Instantiate(buildScript.Walls[wall],wallPoint.position,Quaternion.Euler(0,0,0),wallPoint) as GameObject;
            // surfaces[0] = spawnedRoof.transform.GetChild(0).GetComponent<NavMeshSurface>();
        }

        setStains(wDecorId);

        if(roof > -1)//if not removed
        {   
            spawnedRoof = Instantiate(buildScript.Roofs[roof],roofPoint.position,Quaternion.Euler(0,0,0),roofPoint) as GameObject;
            floorScript = spawnedRoof.GetComponent<FloorInfo>();
            floorScript.setFloorData(buildInfo,gridStart);
            surfaces[0] = spawnedRoof.transform.GetChild(0).GetComponent<NavMeshSurface>();
        }   

        setbiome();
        // spawnBuildings(buildInfo);
        // bakeNavMeshes();
    }

    public void genBuildings(int[,] buildInfo)
    {
        spawnBuildings(buildInfo);
    }

    private void spawnBuildings(int[,] buildInfo)//spawns buildings at grid positions
    {
        if(floorScript)
        {
            for(int x=0; x<buildInfo.GetLength(0); x++)
            {
                for(int z=0; z<buildInfo.GetLength(1); z++)
                {
                    if(buildInfo[x,z] > 0)
                    {
                        build currentBuilding = currentBiome.buildings[buildInfo[x,z]];
                        bool hasRoofSpace = genScript.checkHasRoof(xPos,yPos,x,z,currentBuilding.size);//if there is the same amount of space above the spawned object
                        Debug.Log(hasRoofSpace);

                        Vector3 spawnPos = gridStart.position;
                        spawnPos.z += 4.4f * x;
                        spawnPos.x += 4.4f * z;

                        GameObject spawnNewB = currentBuilding.spawnObj;
                        if(currentBuilding.spawnObjRoof && hasRoofSpace)//if it can spawn roof object
                        {
                            spawnNewB = currentBuilding.spawnObjRoof;
                        }

                        GameObject spawnB = Instantiate(spawnNewB,spawnPos,Quaternion.Euler(0,0,0),gridStart) as GameObject;
                        spawnedBuildings.Add(spawnB);//adds spawned building to spawned building list for later use

                        if(currentBuilding.useStain)
                        {
                            spawnB.GetComponentInChildren<MeshRenderer>().material = currentBiome.stainMaterial;
                        }   
                    }
                }
            }
        }
    }

    private void setStains(int[] wDecorId)
    {
        // stainMaterials
        int decorOffset = -15;
        for(int i=0; i<wDecorId.Length-1; i++)
        {
            Vector3 spawnPos = wallDecor.position;
            spawnPos.x += decorOffset;
            if(wDecorId[i] > 0)
            {
                spawnedWDecor[i] = Instantiate(currentBiome.WallStains[wDecorId[i]],spawnPos,Quaternion.Euler(0,0,0),wallPoint) as GameObject;
                spawnedWDecor[i].transform.GetChild(0).GetComponent<MeshRenderer>().material = currentBiome.stainMaterial;
               
            }
            decorOffset += 15;
        }

        //last special decor item including plants and stuff
        if(currentBiome.specialWallDecor[wDecorId[3]] != null)
        {
            Vector3 spawnPos = wallDecor.position;
            spawnedWDecor[3] = Instantiate(currentBiome.specialWallDecor[wDecorId[3]],spawnPos,Quaternion.Euler(0,0,0),this.transform) as GameObject;
        }
    }

    private void setbiome()
    {
        Material setMat = defaultMaterial;
        if(Values.debugBiomeMaterials)
        {
           setMat = currentBiome.roomMat;
        }
        
        if(spawnedWall)
        {
            spawnedWall.transform.GetChild(0).GetComponent<MeshRenderer>().material = setMat;
            spawnedWall.transform.GetChild(1).GetComponent<MeshRenderer>().material = setMat;
        }

        if(spawnedRoof)
        {
            spawnedRoof.transform.GetChild(0).GetComponent<MeshRenderer>().material = setMat;
        }
    }

    private void bakeNavMeshes()
    {
        for(int i=0; i<surfaces.Length; i++)
        {
            if(surfaces[i])
            {
                surfaces[i].BuildNavMesh();
            }
        }
    }

    public FloorInfo getFloor()
    {
        if(floorScript)
        {
            return floorScript;
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
