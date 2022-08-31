using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Building", menuName = "Rooms/Item")]
public class ItemData : ScriptableObject
{
    public string Name = "Ai";
    [Header("Type Item: 0=Tool--1=Clothing--2=Food--3=Resource")]
    public int type = 0;
}
