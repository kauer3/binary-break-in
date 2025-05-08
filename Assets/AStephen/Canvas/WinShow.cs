using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinShow : MonoBehaviour
{
    public GameObject winScreenCanvas; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            winScreenCanvas.SetActive(true);
            Debug.Log("Canvas activated on player collision.");
        }
    }
}
