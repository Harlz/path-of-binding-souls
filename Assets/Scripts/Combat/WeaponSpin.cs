//---------------------------------
//	File:	WeaponSpin.cs
//	Author: Harley Laurie
//	Brief:	
//---------------------------------

using UnityEngine;
using System.Collections;

public class WeaponSpin : MonoBehaviour
{
    public float RotationSpeed = 15.0f;

    void Update ()
    {
        if(Input.GetMouseButton(1))
            transform.RotateAround(transform.position, transform.parent.right, Time.deltaTime * RotationSpeed);
    }
}