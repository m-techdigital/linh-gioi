# Linh Giới Online — Codex Continuous Work Rules

This repository uses persistent continuous-work mode. Read this file before making changes.

## Quy tắc owner — làm theo batch, tránh vòng lặp nhỏ (2026-09-10)

- Trước khi sửa, chốt một kết quả người chơi có thể kiểm chứng và danh sách thay đổi phụ thuộc trong `NEXT-ACTION.md`. Với batch visual, gom camera/tỷ lệ, asset budget, grounding, HUD/input và ba profile mobile/tablet/PC trước kiểm tra tích hợp. Không chia từng tọa độ, label, texture hoặc profile thành một vòng build riêng.
- Đọc bài học liên quan trước khi xử lý: `docs/art/LGO-MAP01A-ASSET-OPTIMIZATION-LESSONS.md` và audit công việc đã có. Tái sử dụng source/tool/evidence phù hợp; không nghiên cứu hoặc tạo lại từ đầu nếu kết luận còn đúng.
- Dùng source review và kiểm tra nhẹ trong lúc triển khai. Chỉ chạy Unity EditMode/build/capture khi tập thay đổi của batch đã sẵn sàng. Khi cần kiểm nhỏ để chẩn đoán lỗi, chọn đúng test/gate liên quan; không mặc định chạy cả bộ.
- Gom lỗi tìm thấy trong một lượt review thành một danh sách, sửa cùng lượt rồi kiểm lại phần bị ảnh hưởng. Chỉ lặp gate tốn thời gian khi có thay đổi mới liên quan hoặc lỗi chưa giải quyết; ghi rõ trigger. Không chạy lại gate đã pass chỉ vì vừa cập nhật tài liệu/status.
- Nếu cùng một lỗi hoặc kết quả không đạt lặp lại hai lần, dừng thử mò thông số: xem log/evidence và phân tích nguyên nhân trước lần sửa tiếp. Không tạo thêm ảnh hoặc build tiếp với cùng giả định thất bại.
- Ghi một tóm tắt cuối batch: kết quả thực tế, số lượt build/capture và lý do chạy lại, lỗi còn lại, evidence và bước kế tiếp. Không sinh nhiều báo cáo/status hoặc commit cho từng chỉnh nhỏ; không dùng cập nhật trạng thái làm thay thế tiến độ triển khai.
- Không kết thúc công việc chỉ sau một chỉnh nhỏ nếu còn bước an toàn trong batch đã được owner giao. Báo tiến độ ngắn theo kết quả/điểm nghẽn; tránh lặp lại kế hoạch qua nhiều lượt.
- Chốt số pixel hiển thị và ngân sách tải/texture trước khi tạo asset. Không mặc định sinh 2K/4K rồi downsample. Tách module có ý nghĩa, dùng chung atlas theo vòng đời tải; lưu lỗi và cách xử lý đã xác nhận để phiên sau không lặp lại.
- Batch lớn không phải lý do bỏ test bắt buộc, bỏ review Player, nới assertion, sửa frozen surfaces hoặc claim mobile thiết bị thật từ ảnh giả lập tỷ lệ trên macOS. Các chỉ đạo mới của owner được nhập vào batch đang làm, không khởi động lại quy trình từ đầu.

## Operating Loop

