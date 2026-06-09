using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static BombLine;

public class Bomb : MonoBehaviour
{
    GameObject commandText;

    int countWire = 1;
    command trig;
    public enum command
    {
        CutRed,
        DontCutRed
    }

    // Start is called before the first frame update
    void Start()
    {
        commandText = GameObject.Find("Command Text");
        trig = SetDefuseTrigger();
    }

    command SetDefuseTrigger()
    {
        Array commands = Enum.GetValues(typeof(command));
        command randomCommand = (command)commands.GetValue(UnityEngine.Random.Range(0, commands.Length));

        switch (randomCommand)
        {
            case command.CutRed:
                commandText.GetComponent<TextMeshProUGUI>().text = "Cut the RED wire!";
                break;
            case command.DontCutRed:
                commandText.GetComponent<TextMeshProUGUI>().text = "Don't cut the RED wire!";
                break;
        }

        return randomCommand;
    }

    public void DefuseBomb(wireType clickedWire)
    {
        switch (trig)
        {
            case command.CutRed:
                if (clickedWire == wireType.Red)
                {
                    countWire--;
                    if (countWire == 0)
                        BombManager.instance.defused = true;
                }
                else
                    BombManager.instance.wrong = true;
                break;

            case command.DontCutRed:
                if (clickedWire == wireType.Blue)
                {
                    countWire--;
                    if (countWire == 0)
                        BombManager.instance.defused = true;
                }
                else
                    BombManager.instance.wrong = true;
                break;
        }
    }

    public void OnClickRedButton()
    {
        DefuseBomb(wireType.Red);
    }

    public void OnClickBlueButton()
    {
        DefuseBomb(wireType.Blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
