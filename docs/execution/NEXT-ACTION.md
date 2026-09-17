# NEXT ACTION — Whole Product / after P0 control-integrity closure

1. P0 is closed from current-run Player evidence. Preserve review-hotkey gating, truthful pointer/touch Menu help, foreground input blocking and Character Hub V59; do not reopen those without a concrete regression.
2. Next subproject: **Product Auth Foundation**. Replace the presentation-only login path with a real product auth/session boundary while keeping `/dev/auth/login` development-only and without leaking secrets into PlayerPrefs/logs.
3. Product Auth scope for the next design/plan: login credential contract, session/token lifecycle, logout, expiry/unauthorized handling, client secure session state, Java API endpoints/storage boundary, deterministic tests and Player-visible error/loading states. Register and password-recovery remain the following independent subproject; do not fold them into the first auth slice.
4. Carry forward visual debt: Character Select mobile landscape partially hides the left secondary motto behind the stage. Fix it with the Character persistence/create-character work, not inside auth.
5. Every visible task from now on must preserve before/after screenshots plus intermediate defect evidence under its task evidence directory.
6. Design-change gate: preserve approved design when viable. If a screen is missing, contradictory, infeasible, or materially weak in Player, create an old-vs-proposed-new visual comparison (matching PC/mobile/tablet viewports where relevant) and obtain owner approval before replacing the canonical visual design. Bug/fidelity fixes that implement the existing approved design do not require a new design approval.

# NEXT ACTION — Character Hub / after v55

1. V53 Pháp Trận is already published/reused; do not rerun its completed Unity/build/capture evidence.
2. V54 Kình Lực is rejected and not published because the candidate had only class-level VFX evidence, not a direct skill mapping. Do not infer it from Chiến Ý.
3. V55 Cơ Trận now uses the direct Hệ Thống Cơ Trận ground-deploy-circle motif; keep it unless a concrete Player defect appears.
4. Next: audit exact/direct mappings only, starting with Kiếm Ngự Kiếm ↔ Ngự Kiếm. Keep current art if its sword-control motif is already readable; otherwise redraw only with direct evidence.
5. Preserve shared frame/content split, exact ownership, actor/gameplay, branch/worktree. Build/capture only if runtime art changes.

# NEXT ACTION — Character Hub / after v53

1. Audit current Pháp `Kết Giới` against direct VFX `Kết giới (Field)`; keep it if current circular-field motif is already readable.
2. Do not remap Hỏa/Băng/Lôi/Linh/Trọng Lực/Nguyên Tố/Tinh Thần solely by name similarity; require direct visual or semantic evidence.
3. Continue Cơ/Pháp only for concrete object-vs-action/palette defects; do not regress repaired Võ/Linh/Hỏa Tuyến/Thanh Tẩy.
4. Keep shared frame/content separation, exact ownership and actor; build/capture only when runtime art changes.

# NEXT ACTION — Character Hub / after v52

1. Audit Pháp against its class demo first because several runtime/demo labels have direct matches; fix only direct motif mismatches.
2. For Cơ, keep `Tháp Cơ`, `Thiết Vệ`, `Hỏa Tuyến`, `Cơ Trận` unless a concrete defect is found. Do not force-map ambiguous `Cơ Lôi`, `Truy Kích`, `Linh Cơ`, `Pháo Kích`, `Cơ Nỏ`.
3. Keep Võ `Kình Lực` unchanged until a direct source/semantic contract exists.
4. Keep shared frame/content separation, exact ownership and actor; build/capture only when runtime art changes.

# NEXT ACTION — Character Hub / after v51

1. Do not force-map Võ `Kình Lực` to `Chiến Ý`; current evidence is insufficient. Revisit only with a direct source/semantic contract.
2. Audit Cơ/Pháp remaining skill icons against their own class demo boards, starting with obvious object-vs-action or wrong-palette cases.
3. Keep Hỏa Tuyến/Thanh Tẩy and the repaired Linh set unchanged unless a concrete visual defect is demonstrated at 64/128px.
4. Keep shared frame/content separation, exact ownership and actor; build/capture only when runtime art changes.

# NEXT ACTION — Character Hub / v50 onward

