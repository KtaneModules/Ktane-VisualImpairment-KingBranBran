using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool anyPressed;
    private HashSet<int> buttonsLeftToPress;
    private int roundsFinished = 0;
    private bool moduleSolved = false;
    private int stageCount;
    public GameObject buttonGroup;

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        stageCount = Random.Range(2, 5);

        DebugLog("This module will have {0} stages.", stageCount);
        DebugLog("Start of stage #1.");

        impairment.OnActivate += ResetModule;
        buttonsLeftToPress = new HashSet<int>();

        for (int i = 0; i < buttons.Length; i++)
            buttons[i].OnInteract += ButtonPressed(i);
    }

    private KMSelectable.OnInteractHandler ButtonPressed(int rotated)
    {
        return delegate
        {
            Audio.PlaySoundAtTransform("tick", buttons[rotated].transform);
            if (moduleSolved)
                return false;

            var i = unrotate(rotated);
            DebugLog("You pressed {0}{1}, which is {2}{3} in the original picture.", (char)('A' + rotated % 5), (char)('1' + rotated / 5), (char)('A' + i % 5), (char)('1' + i / 5));
            if (picture[i] == colorList[color])
            {
                buttonsLeftToPress.Remove(i);
                if (!anyPressed)
                {
                    ClearPicture();
                    anyPressed = true;
                }
                buttons[rotated].gameObject.GetComponent<Renderer>().material = materials[color];
                if (buttonsLeftToPress.Count == 0)
                    HandleRoundEnd();
            }
            else
            {
                DebugLog("Button was incorrect.");
                impairment.HandleStrike();
                StartCoroutine(DelayThenReset());
                return false;
            }

            return false;
        };
    }

    private int unrotate(int i)
    {
        var x = i % 5;
        var y = i / 5;
        switch (orientation)
        {
            case 1: return i;
            case 2: return 5 * (4 - x) + y;
            case 3: return 5 * (4 - y) + 4 - x;
            case 4: return 5 * x + 4 - y;
            case 5: return 5 * y + 4 - x;
            case 6: return 5 * (4 - x) + 4 - y;
            case 7: return 5 * (4 - y) + x;
            default: return 5 * x + y;
        }
    }

    private int rotate(int i)
    {
        var x = i % 5;
        var y = i / 5;
        switch (orientation)
        {
            case 1: return i;
            case 2: return 5 * x + 4 - y;
            case 3: return 5 * (4 - y) + 4 - x;
            case 4: return 5 * (4 - x) + y;
            case 5: return 5 * y + 4 - x;
            case 6: return 5 * (4 - x) + 4 - y;
            case 7: return 5 * (4 - y) + x;
            default: return 5 * x + y;
        }
    }

    private void HandleRoundEnd()
    {
        roundsFinished++;
        DebugLog("Stage #{0} finished.", roundsFinished);
        if (roundsFinished == stageCount)
        {
            DebugLog("Module passed.");
            Audio.PlaySoundAtTransform("modulepass", buttons[12].transform);
            foreach (KMSelectable button in buttons)
            {
                button.gameObject.GetComponent<Renderer>().material = materials[7];
            }
            indicator.gameObject.GetComponent<Renderer>().material = materials[4];
            moduleSolved = true;
            impairment.HandlePass();
            return;
        }
        else
        {
            Audio.PlaySoundAtTransform("stagepass", buttons[12].transform);
            DebugLog("Start of stage #{0}.", roundsFinished + 1);
            StartCoroutine(DelayThenReset());
        }
    }

    private void ResetModule()
    {
        color = Random.Range(0, 4);

        indicator.GetComponent<Renderer>().material = materials[color];

        DebugLog("Color to press: {0}", new[] { "Blue", "Green", "Red", "White" }[color]);

        var colorNumbers = Enumerable.Range(3, 4).ToList();
        White = PickRandomFrom(colorNumbers);
        Blue = PickRandomFrom(colorNumbers);
        Green = PickRandomFrom(colorNumbers);
        Red = PickRandomFrom(colorNumbers);

        buttonsLeftToPress.Clear();
        anyPressed = false;

        PickPicture();

        DebugLog("Buttons to press: {0}", string.Join(", ", buttonsLeftToPress.Select(ix => rotate(ix)).Select(ix => "" + (char)('A' + ix % 5) + (char)('1' + ix / 5)).OrderBy(str => str).ToArray()));
    }

    private int PickRandomFrom(List<int> list)
    {
        var ix = Random.Range(0, list.Count);
        var result = list[ix];
        list.RemoveAt(ix);
        return result;
    }

    private void ClearPicture()
    {
        foreach (KMSelectable button in buttons)
        {
            button.gameObject.GetComponent<Renderer>().material = materials[4];
        }
    }

    void PickPicture()
    {
        orientation = Random.Range(1, 9);
        pictureNum = Random.Range(0, 9);
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
                break;
        }

        for (int i = 0; i < picture.Length; i++)
        {
            if (picture[i] == "W")
            {
                buttons[rotate(i)].GetComponent<Renderer>().material = materials[White];
            }
            else if (picture[i] == "B")
            {
                buttons[rotate(i)].GetComponent<Renderer>().material = materials[Blue];
            }
            else if (picture[i] == "G")
            {
                buttons[rotate(i)].GetComponent<Renderer>().material = materials[Green];
            }
            else if (picture[i] == "R")
            {
                buttons[rotate(i)].GetComponent<Renderer>().material = materials[Red];
            }

            if (picture[i] == colorList[color])
                buttonsLeftToPress.Add(i);
        }

        DebugLog("Picture {0} was chosen at rotation {1}{2}.", pictureNum + 1, ((orientation - 1) % 4) * 90, orientation >= 4 ? " and flipped" : "");
    }

    private void DebugLog(string log, params object[] args)
    {
        var logData = string.Format(log, args);
        Debug.LogFormat("[Visual Impairment #{0}] {1}", _moduleId, logData);
    }

    IEnumerator DelayThenReset()
    {
        indicator.gameObject.GetComponent<Renderer>().material = materials[4];
        foreach (KMSelectable button in buttons)
            button.gameObject.GetComponent<Renderer>().material = materials[7];
        yield return new WaitForSeconds(1);
        ResetModule();
    }

#pragma warning disable 414
    private string TwitchHelpMessage = @"Use “!{0} a4 b3 d2” to press the squares. Letters are columns and numbers are rows.";
#pragma warning restore 414

    private static string[] supportedTwitchCommands = new[] { "press", "click", "submit" };

    IEnumerator ProcessTwitchCommand(string command)
    {
        var parts = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        var skip = 0;
        if (parts.Length > 0 && supportedTwitchCommands.Contains(parts[0]))
            skip = 1;

        if (parts.Length > skip && parts.Skip(skip).All(part => part.Length == 2 && "abcde".Contains(part[0]) && "12345".Contains(part[1])))
        {
            yield return null;

            for (int i = skip; i < parts.Length; i++)
            {
                var x = parts[i][0] - 'a';
                var y = parts[i][1] - '1';
                buttons[y * 5 + x].OnInteract();
                yield return new WaitForSeconds(.1f);
            }
        }
    }
}
