using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private Rigidbody2D _myBody;
    private float _movementX;
    private bool _isGrounded = true;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMoveKeyboard();
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && _isGrounded)
        {
            _isGrounded = false;
            _myBody.velocity = Vector2.up * jumpForce;
        }
    }

    void PlayerMoveKeyboard() {

        _movementX = Input.GetAxisRaw("Horizontal");

        transform.position += moveForce * Time.deltaTime * new Vector3(_movementX, 0f, 0f) ;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
         {
            _isGrounded = true;
        }

    }

}
