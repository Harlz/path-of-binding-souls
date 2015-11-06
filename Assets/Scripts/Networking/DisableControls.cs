//---------------------------------
//	File:	DisableControls.cs
//	Author: Harley Laurie
//	Brief:	When a player is spawned into a network game
//  we want to disable the controls for that player if the
//  local player does not have access.
//---------------------------------

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DisableControls : MonoBehaviour
{
    void Start ()
    {
        //Get the network identity component
        NetworkIdentity ID = GetComponent<NetworkIdentity>();
        //Check if we aren't the owner of this object
        if(!ID.isLocalPlayer)
        //Disable the controls for this player if its not ours
        {
            print("Remote player found, disabling their control scripts");
            GetComponent<PlayerController>().enabled = false;
            GetComponent<PlayerTargetting>().enabled = false;
        }
	}
}