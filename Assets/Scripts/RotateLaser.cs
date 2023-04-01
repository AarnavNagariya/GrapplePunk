using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLaser : MonoBehaviour
{
    [SerializeField] private float rotationvalue = 1f;


    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0, 0, rotationvalue * Time.deltaTime);
    }
}
