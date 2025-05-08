using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTrigger : MonoBehaviour
{
    public GameObject Game;
    public Transform gamePosition;
    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        gamePosition = GameObject.Find("Minigames Position").transform;
    }

    public GameManager StartGame()
    {
        activated = true;
        GameObject GameInstance = Instantiate(Game, gamePosition);

        if (GameInstance.TryGetComponent<GameManager>(out GameManager gameManager))
        {
            return gameManager;
        }
        else
        {
            return null;
        }
    }
}
