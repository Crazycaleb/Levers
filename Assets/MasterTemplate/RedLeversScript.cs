using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class RedLeversScript : MonoBehaviour {

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

   string serialNumber;
   int secondNumber;

   List<int> flipOrder = new List<int>();
   List<int> colorOrder = new List<int>();

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

      //Adds red to the beginning of the list, since red is forced in the first position
      colorOrder.Insert(0, 0);

      for (int i = 0; i < 8; i++)
      {
         Buttons[i].OnInteract += ButtonsPressed(i);
      }
      /*
      foreach (KMSelectable object in keypad) {
          object.OnInteract += delegate () { keypadPress(object); return false; };
      }
      */

      //button.OnInteract += delegate () { buttonPress(); return false; };
      

   }

   void Start()
   {
      ModuleActivated = true;
      StageOne();
   }

   //First Lever Flip

   void StageOne()
   {



      //Red

      if (colorOrder[7] == 0)
      {
         if (Bomb.IsIndicatorOff("SND"))
         {
            flipOrder.Add(0);
         }
         else if (Bomb.IsIndicatorOn("CAR"))
         {
            flipOrder.Add(2);
         }
         else if (Bomb.IsIndicatorOff("IND"))
         {
            flipOrder.Add(1);
         }
         else if (Bomb.IsIndicatorOn("FRQ"))
         {
            flipOrder.Add(4);
         }
         else if (Bomb.IsIndicatorOff("FRK"))
         {
            flipOrder.Add(6);
         }
         else if (Bomb.IsIndicatorOn("NSA"))
         {
            flipOrder.Add(3);
         }
         else if (Bomb.IsIndicatorOff("BOB"))
         {
            flipOrder.Add(5);
         }
         else if (Bomb.IsIndicatorOn("TRN"))
         {
            flipOrder.Add(7);
         }
         else if (Bomb.IsIndicatorOff("SIG"))
         {
            flipOrder.Add(1);
         }
         else if (Bomb.IsIndicatorOn("CLR"))
         {
            flipOrder.Add(6);
         }
         else if (Bomb.IsIndicatorOff("MSA"))
         {
            flipOrder.Add(0);
         }
         else if (Bomb.IsIndicatorOn("BOB"))
         {
            flipOrder.Add(7);
         }
         else
         {
            flipOrder.Add(0);
         }
      }


      //Yellow


      else if (colorOrder[7] == 1)
      {
         if (Bomb.IsIndicatorOff("SND"))
         {
            flipOrder.Add(5);
         }
         else if (Bomb.IsIndicatorOn("CAR"))
         {
            flipOrder.Add(7);
         }
         else if (Bomb.IsIndicatorOff("IND"))
         {
            flipOrder.Add(3);
         }
         else if (Bomb.IsIndicatorOn("FRQ"))
         {
            flipOrder.Add(0);
         }
         else if (Bomb.IsIndicatorOff("FRK"))
         {
            flipOrder.Add(4);
         }
         else if (Bomb.IsIndicatorOn("NSA"))
         {
            flipOrder.Add(2);
         }
         else if (Bomb.IsIndicatorOff("BOB"))
         {
            flipOrder.Add(1);
         }
         else if (Bomb.IsIndicatorOn("TRN"))
         {
            flipOrder.Add(6);
         }
         else if (Bomb.IsIndicatorOff("SIG"))
         {
            flipOrder.Add(4);
         }
         else if (Bomb.IsIndicatorOn("CLR"))
         {
            flipOrder.Add(5);
         }
         else if (Bomb.IsIndicatorOff("MSA"))
         {
            flipOrder.Add(2);
         }
         else if (Bomb.IsIndicatorOn("BOB"))
         {
            flipOrder.Add(3);
         }
         else
         {
            flipOrder.Add(1);
         }
      }


      //Green


      else if (colorOrder[7] == 2)
      {
         if (Bomb.IsIndicatorOff("SND"))
         {
            flipOrder.Add(3);
         }
         else if (Bomb.IsIndicatorOn("CAR"))
         {
            flipOrder.Add(6);
         }
         else if (Bomb.IsIndicatorOff("IND"))
         {
            flipOrder.Add(7);
         }
         else if (Bomb.IsIndicatorOn("FRQ"))
         {
            flipOrder.Add(2);
         }
         else if (Bomb.IsIndicatorOff("FRK"))
         {
            flipOrder.Add(1);
         }
         else if (Bomb.IsIndicatorOn("NSA"))
         {
            flipOrder.Add(5);
         }
         else if (Bomb.IsIndicatorOff("BOB"))
         {
            flipOrder.Add(0);
         }
         else if (Bomb.IsIndicatorOn("TRN"))
         {
            flipOrder.Add(4);
         }
         else if (Bomb.IsIndicatorOff("SIG"))
         {
            flipOrder.Add(3);
         }
         else if (Bomb.IsIndicatorOn("CLR"))
         {
            flipOrder.Add(7);
         }
         else if (Bomb.IsIndicatorOff("MSA"))
         {
            flipOrder.Add(5);
         }
         else if (Bomb.IsIndicatorOn("BOB"))
         {
            flipOrder.Add(4);
         }
         else
         {
            flipOrder.Add(3);
         }
      }


      //Blue


      else
      {
         if (Bomb.IsIndicatorOff("SND"))
         {
            flipOrder.Add(1);
         }
         else if (Bomb.IsIndicatorOn("CAR"))
         {
            flipOrder.Add(4);
         }
         else if (Bomb.IsIndicatorOff("IND"))
         {
            flipOrder.Add(5);
         }
         else if (Bomb.IsIndicatorOn("FRQ"))
         {
            flipOrder.Add(6);
         }
         else if (Bomb.IsIndicatorOff("FRK"))
         {
            flipOrder.Add(0);
         }
         else if (Bomb.IsIndicatorOn("NSA"))
         {
            flipOrder.Add(7);
         }
         else if (Bomb.IsIndicatorOff("BOB"))
         {
            flipOrder.Add(3);
         }
         else if (Bomb.IsIndicatorOn("TRN"))
         {
            flipOrder.Add(2);
         }
         else if (Bomb.IsIndicatorOff("SIG"))
         {
            flipOrder.Add(0);
         }
         else if (Bomb.IsIndicatorOn("CLR"))
         {
            flipOrder.Add(1);
         }
         else if (Bomb.IsIndicatorOff("MSA"))
         {
            flipOrder.Add(3);
         }
         else if (Bomb.IsIndicatorOn("BOB"))
         {
            flipOrder.Add(6);
         }
         else
         {
            flipOrder.Add(2);
         }
      }
      Debug.LogFormat("[Red Levers #{0}] Lever to flip for stage 1 is {1}", ModuleId, flipOrder[0] + 1);
      StageTwo();
   }

   void StageTwo()
   {

      //Red 

      if (colorOrder[flipOrder[0]] == 0)
      {
         if (flipOrder[0] == 0)
         {
            flipOrder.Add(1);
         }
         else if (flipOrder[0] == 1)
         {
            flipOrder.Add(4);
         }
         else if (flipOrder[0] == 2)
         {
            flipOrder.Add(3);
         }
         else if (flipOrder[0] == 3)
         {
            flipOrder.Add(5);
         }
         else if (flipOrder[0] == 4)
         {
            flipOrder.Add(3);
         }
         else if (flipOrder[0] == 5)
         {
            flipOrder.Add(1);
         }
         else if (flipOrder[0] == 6)
         {
            flipOrder.Add(7);
         }
         else
         {
            flipOrder.Add(6);
         }
      }



      //Yellow



      else if (colorOrder[flipOrder[0]] == 1)
      {
         if (flipOrder[0] == 0)
         {

         }
         else if (flipOrder[0] == 1)
         {
            flipOrder.Add(0);
         }
         else if (flipOrder[0] == 2)
         {
            flipOrder.Add(6);
         }
         else if (flipOrder[0] == 3)
         {
            flipOrder.Add(0);
         }
         else if (flipOrder[0] == 4)
         {
            flipOrder.Add(7);
         }
         else if (flipOrder[0] == 5)
         {
            flipOrder.Add(0);
         }
         else if (flipOrder[0] == 6)
         {
            flipOrder.Add(2);
         }
         else
         {
            flipOrder.Add(0);
         }
      }


      //Green


      else if (colorOrder[flipOrder[0]] == 2)
      {
         if (flipOrder[0] == 0)
         {

         }
         else if (flipOrder[0] == 1)
         {
            flipOrder.Add(5);
         }
         else if (flipOrder[0] == 2)
         {
            flipOrder.Add(4);
         }
         else if (flipOrder[0] == 3)
         {
            flipOrder.Add(1);
         }
         else if (flipOrder[0] == 4)
         {
            flipOrder.Add(2);
         }
         else if (flipOrder[0] == 5)
         {
            flipOrder.Add(3);
         }
         else if (flipOrder[0] == 6)
         {
            flipOrder.Add(4);
         }
         else
         {
            flipOrder.Add(2);
         }
      }


      //Blue


      else
      {
         if (flipOrder[0] == 0)
         {

         }
         else if (flipOrder[0] == 1)
         {
            flipOrder.Add(6);
         }
         else if (flipOrder[0] == 2)
         {
            flipOrder.Add(7);
         }
         else if (flipOrder[0] == 3)
         {
            flipOrder.Add(5);
         }
         else if (flipOrder[0] == 4)
         {
            flipOrder.Add(3);
         }
         else if (flipOrder[0] == 5)
         {
            flipOrder.Add(7);
         }
         else if (flipOrder[0] == 6)
         {
            flipOrder.Add(1);
         }
         else
         {
            flipOrder.Add(5);
         }
      }

      Debug.LogFormat("[Red Levers #{0}] Lever to flip for stage 2 is {1}", ModuleId, flipOrder[1] + 1);
      StageThree();
   }

   void StageThree()
   {
      //First Lever Red
      if (colorOrder[flipOrder[0]] == 0)
      {
         if (colorOrder[flipOrder[1]] == 0)
         {
            flipOrder.Add(7);
         }
         else if (colorOrder[flipOrder[1]] == 1)
         {
            flipOrder.Add(0);
         }
         else if (colorOrder[flipOrder[1]] == 2)
         {
            flipOrder.Add(2);
         }
         else
         {
            flipOrder.Add(4);
         }
      }

      //First Lever Yellow

      else if (colorOrder[flipOrder[0]] == 1)
      {
         if (colorOrder[flipOrder[1]] == 0)
         {
            flipOrder.Add(6);
         }
         else if (colorOrder[flipOrder[1]] == 1)
         {
            flipOrder.Add(1);
         }
         else if (colorOrder[flipOrder[1]] == 2)
         {
            flipOrder.Add(4);
         }
         else
         {
            flipOrder.Add(5);
         }
      }

      //First Lever Green

      else if (colorOrder[flipOrder[0]] == 2)
      {
         if (colorOrder[flipOrder[1]] == 0)
         {
            flipOrder.Add(0);
         }
         else if (colorOrder[flipOrder[1]] == 1)
         {
            flipOrder.Add(3);
         }
         else if (colorOrder[flipOrder[1]] == 2)
         {
            flipOrder.Add(6);
         }
         else
         {
            flipOrder.Add(2);
         }
      }

      //First Lever Blue

      else
      {
         if (colorOrder[flipOrder[1]] == 0)
         {
            flipOrder.Add(3);
         }
         else if (colorOrder[flipOrder[1]] == 1)
         {
            flipOrder.Add(5);
         }
         else if (colorOrder[flipOrder[1]] == 2)
         {
            flipOrder.Add(7);
         }
         else
         {
            flipOrder.Add(1);
         }
      }

      if (flipOrder[2] == flipOrder[0] || flipOrder[2] == flipOrder[1])
      {
         serialNumber = Bomb.GetSerialNumber();
         secondNumber = int.Parse(serialNumber.Where(char.IsDigit).ElementAt(1).ToString());

         Debug.LogFormat("[Red Levers #{0}] Special condition triggered. Serial: {1}, Second digit: {2}", ModuleId, serialNumber, secondNumber);

         int candidate = flipOrder[2];
         int steps = secondNumber;

         for (int i = 0; i < steps; i++)
         {
            candidate = (candidate + 7) % 8; // Move one to the left with wraparound
         }

         while (flipOrder.Contains(candidate))
         {
            candidate = (candidate + 7) % 8; // Shift left again until unflipped
         }

         flipOrder[2] = candidate;

         Debug.LogFormat("[Red Levers #{0}] New third flip due to special condition: {1}", ModuleId, candidate + 1);

         
      }
      Debug.LogFormat("[Red Levers #{0}] Lever to flip for stage 3 is {1}", ModuleId, flipOrder[2] + 1);
      StageFour();
   }

   void StageFour()
   {
      //Red 

      if (colorOrder[flipOrder[2]] == 0)
      {
         if (flipOrder[2] == 0)
         {
            flipOrder.Add(7);
         }
         else if (flipOrder[2] == 1)
         {
            flipOrder.Add(0);
         }
         else if (flipOrder[2] == 2)
         {
            flipOrder.Add(1);
         }
         else if (flipOrder[2] == 3)
         {
            flipOrder.Add(4);
         }
         else if (flipOrder[2] == 4)
         {
            flipOrder.Add(2);
         }
         else if (flipOrder[2] == 5)
         {
            flipOrder.Add(3);
         }
         else if (flipOrder[2] == 6)
         {
            flipOrder.Add(5);
         }
         else
         {
            flipOrder.Add(6);
         }
      }



      //Yellow



      else if (colorOrder[flipOrder[2]] == 1)
      {
         if (flipOrder[2] == 0)
         {

         }
         else if (flipOrder[2] == 1)
         {
            flipOrder.Add(2);
         }
         else if (flipOrder[2] == 2)
         {
            flipOrder.Add(3);
         }
         else if (flipOrder[2] == 3)
         {
            flipOrder.Add(6);
         }
         else if (flipOrder[2] == 4)
         {
            flipOrder.Add(5);
         }
         else if (flipOrder[2] == 5)
         {
            flipOrder.Add(7);
         }
         else if (flipOrder[2] == 6)
         {
            flipOrder.Add(0);
         }
         else
         {
            flipOrder.Add(1);
         }
      }


      //Green


      else if (colorOrder[flipOrder[2]] == 2)
      {
         if (flipOrder[2] == 0)
         {

         }
         else if (flipOrder[2] == 1)
         {
            flipOrder.Add(4);
         }
         else if (flipOrder[2] == 2)
         {
            flipOrder.Add(5);
         }
         else if (flipOrder[2] == 3)
         {
            flipOrder.Add(0);
         }
         else if (flipOrder[2] == 4)
         {
            flipOrder.Add(3);
         }
         else if (flipOrder[2] == 5)
         {
            flipOrder.Add(0);
         }
         else if (flipOrder[2] == 6)
         {
            flipOrder.Add(1);
         }
         else
         {
            flipOrder.Add(4);
         }
      }


      //Blue


      else
      {
         if (flipOrder[2] == 0)
         {

         }
         else if (flipOrder[2] == 1)
         {
            flipOrder.Add(6);
         }
         else if (flipOrder[2] == 2)
         {
            flipOrder.Add(7);
         }
         else if (flipOrder[2] == 3)
         {
            flipOrder.Add(2);
         }
         else if (flipOrder[2] == 4)
         {
            flipOrder.Add(7);
         }
         else if (flipOrder[2] == 5)
         {
            flipOrder.Add(2);
         }
         else if (flipOrder[2] == 6)
         {
            flipOrder.Add(3);
         }
         else
         {
            flipOrder.Add(5);
         }
      }

      if (flipOrder[3] == flipOrder[0] || flipOrder[3] == flipOrder[1] || flipOrder[3] == flipOrder[2])
      {
         char firstLetter = Bomb.GetSerialNumberLetters().First();

         int shiftAmount = char.ToUpper(firstLetter) - 'A' + 1;

         Debug.LogFormat("[Red Levers #{0}] Special condition triggered. First serial letter: {1}, A1Z26 value: {2}", ModuleId, firstLetter, shiftAmount);

         int candidate = flipOrder[3];

         // Move right by shiftAmount with wraparound
         for (int i = 0; i < shiftAmount; i++)
         {
            candidate = (candidate + 1) % 8;
         }

         // Keep shifting right until an unflipped lever is found
         while (flipOrder.Contains(candidate))
         {
            candidate = (candidate + 1) % 8;
         }

         flipOrder[3] = candidate;
         Debug.LogFormat("[Red Levers #{0}] New fourth flip after special adjustment: {1}", ModuleId, candidate + 1);
      }
      Debug.LogFormat("[Red Levers #{0}] Lever to flip for stage 4 is {1}", ModuleId, flipOrder[3] + 1);

      FinalStage();
   }



   void FinalStage()
   {
    List<int> unflipped = Enumerable.Range(0, 8).Where(i => !flipOrder.Contains(i)).ToList();
    Debug.LogFormat("[Red Levers #{0}] Unflipped levers before final stage: {1}", ModuleId, string.Join(", ", unflipped.ConvertAll(i => (i + 1).ToString()).ToArray()));

    // Case 1: Lit BOB and >4 batteries
    if (Bomb.IsIndicatorOn("BOB") && Bomb.GetBatteryCount() > 4)
    {
        Debug.LogFormat("[Red Levers #{0}] Lit BOB + >4 batteries: Flipping remaining left to right", ModuleId);
        flipOrder.AddRange(unflipped.OrderBy(i => i));
    }

    // Case 2: First lever (index 0) not flipped
    else if (!flipOrder.Contains(0))
    {
        Debug.LogFormat("[Red Levers #{0}] First lever not flipped yet: Flipping it first, then rest right to left", ModuleId);
        flipOrder.Add(0);
        unflipped.Remove(0);
        flipOrder.AddRange(unflipped.OrderByDescending(i => i));
    }

    // Case 3: All flipped levers so far have unique colors
    else if (flipOrder.Select(i => colorOrder[i]).Distinct().Count() == flipOrder.Count)
    {
        var reversedUnflipped = unflipped.OrderByDescending(i => i).ToList();
        if (reversedUnflipped.Count > 1)
        {
            Debug.LogFormat("[Red Levers #{0}] All flipped colors unique: Flipping second unflipped from right, then left to right", ModuleId);
            flipOrder.Add(reversedUnflipped[1]);
            reversedUnflipped.RemoveAt(1);
        }
        else if (reversedUnflipped.Count == 1)
        {
            Debug.LogFormat("[Red Levers #{0}] Only one unflipped lever: Flipping it", ModuleId);
            flipOrder.Add(reversedUnflipped[0]);
            reversedUnflipped.Clear();
        }
        flipOrder.AddRange(reversedUnflipped.OrderBy(i => i));
    }

    // Case 4: No red levers flipped yet (color 0)
    else if (!flipOrder.Any(i => colorOrder[i] == 0))
    {
        Debug.LogFormat("[Red Levers #{0}] No red levers flipped: Flipping first unflipped left to right, then rest right to left", ModuleId);
        var leftToRight = unflipped.OrderBy(i => i).ToList();
        flipOrder.Add(leftToRight[0]);
        leftToRight.RemoveAt(0);
        flipOrder.AddRange(leftToRight.OrderByDescending(i => i));
    }

    // Case 5: Default – flip all remaining right to left
    else
    {
        Debug.LogFormat("[Red Levers #{0}] Default case: Flipping all remaining right to left", ModuleId);
        flipOrder.AddRange(unflipped.OrderByDescending(i => i));
    }

    Debug.LogFormat("[Red Levers #{0}] Final flip order: {1}", ModuleId, string.Join(", ", flipOrder.ConvertAll(i => (i + 1).ToString()).ToArray()));
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
           if (i == flipOrder[j])
           {
              animators[i].SetBool("Flip", true);
              Audio.PlaySoundAtTransform("Sound" + Rnd.Range(1,6), transform);
              Debug.LogFormat("[Red Levers #{0}] Pressed lever #{1}, expected #{2}.", ModuleId, i + 1, flipOrder[j] + 1);
              j++;
              if (j == 8) //All Switches flipped = handle solve
              {
                 StartCoroutine(SolveAnimation());
                 Module.HandlePass();
                 ModuleSolved = true;
                 Debug.LogFormat("[Red Levers #{0}] All levers flipped. Module Solved!", ModuleId);
              }

           }
           else //You fucked up lol
           {
              Module.HandleStrike();
              Debug.LogFormat("[Red Levers #{0}] Pressed lever #{1}, expected #{2}. Strike!", ModuleId, i + 1, flipOrder[j] + 1);
           }

            return false;
        };
   }

   IEnumerator SolveAnimation()
   {
      yield return new WaitForSeconds(0.5f);
      for (int k = 0; k < 8; k++)
      {
         yield return new WaitForSeconds(0.1f);
         animators[k].SetBool("Flip", false);
         Audio.PlaySoundAtTransform("Sound5", transform);
      }
      yield return new WaitForSeconds(0.05f);
      for (int k = 0; k < 8; k++)
      {
         yield return new WaitForSeconds(0.1f);
         animators[k].SetBool("Flip", true);
         Audio.PlaySoundAtTransform("Sound5", transform);
      }
      yield return new WaitForSeconds(0.05f);
      for (int k = 0; k < 8; k++)
      {
         yield return new WaitForSeconds(0.1f);
         animators[k].SetBool("Flip", false);
         Audio.PlaySoundAtTransform("Sound5", transform);
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
   }

   IEnumerator TwitchHandleForcedSolve () {
      yield return null;
   }
}
