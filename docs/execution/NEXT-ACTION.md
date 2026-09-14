## Quick Resume

- `OPERATIONAL_GOAL_CURRENT`: hoàn thiện Character Hub 5 tab trên một shared base; screen đang active duy nhất là Tiềm năng theo `redesign-v4-five-tabs/04-tiem-nang-five-tab-APPROVED.png`.
- Checkpoint v5 đã dựng sẵn topology 600×520 và detail-right; `Võ/Kiếm/Pháp/Cơ/Linh` chỉ bind profile/data/icon/text/state vào cùng object tree.
- Player: `build/map01a-character-hub-potential-fidelity-player-v5/LinhGioiOnline.app`.
- Evidence hiện hành duy nhất: `build/map01a-character-hub-potential-fidelity-runtime-v5/{pc,mobile,tablet}/{potential-default,potential}.png`; 9 frame/profile, `usesOsMouseOrKeyboard=false`.
- Không phát triển class/pose/wardrobe/source art trong task này; không phục hồi renderer cũ hoặc dựng UI theo class.

## Next task

1. Owner review v5, tập trung tỷ lệ topology/core figure/năm node-value-add và hierarchy current/next/cost ở detail-right.
2. Chỉ sửa feedback visual cụ thể trên shared topology/detail; khi owner duyệt mới khóa screen và chọn canonical UI screen tiếp theo.
3. Không đổi canonical, class art, pose, wardrobe, camera/base/scale hoặc frozen surface.

## Current blocker

`NEED_HUMAN_VISUAL_REVIEW`: code, test và Player capture đã qua gate; acceptance hình ảnh của screen Tiềm năng cần owner duyệt.

Evidence:
`build/map01a-character-hub-potential-fidelity-runtime-v5/{pc,mobile,tablet}/{potential-default,potential}.png`

## Checkpoint detail

- Một `CharacterHubPotentialTopology` sở hữu vòng/đường nối/runic anchors/core figure/năm khung node/năm ô giá trị/năm ô cộng/glyph; overlay chỉ nhận input và bind icon/tên/value/selection.
- Một `Map01A Potential Detail Facts` sở hữu summary/current effect/next effect/cost. `CharacterHubPotentialPreview` truyền dữ liệu có cấu trúc, không nhét layout vào một chuỗi copy.
- Skill và Tiềm năng là hai topology tách biệt. Test đổi đủ năm class giữ nguyên reference panel/node/topology/detail.
- TDD RED `0/1`, GREEN targeted `3/3`; full Unity EditMode `287 total / 286 passed / 0 failed / 1 ignored`.
- Shared UI validator pass; governance `23/23`; pose pack `12/12`; registered capture `19/19`; no-3D/no-source-images pass.
- Player v5 `Succeeded`, `errors=0`, `warnings=48`; frozen diff/change budget/`git diff --check` pass.
- Chưa claim owner visual acceptance hoặc art/wardrobe đủ năm class.
