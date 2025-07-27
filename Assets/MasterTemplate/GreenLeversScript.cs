using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class GreenLeversScript : MonoBehaviour
{

    public KMBombModule Module;
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public Material[] Colors;
    public MeshRenderer[] Switches;
    public KMSelectable[] Buttons;
    public Animator[] animators;
    public GameObject[] highlights;
    int x;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;
    private bool ModuleActivated;
    List<int> colorOrder = new List<int>();
    List<bool[]> switchChecks = new List<bool[]>();
    List<int> currentUps = new List<int>();
    List<int> currentDowns = new List<int>();

    //Patterns for the venn diagram checks, judge me all you want.
    bool[] pattern1 = new bool[] { false, false, false, false };
    bool[] pattern2 = new bool[] { true, false, false, false };
    bool[] pattern3 = new bool[] { false, true, false, false };
    bool[] pattern4 = new bool[] { false, false, true, false };
    bool[] pattern5 = new bool[] { false, false, false, true };
    bool[] pattern6 = new bool[] { true, true, false, false };
    bool[] pattern7 = new bool[] { true, false, true, false };
    bool[] pattern8 = new bool[] { true, false, false, true };
    bool[] pattern9 = new bool[] { false, true, true, false };
    bool[] pattern10 = new bool[] { false, true, false, true };
    bool[] pattern11 = new bool[] { false, false, true, true };
    bool[] pattern12 = new bool[] { true, true, true, false };
    bool[] pattern13 = new bool[] { true, true, false, true };
    bool[] pattern14 = new bool[] { true, false, true, true };
    bool[] pattern15 = new bool[] { false, true, true, true };
    bool[] pattern16 = new bool[] { true, true, true, true };

    /// ///////////////////////////////////////////


    List<int> flippedSwitches = new List<int>();

    List<int> upAnswer = new List<int>();
    List<int> downAnswer = new List<int>();

    //Initialization
    void Awake()
    {
        ModuleId = ModuleIdCounter++;
        for (int i = 1; i < 8; i++)
        {
            x = Rnd.Range(0, 4);
            Switches[i].material = Colors[x];
            colorOrder.Add(x);
        }


        //Adds Green to the beginning of the list, since green is forced in the first position
        colorOrder.Insert(0, 2);
        Debug.LogFormat("[Green Levers #{0}] Lever colors are {1}", ModuleId, colorString());
        for (int i = 0; i < 8; i++)
        {
            Buttons[i].OnInteract += ButtonsPressed(i);
        }
    }

    void Start()
    {
        ModuleActivated = true;
        ModuleGeneration();
    }

    void ModuleGeneration()
    {

        //Initialize boolean arrays for the list

        for (int h = 0; h < 7; h++)
        {
            switchChecks.Add(new bool[4]);
        }

        ////////////////////////////////// Checks for conditions for Venn Diagram (Red,Green,Blue,Black)

        //Checking for even position
        for (int i = 0; i < 7; i++)
        {
            if ((i + 1) % 2 == 0)
            {
                switchChecks[i][0] = true;
            }
            else
            {
                switchChecks[i][0] = false;
            }
        }

        //Checking for color match to left

        for (int j = 0; j < 7; j++)
        {
            if (colorOrder[j] == colorOrder[j + 1])
            {
                switchChecks[j][1] = true;
            }
            else
            {
                switchChecks[j][1] = false;
            }
        }

        //Checking for color match to right

        for (int k = 0; k < 7; k++)
        {
            if (k == 6)
            {
                if (colorOrder[7] == colorOrder[0])
                {
                    switchChecks[6][2] = true;
                }
                else
                {
                    switchChecks[6][2] = false;
                }
            }
            else if (colorOrder[k + 1] == colorOrder[k + 2])
            {
                switchChecks[k][2] = true;
            }
            else
            {
                switchChecks[k][2] = false;
            }
        }

        //Checking for BOB

        for (int l = 0; l < 7; l++)
        {
            if (Bomb.IsIndicatorPresent("BOB"))
            {
                switchChecks[l][3] = true;
            }
            else
            {
                switchChecks[l][3] = false;
            }
        }



        //Log of conditions for each lever
        Debug.LogFormat("[Green Levers #{0}] The logging for the lever conditions will be in the order of Red, Green, Blue, and Black from the venn diagram.", ModuleId);
        for (int m = 0; m < 7; m++)
        {
            Debug.LogFormat("[Green Levers #{0}] Lever {1} conditions: [{2}]", ModuleId, m + 1, string.Join(", ", switchChecks[m].Select(b => b.ToString()).ToArray()));
        }

        //////////////// Venn Diagram Checks (U,D,R,Y,G,B,!R,!Y,!G,!B)
        for (int n = 0; n < 7; n++)
        {

            // Up
            if (ArraysEqual(switchChecks[n], pattern1) || ArraysEqual(switchChecks[n], pattern16))
            {
                upAnswer.Add(n + 1);
            }
            // Down
            else if (ArraysEqual(switchChecks[n], pattern6) || ArraysEqual(switchChecks[n], pattern9))
            {
                downAnswer.Add(n + 1);
            }

            // Red
            else if (ArraysEqual(switchChecks[n], pattern3) || ArraysEqual(switchChecks[n], pattern8))
            {
                if (colorOrder[n + 1] == 0)
                {
                    downAnswer.Add(n + 1);
                }
                else
                {
                    upAnswer.Add(n + 1);
                }
            }
            // Yellow
            else if (ArraysEqual(switchChecks[n], pattern7) || ArraysEqual(switchChecks[n], pattern13))
            {
                if (colorOrder[n + 1] == 1)
                {
                    downAnswer.Add(n + 1);
                }
                else
                {
                    upAnswer.Add(n + 1);
                }
            }

            // Green
            else if (ArraysEqual(switchChecks[n], pattern5))
            {
                if (colorOrder[n + 1] == 2)
                {
                    downAnswer.Add(n + 1);
                }
                else
                {
                    upAnswer.Add(n + 1);
                }
            }

            // Blue

            else if (ArraysEqual(switchChecks[n], pattern15))
            {
                if (colorOrder[n + 1] == 3)
                {
                    downAnswer.Add(n + 1);
                }
                else
                {
                    upAnswer.Add(n + 1);
                }
            }

            // Not Red
            else if (ArraysEqual(switchChecks[n], pattern14))
            {
                if (colorOrder[n + 1] != 0)
                {
                    downAnswer.Add(n + 1);
                }
                else
                {
                    upAnswer.Add(n + 1);
                }
            }
            // Not Yellow
            else if (ArraysEqual(switchChecks[n], pattern4))
            {
                if (colorOrder[n + 1] != 1)
                {
                    downAnswer.Add(n + 1);
                }
                else
                {
                    upAnswer.Add(n + 1);
                }
            }

            // Not Green
            else if (ArraysEqual(switchChecks[n], pattern2) || ArraysEqual(switchChecks[n], pattern11))
            {
                if (colorOrder[n + 1] != 2)
                {
                    downAnswer.Add(n + 1);
                }
                else
                {
                    upAnswer.Add(n + 1);
                }
            }

            // Not Blue
            else if (ArraysEqual(switchChecks[n], pattern10) || ArraysEqual(switchChecks[n], pattern12))
            {
                if (colorOrder[n + 1] != 3)
                {
                    downAnswer.Add(n + 1);
                }
                else
                {
                    upAnswer.Add(n + 1);
                }
            }
            else
            {
                Debug.LogFormat("[Green Levers #{0}] How did you get here?", ModuleId);
            }

        }
        Debug.LogFormat("[Green Levers #{0}] The levers that should be flipped up are: {1}", ModuleId, string.Join(", ", upAnswer.ConvertAll(b => b.ToString()).ToArray()));
        Debug.LogFormat("[Green Levers #{0}] The levers that should be flipped down are: {1}", ModuleId, string.Join(", ", downAnswer.ConvertAll(b => b.ToString()).ToArray()));
    }

    string colorString()
    {
        string output = "";
        for (int i = 0; i < 8; i++)
        {
            output += "RYGB"[colorOrder[i]];
        }
        return output;
    }

    bool ArraysEqual(bool[] a, bool[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    private KMSelectable.OnInteractHandler ButtonsPressed(int i)
    {
        return delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[i].transform);
            Buttons[i].AddInteractionPunch(0.5f);

            if (ModuleSolved || !ModuleActivated)
                return false;

            if (i == 0)
            {
                // Submission lever: only allow flipping once
                if (!animators[i].GetBool("Flip"))
                {
                    animators[i].SetBool("Flip", true);
                    Audio.PlaySoundAtTransform("Sound" + Rnd.Range(1, 6), transform);
                    Debug.LogFormat("[Green Levers #{0}] First lever flipped. Checking solution.", ModuleId);

                    // In the circumstance of striking, clear the lists so there isn't anything additional messing the answers up later.
                    currentUps.Clear();
                    currentDowns.Clear();

                    for (int j = 1; j < 8; j++) // Check levers and places them in appropriate list.
                    {
                        if (animators[j].GetBool("Flip"))
                        {
                            currentDowns.Add(j);
                        }
                        else
                        {
                            currentUps.Add(j);
                        }
                    }
                }
                bool correct = Enumerable.SequenceEqual(currentUps.OrderBy(n => n), upAnswer.Select(x => x).OrderBy(n => n)) &&
                             Enumerable.SequenceEqual(currentDowns.OrderBy(n => n), downAnswer.Select(x => x).OrderBy(n => n));
                if (correct)
                {
                    Debug.LogFormat("[Green Levers #{0}] Correct solution submitted. Module solved!", ModuleId);
                    ModuleSolved = true;
                    Module.HandlePass();
                }
                else
                {
                    Debug.LogFormat("[Green Levers #{0}] Incorrect solution submitted. Strike!", ModuleId);
                    Debug.LogFormat("[Green Levers #{0}] The levers that should be flipped up are: {1}", ModuleId, string.Join(", ", upAnswer.Select(n => n.ToString()).ToArray()));
                    Debug.LogFormat("[Green Levers #{0}] The levers that should be flipped down are: {1}", ModuleId, string.Join(", ", downAnswer.Select(n => n.ToString()).ToArray()));
                    Module.HandleStrike();
                    StartCoroutine(ResetSubmissionLever());
                }
            }
            else
            {
                // Input levers: freely toggle
                bool currentlyFlipped = animators[i].GetBool("Flip");
                animators[i].SetBool("Flip", !currentlyFlipped);
                Audio.PlaySoundAtTransform("Sound" + Rnd.Range(1, 6), transform);
                highlights[i].transform.localPosition = new Vector3(0f, 0f, currentlyFlipped ? -0.0004f : 0.0004f);
            }
            return false;
        };
    }

    private IEnumerator ResetSubmissionLever()
    {
        yield return new WaitForSeconds(1f);
        animators[0].SetBool("Flip", false);
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} flip 1 2 3 4 5 6 7 8 [Flip said levers, numbered 1-8 from left to right.]";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        var parameters = command.ToLowerInvariant().Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (parameters.Length == 0)
            yield break;

        var skip = (new[] { "flip", "press", "switch", "toggle" }).Contains(parameters[0]) ? 1 : 0;

        if (parameters.Skip(skip).Any(i => { int val; return !int.TryParse(i.Trim(), out val) || val < 1 || val > 8; }))
            yield break;

        var sws = parameters.Skip(skip).Select(i => int.Parse(i.Trim()) - 1).ToArray();
        if (sws.Distinct().Count() != sws.Count())
        {
            yield return "sendtochaterror Duplicate lever flips detected! Command ignored.";
            yield break;
        }
        if (sws.Any(i => animators[i].GetBool("Flip")))
        {
            yield return "sendtochaterror Attempting to flip an already flipped lever! Command ignored.";
            yield break;
        }
        yield return null;

        foreach (var sw in sws)
        {
            Buttons[sw].OnInteract();
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
        for (int i = 1; i < 8; i++)
        {
            if (upAnswer.Contains(i) == animators[i].GetBool("Flip"))
            {
                Buttons[i].OnInteract();
                yield return new WaitForSeconds(0.25f);
            }
        }
        Buttons[0].OnInteract();
    }
}
