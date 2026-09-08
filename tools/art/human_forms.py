"""Shared authoring forms from pinned CC0 mesh/target data; no runtime dependency.

Stable source vertex IDs allow head, hair and clothing bindings to survive a
form change. These are adult anatomy starting points, not finished NPC designs.
"""
from dataclasses import dataclass
from functools import lru_cache
from pathlib import Path
import hashlib
import json

SEED = Path(__file__).resolve().parents[2] / 'client/art-source/common-human/makehuman'
AGES = {'young': 0.0, 'middle': 0.45, 'elder': 1.0}
BUILDS = {'lean': -0.7, 'balanced': 0.0, 'full': 0.7}

@dataclass(frozen=True)
class HumanForm:
    sex: str = 'male'
    age: str = 'young'
    build: str = 'balanced'
    height: float = 1.79

    def __post_init__(self):
        if self.sex not in ('male', 'female') or self.age not in AGES or self.build not in BUILDS:
            raise ValueError('Unknown authoring form')
        if not 1.4 <= self.height <= 2.1:
            raise ValueError('Adult base height must be within 1.4–2.1 m')

    @property
    def key(self):
        return f'{self.sex}_{self.age}_{self.build}'


def presets():
    return [HumanForm(sex, age, build) for sex in ('male', 'female') for age in AGES for build in BUILDS]


def verify_seed():
    manifest = json.loads((SEED / 'source.json').read_text())
    for name, expected in manifest['files'].items():
        if hashlib.sha256((SEED / name).read_bytes()).hexdigest() != expected:
            raise ValueError('Authoring seed changed: ' + name)
    return manifest


@lru_cache(maxsize=1)
def topology():
    vertices, groups, uvs, face_uvs = [], {}, [], {}
    group = ''
    for line in (SEED / 'base.obj').read_text().splitlines():
        fields = line.split()
        if not fields:
            continue
        if fields[0] == 'v':
            vertices.append(tuple(map(float, fields[1:4])))
        elif fields[0] == 'vt':
            uvs.append(tuple(map(float, fields[1:3])))
        elif fields[0] == 'g':
            group = fields[1]
        elif fields[0] == 'f':
            groups.setdefault(group, []).append(tuple(int(v.split('/')[0]) - 1 for v in fields[1:]))
            face_uvs.setdefault(group, []).append(tuple(int(v.split('/')[1]) - 1 for v in fields[1:]))
    return vertices, groups, uvs, face_uvs


@lru_cache(maxsize=16)
def target(name):
    result = {}
    for line in (SEED / name).read_text().splitlines():
        if not line or line.startswith('#'):
            continue
        fields = line.split()
        result[int(fields[0])] = tuple(map(float, fields[1:4]))
    return result


def raw_coordinates(form):
    vertices = [list(v) for v in topology()[0]]
    age = AGES[form.age]
    weights = {f'asian-{form.sex}-young.target': 1-age, f'asian-{form.sex}-old.target': age}
    build = BUILDS[form.build]
    if build:
        stem = f'universal-{form.sex}-young-averagemuscle-'
        # A delta relative to average preserves the currently accepted balanced base.
        weights[stem + ('min' if build < 0 else 'max') + 'weight.target'] = abs(build)
        weights[stem + 'averageweight.target'] = -abs(build)
        weights['head-fat-' + ('decr' if build < 0 else 'incr') + '.target'] = abs(build)
    for name, weight in weights.items():
        if weight:
            for index, delta in target(name).items():
                for axis in range(3):
                    vertices[index][axis] += delta[axis] * weight
    return vertices


def coordinates(form):
    vertices = raw_coordinates(form)
    body_ids = set(i for face in topology()[1]['body'] for i in face)
    low = min(vertices[i][1] for i in body_ids)
    high = max(vertices[i][1] for i in body_ids)
    scale = form.height / (high-low)
    return [(x*scale, -z*scale, (y-low)*scale) for x,y,z in vertices]


def landmarks(form):
    vertices = coordinates(form)
    result = {'crown': (0, 0, form.height)}
    for name, faces in topology()[1].items():
        if name.startswith('joint-') or name in ('helper-l-eye', 'helper-r-eye'):
            ids = set(i for face in faces for i in face)
            result[name] = tuple(sum(vertices[i][a] for i in ids)/len(ids) for a in range(3))
    return result
