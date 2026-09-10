# Linh Giới Online — 2D Production Workflow Research v0.1

Ngày cập nhật: 2026-09-10
Phạm vi: nhánh `feature/2d`, bỏ 3D/Meshy, ưu tiên map/NPC/trang bị 2D có thể lên runtime nhanh nhưng không phá North Star Social Action MMORPG.

## Kết luận vận hành

Không tiếp tục dựng art runtime bằng cách chỉnh từng khối thủ công. Từ batch tiếp theo, pipeline chuẩn là:

1. Chốt module trước khi polish: `tile`, `prop`, `npc`, `equipment`, `vfx`, `ui-chip`.
2. Mỗi module có source data riêng: id, slot/layer, anchor, palette, tags an toàn, level/route/context.
3. Runtime chỉ đọc data và render layer/slot; không hardcode từng mẫu mới trong controller.
4. Visual checkpoint phải capture Player thật bằng tool 2D chuẩn, rồi review ảnh trước khi claim pass.
5. Art blockout chỉ được dùng để kiểm layout/flow. Khi đã cần “giống ảnh”, chuyển sang spritesheet/atlas/PSD-layer workflow, không kéo dài việc ghép hình bằng rectangle primitive.

## Bài học từ nguồn ngoài

- Unity khuyến nghị Tilemap để giảm scene size, serialization overhead, renderer overhead và batching cost cho game 2D. Vì vậy Đông Môn/Linh Thành nên đi theo chunk tilemap + prop layer, không spawn quá nhiều GameObject riêng lẻ cho mỗi mảng nền.
- Sprite Atlas nên gom các sprite thường xuất hiện cùng scene vào cùng atlas, đồng thời tách atlas nhỏ theo usage chung. Với Linh Giới: `dong-mon-environment`, `linh-thanh-common`, `npc-gatekeeper`, `starter-equipment`, `ui-icons` là các cụm hợp lý.
- 2D paper-doll/avatar system là mô hình phù hợp cho MMO có đổi tóc/mắt/trang bị: mỗi bộ phận hoặc trang bị là layer/sprite riêng, runtime xếp theo slot thay vì vẽ sẵn mọi tổ hợp. Điều này giảm số asset cần vẽ và hỗ trợ mix-match quần/áo/phụ kiện.
- Spine skin/attachment workflow cho thấy cách production tốt: có template skin placeholders trước, sau đó gắn attachment cho từng skin. Nếu sau này dùng skeletal 2D, Linh Giới nên map slot hiện tại sang placeholder chuẩn như `hair_front`, `eyes`, `torso`, `pants`, `boots`, `weapon`, `back`, `pet`.
- Unity PSD Importer có character rig mode tạo Prefab từ layer của file nguồn. Khi bắt đầu làm art thật, nên yêu cầu output dạng layered PSD/PSB hoặc spritesheet tách layer để import có hệ thống, thay vì chỉ dùng ảnh concept phẳng.
- MapleStory là ví dụ đúng hướng sản phẩm: 2D side-scrolling MMORPG, world nhiều vùng, NPC/item/quest/social/trade/guild là vòng đời chính. Linh Giới nên ưu tiên map hub + NPC + item/equipment visible sớm trước khi mở hệ backend lớn.

## Quy tắc áp dụng cho Linh Giới từ batch sau

### Map

- Dùng 5 lớp cố định: sky/fog, far background, mid background, gameplay tilemap, foreground.
- Không thêm chi tiết random. Mỗi prop phải có id, anchor, layer, role và route context.
- Đông Môn cần ưu tiên: cổng thành, bia luyện khí, cầu gỗ, thác/nước linh, rừng ngoài thành, slime arena.
- Linh Thành cần ưu tiên: quảng trường, học viện, thương phố, đền linh, khu rèn, bang hội, cảng.

### NPC

- NPC phải là data-driven sprite source, không hardcode riêng từng NPC.
- NPC blockout hợp lệ chỉ để giữ flow; khi polish, mỗi NPC cần silhouette rõ: đầu/tóc/mũ, thân áo, tay/đạo cụ, bóng, phụ kiện nhận diện.
- Người Giữ Cổng batch hiện tại đã có `DongMonNpcSprites.json`; bước tiếp theo là thay màu/hình bằng spritesheet/atlas thật cho 13 part hiện có.

