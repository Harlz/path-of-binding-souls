//---------------------------------
//	File:	FindAngle.cs
//	Author: Harley Laurie
//	Brief:	Taking 3 position vectors A,B and C
//	returns the angle ABC in degrees
//---------------------------------

using UnityEngine;
using System.Collections;

public class FindAngle : MonoBehaviour 
{
	public float FindAngleABC(Vector3 A, Vector3 B, Vector3 C)
	{
		return Vector3.Angle (C - B, A - B);
	}
}