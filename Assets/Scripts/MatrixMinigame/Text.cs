using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Text : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshPro textGameObject;
    [SerializeField] TMPro.TextMeshPro keyGameObject;
    Material mainTextMaterial;
    Material keyMaterial;

    //[SerializeField] GameObject generatorObject;
    GameManager generator;
    [SerializeField] ParticleSystem particles;
    [SerializeField] float fadeOutTime = 1.5f;
    string keyCode = "";
    public char? key = null;
    public int columnIndex;
    Color glow = new Color(0, 4.86714363f, 0, 0.0196078438f);
    Color face = new Color(0,4.86714411f,0,1);
    Color underlay = new Color(0, 0.757f, 0, 0.157f);

    void Awake()
    {
        generator = GetComponentInParent<GameManager>();
        mainTextMaterial = textGameObject.fontMaterial;
        keyMaterial = keyGameObject.fontMaterial;
    }

    void Update()
    {
        transform.position += Vector3.down * 1.5f * Time.deltaTime;

        if (keyCode != "" && Input.GetKeyDown(keyCode))
        {
            particles.Play();
            AnimateToHide();
            generator.UpdateScore();
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        float start = 1;
        float end = -1;
        float value;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime / fadeOutTime;
            value = Mathf.Lerp(start, end, t);
            mainTextMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, value);
            if (value >= 0)
            {
                mainTextMaterial.SetFloat(ShaderUtilities.ID_GlowPower, value);
                keyMaterial.SetFloat(ShaderUtilities.ID_GlowPower, value);
            }
            /*
            else
            {
                keyMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, value);
                keyMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, value);
                if (value < -0.25)
                {
                    mainTextMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, value);
                }
            }
            */

            yield return null;
        }
    }

    IEnumerator ShrinkText()
    {
        float t = 0;
        float value;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime / fadeOutTime;
            value = Mathf.Lerp(0, -1, t);

            mainTextMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, value);
            keyMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, value);
            keyMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, value);
            yield return null;
        }
        DeactivateObject();
    }

    IEnumerator WaitToGetDistance(int size)
    {
        yield return new WaitForSeconds(size*2);
        generator.FreeColumn(columnIndex);
    }

    public void SetKey(char keyChar, int column, string codeString, int size)
    {
        ResetMaterials();
        textGameObject.text = codeString;
        key = keyChar;
        keyCode = keyChar.ToString();
        keyGameObject.text = keyCode;
        keyCode = keyCode.ToLower();

        columnIndex = column;

        StartCoroutine(WaitToGetDistance(size));
    }

    void ResetMaterials()
    {
        mainTextMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 1);
        mainTextMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 1);
        mainTextMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, -0.25f);
        keyMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 1);
        keyMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0);
        keyMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0);
        mainTextMaterial.SetColor(ShaderUtilities.ID_FaceColor, face);
        mainTextMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, underlay);
        mainTextMaterial.SetColor(ShaderUtilities.ID_GlowColor, glow);
    }

    void AnimateToHide()
    {
        StartCoroutine(FadeOut());
        StartCoroutine(ShrinkText());
        keyCode = "";
    }

    void DeactivateObject()
    {
        StopAllCoroutines();
        generator.DeactivateTextObject(this);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D()
    {
        if (keyCode != "")
        {
            AnimateToHide();
            mainTextMaterial.SetColor(ShaderUtilities.ID_FaceColor, Color.red);
            mainTextMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.red);
            mainTextMaterial.SetColor(ShaderUtilities.ID_GlowColor, Color.red);
        }
    }
}
