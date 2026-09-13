# LGO outfit body/rig prototype scope — rejected evidence, 2026-09-13

Status: **STOPPED / OWNER REJECTED**. The rigid and skinned Blender body-card Player results kept limbs and torso visually detached. Do not continue this scope, promote its source, or reuse it under another rig/mesh name. The active route is whole-body six-pose outfit authoring in `docs/art/LGO-SIX-POSE-REGISTERED-OUTFIT-PIPELINE-LOCK-v1.md`.

## Phạm vi được phép

Thử nghiệm riêng dưới `ArchitectureProbe`/`build/outfit-body-rig-prototype-2026-09-13`; không thay production body, không thay six-pose authority hiện có và không phục hồi mẫu skeletal/cutout đã bị owner bác.

Đường công cụ duy nhất cho lượt này là Blender local `4.5.5 LTS`: tạo source `.blend` editable, xuất FBX cho Unity probe. Không tiếp tục sleeve-add, sleeve-capsule, pixel deletion, flat-panel direct-fit hoặc tái sử dụng candidate bị bác dưới tên mới.

## Source/rig cần thay trong thử nghiệm

- Body proxy: body flat-card có armature riêng, dùng để kiểm chuyển động và che khuất; chưa phải body production.
- Outfit Pháp có tay: `phap_lv001_outer_top_sleeved_a`, gồm torso, hem, sleeve L/R và cuff bám bone tay.
- Đai/giáp tháo ghép: `equipment__waist_belt` và `equipment__shoulder_chest_guard`.
- Item thứ hai cùng họ: `phap_lv001_outer_top_sleeved_b`, dùng lại cùng rig/bone naming/reuse key; khác material/collar, không có per-pose offset.

## Điều kiện thành công thực tế

1. `.blend` mở lại được và FBX xuất được.
2. Unity probe import FBX, chạy state idle/run/jump/attack/return_to_idle.
3. Trong lúc run, item áo A đổi sang áo B mà animation state không reset.
4. Belt và shoulder/chest guard còn tồn tại/toggle được như slot tháo ghép.
5. Báo cáo phải tách rõ source-ready, Player-runtime và visual-final.

## Điều kiện dừng giả thuyết

Nếu Unity standalone Player tiếp tục bị shader compile/project config kéo dài mà không ra runtime-report, không tạo thêm validator thay thế. Cần sửa capability build/probe nhẹ hoặc môi trường Player capture trước khi claim hành vi Player.
