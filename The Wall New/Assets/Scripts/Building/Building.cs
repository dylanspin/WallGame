using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Set Data")]
    [SerializeField] private Transform GridPrefab;
    [SerializeField] private Grid gridScript;

    [Header("Build Data")]
    [SerializeField] public GameObject[] Roofs;
    [SerializeField] public GameObject[] Walls;
    [SerializeField] public GameObject[] WallStains;
    [SerializeField] public GameObject[] specialWallDecor;
    [SerializeField] public build[] buildings;
    
    [Header("Materials")]
    [SerializeField] public Material[] biomeMaterials;
    [SerializeField] public Material[] stainMaterials;



    private bool isShowing = false;

    // [Header("private Data")]
    public void setBuild(Room roomScript)
    {
        GridPrefab.gameObject.SetActive(true);
        gridScript.showGridFloor(roomScript.getFloor());
        GridPrefab.transform.position = roomScript.getBuildGrid().position;
        isShowing = true;
    }

    public void stopBuild()
    {
        GridPrefab.gameObject.SetActive(false);
        isShowing = false;
    }

    public bool shows()
    {
        return isShowing;
    }
}
