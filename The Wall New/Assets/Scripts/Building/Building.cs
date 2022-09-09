using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Building : MonoBehaviour
{
    [Header("Set Data")]
    [SerializeField] private Transform notesCover;
    [SerializeField] private Transform GridPrefab;
    [SerializeField] private Grid gridScript;

    [Header("Build Data")]
    [SerializeField] public GameObject[] Roofs;
    [SerializeField] public GameObject[] Walls;
    [SerializeField] public GameObject[] Water;
    [SerializeField] public Biome[] biomes;
    
    [Header("Materials")]
    [SerializeField] public Material[] biomeMaterials;
    [SerializeField] public Material[] stainMaterials;

    [Header("Private Data")]
    private bool isShowing = false;
    private Room currentRoom;

    // [Header("private Data")]
    public void setBuild(Room roomScript)
    {
        currentRoom = roomScript;
        GridPrefab.gameObject.SetActive(true);
        gridScript.showGridFloor(currentRoom.getFloor());
        GridPrefab.transform.position = currentRoom.getBuildGrid().position;
        isShowing = true;
    }
    
    public void stopBuild()
    {
        GridPrefab.gameObject.SetActive(false);
        isShowing = false;
    }

    public void building()
    {
        notesCover.transform.position = currentRoom.getBuildGrid().position;
    }

    public bool shows()
    {
        return isShowing;
    }
}
