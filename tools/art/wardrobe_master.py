"""Shared editable garment assembly for Keeper and arrival player exporters."""
import bpy

def join_master(rig, name):
    parts = [obj for obj in bpy.context.scene.objects if obj.type == 'MESH' and obj.asset_data is not None]
    assert parts and all(obj.data.attributes.get('appearance_slot') is not None for obj in parts)
    for part in parts:
        bpy.context.view_layer.objects.active = part
        for modifier in list(part.modifiers):
            if modifier.type != 'ARMATURE':
                bpy.ops.object.modifier_apply(modifier=modifier.name)
    bpy.ops.object.select_all(action='DESELECT')
    for part in parts:
        part.select_set(True)
    bpy.context.view_layer.objects.active = parts[0]
    bpy.ops.object.join()
    mesh = bpy.context.object
    mesh.name = name
    old_materials = list(mesh.data.materials)
    body = bpy.data.materials['Keeper Reconstruction']
    face = bpy.data.materials['Keeper Reconstruction Face']
    assert all(old_materials[polygon.material_index] in (body, face) for polygon in mesh.data.polygons)
    surface_ids = [0 if old_materials[polygon.material_index] == body else 1 for polygon in mesh.data.polygons]
    mesh.data.materials.clear()
    mesh.data.materials.append(body)
    mesh.data.materials.append(face)
    for polygon, surface in zip(mesh.data.polygons, surface_ids):
        polygon.material_index = surface
    assert mesh.find_armature() == rig
    return mesh

def save_joined(mesh, source, output):
    assert output.resolve() != source.resolve(), 'Never overwrite editable source'
    output.parent.mkdir(parents=True, exist_ok=True)
    for obj in list(bpy.context.scene.objects):
        if obj.type == 'MESH' and obj != mesh:
            bpy.data.objects.remove(obj, do_unlink=True)
    mesh.asset_clear()
    bpy.context.preferences.filepaths.save_version = 0
    bpy.ops.wm.save_as_mainfile(filepath=str(output.resolve()), compress=True)
