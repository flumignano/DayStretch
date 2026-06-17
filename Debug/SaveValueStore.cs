using System;
using Verse;

namespace DayStretch
{
    public class DayStretchGameComp : GameComponent
    {
        // Fresh colonies get a short one-time window where the global setting can
        // be bound into the save. After that, the load-time mismatch warning is
        // the safer path because real gameplay may already have advanced.
        private const int NewColonySetupGraceTicks = DayConstants.VanillaTicksPerHour;

        public float savedTimeMultiplier = 1f;
        public bool setupCompleted = true;
        public int setupStartTick = -1;

        public DayStretchGameComp(Game game) : base()
        {
        }
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            savedTimeMultiplier = Settings.Instance.TimeMultiplier;
            setupCompleted = false;
            setupStartTick = Find.TickManager.TicksGame;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            // Do not persist an open setup window. If the player saves before
            // binding the multiplier, the next load should use the normal
            // mismatch warning instead of pretending the save is still pristine.
            if (Scribe.mode == LoadSaveMode.Saving && !setupCompleted)
            {
                CloseNewColonySetupWindow();
            }

            Scribe_Values.Look(ref savedTimeMultiplier, "DayStretched_SavedTimeMultiplier", 1f);
            Scribe_Values.Look(ref setupCompleted, "DayStretched_SetupCompleted", true);
            Scribe_Values.Look(ref setupStartTick, "DayStretched_SetupStartTick", -1);
        }
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            CloseNewColonySetupWindowIfExpired();
        }
        public bool CanBindNewColonySetup()
        {
            // Re-check expiry at the point of use; the settings window can stay
            // open while the colony ticks forward.
            CloseNewColonySetupWindowIfExpired();
            return !setupCompleted;
        }
        public bool CompleteSetup(float timeMultiplier)
        {
            if (!CanBindNewColonySetup())
            {
                return false;
            }

            savedTimeMultiplier = timeMultiplier;
            setupCompleted = true;
            setupStartTick = -1;
            return true;
        }
        public void CancelSetup()
        {
            CloseNewColonySetupWindow();
        }
        public static float ForCurrentSave()
        {
            var comp = Current.Game?.GetComponent<DayStretchGameComp>();
            return comp != null ? comp.savedTimeMultiplier : Settings.Instance.TimeMultiplier;
        }
        private void RestoreSavedMultiplierSetting()
        {
            Settings.Instance.TimeMultiplier = savedTimeMultiplier;
            Settings.Instance.FakeTimeMultiplier = savedTimeMultiplier;
            if (!Settings.Instance.ShouldWorkFollow)
            {
                Settings.Instance.WorkMultiplier = savedTimeMultiplier;
                Settings.Instance.FakeWorkMultiplier = savedTimeMultiplier;
            }
            Settings.Instance.Mod.WriteSettings();
        }


        public override void LoadedGame()
        {
            base.LoadedGame();
            // An incomplete setup state should never survive a save/load cycle.
            // Existing saves are handled by the saved-vs-current multiplier check.
            if (!setupCompleted)
            {
                CloseNewColonySetupWindow();
            }

            if (savedTimeMultiplier != Settings.Instance.TimeMultiplier)
            {
                string text = $"The saved time multiplier for this save is {savedTimeMultiplier}.\n" +
                              $"Your current multiplier is {Settings.Instance.TimeMultiplier}.\n" +
                              $"Continuing with the current multiplier may cause save corruption and many bugs.\n\n" +
                              $"Do you want to switch to the saved multiplier?\n\n" +
                              $"Though also note that the mod cannot be added midgame.";
                string text2 = $"Changing a played colony to a different multiplier is not considered safe.\n\n" +
                               $"Only continue if this is a newly started colony setup or you accept the risk.\n\n" +
                               $"Mark this save as using the current multiplier?";



                Find.WindowStack.Add(new Dialog_MessageBox(
                    text,
                    "Yes".Translate(), () =>
                    {
                        RestoreSavedMultiplierSetting();
                        Find.WindowStack.Add(new Dialog_MessageBox("Done! Please restart the game for the changes to apply. DO NOT SAVE NOW"));
                    },
                    "No".Translate(), () =>
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox(
                        text2,
                        "Yes".Translate(), () =>
                        {
                            Find.WindowStack.Add(new Dialog_MessageBox("You have been warned."));
                            savedTimeMultiplier = Settings.Instance.TimeMultiplier;
                            setupCompleted = true;
                        },
                            "No".Translate(), () =>
                            {
                                RestoreSavedMultiplierSetting();
                                Find.WindowStack.Add(new Dialog_MessageBox("Value changed! Please restart the game for the changes to apply. DO NOT SAVE NOW"));
                            }
                    ));
                    }
                ));
            }
        }
        private void CloseNewColonySetupWindowIfExpired()
        {
            if (setupCompleted)
            {
                return;
            }

            int ticksGame = Find.TickManager.TicksGame;
            if (setupStartTick < 0 || ticksGame > setupStartTick + NewColonySetupGraceTicks)
            {
                CloseNewColonySetupWindow();
            }
        }
        private void CloseNewColonySetupWindow()
        {
            setupCompleted = true;
            setupStartTick = -1;
        }
    }
}
