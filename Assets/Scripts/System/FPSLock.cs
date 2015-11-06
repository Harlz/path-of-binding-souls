//---------------------------------
//	File:	FPSLock.cs
//	Author: Harley Laurie
//	Brief:	
//---------------------------------

using UnityEngine;
using System.Collections;

public class FPSLock : MonoBehaviour 
{
	public bool EnableVSYNC = true;
	public int MaxFPS = 60;

	void Awake()
	{
		QualitySettings.vSyncCount = EnableVSYNC ? 1 : 0;
		Application.targetFrameRate = MaxFPS;
	}
}