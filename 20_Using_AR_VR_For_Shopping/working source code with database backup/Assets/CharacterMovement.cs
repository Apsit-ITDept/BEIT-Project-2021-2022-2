using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Vector3 directionToMove;
    float gravity;
    CharacterController characterController;

    Transform mainCameraTransform;
    public float speed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        gravity = -9.8f;
        directionToMove = Vector3.zero;
        characterController = gameObject.GetComponent<CharacterController>();
        mainCameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!characterController.isGrounded)
        {
            directionToMove.y += gravity * Time.deltaTime;
        } else {
            if (mainCameraTransform.eulerAngles.x >= 54f && mainCameraTransform.eulerAngles.x <= 65f)
            {
                directionToMove = mainCameraTransform.TransformDirection(Vector3.forward);
                directionToMove += directionToMove * speed;
            } else {
                directionToMove = Vector3.zero;
            }
        }

        characterController.Move(directionToMove * Time.deltaTime); 
    }
}
