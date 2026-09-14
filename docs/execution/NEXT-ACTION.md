## Active — CONTINUE: Character Hub base-first, audit tích hợp năm tab — 2026-09-15

`OPERATIONAL_GOAL_CURRENT`. Canonical vẫn là bộ năm tab tại
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

### Checkpoint hiện hành

- Skill, Tiềm năng và Linh thú được dựng một lần trên shared component tree. Đổi class chỉ bind profile/data/icon/text/state; không xóa panel hoặc tạo lại node/row.
- Tiềm năng dùng một vector topology cố định vẽ sẵn vòng đồng tâm, năm đường nối, core và toàn bộ khung tròn của năm node. Button phía trên chỉ nhận tương tác và bind icon/text/state; Skill giữ graph `3×3` và bốn ô trang bị riêng, không trộn topology với Tiềm năng.
- Linh thú có đúng hai skill row cố định với icon/name/level/description từ `CharacterHubSpiritPetPreview`; test đổi đủ năm class giữ nguyên reference panel/node/topology/row. Validator chặn việc đưa `RemoveFromHierarchy()` hoặc gọi lại initializer vào class refresh.
- Player: `build/map01a-character-hub-spirit-base-player-v3/LinhGioiOnline.app`.
- Evidence đã xem: `build/map01a-character-hub-spirit-base-runtime-v3/{pc,mobile,tablet}/`, 9 frame/profile, không dùng chuột OS; màn Linh thú không wrap/cắt/chồng sau khi bỏ overflow ngoài canonical. Pack capture vẫn chỉ có body div4 nên không dùng để nghiệm thu wardrobe/class art.

### Bước kế tiếp hợp lệ

1. Chạy full gate cho batch Linh thú, sau đó audit tích hợp đủ năm tab theo shell → hierarchy → asset → typography → state và chụp lại khi có thay đổi nhìn thấy.
2. Không tiếp tục phát triển class/pose/wardrobe trong task này. Không sinh/sửa source art ngẫu nhiên hoặc dựng renderer/layout riêng cho từng class.
3. Chỉ chuyển sang screen UI tiếp theo khi có một canonical design, scenario/state/interaction và asset gate; không mở lại screen cũ nếu không có regression/evidence cụ thể.

### Gate hiện hành

- Tiềm năng targeted RED `0/1`, GREEN `1/1`; Linh thú targeted RED `0/1`, GREEN `1/1`.
- Full EditMode `287 total / 286 passed / 0 failed / 1 ignored`; shared governance `23/23`; pose pack `12/12`; registered capture `19/19`; no-3D/no-source-images pass.
- Shared-skin, no-3D, no-source-images, frozen diff và `git diff --check` pass.
- Player build `errors=0`, `warnings=48`; 27 frame Linh thú ở ba viewport đã xem theo phạm vi nêu trên.
- Trạng thái: `CONTINUE`; chưa claim owner visual acceptance.
