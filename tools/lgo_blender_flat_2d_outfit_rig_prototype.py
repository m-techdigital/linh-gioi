#!/usr/bin/env python3.12
"""Create a bounded Blender flat-card 2D outfit rig prototype.

This is a source/rig experiment only. It creates an editable .blend and a Unity
FBX for ArchitectureProbe; it does not replace production body/outfit assets.
"""
from __future__ import annotations

import argparse
import json
import shutil
import sys
from pathlib import Path

RENDER_PATH = "BLENDER_FLAT_CARD_RIG_TO_UNITY_FBX_PLAYER"
ACTIONS = ["idle", "run", "jump", "attack", "return_to_idle"]
PRIMARY_UPPER = "phap_lv001_outer_top_sleeved_a"
SECOND_UPPER = "phap_lv001_outer_top_sleeved_b"


def validate_prototype_report(report: dict) -> dict:
    failures: list[str] = []
    if report.get("renderPath") != RENDER_PATH:
        failures.append("WRONG_RENDER_PATH")
    source = report.get("source") or {}
    if not source.get("blendExists") or not source.get("editableRig"):
        failures.append("EDITABLE_BLEND_RIG_MISSING")
    if not source.get("fbxExists"):
        failures.append("UNITY_FBX_EXPORT_MISSING")
    armature = report.get("armature") or {}
    if armature.get("boneCount", 0) < 14:
        failures.append("ARMATURE_TOO_SMALL_FOR_BODY_AND_SLEEVES")
    if armature.get("usesBoneScaleAnimation"):
        failures.append("BONE_SCALE_ANIMATION_FORBIDDEN")
    if set(ACTIONS) - set(report.get("actions") or []):
        failures.append("REQUIRED_ACTIONS_MISSING")

    items = report.get("outfitItems") or []
    upper_items = [item for item in items if item.get("family") == "outer_top"]
    if len(upper_items) < 2:
        failures.append("SECOND_UPPER_ITEM_MISSING")
    if upper_items:
        first_id = upper_items[0].get("itemId")
        for item in upper_items:
            if not item.get("hasSleeves"):
                failures.append("SLEEVED_UPPER_MISSING")
                break
            if item.get("perPoseOffsets"):
                failures.append("ITEM_USES_PER_POSE_OFFSETS")
                break
        if len(upper_items) >= 2 and upper_items[1].get("usesSameRigAs") != first_id:
            failures.append("SECOND_ITEM_DOES_NOT_REUSE_FIRST_RIG")
    slots = set(report.get("detachableSlots") or [])
    if "waist_belt" not in slots:
        failures.append("DETACHABLE_WAIST_BELT_MISSING")
    if "shoulder_chest_guard" not in slots:
        failures.append("DETACHABLE_SHOULDER_CHEST_GUARD_MISSING")
    if report.get("runtimePromotionAllowed") is not False:
        failures.append("RUNTIME_PROMOTION_MUST_REMAIN_FALSE")
    return {
        "status": "SOURCE_READY_FOR_PLAYER_PROBE" if not failures else "REJECT_SOURCE_PROTOTYPE",
        "failures": failures,
        "runtimePromotionAllowed": False,
    }


def _parse_args(argv: list[str]) -> argparse.Namespace:
    args = argv[argv.index("--") + 1:] if "--" in argv else argv[1:]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output-dir", required=True, type=Path)
    parser.add_argument("--unity-fbx", type=Path, help="Optional Unity Assets destination for the generated FBX")
    return parser.parse_args(args)