1. Audit Võ `Kình Lực` against direct Võ demo motif; replace only with evidenced action/energy artwork, not object/generic emblem.
2. Audit Linh `Hộ Mệnh` and other remaining Linh icons for direct Hộ/Linh Vực/support mappings; do not infer gameplay aliases.
3. Keep Hỏa Tuyến/Thanh Tẩy unchanged unless a concrete 64/128px defect is found; current motifs remain readable.
4. Keep shared frame/content separation, exact ownership and actor. Build/capture only when runtime art changes.

# NEXT ACTION — Character Hub / Skill art theo demo

## Active — sau v49 Linh Phù/Trói Hồn
- Giữ worktree/branch hiện hành; không reset/restore source.
- v49 redraw `linh_skill_5` Linh Phù → Ấn Linh và `linh_skill_6` Trói Hồn → Linh Trói ở mức visual-only; native transparent tím/trắng, không source crop/gamma/UI scaling.
- Exactly2module đổi,47module+shared frame giữ nguyên; actor/runtime names/levels/gameplay không đổi. Packer13/13, Unity315/315, build0error0warning,132frames/3viewport, eye3 selected. Chưa owner-accept toàn class/5tab.
- Tiếp theo: audit Linh Hồi Phục/Hộ Mệnh/Cộng Hưởng/Linh Giới và Võ Kình Lực theo demo; chỉ sửa khi mapping visual có bằng chứng rõ.

## Active — sau v48 Bộ Pháp/Đột Kích readability
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`; không đổi/reset/restore source.
- v48 redraw native transparent `vo_skill_5` Bộ Pháp và `vo_skill_7` Đột Kích: silhouette/nét sáng và dày hơn ở64–128px, không gamma/tint/UI scale; visual-only mapping Bước Thần/Lướt áp sát, không đổi gameplay.
- Shared frame/actor/runtime IDs/names/levels giữ nguyên; đúng2module đổi,47module còn lại giữ nguyên. Unity315/315, packer13/13, build0error0warning,132frame/3viewport; eye-review3 selected. Chưa owner-accept toàn class/5tab.
- Bước tiếp theo: re-audit Hỏa Tuyến/Thanh Tẩy line-weight ở64–128px và tiếp tục các icon Võ/Linh còn object-vs-action drift; chỉ build/capture khi runtime art đổi.

## After v47
1. Re-audit Hỏa Tuyến and Thanh Tẩy beside the new Võ group at64/128px; correct line-weight/content, not gamma.
2. Polish Bộ Pháp/Đột Kích silhouettes against Bước Thần/Lướt áp sát while keeping current visual-only mapping and shared frame.
3. Audit remaining Võ Kình Lực/Hộ Thể/Phá Giáp/Phản Đòn for obvious object-vs-action drift; change only when demo mapping is supported.
4. Build/capture only after runtime art changes; preserve actor, runtime IDs/names/levels and exact ownership.

# NEXT ACTION — Character Hub / continue visual skill alignment

## v46 next
1. Refine Võ Liên Kích and Chấn Kình against pinned Liên Quyền/Xung Kích demo motifs; reject hand/armor/object substitutes.
2. Revisit Bộ Pháp/Đột Kích at 64–128px: increase readable action silhouette by redraw, not gamma/UI scale; keep visual-only mapping and shared frame.
3. Re-audit Hỏa Tuyến/Thanh Tẩy together with the new Võ group for line-weight consistency.
4. Only build/capture again when runtime artwork changes; preserve actor, runtime IDs/names/levels and exact ownership.

# NEXT ACTION — Character Hub / tiếp tục hoàn thiện artwork Skill

## Active
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`. Kiểm pause, claims, job và tác dụng phụ trước batch; không tác động phiên khác. Cuối lượt gọi `v3.finish` với CONTINUE khi còn bước hợp lệ.
- Canonical vẫn `redesign-v4-five-tabs`. Giữ shared base, vòng/content độc lập, exact ownership, actor và frozen surfaces. Không đổi tên/cấp/gameplay theo nhãn demo.

