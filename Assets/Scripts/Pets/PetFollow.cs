//---------------------------------
//	File:	PetFollow.cs
//	Author: Harley Laurie
//	Brief:	
//---------------------------------

using UnityEngine;
using System.Collections;

public class PetFollow : MonoBehaviour
{
    Transform PlayerTransform;
    
    public int FollowCloseLimit; //Stops following the player when we get this close
    public int FollowFarLimit; //Starts moving towards the player when we are this far away
    public int ResetDistance; //If the pet gets too far away it teleports back to the player

    private bool Following; //Are we moving towards the player right now?

    void Start()
    {
        PlayerTransform = GameObject.Find("Player").transform;
    }

    void Update()
    {
        float PlayerDistance = Vector3.Distance(transform.position, PlayerTransform.position);

        //Follow the player if we are too far away but dont follow if were too close
        Following = PlayerDistance > FollowFarLimit ? true :
            PlayerDistance < FollowCloseLimit ? false : true;

        if (Following)
        {
            GetComponent<NavMeshAgent>().Resume();
            GetComponent<NavMeshAgent>().SetDestination(PlayerTransform.position);
        }
        else
            GetComponent<NavMeshAgent>().Stop();

        transform.LookAt(PlayerTransform);
    }
}