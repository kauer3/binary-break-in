using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefcase : MonoBehaviour
{
    public BriefcaseHolder player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Detect player with gameObject: " + other.gameObject);
            if (other.TryGetComponent<BriefcaseHolder>(out BriefcaseHolder player))
            {
                player.hasBriefcase = true;
            }
            Destroy(gameObject);
        }
    }
}
