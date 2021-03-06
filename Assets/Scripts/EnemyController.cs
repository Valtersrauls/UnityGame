﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    Animator animator;

    private enum AIStatus { Patrol, Follow, Wait }

    private const float CloseEnoughWaypointDistance = 1.0f;

    [Range(0f, 1f)]
    [SerializeField]
    private float closeEnoughFollowDistance = 0.5f;

    [Range(1f, 100f)]
    [SerializeField]
    private float maxSightDistance = 10f;

    [Range(1f, 360f)]
    [SerializeField]
    private float fieldOfViewAnglePatrol = 60f;

    [Range(1f, 360f)]
    [SerializeField]
    private float fieldOfViewAngleFollow = 180f;

    [Range(0f, 30f)]
    [SerializeField]
    private float loseInterestTime = 5f;

    [Range(0f, 30f)]
    [SerializeField]
    private float patrolWaitTime = 5f;

    [SerializeField]
    private Waypoint[] _waypoints;

    private AIStatus aiStatus = AIStatus.Patrol;

    private GameObject _player;
    private NavMeshAgent _navMeshAgent;

    private uint currentWaypointIndex = 0;
    private Vector3 destinationPosition;
    private float distanceToDestination;
    private float accumulatedLoseInterestRate = 0f;
    private float accumulatedWaitTime = 0f;

    private float FieldOfViewAngle
    {
        get
        {
            switch (aiStatus)
            {
                case AIStatus.Follow:
                    return fieldOfViewAngleFollow;
                case AIStatus.Patrol:
                    return fieldOfViewAnglePatrol;
                case AIStatus.Wait:
                    return fieldOfViewAngleFollow;
                default:
                    throw new System.Exception("Unsupported AiStatus: " + aiStatus);
            }
        }
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _navMeshAgent = GetComponent<NavMeshAgent>();
        //_waypoints = FindObjectsOfType<Waypoint>();

        UpdateTargetWaypointDestination();
        UpdateNavMeshAgentDestinationPosition();

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        distanceToDestination = Vector3.Distance(transform.position, destinationPosition);

        switch (aiStatus)
        {
            case AIStatus.Patrol:
                animator.SetBool("walking", true);
                PerformPatrol();
                break;
            case AIStatus.Follow:
                animator.SetBool("walking", true);
                PerformFollow();
                break;
            case AIStatus.Wait:
                animator.SetBool("walking", false);
                PerformWait();
                break;
        }

    }

    /// Tests if player can be seen
    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 direction = _player.transform.position - transform.position;

        //Check if raycast can hit player (no obstacles in between)
        if (Physics.Raycast(transform.position, direction, out hit, maxSightDistance))
        {
            if (hit.transform.tag == "Player") //check if other object tag is Player to avoid conflicts with scene geometry
            {
                //Check if direction is within FOV angle
                if ((Vector3.Angle(direction, transform.forward)) <= FieldOfViewAngle) return true;
            }
        }

        return false;
    }

    /// Follow Player. If not player not seen - move to last known pos
    private void PerformFollow()
    {
        bool canSeePlayer = CanSeePlayer();
        bool closeEnoughToDestination = distanceToDestination <= closeEnoughFollowDistance;

        if (canSeePlayer)
        {
            //Can see player - update position

            destinationPosition = _player.transform.position;
            UpdateNavMeshAgentDestinationPosition();

            accumulatedLoseInterestRate = 0f;
        }
        else
        {
            if (closeEnoughToDestination)
            {
                accumulatedLoseInterestRate += Time.deltaTime;
                if (accumulatedLoseInterestRate >= loseInterestTime) ResumePatrolling(true);
            }
        }
    }

    /// Perform patrolling between points
    private void PerformPatrol()
    {
        //If player has been seen while patrolling - start to follow
        if (CanSeePlayer()) StartFollowing();

        if (distanceToDestination <= CloseEnoughWaypointDistance)
        {
            StartWaiting();
            //MoveNextWaypoint();
            //UpdateTargetWaypointDestination();
            //UpdateNavMeshAgentDestinationPosition();
        }
    }

    private void PerformWait()
    {
        accumulatedWaitTime += Time.deltaTime;
        if (CanSeePlayer())
        {
            accumulatedWaitTime = 0f;
            StartFollowing();
        }
        if (accumulatedWaitTime >= patrolWaitTime)
        {
            accumulatedWaitTime = 0f;
            MoveNextWaypoint();
            UpdateTargetWaypointDestination();
            UpdateNavMeshAgentDestinationPosition();
            ResumePatrolling(false);
        }
    }

    private void StartFollowing()
    {
        Debug.Log("Spotted player while patrolling! Commence follow!");
        aiStatus = AIStatus.Follow;
    }

    private void ResumePatrolling(bool wasFollowing)
    {
        Debug.Log("AI lost interest - resuming patrol");
        aiStatus = AIStatus.Patrol;
        if (wasFollowing)
        {
            FindClosestWaypoint();
            UpdateNavMeshAgentDestinationPosition();
        }  
    }

    private void StartWaiting()
    {
        Debug.Log("AI is waiting");
        aiStatus = AIStatus.Wait;
    }

    private void UpdateNavMeshAgentDestinationPosition()
    {
        _navMeshAgent.SetDestination(destinationPosition);
    }

    #region Patrol Methods

    private void FindClosestWaypoint()
    {
        uint closestIndex = 0;
        float foundMinDistance = float.MaxValue;

        for (uint i = 0; i < _waypoints.Length; i++)
        {
            Waypoint waypoint = _waypoints[i];
            float dist = Vector3.Distance(transform.position, waypoint.transform.position);

            if (dist < foundMinDistance)
            {
                foundMinDistance = dist;
                closestIndex = i;
            }
        }

        currentWaypointIndex = closestIndex;
    }

    private void MoveNextWaypoint()
    {
        if (++currentWaypointIndex >= _waypoints.Length)
        {
            currentWaypointIndex = 0;
            Debug.Log("Waypoints reseted");
        }
    }
    private void UpdateTargetWaypointDestination()
    {
        Waypoint waypoint = _waypoints[currentWaypointIndex];
        Debug.Log("Waypoint updated");
        destinationPosition = waypoint.transform.position;
    }
    #endregion

}
