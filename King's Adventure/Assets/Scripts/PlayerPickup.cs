using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{

    [SerializeField] LayerMask pickupMask;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform pickupTarget;
    [Space]
    [SerializeField] float pickupRange;
    Rigidbody currentObject;

    [Header("Controls")]
    [SerializeField] KeyCode pickupKey = KeyCode.E;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(pickupKey))
        {

            if(currentObject)
            {
                currentObject.useGravity = true;
                currentObject = null;
                return;
            }

            Ray CameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if(Physics.Raycast(CameraRay, out RaycastHit HitInfo, pickupRange, pickupMask))
            {
                currentObject = HitInfo.rigidbody;
                currentObject.useGravity = false;
            }
        }
    }

    void FixedUpdate()
    {
        if(currentObject)
        {
            Vector3 DirectionToPoint = pickupTarget.position - currentObject.position;
            float DistanceToPoint = DirectionToPoint.magnitude;

            currentObject.velocity = DirectionToPoint * 12f * DistanceToPoint;
        }
    }
}
