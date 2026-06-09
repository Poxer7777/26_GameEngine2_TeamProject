using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Tooltip("이 타일에서 요구하는 목표 각도 (회전 로직용)")]
    public float targetAngle = 180f;

    [HideInInspector]
    public double targetTime;
    // 음악 시작 후 이 타일을 밟아야 하는 절대 시간 (초 단위)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
