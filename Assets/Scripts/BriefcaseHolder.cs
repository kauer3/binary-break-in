using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefcaseHolder : MonoBehaviour
{
    public bool hasBriefcase;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FirstRoom"))
        {
            // Finish game
            Debug.Log("Game finished!");
        }
    }
}
