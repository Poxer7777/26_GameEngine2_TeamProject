using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerControl : MonoBehaviour
{
    public GameObject player;
    CharacterController controller;

    float horizontalInput;
    float verticalInput;
    Vector3 move;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (SansManager.Instance.isGame)
        {
            move = transform.right * horizontalInput + transform.forward * verticalInput;
            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        if (move.magnitude > 0.05f)
        {
            Vector3 lookDirection = new Vector3(move.x, 0, move.z);

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            float rotationSpeed = 15f;
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
