import unittest
import json
from pathlib import Path

from PIL import Image, ImageDraw

from review_lgo_paper_doll_pack import compare_bind_pose
from pack_lgo_vo_lv1_map_avatar import project_canvas_rect, registration_errors


class RegisteredSpaceTests(unittest.TestCase):
    def test_cropping_does_not_recenter_or_change_ground(self):
        left = project_canvas_rect((300, 1200, 420, 1484))
        right = project_canvas_rect((600, 1200, 720, 1484))
        self.assertAlmostEqual(left["worldW"], right["worldW"])
        self.assertAlmostEqual(0, left["dy"] - left["worldH"] / 2)
        self.assertAlmostEqual(300 / 1536 * 1.7, right["dx"] - left["dx"])

    def test_repacking_and_texture_resolution_do_not_change_world_bounds(self):
        part = {"id": "boot", **project_canvas_rect((300, 1200, 420, 1484)),
                "x": 6, "y": 20, "w": 30, "h": 71}
        self.assertEqual([], registration_errors([part]))
        part.update(x=400, y=600, w=15, h=36)
        self.assertEqual([], registration_errors([part]))

    def test_changed_offset_or_scale_is_rejected_even_with_valid_profile_id(self):
        original = {"id": "boot", **project_canvas_rect((300, 1200, 420, 1484))}
        for key in ("worldW", "worldH", "dx", "dy"):
            with self.subTest(key=key):
                modified = dict(original)
                modified[key] += .01
                self.assertTrue(registration_errors([modified]))

    def test_missing_or_invalid_registration_cannot_pass(self):
        self.assertTrue(registration_errors([{"id": "new-item"}]))
        for box in ((0, 0, 0, 100), (-1, 0, 20, 100), (0, 0, 1025, 1536), (0, 0, float('nan'), 1)):
            with self.subTest(box=box), self.assertRaises(ValueError):
                project_canvas_rect(box)


class BindPoseFitTests(unittest.TestCase):
    def setUp(self):
        self.reference = Image.new("RGBA", (160, 220))
        draw = ImageDraw.Draw(self.reference)
        draw.ellipse((64, 12, 96, 44), fill="#d6a77e")
        draw.rectangle((56, 44, 104, 132), fill="#d3a335")
        draw.rectangle((56, 132, 72, 206), fill="#30343d")
        draw.rectangle((88, 132, 104, 206), fill="#30343d")

    def test_same_registered_outfit_passes(self):
        self.assertTrue(compare_bind_pose(self.reference, self.reference.copy())["passed"])

    def test_recentered_item_bundle_fails_without_bbox_normalization(self):
        shifted = Image.new("RGBA", self.reference.size)
        shifted.alpha_composite(self.reference, (12, 0))
        self.assertFalse(compare_bind_pose(self.reference, shifted)["passed"])

    def test_changed_scale_fails_even_when_ground_is_preserved(self):
        resized = self.reference.resize((192, 264))
        candidate = Image.new("RGBA", self.reference.size)
        candidate.alpha_composite(resized, (-16, -41))
        self.assertFalse(compare_bind_pose(self.reference, candidate)["passed"])

    def test_wrong_outfit_colors_fail_even_with_identical_alpha(self):
        wrong = Image.new("RGBA", self.reference.size, "#1345d7")
        wrong.putalpha(self.reference.getchannel("A"))
        self.assertFalse(compare_bind_pose(self.reference, wrong)["passed"])

    def test_empty_images_are_not_a_success(self):
        with self.assertRaises(ValueError):
            compare_bind_pose(self.reference, Image.new("RGBA", self.reference.size))