def _build_scene(output_dir: Path, unity_fbx: Path | None) -> None:
    import bpy

    output_dir.mkdir(parents=True, exist_ok=True)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()
    for action in list(bpy.data.actions):
        bpy.data.actions.remove(action)

    def mat(name: str, color: tuple[float, float, float, float]):
        material = bpy.data.materials.new(name)
        material.diffuse_color = color
        return material

    skin = mat("editable_skin_proxy", (0.92, 0.66, 0.52, 1.0))
    hair = mat("editable_hair_proxy", (0.08, 0.12, 0.18, 1.0))
    robe_a = mat("phap_outer_top_sleeved_a_blue", (0.13, 0.21, 0.40, 1.0))
    robe_b = mat("phap_outer_top_sleeved_b_teal", (0.08, 0.34, 0.36, 1.0))
    belt_mat = mat("detachable_belt_gold", (0.95, 0.62, 0.18, 1.0))
    guard_mat = mat("detachable_guard_gold", (0.90, 0.70, 0.28, 1.0))
    line = mat("ink_line", (0.03, 0.025, 0.02, 1.0))

    armature_data = bpy.data.armatures.new("LGO_Flat2D_EditableRig")
    armature = bpy.data.objects.new("LGO_Flat2D_EditableRig", armature_data)
    bpy.context.collection.objects.link(armature)
    bpy.context.view_layer.objects.active = armature
    armature.select_set(True)
    bpy.ops.object.mode_set(mode="EDIT")
    bones = {
        "root": ((0, 0, 0.0), (0, 0, 0.18), None),
        "pelvis": ((0, 0, 0.18), (0, 0, 0.62), "root"),
        "spine": ((0, 0, 0.62), (0, 0, 1.26), "pelvis"),
        "neck": ((0, 0, 1.26), (0, 0, 1.38), "spine"),
        "head": ((0, 0, 1.38), (0, 0, 1.72), "neck"),
        "upper_arm.L": ((-0.22, 0, 1.20), (-0.55, 0, 1.00), "spine"),
        "forearm.L": ((-0.55, 0, 1.00), (-0.78, 0, 0.78), "upper_arm.L"),
        "hand.L": ((-0.78, 0, 0.78), (-0.88, 0, 0.70), "forearm.L"),
        "upper_arm.R": ((0.22, 0, 1.20), (0.55, 0, 1.00), "spine"),
        "forearm.R": ((0.55, 0, 1.00), (0.78, 0, 0.78), "upper_arm.R"),
        "hand.R": ((0.78, 0, 0.78), (0.88, 0, 0.70), "forearm.R"),
        "thigh.L": ((-0.14, 0, 0.20), (-0.20, 0, -0.36), "pelvis"),
        "shin.L": ((-0.20, 0, -0.36), (-0.18, 0, -0.92), "thigh.L"),
        "thigh.R": ((0.14, 0, 0.20), (0.20, 0, -0.36), "pelvis"),
        "shin.R": ((0.20, 0, -0.36), (0.18, 0, -0.92), "thigh.R"),
        "weapon_socket.R": ((0.88, 0, 0.70), (1.04, 0, 0.66), "hand.R"),
    }
    for name, (head_pos, tail_pos, parent) in bones.items():
        bone = armature_data.edit_bones.new(name)
        bone.head = head_pos
        bone.tail = tail_pos
        if parent:
            bone.parent = armature_data.edit_bones[parent]
            bone.use_connect = False
    bpy.ops.object.mode_set(mode="POSE")
    for bone in armature.pose.bones:
        bone.rotation_mode = "XYZ"
    bpy.ops.object.mode_set(mode="OBJECT")

    def card(name: str, loc: tuple[float, float, float], scale: tuple[float, float, float], material, bone: str):
        bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
        obj = bpy.context.object
        obj.name = name
        obj.scale = scale
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        obj.data.materials.append(material)
        obj.parent = armature
        obj.parent_type = "BONE"
        obj.parent_bone = bone
        return obj

    def empty(name: str):
        obj = bpy.data.objects.new(name, None)
        bpy.context.collection.objects.link(obj)
        obj.empty_display_type = "PLAIN_AXES"
        return obj

    # Body proxy: editable source, not production anatomy.
    card("body__torso_skin", (0, 0.03, 0.92), (0.25, 0.018, 0.36), skin, "spine")
    card("body__head_skin", (0, 0.03, 1.56), (0.18, 0.018, 0.20), skin, "head")
    card("body__hair", (0, 0.035, 1.72), (0.22, 0.016, 0.09), hair, "head")
    for side, x in (("L", -0.40), ("R", 0.40)):
        card(f"body__upper_arm_skin_{side}", (x, 0.025, 1.08), (0.09, 0.014, 0.20), skin, f"upper_arm.{side}")
        card(f"body__forearm_skin_{side}", (x * 1.55, 0.025, 0.88), (0.08, 0.014, 0.20), skin, f"forearm.{side}")
        card(f"body__hand_skin_{side}", (x * 1.95, 0.025, 0.70), (0.07, 0.014, 0.06), skin, f"hand.{side}")
    for side, x in (("L", -0.15), ("R", 0.15)):
        card(f"body__leg_skin_{side}", (x, 0.02, -0.28), (0.08, 0.014, 0.50), skin, f"thigh.{side}")
        card(f"body__shin_skin_{side}", (x * 1.15, 0.02, -0.76), (0.08, 0.014, 0.28), skin, f"shin.{side}")

    def make_upper(root_name: str, material, collar_extra: bool):
        root = empty(root_name)
        pieces = [
            card(root_name + "__torso", (0, -0.01, 0.88), (0.32, 0.026, 0.42), material, "spine"),
            card(root_name + "__hem", (0, -0.012, 0.50), (0.38, 0.026, 0.12), material, "pelvis"),
            card(root_name + "__sleeve_L", (-0.43, -0.012, 1.05), (0.12, 0.024, 0.24), material, "upper_arm.L"),
            card(root_name + "__sleeve_R", (0.43, -0.012, 1.05), (0.12, 0.024, 0.24), material, "upper_arm.R"),
            card(root_name + "__cuff_L", (-0.62, -0.014, 0.88), (0.11, 0.024, 0.06), line, "forearm.L"),
            card(root_name + "__cuff_R", (0.62, -0.014, 0.88), (0.11, 0.024, 0.06), line, "forearm.R"),
        ]
        if collar_extra:
            pieces.append(card(root_name + "__collar", (0, -0.016, 1.26), (0.20, 0.024, 0.08), guard_mat, "spine"))
        for obj in pieces:
            obj["lgo_slot_family"] = "outer_top"
            obj["lgo_rig_reuse_key"] = PRIMARY_UPPER
        return root

    make_upper("equipment__upper_primary", robe_a, True)
    make_upper("equipment__upper_variant", robe_b, False)
    card("equipment__waist_belt", (0, -0.04, 0.60), (0.40, 0.035, 0.055), belt_mat, "pelvis")
    card("equipment__shoulder_chest_guard", (0, -0.05, 1.16), (0.34, 0.034, 0.055), guard_mat, "spine")
    card("equipment__rigid_hand_item", (1.08, -0.02, 0.66), (0.28, 0.024, 0.035), guard_mat, "weapon_socket.R")

    poses = {
        "idle": {"spine": (0, 0, 0.02)},
        "run": {"thigh.L": (0.58, 0, 0), "shin.L": (-0.35, 0, 0), "thigh.R": (-0.50, 0, 0), "shin.R": (0.35, 0, 0), "upper_arm.L": (-0.28, 0, 0), "upper_arm.R": (0.28, 0, 0)},
        "jump": {"pelvis": (0.18, 0, 0), "thigh.L": (0.80, 0, 0), "shin.L": (-0.85, 0, 0), "thigh.R": (0.74, 0, 0), "shin.R": (-0.80, 0, 0), "upper_arm.L": (-0.55, 0, 0), "upper_arm.R": (-0.48, 0, 0)},
        "attack": {"spine": (0, 0, -0.10), "upper_arm.R": (0, -0.95, 0.25), "forearm.R": (0, -0.55, 0.10)},
        "return_to_idle": {"spine": (0, 0, 0.01), "upper_arm.R": (0, -0.50, 0.10)},
    }
    for name, rotation_by_bone in poses.items():
        action = bpy.data.actions.new(name)
        armature.animation_data_create()
        armature.animation_data.action = action
        for frame, blend in ((1, 0.0), (12, 1.0), (24, 0.0)):
            for pose_bone in armature.pose.bones:
                target = rotation_by_bone.get(pose_bone.name, (0, 0, 0))
                pose_bone.rotation_euler = tuple(value * blend for value in target)
                pose_bone.keyframe_insert("rotation_euler", frame=frame, group=pose_bone.name)
        action.use_fake_user = True
    armature.animation_data.action = None

    camera_data = bpy.data.cameras.new("Flat2D_OrthoCamera")
    camera = bpy.data.objects.new("Flat2D_OrthoCamera", camera_data)
    bpy.context.collection.objects.link(camera)
    camera.location = (0, -5, 0.35)
    camera.rotation_euler = (1.5708, 0, 0)
    camera_data.type = "ORTHO"
    camera_data.ortho_scale = 3.2
    bpy.context.scene.camera = camera

    blend = output_dir / "lgo-flat-2d-outfit-rig-prototype.blend"
    fbx = output_dir / "lgo-flat-2d-outfit-rig-prototype.fbx"
    bpy.ops.wm.save_as_mainfile(filepath=str(blend))
    bpy.ops.export_scene.fbx(filepath=str(fbx), use_selection=False, add_leaf_bones=False, bake_anim=True, bake_anim_use_all_actions=True)
    if unity_fbx is not None:
        unity_fbx.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(fbx, unity_fbx)

    report = {
        "renderPath": RENDER_PATH,
        "source": {
            "blend": str(blend),
            "blendExists": blend.is_file(),
            "fbx": str(fbx),
            "fbxExists": fbx.is_file(),
            "unityFbx": str(unity_fbx) if unity_fbx else None,
            "editableRig": True,
        },
        "armature": {
            "name": armature.name,
            "boneCount": len(armature.data.bones),
            "bones": sorted(b.name for b in armature.data.bones),
            "usesBoneScaleAnimation": False,
        },
        "outfitItems": [
            {"itemId": PRIMARY_UPPER, "family": "outer_top", "hasSleeves": True, "usesSameRigAs": "self", "perPoseOffsets": {}},
            {"itemId": SECOND_UPPER, "family": "outer_top", "hasSleeves": True, "usesSameRigAs": PRIMARY_UPPER, "perPoseOffsets": {}},
        ],
        "detachableSlots": ["waist_belt", "shoulder_chest_guard"],
        "actions": ACTIONS,
        "runtimePromotionAllowed": False,
        "claimLimit": "Thử nghiệm source/rig riêng bằng Blender flat-card; chưa thay production, chưa phải visual final của nhân vật game.",
    }
    report.update(validate_prototype_report(report))
    (output_dir / "source-report.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    if report["status"] != "SOURCE_READY_FOR_PLAYER_PROBE":
        raise RuntimeError(report["failures"])


def main() -> int:
    args = _parse_args(sys.argv)
    _build_scene(args.output_dir.resolve(), args.unity_fbx.resolve() if args.unity_fbx else None)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
