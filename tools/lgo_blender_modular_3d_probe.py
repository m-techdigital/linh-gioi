#!/usr/bin/env python3
"""Build and audit a bounded direct-3D modular-character Blender probe.

The generated character is synthetic engineering evidence. It proves the local
Blender -> GLB data path and module semantics, never LGO visual quality.
"""

import argparse
import json
import struct
import sys
from pathlib import Path


def probe_contract():
    return {
        "gateId": "LGO_MODULAR_3D_TOOLCHAIN_PROBE_01",
        "renderPath": "DIRECT_3D_RUNTIME",
        "equipmentSlots": ["upper", "lower", "footwear", "waist", "rigid_hand_item"],
        "actions": ["idle", "run", "jump", "attack", "return_to_idle"],
    }


def validate_probe_report(report):
    contract = probe_contract()
    failures = []
    if report.get("renderPath") != contract["renderPath"]:
        failures.append("NOT_DIRECT_3D_RUNTIME")
    if not set(contract["equipmentSlots"]) <= set(report.get("equipmentSlots", [])):
        failures.append("MISSING_REQUIRED_EQUIPMENT_SLOTS")
    if not set(contract["actions"]) <= set(report.get("actions", [])):
        failures.append("MISSING_REQUIRED_ACTIONS")
    if report.get("armature", {}).get("boneCount", 0) < 1:
        failures.append("ARMATURE_MISSING")
    for garment in report.get("softGarments", []):
        if not garment.get("hasArmatureModifier") or not garment.get("vertexGroups"):
            failures.append("SOFT_GARMENT_MISSING_AUTHORED_WEIGHTS")
            break
    rigid = report.get("rigidHandItem", {})
    if rigid.get("hasArmatureModifier"):
        failures.append("RIGID_ITEM_USES_SOFT_DEFORMATION")
    if not rigid.get("parentBone"):
        failures.append("RIGID_ITEM_SOCKET_MISSING")
    artifact = report.get("artifact", {})
    if not artifact.get("glbExists") or artifact.get("glbBytes", 0) <= 0:
        failures.append("GLB_EXPORT_MISSING")
    if not artifact.get("fbxExists") or artifact.get("fbxBytes", 0) <= 0:
        failures.append("UNITY_FBX_EXPORT_MISSING")
    if artifact.get("animationCount", 0) < len(contract["actions"]):
        failures.append("GLB_ANIMATIONS_MISSING")
    return {
        "status": "NARROW_TECHNICAL_PASS_TOOLCHAIN_ONLY" if not failures else "REJECT_TECHNICAL_PROBE",
        "failures": failures,
        "runtimePromotionAllowed": False,
    }


def _glb_json(path):
    data = Path(path).read_bytes()
    if len(data) < 20 or data[:4] != b"glTF":
        raise ValueError("not a GLB file")
    _, version, total_length = struct.unpack_from("<4sII", data, 0)
    if version != 2 or total_length != len(data):
        raise ValueError("invalid GLB header")
    chunk_length, chunk_type = struct.unpack_from("<II", data, 12)
    if chunk_type != 0x4E4F534A:
        raise ValueError("GLB first chunk is not JSON")
    return json.loads(data[20 : 20 + chunk_length].decode("utf-8").rstrip(" \t\r\n\0"))


def _parse_blender_args(argv):
    arguments = argv[argv.index("--") + 1 :] if "--" in argv else argv[1:]
    parser = argparse.ArgumentParser()
    parser.add_argument("--output-dir", required=True, type=Path)
    return parser.parse_args(arguments)


def _make_box(bpy, name, location, scale, material):
    bpy.ops.mesh.primitive_cube_add(location=location)
    obj = bpy.context.object
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def _make_material(bpy, name, color):
    material = bpy.data.materials.new(name)
    material.diffuse_color = (*color, 1.0)
    return material


def _bone_parent(obj, armature, bone_name):
    obj.parent = armature
    obj.parent_type = "BONE"
    obj.parent_bone = bone_name


def _soft_bind(obj, armature, weights):
    obj.parent = armature
    modifier = obj.modifiers.new("Armature", "ARMATURE")
    modifier.object = armature
    indices = list(range(len(obj.data.vertices)))
    for bone_name, weight in weights.items():
        group = obj.vertex_groups.new(name=bone_name)
        group.add(indices, weight, "REPLACE")


