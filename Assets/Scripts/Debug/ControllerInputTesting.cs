//---------------------------------
//	File:	ControllerInputTesting.cs
//	Author: Harley Laurie
//	Brief:	
//---------------------------------

using UnityEngine;
using System.Collections;

public class ControllerInputTesting : MonoBehaviour
{
    void Update ()
    {
        if (Input.GetButton("Sprint"))
            print("sprint");
	}
}