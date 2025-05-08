using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    private Animator _doorAnimator;
    public bool locked = true;

    public ConnectionHandler thumbdrive;

    // Start is called before the first frame update
    void Start()
    {
        _doorAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);
        if (other.CompareTag("Enemy") || (!locked && other.CompareTag("Player")))
        {
            Debug.Log("Enemy entering");
            //_doorAnimator.SetTrigger("Open");
            _doorAnimator.SetBool("Open", true);
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Enemy") || other.CompareTag("Player")))
        {
            //_doorAnimator.SetTrigger("Close");
            _doorAnimator.SetBool("Open", false);
            Debug.Log("Enemy leaving");
        }
    }
}
