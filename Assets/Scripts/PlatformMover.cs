using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] private Vector2 endPosition;
    private Vector2 startPosition;
    private float t;
    [SerializeField] private float tOffset = 0;
    private bool incT;
    [SerializeField] private float speed;
    
    void Start()
    {
        startPosition = transform.position;
        t = tOffset;
        incT = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (incT)
        {
            t += speed * Time.fixedDeltaTime;
            if (t > 1)
            {
                t = 2 - t;
                incT = false;
            }
        }
        else
        {
            t -= speed * Time.fixedDeltaTime;
            if (t < 0)
            {
                t *= -1;
                incT = true;
            }
        }

        transform.position = Vector2.Lerp(startPosition, endPosition, t);
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        col.gameObject.transform.SetParent(gameObject.transform,true);
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        col.gameObject.transform.parent = null;
    }
}
