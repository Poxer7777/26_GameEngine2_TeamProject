using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SansManager : MonoBehaviour
{
    public static SansManager Instance;

    [Header("Game Setting")]
    public float currentTime;
    public bool isGame;
    public bool gameOver;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 1.1f;
        isGame = true;
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 게임이 진행중일때 시간이 흐름 (플레이어가 파괴되었을 때 종료)
        if (isGame)
        {
            currentTime -= Time.deltaTime * GameManager.instance.gameSpeed;

            // 시간이 모두 흘렀을 때 [Clear]
            if (currentTime < 0)
            {
                isGame = false;
                currentTime = 0;
                Debug.Log("Game Clear!");

                Invoke(nameof(Clear), 2f);
            }

            // 플레이어가 파괴되었을 때 [Fail]
            if (gameOver)
            {
                isGame = false;
                Debug.Log("Game Fail!");

                Invoke(nameof(Fail), 2f);
            }
        }
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
