using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    public static FruitManager instance;

    // 인스펙터에서 0단계부터 마지막 단계까지 순서대로 정렬해야 합니다!
    public GameObject[] fruits;

    [Header("Game Settings")]
    public float gameTime = 10f;
    private float currentTime;
    public int mergedFruits = 0;
    public bool isGame;

    [Header("UI Reference")]
    public TextMeshProUGUI timerText;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        currentTime = gameTime;
        isGame = true;
    }

    public void MergeFruits(Fruit fruitA, Fruit fruitB)
    {
        // 중복 실행 방지
        if (fruitA.isMerged || fruitB.isMerged) return;

        fruitA.isMerged = true;
        fruitB.isMerged = true;

        // 두 과일의 중간 위치
        Vector3 spawnPos = (fruitA.transform.position + fruitB.transform.position) * 0.5f;

        // 현재 과일의 레벨에서 정확히 +1 단계를 계산
        int nextLevel = fruitA.level + 1;

        // 배열 범위를 넘지 않을 때만(마지막 단계가 아닐 때만) 다음 과일 생성
        if (nextLevel < fruits.Length)
        {
            // ★ 정확히 계산된 차기 레벨의 프리팹을 소환합니다.
            Instantiate(fruits[nextLevel], spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.Log("최종 과일 단계입니다. 더 이상 합성할 수 없습니다!");
        }

        // 기존 과일 제거
        Destroy(fruitA.gameObject);
        Destroy(fruitB.gameObject);
        
        mergedFruits++;
    }

    void Update()
    {
        if (isGame)
        {
            // 2개의 과일을 합쳤을 때 [Clear]
            if (mergedFruits >= 2)
            {
                isGame = false;
                Debug.Log("Clear!!");

                Invoke(nameof(Clear), 2f);
            }

            // 제한 시간이 0이 되었을 때 [Fail]
            if (currentTime < 0)
            {
                isGame = false;
                Debug.Log("Time Over!!");

                Invoke(nameof(Fail), 2f);
            }
        }

        // 타이머 업데이트
        if (isGame)
        {
            currentTime -= Time.deltaTime;
        }

        timerText.text = $"Time: {Mathf.Max(0, currentTime):F1}";
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