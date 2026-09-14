## Active — FIX_REQUIRED: hoàn thiện tab Rương đồ theo design đã duyệt — 2026-09-14

`OPERATIONAL_GOAL_CURRENT`. Đây là goal duy nhất được dùng để chọn công việc trong worktree này. Goal class/pose/wardrobe/source trước đây đã bị owner loại khỏi scope; không được resume, build, capture hoặc dùng làm fallback. Không rollback code class hiện có.

Mục tiêu hiện hành: hoàn thiện UI/UX Layout character hub theo đúng bộ design duy nhất tại:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

Năm tab cấp cao dùng một shared shell/base:

1. `Nhân vật`
2. `Rương đồ`
3. `Kỹ năng`
4. `Tiềm năng`
5. `Linh thú`

Shared layout tổng đã qua Player gate mới tại `build/map01a-five-tab-layout-runtime-v4/{pc,mobile,tablet}/`: một shell, một hàng năm tab, body hai cột và detail-right giữ cùng composition ở cả ba viewport. Tab `Nhân vật` đã qua gate chi tiết tại `build/map01a-character-inspector-runtime-v1/{pc,mobile,tablet}/`; active duy nhất tiếp theo là `Rương đồ`.

### Batch kế tiếp

- Đối chiếu duy nhất `02-ruong-do-phan-loai-doc-tab-compact-APPROVED.png`; hoàn thiện category rail dọc, capacity/search/sort, grid năm cột, footer action và detail-right của tab `Rương đồ`.
- Mọi sửa đổi lặp phải đi qua `CongDongLamArrivalHud.Skin.cs` hoặc shared layout constant; không căn offset vụn để chữa một frame.
- Giữ đúng một canonical cho mỗi tab theo `docs/design/LGO-MAP01A-CHARACTER-HUB-SCREEN-CONTRACT-v1.0.md`; không tạo design song song.
- PC `1600×900`, mobile landscape `1600×720`, tablet `1024×768` phải giữ cùng composition bằng scale, không wrap/stack.
- Chỉ khi Player `Rương đồ` đạt ở đủ ba viewport mới chuyển `Kỹ năng`; sau đó mới `Tiềm năng → Linh thú`.
- Không tạo icon/art tạm. Chỉ dùng asset có canonical/provenance và đúng pixel budget.

### Gate trước checkpoint

- Focused Unity/EditMode cho năm tab và shared layout.
- `python3.12 tools/validate_lgo_ui_shared_skin.py` và unit test của validator.
- `python3.12 tools/validate_2d_branch_no_3d.py`.
- `python3.12 tools/validate_2d_branch_no_source_images.py`.
- Frozen diff audit đối với `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`.
- Build Player một lần sau batch; capture mặc định/search/chọn món/khóa món của `Rương đồ` ở ba viewport và xem trực tiếp trước khi cập nhật trạng thái.

Không chuyển sang Login, HUD, NPC, class, pose, wardrobe hoặc source trong batch này.
