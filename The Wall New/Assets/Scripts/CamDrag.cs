using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamDrag : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private Building buildScript;
    
    [Header("Movement stuff")]
    //max dimensions 
    [SerializeField] private float[] horizontalBounds = new float[2];//max zoom horizontal
    [SerializeField] private float[] minHorizontal = new float[2];//min zoom horizontal
    [SerializeField] private float[] verticleBounds = new float[2];//max zoom verticle
    [SerializeField] private float[] minverticleBounds = new float[2];//max zoom horizontal
    private float[] heightBounds = new float[2];
    private float[] sideBounds = new float[2];
    [SerializeField] private float[] dragSpeeds = new float[2];//speed of the camera drag
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask roomMask;
    private bool dragging = false;
    private Vector3 oldPos;
    private Vector3 panOrigin;
    [SerializeField] private float zoomInValue = 0.5f;
    private float value = 0;
    private float dragSpeed = 3;//speed of the camera drag
    [SerializeField] private float[] maxAndMin = new float[2]; ///max zoom in and out values
    private Vector3 startpos;

    private void Start()
    {   
        startpos = cam.transform.position;
        setZoom(value);
    }   
    
    private void Update()
    {
        selectRoom();

        if(Input.GetMouseButtonDown(0))///left mouse button but later on needs to be drag
        {
            dragging = true;
            oldPos = transform.position;
            panOrigin = cam.ScreenToViewportPoint(Input.mousePosition);                    
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition) - panOrigin;    
            pos.x *= 1;
            transform.position = oldPos + -pos * dragSpeed;                                         
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, sideBounds[0], sideBounds[1]), Mathf.Clamp(transform.position.y, heightBounds[0], heightBounds[1]), startpos.z + value);

        if(Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
        {
            zoomIn(false); 
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
        {
            zoomIn(true);
        }
    }

    private void selectRoom()
    {
        if(Input.GetMouseButtonDown(1))
        { 
            RaycastHit hit; 
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //////later on needs to be the position of the tap on the screen
            Debug.DrawRay(Input.mousePosition, transform.forward * 800, Color.green);
            if(Physics.Raycast(ray,out hit,800.0f,roomMask)) 
            {
                Room newRoom = hit.transform.parent.GetComponent<Room>();
                Transform roomCenterPoint = hit.transform.parent.transform.GetChild(0).transform;
                setZoomLevel(160f);
                Vector3 newPos = new Vector3(roomCenterPoint.position.x,roomCenterPoint.position.y,this.transform.position.z);
                newPos.y += 5;
                this.transform.position = newPos;
                showBuilding(newRoom);
            }
        }
    }

    private void showBuilding(Room newRoom)
    {
        buildScript.setBuild(newRoom);
    }

    private void setZoomLevel(float amount)
    {
        Debug.Log(maxAndMin[0] + " " + maxAndMin[1]);
        if(amount >= maxAndMin[0] | amount <= maxAndMin[1])
        {
            value  = amount;
            setZoom(value);
            transform.localPosition = new Vector3(cam.transform.position.x,cam.transform.position.y,startpos.z + value);
        }else{
            Debug.Log("To much");
            ///to much zoom
        }
    }

    private void zoomIn(bool up)
    {
        float adding = zoomInValue * Time.deltaTime;

        if(up)
        {
            if(value - adding > maxAndMin[0])
            {
                value -= adding;
            }else{
                value = maxAndMin[0];
            }
            
            if(buildScript.shows())
            {
                if(value < 160f)
                {
                    buildScript.stopBuild();
                }
            }
        }else{
            if(value + adding < maxAndMin[1])
            {
                value += adding;
            }else{
                value = maxAndMin[1];
            }
        }

        setZoom(value);
        
        transform.localPosition = new Vector3(cam.transform.position.x,cam.transform.position.y,startpos.z + value);
    }

    public void goTo(Vector3 newPos)///goes to selected something
    {
        this.transform.position = newPos;
    }

    public void setZoom(float levelZoom)//needs to be optimizeddddd//////////////////////
    {
        float zoomProcentage = ((levelZoom - maxAndMin[1]) * 100) / (maxAndMin[0] - maxAndMin[1]);

        dragSpeed = dragSpeeds[0] - ((dragSpeeds[0] - dragSpeeds[1]) / 100 * zoomProcentage);

        //verticle
        float add1 = ((verticleBounds[0] - minverticleBounds[0]) / 100 * zoomProcentage);
        float add2 = ((verticleBounds[1] - minverticleBounds[1]) / 100 * zoomProcentage);

        heightBounds[0] = verticleBounds[0] - add1;
        heightBounds[1] = verticleBounds[1] - add2;

        //horizontal
        add1 = ((horizontalBounds[0] - minHorizontal[0]) / 100 * zoomProcentage);
        add2= ((horizontalBounds[1] - minHorizontal[1]) / 100 * zoomProcentage);

        sideBounds[0] = horizontalBounds[0] - add1;
        sideBounds[1] = horizontalBounds[1] - add2;
    }
}
