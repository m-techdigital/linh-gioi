# M4 Visible UI Review Command v0.14.0

Use from repo root `LinhGioiOnline`.

Build and open a fresh macOS player:

```bash
./tools/run_m4_visible_ui_review.sh --rebuild
```

Open an existing built macOS player:

```bash
./tools/run_m4_visible_ui_review.sh --open-existing
```

Stop the local review API/player:

```bash
./tools/run_m4_visible_ui_review.sh --stop
```

The review harness opens the player in a visible 1280x720 window:

```text
-screen-fullscreen 0 -screen-width 1280 -screen-height 720
```

Logs are written under:

```text
build/manual-ui/
```

Screenshots to capture:

- Login / Gate Entry before pressing Open Gate.
- Character Hall after login, including empty/list/create/select state.
- World HUD after entering the world.

Manual checklist:

- Login: title, API status, dev key field, Open Gate button, and error/loading text are readable.
- Character Hall: empty/list/create/select state, selected preview, and Enter World action are visible.
- World HUD: compact status strip, quiet position/debug panel, Save Position, Back to Lobby, and movement hint are visible.
- Exit: Quit button and Escape key can close the visible player safely.
- Decorative panels do not cover important text or actions.

Non-claims:

- not final production UI
- not final production art
- no production auth
- no DB persistence
- no full MMO gameplay
- no combat system
