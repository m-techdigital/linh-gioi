#!/usr/bin/env python3
"""Reproducible Blender proof for rigid bone parenting, reopen and RGBA output.

The geometry is intentionally diagnostic. It is never a character-art candidate.
"""

from __future__ import annotations

import argparse
import json
import math
import sys
from pathlib import Path
from typing import Any


def capability_probe_spec() -> dict[str, Any]:
    return {
        "role": "INFRASTRUCTURE_CAPABILITY_ONLY",
        "runtimeEligible": False,
        "parts": {
            "Torso": "Chest",
            "UpperArmNear": "UpperArmNear",
            "LowerArmNear": "LowerArmNear",
        },
        "requiresTransparentFilm": True,
        "allowedTransformChannels": ["location", "rotation_euler"],
        "forbidden": ["ARMATURE_MODIFIER", "VERTEX_GROUP", "SHAPE_KEY", "SCALE_KEY"],
    }


def decide_capability_status(audit: dict[str, Any]) -> str:
    required_true = ("freshProcessReopen", "filmTransparent")
    failure_lists = (
        "boneParentFailures",
        "modifierFailures",
        "vertexGroupFailures",
        "shapeKeyFailures",
        "scaleFailures",
        "scaleCurveFailures",
    )
    valid = (
        all(audit.get(key) is True for key in required_true)
        and audit.get("renderChannels") == 4
        and float(audit.get("alphaMin", 1.0)) < 1.0
        and float(audit.get("alphaMax", 0.0)) > 0.0
        and all(not audit.get(key) for key in failure_lists)
    )
    return "BLENDER_RIGID_RGBA_REOPEN_PASS" if valid else "BLENDER_RIGID_RGBA_REOPEN_FAIL"


def _arguments_after_double_dash() -> list[str]:
    if "--" not in sys.argv:
        return []
    return sys.argv[sys.argv.index("--") + 1 :]


def _make_material(name: str, rgba: tuple[float, float, float, float]):
    import bpy

    material = bpy.data.materials.new(name)
    material.diffuse_color = rgba
    return material


def _make_rigid_box(name: str, center: tuple[float, float, float], dimensions: tuple[float, float, float], material):
    import bpy

    bpy.ops.mesh.primitive_cube_add(location=center)
    obj = bpy.context.object
    obj.name = name
    obj.dimensions = dimensions
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def _parent_to_bone(obj, armature, bone_name: str) -> None:
    world_matrix = obj.matrix_world.copy()
    obj.parent = armature
    obj.parent_type = "BONE"
    obj.parent_bone = bone_name
    obj.matrix_world = world_matrix


def create_probe(output_root: Path) -> Path:
    import bpy

    output_root.mkdir(parents=True, exist_ok=True)
    bpy.ops.wm.read_factory_settings(use_empty=True)

    bpy.ops.object.armature_add(enter_editmode=True, location=(0.0, 0.0, 0.0))
    armature = bpy.context.object
    armature.name = "LGO_RigidCapabilitySkeleton"
    edit_bones = armature.data.edit_bones
    initial = edit_bones[0]
    initial.name = "Chest"
    initial.head = (0.0, -0.4, 0.0)
    initial.tail = (0.0, 0.8, 0.0)

    upper = edit_bones.new("UpperArmNear")
    upper.head = (0.0, 0.55, 0.0)
    upper.tail = (0.75, 0.25, 0.0)
    upper.parent = initial

    lower = edit_bones.new("LowerArmNear")
    lower.head = upper.tail
    lower.tail = (1.35, -0.05, 0.0)
    lower.parent = upper
    bpy.ops.object.mode_set(mode="OBJECT")

    torso = _make_rigid_box("Torso", (0.0, 0.1, 0.0), (0.85, 1.25, 0.18), _make_material("TorsoMaterial", (0.10, 0.55, 0.75, 1.0)))
    upper_part = _make_rigid_box("UpperArmNear", (0.43, 0.38, -0.04), (0.95, 0.30, 0.16), _make_material("UpperMaterial", (0.92, 0.35, 0.24, 1.0)))
    upper_part.rotation_euler.z = math.radians(-22.0)
    lower_part = _make_rigid_box("LowerArmNear", (1.03, 0.10, -0.08), (0.80, 0.27, 0.14), _make_material("LowerMaterial", (0.98, 0.66, 0.22, 1.0)))
    lower_part.rotation_euler.z = math.radians(-27.0)
    for obj, bone_name in ((torso, "Chest"), (upper_part, "UpperArmNear"), (lower_part, "LowerArmNear")):
        _parent_to_bone(obj, armature, bone_name)
        obj.scale = (1.0, 1.0, 1.0)

    for bone_name, frame, degrees in (
        ("UpperArmNear", 1, 0.0),
        ("UpperArmNear", 12, 38.0),
        ("LowerArmNear", 1, 0.0),
        ("LowerArmNear", 12, -70.0),
    ):
        pose_bone = armature.pose.bones[bone_name]
        pose_bone.rotation_mode = "XYZ"
        pose_bone.rotation_euler.z = math.radians(degrees)
        pose_bone.keyframe_insert(data_path="rotation_euler", frame=frame, index=2)

    bpy.ops.object.camera_add(location=(0.55, 0.2, 10.0), rotation=(0.0, 0.0, 0.0))
    camera = bpy.context.object
    camera.name = "LGO_OrthoCamera"
    camera.data.type = "ORTHO"
    camera.data.ortho_scale = 3.8
    bpy.context.scene.camera = camera

    scene = bpy.context.scene
    scene.render.engine = "BLENDER_WORKBENCH"
    scene.render.film_transparent = True
    scene.render.image_settings.file_format = "PNG"
    scene.render.image_settings.color_mode = "RGBA"
    scene.render.resolution_x = 512
    scene.render.resolution_y = 512
    scene.render.resolution_percentage = 100
    scene.render.filepath = str(output_root / "create-frame-012.png")
    scene.frame_set(12)
    bpy.ops.render.render(write_still=True)

    blend_path = output_root / "rigid-capability-probe.blend"
    bpy.ops.wm.save_as_mainfile(filepath=str(blend_path))
    (output_root / "probe-spec.json").write_text(json.dumps(capability_probe_spec(), indent=2) + "\n")
    return blend_path