### Trang bị/nhân vật

- Giữ paper-doll slots: base, eyes, hair_back, hair_front, torso/top, pants, waist, gloves, boots, weapon, back, accessory, pet.
- Mỗi item có icon hành trang và layer mặc trên người. Không tạo item chỉ có icon mà không có runtime preview.
- Nam/nữ dùng chung quy chuẩn slot/anchor, nhưng asset riêng cho form và silhouette.

### Kiểm thử/evidence

- Player visual 2D dùng `tools/capture_lgo_2d_onboarding_visual.py`. Lý do: macOS Player cần launch từ `Contents/MacOS`; chạy trực tiếp từ repo root có thể init engine rồi thoát trước khi `GameBootstrap` ghi manifest.
- Bắt buộc qua: EditMode, 2D smoke, macOS Player build, 2D visual capture, `lgo_runtime_smoke_matrix --phase two-d`, `lgo_visual_evidence_matrix --verify-current`, no-3D/no-source-image guards.
- Không dùng full M4 visual runtime khi chỉ thay map/NPC 2D onboarding, trừ khi thay đổi chạm login/session/combat full route. Điều này tránh build cao không cần thiết.

## Hiệu chỉnh theo ảnh owner gửi ngày 2026-09-10

Owner chỉ ra nguy cơ thành phẩm không đạt ảnh gameplay illustrated vừa gửi trong chat. Ảnh có thành phố tiên hiệp xanh/vàng nhiều lớp, kiến trúc xa giàu chi tiết, sàn đá side-view rõ ràng, nhân vật anime có tỷ lệ và trang phục chi tiết, foreground tối tạo chiều sâu, kiếm khí xanh và HUD chia vùng ở viền màn hình. Đây là chuẩn thị giác tham chiếu, không phải runtime asset hoặc bằng chứng các tính năng trong ảnh đã được mở gate.

Runtime hiện tại vẫn là blockout. Polish shape của Người Giữ Cổng chỉ đóng checkpoint kỹ thuật; không tiếp tục thêm rectangle/primitive để tiến tới chất lượng ảnh. Tilemap/Sprite Atlas là phương tiện tổ chức/render, không thay thế công việc mỹ thuật.

Batch tiếp theo: một góc Đông Môn trong Player bằng art mới, trước khi nhân rộng map.

- Reuse scenario tutorial Đông Môn: đứng tại cổng → tiến gần Người Giữ Cổng → thoại. Không mở thêm combat, reward hoặc hệ thống HUD chỉ vì chúng xuất hiện trong ảnh.
- Làm design/demo có nhãn draft trước: bố cục camera 1280×720, các lớp xa/giữa/gần/foreground, gameplay plane, khoảng trống HUD, tỷ lệ NPC và vùng tương tác. Phân biệt asset đề xuất và runtime hiện có.
- Tạo background illustrated mới theo lớp; tileset nền và prop atlas sạch cho cổng/đèn/cờ/đá. Không crop ảnh tham chiếu hoặc dùng nguyên concept làm nền runtime. Mỗi asset cần source/provenance, kích thước, anchor, layer, import settings và budget.
- Giữ ownership: tab 5 class phụ trách nhân vật/trang bị/class art; batch Đông Môn chỉ map/prop/NPC đã thống nhất, không ghi đè source của tab kia.
- Gate mỹ thuật: đặt capture Player cạnh ảnh tham chiếu, đánh giá bố cục, chiều sâu, palette/ánh sáng, tỷ lệ nhân vật, độ chi tiết/coherence và khả năng đọc HUD. Test kỹ thuật pass không thay thế gate này; không ghi production-art PASS khi vẫn chỉ có blockout.
- Khi chưa đạt, sửa art/demo của cùng góc nhìn thay vì nhân rộng tile/prop hoặc thêm feature. Chỉ nhân rộng sau khi owner duyệt hướng bằng capture thật.

## Mốc thời gian thực tế để đạt “giống ảnh”

