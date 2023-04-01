using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class FadeText : MonoBehaviour
{
    [SerializeField] private float cooldown = 2f;

    void Start()
    {
    }

    void Update()
    {
        StartCoroutine(wait());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(cooldown);
        Destroy(gameObject);
    }
}