## Quick Resume

- `OPERATIONAL_GOAL_CURRENT`: hoàn thiện Character Hub 5 tab trên một shared base; screen active duy nhất vẫn là Tiềm năng theo `redesign-v4-five-tabs/04-tiem-nang-five-tab-APPROVED.png`.
- Checkpoint v17 giữ một topology tĩnh 600×520, một detail template và catalog/state bất biến. Võ/Kiếm/Pháp/Cơ/Linh chỉ bind icon/text/value/selection/recommendation; class không tạo UI hay hình học riêng.
- Player: `build/character-hub-five-profile-player-v17/LinhGioiOnline.app`.
- Evidence hiện hành: `build/character-hub-five-profile-runtime-v17/{pc,mobile,tablet}/`; mỗi viewport có 19 frame, trong đó đủ `potential-{vo,kiem,phap,co,linh}-{default,selected}.png`. Manifest xác nhận capture nội bộ, không dùng chuột/phím OS và không đổi renderer authority.
- Không phát triển class/pose/wardrobe/source art trong task này; không phục hồi renderer cũ hoặc dựng UI theo class.

## Next task

1. Owner review v17 trên ba viewport, ưu tiên topology/medallion, default/selected đủ năm profile và hierarchy detail-right.
2. Chỉ sửa feedback visual cụ thể trên shared topology/detail. Khi owner duyệt mới khóa Tiềm năng và chọn canonical screen tiếp theo.
3. Không đổi canonical, class art, pose, wardrobe, camera/base/scale hoặc frozen surface.

## Current blocker

`NEED_HUMAN_VISUAL_REVIEW`: code, test và Player capture đã qua gate; acceptance hình ảnh của màn Tiềm năng cần owner duyệt.

Evidence:
`build/character-hub-five-profile-runtime-v17/{pc,mobile,tablet}/potential-{vo,kiem,phap,co,linh}-{default,selected}.png`

## Checkpoint detail

- `CharacterHubPotentialTopology` sở hữu toàn bộ vòng, đường nối, core, năm outer slot frame, value/add frame và glyph; overlay trong suốt chỉ bind dữ liệu/tương tác.
- Hook evidence bind từng profile vào cùng object tree và kiểm `ActiveEquipmentClassId` không đổi. Đây là capture data-only, không phải class switch gameplay hay renderer mới.
- TDD RED `0/1`, GREEN `1/1`; full Unity EditMode `288 total / 287 passed / 0 failed / 1 ignored`; capture/shared asset tests `33/33`; no-3D/no-source/frozen diff pass.
- Chưa claim owner visual acceptance hoặc art/wardrobe đủ năm class.
