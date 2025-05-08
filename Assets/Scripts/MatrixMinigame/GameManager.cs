using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<Text> textObjects = new List<Text>();
    [SerializeField] List<Column> columns = new List<Column>();
    List<char> activeKeys = new List<char>();
    List<char> characters = new List<char> { '日', 'ﾊ', 'ﾐ', 'ﾋ', 'ｰ', 'ｳ', 'ｼ', 'ﾅ', 'ﾓ', 'ﾆ', 'ｻ', 'ﾜ', 'ﾂ', 'ｵ', 'ﾘ', 'ｱ', 'ﾎ', 'ﾃ', 'ﾏ', 'ｹ', 'ﾒ', 'ｴ', 'ｶ', 'ｷ', 'ﾑ', 'ﾕ', 'ﾗ', 'ｾ', 'ﾈ', 'ｽ', 'ﾀ', 'ﾇ', 'ﾍ', 'ｦ', 'ｲ', 'ｸ', 'ｺ', 'ｿ', 'ﾁ', 'ﾄ', 'ﾉ', 'ﾌ', 'ﾔ', 'ﾖ', 'ﾙ', 'ﾚ', 'ﾝ', '0', '1', '2', '3', '4', '5', '8', '9', 'Z', '=', '+', '<', '>', 'ｸ' };
    List<string> stylesTags = new List<string> { "<style=Grad1>", "<style=Grad2>", "<style=Grad3>", "<style=Grad4>", "</gradient>" };

    int targetScore = 5;
    int currentScore = 0;

    public FinalPassword passwordHolder;

    public event Action OnGameEnd;

    class Column
    {
        public int x;
        public char? busyWithKey;

        public Column(int xPos, char? key) {
            x = xPos;
            busyWithKey = key;
        }
    }

    private void Start()
    {
        passwordHolder = FindAnyObjectByType<FinalPassword>();

        for (int i = -8; i < 9; i++)
        {
            columns.Add(new Column(i, null));
        }

        StartCoroutine(ManageGenerations());
    }

    private IEnumerator ManageGenerations()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        while (true)
        {
            if (textObjects.Count == 0)
            {
                Debug.Log("No more text objects available");
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            char key = (char)Random.Range(65, 91);
            if (activeKeys.Contains(key))
            {
                continue;
            }

            activeKeys.Add(key);

            int index;
            do
            {
                index = Random.Range(0, 17);
            } while (columns.ElementAt(index).busyWithKey != null);

            Column assignedColumn = columns.ElementAt(index);

            assignedColumn.busyWithKey = key;

            int size = Random.Range(4, 9);

            for (int i = 0; i < size; i++)
            {
                if (i < 5)
                {
                    sb.Append(stylesTags[i]);
                }
                sb.Append(characters[Random.Range(0, characters.Count)]);
            }

            textObjects[0].transform.position = new Vector3(assignedColumn.x, gameObject.transform.position.y + 5.9f, 0);
            textObjects[0].gameObject.SetActive(true);
            textObjects[0].SetKey(key, index, sb.ToString(), size);
            textObjects.RemoveAt(0);
            sb.Clear();

            yield return new WaitForSeconds(Random.Range(0.2f, 1f));
        }
    }

    public void FreeColumn(int columnIndex)
    {
        columns.ElementAt(columnIndex).busyWithKey = null;
    }

    public void DeactivateTextObject(Text textObject)
    {
        if (textObject.key != null)
        {
            activeKeys.Remove(textObject.key.Value);
            if (columns.ElementAt(textObject.columnIndex).busyWithKey == textObject.key)
            {
                columns.ElementAt(textObject.columnIndex).busyWithKey = null;
            }
            textObject.key = null;
        }
        textObjects.Add(textObject);
    }

    public void UpdateScore()
    {
        currentScore++;

        if (currentScore >= targetScore)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        string code = Random.Range(10, 100).ToString();
        Debug.Log("Password is :" + code);
        passwordHolder.codes.Add(code);

        OnGameEnd?.Invoke();
        Destroy(gameObject);
    }
}
