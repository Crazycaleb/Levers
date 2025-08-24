using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class BlueLeversScript : MonoBehaviour
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
    List<int> colorOrder = new List<int>();
    List<int> safeLevers = new List<int>();
    int[] scores = new int[3];
    DateTime today;
    int todayDay;
    string serial;
    int fourth;
    int leftmost;
    int rightmost;
    int rowNumber;
    int leverA;
    int leverB;
    int leverC;
    int scoreA = 1;
    int scoreB = 1;
    int scoreC = 1;
    int a;
    int b;
    int c;
    int answer;
    int SNDigitSum;

    //private List<int> flippedSwitches = new List<int>();

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


        //Adds blue to the beginning of the list, since yellow is forced in the first position
        colorOrder.Insert(0, 3);
        Debug.LogFormat("Color Order: " + colorOrder.Join(","));
        Debug.LogFormat("[Blue Levers #{0}] Lever colors are {1}", ModuleId, colorString());
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
        //Grab Safe Lever candidates
        TableOne(colorOrder[2], colorOrder[1]);
        TableOne(colorOrder[4], colorOrder[3]);
        uniqueCheck();
        TableOne(colorOrder[5], colorOrder[6]);
        uniqueCheck2();

        leverA = safeLevers[0];
        leverB = safeLevers[1];
        leverC = safeLevers[2];

        Debug.LogFormat("[Blue Levers #{0}] Safe lever candidates are {1}.", ModuleId, safeLevers.Select(x => x + 1).Join(", "));
        a = leverA;
        b = leverB;
        c = leverC;
        string indicators = Bomb.GetIndicators().Join("");
        today = DateTime.Today;
        todayDay = today.Day;
        serial = Bomb.GetSerialNumber().ToUpper();
        fourth = serial[3] - 'A' + 1;
        SNDigitSum = Bomb.GetSerialNumberNumbers().Sum();




        //Scoring time

        //Candidate A

        if (colorOrder[leverA] == 0)
        {
            scoreA += 1;
            Debug.LogFormat("[Blue Levers #{0}] Candidate A is red. Adding 1 to Candidate A's score.", ModuleId);
        }
        if (leverA == 1)
        {
            scoreA += 3;
            Debug.LogFormat("[Blue Levers #{0}] Candidate A is the second lever. Adding 3 to Candidate A's score.", ModuleId);
        }
        if (colorOrder[7] == 0)
        {
            scoreA += 5;
            Debug.LogFormat("[Blue Levers #{0}] Last lever is red. Adding 5 to Candidate A's score.", ModuleId);
        }
        if (indicators.Contains("IND"))
        {
            scoreA -= 2;
            Debug.LogFormat("[Blue Levers #{0}] IND indicator is present. Subtracting 2 from Candidate A's score.", ModuleId);
        }
        if (Bomb.IsPortPresent(Port.Serial))
        {
            scoreA -= 2;
            Debug.LogFormat("[Blue Levers #{0}] Serial port is present. Subtracting 2 from Candidate A's score.", ModuleId);
        }
        if (Bomb.GetPortPlates().Count() > 1)
        {
            scoreA += Bomb.GetBatteryCount();
            Debug.LogFormat("[Blue Levers #{0}] More than 1 port plate is present. Adding the amount of batteries [{1}] to Candidate A's score.", ModuleId, Bomb.GetBatteryCount());
        }
        if (Bomb.GetModuleNames().Any(x => x == "Red Levers"))
        {
            scoreA = (int)Math.Ceiling(scoreA / 2d);
            Debug.LogFormat("[Blue Levers #{0}] Red Levers module is present on the bomb. Dividing Candidate A's score by 2 and rounding up.", ModuleId);
        }
        if (todayDay == 1)
        {
            scoreA += 10;
            Debug.LogFormat("[Blue Levers #{0}] Bomb was generated on the 1st of the month. Adding 10 to Candidate A's score.", ModuleId);
        }
        if (colorOrder[leverC] == 3)
        {
            scoreA += 6;
            Debug.LogFormat("[Blue Levers #{0}] Candidate C's color is blue. Adding 6 to Candidate A's score.", ModuleId);
        }
        if (Bomb.GetModuleNames().Any(x => x == "Forget Me Not"))
        {
            scoreA -= SNDigitSum;
            Debug.LogFormat("[Blue Levers #{0}] A Forget Me Not is present on the bomb. Subtracting the sum of the serial number digits [{1}] from Candidate A's score.", ModuleId, SNDigitSum);
        }

        //Candidate B
        if (colorOrder[leverB] == 2)
        {
            scoreB += 2;
            Debug.LogFormat("[Blue Levers #{0}] Candidate B's lever is green. Adding 2 to Candidate B's score.", ModuleId);
        }
        if (leverB == 7)
        {
            scoreB += 2;
            Debug.LogFormat("[Blue Levers #{0}] Candidate B's position is the last lever. Adding 2 to Candidate B's score.", ModuleId);
        }
        if (colorOrder[7] == 2)
        {
            scoreB += 6;
            Debug.LogFormat("[Blue Levers #{0}] The last lever is green. Adding 6 to Candidate B's score.", ModuleId);
        }
        if (indicators.Contains("SIG"))
        {
            scoreB -= 3;
            Debug.LogFormat("[Blue Levers #{0}] SIG indicator is present. Subtracting 3 from Candidate B's score.", ModuleId);
        }
        if (Bomb.IsPortPresent(Port.DVI))
        {
            scoreB -= 1;
            Debug.LogFormat("[Blue Levers #{0}] DVI-D port is present. Subtracting 1 from Candidate B's score.", ModuleId);
        }
        if (Bomb.GetIndicators().Count() > 1)
        {
            scoreB += Bomb.GetPortPlates().Count();
            Debug.LogFormat("[Blue Levers #{0}] Indicator count is greater than 1. Adding the number of port plates [{1}] to Candidate B's score.", ModuleId, Bomb.GetPortPlates().Count());
        }
        if (Bomb.GetModuleNames().Any(x => x == "Green Levers"))
        {
            scoreB = (int)Math.Floor(scoreB / 2d);
            Debug.LogFormat("[Blue Levers #{0}] Green Levers module is present on the bomb. Dividing Candidate B's score by 2, rounding down.", ModuleId);
        }
        if (todayDay == 15)
        {
            scoreB += 11;
            Debug.LogFormat("[Blue Levers #{0}] Bomb was generated on the 15th day of the month. Adding 11 to Candidate B's score.", ModuleId);
        }
        if (colorOrder[leverA] == 1)
        {
            scoreB += 5;
            Debug.LogFormat("[Blue Levers #{0}] Candidate A's color is yellow. Adding 5 to Candidate B's score.", ModuleId);
        }
        if (Bomb.GetModuleNames().Count - Bomb.GetSolvableModuleNames().Count() >= 1) //Subtract the 4th character of the serial number
        {
            scoreB -= fourth;
            Debug.LogFormat("[Blue Levers #{0}] Needy module is present on the bomb. Subtracting the alphabetic position of the 4th character of the serial number [{1}] from Candidate B's score.", ModuleId, fourth);
        }

        //Candidate C
        if (colorOrder[leverC] != 3)
        {
            scoreC += 3;
            Debug.LogFormat("[Blue Levers #{0}] Candidate C's lever color is not blue. Adding 3 to Candidate C's score.", ModuleId);
        }
        if (leverC != 5)
        {
            scoreC += 1;
            Debug.LogFormat("[Blue Levers #{0}] Candidate C's position is not the 6th. Adding 1 to Candidate C's score.", ModuleId);
        }
        if (colorOrder[7] == 1)
        {
            scoreC += 5;
            Debug.LogFormat("[Blue Levers #{0}] The last lever's color is yellow. Adding 5 to Candidate C's score.", ModuleId);
        }
        if (indicators.Contains("FRK"))
        {
            scoreC -= 1;
            Debug.LogFormat("[Blue Levers #{0}] FRK indicator is present. Subtracting 1 from Candidate C's score.", ModuleId);
        }
        if (Bomb.IsPortPresent(Port.PS2))
        {
            scoreC -= 3;
            Debug.LogFormat("[Blue Levers #{0}] PS/2 port is present. Subtracting 3 from Candidate C's score.", ModuleId);
        }
        if (Bomb.GetBatteryCount() > 1)
        {
            scoreC += Bomb.GetIndicators().Count();
            Debug.LogFormat("[Blue Levers #{0}] More than 1 battery is on the bomb. Adding the number of indicators [{1}] to Candidate C's score.", ModuleId, Bomb.GetIndicators().Count());
        }
        if (Bomb.GetModuleNames().Any(x => x == "Yellow Levers"))//Round Normally
        {
            scoreC = Mathf.RoundToInt(scoreC / 2f);
            Debug.LogFormat("[Blue Levers #{0}] Yellow Levers module is present on the bomb. Dividing Candidate C's score by 2, rounding normally.", ModuleId);
        }
        if (todayDay == 29)
        {
            scoreC += 10;
            Debug.LogFormat("[Blue Levers #{0}] Bomb was generated on the 29th day of the month. Adding 10 to Candidate C's score.", ModuleId);
        }
        if (colorOrder[leverB] == 0)
        {
            scoreC += 7;
            Debug.LogFormat("[Blue Levers #{0}] Candidate B's color is red. Adding 7 to Candidate C's score.", ModuleId);
        }
        if (Bomb.GetModuleIDs().ToArray().Intersect(new[] { "WireSequence", "Wires", "WhosOnFirst", "NeedyVentGas", "Simon", "Password", "Morse", "Memory", "Maze", "NeedyKnob", "Keypad", "Venn", "NeedyCapacitor", "BigButton" }).Any()) //figure out vanilla check
        {
            scoreC -= Bomb.GetModuleIDs().ToArray().Intersect(new[] { "WireSequence", "Wires", "WhosOnFirst", "NeedyVentGas", "Simon", "Password", "Morse", "Memory", "Maze", "NeedyKnob", "Keypad", "Venn", "NeedyCapacitor", "BigButton" }).Count();
            Debug.LogFormat("[Blue Levers #{0}] At least 1 vanilla module is present on the bomb. Subtracting the total amount of vanilla modules on the bomb [{1}] from Candidate C's score.", ModuleId, Bomb.GetModuleIDs().ToArray().Intersect(new[] { "WireSequence", "Wires", "WhosOnFirst", "NeedyVentGas", "Simon", "Password", "Morse", "Memory", "Maze", "NeedyKnob", "Keypad", "Venn", "NeedyCapacitor", "BigButton" }).Count());
        }

        //Checks for first removal

        scores[0] = scoreA;
        scores[1] = scoreB;
        scores[2] = scoreC;

        Debug.LogFormat("[Blue Levers #{0}] Final score for A: {1}", ModuleId, scores[0]);
        Debug.LogFormat("[Blue Levers #{0}] Final score for B: {1}", ModuleId, scores[1]);
        Debug.LogFormat("[Blue Levers #{0}] Final score for C: {1}", ModuleId, scores[2]);

        if (scoreA != scoreB || scoreA != scoreC || scoreB != scoreC)
        {
            if (scoreA < scoreB && scoreA < scoreC)
            {
                safeLevers.RemoveAt(0);
                Debug.LogFormat("[Blue Levers #{0}] Removing candidate A from consideration.", ModuleId);
            }
            else if (scoreB < scoreA && scoreB < scoreC)
            {
                safeLevers.RemoveAt(1);
                Debug.LogFormat("[Blue Levers #{0}] Removing candidate B from consideration.", ModuleId);
            }
            else
            {
                safeLevers.RemoveAt(2);
                Debug.LogFormat("[Blue Levers #{0}] Removing candidate C from consideration.", ModuleId);
            }
        }
        else if (scoreA == scoreB)
        {
            safeLevers.RemoveAt(2);
            Debug.LogFormat("[Blue Levers #{0}] Removing candidate C from consideration.", ModuleId);
        }
        else if (scoreA == scoreC)
        {
            safeLevers.RemoveAt(1);
            Debug.LogFormat("[Blue Levers #{0}] Removing candidate B from consideration.", ModuleId);
        }
        else if (scoreB == scoreC)
        {
            safeLevers.RemoveAt(0);
            Debug.LogFormat("[Blue Levers #{0}] Removing candidate A from consideration.", ModuleId);
        }
        else
        {
            int lastDigit = Bomb.GetSerialNumber().Last();
            safeLevers.RemoveAt(lastDigit % 3);
            Debug.LogFormat("[Blue Levers #{0}] Removing candidate {1} from consideration.", ModuleId, characterString(lastDigit % 3));
        }

        Debug.LogFormat("[Blue Levers #{0}] Current safe lever candidates are {1}.", ModuleId, safeLevers.Select(x => x + 1).Join(" and "));

        //Final step
        if (safeLevers[0] < safeLevers[1])
        {
            leftmost = safeLevers[0];
            rightmost = safeLevers[1];
        }
        else
        {
            leftmost = safeLevers[1];
            rightmost = safeLevers[0];
        }

        TableTwo(colorOrder[rightmost], colorOrder[leftmost]);
        Debug.LogFormat("[Blue Levers #{0}] Using row {1} for the final table", ModuleId, rowString(rowNumber));
        TableThree(rowNumber, colorOrder[7]);

        Debug.LogFormat("[Blue Levers #{0}] The correct lever to flip is: {1}", ModuleId, answer + 1);
    }





    void TableOne(int i, int j)
    {
        //i is rows, j is columns

        int[][] table = new int[4][]{
            new int[] { 1, 4, 3, 6 },
            new int[] { 2, 7, 0, 4 },
            new int[] { 3, 6, 5, 1 },
            new int[] { 0, 5, 2, 7 },
        };
        safeLevers.Add(table[i][j]);
    }

    void uniqueCheck()
    {
        if (safeLevers[1] == safeLevers[0])
        {

            Debug.LogFormat("[Blue Levers #{0}] Special condition triggered. Shifting right for second safe lever candidate.", ModuleId);

            int candidate = safeLevers[1];

            // Keep shifting right until an unused safe lever is found
            while (safeLevers.Contains(candidate))
            {
                candidate = (candidate + 1) % 8;
            }
            
            safeLevers[1] = candidate;
            Debug.LogFormat("[Blue Levers #{0}] New second safe lever candidate after special adjustment: {1}", ModuleId, candidate + 1);
        }
    }

    void uniqueCheck2()
    {
        if (safeLevers[2] == safeLevers[0] || safeLevers[2] == safeLevers[1])
        {

            Debug.LogFormat("[Blue Levers #{0}] Special condition triggered. Shifting right for third safe lever candidate.", ModuleId);

            int candidate = safeLevers[2];

            // Keep shifting right until an unused safe lever is found
            while (safeLevers.Contains(candidate))
            {
                candidate = (candidate + 1) % 8;
            }
            safeLevers[2] = candidate;
            Debug.LogFormat("[Blue Levers #{0}] New third safe lever candidate after special adjustment: {1}", ModuleId, candidate + 1);
        } 
    }


    //Determines row used in final table

    void TableTwo(int i, int j)
    {
        int[][] table = new int[4][]{
            new int[] { 4, 3, 7, 6 },
            new int[] { 7, 6, 3, 0 },
            new int[] { 2, 1, 5, 4 },
            new int[] { 0, 5, 1, 2 },
        };
        rowNumber = table[i][j];
    }

    void TableThree(int i, int j)
    {
        int[][] primaryTable = new int[][]{
            new int[] { a, b, c, b },
            new int[] { c, b, a, c },
            new int[] { b, c, b, a },
            new int[] { b, a, c, c },
            new int[] { c, b, a, a },
            new int[] { c, b, b, c },
            new int[] { a, c, c, b },
            new int[] { a, c, b, a },
        };

        answer = primaryTable[i][j];



        int[][] secondaryTable = new int[][]{
            new int[] { b, c, a, a },
            new int[] { b, a, c, a },
            new int[] { a, a, a, c },
            new int[] { c, b, a, b },
            new int[] { b, c, c, b },
            new int[] { a, a, c, a },
            new int[] { b, b, a, c },
            new int[] { c, a, a, b },
        };

        if (!safeLevers.Contains(answer))
        {
            answer = secondaryTable[i][j];
        }
    }

    //Logging purposes

    string colorString()
    {
        string output = "";
        for (int i = 0; i < 8; i++)
        {
            output += "RYGB"[colorOrder[i]];
        }
        return output;
    }

    string characterString(int i)
    {
        return "ABC"[i].ToString();
    }

    string rowString(int i)
    {
        return "ABCDEFGH"[i].ToString();
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
            if (i == answer)
            {
                animators[i].SetBool("Flip", true);
                Audio.PlaySoundAtTransform("Sound" + Rnd.Range(1, 6), transform);
                Module.HandlePass();
                ModuleSolved = true;
                Debug.LogFormat("[Blue Levers #{0}] Correctly flipped lever #{1}. Module Solved!", ModuleId, answer);
            }
            else //You fucked up lol
            {
                Module.HandleStrike();
                Debug.LogFormat("[Blue Levers #{0}] Pressed lever #{1}. You weren't supposed to flip that! Strike!", ModuleId, i + 1);
            }
            return false;
        };
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} flip 1 2 3 4 5 6 7 8 [Flip said levers, numbered 1-8 from left to right.]";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        var m = Regex.Match(command, @"^\s*(?:(flip|press|switch|toggle)\s+)?(?<d>\d)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!m.Success)
            yield break;

        int num = int.Parse(m.Groups["d"].Value);
        if (num < 1 || num > 8)
            yield break;
        yield return null;
        Buttons[num - 1].OnInteract();
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
        Buttons[answer].OnInteract();
        yield break;
    }
}