- 0.5–1 ngày: placeholder đẹp hơn, silhouette NPC/map rõ hơn, vẫn là blockout.
- 3–5 ngày: một map Đồng Môn playable có tileset/prop/NPC basic đẹp tương đối, đủ duyệt hướng.
- 1–2 tuần: vertical slice gần board concept cho Đồng Môn: parallax, tileset coherent, NPC sprite tốt, UI map/minimap ổn, một vài animation/vfx.
- Nhiều tuần đến vài tháng: toàn bộ world map + Linh Thành + nhiều vùng có chất lượng đồng đều như concept.

## Nguồn đã tham khảo

- Unity — Optimize performance of 2D games with Unity Tilemap: https://unity.com/how-to/optimize-performance-2d-games-unity-tilemap
- Unity Manual — Sprite Atlas workflow: https://docs.unity3d.com/2022.2/Documentation/Manual/SpriteAtlasWorkflow.html
- Unity Blog — 2D Tilemap asset workflow from image to level: https://unity.com/blog/games/2d-tilemap-asset-workflow-from-image-to-level
- Unity Documentation — 2D PSD Importer summary: https://docs.unity3d.com/Packages/com.unity.2d.psdimporter@3.0/manual/index.html
- Spine — Mix and Match / skins and attachments: https://en.esotericsoftware.com/spine-unity-mix-and-match
- RPG Maker forum — paper-doll explanation as separate equipment sprite sheets overlaid on character: https://forums.rpgmakerweb.com/index.php?threads/paperdolls-is-it-possible.46337/
- MapleStory overview as 2D side-scrolling MMORPG reference: https://en.wikipedia.org/wiki/MapleStory

## Chuẩn base-first dùng từ CLASS-VO-01B2 — 2026-09-10

Mọi class dùng một contract chung theo thứ tự `base skeleton → bone hierarchy → slot placeholder → attachment → skin/loadout → animation/action`. ID class, giới tính, cấp và item chỉ là dữ liệu; không tạo renderer, action handler hoặc layout riêng bằng cách sao chép controller. Runtime chung đầu tiên là `TwoDSkeletalPaperDollRig`: dựng toàn bộ bone trước, nối parent sau, attachment làm con trực tiếp của bone và pose lưu rotation cục bộ so với parent.

- Character: một skeleton topology cho nam/nữ nếu joint contract tương thích; asset hình riêng được phép nhưng ID bone/slot phải giống nhau. Đồ đôi khai báo hai attachment trái/phải; đồ một khối vẫn đi qua cùng attachment path. Equip/unequip chỉ đổi visibility/loadout state, không sinh texture mới.
- Action: input PC/touch map về action ID chung (`move/run/jump/basic/class_skill/equip/toggle_panel`). Class config chỉ đổi animation clip, timing, range, damage và VFX; không tạo nhánh UI/input riêng cho từng class.
- UI/UX: dùng base panel và responsive rules hiện có; mobile/tablet/PC thay profile kích thước, wrap/safe-area và mật độ, không nhân ba layout hierarchy. Class skin chỉ cung cấp nội dung/icon/theme hợp lệ trong design token đã khóa.
- Asset: atlas theo nhóm cùng tải; source ở kích thước gần runtime, 512–1024 cho sheet hiện tại; không upscale rồi downsample. Variant thiết bị chỉ dùng khi profiling chứng minh cần. Pack Võ hiện giữ 7 PNG/724.795 byte dù tăng từ 32 lên 96 attachment metadata.

Bài học bắt buộc không lặp:

1. Tách một ảnh đôi ở chính giữa chỉ giải quyết găng/giày; các slot còn lại vẫn đứng ngoài skeleton. Mọi slot phải đi qua attachment-to-bone contract.
2. Xoay từng sprite quanh pivot độc lập làm cẳng tay rời bắp tay và cẳng chân rời đùi. Bone phải có parent hierarchy thật.
3. Rotation pose cũ là góc world; gán trực tiếp thành local làm góc con cộng dồn quá mức. Packer phải chuyển `local = world - parentWorld`; unit test tái dựng góc world để khóa lỗi này.
4. Capture thay đồ có state nối tiếp. Sau frame chứng minh tháo slot phải mặc lại trước frame progression/full outfit, nếu không ảnh review sau đó gây hiểu sai chất lượng bộ đồ.
5. Technical capture PASS chỉ xác nhận state/schema. Capture v1 vẫn cho thấy nữ Lv30 thiếu coherence; chỉ v2 sau hierarchy/local rotation mới được dùng làm evidence kỹ thuật. Art attachment vẫn cần source rig-compatible, chưa claim production-final.

