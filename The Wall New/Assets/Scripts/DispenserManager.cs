using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserManager : MonoBehaviour
{
    [SerializeField] private Transform[] botomDis;
    [SerializeField] private Transform[] topDis;
    [SerializeField] private float heightOffset = 15;

    public void setRooms(int[] roomHeights)
    {
        Transform[,] dispensers = new Transform[,] {
            {botomDis[0],botomDis[1]},
            {topDis[0],topDis[1]}
        };
        
        for(int i=0; i<dispensers.GetLength(0); i++)
        {
            for(int b=0; b<dispensers.GetLength(1); b++)
            {
                Vector3 pos = dispensers[i,b].transform.position; 
                pos.y = 7 + (roomHeights[i] * heightOffset);
                dispensers[i,b].transform.position = pos;
            }
        }
    }
}
