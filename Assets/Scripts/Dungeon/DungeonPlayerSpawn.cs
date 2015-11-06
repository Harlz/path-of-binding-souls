//---------------------------------
//	File:	DungeonPlayerSpawn.cs
//	Author: Harley Laurie
//	Brief:	Allows the player to spawn into
//  the dungeon after the generation or loading
//  has been completed.
//---------------------------------

using UnityEngine;
using System.Collections;

public class DungeonPlayerSpawn : MonoBehaviour
{
    private GameObject SpawnInstructions; //UI instructions telling the player how to spawn
    private bool PlayerCanSpawn; //Is the player allowed to spawn into the game yet?

    //Prefabs for spawning in the player
    public GameObject PlayerCharacterPrefab;
    public GameObject PlayerCameraPrefab;

    void Start()
    {
        SpawnInstructions = GameObject.Find("Spawn Instructions");
        PlayerCanSpawn = true;
    }

    void Update()
    {
        //Allow the player to spawn by pressing the return key
        if(PlayerCanSpawn)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                //Turn off the instructions telling the player how to spawn
                SpawnInstructions.SetActive(false);
                PlayerCanSpawn = false;
                //Get the room the player will spawn in
                DungeonRoom StartRoom = GetComponent<DungeonCreator>().GetStartRoom();
                //Spawn the player in the start room
                GameObject PlayerCharacter = (GameObject)Instantiate(PlayerCharacterPrefab, StartRoom.DungeonFloor.transform.position, Quaternion.identity);
                //Spawn the player camera near the player
                GameObject PlayerCamera = (GameObject)Instantiate(PlayerCameraPrefab, StartRoom.DungeonFloor.transform.position, Quaternion.identity);
                //Set the player cameras target
                PlayerCamera.GetComponent<PlayerCamera>().Target = PlayerCharacter.transform.FindChild("CameraPivot");
                //Turn off the main camera
                GameObject.Find("Main Camera").SetActive(false);
            }
        }
    }

    public void EnablePlayerSpawning()
    {
        print("Player Spawning enabled");
        PlayerCanSpawn = true;
    }
}