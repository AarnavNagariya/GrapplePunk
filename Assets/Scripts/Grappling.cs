using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Grappling : MonoBehaviour
{
    public Camera MainCamera;
    public LineRenderer lr;
    public LineRenderer lr2;
    public DistanceJoint2D distancejoint;
    public GameObject gunholder;
    public float grappablelayer = 2f;    
    public Rigidbody2D _myBody;
    [SerializeField] private float MaxDistanceForGrapple;
    [SerializeField] private float MaxAngleToRotate;

    private Vector3 temp;

    int playerLayer;
    int layerMask;
    
    void Start()
    {
        distancejoint.enabled = false;
        lr.enabled = false;
        lr2.enabled = false;
        playerLayer = LayerMask.NameToLayer("Player");
        layerMask = ~(1 << playerLayer);
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Level_1");
        }

        Vector2 mousePos = (Vector2)MainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = new Vector3 (mousePos.x,mousePos.y,0f) - transform.position;
        float rotz = Mathf.Atan2(rotation.y,rotation.x) * Mathf.Rad2Deg;
        gunholder.transform.rotation = Quaternion.Euler(0,0,rotz + 90);

        if (Input.GetKeyDown(KeyCode.Mouse0) && !lr2.enabled)
        {
           
            if (Physics2D.Raycast(transform.position, rotation.normalized))
            {
                for(int i = 0; i < MaxAngleToRotate ; i++)
                {
                    Vector3 curvector = Quaternion.Euler(0, 0, i) * rotation;
                    RaycastHit2D _hit = Physics2D.Raycast(transform.position, curvector.normalized ,MaxDistanceForGrapple,layerMask);
                    if(_hit)
                    {
                        if (Vector2.Distance(_hit.point, transform.position) <= MaxDistanceForGrapple && _hit.transform.gameObject.layer == grappablelayer)
                        {
                            lr.enabled = true;
                            lr.SetPosition(0,_hit.point);
                            lr.SetPosition(1,transform.position);
                            distancejoint.connectedAnchor = _hit.point;
                            distancejoint.enabled = true;
                        }
                        break;
                    }
                    curvector = Quaternion.Euler(0, 0, -i) * rotation;
                    _hit = Physics2D.Raycast(transform.position, curvector.normalized ,MaxDistanceForGrapple,layerMask);
                    if(_hit)
                    {
                        if (Vector2.Distance(_hit.point, transform.position) <= MaxDistanceForGrapple && _hit.transform.gameObject.layer == grappablelayer)
                        {
                            lr.enabled = true;
                            lr.SetPosition(0,_hit.point);
                            lr.SetPosition(1,transform.position);
                            distancejoint.connectedAnchor = _hit.point;
                            distancejoint.enabled = true;
                        }
                        break;
                    }
                }
            }
        }
        
        else if (Input.GetKeyUp(KeyCode.Mouse0) && !lr2.enabled)
        {
            distancejoint.enabled = false;
            lr.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !lr.enabled)
        {
            if (Physics2D.Raycast(transform.position, rotation.normalized))
            {
                for(int i = 0; i < MaxAngleToRotate ; i++)
                {
                    Vector3 curvector = Quaternion.Euler(0, 0, i) * rotation;
                    RaycastHit2D _hit = Physics2D.Raycast(transform.position, curvector.normalized ,MaxDistanceForGrapple,layerMask);
                    if(_hit)
                    {
                        if (Vector2.Distance(_hit.point, transform.position) <= MaxDistanceForGrapple && _hit.transform.gameObject.layer == grappablelayer)
                        {
                            _myBody.gravityScale = -10f;
                            lr2.enabled = true;
                            lr2.SetPosition(0,_hit.point);
                            lr2.SetPosition(1,transform.position);
                            distancejoint.connectedAnchor = _hit.point;
                            distancejoint.enabled = true;
                        }
                        break;
                    }
                    curvector = Quaternion.Euler(0, 0, -i) * rotation;
                    _hit = Physics2D.Raycast(transform.position, curvector.normalized ,MaxDistanceForGrapple,layerMask);
                    if(_hit)
                    {
                        if (Vector2.Distance(_hit.point, transform.position) <= MaxDistanceForGrapple && _hit.transform.gameObject.layer == grappablelayer)
                        {
                            _myBody.gravityScale = -10f;
                            lr2.enabled = true;
                            lr2.SetPosition(0,_hit.point);
                            lr2.SetPosition(1,transform.position);
                            distancejoint.connectedAnchor = _hit.point;
                            distancejoint.enabled = true;
                        }
                        break;
                    }
                }
            }
        }
        
        else if (Input.GetKeyUp(KeyCode.Mouse1) && !lr.enabled)
        {
            distancejoint.enabled = false;
            lr2.enabled = false;
            _myBody.gravityScale = 0f;
        }
        if (distancejoint.enabled)
        {
            if(lr.enabled)lr.SetPosition(1,transform.position);
            if(lr2.enabled)lr2.SetPosition(1,transform.position);
        }
        
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, MaxDistanceForGrapple);
    }


}
