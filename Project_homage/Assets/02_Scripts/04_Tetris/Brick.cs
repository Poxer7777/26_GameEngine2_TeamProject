using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public bool isMoving = true;

    [Header("Timing")]
    public float fallTime = 0.75f;    // 블록이 한 칸 떨어지는 주기 (초)
    private float lastFallTime;     // 마지막으로 떨어진 시간 기록

    // Start is called before the first frame update
    void Start()
    {
        lastFallTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TetrisManager.instance.isGame || !isMoving) return;

        // 1. 좌측 이동
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += Vector3.left;
            if (!CheckIsValidPosition()) // 자식들 중 하나라도 벽에 걸리면
                transform.position -= Vector3.left; // 이동 취소
        }
        // 2. 우측 이동
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += Vector3.right;
            if (!CheckIsValidPosition())
                transform.position -= Vector3.right;
        }
        // 3. 소프트 드롭 (DownArrow 눌러서 빨리 내리기)
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveDown();
        }

        // 자동으로 일정 시간마다 한 칸씩 아래로 이동
        if (Time.time - lastFallTime >= fallTime)
        {
            MoveDown();
            lastFallTime = Time.time;
        }
    }

    // 블록을 한 칸 아래로 내리고, 착지 여부를 판단하는 함수
    void MoveDown()
    {
        transform.position += Vector3.down;

        // 아래로 내렸는데 유효하지 않은 위치라면? (바닥이나 다른 블록에 닿음)
        if (!CheckIsValidPosition())
        {
            transform.position -= Vector3.down; // 원래 정수 위치로 복구
            isMoving = false; // 이 블록은 이제 조작 끝

            // 데이터 등록 및 라인 클리어
            BoardData.AddToGrid(transform);       // 가상 지도 배열에 나를 박아넣음
            BoardData.CheckLines();              // 꽉 찬 줄이 있으면 터트리고 당기기

            // 다음 블록 생성
            FindObjectOfType<SpawnBrick>().SpawnTetrisBrick();

            // 현재 스크립트 컴포넌트를 비활성화하여 더 이상 Input을 받지 않게 합니다.
            enabled = false;
        }
    }

    // 부모의 중심점이 아닌, 자식 큐브 3개의 개별 위치를 전부 검사합니다.
    bool CheckIsValidPosition()
    {
        foreach (Transform child in transform)
        {
            // 각 자식 큐브의 실제 게임 세상(World) 좌표를 가져옵니다.
            Vector2 v = child.position;

            // 2단계에서 만든 경계선/중복 검사 함수에 대입
            if (!BoardData.IsInsidePosition(v))
            {
                return false; // 단 하나라도 맵 밖으로 나가거나 겹치면 무효!
            }
        }
        return true; // 4개 큐브 모두 안전함
    }
}
