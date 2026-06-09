using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardData : MonoBehaviour
{
    public static BoardData instance;

    [Header("Map Initializer")]
    public GameObject dummyCubePrefab;

    [Header("Grid Settings")]
    public static int width = 6;
    public static int height = 10;

    private static Transform[,] grid = new Transform[width, height];

    void Awake()
    {
        instance = this;
    }

    public static void InitializeRandomLines()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<BoardData>();
        }

        if (instance == null || instance.dummyCubePrefab == null)
        {
            Debug.LogError("BoardData 인스턴스 또는 dummyCubePrefab이 설정되지 않았습니다.");
            return;
        }

        int lastEmptyX = -1; // 바로 아래 줄의 빈 칸 위치를 기억할 변수 (-1은 없음)

        // 바닥 4개의 줄 (y = 0, 1, 2, 3) 생성
        for (int y = 0; y < 4; y++)
        {
            int currentEmptyX = -1;

            // 바로 아래 줄과 겹치지 않는 빈 칸 위치(X) 무작위 선정
            while (true)
            {
                currentEmptyX = Random.Range(0, width);
                if (currentEmptyX != lastEmptyX)
                {
                    break; // 아래 줄 구멍 위치와 다르면 통과
                }
            }

            // 구멍 위치를 기억해 둠 (다음 루프에서 '아래 줄'이 됨)
            lastEmptyX = currentEmptyX;

            // 선정된 구멍(currentEmptyX)을 제외하고 나머지 칸에 블록 배치
            for (int x = 0; x < width; x++)
            {
                if (x == currentEmptyX) continue; // 이 칸은 비워둠

                // 3D 큐브 생성 및 위치 설정
                Vector3 spawnPos = new Vector3(x, y, 0);
                GameObject cube = Instantiate(instance.dummyCubePrefab, spawnPos, Quaternion.identity);

                // grid에 데이터 등록
                grid[x, y] = cube.transform;
            }
        }
    }
    public static bool IsInsidePosition(Vector2 pos)
    {
        int roundedX = Mathf.RoundToInt(pos.x);
        int roundedY = Mathf.RoundToInt(pos.y);

        // 해당 위치가 보드의 범위 안에 있는지 검사
        if (roundedX < 0 || roundedX >= width || roundedY < 0)
        {
            return false;
        }

        // 해당 자리에 이미 고정된 블록 조각(Transform)이 존재하는지 검사
        if (roundedY < height)
        {
            if (grid[roundedX, roundedY] != null)
                return false;
        }
        return true;
    }

    public static void AddToGrid(Transform brickTransform)
    {
        foreach (Transform child in brickTransform)
        {
            int roundedX = Mathf.RoundToInt(child.position.x);
            int roundedY = Mathf.RoundToInt(child.position.y);

            // 정상적인 플레이 범위 내라면 배열에 큐브의 Transform 저장
            if (roundedY < height && roundedX >= 0 && roundedX < width)
            {
                grid[roundedX, roundedY] = child;
            }
        }
    }

    // 특정 행(Y)이 블록으로 가득 찼는지 검사
    public static bool IsLineFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
                return false; // 한 칸이라도 비어있으면 꽉 찬 게 아님
        }
        return true;
    }

    // 가득 찬 특정 행(Y)을 지우고 오브젝트 파괴
    public static void DeleteLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject); // 3D 큐브 파괴
                grid[x, y] = null; // 데이터 비우기
            }
        }
    }

    // 지정한 행(Y) 위의 모든 블록들을 한 칸 아래로 내림
    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                // 데이터를 한 칸 아래행으로 복사
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // 실제 3D 오브젝트 위치도 Y축으로 -1 이동
                grid[x, y - 1].position += Vector3.down;
            }
        }
    }

    // 삭제된 행 위쪽에 있는 모든 행들을 한 칸씩 끌어내리는 함수
    public static void DecreaseRowsAbove(int startY)
    {
        for (int i = startY; i < height; i++)
        {
            DecreaseRow(i);
        }
    }

    // 전체 보드를 아래에서부터 검사하여 가득 찬 줄을 모두 지우고 정렬
    public static void CheckLines()
    {
        for (int y = 0; y < height; y++)
        {
            if (IsLineFull(y))
            {
                DeleteLine(y);
                TetrisManager.instance.clearedLines++;
                DecreaseRowsAbove(y + 1);
                y--; // 줄이 내려왔으므로 현재 행을 다시 한 번 검사
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