def _build_scene(output_dir):
    import bpy

    output_dir.mkdir(parents=True, exist_ok=True)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    for block in bpy.data.actions:
        bpy.data.actions.remove(block)

    armature_data = bpy.data.armatures.new("LGO_Modular3D_Armature")
    armature = bpy.data.objects.new("LGO_Modular3D_Armature", armature_data)
    bpy.context.collection.objects.link(armature)
    bpy.context.view_layer.objects.active = armature
    armature.select_set(True)
    bpy.ops.object.mode_set(mode="EDIT")

    bones = {
        "root": ((0, 0, 0), (0, 0, 0.2), None),
        "pelvis": ((0, 0, 0.2), (0, 0, 0.7), "root"),
        "spine": ((0, 0, 0.7), (0, 0, 1.35), "pelvis"),
        "head": ((0, 0, 1.35), (0, 0, 1.75), "spine"),
        "upper_arm.L": ((0, 0, 1.28), (-0.42, 0, 1.18), "spine"),
        "forearm.L": ((-0.42, 0, 1.18), (-0.75, 0, 0.98), "upper_arm.L"),
        "hand.L": ((-0.75, 0, 0.98), (-0.92, 0, 0.9), "forearm.L"),
        "upper_arm.R": ((0, 0, 1.28), (0.42, 0, 1.18), "spine"),
        "forearm.R": ((0.42, 0, 1.18), (0.75, 0, 0.98), "upper_arm.R"),
        "hand.R": ((0.75, 0, 0.98), (0.92, 0, 0.9), "forearm.R"),
        "thigh.L": ((-0.16, 0, 0.28), (-0.16, 0, -0.35), "pelvis"),
        "shin.L": ((-0.16, 0, -0.35), (-0.16, 0, -0.95), "thigh.L"),
        "thigh.R": ((0.16, 0, 0.28), (0.16, 0, -0.35), "pelvis"),
        "shin.R": ((0.16, 0, -0.35), (0.16, 0, -0.95), "thigh.R"),
        "weapon_socket.R": ((0.92, 0, 0.9), (1.05, 0, 0.86), "hand.R"),
    }
    for name, (head, tail, parent) in bones.items():
        bone = armature_data.edit_bones.new(name)
        bone.head, bone.tail = head, tail
        if parent:
            bone.parent = armature_data.edit_bones[parent]
    bpy.ops.object.mode_set(mode="POSE")
    for pose_bone in armature.pose.bones:
        pose_bone.rotation_mode = "XYZ"
    bpy.ops.object.mode_set(mode="OBJECT")

    skin = _make_material(bpy, "Skin", (0.52, 0.28, 0.18))
    cloth = _make_material(bpy, "UpperCloth", (0.12, 0.24, 0.42))
    lower_mat = _make_material(bpy, "LowerCloth", (0.09, 0.1, 0.15))
    gold = _make_material(bpy, "RigidGold", (0.72, 0.43, 0.08))

    body_parts = [
        _make_box(bpy, "body_torso_proxy", (0, 0, 1.0), (0.28, 0.16, 0.38), skin),
        _make_box(bpy, "body_head_proxy", (0, 0, 1.58), (0.2, 0.18, 0.22), skin),
    ]
    _bone_parent(body_parts[0], armature, "spine")
    _bone_parent(body_parts[1], armature, "head")

    upper = _make_box(bpy, "equipment__upper", (0, -0.03, 1.02), (0.31, 0.2, 0.4), cloth)
    lower = _make_box(bpy, "equipment__lower", (0, -0.02, 0.4), (0.29, 0.19, 0.28), lower_mat)
    _soft_bind(upper, armature, {"spine": 1.0})
    _soft_bind(lower, armature, {"pelvis": 1.0})

    waist = _make_box(bpy, "equipment__waist", (0, -0.23, 0.67), (0.34, 0.04, 0.07), gold)
    _bone_parent(waist, armature, "pelvis")
    for side, x in (("L", -0.16), ("R", 0.16)):
        shoe = _make_box(bpy, f"equipment__footwear_{side}", (x, -0.05, -1.02), (0.16, 0.3, 0.08), lower_mat)
        _bone_parent(shoe, armature, f"shin.{side}")
    weapon = _make_box(bpy, "equipment__rigid_hand_item", (1.18, 0, 0.84), (0.34, 0.035, 0.035), gold)
    _bone_parent(weapon, armature, "weapon_socket.R")

    action_poses = {
        "idle": {"spine": (0.0, 0.0, 0.03)},
        "run": {"thigh.L": (0.55, 0.0, 0.0), "thigh.R": (-0.55, 0.0, 0.0), "upper_arm.R": (-0.4, 0.0, 0.0)},
        "jump": {"thigh.L": (0.75, 0.0, 0.0), "thigh.R": (0.75, 0.0, 0.0), "shin.L": (-0.8, 0.0, 0.0), "shin.R": (-0.8, 0.0, 0.0)},
        "attack": {"upper_arm.R": (0.0, -0.9, 0.2), "forearm.R": (0.0, -0.45, 0.0)},
        "return_to_idle": {"upper_arm.R": (0.0, -0.75, 0.1)},
    }
    for action_name, rotations in action_poses.items():
        action = bpy.data.actions.new(action_name)
        armature.animation_data_create()
        armature.animation_data.action = action
        for pose_bone in armature.pose.bones:
            pose_bone.rotation_euler = (0, 0, 0)
            pose_bone.keyframe_insert("rotation_euler", frame=1, group=pose_bone.name)
        for bone_name, rotation in rotations.items():
            armature.pose.bones[bone_name].rotation_euler = rotation
        for pose_bone in armature.pose.bones:
            pose_bone.keyframe_insert("rotation_euler", frame=12, group=pose_bone.name)
        action.use_fake_user = True
    armature.animation_data.action = None

    blend_path = output_dir / "modular-3d-technical-probe.blend"
    glb_path = output_dir / "modular-3d-technical-probe.glb"
    fbx_path = output_dir / "modular-3d-technical-probe.fbx"
    bpy.ops.wm.save_as_mainfile(filepath=str(blend_path))
    bpy.ops.export_scene.gltf(
        filepath=str(glb_path),
        export_format="GLB",
        export_animations=True,
        export_animation_mode="ACTIONS",
    )
    bpy.ops.export_scene.fbx(
        filepath=str(fbx_path),
        use_selection=False,
        add_leaf_bones=False,
        bake_anim=True,
        bake_anim_use_all_actions=True,
    )
    glb = _glb_json(glb_path)
    soft_garments = []
    for obj in (upper, lower):
        soft_garments.append({
            "slot": obj.name.split("__", 1)[1],
            "hasArmatureModifier": any(mod.type == "ARMATURE" for mod in obj.modifiers),
            "vertexGroups": [group.name for group in obj.vertex_groups],
        })
    report = {
        **probe_contract(),
        "blenderVersion": bpy.app.version_string,
        "armature": {
            "boneCount": len(armature.data.bones),
            "bones": sorted(bone.name for bone in armature.data.bones),
            "bodyBoneLengthDriftRatio": 0.0,
            "usesBoneScaleAnimation": False,
        },
        "softGarments": soft_garments,
        "rigidHandItem": {
            "slot": "rigid_hand_item",
            "parentBone": weapon.parent_bone,
            "hasArmatureModifier": any(mod.type == "ARMATURE" for mod in weapon.modifiers),
        },
        "artifact": {
            "blend": str(blend_path),
            "glb": str(glb_path),
            "glbExists": glb_path.is_file(),
            "glbBytes": glb_path.stat().st_size if glb_path.is_file() else 0,
            "fbx": str(fbx_path),
            "fbxExists": fbx_path.is_file(),
            "fbxBytes": fbx_path.stat().st_size if fbx_path.is_file() else 0,
            "animationCount": len(glb.get("animations", [])),
            "nodeCount": len(glb.get("nodes", [])),
            "skinCount": len(glb.get("skins", [])),
        },
        "claimLimit": "Synthetic toolchain/export evidence only; no Unity import, Player motion, LGO art, unseen-item, performance, or visual approval.",
    }
    report.update(validate_probe_report(report))
    (output_dir / "report.json").write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    if report["status"] != "NARROW_TECHNICAL_PASS_TOOLCHAIN_ONLY":
        raise RuntimeError(report["failures"])


def main():
    args = _parse_blender_args(sys.argv)
    _build_scene(args.output_dir.resolve())


if __name__ == "__main__":
    main()
