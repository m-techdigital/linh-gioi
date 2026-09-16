# NEXT ACTION — Character Hub / Skill artwork

## Active owner steer — một base cho cùng chức năng
- Worktree: `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`; branch `codex/character-hub-v22`, upstream `origin/feature/2d`; fetch trước sửa/push.
- Canonical: `redesign-v4-five-tabs`; không mở class/pose/wardrobe/source renderer/camera/frozen hoặc screen khác.
- v27 đã bỏ `SharedHudIcons`, `SharedSkills`, `KiemSkills` và đường fallback HUD. Một `SkillsFor(classId)` đọc `skill-library.json`; 45 record giữ nguyên classId/id/name/level/description từ export Unity trước chuyển đổi.
- Một base `BindLgoSkillIconLayers` dùng lại `CreateLgoCircularIconFrame` của Tiềm năng. 19 instance Skill gồm tree9/equipped4/category3/detail1/pet2 giữ cùng Sprite master và không dựng lại khi đổi class.
- Atlas Skill 512×512/252747byte: một `frame` +12 `inner-symbol`, cell128px. Generator `tools/pack_lgo_skill_icons.py`; source UI pin SHA, registered centers, ring/content mask riêng, max import512 và budget400000. Không crop screenshot/reference gameplay.
- Đã xuất13PNG rời tại `build/character-hub-skill-base-v27/modules/`; ảnh kiểm3cột `frame-content-composite.png` (frame/content/assembled).
- Proof chuyển45record: `catalog-before.json`, `catalog-migration-proof.json` trong evidence v27. Thay36 iconId từ HUD sang đúng skillId chưa có art, không đổi tên/cấp/mô tả thành dữ liệu mới.

## Việc tiếp theo — artwork đúng 36 skill, không viết UI riêng
- [ ] Bổ sung36 inner artworks Võ/Pháp/Cơ/Linh theo `build/character-hub-skill-base-v27/missing-artwork.json`; nguồn cần kiểm semantically với tên skill, không gán lại HUD hoặc dùng Kiếm thay.
- [ ] Đăng ký artwork có provenance, cùng cell/aperture128px và cùng pipeline; chỉ mở rộng data/atlas, không thêm builder/layout theo class.
- [ ] Kiểm đủ tree/equipped/detail, selection theo class, ring cùng instance và không rò sang Tiềm năng; Player PC/mobile/tablet rồi eye audit.
- Missing art hiện hiển thị tên +“Chưa có icon”; đây là trạng thái thiếu nội dung, KHÔNG phải icon hoàn thiện hay visual acceptance.

## Bằng chứng kỹ thuật / giới hạn
- v26 prerequisite commit `cfe8e5e8`:302/302 graphics EditMode,39Python; fact rows bảo toàn giá trị, sửa wrap value và fixed-height Pet. Tablet Pet còn dùng bounded scroll.
- v27 initial full304/304 và87frame. Review đủ5class×3viewport +4tabPC; ring/content khớp và HUD sai không còn. Review source bắt tooltip cũ khi đổi Võ→Kiếm; thêm RED và sửa bind tooltip cùng dữ liệu.
- Final evidence: `build/character-hub-skill-base-v27/{final-editmode.xml,final-build.log,runtime-final,review.json}`; kiểm kết quả thực trước push. Python42 gồm3packer+7skinasset+25governance+7capture; replay PNG phải giống từng byte.
- Không claim production skill balance/behavior hoặc đủ artwork5class. Chỉ9skillKiếm+3category có nguồn UI hiện hành. Review độc lập Codex không được tính PASS; mobile/tablet là viewport macOS, không phải thiết bị thật.

`CONTINUE / VISUAL_FIX_REQUIRED / ARTWORK_INCOMPLETE` — thiếu art phải giải quyết ở content pipeline chung, không che bằng icon sai hay test xanh.
