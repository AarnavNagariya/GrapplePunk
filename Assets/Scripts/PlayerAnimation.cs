using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private int runningHash;
    private int inAirHash;


    private PlayerMovement movementScript;

    public bool isFacingLeft;
    private Vector2 facingLeft;

    [SerializeField] private bool spawnFacingLeft;
    [SerializeField] private float animationMinSpeed;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        runningHash = Animator.StringToHash("Running");
        inAirHash = Animator.StringToHash("InAir");
        movementScript = GetComponent<PlayerMovement>();
        facingLeft = new Vector2(-transform.localScale.x, transform.localScale.y);
        if (spawnFacingLeft)
        {
            transform.localScale = facingLeft;
            isFacingLeft = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(runningHash, Mathf.Abs(movementScript.horizontalSpeed) > animationMinSpeed);
        animator.SetBool(inAirHash, !movementScript.grounded);
    }

    void FixedUpdate()
    {
        if (movementScript.horizontalSpeed > 0 && isFacingLeft)
        {
            isFacingLeft = false;
            Flip();
        }

        if (movementScript.horizontalSpeed < 0 && !isFacingLeft)
        {
            isFacingLeft = true;
            Flip();
        }
    }

    void Flip()
    {
        transform.localScale = isFacingLeft ? facingLeft : new Vector2(-transform.localScale.x, transform.localScale.y);
    }
}