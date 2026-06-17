# Token-Saving Workflow

Purpose: keep Codex and other LLM sessions focused by storing durable facts in the repo and sending agents compact handoffs instead of raw chat history, full logs, or broad diffs.

## Operating Model

Use a context pyramid:

1. Raw facts stay local: source files, git history, RimWorld logs, saves, screenshots, and build output.
2. Local tools extract the important facts: changed files, commits, error fingerprints, and relevant log lines.
3. Codex receives the smallest useful packet: goal, branch, files, observations, verification, and open risks.
4. Durable conclusions go back into docs so future sessions do not rediscover them.

## Standard Flow

1. Start from `docs/project-state.md`.
2. Use `rg`, `git diff`, `git log`, and the scripts under `.codex/scripts` before asking an LLM to reason over raw output.
3. For each branch, create or update a branch handoff using `docs/branch-handoffs/TEMPLATE.md`.
4. After a user test, write a short report using `docs/test-reports/TEMPLATE.md`.
5. Update `docs/project-state.md` only with compact current facts.

## Enforcement Points

- `AGENTS.md` makes the workflow visible to future Codex agents automatically.
- `.codex/scripts/Test-Workflow.ps1` checks for missing workflow artifacts and oversized project state.
- `.github/pull_request_template.md` asks contributors to record scope, verification, documentation, and risk before merge.

## Agent Rules

- Give agents exact files or line ranges whenever possible.
- Do not paste full logs into chat when a local file path and search terms will do.
- Use subagents for bounded read-heavy work, not broad repeated repo exploration.
- Ask implementation agents for a handoff packet before any documentation agent runs.
- Give documentation agents the handoff packet and relevant docs, not the whole conversation.

## Compression Rules

- Prefer bullet facts over narrative.
- Preserve exact commit hashes, branch names, file paths, and user-tested results.
- Replace repeated background with links to local docs.
- Store decisions once in `docs/decisions`; refer to the decision file afterward.
- Keep `AGENTS.md` reserved for stable instructions, not project history.

## When To Spend More Tokens

Spend context deliberately when:

- a Harmony transpiler touches shared execution behavior
- a RimWorld save format or startup lifecycle assumption is being changed
- the user reports a new runtime error
- source-code references from RimWorld assemblies are needed to verify behavior
