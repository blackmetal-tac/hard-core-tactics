using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using System;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent playerAgent;

    private GameObject clickMarker;
    private GameObject executeButton;
    private GameObject crosshair;
    private GameObject enemy;
    private LineRenderer walkPath;

    public float mechSpeed = 3.5f;
    private bool inMove;

    private float turnTime = 3;
    private float timeValue;
    private static TextMeshProUGUI timer;

    private float walkDistance = 3;

    private NavMeshPath path;

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();

        playerAgent = GetComponent<NavMeshAgent>();
        clickMarker = GameObject.Find("ClickMarker");
        clickMarker.transform.localScale = Vector3.zero;
        walkPath = GetComponent<LineRenderer>();
        executeButton = GameObject.Find("ExecuteButton");
        inMove = false;

        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        timeValue = turnTime;
        timer.text = timeValue.ToString();

        crosshair = GameObject.Find("Crosshair");
        enemy = GameObject.Find("Enemy");

        //Set path parameters
        walkPath.startWidth = 0.02f;
        walkPath.endWidth = 0.02f;
        walkPath.positionCount = 0;

        LeanTween.reset();
    }

    // Update is called once per frame
    void Update()
    {
        crosshair.transform.position = enemy.transform.position;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;            

            if (!inMove)
            {
                MoveToClick();                
            }
        }        

        //Timer display      
        timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");
        if (inMove && timeValue > 0)
        {            
            timeValue -= Time.deltaTime;
        }
        else
        {
            inMove = false;
            timeValue = turnTime;            
        }

        //Draw player path
        if (playerAgent.hasPath)
        {
            DrawPath();
        }
        else 
        {
            clickMarker.transform.localScale = Vector3.zero;
        }        

        clickMarker.transform.Rotate(new Vector3(0, 0, -Time.deltaTime * 50));       
    }

    private void MoveToClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit);

        if(isHit)
        {
            SetDestination(hit.point);           
        }       
    }

    //Draw click marker
    private void SetDestination(Vector3 target)
    {
        playerAgent.speed = 0;

        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
        for (int i = 0; i < path.corners.Length - 1; i++) 
        {
            float segmentDistance = (path.corners[i + 1] - path.corners[i]).magnitude;
            if (segmentDistance <= walkDistance)
            {
                playerAgent.SetDestination(target);
            }
            else
            {
                Vector3 finalPoint = path.corners[i] + ((path.corners[i + 1] - path.corners[i]).normalized * walkDistance);
                NavMesh.CalculatePath(transform.position, finalPoint, NavMesh.AllAreas, path);
                playerAgent.SetPath(path);                
                break;
            }
        }
    }

    //Constantly draws players path
    private void DrawPath()
    {
        walkPath.positionCount = playerAgent.path.corners.Length;
        walkPath.SetPosition(0, transform.position);

        if (playerAgent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < playerAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(playerAgent.path.corners[i].x, playerAgent.path.corners[i].y,
                    playerAgent.path.corners[i].z);
            walkPath.SetPosition(i, pointPosition);
            clickMarker.transform.position = new Vector3(pointPosition.x, 0.05f, pointPosition.z);
            clickMarker.transform.localScale = new Vector3(0, 0, 0);
            LeanTween.scale(clickMarker, Vector3.one * 0.2f, 0.2f);
        }
    }   
    
    //Start turn
    public void ExecuteOrder()
    {
        executeButton.GetComponent<AudioSource>().Play();
        LeanTween.scaleX(executeButton.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

        playerAgent.speed = mechSpeed;
        inMove = true;
    }
}
