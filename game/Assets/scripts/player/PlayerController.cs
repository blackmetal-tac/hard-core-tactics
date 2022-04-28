using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent playerAgent;

    private GameObject clickMarker;
    private LineRenderer walkPath;

    // Start is called before the first frame update
    void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
        clickMarker = GameObject.Find("ClickMarker");
        clickMarker.transform.localScale = new Vector3(0, 0, 0);
        walkPath = GetComponent<LineRenderer>();

        walkPath.startWidth = 0.02f;
        walkPath.endWidth = 0.02f;
        walkPath.positionCount = 0;

        LeanTween.reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveToClick();
        }

        if (Vector3.Distance(playerAgent.destination, transform.position) <= playerAgent.stoppingDistance)
        {
            clickMarker.transform.localScale = new Vector3(0, 0, 0);
        }
        else if (playerAgent.hasPath)
        {
            DrawPath();
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

    private void SetDestination(Vector3 target)
    {
        playerAgent.SetDestination(target);
        clickMarker.transform.position = new Vector3(target.x, 0.2f, target.z);
        clickMarker.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(clickMarker, new Vector3(0.2f, 0.2f, 0.2f), 0.2f);
    }

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

        }
    }
}
