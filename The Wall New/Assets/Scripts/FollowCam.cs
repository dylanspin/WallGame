using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 0.1f;

    void Update()
    {
        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, followSpeed);
        }
    }
}
