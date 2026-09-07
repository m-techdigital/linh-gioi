"""Shared wardrobe ownership transport for Blender FBX exports."""
import json
from pathlib import Path


def label_sections(mesh):
    # Semantic material sections transport module ownership through FBX without guessing from bone weights or height.
    catalog = json.loads((Path(__file__).resolve().parents[2] / 'client/art-source/appearance-slots.json').read_text())
    parts = mesh.data.attributes.get('appearance_slot')
    assert parts is not None and parts.domain == 'FACE' and parts.data_type == 'INT', 'Label every face with its wardrobe slot before export'
    part_ids = [entry.value for entry in parts.data]
    material_ids = [polygon.material_index for polygon in mesh.data.polygons]
    assert all(str(part) in catalog for part in part_ids), 'Unknown or unassigned wardrobe slot'
    source_materials = list(mesh.data.materials)
    assert [material.name for material in source_materials] == ['Keeper Reconstruction', 'Keeper Reconstruction Face'], 'Expected shared Body/Face material order'
    sections = sorted(set(zip(part_ids, material_ids)))
    mesh.data.materials.clear()
    for part, surface in sections:
        material = source_materials[surface].copy()
        material.name = 'LGO_' + catalog[str(part)]['key'] + ('_Face' if surface == 1 else '_Body')
        mesh.data.materials.append(material)
    section_indices = {section: index for index, section in enumerate(sections)}
    for polygon in mesh.data.polygons:
        polygon.material_index = section_indices[(part_ids[polygon.index], material_ids[polygon.index])]
    return sections, [catalog[str(part)]["key"] for part in sorted(set(part_ids))]
