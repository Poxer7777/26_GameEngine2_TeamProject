using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;

    [Header("Game Setting")]
    public bool isGame;
    public bool gameOver;
    public bool gameClear;
    public float waitTime = 1f;

    public AudioSource RhythmMusic;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(StartGame), waitTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGame)
        {
            if (gameClear)
            {
                isGame = false;

                Invoke(nameof(Clear), 2f);
            }

            // 비동의 버튼을 눌렀을 때 [Fail]
            if (gameOver)
            {
                isGame = false;

                Invoke(nameof(Fail), 2f);
            }
        }
    }

    void StartGame()
    {
        isGame = true;
    }

    void Clear()
    {
        GameManager.instance.RoundStandby();
    }

    void Fail()
    {
        GameManager.instance.failedGame();
    }
}
