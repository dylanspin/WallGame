using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInfo : MonoBehaviour
{
    [Header("If there is floor")]
    [SerializeField] private bool[] row1 = new bool[11];
    [SerializeField] private bool[] row2 = new bool[11];
    [SerializeField] private bool[] row3 = new bool[11];
    [SerializeField] private bool[] row4 = new bool[11];

    [Header("If build")]
    [SerializeField] private int[,] buildData = new int[4,11];

    [Header("Private data")]
    private Transform gridStartPoint;

    public void setFloorData(int[,] newData,Transform newStart)
    {
        buildData = newData;
        gridStartPoint = newStart;
    }

    public bool checkIsFloor(int x, int y)
    {
        if(x == 0)
        {
            return row1[y];
        }
        else if(x == 1)
        {
            return row2[y];
        }
        else if(x == 2)
        {
            return row3[y];
        }
        else 
        {
            return row4[y];
        }
    }

    public bool checkHasRoof(int i, int b,int[] size)//returs bool if has roof
    {
        bool clear = true;
        for(int c=0; c<size[0]; c++)
        {
            for(int d=0; d<size[1]; d++)
            {
                if(clear)
                {
                    if(!checkIsFloor(i + c,b + d))
                    {
                        clear = false; //if not possible
                    }
                }
            }
        }

        return clear;
    }



    public int[,] getBuild()
    {
        return buildData;
    }

    public bool[,] getFloorData()
    {
        bool[,] data = new bool[,] {
            {row1[0],row1[1],row1[2],row1[3],row1[4],row1[5],row1[6],row1[7],row1[8],row1[9],row1[10]},
            {row2[0],row2[1],row2[2],row2[3],row2[4],row2[5],row2[6],row2[7],row2[8],row2[9],row2[10]},
            {row3[0],row3[1],row3[2],row3[3],row3[4],row3[5],row3[6],row3[7],row3[8],row3[9],row3[10]},
            {row4[0],row4[1],row4[2],row4[3],row4[4],row4[5],row4[6],row4[7],row4[8],row4[9],row4[10]},
        };

        return data;
    }
}
