using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPassword : MonoBehaviour
{
    public List<string> codes = new List<string>();
    public LevelGenerator levelGenerator;
    int computers;

    private void Start()
    {
        computers = levelGenerator.computers;
    }

    public string GetCode()
    {
        string finalCode = "";
        codes.ForEach((code) =>
        {
            finalCode += code;
        });

        while (codes.Count < computers)
        {
            codes.Add("***");
        }

        return finalCode;
    }
}
