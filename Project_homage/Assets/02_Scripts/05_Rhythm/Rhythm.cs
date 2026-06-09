using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rhythm : MonoBehaviour
{
    public static Rhythm Instance;

    public Image redBall;
    public Image blueBall;

    [Header("Audio")]
    public AudioSource bgmSource;
    public float bpm = 128f;
    private double musicStartTime;

    [Header("Map & Hierarchy")]
    public List<Tile> mapTiles = new List<Tile>();
    private int currentTileIndex = 1;

    [Header("MS Timing Judgment")]
    public float perfectThresholdMs = 50f;
    public float greatThresholdMs = 100f;

    private Transform currentPivot;
    private Transform currentOrbit;
    private float rotationSpeed;
    private bool isClockwise = true;

    // 배치 및 역산 핵심 변수
    private float startAngleOffset;
    private float orbitRadius;
    private float lastPivotAngle;
    private double lastPivotTargetTime; // 유저 입력 시간이 아닌, 타일의 '절대적 정박 시간'을 저장합니다.

    private float visualCurrentAngle; // 화면에 실제로 그려질 공의 부드러운 현재 각도
    private bool isFirstFrameAfterStart = true; // ★ 시작 시 순간 가속 방지용 플래그

    void Start()
    {
        if (Instance == null) Instance = this;

        currentPivot = redBall.transform;
        currentOrbit = blueBall.transform;

        // 1. 초기 위치 설정 (0번 타일에 피벗 고정)
        if (mapTiles.Count > 0)
            currentPivot.position = mapTiles[0].transform.position;

        // 2. 인스펙터 실제 회전 반경 계산
        orbitRadius = Vector3.Distance(currentPivot.position, currentOrbit.position);

        // ★ [위치 이동] 타일 박자 타임라인을 계산하기 "전에" 각도 데이터부터 올바르게 정렬합니다.
        if (mapTiles.Count > 1)
        {
            // 1번 타일(첫 목표)의 정박 각도를 기준으로 삼습니다.
            float firstTargetAngle = mapTiles[1].targetAngle;

            // 시계 방향으로 회전하며 왼쪽에서 오른쪽으로 안착하려면, 
            // 시작 각도는 첫 목표 각도보다 정확히 180도 '앞선(큰)' 위치여야 합니다.
            lastPivotAngle = firstTargetAngle + 180f;

            // 0번 타일의 내부 각도 데이터도 이 흐름에 맞게 "먼저" 정렬해 줍니다.
            mapTiles[0].targetAngle = lastPivotAngle;
        }
        else
        {
            // 예외 처리용 (기존 로직 유지)
            Vector3 direction = currentOrbit.position - currentPivot.position;
            startAngleOffset = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lastPivotAngle = startAngleOffset;
            if (mapTiles.Count > 0) mapTiles[0].targetAngle = startAngleOffset;
        }

        // 비주얼 시작 각도도 정박 시작 각도와 100% 일치시킵니다.
        visualCurrentAngle = lastPivotAngle;

        // 3. 속도 정립 및 올바른 데이터를 기반으로 정박 시간(targetTime) 계산
        float secondsPerBeat = 60f / bpm;
        rotationSpeed = 180f / secondsPerBeat;

        // 이제 완벽히 정렬된 타일 각도로 정박 초를 계산하므로 싱크가 소수점까지 맞아떨어집니다.
        CalculateTileTargetTimes(secondsPerBeat);

        // 4. 첫 출발 상태 및 오디오 예약 초기화
        isClockwise = true;
        musicStartTime = AudioSettings.dspTime + 1.0f;
        lastPivotTargetTime = 0f;
        isFirstFrameAfterStart = true;

        bgmSource.PlayScheduled(musicStartTime);
    }

    void CalculateTileTargetTimes(float secondsPerBeat)
    {
        double accumulatedTime = 0;
        if (mapTiles.Count > 0) mapTiles[0].targetTime = 0;

        for (int i = 1; i < mapTiles.Count; i++)
        {
            float angleInterval = Mathf.Abs(mapTiles[i].targetAngle - mapTiles[i - 1].targetAngle);
            if (angleInterval == 0) angleInterval = 180f;

            float beatRatio = angleInterval / 180f;
            accumulatedTime += (secondsPerBeat * beatRatio);

            mapTiles[i].targetTime = accumulatedTime;
        }
    }

    void Update()
    {
        if (!RhythmManager.Instance.isGame || AudioSettings.dspTime < musicStartTime || currentTileIndex >= mapTiles.Count) return;

        // 1. 키 입력 및 판정은 오직 절대적인 dspTime 기준으로만 처리 (박자 밀림 0%)
        double currentProgressTime = AudioSettings.dspTime - musicStartTime;
        if (Input.anyKeyDown)
        {
            CheckMsTimingAndSwitch(currentProgressTime);
            currentProgressTime = AudioSettings.dspTime - musicStartTime;
        }

        // 매 프레임 모니터 주사율에 맞춰 부드럽게 흐르는 Time.deltaTime을 사용하여 가상 각도를 계산합니다.
        float directionMultiplier = isClockwise ? -1f : 1f;

        // 프레임 드랍이나 미세한 렉으로 인해 'Time.deltaTime 기반 각도'와 '실제 오디오 정박 각도'가 벌어지는 것을 방지합니다.
        // 현재 오디오 정박 기준 위치를 계산합니다.
        double timeSinceLastTileTarget = currentProgressTime - lastPivotTargetTime;
        if (timeSinceLastTileTarget < 0) timeSinceLastTileTarget = 0;
        float audioTargetAngle = lastPivotAngle + ((float)timeSinceLastTileTarget * rotationSpeed * directionMultiplier);

        // 눈에 보이는 각도가 실제 오디오 박자 각도와 너무 벌어지면 오디오 각도로 부드럽게 보정(Lerp)해 줍니다.
        // 이 처리를 통해 끊김은 100% 사라지고, 음악 싱크를 강제로 따라가게 됩니다.
        if (isFirstFrameAfterStart)
        {
            visualCurrentAngle = audioTargetAngle;
            isFirstFrameAfterStart = false; // 이후 프레임부터는 정상적으로 deltaTime 회전 및 Lerp 적용
        }
        else
        {
            // 정상 프레임 플레이 시의 부드러운 deltaTime 기반 회전
            float angleDelta = rotationSpeed * Time.deltaTime * directionMultiplier;
            visualCurrentAngle += angleDelta;
        }

        visualCurrentAngle = Mathf.Lerp(visualCurrentAngle, audioTargetAngle, Time.deltaTime * 25f);

        // 최종적으로 부드러워진 각도를 대입합니다.
        SetOrbitPositionByAngle(visualCurrentAngle);
    }

    void SetOrbitPositionByAngle(float angleDegrees)
    {
        float radians = angleDegrees * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * orbitRadius;
        currentOrbit.position = currentPivot.position + offset;
    }

    void CheckMsTimingAndSwitch(double currentProgressTime)
    {
        Tile targetTile = mapTiles[currentTileIndex];

        // 판정 역시 유저가 누른 시점(currentProgressTime)과 음악 정박(targetTile.targetTime)만 순수하게 비교
        float timeDiffInSeconds = (float)(currentProgressTime - targetTile.targetTime);
        float timeDiffInMs = timeDiffInSeconds * 1000.0f;
        float absDiffMs = Mathf.Abs(timeDiffInMs);

        if (absDiffMs > 100f)
        {
            if (timeDiffInMs < 0)
                Debug.Log($"<color=red>[Game Over]</color> 너무 빠름! 오차: {timeDiffInMs:F1} ms");
            else
                Debug.Log($"<color=red>[Game Over]</color> 너무 느림! 오차: {timeDiffInMs:F1} ms");

            RhythmManager.Instance.gameOver = true;
            bgmSource.Stop();
            return;
        }

        if (absDiffMs <= 50f)
            Debug.Log($"<color=cyan>정확 (Perfect!)</color> 오차: {timeDiffInMs:F1} ms");
        else if (timeDiffInMs < 0)
            Debug.Log($"<color=yellow>빠름 (Early)</color> 오차: {timeDiffInMs:F1} ms");
        else
            Debug.Log($"<color=orange>느림 (Late)</color> 오차: {timeDiffInMs:F1} ms");

        ProceedToNextTile();
    }

    void ProceedToNextTile()
    {
        currentTileIndex++;

        if (currentTileIndex >= mapTiles.Count)
        {
            return;
        }

        // 1. 축 교체 및 스위칭
        Transform previousPivot = currentPivot;
        currentPivot = currentOrbit;
        currentOrbit = previousPivot;

        currentPivot.position = mapTiles[currentTileIndex - 1].transform.position;

        // 2. 새로운 정박 기준 각도 설정
        lastPivotAngle = mapTiles[currentTileIndex - 1].targetAngle + 180f;

        // ★ 축이 바뀌는 순간 비주얼 각도도 정박 각도로 정확히 일치시켜 오차를 초기화합니다.
        visualCurrentAngle = lastPivotAngle;

        // 3. 방향 자동 판정
        if (currentTileIndex < mapTiles.Count)
        {
            float angleDiff = mapTiles[currentTileIndex].targetAngle - mapTiles[currentTileIndex - 1].targetAngle;
            isClockwise = (angleDiff <= 0f);
        }
        else
        {
            isClockwise = true;
        }

        // 음악적 정박 타겟 시간 대입
        lastPivotTargetTime = mapTiles[currentTileIndex - 1].targetTime;
    }
}