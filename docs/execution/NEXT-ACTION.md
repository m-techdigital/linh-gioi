## Active — NEED_HUMAN_VISUAL_REVIEW: character hub năm tab — 2026-09-14

`OPERATIONAL_GOAL_CURRENT`. Đây là goal duy nhất được dùng để chọn công việc trong worktree này. Goal class/pose/wardrobe/source trước đây đã bị owner loại khỏi scope; không được resume, build, capture hoặc dùng làm fallback. Không rollback code class hiện có.

Mục tiêu hiện hành: hoàn thiện UI/UX Layout character hub theo đúng bộ design duy nhất tại:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

Năm tab cấp cao dùng một shared shell/base:

1. `Nhân vật`
2. `Rương đồ`
3. `Kỹ năng`
4. `Tiềm năng`
5. `Linh thú`

Shared layout tổng và cả năm tab đã qua gate kỹ thuật/visual nội bộ theo đúng thứ tự. Evidence cuối cho `Linh thú`: `build/map01a-spirit-layout-runtime-v1/{pc,mobile,tablet}/`; evidence từng screen được trỏ trong UI review catalog. Gate hiện tại là owner xem Player/evidence của năm layout trước khi mở screen ngoài character hub.

### Gate kế tiếp

- Owner review lần lượt `Nhân vật → Rương đồ → Kỹ năng → Tiềm năng → Linh thú` bằng evidence hiện hành hoặc Player cuối `build/map01a-spirit-layout-player-v1/LinhGioiOnline.app`.
- Nếu có feedback, sửa đúng screen có evidence; không mở lại screen đã khóa chỉ vì khác dữ liệu/art mà contract hiện hành chưa sở hữu.
- Không chuyển Login/HUD/NPC hoặc phần UI tiếp theo trước review này. Không resume class/pose/wardrobe/source và không rollback code class.

### Gate đã đạt

- Focused Unity/EditMode cho năm tab và shared layout.
- `python3.12 tools/validate_lgo_ui_shared_skin.py` và unit test của validator.
- `python3.12 tools/validate_2d_branch_no_3d.py`.
- `python3.12 tools/validate_2d_branch_no_source_images.py`.
- Frozen diff audit đối với `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`.
- Player từng batch đã build; mỗi screen có capture PC `1600×900`, mobile landscape `1600×720`, tablet `1024×768` và đã được xem trực tiếp.

Không chuyển sang Login, HUD, NPC, class, pose, wardrobe hoặc source trong batch này.
