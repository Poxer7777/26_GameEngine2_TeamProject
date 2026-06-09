using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombLine : MonoBehaviour
{
    public GameObject bomb;
    Bomb b;
    // Start is called before the first frame update
    public enum wireType
    {
        Red,
        Yellow,
        Green,
        Blue
    }

    void Start()
    {
        b = GetComponent<Bomb>();
    }

    public void OnClickRedButton()
    {
        b.DefuseBomb(wireType.Red);
    }

    public void OnClickYellowButton()
    {
        b.DefuseBomb(wireType.Yellow);
    }

    public void OnClickGreenButton()
    {
        b.DefuseBomb(wireType.Green);
    }

    public void OnClickBlueButton()
    {
        b.DefuseBomb(wireType.Blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
