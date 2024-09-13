using UnityEngine;
using UnityEngine.UI;
using Winch.Core;
using System.Collections;
using Winch.Util;
using TMPro;
using System;
using Winch.Core.API;
using Yarn.Unity;

namespace DeepSubmergence {
    public static class SeaBaseDock {

        private static readonly string[] quest0requiredfish = {
            "deepsubmergence.fishboltfish",
            "deepsubmergence.fishnetsquid",
            "deepsubmergence.fishshoal",
        };
        private static readonly string[] quest1requiredfish = {
            "deepsubmergence.fishelectriceel",
            "deepsubmergence.fishironlungfish",
            "deepsubmergence.fishlampcrab",
            "deepsubmergence.fishneedlefish",
        };
        private static readonly string[] quest2requiredfish = {
            "deepsubmergence.fishsteelhead",
            "deepsubmergence.fishtorpedofish",
            "deepsubmergence.fishshreddershark",
        };
        private static readonly string[] quest3requiredfish = {
            "deepsubmergence.fishtrenchwhale",
        };
        private static readonly string[] diverSprites = {
            "deepsubmergence.uidiver0",
            "deepsubmergence.uidiver1",
            "deepsubmergence.uidiver2",
            "deepsubmergence.uidiver3",
        };
        private static readonly string[] quest0Dialogues = {
            "deepsubmergence.quest0dialogue0",
            "deepsubmergence.quest0dialogue1",
            "deepsubmergence.quest0dialogue2",
            "deepsubmergence.quest0dialogue3",
            "deepsubmergence.quest0dialogue4",
            "deepsubmergence.quest0dialogue5",
        };
        private static readonly string[] quest1Dialogues = {
            "deepsubmergence.quest1dialogue0",
            "deepsubmergence.quest1dialogue1",
            "deepsubmergence.quest1dialogue2",
            "deepsubmergence.quest1dialogue3",
            "deepsubmergence.quest1dialogue4",
            "deepsubmergence.quest1dialogue5",
        };
        private static readonly string[] quest2Dialogues = {
            "deepsubmergence.quest2dialogue0",
            "deepsubmergence.quest2dialogue1",
            "deepsubmergence.quest2dialogue2",
            "deepsubmergence.quest2dialogue3",
            "deepsubmergence.quest2dialogue4",
            "deepsubmergence.quest2dialogue5",
        };
        private static readonly string[] quest3Dialogues = {
            "deepsubmergence.quest3dialogue0",
            "deepsubmergence.quest3dialogue1",
            "deepsubmergence.quest3dialogue2",
            "deepsubmergence.quest3dialogue3",
            "deepsubmergence.quest3dialogue4",
            "deepsubmergence.quest3dialogue5",
        };
        private static readonly string[] questDoneDialogues = {
            "deepsubmergence.questdonedialogue0",
            "deepsubmergence.questdonedialogue1",
        };
        
        private const string PROGRESSION_SAVE_KEY = "deepsubmergence.questprogress";

        public static void Initialize()
        {
            DredgeEvent.OnDialogueRunnerLoaded += OnDialogueRunnerLoaded;
        }

        private static void OnDialogueRunnerLoaded(DredgeDialogueRunner dialogueRunner)
        {
            dialogueRunner.AddFunction("GetDeepSubmergenceProgress", GetDeepSubmergenceProgress);
            dialogueRunner.AddCommandHandler("IncrementDeepSubmergenceProgress", IncrementDeepSubmergenceProgress);
            dialogueRunner.AddCommandHandler("UpdateDeepSubmergenceProgress", UpdateDeepSubmergenceProgress);
        }

        private static int GetDeepSubmergenceProgress() => GameManager.Instance.DialogueRunner.GetIntVariable(PROGRESSION_SAVE_KEY);

        private static void IncrementDeepSubmergenceProgress() => GameManager.Instance.DialogueRunner.AdjustIntVariable(PROGRESSION_SAVE_KEY, 1);

        private static void UpdateDeepSubmergenceProgress()
        {
			// Check for progression
			string[] requiredFish = GetRequiredFishListForProgressLevel(GetDeepSubmergenceProgress());

			if (requiredFish != null)
			{
				bool hasAllRequiredFish = true;
				for (int i = 0, count = requiredFish.Length; i < count; ++i)
				{
					hasAllRequiredFish &= Utils.HasItemInCargo(requiredFish[i]);
				}

				if (hasAllRequiredFish)
				{
                    IncrementDeepSubmergenceProgress();

					// Take the fish
					for (int i = 0, count = requiredFish.Length; i < count; ++i)
					{
						Utils.DestroyItemInCargo(requiredFish[i]);
					}
				}
			}
		}

        private static string[] GetRequiredFishListForProgressLevel(int progress){
            if(progress == 0){ return quest0requiredfish; }
            else if(progress == 1){ return quest1requiredfish; }
            else if(progress == 2){ return quest2requiredfish; }
            else if(progress == 3){ return quest3requiredfish; }
            return null;
        }
    }
}