class RegisteredBodyIndependenceTests(unittest.TestCase):
    def test_female_inner_top_never_changes_body_pixels(self):
        root = Path(__file__).resolve().parents[1] / "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses"
        equipment = json.loads((root / "VoRegisteredFemaleEquipmentLv1/manifest.json").read_text())
        body = json.loads((root / "VoClosedBodyLv1/manifest.json").read_text())
        variants = {frozenset(p["occlusionState"]): p for p in equipment["parts"] if p["kind"] == "native-body-variant"}
        original = next(p for p in body["parts"] if p["gender"] == "female" and p["kind"] == "native-body")
        with Image.open(root / "VoRegisteredFemaleEquipmentLv1/equipment-atlas.png") as texture, Image.open(root / "VoClosedBodyLv1/closed-body-atlas.png") as body_texture:
            def restore(atlas, part):
                rect = part["sourceCanvasRect"]
                self.assertEqual((rect[2] - rect[0], rect[3] - rect[1]), (part["w"] * 4, part["h"] * 4))
                result = Image.new("RGBA", (256, 384))
                result.paste(atlas.convert("RGBA").crop((part["x"], atlas.height - part["y"] - part["h"], part["x"] + part["w"], atlas.height - part["y"])), (rect[0] // 4, rect[1] // 4))
                return result
            for other in [frozenset(), frozenset(["head_hair"]), frozenset(["lower_garment"]), frozenset(["head_hair", "lower_garment"])]:
                with self.subTest(other=sorted(other)):
                    selected = variants[other | {"inner_top"}]
                    expected = restore(texture, variants[other]) if other else restore(body_texture, original)
                    actual = restore(texture, selected)
                    rect = tuple(v // 4 for v in selected["sourceCanvasRect"])
                    self.assertEqual(expected.crop(rect).tobytes(), actual.crop(rect).tobytes(), "Inner top must select equipment, not rewrite skin")




class RegisteredBindDumpTests(unittest.TestCase):
    def setUp(self):
        import tempfile
        import hashlib
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.pack = self.root / 'client/Unity/Assets/Pack'
        self.pack.mkdir(parents=True)
        self.runtime = self.root / 'client/Unity/Assets/Game/World/Runtime/TwoDRegisteredOutfit.cs'
        paths = ['Assets/Game/World/Runtime/TwoDRegisteredOutfit.cs',
                 'Assets/Game/Tests/EditMode/TwoDRegisteredEquipmentTests.cs',
                 'Assets/Game/World/Runtime/Resources/LGOClasses/VoRegisteredLv1/manifest.json',
                 'Assets/Game/World/Runtime/Resources/LGOClasses/VoRegisteredLv1/anatomical-meshes.json', 'Packages/packages-lock.json']
        self.inputs = []
        for path in paths:
            local = self.root / 'client/Unity' / path; local.parent.mkdir(parents=True, exist_ok=True); local.write_text('fixture')
            self.inputs.append({'path': path, 'sha256': hashlib.sha256(local.read_bytes()).hexdigest()})
        self.source = self.root / 'source.png'
        image = Image.new('RGBA', (256, 384))
        ImageDraw.Draw(image).rectangle((4, 4, 11, 11), fill='#d4a340')
        image.save(self.source)
        atlas = Image.new('RGBA', (16, 16)); atlas.paste(image.crop((4, 4, 12, 12)), (2, 6))
        atlas.save(self.pack / 'atlas.png')
        sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
        part = {'id': 'shirt', 'x': 2, 'y': 2, 'w': 8, 'h': 8, 'source': str(self.source),
                'sourceSha256': sha(self.source), **project_canvas_rect((16, 16, 48, 48))}
        (self.pack / 'manifest.json').write_text(json.dumps({'parts': [part]}))
        self.layer = {'id': 'shirt', 'atlas': 'Assets/Pack/atlas.png', 'order': 1,
                      'atlasSha256': sha(self.pack / 'atlas.png'), 'manifestSha256': sha(self.pack / 'manifest.json'),
                      'points': [{'x': x, 'y': y} for x, y in [(16, 48), (48, 48), (48, 16), (16, 16)]],
                      'uv': [{'x': x / 16, 'y': y / 16} for x, y in [(2, 2), (10, 2), (10, 10), (2, 10)]],
                      'triangles': [0, 1, 2, 0, 2, 3]}

    def review(self):
        from review_lgo_paper_doll_pack import render_registered_bind_dump
        dump = self.root / 'dump.json'
        dump.write_text(json.dumps({'gender': 'male', 'layers': [self.layer], 'inputs': self.inputs}))
        return render_registered_bind_dump(self.root, dump, self.root / 'review')

    def test_original_sources_match_bound_geometry(self):
        self.assertTrue(self.review()['referenceFitPassed'])

    def test_displaced_runtime_vertices_fail_without_recentering(self):
        for p in self.layer['points']:
            p['x'] += 40
        self.assertFalse(self.review()['referenceFitPassed'])

    def test_missing_runtime_triangle_fails(self):
        self.layer['triangles'] = [0, 1, 2]
        self.assertFalse(self.review()['referenceFitPassed'])

    def test_stale_dump_is_rejected(self):
        self.layer['atlasSha256'] = '0' * 64
        with self.assertRaisesRegex(ValueError, 'current atlas'):
            self.review()

    def test_changed_source_is_rejected(self):
        self.source.write_bytes(b'changed')
        with self.assertRaisesRegex(ValueError, 'source fingerprint'):
            self.review()

    def test_changed_runtime_code_rejects_old_geometry(self):
        self.runtime.write_text('changed binding implementation')
        with self.assertRaisesRegex(ValueError, 'current runtime/exporter'):
            self.review()

    def test_missing_runtime_fingerprint_is_rejected(self):
        self.inputs.pop()
        with self.assertRaisesRegex(ValueError, 'Incomplete runtime/exporter'):
            self.review()


if __name__ == "__main__":
    unittest.main()
