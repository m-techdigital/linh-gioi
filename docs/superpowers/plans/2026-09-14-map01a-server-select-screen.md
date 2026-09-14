# Map01A Server Select Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Tạo màn Chọn máy chủ bám canonical đã khóa, mở được từ Entry và Character Select, dùng đúng một server hiện có và phản hồi local trung thực.

**Architecture:** Một partial sở hữu hierarchy/state/route của screen; hai source screen chỉ gọi cùng một entry point và ghi nguồn quay lại. Toàn style nằm ở shared Skin, reuse Entry scene layers và HUD icon atlas, không thêm runtime texture.

**Tech Stack:** Unity C#, UI Toolkit, Unity EditMode tests, Player capture harness.

**Spec:** `docs/design/LGO-MAP01A-SERVER-SELECT-SCREEN-CONTRACT-v1.0.md`

## Global Constraints

- Chỉ một canonical design cho screen.
- Không bịa server/region/ping/backend state.
- PC/mobile landscape/tablet giữ cùng composition; không wrap/stack.
- Không sửa class/pose/wardrobe/source hoặc frozen surfaces.

---

### Task 1: Khóa hierarchy và route bằng test

**Files:**
- Modify: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Consumes: `OpenServerSelect(...)`, `CloseServerSelect(...)`, screen source state.
- Produces: regression cho một server thật, route hai nguồn, confirm/back và không đổi class.

- [x] Viết test yêu cầu `Map01A Server Select Overlay`, `Map01A Server Select Card`, `Map01A Server Select Confirm`, `Map01A Server Select Back` và loại server giả.
- [x] Chạy focused test để ghi nhận RED vì screen chưa tồn tại.

### Task 2: Dựng screen trên shared base

**Files:**
- Create: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.ServerSelect.cs`
- Create: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.ServerSelect.cs.meta`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Entry.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.CharacterSelect.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs`

**Interfaces:**
- Consumes: Entry background/brand layers, `GetMap01AHudIconSprite("server")`, shared label/button/card helpers.
- Produces: `OpenServerSelect(ServerSelectReturnTarget)`, `CloseServerSelect(bool confirm)`, `UpdateServerSelectScreen()`.

- [x] Thêm enum nguồn quay lại và một overlay duy nhất; server card chứa `S1 · Đông Lâm`, `Mượt`, `Máy chủ hiện tại`.
- [x] Nối server row Entry và Character Select vào cùng `OpenServerSelect(...)`.
- [x] Xác nhận/quay lại phục hồi đúng source; Escape đóng server select trước các overlay bên dưới.
- [x] Thêm helper/class server-select vào shared Skin rồi chạy shared-skin validator.
- [x] Chạy focused test và full `TwoDCharacterRuntimeStateTests` để xác nhận GREEN.

### Task 3: Player evidence và checkpoint

**Files:**
- Modify: `docs/design/LGO-MAP01A-UI-REVIEW-CATALOG-v0.1.md`
- Modify: `docs/execution/PROJECT-STATE.md`
- Modify: `docs/execution/NEXT-ACTION.md`
- Modify: `build/codex-autopilot/status.json` (ignored runtime status)

**Interfaces:**
- Consumes: capture flag cho server-select và Player build hiện hành.
- Produces: evidence ba viewport, visual conclusion và checkpoint push.

- [x] Build Player sau C# changes.
- [x] Capture `server-select.png` tại PC `1600×900`, mobile landscape `1600×720`, tablet `1024×768`; không dùng chuột/bàn phím hệ điều hành.
- [x] Xem trực tiếp cả ba ảnh, gom mọi sai lệch layout lớn thành một lượt sửa và chỉ capture lại khi code đổi.
- [x] Chạy UI catalog/shared-skin/no-3D/no-source/change-budget/frozen audit.
- [x] Cập nhật state và next action sang design gate Đăng ký chỉ khi toàn gate sạch; commit/push một batch coherent.
