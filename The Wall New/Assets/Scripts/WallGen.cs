using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WallGen : MonoBehaviour
{
    [Header("Chances")]
    [SerializeField] private int maxWaterSpawns = 4;
    [SerializeField] private int waterChance = 70;

    [Header("Set Data")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private GameObject roomCreator;
    [SerializeField] private Vector2 roomOffsets = new Vector2(50,15);
    [SerializeField] private Vector2 wallDimensions = new Vector2(10,15);
    [SerializeField] private List<int> biomeRarity = new List<int>();

    [Header("Scripts")]
    [SerializeField] private Room roomScript;
    [SerializeField] private DispenserManager dispenserScript;
    [SerializeField] private Building buildScript;

    [Header("Private data")]
    private int[,] wallRoomIds = new int[10,15];//room biome data
    private int[,] wallBroken = new int[10,15];//right wall data
    private int[,,] wallDecor = new int[10,15,4];//back wall decor ids
    private int[,,,] roomBuild = new int[10,15,4,11];//Grid Build data
    private int[,] roofBroken = new int[10,15];//floor data
    private int[,] isSpecial =  new int[10,15];//if its indestructable story areae
    private int[,] isWater =  new int[10,15];//if its water 0 = none 1 = top water 2 = deep water
    private int[] dispenserHeights = new int[2];//height positions of the item dispensers
    private GameObject[,] rooms = new GameObject[10,15];//spawned rooms

    [Header("Biome Data")]
    private int maxBiomeChance = 0;

    private void Start()
    {
        setStartLengths();
        genWall();
    }

   
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            bakeNavMeshes();
        }
    }

    private void setStartLengths()
    {
        wallRoomIds = new int[(int)wallDimensions.x,(int)wallDimensions.y];
        wallBroken = new int[(int)wallDimensions.x,(int)wallDimensions.y];
        wallDecor = new int[(int)wallDimensions.x,(int)wallDimensions.y,4];
        roomBuild = new int[(int)wallDimensions.x,(int)wallDimensions.y,4,11];
        roofBroken = new int[(int)wallDimensions.x,(int)wallDimensions.y];
        isSpecial = new int[(int)wallDimensions.x,(int)wallDimensions.y];
        isWater = new int[(int)wallDimensions.x,(int)wallDimensions.y];
        rooms = new GameObject[(int)wallDimensions.x,(int)wallDimensions.y];
        setMaxBiomeChance();//sets the max length for the biome chances
    }

    private void setMaxBiomeChance()
    {
        for(int i=0; i<buildScript.biomes.Length; i++)
        {
            for(int b=0; b<buildScript.biomes[i].rarity; b++)
            {
                biomeRarity.Add(i);
            }
        }
    }

    private void genWall()
    {
        Vector3 spawnPos = startPoint.position;
        for(int i=0; i<wallRoomIds.GetLength(0); i++)
        {
            Transform rowParrent = new GameObject("Row" + i).transform;
            rowParrent.position = spawnPos;
            rowParrent.parent = startPoint;
            for(int b=0; b<wallRoomIds.GetLength(1); b++)
            {
                rooms[i,b] = Instantiate(roomCreator,spawnPos,Quaternion.Euler(0,0,0),rowParrent) as GameObject;//spawns individual rooms in wall
                setWall(i,b);//sets walls
                setRoof(i,b);//sets roofs
                spawnPos.x += roomOffsets.y;
            }
            spawnPos.x = startPoint.position.x;
            spawnPos.y += roomOffsets.x;
        }

        createPreRooms();//makes premade stuff like the market

        for(int i=0; i<wallRoomIds.GetLength(0); i++)//goes over it for the second time to set last things
        {
            for(int b=0; b<wallRoomIds.GetLength(1); b++)
            {
                setBiomes(i,b);//sets biomes
                setDecor(i,b);//set wall decor
                setBuildingRoom(i,b);
                int[] wallAllDecor = {wallDecor[i,b,0],wallDecor[i,b,1],wallDecor[i,b,2],wallDecor[i,b,3]};//wall decor array
                //generates base room with out the buildings
                rooms[i,b].GetComponent<Room>().generate(wallRoomIds[i,b],wallBroken[i,b],wallAllDecor,roofBroken[i,b],returnGridInfo(i,b),isSpecial[i,b],isWater[i,b],buildScript,this,i,b);
            }
        }

        for(int i=0; i<wallRoomIds.GetLength(0); i++)//goes over it for the third time to spawn buildings
        {
            for(int b=0; b<wallRoomIds.GetLength(1); b++)
            {
                rooms[i,b].GetComponent<Room>().genBuildings(returnGridInfo(i,b));//generates buildings in the room this is needed later so the rooms above have the floor information
            }
        }

        // bakeNavMeshes();//backes new navMesh //disable when testing to make the gen faster
    }

    private void setDecor(int i, int b)//sets wall decor
    {
        Biome setBiome = buildScript.biomes[wallRoomIds[i,b]];
        if(isSpecial[i,b] == 0)
        {
            wallDecor[i,b,3] = Random.Range(0,setBiome.specialWallDecor.Length);
            
            if(setBiome.specialWallDecor[wallDecor[i,b,3]] == null || setBiome.bothBackDecor)//if doesnt have special decor then add stains
            {
                if(Random.Range(0,20) > 10)
                {
                    for(int z=0; z<2; z++)
                    {
                        List<int> keepTrack = new List<int>();
                        if(Random.Range(0,20) > 5)
                        {
                            int newRandom = Random.Range(1,setBiome.WallStains.Length);
                            keepTrack.Add(newRandom);
                            wallDecor[i,b,z] = newRandom;//wall decor
                            while(!keepTrack.Contains(newRandom))
                            {
                                newRandom = Random.Range(1,setBiome.WallStains.Length);
                                keepTrack.Add(newRandom);
                                wallDecor[i,b,z] = newRandom;//wall decor
                            }
                        }
                    }
                }
            }
        }
    }

    private void setBuildingRoom(int i, int b)//sets random spawned buildings in room
    {
        if(isSpecial[i,b] == 0)
        {
            for(int x=0; x<roomBuild.GetLength(2); x++)
            {
                for(int z=0; z<roomBuild.GetLength(3); z++)
                {
                    if(roomBuild[i,b,x,z] == 0)//if not blocked
                    {
                        if(Random.Range(0,10) > 4)
                        {
                            Biome setBiome = buildScript.biomes[wallRoomIds[i,b]];
                            int randomId = Random.Range(1,setBiome.buildings.Length);
                            build selectedB = setBiome.buildings[randomId];
                            bool clear = true;

                            FloorInfo currentFloorInfo = null;
                            if(roofBroken[i,b] > -1)
                            {
                                currentFloorInfo = buildScript.Roofs[roofBroken[i,b]].GetComponent<FloorInfo>();
                            }

                            for(int c=0; c<selectedB.size[0]; c++)
                            {
                                for(int d=0; d<selectedB.size[1]; d++)
                                {
                                    if(clear)
                                    {
                                        if(x + c < roomBuild.GetLength(2) && z + d < roomBuild.GetLength(3) && roomBuild[i,b,x + c,z + d] == 0)
                                        {
                                            if(currentFloorInfo != null)
                                            {
                                                if(!currentFloorInfo.checkIsFloor(x + c,z + d))
                                                {
                                                    clear = false; //if not possible
                                                }
                                            }
                                            else
                                            {
                                                clear = false; //if not possible
                                            }
                                        }
                                        else
                                        {
                                            clear = false; //if not possible
                                        }
                                    }
                                }
                            }
                            if(clear)
                            {
                                for(int c=0; c<selectedB.size[0]; c++)
                                {
                                    for(int d=0; d<selectedB.size[1]; d++)
                                    {
                                        if(x + c < roomBuild.GetLength(2) && z + d < roomBuild.GetLength(3))
                                        {
                                            roomBuild[i,b,x + c,z + d] = -1;//makes it blocked ground
                                        }
                                    }
                                }
                                roomBuild[i,b,x,z] = randomId;//sets actual build point to build
                            }
                        }
                    }
                }
            }
        }
    }

    private void setWall(int i, int b)
    {
        if(b + 1 == (int)wallDimensions.y)//right side of wall
        {
            wallBroken[i,b] = -1;//completly removed
        }
        else
        {
            if(Random.Range(0,20) > 13)
            {
                if(Random.Range(0,20) > 13)
                {
                    wallBroken[i,b] = -1;//completly removed
                }
                else
                {
                    if(i < wallBroken.GetLength(0)/3)//if bottom half of the wall 
                    {
                        if(Random.Range(0,10) > 7)//chance of wall being broken
                        {
                            wallBroken[i,b] = Random.Range(2,buildScript.Walls.Length);//always broken
                        }
                        else
                        {
                            wallBroken[i,b] = Random.Range(1,buildScript.Walls.Length);//chance broken
                        }
                    }
                    else
                    {
                        wallBroken[i,b] = Random.Range(1,buildScript.Walls.Length);//chance broken
                    }
                }
            }
        }
    }

    private void setRoof(int i, int b)
    {
        if(i == 0)//top of wall room
        {
            roofBroken[i,b] = 0;//completly gone
        }
        else
        {
            if(Random.Range(0,20) > 15)
            {
                if(Random.Range(0,20) > 15)
                {
                    roofBroken[i,b] = -1;//completly gone
                }
                else
                {
                    if(i < roofBroken.GetLength(0)/2)//if bottom half of the wall 
                    {
                        if(Random.Range(0,10) > 7)//chance of floor being broken
                        {
                            roofBroken[i,b] = Random.Range(2,buildScript.Roofs.Length);//always broken
                        }
                        else
                        {
                            roofBroken[i,b] = Random.Range(1,buildScript.Roofs.Length);//chance broken
                        }
                    }
                    else
                    {
                        roofBroken[i,b] = Random.Range(1,buildScript.Roofs.Length);//chance broken
                    }
                }
            }
        }
    }

    private void setBiomes(int i, int b)
    {
        if(wallRoomIds[i,b] == 0 && isSpecial[i,b] == 0)//if its not a biome already // if its not a special area
        {
            int randomBiome = biomeRarity[Random.Range(0,biomeRarity.Count)];
            setBiomeSpread(randomBiome,i,b);
        }
    }

    private void setBiomeSpread(int newBiome,int xPos,int yPos)
    {
        int xStart = Random.Range(0,xPos);
        int xEnd = Random.Range(xPos,(int)wallDimensions.x);
        int maxLength = Random.Range(2,6);
        int[] counts = new int[2];
        for(int i=xPos; i<wallDimensions.x; i++)////can be better optimized to count the extra height as long as it doesnt go over the max height
        {
            if(counts[0] < maxLength)
            {
                counts[0] ++;
                int yStart = Random.Range(0,yPos);
                int yEnd = Random.Range(yPos,(int)wallDimensions.y+1);
                for(int b=yStart; b<yEnd; b++)
                {
                    if(wallRoomIds[i,b] == 0)
                    {
                        int maxLength2 = Random.Range(3,4);
                        if(counts[1] < maxLength2)
                        {
                            counts[1] ++;
                            if(isSpecial[i,b] == 0)
                            {
                                wallRoomIds[i,b] = newBiome;
                            }
                        }
                    }
                }
                counts[1] = 0;
            }
        }
    }

    private void createPreRooms()
    {
        setDispensers();
        createMarket();
        createStart();
        createWater();
        //create starting zones
    }

    /*Still needs to instantiate the market it self*/
    private void createMarket()
    {   
        int xStart = Random.Range(0,(int)wallDimensions.x-8);
        int yStart = Random.Range(1,(int)wallDimensions.y-4);
        while(Mathf.Abs(xStart - dispenserHeights[0]) < 5)//makes sure the market is always at least 5 in height away from the starting spot
        {
            xStart = Random.Range(0,(int)wallDimensions.x-4);
        }
        
        for(int i=xStart; i<xStart + 5; i++)
        {
            for(int b=yStart; b<yStart + 4; b++)
            {
                if(b != yStart + 3)
                {
                    wallBroken[i,b] = -1;
                }
                else
                {
                    setWall(i,b);
                    // wallBroken[i,b] = Random.Range(1,buildScript.Walls.Length);
                }

                if(i != xStart)
                {
                    roofBroken[i,b] = -1;
                }
                else
                { 
                    roofBroken[i,b] = 0;
                }

                isSpecial[i,b] = 1;//sets slot to special market
            }
        }
    }

    private void setDispensers()//sets the side dispensers
    {
        dispenserHeights[0] = Random.Range(3,8);
        dispenserHeights[1] = Random.Range((int)wallDimensions.x-13,(int)wallDimensions.x - 3);

        dispenserScript.setRooms(dispenserHeights);
    }

    private void createStart()//sets area's to discovered that can be seen from the start
    {
        bool stopedY = false;
        for(int i=0; i<wallDimensions.y; i++)
        {
            if(i < 5)
            {
                wallBroken[dispenserHeights[0],i] = Random.Range(1,buildScript.Walls.Length);
                checkUpStart(dispenserHeights[0],i);
            }
            else
            {
                if(wallBroken[dispenserHeights[0],i] != 0 && !stopedY)
                {
                    checkUpStart(dispenserHeights[0],i);
                }
                else
                {
                    stopedY = true;
                }
            }
        }
    }

    private void createWater()
    {
        int countWaterSpawns = 0;

        for(int i=0; i<isWater.GetLength(0)/2; i++)
        {
            for(int b=0; b<isWater.GetLength(1); b++)
            {
                if(countWaterSpawns < maxWaterSpawns)
                {
                    if(isWater[i,b] == 0)//if isnt already water
                    {
                        if(Random.Range(0,75) > waterChance)
                        {
                            if(isSpecial[i,b] == 0)//if it isnt the market or some other special room
                            {
                                setHorizontalWater(i,b,true);
                                countWaterSpawns ++;
                            }
                        }
                    }
                }
            }
        }
    }

    /*
        Needs on top a different water spawn
        Layer below top need be full water spawn
        Needs to not be on the start layer
    */

    private void setHorizontalWater(int i, int b,bool first)
    {
        setWater(i,b,first);
        bool left = true;
        bool right = true;

        for(int x=b; x<wallBroken.GetLength(1); x++)//checks the right of the water source
        {
            if(right)
            {
                if(isWater[i,x] == 0)
                {
                    setWater(i,x,first);
                }          

                if(roofBroken[i,x] != 0 || isSpecial[i,x] != 0)//if there is a hole in the floor
                {
                    if(isWater[i-1,x] == 0)
                    {
                        setWater(i-1,x,false);
                        setHorizontalWater(i-1,x,false);
                    }
                }

                if(wallBroken[i,x] == 0 || isSpecial[i,x] != 0)
                {
                    right = false;
                }
            }
        }
        
        for(int x=b; x>1; x--)//checks the left of the water source
        {
            if(left)
            {
                if(isWater[i,x] == 0)
                {
                    setWater(i,x,first);
                }        

                if(roofBroken[i,x] != 0 || isSpecial[i,x] != 0)//if there is a hole in the floor
                {
                    if(isWater[i-1,x] == 0)
                    {
                        setWater(i-1,x,false);
                        setHorizontalWater(i-1,x,false);
                    }
                }

                if(wallBroken[i,x-1] == 0 || isSpecial[i,x-1] != 0)
                {
                    left = false;
                }
            }
        }
    }

    private void setWater(int i, int b, bool first)
    {
        if(first)
        {
            isWater[i,b] = 2;
        }
        else
        {
            isWater[i,b] = 1;
        }
    }

    /////////for later use with non discovered area
    private void checkUpStart(int xPos,int yPos)//checks if roofs are open if so set to green
    {
        bool[] stopped = {false,false};///needs to be shorter
        for(int i=xPos; i<wallDimensions.x; i++)
        {
            if(!stopped[0] && roofBroken[i,yPos] != 0)
            {
                //set discovered
                // rooms[i,yPos].GetComponent<Image>().color = roomColors[11];
            }
            else
            {
                stopped[0] = true;
            }
        }

        for(int i=xPos; i<0; i--)
        {
            if(!stopped[1] && roofBroken[i,yPos] != 0)
            {
                //set discovered
                // rooms[i,yPos].GetComponent<Image>().color = roomColors[11];
            }
            else
            {
                stopped[1] = true;
            }
        }
    }

    public bool checkHasRoof(int roomX, int roomY ,int i, int b, int[] size)//for somebuildings who have a roof special object check if it has a roof
    {
        if(roomX == rooms.GetLength(0) - 1)//is top of wall so always has roof
        {
            return true;
        }
        else
        {
            FloorInfo floorScript = rooms[roomX+1,roomY].GetComponent<Room>().getFloor();//gets the floor data from the roof above the given room
            if(floorScript == null)//if room above doesnt have floor
            {
                return false;
            }

            return floorScript.checkHasRoof(i,b,size);//check if has roof space above
        }
    }

    private void bakeNavMeshes()
    {
        NavMeshSurface[] roomSurfaces = rooms[0,0].GetComponent<Room>().getSurfaces();  
        if(roomSurfaces[0])
        {
            roomSurfaces[0].BuildNavMesh();
        }
    }

    public int[,] returnGridInfo(int i, int b)//gives the building data
    {
        int[,] data = new int[,] {
            {roomBuild[i,b,0,0],roomBuild[i,b,0,1],roomBuild[i,b,0,2],roomBuild[i,b,0,3],roomBuild[i,b,0,4],roomBuild[i,b,0,5],roomBuild[i,b,0,6],roomBuild[i,b,0,7],roomBuild[i,b,0,8],roomBuild[i,b,0,9],roomBuild[i,b,0,10]},
            {roomBuild[i,b,1,0],roomBuild[i,b,1,1],roomBuild[i,b,1,2],roomBuild[i,b,1,3],roomBuild[i,b,1,4],roomBuild[i,b,1,5],roomBuild[i,b,1,6],roomBuild[i,b,1,7],roomBuild[i,b,1,8],roomBuild[i,b,1,9],roomBuild[i,b,1,10]},
            {roomBuild[i,b,2,0],roomBuild[i,b,2,1],roomBuild[i,b,2,2],roomBuild[i,b,2,3],roomBuild[i,b,2,4],roomBuild[i,b,2,5],roomBuild[i,b,2,6],roomBuild[i,b,2,7],roomBuild[i,b,2,8],roomBuild[i,b,2,9],roomBuild[i,b,2,10]},
            {roomBuild[i,b,3,0],roomBuild[i,b,3,1],roomBuild[i,b,3,2],roomBuild[i,b,3,3],roomBuild[i,b,3,4],roomBuild[i,b,3,5],roomBuild[i,b,3,6],roomBuild[i,b,3,7],roomBuild[i,b,3,8],roomBuild[i,b,3,9],roomBuild[i,b,3,10]},
        };

        return data;
    }
}
