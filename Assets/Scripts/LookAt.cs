//---------------------------------
//	File:	LookAt.cs
//	Author: Harley Laurie
//	Brief:	
//---------------------------------

using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour 
{
	public Transform Target;

	void Update()
	{
		transform.LookAt (Target.position);
	}
}