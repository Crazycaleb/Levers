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
	DateTime today;
	int todayDay;
	int leverA;
	int leverB;
	int leverC;
	int scoreA = 1;
	int scoreB = 1;
	int scoreC = 1;
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
		TableOne(colorOrder[1], colorOrder[2]);
		TableOne(colorOrder[3], colorOrder[4]);
		TableOne(colorOrder[6], colorOrder[5]);

		//Shift right if needed
		uniqueCheck();

		leverA = safeLevers[0];
		leverB = safeLevers[1];
		leverC = safeLevers[2];

		string indicators = Bomb.GetIndicators();
		today = DateTime.Today;
		todayDay = today.Day;
		SNDigitsSum = Bomb.GetSerialNumberNumbers().Sum();


		//Scoring time

		//Candidate A

		if (colorOrder[leverA] == 0)
		{
			scoreA += 1;
		}
		if (leverA == 1)
		{
			scoreA += 3;
		}
		if (colorOrder[7] == 0)
		{
			scoreA += 5;
		}
		if (indicators.Contains("IND"))
		{
			scoreA -= 2;
		}
		if (Bomb.IsPortPresent(Port.Serial))
		{
			scoreA -= 2;
		}
		if (Bomb.GetPortPlates().Count() > 1)
		{
			scoreA += Bomb.GetBatteryCount();
		}
		if (Bomb.GetModuleNames().Any(x => x == "Red Levers"))
		{
			scoreA = Math.Ceiling(scoreA / 2);
		}
		if (todayDay == 1)
		{
			scoreA += 10;
		}
		if (colorOrder[leverC] == 3)
		{
			scoreA += 6;
		}
		if (Bomb.GetModuleNames().Any(x => x == "Forget Me Not"))
		{
			scoreA -= SNDigitSum;
		}

		//Candidate B
		if (colorOrder[leverB] == 2)
		{
			scoreB += 2;
		}
		if (leverB == 7)
		{
			scoreB += 2;
		}
		if (colorOrder[7] == 2)
		{
			scoreB += 6;
		}
		if (indicators.Contains("SIG"))
		{
			scoreB -= 3;
		}
		if (Bomb.IsPortPresent(Port.DVID))
		{
			scoreB -= 1;
		}
		if (Bomb.GetIndicators().Count() > 1)
		{
			scoreB += Bomb.GetPortPlates().Count();
		}
		if (Bomb.GetModuleNames().Any(x => x == "Green Levers"))
		{
			scoreB = Math.Floor(scoreB / 2);
		}
		if (todayDay == 15)
		{
			scoreB += 11;
		}
		if (colorOrder[leverA] == 1)
		{
			scoreB += 5;
		}
		if (Bomb.GetModuleNames().Any(x => x == "Forget Me Not")) //Think of a new score for this+figure out needy check
		{
			scoreB -= SNDigitSum;
		}

		//Candidate C
		if (colorOrder[leverC] != 3)
		{
			scoreC += 3;
		}
		if (leverB != 5)
		{
			scoreC += 1;
		}
		if (colorOrder[7] == 1)
		{
			scoreC += 5;
		}
		if (indicators.Contains("FRK"))
		{
			scoreC -= 1;
		}
		if (Bomb.IsPortPresent(Port.PS2))
		{
			scoreC -= 3;
		}
		if (Bomb.GetBatteryCount() > 1)
		{
			scoreC += Bomb.GetIndicators().Count();
		}
		if (Bomb.GetModuleNames().Any(x => x == "Yellow Levers"))//Round Normally
		{
			scoreC = (int)Math.Round(scoreC / 2);
		}
		if (todayDay == 29)
		{
			scoreC += 10;
		}
		if (colorOrder[leverB] == 0)
		{
			scoreC += 7;
		}
		if (Bomb.GetModuleNames().Any(x => x == "Forget Me Not")) //figure out vanilla check
		{
			scoreC -= SNDigitSum;
		}
	}

	void TableOne(int i, int j)
	{
		//i is columns, j is rows

		//Red
		if (i == 0)
		{
			if (j == 3)
			{
				safeLevers.Add(0);
			}
			else if (j == 2)
			{
				safeLevers.Add(3);
			}
			else if (j == 1)
			{
				safeLevers.Add(2);
			}
			else
			{
				safeLevers.Add(1);
			}
		}

		//Yellow
		else if (i == 1)
		{
			if (j == 3)
			{
				safeLevers.Add(5);
			}
			else if (j == 2)
			{
				safeLevers.Add(6);
			}
			else if (j == 1)
			{
				safeLevers.Add(7);
			}
			else
			{
				safeLevers.Add(4);
			}
		}

		//Green
		else if (i == 2)
		{
			if (j == 3)
			{
				safeLevers.Add(2);
			}
			else if (j == 2)
			{
				safeLevers.Add(5);
			}
			else if (j == 1)
			{
				safeLevers.Add(0);
			}
			else
			{
				safeLevers.Add(3);
			}
		}

		//Blue
		else
		{
			if (j == 3)
			{
				safeLevers.Add(7);
			}
			else if (j == 2)
			{
				safeLevers.Add(1);
			}
			else if (j == 1)
			{
				safeLevers.Add(4);
			}
			else
			{
				safeLevers.Add(6);
			}
		}
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

}
