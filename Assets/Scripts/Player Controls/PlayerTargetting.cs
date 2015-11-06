//---------------------------------
//	File:	PlayerTargetting.cs
//	Author: Harley Laurie
//	Brief:	Allows the player to lock onto
//  a target for help in combat.
//---------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Need for List
using UnityEngine.UI;

public class PlayerTargetting : MonoBehaviour 
{
	//How close must an enemy be for us to target it?
	public float m_maxTargetDistance;
	//Do we currently have a target locked?
	private bool m_targetLocked = false;
	//Our current target
	private GameObject m_target = null;
	//Targetting reticle
	public Image m_reticle;
	private RectTransform m_reticleRect;
	private RectTransform m_canvasRect;

	//Getters
	public bool HasTarget()
	{
		return m_targetLocked;
	}
	public GameObject GetTarget()
	{
		return m_target;
	}

	void Start()
	{
		//Disable the target reticle until the player acquires a target
        if(m_reticle)
        {
            m_reticle.enabled = false;
            m_reticleRect = m_reticle.rectTransform;
            m_canvasRect = m_reticle.canvas.GetComponent<RectTransform>();
        }
	}

	void Update()
	{
		if(Input.GetMouseButtonDown (2))
		{
			if(!m_targetLocked)
				//If the player has no target, allow them to acquire a new target
				AcquireNewTarget();
			else
				//Otherwise, drop the current target
				DropTarget ();
		}

		//If we have a target, keep the target reticle hovering over them
		if (m_targetLocked)
			UpdateReticle ();
	}

	private void AcquireNewTarget()
	{
		//Find all enemies in the scene
		GameObject[] l_potentialTargets = GameObject.FindGameObjectsWithTag ("Enemy");
		//Create a list to store targets that are close enough
		List<GameObject> l_targetsInRange = new List<GameObject> ();
		//And a second list to store targets in range, that can be seen
		List<GameObject> l_visibleTargetsInRange = new List<GameObject> ();
		//Remove the targets that are too far away
		for ( int i = 0; i < l_potentialTargets.Length; i++ )
		{
			//Find the distance
			float l_distance = Vector3.Distance( l_potentialTargets[i].transform.position, transform.position );
			//If its close enough, add it to the list l_targetsInRange
			if ( l_distance <= m_maxTargetDistance )
				l_targetsInRange.Add(l_potentialTargets[i]);
		}
		//Find which enemies are in view
		foreach ( GameObject Enemy in l_targetsInRange )
		{
			Vector3 viewPos = Camera.main.WorldToViewportPoint(Enemy.transform.position);

			//If the X and Y of the returned point is between 0 and 1, and the z is positive, then the
			//point is in front of the camera
			if ( ( viewPos.x >= 0.0f ) && ( viewPos.x <= 1.0f ) )
			{
				if ( ( viewPos.y >= 0.0f ) && ( viewPos.y <= 1.0f ) )
				{
					if ( viewPos.z >= 0.0f )
					{
						//Now that we know this target is in front of the camera, we need to test if its visible
						RaycastHit hit;
						if ( Physics.Raycast ( Camera.main.transform.position, Enemy.transform.position - Camera.main.transform.position, out hit) )
						{
							if ( hit.transform.tag == "Enemy" )
							{
								//Add these enemies to the l_visibleTargetsInRangeList
								l_visibleTargetsInRange.Add(Enemy.gameObject);
							}
						}
					}
				}
			}
		}
		//Find which visible target in range is going to be our new target
		GameObject l_newTarget = null;
		float l_dot = -2.0f;
		foreach (GameObject Enemy in l_visibleTargetsInRange)
		{
			Vector3 l_localPoint = Camera.main.transform.InverseTransformPoint(Enemy.transform.position).normalized;
			Vector3 l_forward = Vector3.forward;
			float l_test = Vector3.Dot (l_localPoint, l_forward);
			if(l_test > l_dot)
			{
				l_dot = l_test;
				l_newTarget = Enemy;
			}
		}
		if(l_newTarget != null)
		{
			//We have our new target
			m_targetLocked = true;
			m_target = l_newTarget;
			//Enable the targetting reticle
			m_reticle.enabled = true;	
		}
	}

	private void DropTarget()
	{
		m_targetLocked = false;
		m_target = null;
		//Enable the targetting reticle
		m_reticle.enabled = false;
	}

	private void UpdateReticle()
	{
		Vector2 ViewportPosition= Camera.main.WorldToViewportPoint(m_target.transform.position);
		Vector2 WorldObject_ScreenPosition = new Vector2(
			((ViewportPosition.x*m_canvasRect.sizeDelta.x)-(m_canvasRect.sizeDelta.x*0.5f)),
			((ViewportPosition.y*m_canvasRect.sizeDelta.y)-(m_canvasRect.sizeDelta.y*0.5f)));
		
		//now you can set the position of the ui element
		m_reticleRect.anchoredPosition = WorldObject_ScreenPosition;
	}
}