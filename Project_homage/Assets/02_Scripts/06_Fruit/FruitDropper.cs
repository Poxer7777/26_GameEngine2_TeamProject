using UnityEngine;

public class FruitDropper : MonoBehaviour
{
    [Header("Fruit Settings")]
    // 생성할 과일 프리팹들 (0단계부터 순서대로 인펙터에서 넣어줍니다)
    public GameObject[] fruitPrefabs;

    // 과일이 생성될 고정된 Y축 높이 (예: 화면 상단 높이 6.0)
    public float spawnYPosition = 6f;

    // 벽을 벗어나지 않게 제안할 X축 가로 범위 (예: -3.0 ~ 3.0)
    public float minX = -3f;
    public float maxX = 3f;

    private GameObject currentFruit; // 현재 대기 중인 과일

    void Start()
    {
        // 게임 시작 시 첫 번째 과일 대기
        PrepareNextFruit();
    }

    void Update()
    {
        // 1. 마우스의 현재 X 위치를 계산하여 대기 중인 과일을 움직입니다.
        MoveCurrentFruitWithMouse();

        // 2. 마우스 왼쪽 버튼 클릭 시 과일을 떨어뜨립니다.
        if (Input.GetMouseButtonDown(0))
        {
            DropFruit();
        }
    }

    /// <summary>
    /// 마우스 위치를 추적하여 과일을 가로로 이동시키는 함수
    /// </summary>
    void MoveCurrentFruitWithMouse()
    {
        if (currentFruit == null) return;

        // 마우스의 화면 좌표(Pixel)를 게임 월드 좌표(Unit)로 변환
        Vector3 mouseScreenPos = Input.mousePosition;

        // 메인 카메라를 통해 변환 (카메라와의 거리 Z값을 임의로 설정)
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));

        // X 좌표는 마우스를 따르되, 맵 밖으로 벗어나지 않게 제한(Clamp)합니다.
        float clampedX = Mathf.Clamp(mouseWorldPos.x, minX, maxX);

        // 과일의 위치를 상단 고정 높이(spawnYPosition)에 둡니다.
        currentFruit.transform.position = new Vector3(clampedX, spawnYPosition, 0f);
    }

    /// <summary>
    /// 다음 던질 과일을 상단에 생성(대기)하는 함수
    /// </summary>
    void PrepareNextFruit()
    {
        // 수박게임 규칙: 초반 낮은 단계(0~2단계)의 과일 중 하나를 랜덤 선택
        int randomIndex = Random.Range(0, Mathf.Min(3, fruitPrefabs.Length));

        // 고정된 상단 위치에 생성
        Vector3 spawnPos = new Vector3(0f, spawnYPosition, 0f);
        currentFruit = Instantiate(fruitPrefabs[randomIndex], spawnPos, Quaternion.identity);

        // 중요: 대기 중일 때는 중력 때문에 떨어지면 안 되므로 Rigidbody를 잠시 끕니다.
        Rigidbody rb = currentFruit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    /// <summary>
    /// 마우스를 클릭했을 때 물리 엔진을 켜서 과일을 떨어뜨리는 함수
    /// </summary>
    void DropFruit()
    {
        if (currentFruit == null) return;

        // ★ [핵심 보완] 마우스 대기 위치보다 살짝 아래에서 낙하를 시작하도록 Y축 좌표를 조절합니다.
        // spawnYPosition이 11이라면, 약 2 단위 아래인 9 지점으로 순간이동 시킵니다.
        Vector3 dropPosition = currentFruit.transform.position;
        dropPosition.y = spawnYPosition - 2f;
        currentFruit.transform.position = dropPosition;

        // 잠가두었던 중력(isKinematic)을 해제하여 아래로 떨어지게 만듭니다.
        Rigidbody rb = currentFruit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // 손을 떠났으므로 참조를 비우고, 다음 과일을 준비합니다.
        currentFruit = null;

        // 다음 과일이 스폰될 때 바로 윗 공간(spawnYPosition)에 새로 생기므로 
        // 방금 떨어뜨린 과일과 절대 겹치지 않습니다!
        Invoke("PrepareNextFruit", 0.5f);
    }
}