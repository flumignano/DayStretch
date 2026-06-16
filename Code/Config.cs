using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
namespace DayStretch
{
    public class DayStretch : ModSettings
    {
        public float FakeTimeMultiplier = 1f;
        public float TimeMultiplier = 1f;
        public float FakeWorkMultiplier = 1f;
        public float WorkMultiplier = 1f;
        public bool ShouldWorkFollow = true;
        public bool StopShowing = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref TimeMultiplier, "TimeMultiplier", 1f);
            Scribe_Values.Look(ref FakeTimeMultiplier, "FakeTimeMultiplier", 1f); // for the apply button
            Scribe_Values.Look(ref WorkMultiplier, "WorkRelated", 1f);
            Scribe_Values.Look(ref FakeWorkMultiplier, "FakeWorkRelated", 1f); // for the apply button
            Scribe_Values.Look(ref ShouldWorkFollow, "ShouldWorkFollow", true);
            Scribe_Values.Look(ref StopShowing, "StopShowing", false);
            base.ExposeData();
        }
    }
    public class Settings : Mod
    {
        DayStretch settings;
        public static DayStretch Instance;
        public Settings(ModContentPack content) : base(content)
        {
            settings = GetSettings<DayStretch>();
            Instance = settings;
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("Day length multiplier: " + settings.FakeTimeMultiplier.ToString("0.0"));
            settings.FakeTimeMultiplier = listingStandard.Slider(settings.FakeTimeMultiplier, 0.1f, 20f);
            settings.FakeTimeMultiplier = (float)Math.Round(settings.FakeTimeMultiplier, 1);
            listingStandard.Label($"Current day length multiplier: {settings.TimeMultiplier}");
            listingStandard.CheckboxLabeled("Should work use its own slider ", ref settings.ShouldWorkFollow);
            if (settings.ShouldWorkFollow)
            {
                listingStandard.Label("Work Related Multiplier: " + settings.FakeWorkMultiplier.ToString("0.0"));
                settings.FakeWorkMultiplier = listingStandard.Slider(settings.FakeWorkMultiplier, 0.1f, 20f);
                settings.FakeWorkMultiplier = (float)Math.Round(settings.FakeWorkMultiplier, 1);
                listingStandard.Label($"Current Work Related Multiplier: {settings.WorkMultiplier}"); // hello everybody my name is Multiplier
            }
            else
            {
                settings.FakeWorkMultiplier = settings.TimeMultiplier;
                settings.WorkMultiplier = settings.TimeMultiplier;
            }
            if (listingStandard.ButtonText("Apply"))
            {
                ApplySettings();
            }
            listingStandard.CheckboxLabeled("Hide save mismatch warning popups ", ref settings.StopShowing);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        private void ApplySettings()
        {
            settings.TimeMultiplier = settings.FakeTimeMultiplier;
            if (settings.ShouldWorkFollow)
            {
                settings.WorkMultiplier = settings.FakeWorkMultiplier;
            }
            else
            {
                settings.FakeWorkMultiplier = settings.TimeMultiplier;
                settings.WorkMultiplier = settings.TimeMultiplier;
            }

            WriteSettings();

            var comp = Current.Game?.GetComponent<DayStretchGameComp>();
            if (comp != null && !comp.setupCompleted)
            {
                ShowNewColonySetupPrompt(comp);
                return;
            }

            ShowAppliedRestartMessage(comp != null);
        }
        private void ShowNewColonySetupPrompt(DayStretchGameComp comp)
        {
            string text = $"DayStretch saved these settings immediately.\n{MultiplierSummary()}\n\n" +
                          $"This setup step is intended for a newly started colony before real play.\n\n" +
                          $"DayStretch will bind this save to day length multiplier {settings.TimeMultiplier:0.0}x.\n\n" +
                          $"After confirming, save this colony, restart RimWorld, then continue playing.";

            Find.WindowStack.Add(new Dialog_MessageBox(
                text,
                "Bind save", () =>
                {
                    comp.CompleteSetup(settings.TimeMultiplier);
                    Find.WindowStack.Add(new Dialog_MessageBox("DayStretch setup is bound to this save. Save this colony now, restart RimWorld, then continue."));
                },
                "Not now", () =>
                {
                    Find.WindowStack.Add(new Dialog_MessageBox("Settings were saved globally, but this save was not bound to the new multiplier. Save mismatch warnings will remain available when this colony is loaded."));
                }
            ));
        }
        private void ShowAppliedRestartMessage(bool hasCurrentGame)
        {
            string text = $"DayStretch saved these settings immediately.\n{MultiplierSummary()}\n\n" +
                          $"Restart RimWorld for the settings to load.";

            if (hasCurrentGame)
            {
                text += "\n\nThis does not convert existing colonies. If this save was already played with another multiplier, DayStretch will keep the mismatch warning available on load.";
            }

            Find.WindowStack.Add(new Dialog_MessageBox(text));
        }
        private string MultiplierSummary()
        {
            if (settings.ShouldWorkFollow)
            {
                return $"New multipliers: day length {settings.TimeMultiplier:0.0}x, work {settings.WorkMultiplier:0.0}x.";
            }

            return $"New multiplier: day length and work {settings.TimeMultiplier:0.0}x.";
        }
        public override string SettingsCategory()
        {
            return "DayStretch";
        }
    }
}


