## Quick Resume

- `OPERATIONAL_GOAL_CURRENT`: hoàn thiện Character Hub 5 tab trên một shared base; screen đang active duy nhất là Tiềm năng theo `redesign-v4-five-tabs/04-tiem-nang-five-tab-APPROVED.png`.
- Checkpoint v16 giữ một topology tĩnh 600×520 và catalog/state bất biến cho đủ năm class. Topology sở hữu vòng slot ngoài, đường nối, value/add frame và lõi; overlay chỉ bind icon/text/value/click, không còn hình học node lặp lại. Shared inspector tiếp tục giữ hierarchy và cost/action không chồng.
- Player: `build/character-hub-potential-overlay-player-v16/LinhGioiOnline.app`.
- Evidence hiện hành duy nhất: `build/character-hub-potential-overlay-runtime-v16/{pc,mobile,tablet}/{potential-default,potential}.png`; lần lượt 1280×720, 1600×720, 1024×768; 9 frame/profile, `usesOsMouseOrKeyboard=false`. Capture bằng `python3.12 tools/capture_lgo_character_hub.py ...`; không nhận evidence nếu ba profile dùng sai kích thước.
- Không phát triển class/pose/wardrobe/source art trong task này; không phục hồi renderer cũ hoặc dựng UI theo class.

## Next task

1. Owner review v16, tập trung một outer slot frame + medallion icon, độ sâu vòng rune/lõi thiền, alignment icon/tên/value và hierarchy detail-right trên đủ ba viewport thật.
2. Chỉ sửa feedback visual cụ thể trên shared topology/detail; khi owner duyệt mới khóa screen và chọn canonical UI screen tiếp theo.
3. Không đổi canonical, class art, pose, wardrobe, camera/base/scale hoặc frozen surface.

## Current blocker

`NEED_HUMAN_VISUAL_REVIEW`: code, test và Player capture đã qua gate; acceptance hình ảnh của screen Tiềm năng cần owner duyệt.

Evidence:
`build/character-hub-potential-overlay-runtime-v16/{pc,mobile,tablet}/{potential-default,potential}.png`

## Checkpoint detail

- Một `CharacterHubPotentialTopology` nạp duy nhất asset 600×520 sở hữu vòng/đường nối/core figure/trục mạch/năm khung node/năm ô giá trị/năm ô cộng/glyph; overlay chỉ nhận input và bind icon/tên/value/selection.
- Một `Map01A Potential Detail Facts` sở hữu summary/current effect/next effect/cost. `CharacterHubPotentialPreview` truyền dữ liệu có cấu trúc, không nhét layout vào một chuỗi copy. Description nằm trong hero cạnh icon; status review thừa không còn xuất hiện.
- Skill và Tiềm năng là hai topology tách biệt. Test đổi đủ năm class giữ nguyên reference panel/node/topology/detail và chọn đúng node mặc định từ profile data.
- Hierarchy TDD RED `0/1`, GREEN targeted `1/1`; full Unity EditMode `287 total / 286 passed / 0 failed / 1 ignored`.
- Shared UI validator pass; governance `23/23`; pose pack `12/12`; registered capture `19/19`; no-3D/no-source-images pass.
- v13 bị loại vì cost/action chồng nhau; v15 sửa shared inspector. v16 tách hình học khỏi node overlay và giảm topology từ 59.678 xuống 56.204 byte. Capture đúng PC/tablet/mobile; Player build/test gate được ghi theo evidence v16. Toàn bộ skin giữ native-size import.
- Chưa claim owner visual acceptance hoặc art/wardrobe đủ năm class.