Tham khảo kỹ thuật chính: [Spine skins/skin placeholders](https://en.esotericsoftware.com/spine-skins), [Spine attachments follow bones through slots](https://en.esotericsoftware.com/spine-attachments), [spine-unity mix-and-match](https://en.esotericsoftware.com/spine-unity-mix-and-match), [Unity Sprite Atlas](https://docs.unity3d.com/Manual/class-SpriteAtlas.html). Với wardrobe hữu hạn, prepack attachment vào atlas; không runtime-repack mặc định vì tăng cấp phát texture/cache và làm quản lý memory khó dự đoán.

## Quyết định production dựa trên Unity/Spine — 2026-09-10

Đối chiếu lại tài liệu chính thức cho thấy contract hiện tại đúng hướng, nhưng cần khóa cách áp dụng để các phiên sau không tự thử nhiều pipeline cùng lúc:

- `slotId + itemId` của manifest tương đương `Category + Label` trong Unity Sprite Library. Một loadout chỉ đổi attachment của slot; animation chỉ tác động bone/transform. Không tạo Animator riêng theo bộ đồ và không trộn clip điều khiển trực tiếp `SpriteRenderer.sprite` với clip điều khiển hash của `SpriteResolver` trong cùng controller.
- Mọi sprite có skinning được thay thế trong cùng slot phải dùng cùng skeleton topology, bone ID, pivot và bind pose. Validator phải chặn attachment thiếu bone hoặc khác anchor trước khi build Player; mắt người chỉ dùng để duyệt silhouette/coherence.
- PSB/PSD phân lớp là định dạng nguồn sản xuất khi illustrator bàn giao rig art. Runtime chỉ nhận sprite/atlas đã pack và manifest; không đưa file nguồn nhiều layer vào build. Chỉ cài `2D Animation`/`PSD Importer` trong một batch migration riêng sau khi chứng minh import giữ đúng contract hiện có.
- Atlas được chia theo residency, không theo từng ảnh hoặc từng thiết bị: `map-01a-environment`, `map-01a-actors`, `class-vo-common`, `class-vo-tier-001|010|020|030`, `ui-common`. Ba profile mobile/tablet/PC dùng cùng asset logical; platform override/variant chỉ thêm khi profiler cho thấy memory hoặc chất lượng cần khác.
- Runtime chỉ giữ map hiện tại, class hiện tại và tier đang dùng/gần kề. Khi chuyển map/class/tier phải giải phóng handle của nhóm cũ. `Read/Write Enabled` mặc định tắt vì Unity tạo thêm bản sao texture trong memory; không runtime-repack wardrobe hữu hạn.
- Repository hiện chưa có package `2D Animation`, `PSD Importer` hoặc `Addressables`. Vì vậy checkpoint Võ tiếp tục dùng manifest + atlas + `TwoDSkeletalPaperDollRig`; không đổi dependency giữa batch art/equipment. Bước tái cấu trúc kế tiếp chỉ tách orchestration dùng chung và giữ nguyên output đã capture.
- Tool xử lý ảnh phải preflight `import PIL` trước khi chạy batch. Trên máy hiện tại `python3.12` dùng cho compile/validator không có Pillow, còn `/usr/bin/python3` có Pillow 11.3.0; unit test ảnh phải dùng interpreter có Pillow hoặc một image-tool venv đã pin, không cài dependency ngẫu hứng giữa checkpoint.
- Unity 6 có thể reserialize `.meta` của texture Map01A trong EditMode/smoke dù asset không đổi. Sau gate phải đối chiếu và restore churn này trước `diff --check`; không commit import metadata ngoài scope chỉ vì Editor đã chạm file.

Nguồn chính thức: [Unity Sprite Swap](https://docs.unity3d.com/Packages/com.unity.2d.animation@10.0/manual/SpriteSwapIntro.html), [Unity Sprite Library Asset](https://docs.unity3d.com/Packages/com.unity.2d.animation@10.0/manual/SLAsset.html), [Unity PSD Importer](https://docs.unity3d.com/Packages/com.unity.2d.psdimporter@9.0/manual/PSD-importer-properties.html), [Unity Sprite Atlas properties](https://docs.unity3d.com/Manual/class-SpriteAtlas.html), [Unity Addressables memory management](https://docs.unity3d.com/Packages/com.unity.addressables@2.7/manual/MemoryManagement.html), [Spine skins](https://en.esotericsoftware.com/spine-skins).

## Garment attachment batch Võ Lv1–30 — checkpoint 2026-09-10

- Nguồn chuẩn được gom một lượt thành 8 sheet tại `LGO-Selected-2D-Source-v1/class-work-in-progress/vo-lv001/generated-batch-v6/source-v1/`, tên `vo-lv{001|010|020|030}-{male|female}-attachment-sheet.png`. Mỗi sheet 1448×1086, layout cố định 4×3 và có manifest hash riêng; không đưa sheet nguồn vào runtime/repository.
- Một equipment slot có thể sở hữu nhiều attachment. `light_armor`, `lower_garment`, `arm_guard`, `boots` dùng cặp trái/phải theo bone; `head_hair`, `outer_tunic`, `main_weapon`, `waist` dùng component riêng; `inner_top` và `accessory` giữ layer nhỏ độc lập. Không thiết kế base theo giả định “một slot = một sprite”.
- Nền generated sheet có gradient nhẹ dù prompt yêu cầu màu phẳng. Không dùng global chroma key vì sẽ ăn vào tóc/áo đen; pipeline chỉ flood-fill vùng navy liên thông từ viền từng cell, sau đó crop alpha bounds và scale một lần theo chiều cao bone chuẩn.
- Atlas v8 có 112 attachment metadata cho 4 tier × 2 giới và vẫn chỉ dùng 7 texture 1024². Tổng PNG 885.234 byte, tăng 160.439 byte so với v7 nhưng thay toàn bộ garment silhouette lớn; không tạo texture riêng theo action hoặc theo thiết bị.
- Evidence `build/vo-garment-v1/three-profiles/` đạt 46 frame trên mobile/tablet/PC. Review mắt xác nhận Võ nữ Lv30 đã có silhouette trang phục rõ trong run/jump/basic/Liên Quyền. Chưa đóng class vì cần capture ma trận cởi/mặc đủ 10 slot; trạng thái hiện tại `VO_LV1_30_GARMENT_BATCH_PLAYER_PASS / TEN_SLOT_VISUAL_MATRIX_PENDING`.

## Ten-slot visual matrix và base head v9 — checkpoint 2026-09-10

- Test renderer count không phát hiện tóc bake trong base head. Matrix tháo từng slot cho thấy `head_hair` đã disabled nhưng tóc vẫn hiện; base body phải chứa anatomy/underlayer trung tính, mọi phần tùy biến như tóc phải là attachment.
- Đã tạo base head không tóc riêng nam/nữ tại `generated-batch-v6/base-head-v1/`, giữ nguồn/hash ngoài runtime. Manifest v9 thay đúng hai head subrect; tổng 7 PNG còn 884.660 byte.
- Capture automation không được gán public read-only property quan sát. Lần đầu matrix compile fail vì gán `VoAvatarGender/Level/Mode`; sửa bằng state owner nội bộ trong capture và giữ API public read-only để gameplay khác không bypass transition.
- Evidence `build/vo-ten-slot-matrix-v2/three-profiles/` có 66 frame/profile: 46 flow/action cũ + 10 slot-off nam Lv1 + 10 slot-off nữ Lv30. Review PC và đối chiếu mobile/tablet xác nhận mỗi slot tắt đúng một hoặc nhiều component; hair, weapon, lower garment và các cặp limb nhìn thấy khác biệt rõ.
- Trạng thái `CLASS_VO_LV1_30_VERTICAL_SLICE_PASS` là checkpoint Player local cho avatar/equip/action trên Map01A, không phải production-final hoặc backend inventory persistence. Trước class thứ hai phải tách orchestration khỏi Map01A vào base character/action chung.

## Shared character runtime state — checkpoint 2026-09-10

- `TwoDCharacterRuntimeState` là state owner dùng chung cho mode, giới tính, level band, equipment slot/loadout, run toggle, locomotion hold và exclusive timed action. Map/class controller chỉ áp state vào renderer/rig và xử lý damage/quest riêng; class tiếp theo không sao chép timer hoặc equip logic Võ.
- TDD RED bắt thiếu base type; GREEN cuối đạt `184 total / 183 pass / 0 fail / 1 ignored`. Test riêng khóa cycle presentation, equip/unequip, deterministic capture selection, action exclusivity, duration/progress và run hold.
- Locomotion phase/hold cố ý chỉ tiêu thụ tối đa `0,1s` mỗi tick như runtime cũ để tránh bước animation lớn khi hitch; test phải mô phỏng hai tick `0,1s`, không giả định một call `0,2s` làm hết hold.
- Capture runner phải launch `My.app/Contents/MacOS/<executable>`, không phải bundle `.app`. Lần capture v1 truyền relative path vào subprocess theo profile nên báo sai thiếu Player; tool hiện resolve player/output thành absolute path trước khi fan-out. Player v22 build `168.488.627` byte, `errors=0`; evidence `build/vo-shared-state-v2/three-profiles/` có 66 frame/profile và manifest `voSharedRuntimeStateVerified=True`.
- Review contact sheet xác nhận map/layout và silhouette các frame walk/jump/Liên Quyền, tháo tóc nam Lv1/nữ Lv30 và tháo phụ kiện không regression. Trạng thái `SHARED_CHARACTER_RUNTIME_STATE_PLAYER_PASS`; tiếp theo mới áp base vào Kiếm Lv1–30, bắt đầu bằng audit/batch source đồng nhất trước khi nối runtime.

## Kiếm Lv1–30 batch source candidate extraction — checkpoint 2026-09-10

- Audit 17 sheet Kiếm/common đã tuyển xác nhận `detail/12-female-equipment-grid-redraw-source.png` và `detail/13-male-equipment-grid-redraw-source.png` là cặp đồng nhất nhất để cắt: cùng palette trắng/đen/xanh/vàng, cùng 10 slot và có cột level rõ. Turnaround `03/04` giữ làm chuẩn tỷ lệ/identity; sheet tổng quan và skill/VFX chỉ làm reference, không crop làm runtime.
- `tools/extract_lgo_equipment_grid.py` đọc crop plan data-only, kiểm SHA-256 hai source, flood-fill nền sáng từ biên, loại grid-line component và xuất tên chuẩn `kiem-lv{001|010|020|030}-{male|female}-{slot}.png`. Class sau dùng lại tool và thay plan/tọa độ, không fork script.
- Bản canonical ngoài repository hiện là `classes-lv001-030/kiem/generated-batch-v1/source-v4/`: 80 PNG đủ `2 giới × 4 mốc × 10 slot`, tổng 531.894 byte, kèm manifest/contact/audit. Đây chỉ là coverage của grid; không chứng minh từng ô đã tách đúng hoặc fit base.
- Review contact sheet phát hiện 12 crop nữ ở `inner_top`, `lower_body`, `arm_guard` còn dính body/da/tay/chân nên đã ghi `redraw-required`; 68 crop còn lại giữ `candidate`. Tất cả `runtimeEligible=false`; kiếm nam Lv1 còn một component sáng rời cần zoom source trước khi quyết định cleanup.
- `tools/extract_lgo_equipment_grid.py` nay từ chối alias cũ như `outer_tunic`, `lower_garment`, `waist`, `boots`, bắt buộc đúng 10 slot canonical; manifest v2 luôn gắn `SOURCE_CANDIDATES_EXTRACTED_BASE_FIT_UNVERIFIED`.
- Runtime contract ở `docs/art/LGO-2D-EQUIPMENT-COMPATIBILITY-CONTRACT-v1.md`: mỗi slot giữ item ID độc lập, cho phép phối chéo level khi đã unlock; candidate chỉ thành `approved` sau skeleton/body/bone/anchor/occlusion/motion gate. Món sai phải redraw theo template base, không upscale hoặc bù offset ngẫu nhiên.
- TDD chứng minh mixed loadout `Lv1 weapon + Lv30 hair + Lv10 inner + Lv20 outer`, đồng thời chặn class/skeleton/body/bone sai và asset chưa approved. EditMode `191/190/0/1`; Player build `168.494.995` byte, 0 error; onboarding smoke PASS. Contract chưa nối renderer Kiếm nên không claim visual fit và không capture lại frame không thay đổi.
