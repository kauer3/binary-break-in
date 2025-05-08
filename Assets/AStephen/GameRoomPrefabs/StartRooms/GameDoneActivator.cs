using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDoneActivator : MonoBehaviour
{
    public GameObject targetCollider; 
    public float activationDelay = 60f;
    public GameObject canvasPrefab; 
    private GameObject instantiatedCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger area: " + other.gameObject);

            if (other.TryGetComponent<BriefcaseHolder>(out BriefcaseHolder player))
            {
                if (instantiatedCanvas == null && player.hasBriefcase)
                {
                    Debug.Log("Player has briefcase");
                    instantiatedCanvas = Instantiate(canvasPrefab);
                }
                else
                {
                    Debug.Log("Player does not have briefcase");
                }
            }
        }
    }

    private void Start()
    {
        //targetCollider.SetActive(false); 
        //Invoke("ActivateCollider", activationDelay); 
    }

    //void ActivateCollider()
    //{
        //targetCollider.SetActive(true); 
        //Debug.Log("Collider activated after 1 minute.");
    //}
}
