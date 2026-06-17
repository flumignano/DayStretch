# DayStretch Agent Instructions

These instructions apply to the whole repository.

## Start Here

Before doing project work, read `docs/project-state.md`. Treat it as the compact current state of the project, not as full documentation.

Then inspect only the files needed for the task. Prefer local search and targeted file reads over broad repo-wide exploration.

## Token Discipline

- Use `rg`, `git status`, `git diff`, `git log`, and `.codex/scripts/Make-Handoff.ps1` before asking the model to reason over raw output.
- Do not paste full logs into chat when a file path plus search terms or line ranges will work.
- Keep handoffs compact: branch, goal, files, decisions, verification, risks, next action.
- Keep `docs/project-state.md` small and current. Move details into `docs/test-reports`, `docs/decisions`, or `docs/branch-handoffs`.
- Use subagents only for bounded read-heavy tasks. Give them exact files, folders, or line ranges.

## Branch Workflow

- Do feature and investigation work on a branch, not directly on `dev`, unless the user explicitly says otherwise.
- Before committing meaningful work, run `.codex/scripts/Test-Workflow.ps1`.
- For branches with implementation or investigation value, create or update a branch handoff under `docs/branch-handoffs`.
- After user runtime testing, record the result under `docs/test-reports` and update `docs/project-state.md` only with the short current fact.

## RimWorld-Specific Rules

- Use the RimWorld source references under `C:\Users\Administrator\Documents\source` for analysis when behavior depends on game internals.
- Do not copy RimWorld decompiled code into the mod or docs. Use it only to understand behavior and write original code.
- Be especially careful around Harmony transpilers, save/load behavior, world/map lifecycle, and anything that changes time scaling after a colony has already been created.

## Documentation Rules

- Comments and docs should explain the purpose and safety reason for changes, especially where DayStretch behavior differs from ordinary RimWorld settings.
- Avoid broad comment sweeps. Prefer comments that protect future maintainers from misunderstanding lifecycle, compatibility, or save-safety assumptions.
- Keep documentation factual. Record tested behavior separately from inferred behavior.

