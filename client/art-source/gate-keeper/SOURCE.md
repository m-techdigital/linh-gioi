# Người Giữ Cổng — source theo bộ phận

SCN-001, mẫu đích: `docs/reference-art/linh-gioi-story-design-reference-pack-v0.1/images/reference_only/02_gate_keeper_character_sheet.png`. Source chỉnh sửa: `KeeperWardrobe.blend`; bản ghép dẫn xuất: `KeeperReconstruction.blend`; chưa production-final hoặc owner visual approval.

Nguồn ban đầu: ảnh riêng tạo bằng built-in imagegen → Tencent Hunyuan3D-2.1 → Blender. Các macro pass sau dựng lại anatomy, trang phục, đầu/tóc và nón trên rig dùng chung. Board không vào runtime. Source hiện có 73.815 triangles, 37.863 vertices, 65 bones, tối đa4 influences/vertex. Atlas thân4096×2048 dùng chung với player; mặt1024×512. Chưa tối ưu mobile.

FACE `appearance_slot` theo `client/art-source/appearance-slots.json` là nhãn sở hữu: Headwear, Head, Hair, UpperBody, LowerBody, Cape, Gloves, Boots, Shoulders, Accessories. Exporter chuyển nhãn thành10 section; không suy phần trang phục từ trọng số xương hay chiều cao.

```sh
build/toolchains/blender/Blender.app/Contents/MacOS/Blender -b --python-exit-code 1 --python tools/art/export_keeper_reconstruction.py -- --source client/art-source/gate-keeper/KeeperReconstruction.blend --output build/asset-staging/gate-keeper/export
```

Chép FBX và atlas nếu đã thay vào `Assets/Game/Art/OnboardingCandidate/`, chạy Unity Editor `LinhGioi.Foundation.Editor.NpcAppearanceBaker.BakeKeeper`. Baker giữ module/recipe trong `Editor/KeeperParts/`, ghép thành shared mesh nén Medium và preset: một renderer, hai material,41.581 runtime vertices. Prefab không phụ thuộc source FBX hoặc module Editor. Instance có bones riêng, dùng chung mesh/material. Trang phục độc nhất vẫn phải tính geometry/texture; không claim mỗi bộ dưới100KB hay FPS cảnh đông.

Pass nón/tà áo2026-09-08: nón có mặt kín, đỉnh/viền/phù rủ; giảm lõm ngang áo choàng và lùi phần dưới tránh ủng; hint khuỷu khi chỉ đường. `refine_keeper_hat_drape.py --headwear-only` dựng lại nón mà giữ tà áo; chạy không flag vẫn là migration tà áo một lần từ source cũ, không phải bước bắt buộc mỗi export.

Evidence: `build/visual-evidence/keeper-gesture-desktop/review.json`,8 ảnh Player1920×1080 đã xem; import/build thật qua, build393.834.579B/0 lỗi/0 cảnh báo. Capture góc cận và chỉ đường/hạ tay, chưa chứng minh garment đi/chạy hay cloth simulation. Nón nay có chóp rỗng, dải hoa văn cuộn, khoen phù nhưng nét/chất liệu vẫn đơn giản hơn sheet, bàn tay mở theo bind pose khi chỉ hướng, vai dùng trọng số cứng giữ khối nhưng còn cần sửa phom; các vùng texture ủng còn méo. Hash và thông số hiện tại trong `provenance.json`; không dùng ngân sách bản mobile cũ cho bản PC này.

Nguồn công cụ: [Tencent Hunyuan3D-2.1](https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1), [license nguồn](https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1/blob/main/LICENSE).

Pass chuyển động: `refine_keeper_shoulders.py` chuyển1.408 vertices giáp vai sang trọng số cứng, bảo toàn vị trí ở pose hạ tay60°. `NpcGuideGesture` cache góc ngón từ bind matrices của model rồi blend trong IK pass; không thêm bone/animation asset. Chưa kiểm tra mọi hướng đích hoặc cancel.

Ủng2026-09-08: `repair_boot_projection.py` tái ánh xạ mặt bên/sau từ dải giữa mặt trước sạch, giữ atlas/geometry/weights.8 ảnh PC tại `build/visual-evidence/boots-center-desktop/review.json` đã xem; mảng nền đen giảm nhưng còn kéo ngang màu, chưa final. Bước tiếp: bề mặt ủng riêng rồi bake vào atlas, không tiếp thử ngưỡng UV.

Wardrobe2026-09-08: master giữ13 món/object có nhãn, ba vạt áo mới là bề mặt liền với độ dày chỉnh được. `author_keeper_outfit.py` là migration một lần từ c4fac56, không chạy lại trên source đã chuyển. Sửa master rồi dùng `preview_wardrobe.py --source … --output …` để xem5 góc/tư thế; xuất bằng exporter trên master với `--wardrobe-master`. Bản runtime đã kiểm chứng lượt này xuất từ bản ghép dẫn xuất. Donor cũ có vùng trọng số đùi cô lập gây bật tà; ba vạt dùng chuyển tiếp pelvis/đùi liên tục theo chiều cao. Evidence/thông số/giới hạn mới nhất xem `provenance.json`; không coi ảnh960×540 là bằng chứng độ nét1920×1080.
