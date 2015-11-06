//---------------------------------
//	File:	PlayerSpawning.cs
//	Author: Harley Laurie
//	Brief:	Sets the location of player characters
//  when they are spawned into the scene.
//---------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerSpawning : MonoBehaviour
{
    public GameObject PlayerPrefab;


    public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        print("player spawn");
        //var player = (GameObject)GameObject.Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        //NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}