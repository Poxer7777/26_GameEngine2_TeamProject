using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBrick : MonoBehaviour
{
    public GameObject tetrisPrefab;

    void Start()
    {
        
    }

    public void SpawnTetrisBrick()
    {
        if (tetrisPrefab == null) return;

        // 스폰 위치: 보드 상단 중앙 (가로 6칸 중 3번째 칸 근처, 세로 10칸 맨 위)
        // 정수 단위 좌표계 싱크를 위해 대략 (3, 7, 0) 지점으로 잡습니다.
        float spawnX = Random.Range(0, 6);
        Vector3 spawnPosition = new Vector3(spawnX, 7f, 0f);

        // 블록 생성
        GameObject nextBlock = Instantiate(tetrisPrefab, spawnPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
