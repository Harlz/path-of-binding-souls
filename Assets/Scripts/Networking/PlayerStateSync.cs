//---------------------------------
//	File:	PlayerStateSync.cs
//	Author: Harley Laurie
//	Brief:	Syncs the players position over the network
//---------------------------------

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerStateSync : NetworkBehaviour
{
    [SyncVar]
    Vector3 PlayerPos;

    NetworkIdentity ID;

    void Start()
    {
        //Get the ID
        ID = GetComponent<NetworkIdentity>();
    }

    void Update()
    {
        
        //If we are the owner, send this players location over the network
        if (ID.isLocalPlayer)
        {
            PlayerPos = transform.position; 
            //print("network sync");
            print(PlayerPos);
        }
        else
        {
            transform.position = PlayerPos;
        }
    }
}