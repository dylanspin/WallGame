using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Building", menuName = "Rooms/Ai")]
public class EnemyData : ScriptableObject
{
    public string Name = "Ai";

    [Header("Object that spawned")]
    public GameObject spawnBody;

    [Header("Max health amount")]
    public float maxHealth = 100;

    [Header("Time it takes to fully regenerate")]
    public float healthGen = 60;//if 0 then dont regen
    /*
        For attacks maybe make scriptable objects?
        Think more of this later
    
    */

    [Header("If the Ai can fly")]
    public bool canFly = false;//can go over holes and stufff

    [Header("If the Ai can change movementSpeed")]
    public bool canSprint = false;
    public float walkingSpeed = 10;
    public float runningSpeed = 10;
    
    [Header("If the Ai attacks other enemies")]
    public bool agressive = false;
}
