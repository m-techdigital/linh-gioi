## Active — NEED_HUMAN_VISUAL_REVIEW: character hub chrome v2 — 2026-09-14

`OPERATIONAL_GOAL_CURRENT`. Đây là goal duy nhất được dùng để chọn công việc trong worktree này. Goal class/pose/wardrobe/source trước đây đã bị owner loại khỏi scope; không được resume, build, capture hoặc dùng làm fallback. Không rollback code class hiện có.

Mục tiêu hiện hành: hoàn thiện UI/UX Layout character hub theo đúng bộ design duy nhất tại:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

Năm tab cấp cao dùng một shared shell/base:

1. `Nhân vật`
2. `Rương đồ`
3. `Kỹ năng`
4. `Tiềm năng`
5. `Linh thú`

Feedback owner mới nhất bác bỏ lớp trình bày phẳng của evidence cũ. Shared chrome v2 đã thay lớp modal bằng asset riêng có provenance: khung vàng nhiều lớp, hoa văn nền shell/panel, tab idle/selected, action xanh/vàng, close bát giác và animation mở/đổi tab. Cả năm tab đã được capture lại sau thời gian settle tại `build/map01a-five-tab-chrome-runtime-v3/{pc,mobile,tablet}/`; đây là evidence duy nhất cho review hiện tại.

### Gate kế tiếp

- Owner review lần lượt `Nhân vật → Rương đồ → Kỹ năng → Tiềm năng → Linh thú` bằng evidence chrome v2 hoặc Player `build/map01a-five-tab-chrome-player-v3/LinhGioiOnline.app`.
- Nếu có feedback, gom sai lệch theo shell → tab → hai cột → nội vùng → typography/icon rồi sửa thành một batch; không quay lại chỉnh rời từng pixel.
- Không chuyển Login/HUD/NPC hoặc phần UI tiếp theo trước review này. Không resume class/pose/wardrobe/source và không rollback code class.

### Gate đã đạt

- Focused Unity/EditMode cho năm tab và shared layout.
- `python3.12 tools/validate_lgo_ui_shared_skin.py` và unit test của validator.
- `python3.12 tools/validate_2d_branch_no_3d.py`.
- `python3.12 tools/validate_2d_branch_no_source_images.py`.
- Frozen diff audit đối với `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`.
- Player từng batch đã build; mỗi screen có capture PC `1600×900`, mobile landscape `1600×720`, tablet `1024×768` và đã được xem trực tiếp.
- Chrome v2 dùng bảy texture runtime tổng khoảng 76 KiB, không crop canonical board; validator khóa đủ pack, shared helper, animation marker và import policy không mipmap/không downscale shell.

Không chuyển sang Login, HUD, NPC, class, pose, wardrobe hoặc source trong batch này.
