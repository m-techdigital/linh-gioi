## Active — NEED_HUMAN_VISUAL_REVIEW: Tiềm năng template dùng chung — 2026-09-15

`OPERATIONAL_GOAL_CURRENT`. Canonical vẫn là bộ năm tab tại
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

### Checkpoint hiện hành

- Tiềm năng hiện có đúng một `CharacterHubPotentialTopology` dùng chung, vẽ sẵn vòng ngoài, đường nối, core, đủ năm khung node, năm ô giá trị, năm ô cộng và năm dấu `+`. Class chỉ bind icon/tên/giá trị/recommendation/selection; năm class dùng chung một catalog stat immutable.
- Graph được fit vào cột main `600 px`; bản Player/evidence hợp lệ duy nhất của batch là v3. Không quay lại evidence v1/v2 hoặc cách dựng border/glyph riêng ở từng node.
- Skill, Tiềm năng và Linh thú được dựng một lần trên shared component tree. Đổi class chỉ bind profile/data/icon/text/state; không xóa panel hoặc tạo lại node/row.
- Tiềm năng dùng một vector topology cố định vẽ sẵn vòng đồng tâm, năm đường nối, core và toàn bộ khung tròn của năm node. Button phía trên chỉ nhận tương tác và bind icon/text/state; Skill giữ graph `3×3` và bốn ô trang bị riêng, không trộn topology với Tiềm năng.
- Linh thú có đúng hai skill row cố định với icon/name/level/description từ `CharacterHubSpiritPetPreview`; test đổi đủ năm class giữ nguyên reference panel/node/topology/row. Validator chặn việc đưa `RemoveFromHierarchy()` hoặc gọi lại initializer vào class refresh.
- Player: `build/map01a-character-hub-potential-template-player-v3/LinhGioiOnline.app`.
- Evidence đã xem: `build/map01a-character-hub-potential-template-runtime-v3/{pc,mobile,tablet}/`, 9 frame/profile, không dùng chuột OS; Tiềm năng default/selected không wrap/cắt/chồng và các ô bên phải không còn bị cắt. Pack capture vẫn chỉ có body div4 nên không dùng để nghiệm thu wardrobe/class art.

### Bước kế tiếp hợp lệ

1. Owner review Tiềm năng trên Player `build/map01a-character-hub-potential-template-player-v3/LinhGioiOnline.app` hoặc evidence v3, tập trung vào topology/vòng/đường nối, ô giá trị, ô cộng, glyph và độ fit cột main. Nếu duyệt thì giữ khóa shared base này.
2. Không tiếp tục phát triển class/pose/wardrobe trong task này. Không sinh/sửa source art ngẫu nhiên hoặc dựng renderer/layout riêng cho từng class.
3. Chỉ chuyển sang screen UI tiếp theo khi có một canonical design, scenario/state/interaction và asset gate; không mở lại screen cũ nếu không có regression/evidence cụ thể.

### Gate hiện hành

- Targeted RED/GREEN: shared catalog, value/add frame, plus glyph và giới hạn width đều đã chứng minh fail trước sửa và đạt `1/1` sau sửa.
- Tiềm năng targeted RED `0/1`, GREEN `1/1`; Linh thú targeted RED `0/1`, GREEN `1/1`.
- Full EditMode `287 total / 286 passed / 0 failed / 1 ignored`; shared governance `23/23`; pose pack `12/12`; registered capture `19/19`; no-3D/no-source-images pass.
- Shared-skin, no-3D, no-source-images, frozen diff và `git diff --check` pass.
- Player build v3 `errors=0`, `warnings=48`; Tiềm năng default/selected ở PC/mobile/tablet đã xem theo phạm vi nêu trên.
- Audit tích hợp 15 frame tại `build/map01a-character-hub-spirit-base-runtime-v3/integrated-five-tab-contact-sheet.png`: không thấy wrap/cắt/chồng hoặc topology bị nhân theo screen/class.
- Trạng thái: `NEED_HUMAN_VISUAL_REVIEW`; chưa claim owner visual acceptance hoặc art/wardrobe năm class.
