using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] Transform[] rows;
    [SerializeField] Material[] materials;//0 = nothing build 1 = red is build 2 = can build when draging object
    
    public void showGridFloor(FloorInfo infoScript)
    {
        bool[,] display = new bool[4,11];
        int[,] buildInfo = infoScript.getBuild();
        
        if(infoScript)
        {
            display = infoScript.getFloorData();
        }

        for(int i=0; i<display.GetLength(0); i++)
        {
            for(int b=0; b<display.GetLength(1); b++)
            {
                Transform square = rows[i].GetChild(b);
                setColorSquare(square,buildInfo[i,b]);
                square.gameObject.SetActive(display[i,b]);
            }
        }
    }
    
    private void setColorSquare(Transform square,int buildData)//sets individual square slots colors to indicate their status
    {
        if(buildData == 0)//if free space
        {
            square.GetComponent<MeshRenderer>().material = materials[0];
        }
        else//if blocked
        {
            square.GetComponent<MeshRenderer>().material = materials[1];
        }
    }
}
