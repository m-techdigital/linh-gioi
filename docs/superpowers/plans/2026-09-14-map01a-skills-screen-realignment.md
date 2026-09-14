# Map01A Kỹ năng — implementation plan

Canonical screen duy nhất: `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/03-ky-nang-five-tab-APPROVED.png` (1672×941). Baseline Player: `build/map01a-bag-screen-runtime-v4/{pc,mobile,tablet}/skills.png`. Screen active duy nhất là Kỹ năng; Nhân vật và Rương đồ đã khóa, Tiềm năng/Linh thú không sửa.

## Audit toàn màn trước code

| Vùng | Canonical | Baseline hiện tại | Batch cần xử lý |
|---|---|---|---|
| Shell/tab | shared 1098×724, rail năm tab | đúng shared geometry | giữ nguyên base đã khóa |
| Rail phân loại | 3 card dọc lớn, icon + nhãn | chip chữ nhỏ, icon không có | dùng một shared category-card base, 3 icon riêng |
| Skill graph | 3×3 node tròn, icon là thông tin chính, connector gọn | oval 142×104, chữ chiếm chính, dùng HUD icon tạm | node tròn 96–104, atlas skill riêng, tên/level dưới icon |
| Equipped strip | 4 icon lớn + điểm kỹ năng | icon 46 px và badge rời quá nhỏ | strip card chung, 4 icon cùng atlas, điểm nằm cuối hàng |
| Detail-right | icon lớn, type/level, mô tả, 4 facts, next-level, 2 action | panel đen thưa, copy placeholder, 1 action disabled | hierarchy detail chuyên cho skill trên cùng shared detail base |
| Viewport | cùng landscape composition | không reflow nhưng chữ/icon quá nhỏ ở mobile/tablet | scale toàn shell; không stack, không breakpoint đổi hierarchy |

## State và interaction

- Chọn bất kỳ node nào cập nhật icon, tên, cấp và detail-right bằng callback hiện có.
- `Nâng cấp` và `Trang bị` chỉ hiển thị ở trạng thái read-only/disabled vì chưa có contract ghi progression/loadout. Không giả tăng cấp, trừ điểm hoặc thay skill thật.
- `Bị động` và `Tâm pháp` giữ disabled nếu chưa có dataset; trạng thái disabled phải rõ, không tạo màn giả.
- Capture bắt buộc gồm mặc định và chọn `Kiếm Vũ` ở PC, mobile landscape và tablet.

## Asset gate

- Derivative board duy nhất: `.../redesign-v4-five-tabs/assets-skills/skill-icons-alpha-SELF-REVIEWED-v1.png`; đây là asset triển khai, không phải screen design thứ hai.
- Board 4×3 có 9 skill icon + 3 category icon, alpha thật; manifest `skill-icons-manifest-v1.json` ghi prompt/source/hash/cell map.
- Trước runtime: pack về Resources phải đổi trạng thái `DRAFT_RUNTIME_REVIEW`, giảm kích thước theo cell đồng nhất, giữ alpha, manifest/provenance và thêm no-source-image gate.

## Base-first implementation

1. Viết regression test khóa node tròn, atlas riêng, rail category, equipped strip và hai action detail.
2. Thêm shared base cho category card, skill node, icon/level badge và detail fact row trong `CongDongLamArrivalHud.Skin.cs`.
3. `CongDongLamArrivalHud.CharacterHub.cs` chỉ dựng hierarchy và bind data/callback; không tự style từng node.
4. Loader atlas dùng chung helper hiện có trong `CongDongLamMap01AArtPreview.cs`.
5. Build một Player, capture ba profile một lượt, xem trực tiếp rồi mới sửa batch kế nếu còn sai vùng lớn.
6. Chỉ khi Unity tests, catalog/shared validators, no-3D/no-source, frozen diff và visual gate đều đạt mới khóa Kỹ năng và chuyển Tiềm năng.

Tham khảo kỹ thuật: Unity UI Toolkit tách structure/style/behavior và hỗ trợ reusable templates/USS; `PanelSettings` với `Scale With Screen Size` giữ quy tắc scale toàn UI theo reference resolution. Runtime hiện dùng C# partial + shared Skin tương đương separation đó, không mở thêm UI system.

## Gate result — 2026-09-14

- Player: `build/map01a-skills-screen-player-v3/LinhGioiOnline.app`, build thành công, `errors=0`.
- Evidence được chấp nhận: `build/map01a-skills-screen-runtime-v4/{pc,mobile,tablet}/`; mỗi profile có `skills-default.png`, `skills.png` và manifest tám frame, không dùng chuột/phím OS.
- Visual audit: cùng layout hai cột ở 1600×900, 1600×720 và 1024×768; không stack, không cắt/chồng; rail ba loại, graph 3×3, equipped strip toàn chiều rộng và detail selected đều đọc được.
- Asset: atlas 12 icon RGBA riêng có manifest/hash/provenance, status `DRAFT_RUNTIME_REVIEW`; không mở class/source art.
- Kết luận: `Kỹ năng` đạt layout gate. Screen active kế tiếp là `Tiềm năng`, bắt đầu bằng audit/plan toàn màn trước code.
