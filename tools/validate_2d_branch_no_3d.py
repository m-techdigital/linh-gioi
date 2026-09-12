#!/usr/bin/env python3
from __future__ import annotations

from pathlib import Path
import sys


ROOT = Path(__file__).resolve().parents[1]

FORBIDDEN_PATHS = [
    "assets/characters",
    "client/art-source",
    "client/Unity/Assets/Game/Art/Items",
    "client/Unity/Assets/Game/Art/OnboardingCandidate",
    "client/Unity/Assets/Game/Foundation/DefaultCharacterAppearance.cs",
    "client/Unity/Assets/Game/Foundation/DefaultCharacterBaseCatalog.cs",
    "client/Unity/Assets/Game/Foundation/DefaultCharacterExpression.cs",
    "client/Unity/Assets/Game/Foundation/ItemAppearanceAttachment.cs",
    "client/Unity/Assets/Game/Foundation/ItemAppearanceMaskSet.cs",
    "client/Unity/Assets/Game/Foundation/ItemMeshMask.cs",
    "client/Unity/Assets/Game/Foundation/ItemPresentationCatalog.cs",
    "client/Unity/Assets/Game/Foundation/ItemTryOnSession.cs",
    "client/Unity/Assets/Game/Foundation/NpcAppearanceInstance.cs",
    "client/Unity/Assets/Game/Foundation/NpcAppearancePreset.cs",
    "client/Unity/Assets/Game/Foundation/WardrobeDrapeMotion.cs",
    "client/Unity/Assets/Game/Foundation/Editor/ArrivalOutfitImporter.cs",
    "client/Unity/Assets/Game/Foundation/Editor/DefaultCharacterBaseImporter.cs",
    "client/Unity/Assets/Game/Foundation/Editor/ItemPresentationAuthoring.cs",
    "client/Unity/Assets/Game/Foundation/Editor/" + "Mes" + "hyPlayableBaseImporter.cs",
    "client/Unity/Assets/Game/Foundation/Editor/NpcAppearanceBaker.cs",
    "client/Unity/Assets/Game/Foundation/Editor/NpcAppearanceRecipe.cs",
    "client/Unity/Assets/Game/Foundation/Editor/SocialMotionImporter.cs",
    "client/Unity/Assets/Game/Foundation/Editor/SocialRosterImporter.cs",
    "client/Unity/Assets/Game/Foundation/Editor/StarterWardrobeImporter.cs",
    "client/Unity/Assets/Game/Foundation/Editor/WardrobePoseExporter.cs",
    "client/Unity/Assets/Game/Foundation/Editor/WardrobeSurfaceImporter.cs",
    "client/Unity/Assets/Game/UI/Runtime/CharacterQualityReviewCamera.cs",
    "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
    "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.Evidence.cs",
    "client/Unity/Assets/Game/UI/Runtime/M5VisualEvidenceRunner.cs",
    "client/Unity/Assets/Game/UI/Runtime/OnboardingBlockoutPreview.cs",
    "client/Unity/Assets/Game/UI/Runtime/OnboardingDefaultBasePreview.cs",
    "client/Unity/Assets/Game/UI/Runtime/OnboardingItemPreview.cs",
    "client/Unity/Assets/Game/UI/Runtime/OnboardingSquarePlaces.cs",
    "client/Unity/Assets/Game/UI/Runtime/OnboardingTrainingSquarePreview.cs",
    "client/Unity/Assets/Game/UI/Runtime/OnboardingWardrobeTryOn.cs",
    "client/Unity/Assets/Game/UI/Runtime/VisualRuntimeEvidenceRunner.cs",
    "client/Unity/Assets/Game/World/Runtime/CityArchitectureVisuals.cs",
    "client/Unity/Assets/Game/World/Runtime/CityEverydayProps.cs",
    "client/Unity/Assets/Game/World/Runtime/CityGardenBotany.cs",
    "client/Unity/Assets/Game/World/Runtime/CitySquareDetails.cs",
    "client/Unity/Assets/Game/World/Runtime/NpcGuideGesture.cs",
    "client/Unity/Assets/Game/World/Runtime/OnboardingBlockoutWorld.cs",
    "client/Unity/Assets/Game/World/Runtime/OnboardingTrainingSquare.cs",
    "client/Unity/Assets/Game/World/Runtime/TrainingSquareSky.cs",
    "client/Unity/Assets/Game/World/Runtime/TrainingStoneVisuals.cs",
    "client/Unity/Assets/Game/World/Runtime/WorldHubSetDressing.cs",
    "client/Unity/Assets/Game/World/Runtime/WorldLabelPresenter.cs",
    "client/Unity/Assets/Game/World/Runtime/WorldProceduralVisuals.cs",
    "tools/art",
]

# Retired presentation with no remaining Unity code or serialized GUID consumers.
# Keep shared 2D mesh rendering and M4/M6 gameplay smoke contracts intact.
FORBIDDEN_PATHS += [
    "client/Unity/Assets/Game/Art/Runtime/V2",
    "client/Unity/Assets/Game/Art/Runtime/V3B",
    "client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV2.cs",
    *[
        "client/Unity/Assets/Game/UI/Runtime/" + name + ".cs"
        for name in (
            "RuntimeLoginResponsiveLayout",
            "RuntimeCharacterHallResponsiveLayout",
            "RuntimeSessionMenuLayout",
            "RuntimeWorldHudResponsiveLayout",
            "RuntimeWorldGuidanceView",
            "RuntimeNpcDialogueView",
        )
    ],
]

FORBIDDEN_SUFFIXES = {
    ".fbx",
    ".blend",
    ".blend1",
    ".anim",
    ".overrideController",
}


def main() -> int:
    failures: list[str] = []
    for rel in FORBIDDEN_PATHS:
        path = ROOT / rel
        if path.exists():
            failures.append(rel)

    for base_rel in ["assets", "client/Unity/Assets/Game", "client/art-source", "tools/art"]:
        base = ROOT / base_rel
        if not base.exists():
            continue
        for path in base.rglob("*"):
            if path.is_file() and path.suffix.lower() in FORBIDDEN_SUFFIXES:
                failures.append(str(path.relative_to(ROOT)))

    if failures:
        print("LGO_2D_BRANCH_NO_3D_FAIL")
        for rel in sorted(set(failures))[:240]:
            print(rel)
        if len(set(failures)) > 240:
            print(f"... and {len(set(failures)) - 240} more")
        return 1

    print("LGO_2D_BRANCH_NO_3D_PASS")
    return 0


if __name__ == "__main__":
    sys.exit(main())
