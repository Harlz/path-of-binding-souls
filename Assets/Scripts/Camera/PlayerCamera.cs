//---------------------------------
//	File:	PlayerCamera.cs
//	Author: Harley Laurie
//	Brief:	
//---------------------------------

using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public Transform Target;
    public float Distance = 10.0f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20.0f;
    public float yMaxLimit = 80.0f;

    float distanceMin = 3;
    float distanceMax = 15;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.x;
        y = angles.y;

        //Make the rigidbody not change rotation
        if (transform.GetComponent<Rigidbody>() != null)
            transform.GetComponent<Rigidbody>().freezeRotation = false;
    }

    void LateUpdate()
    {
        //If the camera has a target and the player is holding the LMB
        //let them control the position and facing direction of the camera
        if (Target && Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * Distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }
        y = ClampAngle(y, yMinLimit, yMaxLimit);
        Quaternion Rotation = Quaternion.Euler(y, x, 0);

        Distance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
        /*
        RaycastHit Hit;
        if (Physics.Linecast(Target.position, transform.position, out Hit))
            Distance -= Hit.distance;*/

        Vector3 Position = Rotation * new Vector3(0.0f, 0.0f, -Distance) + Target.position;

        transform.rotation = Rotation;
        transform.position = Position;
    }

    static float ClampAngle ( float Angle, float Min, float Max)
    {
        if (Angle < -360)
            Angle += 360;
        if (Angle > 360)
            Angle -= 360;
        return Mathf.Clamp(Angle, Min, Max);
    }
}