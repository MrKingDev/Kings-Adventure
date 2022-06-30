using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSway : MonoBehaviour
{

    [Header("Sway Settings")]
    public bool toggleSway = true;
    [Space]
    public float amount;
    public float maxSway;
    public float smoothAmount;

    [Header("Tilt Settings")]
    public bool toggleTilt = true;
    [Space]
    public float tiltAmount;
    public float maxTiltSway;
    public float smoothAmountTilt;
    public bool tiltDirectionX, tiltDirectionY, tiltDirectionZ;

    Vector3 initialPosition;
    Quaternion initialRotation;


    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(toggleSway)
        {
            Sway();
        }

        if(toggleTilt)
        {
            TiltSway();
        }
    }

    void Sway()
    {
        float moveX = Input.GetAxis("Mouse X") * amount;
        float moveY = Input.GetAxis("Mouse Y") * amount;

        moveX = Mathf.Clamp(moveX, -maxSway, maxSway);

        Vector3 finialPos = new Vector3(moveX, 0, moveY);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finialPos + initialPosition, Time.deltaTime * smoothAmount);
    }

    void TiltSway()
    {
        float tiltY = Input.GetAxis("Mouse X") * tiltAmount;
        float tiltX = Input.GetAxis("Mouse Y") * tiltAmount;

        tiltY = Mathf.Clamp(tiltY, -maxTiltSway, maxTiltSway);
        tiltX = Mathf.Clamp(tiltX, -maxTiltSway, maxTiltSway);

        Quaternion finialRotation = Quaternion.Euler(new Vector3(tiltDirectionX ? -tiltX : 0, tiltDirectionY ? tiltY : 0, tiltDirectionZ ? tiltY : 0));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finialRotation * initialRotation, Time.deltaTime * smoothAmountTilt);
    }
}
