using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using System;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    private Camera camMain;
    private static TextMeshProUGUI timer;

    //NavMesh
    private NavMeshAgent playerAgent;
    private NavMeshPath path;
    private LineRenderer walkPath;

    //Objects
    private GameObject clickMarker, executeButton, buttonFrame, crosshair, enemy, playerUI;
    public GameObject projectile, firePoint;    

    //Audio
    private AudioSource audioUI;
    public AudioClip buttonClick;

    //Unit stats
    public static float mechSpeed = 3.5f;
    public static bool inAction;
    private float walkDistance = 3f;
    private float turnTime = 3f;
    private float timeValue;

    //Attack parameters
    ObjectPooler objectPooler;
    public float fireDelay = 1f;
    public int burstSize = 3;
    public float fireRate = 1f;

    // Start is called before the first frame update
    void Start()
    {
        camMain = Camera.main;
        objectPooler = ObjectPooler.Instance;

        //Navmesh setup
        path = new NavMeshPath();
        playerAgent = GetComponent<NavMeshAgent>();

        //Path
        clickMarker = GameObject.Find("ClickMarker");
        clickMarker.transform.localScale = Vector3.zero;
        walkPath = GetComponent<LineRenderer>();
        walkPath.startWidth = 0.02f;
        walkPath.endWidth = 0.02f;
        walkPath.positionCount = 0;

        //UI
        executeButton = GameObject.Find("ExecuteButton");
        audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        buttonFrame = executeButton.transform.GetChild(1).gameObject;
        inAction = false;
        playerUI = GameObject.Find("PlayerUI");

        //Turn timer
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        timeValue = turnTime;
        timer.text = timeValue.ToString();

        //Set target
        crosshair = GameObject.Find("Crosshair");
        enemy = GameObject.Find("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        //Crosshair position
        crosshair.transform.position = camMain.WorldToScreenPoint(enemy.transform.position);
        playerUI.transform.position = camMain.WorldToScreenPoint(transform.position);

        //Mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!inAction)
            {
                MoveToClick();
            }
        }

        //If in ACTION PHASE
        if (inAction)
        { 
            this.FireBurst(objectPooler, "Bullet", firePoint, fireDelay, burstSize, fireRate);
        } 

        //Timer display      
        timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");
        if (inAction && timeValue > 0)
        {            
            timeValue -= Time.deltaTime;
        }
        else
        {
            inAction = false;
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
            clickMarker.transform.localScale = Vector3.zero;
            clickMarker.transform.DOScale(Vector3.one * 0.2f, 0.2f);
        }
    }   
    
    //Start turn
    public void ExecuteOrder()
    {
        audioUI.PlayOneShot(buttonClick);
        buttonFrame.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.buttonDelay, () => {
            buttonFrame.transform.DOScaleX(1f, 0.1f);            
        });

        playerAgent.speed = mechSpeed;
        inAction = true;
    }
}
