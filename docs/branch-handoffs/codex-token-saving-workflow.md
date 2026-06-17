# Branch Handoff

Branch: `codex/token-saving-workflow`

Base branch: `dev`

Head commit: branch head containing this handoff

## Goal

Add a lightweight workflow layer that reduces future Codex/LLM token use and makes the workflow easier for future agents and human contributors to follow.

## Scope

Files intentionally changed:

- `docs/project-state.md`
- `docs/token-saving-workflow.md`
- `docs/branch-handoffs/TEMPLATE.md`
- `docs/test-reports/TEMPLATE.md`
- `.codex/scripts/Make-Handoff.ps1`
- `AGENTS.md`
- `.github/pull_request_template.md`
- `.codex/scripts/Test-Workflow.ps1`
- `docs/branch-handoffs/codex-token-saving-workflow.md`

Files intentionally avoided:

- Gameplay code
- XML patch defs
- Build outputs
- Existing untracked local helper files

## Implementation Summary

- Added a compact project-state file so future sessions have a single first read.
- Added branch handoff and test report templates.
- Added a handoff-generation script that summarizes branch state locally.
- Added root `AGENTS.md` so future Codex agents automatically receive the workflow rules.
- Added a pull request template so GitHub review asks for scope, verification, documentation, and risk.
- Added an advisory workflow check script that warns about missing workflow artifacts without blocking local work by default.

## Verification

Commands run:

- `.codex/scripts/Make-Handoff.ps1`
- `.codex/scripts/Test-Workflow.ps1`

User/runtime tests:

- Not applicable. This branch changes project workflow docs and helper scripts only.

## Risks

- The workflow check is intentionally advisory. It cannot guarantee documentation quality.
- Future agents still need to follow `AGENTS.md`; the script helps catch missing artifacts but does not replace human judgment.

## Next Action

Review the workflow branch, then merge it into `dev` if the structure feels right.
