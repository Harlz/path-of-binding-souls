//---------------------------------
//	File:	Menu.cs
//	Author: Harley Laurie
//	Brief:	
//---------------------------------

using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    public void Singleplayer()
    {
        Application.LoadLevel("town");
    }

    public void Multiplayer()
    {
        Application.LoadLevel("network connect");
    }
}