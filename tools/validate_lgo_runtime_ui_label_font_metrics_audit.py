#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read(rel: str) -> str:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing file: {rel}")
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def require(rel: str, *markers: str) -> str:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")
    return text


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains forbidden marker: {marker}")


def check_frozen() -> None:
    result = subprocess.run(
        [
            "git",
            "--no-pager",
            "diff",
            "--name-only",
            "--",
            "protocol",
            "gamedata/schemas",
            "docs/adr",
            "client/Unity/Assets/Game/UI/design-tokens.json",
        ],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "git frozen diff failed")
    elif result.stdout.strip():
        ERRORS.append("frozen surface changed")


def main() -> int:
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "LoginApiLabelFontSize",
        "LoginAccountStatusFontSize",
        "LoginHeroTitleFontSize",
        "LoginHeroCopyFontSize",
        "LoginServerTextInitialFontSize",
        "LobbyIntroMobileFontSize",
        "LobbyIntroDesktopFontSize",
        "EmptyCharacterHintMobileFontSize",
        "EmptyCharacterHintDesktopFontSize",
        "SelectedCharacterNameFontSize",
        "WorldMetaFontSize",
        "WorldNameInitialFontSize",
        "WorldAreaFontSize",
        "WorldStepFontSize",
        "WorldDirectionFontSize",
        "WorldObjectiveInitialFontSize",
        "WorldInteractionInitialFontSize",
        "WorldNameMobileFontSize",
        "WorldNameDesktopFontSize",
        "WorldObjectiveMobileFontSize",
        "WorldObjectiveDesktopFontSize",
        "WorldInteractionMobileFontSize",
        "WorldInteractionDesktopFontSize",
        "DialogueSpeakerMobileFontSize",
        "DialogueSpeakerDesktopFontSize",
        "DialogueSpeakerInitialFontSize",
        "DialogueLineMobileFontSize",
        "DialogueLineDesktopFontSize",
        "DialogueProgressMobileFontSize",
        "DialogueProgressDesktopFontSize",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSpacing.LoginApiLabelFontSize",
        "RuntimeUiSpacing.LoginAccountStatusFontSize",
        "RuntimeUiSpacing.LoginHeroTitleFontSize",
        "RuntimeUiSpacing.LoginHeroCopyFontSize",
        "RuntimeUiSpacing.LoginServerTextInitialFontSize",
        "RuntimeUiSpacing.SelectedCharacterNameFontSize",
        "RuntimeUiSpacing.WorldMetaFontSize",
        "RuntimeUiSpacing.WorldNameInitialFontSize",
        "RuntimeUiSpacing.WorldAreaFontSize",
        "RuntimeUiSpacing.WorldStepFontSize",
        "RuntimeUiSpacing.WorldDirectionFontSize",
        "RuntimeUiSpacing.WorldObjectiveInitialFontSize",
        "RuntimeUiSpacing.WorldInteractionInitialFontSize",
        "RuntimeUiSpacing.LobbyIntroMobileFontSize",
        "RuntimeUiSpacing.EmptyCharacterHintDesktopFontSize",
        "RuntimeUiSpacing.WorldNameMobileFontSize",
        "RuntimeUiSpacing.WorldObjectiveDesktopFontSize",
        "RuntimeUiSpacing.WorldInteractionDesktopFontSize",
        "RuntimeUiSpacing.DialogueSpeakerMobileFontSize",
        "RuntimeUiSpacing.DialogueSpeakerInitialFontSize",
        "RuntimeUiSpacing.DialogueLineDesktopFontSize",
        "RuntimeUiSpacing.DialogueProgressDesktopFontSize",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "apiLabel.style.fontSize = 12;",
        "_account.style.fontSize = 13;",
        "_worldMeta.style.fontSize = 12;",
        "_dialogueLine.style.fontSize = 16;",
        "_lobbyIntro.style.fontSize = mobile ? 13 : 14;",
        "_emptyCharacterHint.style.fontSize = mobile ? 13 : 14;",
        "_worldName.style.fontSize = mobile ? 16 : 17;",
        "_worldObjective.style.fontSize = mobile ? 14 : 15;",
        "_interactionHint.style.fontSize = mobile ? 14 : 15;",
        "_dialogueSpeaker.style.fontSize = mobile ? 15 : 17;",
        "_dialogueLine.style.fontSize = mobile ? 14 : 16;",
        "_dialogueProgress.style.fontSize = mobile ? 12 : 13;",
        "NewCompactStatusLabel(\"Khu vực: xem trước tại sảnh\", RuntimeArtCatalog.Muted, 12)",
        "NewCompactStatusLabel(\"Tiến trình: Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện.\", RuntimeArtCatalog.Spirit, 13)",
        "NewCompactStatusLabel(\"Mục tiêu: gặp Người Giữ Cổng.\", RuntimeArtCatalog.Gold, 14)",
        "RuntimeUiSkin.ApplyText(_loginHeroTitle, RuntimeArtCatalog.Text, 25",
        "RuntimeUiSkin.ApplyText(_loginHeroCopy, RuntimeArtCatalog.Text, 15",
        "RuntimeUiSkin.ApplyText(serverText, RuntimeArtCatalog.Text, 18",
        "RuntimeUiSkin.ApplyText(_selectedName, RuntimeArtCatalog.Gold, 21",
        "RuntimeUiSkin.ApplyText(_worldName, RuntimeArtCatalog.Gold, 17",
        "RuntimeUiSkin.ApplyText(_dialogueSpeaker, RuntimeArtCatalog.Gold, 17",
    )
    require(
        "docs/design/RUNTIME-UI-LABEL-FONT-METRICS-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_LABEL_FONT_METRICS_READY",
        "RuntimeUiSpacing",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-LABEL-FONT-METRICS-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_LABEL_FONT_METRICS_READY",
        "LGO-RUNTIME-UI-LABEL-FONT-METRICS-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_label_font_metrics_audit",
        "validate_lgo_runtime_ui_label_font_metrics_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-LABEL-FONT-METRICS-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_LABEL_FONT_METRICS_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-LABEL-FONT-METRICS-AUDIT v1.0",
        "LGO_RUNTIME_UI_LABEL_FONT_METRICS_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI LABEL FONT METRICS AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_LABEL_FONT_METRICS_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
