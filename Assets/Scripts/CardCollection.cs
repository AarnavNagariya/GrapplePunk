using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    [SerializeField] private LayerMask collectibleLayer;
    [SerializeField] private TextMeshProUGUI cardCounter;
    public GameObject win;
    public int cards = 0;
    private int totalCards = 0;
    private Dictionary<int, bool> collectedCards = new Dictionary<int, bool>();

    private void Start()
    {
        var goArray = FindObjectsOfType<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (collectibleLayer == (collectibleLayer | (1 << goArray[i].layer)))
                totalCards++;
        }

        cardCounter.SetText($"{cards}/{totalCards} Cards");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (collectibleLayer == (collectibleLayer | (1 << col.gameObject.layer)) &&
            !collectedCards.ContainsKey(col.gameObject.GetInstanceID()))
        {
            cards++;
            collectedCards[col.gameObject.GetInstanceID()] = true;
            cardCounter.SetText($"{cards}/{totalCards} Cards");
            Destroy(col.gameObject);
        }
    }
}