using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalCodeManager : MonoBehaviour
{
    public TMP_Text codeText;
    public string correctPasscode = "";
    public GameObject correctMessage;
    private string enteredCode = "";
    FinalPassword passwordHolder;
    public TMP_Text passwordText;

    public GameObject breifCasePrefab;
    Transform computerLocation;

    void Start()
    {
        passwordHolder = FindAnyObjectByType<FinalPassword>();
        computerLocation = GameObject.FindWithTag("EndComputer").transform;
        correctPasscode = passwordHolder.GetCode();
        passwordText.text = correctPasscode;
        correctMessage.SetActive(false);
        ResetCode();
    }

    public void AddDigit(string digit)
    {
        if (enteredCode.Length < correctPasscode.Length)
        {
            enteredCode += digit;
            UpdateCodeText();
        }

        if (enteredCode.Length == correctPasscode.Length)
        {
            CheckCode();
        }
    }

    private void CheckCode()
    {
        Debug.Log("Checking code");
        if (enteredCode == correctPasscode)
        {
            correctMessage.SetActive(true);
            SpawnLocation();
            EndGame();
        }
        else
        {
            ResetCode();
        }
    }

    private void UpdateCodeText()
    {
        codeText.text = enteredCode.PadRight(correctPasscode.Length, '*');
    }

    private void ResetCode()
    {
        enteredCode = "";
        UpdateCodeText();
    }

    public void Clear()
    {
        ResetCode();
        correctMessage.SetActive(false);
    }

    public void SpawnLocation()
    {
        Instantiate(breifCasePrefab, computerLocation.transform.position + Vector3.forward * 2, Quaternion.identity);
    }

    public void EndGame()
    {
        //OnGameEnd?.Invoke();
        Destroy(gameObject);
    }
}
