//---------------------------------
//	File:	Teleporter.cs
//	Author: Harley Laurie
//	Brief:	Allows the player to teleport to the dungeon they are up to
//---------------------------------

using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    private bool PlayerInside; //Is there a player standing on this teleporter?
    private GameObject TeleporterInstructions; //UI Element instructing the player on how to operate the teleporter
    private GameObject ParticleSystem; //The particle effect emitter used to indicate the teleporter is active

    void Start()
    {
        PlayerInside = false;
        TeleporterInstructions = GameObject.Find("TeleporterInstructions");
        TeleporterInstructions.SetActive(false);
        ParticleSystem = transform.FindChild("Particles").gameObject;
        ParticleSystem.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            PlayerInside = true;
            TeleporterInstructions.SetActive(true);
            ParticleSystem.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            PlayerInside = false;
            TeleporterInstructions.SetActive(false);
            ParticleSystem.SetActive(false);
        }
    }

    void Update()
    {
        //If the player is standing inside the teleporter, allow them to enter the
        //dungeon by pressing the action button
        if(PlayerInside)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("Teleporting to dungeon...");
                Application.LoadLevel("Dungeon");
            }
        }
    }
}