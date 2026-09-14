## Active — FIX_REQUIRED: hoàn thiện tab Tiềm năng theo design đã duyệt — 2026-09-14

`OPERATIONAL_GOAL_CURRENT`. Đây là goal duy nhất được dùng để chọn công việc trong worktree này. Goal class/pose/wardrobe/source trước đây đã bị owner loại khỏi scope; không được resume, build, capture hoặc dùng làm fallback. Không rollback code class hiện có.

Mục tiêu hiện hành: hoàn thiện UI/UX Layout character hub theo đúng bộ design duy nhất tại:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

Năm tab cấp cao dùng một shared shell/base:

1. `Nhân vật`
2. `Rương đồ`
3. `Kỹ năng`
4. `Tiềm năng`
5. `Linh thú`

Shared layout tổng, `Nhân vật`, `Rương đồ` và `Kỹ năng` đã qua Player gate. Evidence Kỹ năng hiện hành: `build/map01a-skills-layout-runtime-v1/{pc,mobile,tablet}/`. Active duy nhất tiếp theo là `Tiềm năng`.

### Batch kế tiếp

- Đối chiếu duy nhất `04-tiem-nang-five-tab-APPROVED.png`; hoàn thiện kinh mạch trung tâm, năm node, điểm còn lại, detail-right và trạng thái read-only của tab `Tiềm năng`.
- Mọi sửa đổi lặp phải đi qua `CongDongLamArrivalHud.Skin.cs` hoặc shared layout constant; không căn offset vụn để chữa một frame.
- Giữ đúng một canonical cho mỗi tab theo `docs/design/LGO-MAP01A-CHARACTER-HUB-SCREEN-CONTRACT-v1.0.md`; không tạo design song song.
- PC `1600×900`, mobile landscape `1600×720`, tablet `1024×768` phải giữ cùng composition bằng scale, không wrap/stack.
- Chỉ khi Player `Tiềm năng` đạt ở đủ ba viewport mới chuyển `Linh thú`.
- Không tạo icon/art tạm. Chỉ dùng asset có canonical/provenance và đúng pixel budget.

### Gate trước checkpoint

- Focused Unity/EditMode cho năm tab và shared layout.
- `python3.12 tools/validate_lgo_ui_shared_skin.py` và unit test của validator.
- `python3.12 tools/validate_2d_branch_no_3d.py`.
- `python3.12 tools/validate_2d_branch_no_source_images.py`.
- Frozen diff audit đối với `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`.
- Build Player một lần sau batch; capture mặc định/chọn node của `Tiềm năng` ở ba viewport và xem trực tiếp trước khi cập nhật trạng thái.

Không chuyển sang Login, HUD, NPC, class, pose, wardrobe hoặc source trong batch này.
