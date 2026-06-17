# DayStretch Project State

Last updated: 2026-06-17

## Baseline

Current integration branch: `dev`

The `dev` branch contains:

- dotnet build package-reference support
- core time scaling formula fixes
- GenDate instruction metadata preservation
- explicit new-colony setup flow
- setup-flow hardening after user testing
- removal of obsolete `ResultPatchDef` and `DeltaPatchDef` XML files

## Verified Behavior

The normal new-colony setup path was tested on 2026-06-17:

- launch game
- create a normal colony
- apply the DayStretch timer
- save and quit
- relaunch game
- load the save with the applied timer

The tester observed 0 errors across both launches and both colony/save flows.

## Known Separate Work

The dev quick test scenario remains separate from the normal new-colony path. Do not treat normal-colony success as proof that dev quick test is fixed.

## Context Rules

Before starting a new Codex task, read this file first, then read only the specific files needed for the task. Keep this file compact; move detailed records into `docs/test-reports`, `docs/decisions`, or branch handoff files.

