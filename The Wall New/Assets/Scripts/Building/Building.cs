using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Set Data")]
    [SerializeField] private Transform GridPrefab;
    [SerializeField] private Grid gridScript;
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
