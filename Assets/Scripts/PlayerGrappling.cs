using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerGrappling : MonoBehaviour
{
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference aimAction;
    [SerializeField] private InputActionReference grappleAimAction;
    [SerializeField] private InputActionReference moveHandAction;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform armTransform;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float armMoveSpeed;
    [SerializeField] private Transform grappleLight;
    [SerializeField] private Vector2 grappleRange;
    [SerializeField] private float initialGrappleAngle;
    [SerializeField] private float sensitivity;
    private PlayerAnimation playerAnimationScript;
    private DistanceJoint2D distanceJoint2D;
    private Vector2 anchorPoint;
    private Light2D grappleLight2D;


    public bool grappling;
    public bool aiming = false;

    private float rotateTargetAngle;
    private float armAngle;


    private void OnEnable()
    {
        shootAction.action.Enable();
        aimAction.action.Enable();
        grappleAimAction.action.Enable();
        moveHandAction.action.Enable();


        shootAction.action.performed += (_) => ShootGrapple();
        shootAction.action.canceled += (_) => CancelGrapple();

        aimAction.action.performed += (_) => AimHandArrow();
        aimAction.action.canceled += (_) => RestHand();

        grappleAimAction.action.performed += (_) => AimHand();
        grappleAimAction.action.canceled += (_) => RestHand();
    }

    void AimHandArrow()
    {
        Vector2 dir = aimAction.action.ReadValue<Vector2>();
        rotateTargetAngle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        rotateTargetAngle = Mathf.Min(rotateTargetAngle, grappleRange.y);
        rotateTargetAngle = Mathf.Max(rotateTargetAngle, grappleRange.x);
        aiming = true;
        rotateTargetAngle += 90;
    }

    void AimHand()
    {
        aiming = true;
        rotateTargetAngle = initialGrappleAngle;
        rotateTargetAngle += 90;
    }

    void RestHand()
    {
        aiming = false;
        rotateTargetAngle = 0;
    }

    private void OnDisable()
    {
        shootAction.action.Enable();
        aimAction.action.Enable();
    }

    private void Start()
    {
        distanceJoint2D = GetComponent<DistanceJoint2D>();
        rotateTargetAngle = armTransform.eulerAngles.z;
        grappleLight2D = grappleLight.gameObject.GetComponent<Light2D>();
        playerAnimationScript = GetComponent<PlayerAnimation>();
    }

    void ShootGrapple()
    {
        if (grappling)
            return;
        Vector2 dir = shootPoint.right * (playerAnimationScript.isFacingLeft ? -1 : 1);
        RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, dir, 50f, grappleLayer);
        if (hit.collider != null)
        {
            anchorPoint = hit.point;
            distanceJoint2D.connectedAnchor = anchorPoint;

            distanceJoint2D.enabled = true;
            grappling = true;
            lineRenderer.gameObject.transform.position = anchorPoint;
            lineRenderer.SetPosition(1,
                new Vector3(shootPoint.position.x - anchorPoint.x, shootPoint.position.y - anchorPoint.y, 1));
            lineRenderer.enabled = true;
            grappleLight2D.enabled = true;
        }
    }

    private void Update()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(1,
                new Vector3(shootPoint.position.x - anchorPoint.x, shootPoint.position.y - anchorPoint.y, 1));
            grappleLight.up = -(shootPoint.position - new Vector3(anchorPoint.x, anchorPoint.y));
            grappleLight.position = shootPoint.position;
            float len = (anchorPoint - (Vector2)shootPoint.position).magnitude;
            grappleLight2D.shapePath[2] =
                new Vector3(grappleLight2D.shapePath[2].x, len, grappleLight2D.shapePath[2].z);
            grappleLight2D.shapePath[3] =
                new Vector3(grappleLight2D.shapePath[3].x, len, grappleLight2D.shapePath[3].z);
        }

        if (aiming && !grappling)
        {
            float moveHandDelta = moveHandAction.action.ReadValue<float>();
            rotateTargetAngle += moveHandDelta * sensitivity * Time.deltaTime;
            rotateTargetAngle = Mathf.Min(rotateTargetAngle, grappleRange.y + 90);
            rotateTargetAngle = Mathf.Max(rotateTargetAngle, grappleRange.x + 90);
        }

        armAngle += (rotateTargetAngle - armAngle) * armMoveSpeed * Time.deltaTime;
        armTransform.localEulerAngles =
            new Vector3(armTransform.localEulerAngles.x, armTransform.localEulerAngles.y, armAngle);
    }

    void CancelGrapple()
    {
        distanceJoint2D.enabled = false;
        grappling = false;
        lineRenderer.enabled = false;
        grappleLight2D.enabled = false;
    }
}