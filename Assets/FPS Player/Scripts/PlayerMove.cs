using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;
    [SerializeField] private float movementSpeed;

    private GameObject player;
    private CharacterController characterControler;

    [SerializeField] private KeyCode duckKey;
    [SerializeField] private float duckingHeight;
    [SerializeField] private float stadingHeight;
    [SerializeField] private float duckingSpeed;

    private bool duck = false;

    // Start is called before the first frame update
    void Start()
    {
        characterControler = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        float horizInput = Input.GetAxis(horizontalInputName);
        float vertInput = Input.GetAxis(verticalInputName);

        Vector3 rightMovement = transform.right * horizInput;
        Vector3 forwadMovement = transform.forward * vertInput;

        characterControler.SimpleMove(Vector3.ClampMagnitude(forwadMovement + rightMovement, 1.0f) * movementSpeed);

        DuckInput();
    }

    private void DuckInput()
    {
        if (Input.GetKeyDown(duckKey))
        {
            if (!duck)
            {
                duck = true;
                movementSpeed = movementSpeed / 2;
            }
            else
            {
                Ray ray = new Ray(transform.position, transform.up);
                RaycastHit hitInfo;
                if (!Physics.Raycast(ray, out hitInfo, 1))
                {
                    duck = false;
                    movementSpeed = movementSpeed * 2;
                }
            }
        }
        if (duck)
        {
            if (characterControler.height > duckingHeight)
            {
                characterControler.height -= duckingSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (characterControler.height < stadingHeight)
            {
                characterControler.height += duckingSpeed * Time.deltaTime;
                transform.Translate(0, duckingSpeed / 2 * Time.deltaTime, 0);
            }
        }
    }
}
