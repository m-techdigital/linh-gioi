# Người Giữ Cổng — candidate dùng chung theo bộ phận

SCN-001, mẫu đích: `docs/reference-art/linh-gioi-story-design-reference-pack-v0.1/images/reference_only/02_gate_keeper_character_sheet.png`. Source chỉnh sửa: `KeeperReconstruction.blend`; chưa production-final hoặc owner visual approval.

Nguồn: ảnh nhân vật riêng do built-in imagegen tạo từ sheet → Tencent Hunyuan3D-2.1 shape reconstruction (seed 907, 30 steps, octree 256) → Blender cleanup, nón mới, tóc và garment weights. Board không vào runtime. Source giữ texture gốc để chỉnh sửa; runtime xuất thân 512 và mặt 256. Giữ rest matrices 65 bones, tối đa 4 influences/vertex, 24.400 triangles.

Thuộc tính FACE `npc_part` trong Blender: 1 = nón và tua; 2 = đầu và toàn bộ tóc dài; 3 = thân/trang phục. Exporter chuyển nhãn thành các section FBX; face atlas dùng section riêng. Không suy ngược bộ phận từ trọng số xương hoặc độ cao.

```sh
build/toolchains/blender/Blender.app/Contents/MacOS/Blender -b --python-exit-code 1 --python tools/art/export_keeper_reconstruction.py -- --source client/art-source/gate-keeper/KeeperReconstruction.blend --output build/asset-staging/gate-keeper-v4/optimized-export
```

Sau khi chép FBX và 2 atlas vào `Assets/Game/Art/OnboardingCandidate/`, chạy Unity Editor `LinhGioi.Foundation.Editor.NpcAppearanceBaker.BakeKeeper` (hoặc entrypoint `ArrivalOutfitImporter.ImportKeeper`). Baker tạo module/recipe dưới `Editor/KeeperParts/`, ghép trước thành một mesh nén Medium và preset chung. `NpcAppearanceInstance` chỉ gắn shared mesh/material cùng bone mapping vào instance; không clone mesh hoặc material cho mỗi NPC. Source FBX và module Editor bị kiểm tra loại khỏi dependency của prefab runtime. `Merge(recipe)` hỗ trợ ghép lựa chọn module tương thích cùng bind pose/material layout.

Hiện có ba nhóm; thân và trang phục vẫn là một module, chưa phải thư viện áo/quần/giày tách hoàn chỉnh. Preset dùng lại không phát sinh bộ texture/mesh mới; bộ trang phục độc nhất vẫn phải tính phần geometry/texture của nó. Không claim mỗi bộ dưới 100 KB hoặc FPS cho cảnh đông người.

Đã sửa phom nón, loại mảnh thừa, thu bớt độ xòe tà và tách ảnh hưởng IK tay khỏi vùng cape. Mép vải/hoa văn còn cần art polish; chưa cloth simulation hoặc xác nhận garment đi/chạy. Video trước/sau và capture local ở `build/visual-evidence/keeper-*-optimization/` và `build/visual-evidence/onboarding-blockout/keeper-optimized-verified-mobile/`.

Nguồn công cụ: [Tencent Hunyuan3D-2.1](https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1), [license nguồn](https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1/blob/main/LICENSE), [Unity mesh compression API](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MeshUtility.SetMeshCompression.html). Ghi nguồn không thay cho kiểm tra license production.
