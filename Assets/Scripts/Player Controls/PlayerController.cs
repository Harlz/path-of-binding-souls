//---------------------------------
//	File:	PlayerController.cs
//	Author: Harley Laurie
//	Brief:	
//---------------------------------

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    //Debug
    public bool DrawMovementVector = false;

    //Movement
    public float m_freeTurnSpeed = 300;
    public float m_lockedTurnSpeed = 3;
    public float m_moveSpeed = 3;
    public float m_sprintSpeed = 5;
    public float m_gravity = 0.1f;

    public bool m_canJump = true;
    public float m_jumpHeight = 3.0f;
    private float m_yVelocity = 0.0f;

	private PlayerTargetting m_playerTargetting;

	void Start()
	{
		m_playerTargetting = transform.GetComponent<PlayerTargetting> ();
	}

	void Update()
	{
		//Check if we have a target locked and change the
		//player controls depending on that
        if (m_playerTargetting && m_playerTargetting.HasTarget())
			LockedControls ();
		else if(m_playerTargetting)
			FreeControls();
	}

	private void FreeControls()
	{
		//Find the players movement speed based on input if they player
		//is holding the sprint button
		float l_moveSpeed = m_moveSpeed;
		//Check if the player is holding down the sprint button
		if ((Input.GetKey (KeyCode.LeftShift)) || Input.GetButton ("Sprint"))
			//Increase move speed if they are
			l_moveSpeed = m_sprintSpeed;

		//Get player input
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		//Calculate the players movement axes relative to the camera
		Vector3 l_leftRightMoveAxis = Vector3.Cross (transform.up, Camera.main.transform.forward);
		l_leftRightMoveAxis.Normalize ();
		Vector3 l_forwardBackMoveAxis = Vector3.Cross (l_leftRightMoveAxis, transform.up);
		//Normalize these vectors
		l_forwardBackMoveAxis.Normalize ();
		
        //Draw movement vectors
        if(DrawMovementVector)
        {
            Debug.DrawRay(transform.position, l_leftRightMoveAxis, Color.red);
            Debug.DrawRay(transform.position, l_forwardBackMoveAxis, Color.blue);
        }

		//Calculate the players movement vector based on their input
		//and the movement axes we just calculated
		Vector3 l_playerMovement = h * l_leftRightMoveAxis + v * l_forwardBackMoveAxis;

        CharacterController Controller = GetComponent<CharacterController>();

        //Allow the player to jump by pressing space-bar if they are on the ground
        if (Controller.isGrounded && Input.GetKey(KeyCode.Space) && m_canJump)
            m_yVelocity = m_jumpHeight;

        //Apply gravity if the player is not on the ground
        if(!Controller.isGrounded)
            m_yVelocity -= m_gravity;

        l_playerMovement.y += m_yVelocity;

        //Apply the new movement vector
        Controller.Move (l_playerMovement * l_moveSpeed * Time.deltaTime);
        
        //Rotate towards movement direction
        if (l_playerMovement.x != 0 || l_playerMovement.z != 0)
		{
			Quaternion wanted_rotation = Quaternion.LookRotation(l_playerMovement);
            Vector3 Eulers = wanted_rotation.eulerAngles;
            Eulers.x = transform.rotation.x;
            wanted_rotation.eulerAngles = Eulers;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, wanted_rotation, m_freeTurnSpeed * Time.deltaTime);
		}
	}

	private void LockedControls()
	{
		//move direction is relative to the current target
		Vector3 l_forward = m_playerTargetting.GetTarget ().transform.position - transform.position;
		l_forward.y = 0;
		l_forward = l_forward.normalized;
		Vector3 l_right = new Vector3 (l_forward.z, 0, -l_forward.x);
		//Get player input
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		Vector3 l_moveDirection = (h * l_right + v * l_forward).normalized;

        //Apply the new movement vector
        GetComponent<CharacterController>().Move(l_moveDirection * m_moveSpeed * Time.deltaTime);

		//Rotate towards our current target
		//Find the position we will rotate towards
		Vector3 l_targetPosition = m_playerTargetting.GetTarget ().transform.position;
		l_targetPosition.y = transform.position.y;
		//Get this position relative to ours
		Vector3 l_relativePosition = l_targetPosition - transform.position;
		//Find the desired rotation
		Quaternion l_targetRotation = Quaternion.LookRotation (l_relativePosition);
		//Rotate towards it
		transform.rotation = Quaternion.Slerp (transform.rotation, l_targetRotation, m_lockedTurnSpeed * Time.deltaTime);
	}
}