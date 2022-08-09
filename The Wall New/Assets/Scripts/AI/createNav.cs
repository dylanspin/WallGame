using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class createNav : MonoBehaviour
{
    [SerializeField] private NavMeshSurface surfaces;
    public void bake()///gets called after the generation of wall
    {
        surfaces.BuildNavMesh();
    }
}