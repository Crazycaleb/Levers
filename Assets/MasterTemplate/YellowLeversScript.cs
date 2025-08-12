using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class YellowLeversScript : MonoBehaviour
{

    public KMBombModule Module;
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public Material[] Colors;
    public MeshRenderer[] Switches;
    public KMSelectable[] Buttons;
    public Animator[] animators;

    static int ModuleIdCounter = 1;
    int ModuleId;
    int x;
    int j = 0;
    private bool ModuleSolved;
    private bool ModuleActivated;

    int secondNumber;
    bool fuckedUp = false;
    int? doNotFlip = null;
    int redLevers;
    int yellowLevers;
    int blueLevers;
    int greenLevers;
    int sum = 0;
    int rowNumber;
    int keyLever;
    List<int> colorOrder = new List<int>();
    List<int> switchHits = new List<int>();

    //private List<int> flippedSwitches = new List<int>();

    //Initialization
    void Awake()
    {
        ModuleId = ModuleIdCounter++;
        Bomb.OnBombExploded += delegate () { fuckedUp = false; };
        for (int i = 1; i < 8; i++)
        {
            x = Rnd.Range(0, 4);
            Switches[i].material = Colors[x];
            colorOrder.Add(x);
            if (x == 0)
            {
                redLevers++;

            }
            if (x == 1)
            {
                yellowLevers++;

            }
            if (x == 2)
            {
                greenLevers++;

            }
            if (x == 3)
            {
                blueLevers++;
            }
        }


        //Adds yellow to the beginning of the list, since yellow is forced in the first position
        colorOrder.Insert(0, 1);
        Debug.LogFormat("Color Order: " + colorOrder.Join(","));
        yellowLevers++;
        Debug.LogFormat("[Yellow Levers #{0}] Lever colors are {1}", ModuleId, colorString());
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
        keyLever = Bomb.GetSolvableModuleNames().Count % 8;
        Debug.LogFormat("[Yellow Levers #{0}] Key lever is {1}.", ModuleId, keyLever + 1);
        string vowels = "AEIOU";

        foreach (char c in Bomb.GetSerialNumber().ToUpper())
        {
            if (char.IsLetter(c))
            {
                if (!vowels.Contains(c))
                {
                    sum += c - 'A' + 1;
                }
            }
            else if (char.IsNumber(c))
            {
                sum += c - '0';
            }
        }

        rowNumber = ((sum * Bomb.GetBatteryCount()) + Bomb.GetIndicators().Count()) % 30;


        Debug.LogFormat("[Yellow Levers #{0}] Starting row is {1}.", ModuleId, rowNumber);



        while (doNotFlip == null)
        {

            while (switchHits.Contains(rowNumber))
            {
                rowNumber = (rowNumber + 1) % 30;
            }

            switchHits.Add(rowNumber);

            switch (rowNumber)
            {
                case 0:
                    if (colorOrder[2] == 1)
                    {
                        doNotFlip = 2;
                        LogReason("3rd switch is Yellow.");
                    }
                    else
                    {
                        LogReason("3rd switch is not yellow, moving down " + (Bomb.GetBatteryCount() + 2) + " rows.");
                        rowNumber += Bomb.GetBatteryCount() + 2;
                    }
                    break;

                case 1:
                    if (colorOrder[keyLever] == 0)
                    {
                        doNotFlip = 0;
                        LogReason("The Key Lever is red.");
                    }
                    else
                    {
                        LogReason("The Key Lever is not red, moving down " + ((Bomb.GetOnIndicators().Count() + 1) * 2) + " rows.");
                        rowNumber += (Bomb.GetOnIndicators().Count() + 1) * 2;
                    }
                    break;

                case 2:
                    if (colorOrder[keyLever] == 3)
                    {
                        doNotFlip = keyLever;
                        LogReason("The Key Lever is blue.");
                    }
                    else
                    {
                        LogReason("The Key Lever is not blue, moving down " + (Bomb.GetPorts().Count() + 1) + " rows.");
                        rowNumber += Bomb.GetPorts().Count() + 1;
                    }
                    break;

                case 3:
                    if (Bomb.GetPorts().Count() == 0)
                    {
                        doNotFlip = 5;
                        LogReason("No ports present on the bomb.");
                    }
                    else
                    {
                        LogReason("At least one port is present, moving down " + (keyLever + 1) + " rows.");
                        rowNumber += keyLever + 1;
                    }
                    break;

                case 4:
                    if (colorOrder[1] != 2)
                    {
                        doNotFlip = 1;
                        LogReason("The 2nd lever is not green.");
                    }
                    else
                    {
                        LogReason("The 2nd lever is green, moving down " + (Bomb.GetBatteryHolderCount() + 7) + " rows.");
                        rowNumber += Bomb.GetBatteryHolderCount() + 7;
                    }
                    break;

                case 5:
                    if (keyLever == 1)
                    {
                        doNotFlip = 7;
                        LogReason("The Key Lever is the 2nd lever.");
                    }
                    else
                    {
                        if (colorOrder[keyLever] == 0)
                        {
                            LogReason("The 2nd lever is green, moving down 18 rows.");
                            rowNumber += 18;
                        }
                        else if (colorOrder[keyLever] == 1)
                        {
                            LogReason("The 2nd lever is green, moving down 25 rows.");
                            rowNumber += 25;
                        }
                        else if (colorOrder[keyLever] == 2)
                        {
                            LogReason("The 2nd lever is green, moving down 7 rows.");
                            rowNumber += 7;
                        }
                        else
                        {
                            LogReason("The 2nd lever is green, moving down 2 rows.");
                            rowNumber += 2;
                        }
                    }
                    break;

                case 6:
                    if (blueLevers == 0)
                    {
                        doNotFlip = 4;
                        LogReason("There are no blue levers.");
                    }
                    else
                    {
                        LogReason("There is at least one blue lever, moving down " + ((blueLevers * 3) - 1) + " rows.");
                        rowNumber += (blueLevers * 3) - 1;
                    }
                    break;

                case 7:
                    if (Bomb.GetBatteryCount() > 3)
                    {
                        doNotFlip = 2;
                        LogReason("There are more than 3 batteries.");
                    }
                    else
                    {
                        LogReason("There are 3 or fewer batteries, moving down " + (Bomb.GetBatteryCount() + 3) + " rows.");
                        rowNumber += Bomb.GetBatteryCount() + 3;
                    }
                    break;

                case 8:
                    if (colorOrder[keyLever] != 2)
                    {
                        doNotFlip = 3;
                        LogReason("The Key Lever is not green.");
                    }
                    else
                    {
                        LogReason("The Key Lever is green, moving down " + (keyLever + 1) + " rows.");
                        rowNumber += keyLever + 1;
                    }
                    break;

                case 9:
                    if (!Bomb.GetModuleNames().Any(q => q == "Red Levers"))
                    {
                        doNotFlip = 6;
                        LogReason("There are no Red Lever modules on the bomb.");
                    }
                    else
                    {
                        LogReason("There is a Red Levers module on the bomb, moving down " + (Bomb.GetModuleNames().Count(q => q.Contains("Levers"))) + " rows.");
                        rowNumber += Bomb.GetModuleNames().Count(q => q.Contains("Levers"));
                    }
                    break;

                case 10:
                    if (colorOrder[keyLever] == 0)
                    {
                        doNotFlip = keyLever;
                        LogReason("The Key Lever is red.");
                    }
                    else
                    {
                        LogReason("The Key Lever is not red, moving down " + (yellowLevers + 2) + " rows.");
                        rowNumber += yellowLevers + 2;
                    }
                    break;

                case 11:
                    if (colorOrder[1] == colorOrder[keyLever])
                    {
                        doNotFlip = 1;
                        LogReason("The 2nd lever matches the color of the Key Lever.");
                    }
                    else
                    {
                        LogReason("The 2nd lever's color does not match the Key Lever's color, moving down " + keyLever + " rows.");
                        rowNumber += keyLever;
                    }
                    break;

                case 12:
                    if ((keyLever + 1) == Bomb.GetSerialNumberNumbers().Last())
                    {
                        doNotFlip = 4;
                        LogReason("The Key Lever's position matches the last digit of the serial number.");
                    }
                    else
                    {
                        LogReason("The Key Lever's position does not match the last digit of the serial number, moving down " + Bomb.GetSerialNumberNumbers().Last() + " rows.");
                        rowNumber += Bomb.GetSerialNumberNumbers().Last();
                    }
                    break;

                case 13:
                    if (colorOrder[5] != 1)
                    {
                        doNotFlip = 5;
                        LogReason("The 6th lever's color is not yellow.");
                    }
                    else
                    {
                        LogReason("The 6th lever's color is yellow, moving down " + (Bomb.GetPortPlates().Count() + 2) + " rows.");
                        rowNumber += Bomb.GetPortPlates().Count() + 2;
                    }
                    break;

                case 14:
                    if (Bomb.GetModuleNames().Count - Bomb.GetSolvableModuleNames().Count > 0)
                    {
                        doNotFlip = 6;
                        LogReason("There is a needy present on the bomb.");
                    }
                    else
                    {
                        LogReason("There is no needy present on the bomb, moving down " + (Bomb.GetSolvableModuleNames().Count - 1) + " rows.");
                        rowNumber += Bomb.GetSolvableModuleNames().Count - 1;
                    }
                    break;

                case 15:
                    if (colorOrder[keyLever] != 3)
                    {
                        doNotFlip = 0;
                        LogReason("The Key Lever's color is not blue.");
                    }
                    else
                    {
                        LogReason("The Key Lever's color is blue, moving down " + (Bomb.GetModuleNames().Count(q => q.Contains("Levers")) + 1) + " rows.");
                        rowNumber += Bomb.GetModuleNames().Count(q => q.Contains("Levers")) + 1;
                    }
                    break;

                case 16:
                    if (keyLever != 2)
                    {
                        doNotFlip = 2;
                        LogReason("The Key Lever is not in the 3rd position.");
                    }
                    else
                    {
                        if (keyLever == 1 || keyLever == 3)
                        {
                            LogReason("The Key Lever is in the 3rd position, moving down 1 row.");
                            rowNumber += 1;
                        }
                        else if (keyLever == 0 || keyLever == 4)
                        {
                            LogReason("The Key Lever is in the 3rd position, moving down 2 rows.");
                            rowNumber += 2;
                        }
                        else if (keyLever == 5)
                        {
                            LogReason("The Key Lever is in the 3rd position, moving down 3 rows.");
                            rowNumber += 3;
                        }
                        else if (keyLever == 6)
                        {
                            LogReason("The Key Lever is in the 3rd position, moving down 4 rows.");
                            rowNumber += 4;
                        }
                        else
                        {
                            LogReason("The Key Lever is in the 3rd position, moving down 5 rows.");
                            rowNumber += 5;
                        }
                    }
                    break;

                case 17:
                    if (Bomb.GetBatteryCount() == 0)
                    {
                        doNotFlip = 4;
                        LogReason("There are no batteries on the bomb.");
                    }
                    else
                    {
                        LogReason("There is at least one battery on the bomb, moving down " + (Bomb.GetBatteryCount() * 2) + " rows.");
                        rowNumber += Bomb.GetBatteryCount() * 2;
                    }
                    break;

                case 18:
                    if (colorOrder[keyLever] != colorOrder[7])
                    {
                        doNotFlip = 7;
                        LogReason("The color of the last lever matches the color of the Key Lever.");
                    }
                    else
                    {
                        int[] diffColors = { redLevers, yellowLevers, greenLevers, blueLevers };
                        LogReason("The color of the last lever does not match the color of the Key Lever, moving down " + diffColors[colorOrder[keyLever]] + " row(s).");
                        rowNumber += diffColors[colorOrder[keyLever]]; //Important mention to include itself in the manual*************************************
                    }
                    break;

                case 19:
                    if (colorOrder[7] == 0)
                    {
                        doNotFlip = keyLever;
                        LogReason("The last lever's color is red.");
                    }
                    else
                    {
                        if (colorOrder[keyLever] == 0)
                        {
                            LogReason("The last lever's color is not red, moving down 4 rows.");
                            rowNumber += 4;
                        }
                        else if (colorOrder[keyLever] == 1)
                        {
                            LogReason("The last lever's color is not red, moving down 23 rows.");
                            rowNumber += 23;
                        }
                        else if (colorOrder[keyLever] == 2)
                        {
                            LogReason("The last lever's color is not red, moving down 14 rows.");
                            rowNumber += 14;
                        }
                        else
                        {
                            LogReason("The last lever's color is not red, moving down 5 rows.");
                            rowNumber += 5;
                        }
                    }
                    break;

                case 20:
                    if (colorOrder[1] == colorOrder[2])
                    {
                        doNotFlip = 1;
                        LogReason("The color of the 2nd and 3rd levers match.");
                    }
                    else
                    {
                        LogReason("The color of the 2nd and 3rd levers don't match, moving down " + (keyLever + 2) + " rows.");
                        rowNumber += keyLever + 2; //+1 for the zero-index, +1 for the rule
                    }
                    break;

                case 21:
                    if (colorOrder[keyLever] == 0)
                    {
                        doNotFlip = keyLever;
                        LogReason("The color of the Key Lever is red.");
                    }
                    else
                    {
                        LogReason("The color of the Key Lever is not red, moving down " + (redLevers + 2) + " rows.");
                        rowNumber += redLevers + 2;
                    }
                    break;

                case 22:
                    if (colorOrder[6] != 2)
                    {
                        doNotFlip = 6;
                        LogReason("The 7th lever's color is not green.");
                    }
                    else
                    {
                        LogReason("The 7th lever's color is green, moving down " + (greenLevers + 7) + " rows.");
                        rowNumber += greenLevers + 7;
                    }
                    break;

                case 23:
                    if (Bomb.IsPortPresent(Port.Serial))
                    {
                        doNotFlip = 3;
                        LogReason("There is a serial port on the bomb.");
                    }
                    else
                    {
                        int ports = Bomb.GetPortCount();
                        int serialPorts = Bomb.GetPortCount(Port.Serial);
                        LogReason("There isn't a serial port on the bomb, moving down " + (ports - serialPorts + 1) + " rows.");
                        rowNumber += ports - serialPorts + 1;

                    }
                    break;

                case 24:
                    if (colorOrder[2] == 3)
                    {
                        doNotFlip = 0;
                        LogReason("The 3rd lever's color is blue.");
                    }
                    else
                    {
                        LogReason("The 3rd lever's color is not blue, moving down " + (yellowLevers + 5) + " rows.");
                        rowNumber += yellowLevers + 5;
                    }
                    break;

                case 25:
                    if (Bomb.GetModuleNames().Any(x => x == "Red Levers"))
                    {
                        doNotFlip = keyLever;
                        LogReason("There is a Red Levers module on the bomb.");
                    }
                    else
                    {
                        LogReason("The is not a Red Levers module on the bomb, moving down " + ((keyLever + 1) * 2) + " rows.");
                        rowNumber = (keyLever + 1) * 2;
                    }
                    break;

                case 26:
                    if (Bomb.GetSerialNumberNumbers().Last() == 6)
                    {
                        doNotFlip = 5;
                        LogReason("The last digit of the serial number is 6.");
                    }
                    else
                    {
                        LogReason("The last digit of the serial number is not 6, moving down " + (Bomb.GetSerialNumberNumbers().Last() + 3) + " rows.");
                        rowNumber += Bomb.GetSerialNumberNumbers().Last() + 3;
                    }
                    break;

                case 27: //Forced Key Lever avoidance
                    doNotFlip = keyLever;
                    break;

                case 28:
                    if (colorOrder[2] == colorOrder[7])
                    {
                        doNotFlip = 2;
                        LogReason("The colors of the 3rd and last levers match.");
                    }
                    else
                    {
                        char firstLetter = Bomb.GetSerialNumberLetters().First();
                        LogReason("The colors of the 3rd and last levers don't match, moving down " + (char.ToUpper(firstLetter) - 'A' + 1) + " rows.");
                        rowNumber += char.ToUpper(firstLetter) - 'A' + 1;
                    }
                    break;

                case 29:
                    if (colorOrder[keyLever] == 2)
                    {
                        doNotFlip = keyLever;
                        LogReason("The color of the Key Lever is green.");
                    }
                    else
                    {
                        LogReason("The color of the Key Lever is not green, moving down " + (redLevers + yellowLevers + blueLevers) + " rows.");
                        rowNumber += redLevers + yellowLevers + blueLevers;
                    }
                    break;
            }

            rowNumber = rowNumber % 30;
            if (doNotFlip != null)
            {
                Debug.LogFormat("[Yellow Levers #{0}] Do not flip lever {1}. DO! NOT!", ModuleId, doNotFlip + 1);
            }
        }
    }

    void LogReason(string reason)
    {
        Debug.LogFormat("[Yellow Levers #{0}] Checking row {1}: {2}", ModuleId, rowNumber, reason);
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


    private KMSelectable.OnInteractHandler ButtonsPressed(int i)
    {
        return delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[i].transform);
            Buttons[i].AddInteractionPunch(0.5f);
            if (ModuleSolved || !ModuleActivated) //Nothing will happen if you have solved it or are pressing it early like a bozo
                return false;
            if (animators[i].GetBool("Flip") == true)
            {
                return false;
            }
            if (i == doNotFlip) //You fucked up lol
            {
                fuckedUp = true;
                animators[i].SetBool("Flip", true);
                Audio.PlaySoundAtTransform("Sound" + Rnd.Range(1, 6), transform);
                Debug.LogFormat("[Yellow Levers #{0}] Pressed lever #{1}. You weren't supposed to flip that! Strike!", ModuleId, i + 1);
                Module.HandleStrike();
            }
            else
            {
                if (i != doNotFlip)
                {
                    j++;
                    animators[i].SetBool("Flip", true);
                    Audio.PlaySoundAtTransform("Sound" + Rnd.Range(1, 6), transform);
                }

                if (j == 7)
                {
                    Module.HandlePass();
                    ModuleSolved = true;
                    Debug.LogFormat("[Yellow Levers #{0}] All other levers flipped. Module Solved!", ModuleId);
                }
            }

            return false;
        };
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} flip 1 2 3 4 5 6 7 8 [Flip said levers, numbered 1-8 from left to right.]";
#pragma warning restore 414
    private bool TwitchPlaysActive;

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
        for (int i = 0; i < 8; i++)
        {
            if (animators[i].GetBool("Flip") || i == doNotFlip)
                continue;
            Buttons[i].OnInteract();
            yield return new WaitForSeconds(0.25f);
        }
    }
}
