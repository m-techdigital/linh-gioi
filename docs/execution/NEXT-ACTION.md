## Quick Resume

- `OPERATIONAL_GOAL_CURRENT`: hoàn thiện Character Hub 5 tab trên một shared base; screen đang active duy nhất là Tiềm năng theo `redesign-v4-five-tabs/04-tiem-nang-five-tab-APPROVED.png`.
- Checkpoint v10 giữ template tĩnh 600×520 có sẵn vòng/đường/node/value/add/core; class chỉ bind icon/tên/value/selection trên overlay dùng chung. Import giữ đúng native size bằng `nPOTScale: 0`.
- Player: `build/map01a-character-hub-potential-template-player-v9/LinhGioiOnline.app`.
- Evidence hiện hành duy nhất: `build/map01a-character-hub-potential-template-runtime-v10/{pc,mobile,tablet}/{potential-default,potential}.png`; lần lượt 1280×720, 1024×768, 1600×720; 9 frame/profile, `usesOsMouseOrKeyboard=false`. Capture bằng `python3.12 tools/capture_lgo_character_hub.py ...`; không nhận evidence nếu ba profile dùng sai kích thước.
- Không phát triển class/pose/wardrobe/source art trong task này; không phục hồi renderer cũ hoặc dựng UI theo class.

## Next task

1. Owner review v10, tập trung template topology cố định, alignment icon/tên/value trên năm node và hierarchy current/next/cost ở detail-right trên đủ ba viewport thật.
2. Chỉ sửa feedback visual cụ thể trên shared topology/detail; khi owner duyệt mới khóa screen và chọn canonical UI screen tiếp theo.
3. Không đổi canonical, class art, pose, wardrobe, camera/base/scale hoặc frozen surface.

## Current blocker

`NEED_HUMAN_VISUAL_REVIEW`: code, test và Player capture đã qua gate; acceptance hình ảnh của screen Tiềm năng cần owner duyệt.

Evidence:
`build/map01a-character-hub-potential-template-runtime-v10/{pc,mobile,tablet}/{potential-default,potential}.png`

## Checkpoint detail

- Một `CharacterHubPotentialTopology` nạp duy nhất asset 600×520 sở hữu vòng/đường nối/core figure/trục mạch/năm khung node/năm ô giá trị/năm ô cộng/glyph; overlay chỉ nhận input và bind icon/tên/value/selection.
- Một `Map01A Potential Detail Facts` sở hữu summary/current effect/next effect/cost. `CharacterHubPotentialPreview` truyền dữ liệu có cấu trúc, không nhét layout vào một chuỗi copy. Description nằm trong hero cạnh icon; status review thừa không còn xuất hiện.
- Skill và Tiềm năng là hai topology tách biệt. Test đổi đủ năm class giữ nguyên reference panel/node/topology/detail và chọn đúng node mặc định từ profile data.
- TDD RED `0/1`, GREEN targeted `3/3`; full Unity EditMode `287 total / 286 passed / 0 failed / 1 ignored`.
- Shared UI validator pass; governance `23/23`; pose pack `12/12`; registered capture `19/19`; no-3D/no-source-images pass.
- Player v9 `Succeeded`, `errors=0`; capture v10 đúng PC/tablet/mobile; frozen diff/`git diff --check` pass. Runtime asset 53.995 byte và toàn bộ skin khóa native-size import.
- Chưa claim owner visual acceptance hoặc art/wardrobe đủ năm class.
