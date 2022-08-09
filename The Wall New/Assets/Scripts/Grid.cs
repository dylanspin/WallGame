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
        
        if(infoScript)
        {
            display = infoScript.getFloorData();
        }

        for(int i=0; i<display.GetLength(0); i++)
        {
            for(int b=0; b<display.GetLength(1); b++)
            {
                // rows[i].GetChild(b).GetComponent<MeshRenderer>().material = materials[display[i,b]];
                rows[i].GetChild(b).gameObject.SetActive(display[i,b]);
            }
        }
    }
    
}
