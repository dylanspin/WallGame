using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallGen : MonoBehaviour
{
    [Header("Set Data")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private GameObject roomCreator;
    [SerializeField] private Vector2 roomOffsets = new Vector2(50,15);
    [SerializeField] private Vector2 wallDimensions = new Vector2(10,15);
    [SerializeField] private int[] bioms = new int[10];

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
    private int[] dispenserHeights = new int[2];//height positions of the item dispensers
    private GameObject[,] rooms = new GameObject[10,15];//spawned rooms

    private void Start()
    {
        setStartLengths();
        genWall();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // genWall();
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
        rooms = new GameObject[(int)wallDimensions.x,(int)wallDimensions.y];
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
                if(rooms[i,b] == null)
                {
                    rooms[i,b] = Instantiate(roomCreator,spawnPos,Quaternion.Euler(0,0,0),rowParrent) as GameObject;
                }
                // Image room = rooms[i,b].GetComponent<Image>();
                // resetRoom(i,b);//resets room
                setWall(i,b);//sets walls
                setRoof(i,b);//sets roofs
                spawnPos.x += roomOffsets.y;
            }
            spawnPos.x = startPoint.position.x;
            spawnPos.y += roomOffsets.x;
        }

        createPreRooms();

        for(int i=0; i<wallRoomIds.GetLength(0); i++)//goes over it for the second time to set last things
        {
            for(int b=0; b<wallRoomIds.GetLength(1); b++)
            {
                setBiomes(i,b);//sets biomes
                setDecor(i,b);//set wall decor
                setBuildingRoom(i,b);
                int[] wallAllDecor = {wallDecor[i,b,0],wallDecor[i,b,1],wallDecor[i,b,2],wallDecor[i,b,3]};//wall decor array
                rooms[i,b].GetComponent<Room>().generate(wallRoomIds[i,b],wallBroken[i,b],wallAllDecor,roofBroken[i,b],returnGridInfo(i,b),isSpecial[i,b],buildScript,this,i,b);
            }
        }
    }

    private void setDecor(int i, int b)
    {
        if(isSpecial[i,b] == 0)
        {
            if(Random.Range(0,20) > 10)
            {
                for(int z=0; z<2; z++)
                {
                    List<int> keepTrack = new List<int>();
                    if(Random.Range(0,20) > 5)
                    {
                        int newRandom = Random.Range(1,buildScript.WallStains.Length);
                        keepTrack.Add(newRandom);
                        wallDecor[i,b,z] = newRandom;//wall decor
                        while(!keepTrack.Contains(newRandom))
                        {
                            newRandom = Random.Range(1,buildScript.WallStains.Length);
                            keepTrack.Add(newRandom);
                            wallDecor[i,b,z] = newRandom;//wall decor
                        }
                    }
                }
            }
            ////////needs biome specialized decor
            ////////needs if has wall in room specialized decors
            if(Random.Range(0,20) > 15)
            {
                wallDecor[i,b,3] = Random.Range(1,buildScript.specialWallDecor.Length);///sets extra top layer of special plants or biome related stuff
            }   
        }
    }

    private void setBuildingRoom(int i, int b)
    {
        if(isSpecial[i,b] == 0)
        {
            for(int x=0; x<roomBuild.GetLength(2); x++)
            {
                for(int z=0; z<roomBuild.GetLength(3); z++)
                {
                    if(roomBuild[i,b,x,z] == 0)//if not blocked
                    {
                        if(Random.Range(0,10) > 6)
                        {
                            int randomId = Random.Range(1,buildScript.buildings.Length);
                            build selectedB = buildScript.buildings[randomId];
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
                    wallBroken[i,b] = Random.Range(1,buildScript.Walls.Length);//broken
                }
                // room.sprite = roomImages[1];
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
                    roofBroken[i,b] = Random.Range(1,buildScript.Roofs.Length);//broken
                }

                if(wallBroken[i,b] == -1)
                {   
                    if(roofBroken[i,b] == -1)//if roof is also broken
                    {
                        // room.enabled = false;
                    }
                    else
                    {
                        // room.sprite = roomImages[2];
                    }
                }
                else
                {
                    // room.sprite = roomImages[3];
                }
            }
        }
    }

    private void setBiomes(int i, int b)
    {
        if(Random.Range(0,5) > 2)
        {
            if(wallRoomIds[i,b] == 0 && isSpecial[i,b] == 0)//if its not a biome already // if its not a special area
            {
                setBiomeSpread(bioms[Random.Range(0,bioms.Length)],i,b);
            }
        }
    }

    private void setBiomeSpread(int newBiome,int xPos,int yPos)
    {
        int xStart = Random.Range(0,xPos);
        int xEnd = Random.Range(xPos,(int)wallDimensions.x);
        int maxLength = Random.Range(2,6);
        int[] counts = new int[2];
        for(int i=xPos; i<wallDimensions.x; i++)
        {
            if(counts[0] < maxLength)
            {
                counts[0] ++;
                int yStart = Random.Range(0,yPos);
                int yEnd = Random.Range(yPos,(int)wallDimensions.y+1);
                for(int b=yStart; b<yEnd; b++)
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
                counts[1] = 0;
            }
        }
    }

    private void createPreRooms()
    {
        setDispensers();
        createMarket();
        createStart();
        //create starting zones
    }

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

    private int[,] returnGridInfo(int i, int b)
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
