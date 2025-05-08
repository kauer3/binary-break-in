using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConnectionHandler : MonoBehaviour
{
    public InputActionReference triggerInputActionReference;
    private RaycastHit hit;

    private Transform gameTrigger;
    private Transform parent;

    public BackgroundMusicManager backgroundMusicManager;
    AudioSource audioSource;
    ParticleSystem glitchParticles;

    private bool isConnected = false;

    TriggerDoor door;
    TriggerDoor backDoor;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
        audioSource = GetComponent<AudioSource>();
        glitchParticles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameTrigger != null)
        {
            gameTrigger.gameObject.GetComponent<Outline>().enabled = false;
            gameTrigger = null;
        }

        if (!isConnected && Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1, 1 << 9))
        {
            gameTrigger = hit.transform;
            //Debug.Log("Ray hit " + gameTrigger.gameObject.name);
            if (!gameTrigger.gameObject.GetComponent<GameTrigger>().activated)
            {
                if (gameTrigger.gameObject.GetComponent<Outline>() != null)
                {
                    if (!gameTrigger.gameObject.GetComponent<Outline>().enabled)
                    {
                        gameTrigger.gameObject.GetComponent<Outline>().enabled = true;

                    }
                }
                else
                {
                    Outline outline = gameTrigger.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                }

                if (triggerInputActionReference.action.ReadValue<float>() == 1)
                {
                    isConnected = true;
                    Debug.Log(isConnected);
                    HandleGameStart(gameTrigger);
                }
            }
            else
            {
                gameTrigger = null;
            }
        }

    }

    void HandleGameStart(Transform selectedDoor)
    {
        Debug.Log("HandleGameStart on: " + selectedDoor.gameObject.name);
        foreach (Transform child in selectedDoor)
        {
            if (child != null && child.CompareTag("ThumbDrivePosition"))
            {
                transform.SetParent(child, true);
            }
        }

        door = selectedDoor.GetComponentInParent<TriggerDoor>();

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1, 1 << 9))
        {
            if (hit.transform.gameObject.CompareTag("Door"))
            {
                hit.transform.gameObject.GetComponent<GameTrigger>().activated = true;
                backDoor = hit.transform.gameObject.GetComponentInParent<TriggerDoor>();
            }
        }

        GameManager GameManager = gameTrigger.GetComponent<GameTrigger>().StartGame();
        if (GameManager != null)
        {
            Debug.Log("Subscribing HandleGameEnd to gameEnd event");
            GameManager.OnGameEnd += HandleGameEnd;
            audioSource.Play();
        }

        audioSource.Play();
        glitchParticles.Play();
        backgroundMusicManager.StopBackgroundMusic();
    }

    void HandleGameEnd()
    {
        Debug.Log("Running HandleGameEnd");
        Debug.Log("Door: " + door);
        Debug.Log("Back Door: " + backDoor);

        if (door != null)
        {
            door.locked = false;
            door = null;
        }
        if (backDoor != null)
        {
            backDoor.locked = false;
            backDoor = null;
        }

        transform.SetParent(parent, true);
        transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        transform.localScale = new Vector3(1, 1, 1);
        // audioSource = GetComponent<AudioSource>();
        // glitchParticles = GetComponent<ParticleSystem>();
        glitchParticles.Stop();
        backgroundMusicManager.onHold = false;
        isConnected = false;
    }
}
