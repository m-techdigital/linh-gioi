# Người Giữ Cổng — macro reconstruction candidate

SCN-001, mẫu đích: `docs/reference-art/linh-gioi-story-design-reference-pack-v0.1/images/reference_only/02_gate_keeper_character_sheet.png`. Đây là bản remake để tăng fidelity, chưa production-final hoặc owner visual approval.

Nguồn hình khối: ảnh nhân vật front A-pose riêng do built-in imagegen tạo từ sheet; Tencent Hunyuan3D-2.1 demo chính thức dựng mesh (seed 907, 30 steps, octree 256). Ảnh rear riêng do imagegen tạo phục vụ texture. Board nguyên gốc không được đưa vào runtime. Model được giảm mesh, sửa khối thừa trước mặt, bổ sung khối tóc dài, bake atlas thân 2048 và mặt 512 trong Blender 4.5.13; dùng nguyên rest matrices của bộ xương chung 65 bones, tối đa 4 influences/vertex.

`KeeperReconstruction.blend` là source chỉnh sửa được, chứa mesh, rig và texture đã pack. Export bằng:

```sh
build/toolchains/blender/Blender.app/Contents/MacOS/Blender -b --python-exit-code 1 --python tools/art/export_keeper_reconstruction.py -- --source client/art-source/gate-keeper/KeeperReconstruction.blend --output build/asset-staging/gate-keeper-v4/runtime-export
```

Provenance và điều khoản công cụ: [Tencent Hunyuan3D-2.1](https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1), [license nguồn](https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1/blob/main/LICENSE). File này ghi nguồn thực tế, không chứng nhận art/license production. Input, mesh gốc, log API, recipe macro và ảnh review cục bộ nằm tại `build/asset-staging/gate-keeper-v4/reconstruction-inputs/`.

Khoảng cách còn lại: texture thân chưa sắc như sheet, hoa văn nón còn giản lược, mép cape cần cleanup tiếp; P1 runtime: cape tạo tam giác lên tay khi IK chỉ đường, chưa đạt visual gesture gate; đánh giá hướng guide thân thiện ở camera gameplay, không mở combat/reward/progression.

Validation checkpoint: import Humanoid, build macOS (0 errors/0 warnings), 19/19 EditMode guide/dialogue tests, quick gate và toàn tuyến 960x540 qua. Ảnh Player đã xem tại `build/visual-evidence/onboarding-blockout/keeper-clothed-mobile/`; đây là functional route PASS, không phải fidelity/visual PASS.
