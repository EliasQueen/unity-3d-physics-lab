using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Player Properties")]
    public float Movement_Speed;

    public float InteractionRange = 5.0f;

    [Header("Camera Properties")]
    public Transform Camera;
    public float Camera_Sensitivity = 5.0f;
    private float camera_rotationX = 0.0f;
    private float camera_rotationY = 0.0f;
    private float camera_maxYAngle = 80.0f;
    private PlayerCamera playerCamera;
    public bool Is_Freezed = false;

    void Start() {
        this.Movement_Speed = 5.0f;
        playerCamera = Camera.GetComponent<PlayerCamera>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            playerCamera.is_Locked = !playerCamera.is_Locked;
        }
        
        if(!playerCamera.is_Locked)
        {
            this.Movement_Speed = Input.GetKey(KeyCode.LeftShift) ? 7.5f : 5.0f;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, 0, vertical);

            if (movement.magnitude > 1)
            {
                movement.Normalize();
            }

            transform.Translate(movement * Time.deltaTime * Movement_Speed, Space.Self);

            CameraHandler();
        }
    }

    private void CameraHandler() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        camera_rotationX -= mouseY * Camera_Sensitivity;
        camera_rotationX = Mathf.Clamp(camera_rotationX, -camera_maxYAngle, camera_maxYAngle);

        camera_rotationY += mouseX * Camera_Sensitivity;

        Camera.localRotation = Quaternion.Euler(camera_rotationX, camera_rotationY, 0.0f);

        Camera.localPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        transform.Rotate(Vector3.up * mouseX * Camera_Sensitivity);
    }
}
