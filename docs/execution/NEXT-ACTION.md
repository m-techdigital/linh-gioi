## Active — NEED_HUMAN_VISUAL_REVIEW: character hub vector ornament v8 — 2026-09-14

`OPERATIONAL_GOAL_CURRENT`. Đây là goal duy nhất được dùng để chọn công việc trong worktree này. Goal class/pose/wardrobe/source trước đây đã bị owner loại khỏi scope; không được resume, build, capture hoặc dùng làm fallback. Không rollback code class hiện có.

Mục tiêu hiện hành: owner review character hub theo đúng bộ design duy nhất tại:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

Năm tab cấp cao dùng một shared shell/base:

1. `Nhân vật`
2. `Rương đồ`
3. `Kỹ năng`
4. `Tiềm năng`
5. `Linh thú`

Feedback gần nhất yêu cầu giữ cơ chế modular nhưng thiết kế lại hoa văn. Evidence hiện hành duy nhất là v8: một corner master vector 24 px dạng nút/lá, mirror thành bốn góc, nối với một edge segment 16 px; không crop canonical, không có raster frame hoặc khung chữ L song song.

### Gate kế tiếp

- Owner review lần lượt `Nhân vật → Rương đồ → Kỹ năng → Tiềm năng → Linh thú` bằng evidence `build/map01a-five-tab-depth-runtime-v8/{pc,mobile,tablet}/` hoặc Player `build/map01a-five-tab-depth-player-v8/LinhGioiOnline.app`.
- Nếu có feedback, gom sai lệch theo shell → tab → hai cột → nội vùng → typography/icon thành một batch; không chỉnh rời từng pixel.
- Không chuyển Login/HUD/NPC hoặc phần UI tiếp theo trước review này. Không resume class/pose/wardrobe/source và không rollback code class.

### Gate đã đạt

- Python unit test `30/30`; Unity EditMode thật `284 total / 283 passed / 0 failed / 1 ignored`.
- `python3.12 tools/validate_lgo_ui_shared_skin.py`.
- `python3.12 tools/validate_2d_branch_no_3d.py`.
- `python3.12 tools/validate_2d_branch_no_source_images.py`.
- Frozen diff rỗng cho `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`.
- Player v8 build thành công; 15 frame PC/mobile landscape/tablet đã được xem trực tiếp.
- Bảy texture chrome tổng 63.469 byte; ornament frame là vector authored trong shared base, không phải ảnh crop.

Không chuyển sang Login, HUD, NPC, class, pose, wardrobe hoặc source trong batch này.