## v45 — hai nguồn được thay, chưa nghiệm thu toàn mỹ thuật
- Hỏa Tuyến `co_skill_8`: thay nét hình học bằng các dải hỏa lực có nét lửa và tia sáng; loại nền vuông trung tính bằng alpha, không tăng gamma hoặc sửa RGB nguồn.
- Thanh Tẩy `linh_skill_4`: ba linh thể có nét vẽ và áo linh lực, thay ba hình khối cũ. Loại vòng sáng sinh kèm bằng mask theo tọa độ native 384 px; giữ các đầu, thân và áo. Không dùng vòng sinh kèm làm frame thứ hai.
- Bản mask đầu sai tọa độ làm mất một phần mặt và giữ vòng bên phải đã bị loại. Kiểm điểm ảnh tái hiện lỗi rồi pass sau khi sửa. Bản Hỏa Tuyến còn nền vuông cũng bị loại trước build.
- Liên Kích mới bị loại vì chỉ có một bàn tay, mất ảnh quyền liên hoàn. Hai variant bị model checker chặn không được lưu, nhập hoặc retry. Giữ nguồn Liên Kích v43 và Chấn Kình cũ, không nhận đã sửa hai món đó.
- Atlas 1024×1024, 880.771 byte; vẫn 49 module và 45 skill có hình. Chỉ hai nội dung trên đổi; 47 module khác, frame, năm exact designBindings, skill library, item atlas và code runtime giữ nguyên.
- Source native 384 px, padding chung 32 px vào 448 px; không bbox-fit, scale/offset riêng trong UI hoặc chỉnh gamma. Alpha và PNG portable tái dựng từ raw được kiểm riêng; không hứa model sinh lại đúng pixel.
- Một Player build, hai lượt chọn node 3 và 7 trên cùng binary để xem cả Thanh Tẩy/Hỏa Tuyến ở bảng chi tiết. Tổng 264 ảnh; đã xem 10 ảnh, gồm hai skill trên ba viewport và Võ/Rương đồ/Tiềm năng/Linh thú PC.
- Bộ Võ/Linh/Cơ vẫn chưa đồng nhất mỹ thuật. Hỏa Tuyến còn một đường cong và thiếu dấu hiệu cơ giới rõ; các chi tiết nhỏ của Thanh Tẩy vẫn cần so với toàn bộ demo. Không dùng số lượng sprite hoặc test xanh để gọi hoàn thành.

## Bước tiếp theo cụ thể
1. Tiếp tục nhóm Võ còn sai hình tác động: đối chiếu Bộ Pháp/Đột Kích/Quyền Ý với các vùng Bước Thần/Lướt áp sát/Ý Chí Võ Đạo trong demo Võ. Khóa visual-only mapping trước, không gán alias gameplay; nguồn mới phải thể hiện chuyển động/quyền khí, không vật phẩm hoặc portrait.
2. Liên Kích/Chấn Kình vẫn còn việc. Không dùng các bản bàn tay đơn/giáp/huy hiệu bị loại hoặc lặp lại các lần model đã bị chặn để lấp chỗ.
3. Tiếp tục kiểm độ nhất quán của Hỏa Tuyến/Thanh Tẩy với các icon còn lại ở 64–128 px; sửa artwork khi có sai lệch cụ thể, không thêm vòng gamma hoặc phóng UI riêng.
4. Chỉ chuyển READY_REVIEW khi toàn bộ năm tab thực sự sát canonical và có audit Player đầy đủ. Kết thúc batch không phải nghiệm thu task.

## Evidence và nguồn
`build/character-hub-skill-paint-v45/`: `runtime-review.json`, `source-comparison-final.png` (nguồn, không phải Player), `final-sources/`, `artwork-registry-final.json`, `import-final-proof.json`, `full-editmode.xml`, `runtime-reviewed/{cleanse,fireline}/`, `player-reviewed/LinhGioiOnline.app`.
`lgo-skill-paint-v45-authoring.zip` cùng `.zip.sha256` và `package-proof.json`: nguồn tích lũy gồm cả Linh Thuẫn v44; raw được chọn, mask, recipe và registry portable. Không import ZIP/reference board vào Resources; không có font/model/Player trong gói.
`CONTINUE / PARTIAL_SKILL_PAINT_ALIGNMENT / VISUAL_FIX_REQUIRED` — không nghiệm thu toàn class, Character Hub hoặc thiết bị mobile/tablet thật.
