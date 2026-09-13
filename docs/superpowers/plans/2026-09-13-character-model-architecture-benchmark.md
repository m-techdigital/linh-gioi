# Character Model Architecture Benchmark Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Tạo gate máy đọc được để chạy và so sánh hữu hạn `skeletal_2d` với `modular_3d`, đồng thời chặn quay lại authoring từng pixel/pose trước khi kiến trúc được quyết định.

**Architecture:** Một planner Python thuần đọc manifest benchmark JSON, kiểm gate theo thứ tự, chuẩn hóa vector so sánh và chỉ phát quyết định khi evidence đủ. Một inventory JSON từ trạng thái repo làm input đầu tiên; tài liệu source-space và evidence hiện có vẫn là baseline, không bị promote.

**Tech Stack:** Python 3.12, `unittest`, JSON, Unity Package manifest, Blender CLI evidence.

**Spec:** `docs/superpowers/specs/2026-09-13-character-model-architecture-benchmark-design.md`

## Global Constraints

- Không sửa `protocol/**`, `gamedata/schemas/**`, `docs/adr/**` hoặc `client/Unity/Assets/Game/UI/design-tokens.json`.
- Không normalize từng pose/item và không dùng runtime offset để che source sai.
- Không claim visual/runtime/mobile pass từ source inspection hay ảnh macOS giả lập tỷ lệ.
- Không thay renderer trong batch planner; chỉ tạo gate và evidence inventory.
- Giữ thay đổi tách khỏi các file runtime đang đổi trên `origin/codex/vo-pose-div4-review`.

---

### Task 1: Planner gate theo evidence

**Files:**
- Create: `tools/test_plan_lgo_character_model_architecture_gate.py`
- Create: `tools/plan_lgo_character_model_architecture_gate.py`

**Interfaces:**
- Consumes: `plan_architecture_gate(manifest: dict) -> dict` với record baseline và hai candidate theo spec.
- Produces: status, next action, candidate eligibility, missing evidence, cost vectors và Pareto decision.

- [x] **Step 1: Viết test RED cho thứ tự gate, anti-loop và quyết định Pareto**

Các fixture literal phải chứng minh: thiếu baseline bị chặn; toolchain không đồng nghĩa pass; A được chạy trước khi cả hai chưa probe; per-pose edit làm candidate không eligible; clip có sẵn nhưng thiếu transition/mid-keyframe/transform ownership vẫn bị chặn; visual pending không được quyết định; candidate đủ evidence có thể thắng bằng dominance; trade-off trả `OWNER_TRADEOFF_REQUIRED`.

- [x] **Step 2: Chạy test và xác nhận fail do module chưa tồn tại**

Run: `PYTHONPATH=tools PYTHONPYCACHEPREFIX=build/pycache python3.12 -m unittest tools/test_plan_lgo_character_model_architecture_gate.py`

Expected: FAIL import `plan_lgo_character_model_architecture_gate`.

- [x] **Step 3: Viết implementation tối thiểu**

Planner kiểm schema/gate, chọn next action có thứ tự xác định, và chỉ tính Pareto trên candidate đủ gate. CLI đọc `--manifest`, ghi `--output`; exit 0 chỉ khi benchmark sẵn sàng quyết định, exit 2 khi còn việc hoặc review.

- [x] **Step 4: Chạy test GREEN và regression gate pose hiện hành**

Run: `PYTHONPATH=tools PYTHONPYCACHEPREFIX=build/pycache python3.12 -m unittest tools/test_plan_lgo_character_model_architecture_gate.py tools/test_lgo_pose_pipeline_gates.py`

Expected: tất cả test pass.

### Task 2: Inventory evidence thật và next action

**Files:**
- Create: `build/character-model-architecture-review-01/benchmark-manifest-v1.json`
- Create: `build/character-model-architecture-review-01/next-action-v1.json`
- Modify: `docs/execution/NEXT-ACTION.md`
- Modify: `build/codex-autopilot/status.json`

**Interfaces:**
- Consumes: package/version/tool discovery và evidence cũ đã có.
- Produces: manifest không claim probe pass và planner result chỉ đúng một next action.

- [x] **Step 1: Ghi manifest với toolchain inventory và trạng thái evidence trung thực**

Baseline chỉ ghi những số đo đã tồn tại; `skeletal_2d` là `READY_FOR_PROBE`, `modular_3d` là `READY_FOR_PROBE`, và mọi common-task/visual/performance chưa chạy là false/pending.