- Do not stop after one small task when a valid next task exists.
- In this chat, operate in full continuous-work mode within safe project boundaries: analyze, implement, integrate, clean up, validate, review evidence, update state files, commit/push safe checkpoints, then move to the next roadmap-valid task.
- If runtime/tooling is blocked but other source-safe work remains, record the blocker and evidence path, then continue with a valid task that does not depend on that blocked gate.
- After each task: validate the change, update report/evidence as needed, update `docs/execution/NEXT-ACTION.md`, then continue to the next valid task.
- Use `docs/execution/PROJECT-STATE.md`, `docs/execution/NEXT-ACTION.md`, and `docs/execution/TASK-LEDGER-ROLLUP.md` as the fast handoff spine for future sessions; open full `TASK-LEDGER.md` only when the rollup is insufficient.
- Prefer `python3.12 tools/lgo_state_brief.py` for routine resume/context loading so long marker history does not dominate token usage.
- Keep routine resume output compact: state brief should stay under about 90 lines unless diagnosing a real blocker.
- When running under autopilot, write `build/codex-autopilot/status.json` at the end of each coherent batch.
- If `docs/execution/NEXT-ACTION.md` still contains a valid next task, autopilot status must be `CONTINUE`, not `DONE`.
- Under `tools/lgo_codex_autopilot.sh`, never request approval or escalation; classify blocked local sockets, Unity/player launch, video capture, or runtime permissions in `status.json` instead of waiting for user input.
- `tools/lgo_codex_autopilot.sh` is a local-runtime supervisor and may run Codex CLI with `--dangerously-bypass-approvals-and-sandbox` so localhost sockets and Unity evidence can execute; keep its own bounded rounds/time limits and repository frozen surfaces intact.
- Autopilot should be roadmap-driven, not prompt-fragment-driven: after closing one task, select the next valid task from `NEXT-ACTION.md`, `PROJECT-STATE.md`, milestone roadmap/backlog docs, validators, and latest evidence, then continue within allowed scope.
- Phase/milestone completion is not a stop condition by itself. If gates truly pass and the roadmap has a valid next phase, update project state and continue to that phase.
- If the next phase needs frozen contract/protocol/schema/ADR changes or a major product decision, create the needed request/decision artifact and stop only for that explicit approval gate.
- Owner-facing autopilot progress, `status.json` reason fields, handoff notes, and ledger updates should be Vietnamese so the project owner can understand what is happening during long runs.
- Prefer batches that improve the actual playable game: gameplay already allowed by roadmap, UI/UX, runtime presentation, asset pipeline, performance/weight, maintainability, QA/evidence tooling, and debugging ergonomics.
- If a gate fails, inspect logs and fix the root cause inside allowed paths before stopping. Stop only when the failure is outside allowed scope or the environment truly blocks it.
- If runtime screenshots exist, review them visually. If the UI is ugly, overlapped, blurry, too heavy, or clearly off-reference and the fix is in scope, continue improving rather than reporting success.
- If AI image generation is needed but unavailable in Codex CLI, write an asset request/brief and continue with asset mapping, compression, import settings, UI wiring, and validators that do not require image generation.
- Keep git history clean during continuous work: group related changes into one coherent checkpoint commit after validation, avoid spam commits for tiny edits, and push only through the configured supervisor path.
- Default to fewer commits: commit only after a coherent feature/phase/tooling batch has passed validation, or when the owner explicitly requests handoff/checkpoint packaging. Do not commit merely because one small validator or text edit finished.
- Keep long validation and state dumps concise by default. Use quick/rollup summaries for routine work, and switch to verbose logs only when diagnosing a failure.
- When compiling Python scripts manually, use `PYTHONPYCACHEPREFIX=build/pycache python3.12 -m py_compile ...` so `.pyc` files do not appear under source directories.
- In quick dev-loop profile, skip visual runtime capture by default; force it only for visible runtime changes or before a visual checkpoint.
- Keep change volume proportional to player value. Prefer one coherent improvement that touches existing source over many new task docs, marker-only validators, or repeated status churn.
- Use `python3.12 tools/report_lgo_change_budget.py` during routine work to see file/line churn early. Use `--enforce` only before deliberate checkpoints or handoff gates where over-broad changes should stop the batch.
- Add a new validator only when it protects a recurring failure mode, frozen boundary, package gate, or runtime evidence contract. Routine UI polish should reuse existing validators whenever possible.
- Add or update task/report docs only when they help future operation, handoff, roadmap movement, or evidence review. Do not create documentation just to prove that a tiny edit happened.
- Do not run validation gates in parallel when they share mutable outputs, especially visual evidence directories under `build/visual-evidence/**`; run those phases sequentially to avoid false missing-evidence failures.
- Do not commit generated caches, Unity `Library/Temp/Logs`, pycache, local toolchains, or bulky evidence artifacts unless a task explicitly owns the artifact.
- Prefer source/runtime evidence over assumptions. Never claim PASS from source inspection only.
- Never mask failures with `|| true`.
- Never claim PASS with `executed=0`.
- Keep generated outputs, Unity caches, build folders, pycache, and package artifacts out of source control unless a task explicitly owns the artifact.

## Stop Conditions

Stop only for a real blocker, required frozen contract change, required owner/product decision, unavailable runtime/tooling, unsafe destructive operation, or no valid next action.

Do not stop merely because a task, phase, evidence refresh, or commit just completed. Continue to the next valid action unless every remaining action is blocked or unsafe.

When stopping, update `docs/execution/NEXT-ACTION.md` with the blocker, exact gate, evidence path, and the next allowed action.

Allowed autopilot status values:

- `CONTINUE`
- `BLOCKED`
- `NEED_OWNER_DECISION`
- `NEED_HUMAN_VISUAL_REVIEW`
- `FIX_REQUIRED`
- `DONE`

## Frozen Surfaces

Do not change these without explicit approval and a contract-change task:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

## Runtime / Visual Rules

- Design-first, theo yêu cầu owner 2026-09-07: trước khi triển khai màn/UI/layout, nhân vật, skill, hành trang, vật phẩm hoặc map, phải chỉ ra design/demo cụ thể cùng kịch bản, trạng thái và tương tác cần hỗ trợ. Có mẫu phù hợp thì reuse; thiếu mẫu thì tạo design/demo trước, không tự xây theo cảm giác.
- Bám kịch bản gốc và `docs/02-GDD.md`: Linh Thành là trung tâm, social city và Âm Giới Xâm Lăng là hướng dài hạn. Không biến sân luyện kỹ thuật thành đích sản phẩm hoặc tự mở hệ thống ngoài roadmap.
- Demo mới phải phân biệt rõ draft/đã duyệt và phần runtime hiện có/đề xuất; không tự coi concept là gameplay đã triển khai. Các màn cùng loại dùng base chung; chỉ tạo ngoại lệ khi có design và evidence cụ thể.
- Khi Player đang capture input/ảnh, hoàn tất capture rồi mới mở ảnh review; mất focus giữa smoke phải giữ log thất bại, không nới assertion để claim pass.

- `./tools/lgo_visual_runtime_review.sh` is the visual evidence command.
- Do not use `-nographics` for player visual evidence.
- Build/capture success is not `VISUAL_RUNTIME_PASS`; screenshots must be reviewed.
- If capture fails, classify honestly as `FIX_REQUIRED`, `VISUAL_CAPTURE_TIMEOUT`, `VIDEO_CAPTURE_BLOCKED_ENV`, or `RUNTIME_BLOCKED_ENV`.
