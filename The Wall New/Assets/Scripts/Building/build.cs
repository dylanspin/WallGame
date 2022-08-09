using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Building", menuName = "Rooms/Building")]
public class build : ScriptableObject
{
    public string Name = "Building";
    public int costBuilding = 0;
    public int[] size = {1,1};


}
