using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    public GameObject optionScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void GameOption()
    {
        optionScreen.SetActive(true);
    }

    public void CloseOption()
    {
        optionScreen.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
