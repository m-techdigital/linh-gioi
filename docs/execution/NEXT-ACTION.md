# NEXT ACTION — Character Hub

## Active owner steer — shared Skill icon base
- Worktree: `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, upstream `origin/feature/2d`; fetch before edit/push.
- Goal: một base vòng ngoài/nội dung icon cho cả Võ/Kiếm/Pháp/Cơ/Linh, tương tự Tiềm năng; không build UI riêng theo class, không dùng icon HUD thay skill.
- Audit: layout 3×3 đã chung nhưng `SharedHudIcons` gán cùng chín HUD icon cho 36 skill ngoài Kiếm. Kiếm có nhánh `KiemSkills` và icon bake vòng.
- Source UI thực có chín skill Kiếm +ba category; những board class khác là reference, chưa có module art đúng 36 skill. Không crop screenshot/reference thành runtime hoặc lấy icon class khác lấp.
- [ ] Tách một ring master +12 nội dung từ nguồn UI đã đăng ký; pack deterministic, provenance/hash/pixel budget.
- [ ] Chuyển skill content sang một library dữ liệu, bỏ builder riêng Kiếm và fallback HUD; UI bind theo skillId/iconId.
- [ ] Shared icon layers cho tree/equipped/detail/category/pet skill; giữ instance khi đổi class; missing artwork phải hiện thật và có danh sách ID thiếu.
- [ ] Test5class/cross-tab, Player3viewport; cập nhật state rồi commit/push theo budget.

## v26 fact rows — prerequisite đã kiểm
- Giữ nguyên chuỗi catalog/giá trị; một component fact block/row cho Skill, Tiềm năng, Linh thú; unknown/overflow giữ nguyên, không suy ra số liệu.
- RED3fail; Player phát hiện value bị đo một dòng rồi wrap do maxWidth phần trăm, sửa column definite và bỏ clamp lặp. Pet caption còn fixed-height26: test rendered có skin Pet tái hiện rồi sửa heightAuto/min26.
- Accepted full graphics EditMode302/302, no fail/skip; Python39/39. Capture87 ảnh tại `build/character-hub-facts-v26/runtime-accepted`. Skill/Potential/Pet đã eye-review ở matrix trước sửa Pet; Pet accepted ba viewport không chồng, tablet dùng bounded scroll.
- Không coi v26 là visual acceptance toàn bộ; tablet Pet còn cần tinh chỉnh cân bằng cột/nội dung sau owner steer Skill.
- Không sửa renderer/class/pose/wardrobe/source, frozen surfaces; generated import/settings drift được lưu rồi loại.

`CONTINUE / VISUAL_FIX_REQUIRED`: cùng functionality phải có base chung; art còn thiếu không được dùng test xanh để che.
