using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class VisualImpairment : MonoBehaviour
{

    public KMBombModule impairment;
    public KMSelectable[] buttons;
    public KMAudio Audio;
    public GameObject indicator;
    private static int _moduleIdCounter = 1;
    private int _moduleId;
    private int pictureNum;
    private int orientation;
    private int color;
    public string[] colorList;
    private string[] picture;
    public Material[] materials;
    private int White;
    private int Blue;
    private int Green;
    private int Red;
    private int correctButtons = 0;
    private int correctButtonsPressed = 0;
    private bool firstPress;
    private HashSet<int> buttonsPressed;
    private int roundsFinished = 0;
    private bool moduleSolved = false;
    private int stageCount;
    public GameObject buttonGroup;
    private bool flip;

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        stageCount = Random.Range(1, 4);
        pictureNum = Random.Range(0, 8);
        pictureNum = 8;
        orientation = Random.Range(1, 9);
        orientation = 1;
        color = Random.Range(0, 4);
        PickPicture();
        impairment.OnActivate += Activate;
        White = Random.Range(3, 7);
        Blue = Random.Range(3, 7);
        Green = Random.Range(3, 7);
        Red = Random.Range(3, 7);
        buttonsPressed = new HashSet<int>();
        flip = orientation > 4;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].OnInteract += ButtonPressed(i);
        }

    }


    private KMSelectable.OnInteractHandler ButtonPressed(int i)
    {
        return delegate
        {
            Audio.PlaySoundAtTransform("tick", buttons[i].transform);
            if (buttonsPressed.Contains(i) || moduleSolved)
            {
                return false;
            }
            buttonsPressed.Add(i);
            if (firstPress == false && picture[i] == colorList[color])
            {
                ClearPicture();
                firstPress = true;
            }
            if (picture[i] == colorList[color])
            {
                correctButtonsPressed++;
                if (color == 0)
                {
                    buttons[i].gameObject.GetComponent<Renderer>().material = materials[3];
                }
                else
                {
                    buttons[i].gameObject.GetComponent<Renderer>().material = materials[color - 1];
                }
                if (correctButtons == correctButtonsPressed)
                {
                    HandleRoundEnd();
                }

            }
            else
            {
                DebugLog("Button #{0} was incorrect.", i + 1);
                impairment.HandleStrike();
                StartCoroutine(BreakButtons());
                return false;
            }

            return false;
        };
    }

    private void HandleRoundEnd()
    {
        roundsFinished++;
        DebugLog("Stage #{0} finished.", roundsFinished);
        if (roundsFinished == stageCount)
        {
            DebugLog("Module passed.");
            foreach (KMSelectable button in buttons)
            {
                button.gameObject.GetComponent<Renderer>().material = materials[7];
            }
            indicator.gameObject.GetComponent<Renderer>().material = materials[4];
            moduleSolved = true;
            impairment.HandlePass();
            return;
        }
        ResetModule();
    }

    private void ResetModule()
    {
        pictureNum = Random.Range(0, 8);
        orientation = Random.Range(1, 9);
        color = Random.Range(0, 4);
        PickPicture();
        White = Random.Range(3, 7);
        Blue = Random.Range(3, 7);
        Green = Random.Range(3, 7);
        Red = Random.Range(3, 7);
        buttonsPressed.Clear();
        correctButtons = 0;
        correctButtonsPressed = 0;
        flip = orientation > 4;
        firstPress = false;
        Activate();
    }

    private void ClearPicture()
    {
        foreach (KMSelectable button in buttons)
        {
            button.gameObject.GetComponent<Renderer>().material = materials[4];
        }
    }

    void Update()
    {

    }

    void Activate()
    {
        SetPicture();
        FindCorrectButtonAmount();
        if (color == 0)
        {
            indicator.gameObject.GetComponent<Renderer>().material = materials[3];
        }
        else
        {
            indicator.gameObject.GetComponent<Renderer>().material = materials[color - 1];
        }
    }

    void PickPicture()
    {
        switch (pictureNum)
        {
            case 0:
                picture = new string[25]
                {
                    "R", "G", "R", "W", "G",
                    "B", "W", "B", "R", "R",
                    "G", "B", "R", "B", "G",
                    "B", "W", "W", "G", "B",
                    "R", "B", "W", "G", "R",
                };
                DebugLog("Picture 1 was chosen.");
                break;
            case 1:
                picture = new string[25]
                {
                    "G", "B", "R", "B", "R",
                    "G", "G", "G", "R", "W",
                    "W", "W", "W", "W", "W",
                    "W", "R", "G", "G", "G",
                    "R", "B", "R", "B", "G",
                };
                DebugLog("Picture 2 was chosen.");
                break;
            case 2:
                picture = new string[25]
               {
                    "B", "B", "B", "R", "R",
                    "R", "B", "G", "R", "R",
                    "R", "G", "G", "G", "B",
                    "R", "W", "W", "W", "B",
                    "R", "B", "W", "B", "B",
               };
                DebugLog("Picture 3 was chosen.");
                break;
            case 3:
                picture = new string[25]
               {
                    "B", "R", "W", "W", "B",
                    "W", "R", "B", "R", "B",
                    "W", "B", "R", "W", "R",
                    "G", "W", "B", "W", "G",
                    "B", "G", "G", "G", "B",
               };
                DebugLog("Picture 4 was chosen.");
                break;
            case 4:
                picture = new string[25]
               {
                    "R", "G", "R", "G", "W",
                    "W", "B", "R", "R", "B",
                    "B", "B", "R", "W", "W",
                    "W", "G", "G", "W", "B",
                    "B", "R", "G", "R", "G",
               };
                DebugLog("Picture 5 was chosen.");
                break;
            case 5:
                picture = new string[25]
               {
                    "W", "G", "W", "B", "G",
                    "B", "G", "R", "B", "W",
                    "B", "G", "R", "G", "W",
                    "R", "W", "B", "G", "B",
                    "R", "W", "R", "R", "B",
               };
                DebugLog("Picture 6 was chosen.");
                break;
            case 6:
                picture = new string[25]
               {
                    "G", "R", "W", "R", "W",
                    "R", "W", "R", "W", "B",
                    "B", "R", "W", "B", "G",
                    "W", "G", "B", "G", "B",
                    "R", "W", "G", "B", "G",
               };
                DebugLog("Picture 7 was chosen.");
                break;
            case 7:
                picture = new string[25]
               {
                    "B", "B", "R", "W", "W",
                    "B", "G", "W", "G", "W",
                    "R", "R", "G", "W", "R",
                    "B", "G", "G", "G", "R",
                    "R", "G", "W", "G", "B",
               };
                DebugLog("Picture 8 was chosen.");
                break;
            case 8:
                picture = new string[25]
               {
                    "W", "B", "R", "W", "G",
                    "B", "G", "W", "G", "W",
                    "R", "W", "B", "W", "R",
                    "G", "B", "G", "G", "B",
                    "W", "R", "G", "R", "B",
               };
                DebugLog("Picture 9 was chosen.");
                break;
        }
    }

    void SetPicture()
    {
        if (orientation == 1 || orientation == 5)
        {
            RotatePicture(0);
        }
        else if (orientation == 2 || orientation == 6)
        {
            RotatePicture(90);
        }
        else if (orientation == 3 || orientation == 7)
        {
            RotatePicture(180);
        }
        else if (orientation == 4 || orientation == 8)
        {
            RotatePicture(270);
        }

        for (int i = 0; i < buttons.Length; i++)
            SetColor(buttons[i], i);
    }

    private void SetColor(KMSelectable button, int index)
    {
        while (Blue == White)
        {
            Blue = Random.Range(3, 7);
        }
        while (Green == White || Green == Blue)
        {
            Green = Random.Range(3, 7);
        }
        while (Red == White || Red == Green || Red == Blue)
        {
            Red = Random.Range(3, 7);
        }
        if (picture[index] == "W")
        {
            button.GetComponent<Renderer>().material = materials[White];
        }
        else if (picture[index] == "B")
        {
            button.GetComponent<Renderer>().material = materials[Blue];
        }
        else if (picture[index] == "G")
        {
            button.GetComponent<Renderer>().material = materials[Green];
        }
        else if (picture[index] == "R")
        {
            button.GetComponent<Renderer>().material = materials[Red];
        }
    }

    private void RotatePicture(int degrees)
    {
        buttonGroup.transform.localEulerAngles = new Vector3(0, degrees, flip ? 180 : 0);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Highlight.transform.localPosition = new Vector3(0, flip ? -.52f : .52f, 0);
            buttons[i].Highlight.transform.localEulerAngles = new Vector3(0, 0, flip ? 180 : 0);
        }
    }

    private void DebugLog(string log, params object[] args)
    {
        var logData = string.Format(log, args);
        Debug.LogFormat("[Visual Impairment #{0}] {1}", _moduleId, logData);
    }


    public void FindCorrectButtonAmount()
    {
        foreach (string button in picture)
        {
            if (button == colorList[color])
            {
                correctButtons++;
            }
        }
        DebugLog("Number of correct buttons on this picture: {0}", correctButtons);
    }

    IEnumerator BreakButtons()
    {
        foreach (KMSelectable button in buttons)
        {
            button.gameObject.GetComponent<Renderer>().material = materials[7];
        }
        yield return new WaitForSeconds(1);
        ResetModule();
    }
}