def _alpha_extrema(image_path: Path) -> tuple[int, float, float]:
    import bpy

    image = bpy.data.images.load(str(image_path), check_existing=False)
    image.colorspace_settings.name = "Non-Color"
    channels = image.channels
    pixels = list(image.pixels[:])
    alpha_values = pixels[3::channels] if channels >= 4 else []
    if not alpha_values:
        return channels, 1.0, 1.0
    return channels, min(alpha_values), max(alpha_values)


def audit_reopened_probe(output_root: Path) -> dict[str, Any]:
    import bpy

    spec = capability_probe_spec()
    scene = bpy.context.scene
    scene.frame_set(12)
    reopened_render = output_root / "reopen-frame-012.png"
    scene.render.filepath = str(reopened_render)
    bpy.ops.render.render(write_still=True)

    armature = bpy.data.objects.get("LGO_RigidCapabilitySkeleton")
    bone_parent_failures: list[str] = []
    modifier_failures: list[str] = []
    vertex_group_failures: list[str] = []
    shape_key_failures: list[str] = []
    scale_failures: list[str] = []
    for part_name, bone_name in spec["parts"].items():
        obj = bpy.data.objects.get(part_name)
        if obj is None or armature is None or obj.parent != armature or obj.parent_type != "BONE" or obj.parent_bone != bone_name:
            bone_parent_failures.append(part_name)
            continue
        if obj.modifiers:
            modifier_failures.append(part_name)
        if obj.vertex_groups:
            vertex_group_failures.append(part_name)
        if obj.data.shape_keys is not None:
            shape_key_failures.append(part_name)
        if any(abs(value - 1.0) > 1e-6 for value in obj.scale):
            scale_failures.append(part_name)

    if armature is None or any(abs(value - 1.0) > 1e-6 for value in armature.scale):
        scale_failures.append("LGO_RigidCapabilitySkeleton")

    scale_curve_failures: list[str] = []
    for action in bpy.data.actions:
        for curve in getattr(action, "fcurves", []):
            if "scale" in curve.data_path:
                scale_curve_failures.append(f"{action.name}:{curve.data_path}")

    channels, alpha_min, alpha_max = _alpha_extrema(reopened_render)
    audit = {
        "freshProcessReopen": True,
        "role": spec["role"],
        "runtimeEligible": spec["runtimeEligible"],
        "blendFile": bpy.data.filepath,
        "renderFile": str(reopened_render),
        "filmTransparent": bool(scene.render.film_transparent),
        "renderChannels": channels,
        "alphaMin": alpha_min,
        "alphaMax": alpha_max,
        "boneParentFailures": bone_parent_failures,
        "modifierFailures": modifier_failures,
        "vertexGroupFailures": vertex_group_failures,
        "shapeKeyFailures": shape_key_failures,
        "scaleFailures": scale_failures,
        "scaleCurveFailures": scale_curve_failures,
    }
    audit["status"] = decide_capability_status(audit)
    report_path = output_root / "capability-report.json"
    report_path.write_text(json.dumps(audit, indent=2) + "\n")
    return audit


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--mode", choices=("create", "audit"), required=True)
    parser.add_argument("--output-root", type=Path, required=True)
    arguments = parser.parse_args(_arguments_after_double_dash())
    output_root = arguments.output_root.resolve()
    if arguments.mode == "create":
        print(create_probe(output_root))
        return 0
    report = audit_reopened_probe(output_root)
    print(json.dumps(report, indent=2))
    return 0 if report["status"] == "BLENDER_RIGID_RGBA_REOPEN_PASS" else 2


if __name__ == "__main__":
    raise SystemExit(main())