- [x] **Step 2: Chạy planner**

Run: `PYTHONPATH=tools PYTHONPYCACHEPREFIX=build/pycache python3.12 tools/plan_lgo_character_model_architecture_gate.py --manifest build/character-model-architecture-review-01/benchmark-manifest-v1.json --output build/character-model-architecture-review-01/next-action-v1.json`

Expected: exit 2, status `RUN_SKELETAL_2D_PROBE`.

- [x] **Step 3: Cập nhật handoff/state ngắn gọn**

`NEXT-ACTION` trỏ vào manifest/result và mô tả closure của probe A. `status.json` giữ `CONTINUE`.

### Task 3: Xác minh batch

**Files:**
- Verify all files above.

**Interfaces:**
- Consumes: planner, tests, evidence.
- Produces: batch có thể review, không xung đột file runtime của nhánh song song.

- [x] **Step 1: Chạy test tập trung và parse JSON**

Run: `PYTHONPATH=tools PYTHONPYCACHEPREFIX=build/pycache python3.12 -m unittest tools/test_plan_lgo_character_model_architecture_gate.py tools/test_lgo_pose_pipeline_gates.py`

Run: `python3.12 -m json.tool build/character-model-architecture-review-01/benchmark-manifest-v1.json >/dev/null`

Run: `python3.12 -m json.tool build/character-model-architecture-review-01/next-action-v1.json >/dev/null`

- [x] **Step 2: Kiểm budget và giao nhau với nhánh review**

Run: `python3.12 tools/report_lgo_change_budget.py --max-files 24 --max-changed-lines 1400`

Run: `git diff --name-only HEAD..origin/codex/vo-pose-div4-review`

Không sửa thêm file runtime nằm trong diff của nhánh review. Commit chỉ được tạo sau khi cả batch planner/evidence pass và diff được review.

### Task 4: Source admission gate sau lỗi alpha lặp lại

**Files:**
- Create: `tools/test_audit_lgo_skeletal_source_contract.py`
- Create: `tools/audit_lgo_skeletal_source_contract.py`
- Modify: architecture spec, lessons và handoff state.

**Interfaces:**
- Consumes: manifest component với canvas/origin/slot/ownership và PNG nguồn.
- Produces: `PASS` hoặc `REJECT_SOURCE_CONTRACT`, danh sách lỗi có thể hành động; không promote runtime.

- [x] **Step 1: Viết test RED cho RGB giả alpha, alpha opaque, sai canvas và ownership chéo slot**

- [x] **Step 2: Xác nhận test fail vì module chưa tồn tại**

- [x] **Step 3: Implement validator và chạy test GREEN**

- [x] **Step 4: Chạy trên single-component ImageGen lần hai**

Expected và observed: exit 2; `OWNERSHIP_CROSSES_SLOT_BOUNDARY`, `CANVAS_DIMENSIONS_MISMATCH`, `MISSING_ALPHA_CHANNEL`. Sau hai lần cùng lớp lỗi, đóng phương pháp raster ImageGen hiện tại cho source probe.

### Task 5: Direct-3D authoring/export/import readiness

**Files:**
- Create: `tools/lgo_blender_modular_3d_probe.py`
- Create: `tools/test_lgo_blender_modular_3d_probe.py`
- Create: `client/Unity/Assets/Game/Foundation/Editor/LgoModular3DImportProbe.cs`
- Modify: benchmark evidence và handoff state.

**Interfaces:**
- Blender tạo `.blend`, `.glb` và `.fbx` build-only từ một contract modular xác định.
- Unity verifier import FBX tạm, đếm clip/SkinnedMeshRenderer/equipment object rồi ghi report; model tạm không được giữ trong `Assets`.

- [x] **Step 1: Viết test RED cho năm slot/action, direct-3D path, soft weight và rigid socket**
- [x] **Step 2: Implement Blender probe, chạy test GREEN và export**
- [x] **Step 3: Re-import GLB trong clean Blender process**
- [x] **Step 4: Export FBX, import tạm bằng Unity 6000.3.2f1 và xóa temp model**

Observed: Blender report `NARROW_TECHNICAL_PASS_TOOLCHAIN_ONLY`; GLB có 1 skin/24 node/5 animation; Blender round-trip PASS; Unity nhận 2 SkinnedMeshRenderer, 5 clip và đủ object trang bị. Đây là readiness của đường dữ liệu, không hoàn thành common task hoặc visual/runtime gate.
