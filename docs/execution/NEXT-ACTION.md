# Linh Giới Online — Next Action

Last updated: `2026-09-08`

## Quick Resume

- Checkpoint macro REMAKE keeper v4: đã thay toàn thân, đầu/mặt/tóc, nón, giáp, robe/cape và portrait bằng mesh 3D mới bám sheet; source `client/art-source/gate-keeper/KeeperReconstruction.blend`, 24.898 triangles, 65 bones, 2 atlas. Import/build `keeper-clothed-*`, 19/19 tests guide/dialogue `keeper-reconstruction-tests.xml`, quick gate và toàn tuyến runtime 960x540 `keeper-clothed-mobile.log` qua; ảnh keeper-side/dialogue/keeper-guide-direction đã xem. CHƯA VISUAL PASS: cape vẫn tạo tam giác khi IK chỉ đường (P1), hoa văn/nón/mép vải chưa sắc như design. NEXT: tách và gắn xương cape theo cấu trúc trang phục thực, giữ model remake; xử lý cả vùng biến dạng trong một macro pass rồi capture pose IK thật. Không quay lại primitive, không coi tool là blocker, không đổi frozen surfaces. Evidence tại `build/visual-evidence/onboarding-blockout/keeper-clothed-mobile/`; capture timeout cũ được giữ nguyên.

- Người Giữ Cổng chào bằng lời trong guidance chung cùng nhịp cử chỉ; lần quay lại có lời chào riêng. Hoàn tất “Khám phá tiếp” thì bàn tay/thân chỉ về sân nghỉ và guidance cùng đích; đóng giữa chừng không chỉ, mở lại hủy ngay. Runtime red `guide-social-cues-mobile.log` bắt lỗi hủy muộn sau LateUpdate; setter trạng thái thoại nay hủy đồng bộ, giữ assertion. `guide-social-tests.xml`19/19; build/quick `guide-social-final-*` và `build/dev-loop/guide-social-final-mobile.log` toàn tuyến/greeting hết hạn/movement/return hướng sân/cancel/revisit qua. Ảnh greeting/welcome-back/guide-forecourt960x540 đã xem tại `build/visual-evidence/onboarding-blockout/guide-social-final-mobile/`. Không bubble mới, quest/reward hoặc art final. NEXT: kiểm tra fit/pose trang phục tại góc chơi/animation thật theo v4; chọn sửa lỗi hình thể hoặc flow có evidence, không lặp trang trí nhỏ hoặc báo cáo-only.

- Đình nghỉ có cột/xà/giằng gỗ đồng bộ với ghế theo demo sân, một frame mesh ghép16 phần và material ghế dùng chung; collider cột và mái giữ nguyên. `pavilion-frame-final-build.log`/`pavilion-frame-quick.log` qua; `build/dev-loop/pavilion-frame-mobile.log` toàn tuyến và đi vào đình/camera thấy toàn thân/quay ra qua. Ảnh forecourt/pavilion-entry960x540 đã xem trong `build/visual-evidence/onboarding-blockout/pavilion-frame-mobile/`. Build thử `pavilion-frame-route-build.log` từng lỗi biến harness trùng tên, đã sửa và build lại; không nới assertion. Chưa art final, tương tác ngồi hoặc thiết bị thật. NEXT: chuyển sang fit/pose nhân vật và flow SCN-001/002 theo thứ tự bàn giao; đọc evidence thực để chọn vấn đề player-visible, không kéo dài chuỗi thêm chi tiết trang trí sân.

- Sân nghỉ: thay ghế khối đặc bằng ghế gỗ có nan, chân và tựa hướng vườn theo demo phố/sân; giữ footprint, collider cao theo tựa, một mesh/renderer. Build `pavilion-rest-final-build.log` và quick `pavilion-rest-final-quick.log` qua; `build/dev-loop/pavilion-rest-final-mobile.log` toàn tuyến SCN-001/002/đi sân/quay lại NPC qua, ảnh forecourt/street-outlook960x540 đã xem tại `build/visual-evidence/onboarding-blockout/pavilion-rest-final-mobile/`. Dải răng cưa tưởng trên tường là tên stress-test WWWW, không phải shadow: giữ chẩn đoán `pavilion-shadow-isolation-mobile`, bỏ thay đổi ánh sáng thử nghiệm; capture cảnh quan dùng Minh An sau kiểm tra tên rộng/camera edge. Chưa tương tác ngồi/art final/thiết bị mobile thật. NEXT: tiếp hoàn thiện đình nghỉ/phố theo demo, ưu tiên cấu trúc gỗ/mái và readability tại camera chơi; không mở gameplay ngoài SCN-001/002.

- Đá Luyện có bề mặt xanh xám sáng hơn và dấu xoắn cộng hưởng theo reference SCN-002; nét dày hơn sau review960x540, căn giữa/rộng20cm trên hai mặt. Một mesh128tri dùng chung và material pulse cũ, không texture/đèn/collider mới; giữ đá1.1m và completion/revisit. `build/dev-loop/stone-inlay-readable-mobile.log` toàn tuyến/mesh hướng ngoài/bounds/pulse/phục hồi/đi sân/quay lại NPC qua; ảnh stone-side/complete/complete-settled đã xem trong `build/visual-evidence/onboarding-blockout/stone-inlay-readable-mobile/`. Build `stone-inlay-readable-build.log`, quick `stone-inlay-readable-quick.log` qua. Chưa art final, thiết bị thật, mọi viewport hoặc VISUAL_RUNTIME_PASS. NEXT: tiếp phố/sân và flow khám phá theo storyboard/design sẵn có; không dừng ở polish biểu tượng hoặc validator, giữ base chung và frozen surfaces.

- Checkpoint Người Giữ Cổng trẻ v4 candidate: model/portrait cùng identity nón/tóc đen/áo xanh-ngà đã vào runtime; garment tách vạt trước/tà bên/cape, tay áo liên tục, sửa mặt bị cull và chuyển màu Blender→Unity. Reuse rig/controller/thoại/IK chỉ đá, không đổi gameplay contract. Hai FBX arrival/keeper có65 xương/rest matrix khớp tuyệt đối; keeper13.074 vertices/24.764 triangles sau round-trip,1renderer/6material, dùng lại Skin512/Eyes128. Chưa phải final art/modular assembler, chưa body occlusion/LOD/áo choàng chạy hoặc GPU/mobile-device proof.

- Evidence checkpoint: `build/dev-loop/keeper-v4-tests.xml`19/19; `keeper-v4-final-mobile.log` toàn tuyến SCN-001/002 với hình học mới (trước chỉnh màu); `keeper-v4-color-mobile.log` và `keeper-v4-desktop-diagnostic.log` presentation màu cuối960x540/1920x1080, ảnh đã xem tại `build/visual-evidence/onboarding-blockout/` theo tên tương ứng. Quick `keeper-v4-color-quick.log` qua; build Player thật qua. `keeper-v4-color-desktop.log` từng fail movement-during-greeting, chưa tái hiện khi chỉ thêm diagnostic, giữ log và không claim đã sửa nguyên nhân. Không VISUAL_RUNTIME_PASS/art final. Backup dirty model cũ ở `build/asset-staging/gate-keeper-v4/runtime-before-v4/`; recipe/input/export SHA trong `surface-review-07/asset-metrics.json`. Chênh app local không phải isolated asset build delta.

- NEXT ưu tiên player-visible: đối chiếu storyboard SCN-001/002 để cải thiện nhận diện Đá Luyện/phố-sân tại camera chơi và tiếp fit/chi tiết trang phục theo sheet v4 khi có mẫu đủ cụ thể; reuse base/rig/material, không mở class/skill hàng loạt. Trước claim pipeline nhiều outfit cần pose walking/occlusion và số đo texture memory/render cost thật. Nếu greeting desktop fail lại, dùng diagnostic distance/conversing/focused/frame_dt hiện có để tìm nguyên nhân, không nới assertion. Không quay lại mẫu trưởng bối hoặc các experiment bị loại.

- Người Giữ Cổng nay là Humanoid3D theo turnaround DRAFT, portrait128 render từ cùng model; shared generator/importer/instantiation và base thoại nhận portrait tùy chọn. Idle/quay về người chơi khi thoại/trở về hướng đón khách; collider/range giữ nguyên. Red sprite rồi sửa grounding bằng mesh bake (không dùng culling bounds), tà áo chuyển sang hip support sau review, nâng origin4cm theo số đo. `keeper-robe-{mobile,desktop,tablet}` full route/ground/idle/facing/portrait qua, ảnh đã xem; `keeper-hall-mobile` hai visit và `keeper-shared-import-regression` qua; quick qua. FBX1199548B, portrait20599B, không texture da/mắt/clip mới; đây là source bytes, không build delta. Gom cả seal pulse; 22 file gồm13 asset/meta/reference vượt advisory18, không đổi ngưỡng chung. Chưa art final/visual PASS, tư thế chờ còn cứng. Next: Kiểm chứng Idle_Talking_Loop CC0 và thiết kế cử chỉ giao tiếp Người Giữ Cổng theo turnaround/dialogue demo; tư thế chờ hiện còn cứng. Reuse importer/rig, chỉ thêm clip cần dùng nếu ảnh runtime chứng minh hợp vai trò; giữ root/collider/range, không mở canon/quest/production systems.

- Cử chỉ Đá Luyện dùng Interact Humanoid CC0 dài2s, clip+meta239571B; root motion tắt, tự về locomotion và di chuyển hủy. Red `stone-gesture-early-trace` bắt lệnh crossfade chưa xuất hiện trong Animator state; sửa hủy theo quyền sở hữu yêu cầu, `stone-gesture-early-green` qua. `stone-gesture-final-mobile` full route/hủy giữa động tác, `stone-gesture-final-desktop` full route/tự kết thúc qua; ảnh đã xem. Tablet `stone-gesture-green-tablet` chỉ reuse hình/pose trước sửa early-input, không final-tablet cancellation claim. `stone-gesture-hall-mobile` hai visit/profile/lifecycle qua; importer rerun cùng SHA, quick qua. Không IK/tay chạm mặt đá, combat animation, art final hoặc visual PASS. Next: Đối chiếu khung3 storyboard để làm phản hồi linh khí tại Đá Luyện rõ hơn: nối nhịp sáng dấu đá với cử chỉ và pulse hiện có, giữ asset nhẹ, không thêm đèn/texture lớn hoặc mở skill/quest mới.

- Đá Luyện đổi từ cột nhọn1.5m sang khối thấp1.1m/vai rộng, dấu vàng mặt trước và mặt hướng đường bám slope mesh; chung cube/material, không texture/light mới. Collider0.65x1.5x0.65 giữ nguyên, không claim khớp bề mặt vật lý/art final. Red `stone-silhouette-red` bắt khối cao/mỏng; `stone-silhouette-green-{mobile,desktop,tablet}` silhouette/full route/focus/pulse/camera qua, ảnh đã xem; `stone-silhouette-hall-mobile` hai visit/profile/lifecycle qua, ảnh về sảnh đã xem; quick qua. Không visual PASS/GPU benchmark/khắc chữ canon hoặc production systems. Main không đổi, reuse evidence trước đúng scope. Next: Kiểm chứng clip Interact trong build/asset-staging/quaternius-animation/UAL1_Standard.fbx (CC0), reuse pipeline Humanoid hiện có để nhân vật có cử chỉ khi chạm Đá Luyện theo storyboard. Chỉ import clip cần dùng, root motion tắt, input di chuyển hủy cử chỉ; giữ quest/range/pulse và kiểm tra kích thước clip/lifecycle.

- Guidance main/phố dùng cột và text primitive chung, không chip lồng; phố dùng NewWorldHudRoot và bounded-scroll skin chung, trần30% safe height. Red `guidance-group-red` bắt frame từng dòng. Bản `guidance-group-green-mobile` bị loại sau review vì scrollbar trắng/đệm dư; bỏ group padding và reuse overflow guard, không nới trần. Final `guidance-group-final-{mobile,desktop,tablet}` full route, nội dung thường không cuộn,40 dòng cuộn thật/không che pad qua; ảnh đã xem. `guidance-group-main-desktop` capture/manifest qua, ảnh world-hub đã xem; quick qua. Không asset mới/physical touch, resize trong khi cuộn-thoại, mọi viewport hoặc visual PASS/art final. Next: Đối chiếu Đá Luyện trong storyboard nhập môn, cải thiện hình khối và dấu nhận diện trên đá thay bia nhọn blockout; ưu tiên procedural mesh/material nhẹ, giữ collider/range/feedback hiện có, không mở skill hoặc quest mới.

- Tên hồ sơ nay hiện trên nhân vật bằng WorldLabelPresenter; HUD ghi Linh Môn. Helper chung neo theo bounds/camera và tránh nhãn NPC/đá bằng screen bounds, ẩn/khôi phục trong thoại. Red thiếu nhãn ở `player-identity-red`, red chồng NPC ở `player-identity-green-mobile`; build lỗi accessibility đã sửa tại API World/UI. `player-identity-separated-{mobile,desktop}` full route/tên16 ký tự/thoại qua; `player-identity-confirm-tablet` thêm đá/camera-alley/completion qua; ảnh đã xem. Lượt `player-identity-verified-tablet` thoát sớm, không tính full route, chưa kết luận nguyên nhân. `player-identity-hall-mobile` hai visit/profile/cleanup nhãn-bóng qua; `player-identity-main-regression` technical capture/manifest qua; ảnh đã xem, quick qua. Không asset mới, không mọi vị trí/physical Escape/visual PASS/art final hoặc mapping outfit/gender. Next: Đối chiếu storyboard để gom địa điểm/mục tiêu/chỉ dẫn thành một nhóm HUD chung, bỏ ba dải status-chip rời hiện tại; giữ scroll/safe bounds và trạng thái thoại/completion cho main/phố, không thêm asset hoặc skin riêng.

- Build development mặc định vào Linh Môn từ nút Vào game, không cần cờ onboarding; `--lgo-technical-yard` giữ fixture cũ và luôn ưu tiên, policy release giữ main. `hall-default-route-green.xml`8/8 (red2/8). Review tìm race chọn lại slot khi tải: `hall-entry-reselect-red.log` tái hiện; đã khóa entry/roster, snapshot ID, finally mở khóa. Final `hall-entry-guard-green-{mobile,desktop,tablet}.log` không cờ: lỗi hồ sơ thiếu phục hồi, hai vòng UI Submit/reselection guard/movement/profile/ownership qua, ảnh đã xem; `hall-default-technical-regression.log` harness cũ capture/manifest qua, ảnh đã xem. Quick qua. Không asset mới, không release-binary/cancellation/physical-key/full-lag hoặc visual PASS/art final; model chung chưa mapping outfit/gender, không quest persistence/combat production trong phố. Next: Đối chiếu reference world rồi làm rõ nhận diện người chơi tại Linh Môn: tên hồ sơ trên nhãn nhân vật, tên khu vực trong HUD, dùng base nhãn chung; kiểm tra tên dài/cạnh NPC/thoại, không thêm model hoặc texture nặng.

- Gặp/Luyện/Đã xong dùng glyph hội thoại/bàn tay/check qua một helper chung ở main và phố; tooltip runtime dùng font/skin chung vì Unity tooltip chỉ hỗ trợ Editor. Ba PNG64 tổng3937B, license3208B, không mipmap/readable; generator khóa Lucide1.21.0/resvg2.6.2, tái tạo cùng SHA. Red icon/tooltip và bounds giữ ở `interaction-icon-red`, `interaction-tooltip-red`, `interaction-icon-bounds-red`; đã bỏ padding nút chữ và đổi phép đo rounding sang pixel thực. Final `interaction-tooltip-green-{mobile,desktop,tablet}.log` toàn tuyến/icon/hover-event/safe-bounds qua, ảnh đã xem; `interaction-tooltip-main-desktop.log` capture/manifest qua, ảnh đã xem. Main mobile trước tooltip ở `interaction-icon-main-mobile.log` giữ provenance riêng. Quick qua. Không physical pointer/touch, missing-asset fault injection, GC/GPU benchmark hoặc visual PASS/final art. Next: Kiểm tra gate để sảnh vào Linh Môn mặc định trong build development; giữ đường vào sân kỹ thuật qua cờ và không đổi production. Bám storyboard, test hai vòng vào/về sảnh, lựa chọn nhân vật và cleanup trước chuyển.

- Người Giữ Cổng có vòng focus vàng khi trong tầm, reuse helper/sprite Đá Luyện; ẩn ngoài tầm, trong thoại và sau hoàn tất, đóng giữa chừng trả lại focus. Red `keeper-focus-red.log` thiếu vòng; `keeper-focus-green-mobile.log` giữ lỗi test thiếu bước chờ Update, đã sửa test không nới assertion. Final `keeper-focus-verified-{mobile,desktop,tablet}.log` toàn tuyến/focus qua (960x540,1920x1080,1366x1024), ảnh keeper-side đã xem; thoại mobile đã xem; `keeper-focus-hall-return-mobile.log` hai visit/ownership qua, ảnh về sảnh đã xem; quick qua. Không ảnh import mới, không visual PASS/final art; chưa test focus khi quay lại NPC sau completion hoặc Escape vật lý. Next: nút Gặp/Luyện có biểu tượng hội thoại/bàn tay theo storyboard, dùng base touch action chung và asset nhỏ phù hợp, không mở gameplay mới.

- Sân nối phố: năm nhà xa dùng module/mặt tiền chung, xoay về sân và không collider; bồn cây thêm đất/bóng tiếp xúc bằng helper có sẵn; guidance xác nhận đến sân rồi ẩn, quay lại không phát lại. Bản tháp trắng `street-scenery-green-mobile` đã loại sau review. Red tại `street-scenery-red`, `garden-grounding-red`, `forecourt-arrival-red`. Final `forecourt-arrival-green-{mobile,desktop,tablet}.log` full route/scenery/grounding/feedback/reentry qua, ảnh đã xem; `street-garden-hall-return-mobile` hai vòng lifecycle qua, ảnh về sảnh đã xem; quick qua. Không ảnh import mới/quest persistence/art final/visual PASS/GPU benchmark. Next: vòng focus báo Người Giữ Cổng trong tầm tương tác, reuse base focus của Đá Luyện; tắt khi thoại mở hoặc rời tầm, không đổi range/quest.

- Batch presentation: Trói Bóng bỏ cube hồng trùng, giữ một telegraph cho alert/preview; `shadow-warning-owner-green-{desktop,mobile}` main capture/manifest và lifecycle qua, ảnh đã xem. Phố reuse Skybox/Procedural đã serialize, chỉ đổi camera clear từ SolidColor sang Skybox; `street-sky-red` xác nhận root, `street-sky-green-{mobile,desktop}` full route qua/ảnh đã xem; `street-sky-hall-return-mobile` hai vòng giữ nguyên skybox/ambient/camera, ảnh về sảnh đã xem. Quick/M5 pose/M6 skill qua; không texture mới, không visual PASS/final sky/clouds hoặc missing-sprite fault-injection claim. Next: cảnh mái nhà ngoài tường sân theo vùng scenery của `lgo-street-forecourt-draft-v1.jpg`, reuse module/mesh/material, không mở lối đi hoặc gameplay mới.

- Trói Bóng: Player red `shadow-warning-owner-red.log` xác nhận cube Alert Warning bật cùng telegraph, URP/Lit/màu hồng chứ không shader missing. Đã gom cảnh báo vào vòng hiện có, bỏ cube trùng; fallback chỉ tạo khi thiếu sprite. `shadow-warning-owner-green-{desktop,mobile}.log` exit0/capture/manifest, cảnh báo còn sau timeout và ẩn khi reset qua; ảnh đã xem, quick/M5 pose/M6 skill qua. Chưa commit để gom batch presentation; không ảnh mới/visual PASS, chưa fault-inject missing sprite. Next: nền trời ban ngày nhẹ cho preview phố theo `lgo-street-forecourt-draft-v1.jpg`, giữ camera/lighting và phục hồi môi trường khi về sảnh.

- Panel kỹ năng đã bỏ khung lồng/tiêu đề lặp và override cyan; reuse nút compact chung, ba cột đều. HUD chung cuộn dọc, max-height đo từ mép trên thực tới pad trong safe-panel units. Red `skill-panel-red` bắt cyan; `skill-panel-bounded-mobile` bắt overflow do chỉ trừ header minimum. Final `skill-panel-measured-{desktop,tablet,mobile}.log` exit0/capture/manifest, cuộn nội dung dài qua; ảnh đã xem, quick và hai validator liên quan qua. Mobile giữ skill ở cụm đáy. Shared Menu/Về sảnh lên góc phải, resize/main ba profile qua; phố có sáu đèn reuse mesh/material, `street-hud-final-{mobile,desktop,tablet}` route qua/ảnh đã xem, `street-hud-hall-return-mobile` hai vòng phục hồi header/lifecycle qua. Không ảnh import mới, không physical Escape/mobile-device/visual PASS/final art. Next: truy renderer tạo ô hồng ở Trói Bóng; kiểm chứng fallback/telegraph ownership rồi sửa theo demo, không tăng payload.

- Nối sảnh -> phố candidate bằng `--lgo-onboarding-from-lobby` trong development; default main giữ nguyên. `hall-onboarding-verified-{mobile,desktop,tablet}.log` hai vòng/profile, movement/button/handler Escape, API profile/selection/session và lifecycle qua;8 checkpoint/manifest mỗi profile, ảnh đã xem. Main `hall-onboarding-main-confirm.log` exit0/capture/manifest, ảnh world/menu đã xem; quick `hall-onboarding-verified-quick.log` qua. Red thiếu integration ở `hall-onboarding-red-flow.log`; lượt HUD ẩn bị loại, đã tách UIDocument khỏi cha ẩn. OS Escape bị System Events1002 (`hall-onboarding-key-mobile`), không physical-key claim; movement fail trước telemetry và main mất focus đầu giữ nguyên log, không claim root đã hết. Next concrete player-visible: đưa điều hướng phiên lên góc phải theo storyboard bằng shared HUD layout, bỏ vị trí giữa đáy hiện tại cho cả main/phố; reuse base/asset, test safe bounds và dialogue overlay. Chưa final art/quest persistence/visual PASS, không mở production systems.

- Sau `ba41f51`, root shadow đã xác nhận bằng Player: `shadow-support-red.log` báo Soft/False; `M0ProjectGenerator` bật support, không tăng atlas/distance/cascade/bias/quality. `shadow-support-green-{mobile,desktop}.log` full route Soft/True; ảnh đã xem và răng cưa lớn giảm; main `shadow-support-main.log` exit0/capture/manifest, ảnh world-hub đã xem; quick qua. Tổng file Player343600085->343639988byte, không claim riêng shader delta hoặc GPU benchmark. Next concrete: đối chiếu tọa độ/session/content gate để nối preview phố vào luồng chọn nhân vật bằng opt-in development, reuse UI/session hiện có; không thay default main, protocol/schema hoặc vị trí lưu để ép integration. Chưa visual PASS/art final.

- Batch kiến trúc+paving: bảy nhà dùng module chung, một mesh trim cửa/lưới dùng lại; chân đá/nẹp mái, tường trung tính và trim than tối theo demo. Paving256/ô3x2m giữ collider/camera. `facade-red.log` count0 -> `facade-green-{desktop,tablet}.log` full route/mesh reuse qua; final `street-architecture-final-mobile.log` đúng960x540, kiểm tra từng mesh không null/rỗng sau review. Quick cuối `street-architecture-final-quick.log` qua; ảnh PC/tablet/mobile đã xem. Lượt `facade-green-mobile` đổi960->1920 giữa capture chỉ tính mixed viewport, không fixed-mobile. Gom checkpoint cùng paving, không ảnh runtime import mới. Next concrete: truy nguyên bóng răng cưa dưới mái/khung cửa qua cấu hình URP thật trong Player rồi sửa có so ảnh, không tăng shadow map/bias mò. Chưa final art/main integration/visual PASS.

- Sân nối phố Z15..27 đã đi thật được theo demo `lgo-street-forecourt-draft-v1.jpg`; guidance sau đá dẫn tới sân rồi ẩn khi đến, không bật lại lúc quay về. Runtime red đường chặn/thiếu hướng dẫn/cây chìm được giữ ở `forecourt-red.log`, `forecourt-guidance-red.log`, `forecourt-tree-red.log`. Green flow ba profile `forecourt-verified-*`, kích thước PNG xác nhận; sửa cao độ cây qua `forecourt-grounded-mobile.log`, ảnh đã xem, quick cùng tên qua. Không main map/quest persistence/final art/visual PASS. Next concrete: làm mặt đường đá theo demo sân nối phố, thay vạch ngang blockout bằng paving có tỷ lệ thực và tái sử dụng cho cả phố/sân; không đổi collision/camera hoặc thêm ảnh nặng.

- Evidence caveat: owner xác nhận chuyển monitor trong lượt PC cũ; không dùng `forecourt-green-desktop` (ảnh cuối960x540) hoặc `forecourt-desktop-confirm` (thiếu forecourt/route marker) làm fixed-PC pass. Stone facing từng fail20°/0.79m sau capture (`forecourt-measured-tablet.log`); fixture mới thoát frame capture rồi cộng delta Update đủ0.3s, không nới ngưỡng5°/0.5m hay sửa tốc độ game. `forecourt-verified-*` và `forecourt-grounded-mobile` đều angle0/~1.08m trở lên; không claim đã chứng minh mọi nguyên nhân timing/focus. Bản vista đối xứng cũ bị bỏ sau review (`street-vista-mobile`), không coi là cải thiện được nhận.

- Sau `dcda6a3`: giả thuyết thiếu mapping ngón bị bác bỏ (30 bone có đủ; clip gốc có curl), đã bỏ probe không cần thiết, không sửa importer. Thêm quay model về Người Giữ Cổng khi thoại: runtime red62.5572° -> green0°, giữ position/input-lock/cancel-reopen/đi tiếp, full route exit0 và quick qua; ảnh dialogue đã xem. Evidence `dialogue-facing-{red,green}.log`, `dialogue-facing-quick.log`. Hai source preview/world chưa commit. Next: phản hồi hướng nhìn/tư thế khi chạm Đá Luyện theo storyboard, dùng cơ chế presentation hiện có, không thêm UI hoặc skill giả.

- Cập nhật: `arrival-wrist-wrap-unlocked.log` Player exit0/192frames, ảnh chạy đã xem; blocker loginwindow đã hết. Nhóm rig/đai/băng tay sẵn checkpoint, quick/import/pose/build giữ evidence phía dưới. Owner yêu cầu tiếp tục làm bình thường, chỉ chủ động video bàn giao hoặc đổi sandbox khi owner yêu cầu; screenshot/runtime vẫn dùng cho kiểm chứng cần thiết. Next player-visible: đối chiếu bàn tay/động tác nghỉ với turnaround, kiểm tra mapping ngón trước chỉnh thêm art; không mở main integration khi candidate chưa đạt.

- Băng tay mới lấy mặt cẳng tay/cắt plane và giữ weights; red `wrist-wrap-red.log` có32 mặt phải hướng vào trong, ảnh rear/front mới không còn mảng da xuyên. 21634tris/1172364byte, không texture mới. Blender, `arrival-wrist-wrap-import.log`, build và `arrival-outfit-fit-checkpoint-quick.log` qua. Video mới CHƯA CÓ: Player19412 CPU~0.1%, foreground loginwindow; zero frames, đã TERM đúng PID/wrapper exit241, log `arrival-wrist-wrap-video.log`. Runtime capture bị chặn môi trường, không source compile fail. Chưa commit nhóm rig/đai/băng tay. Tiếp việc asset/source không cần foreground; khi Mac mở lại capture/review băng tay trước checkpoint, không lấy video đai cũ làm evidence model mới.

- Đai áo tiếp sau rig fix: fit từng hàng đơn lẻ bị lõm ở khe tunic/coat, không nhập bản lỗi. Dùng convex support envelope cho dải quấn đã cải thiện front; Unity import/pose/build/quick qua, Player192frames exit0; ảnh idle/chạy đã xem và video8s ở `build/visual-evidence/arrival-sash-envelope/sash-envelope.mp4`. Model21342tris/1158188byte, giảm so với checkpoint; chưa final art. Recipe/model/prefab vẫn gom chưa commit. Next cụ thể: băng tay hở mặt sau khi gập khuỷu trong ảnh rear; sửa mesh/skinning theo turnaround, không đổi camera hoặc thêm texture để che.

- Sau `f727735`, worktree có sửa rig chưa commit: scale xương nối trực tiếp làm endpoint bị nhân .86 nhiều lần (cổ tay lệch~8.5cm), `rig-width-red.log` exit1. Recipe dùng snapshot tọa độ gốc và assertion; build Blender/Unity pose/quick qua. FBX giảm608byte, prefab remap material theo thứ tự export mới; không thêm texture/tam giác. Video8s `build/visual-evidence/arrival-rig-width/rig-width-fixed.mp4` đã xuất, ảnh idle/chạy đã xem. Next: biến dạng vạt/đai theo turnaround, giữ batch để gom; chưa final art hoặc main integration. Đây là trạng thái mới hơn checkpoint sạch bên dưới.

- Checkpoint motion: quick `arrival-motion-checkpoint-quick.log` qua; đã xem arrival PC/tablet/mobile. Main lần đầu fail fresh input sau menu (`arrival-motion-main-regression`); lần chẩn đoán hoàn tất flow/manifest (`arrival-motion-main-diagnostic.log`, wrapper exit0, Player được harness kết thúc -15 sau capture). Trace focused=True/dt~.0333, không thay assertion; chưa xác định nguyên nhân lượt fail trước. Giữ telemetry, không gọi đó là lỗi đã sửa. Next player-visible: kiểm tra biến dạng vạt/tỷ lệ tay và dáng chạy theo turnaround trước đưa model vào main; không mở hệ thống mới hoặc tiếp tục polish wording. Dùng handoff này trong phiên mới, không đọc lại toàn bộ lịch sử.

- Mới nhất: candidate có idle/walk/jog trong preview opt-in; prefab ở Resources, góp payload build. Lỗi tay giơ cao có idle-pose red/green; để Unity dựng T-pose thay vì copy transform FBX đã sửa retarget. Route `arrival-canonical-{mobile,desktop,tablet}.log` exit0, ảnh mobile đã xem. Owner yêu cầu video: hai MP4 8s/960x540 tại `build/visual-evidence/motion-comparison/`; bản trước tái hiện cấu hình lỗi, bản sau cấu hình chuẩn, cùng input. Source/asset đã phục hồi, `arrival-video-restored-canonical.log` exit0. Next: review ảnh PC/tablet, quick/main regression trước checkpoint; chưa final art/visual PASS. Các mục sau là lịch sử.

- Unity candidate đã import ở `Assets/Game/Art/OnboardingCandidate/`, ngoài Resources và chưa được scene tham chiếu. `arrival-avatar-red.log` fail Humanoid thật; bản cuối `arrival-avatar-import-compensated.log` exit 0: Avatar hợp lệ, 1 renderer/6 submesh, baked world height ~1.86m, prefab URP/texture512+128. Hai phép đo trước sai không gian local và BakeMesh scale; sửa fixture bằng `BakeMesh(..., true)` + TransformPoint, không scale lại art/nới khoảng. Quick `arrival-import-quick.log` qua. Recipe source ở `tools/art/build_arrival_outfit_candidate.py`, license/provenance đi cùng asset. Budget WARN29file (asset/meta), chưa commit vì đang gom cả motion/preview. Next: clip idle/walk có nguồn rõ và thử candidate trong blockout opt-in; chưa animation/Player screenshot/final art hoặc thay main.

- Asset bước mới: `build/asset-staging/quaternius-base/arrival-outfit-candidate.fbx` đã export/rest pose và đọc lại bằng Blender: 1159100 byte, 1 mesh skinned, 65 bones, 6 materials, max 4 influences, max weight error 4.47e-8 (`fbx-roundtrip.log` exit 0). Bản đầu 28 mesh/weights lệch được giữ trong log trước; sửa snapshot group IDs khi xóa weights, không bỏ assertion. Tóc có các lọn thay khối ponytail đơn. Render camera đúng source FOV55 cho chiều cao 24.27% tại 960x540, ảnh đã xem; phép đo FOV60 trước đó không phải camera source. Next: kiểm tra import Avatar/material/bounds trong Unity rồi motion trong preview opt-in; chưa clip, chưa Unity evidence, chưa final art. Không tiếp polish ảnh portrait vô hạn hoặc đưa vào main khi chưa kiểm tra.

- Candidate mới nhất: cổ V đã cắt thật, nẹp raycast lên áo và có lớp áo trong; ảnh front đã xem không còn dải X nổi/hở ngực sâu. `outfit-study.log` exit 0, weights chuẩn hóa và normals vạt/đai hướng ngoài qua; không suy ra toàn model hết lỗi backface. Giày kín trong stride, đai còn cứng, tóc vẫn khối study và chất liệu chưa theo demo. Next: silhouette tóc/trang phục và xem model ở kích thước camera game trước quyết định export; chưa animation clip/Unity import. Artifact ở `build/asset-staging/quaternius-base/`; runtime/source game không đổi, chưa claim player-visible runtime progress.

- Model nhập môn: artifact ở `build/asset-staging/quaternius-base/`, chưa import Unity. Áo đã liên tục qua vai bằng plane cut giữ deform weights; bỏ các ống tay rời. Vạt 132 vertex chuyển weights theo body, tối đa 4 ảnh hưởng và chuẩn hóa: `outfit-weights-red.log` exit 1 -> `outfit-study.log` exit 0. Render front/rear/stride đã tạo, front và stride đã xem; chưa animation walk thực. Candidate vẫn lệch demo ở cổ/đai cứng/tóc/giày và chưa kiểm tra normals một mặt cho Unity. Next: hoàn thiện mesh trang phục và kiểm tra backface/biến dạng trước export; không đưa candidate lỗi vào game hoặc đổi demo. Runtime giữ `c2f1fbe`, không rerun gate main vì không đổi source.

- Checkpoint camera/pulse: full route mới `held-clock-{desktop,tablet,mobile}.log` đều exit 0, giữ nguyên input/deadline/assertion; đã xem desktop arrival và tablet camera-exit. Quick `camera-pulse-checkpoint-quick.log` qua. Camera cold-start 3 red -> 3 green bằng Always; main pulse reuse ba profile `stone-focus-final-runtime.log`, ảnh PC/mobile đã xem. Lượt held travel 2.4m cũ chưa rõ nguyên nhân, nay có telemetry để phân biệt khi tái diễn. Next: model/rig theo `lgo-arrival-outfit-turnaround-draft-v1.jpg`, xác minh nguồn/license và mẫu nhập môn trước import; không nhập pack lệch mỹ thuật chỉ để bỏ capsule. Chưa visual PASS hoặc benchmark CPU. Các mục dưới là diễn biến trước checkpoint này.

- Camera arrival đã tái hiện ở 2 FPS: `arrival-cold-red-{0,1,2}.log` đều exit 1, frame 5 rear còn score 0.8; standby RoundRobin giữ điểm cũ. Cho ba shot `StandbyUpdate.Always`, không đổi FOV/offset/boost: `arrival-cold-green-{0,1,2}.log` đều exit 0, ảnh green-0 đã xem đúng trục phố. Quick `arrival-cold-checkpoint-quick.log` qua. Full route chưa pass: `arrival-current-scores-desktop.log` thiếu quãng đường giữ input (2.4m), chưa đủ dữ liệu quy nguyên nhân; lượt `arrival-current-focus-desktop.log` không qua checkpoint đầu, mất foreground sang Chrome, process 48213 sau đó không còn. Next: phân biệt frame-time/focus với input reset trong fixture; không nới gate/claim full camera pass. Nếu desktop đang được dùng, tránh vòng giành focus và tiếp nguồn model/rig phù hợp theo design. Source camera + pulse vẫn chưa commit.

- Pulse Đá Luyện main đã thay cube cyan đặc bằng ground glow procedural chung; giữ điều kiện hiện, vàng khi tìm đá/linh khí khi channel, không texture mới. `stone-focus-final-runtime.log` hoàn tất ba profile, quick và pose validator qua; đã xem PC/mobile gần đá, ảnh so sánh ở `build/visual-evidence/stone-focus-comparison/`. Fixture đầu Find bỏ qua inactive nên fail trước frame bật pulse; sửa phép tìm cả inactive, không nới điều kiện sprite/không collider. Camera arrival chưa fix: `arrival-shot-red-player.log` thực tế exit 0, rear 1.08352077 > side 1.07968557; không tái hiện lỗi ảnh trước. Next: probe chọn shot lúc khởi tạo/focus và số frame warmup để tái hiện góc bên, không chỉnh camera theo giả thuyết khoảng cách chưa chứng minh. Source 4 file chưa commit để gom batch; không visual PASS.

- Guidance đã dùng `RuntimeWorldGuidanceView` chung main/blockout; hiện mục tiêu gặp -> đá -> hoàn tất theo storyboard, ẩn trong thoại, giới hạn safe viewport. Cỡ chữ guidance mobile tăng trong typography chung sau ảnh quá nhỏ. Ba `shared-guidance-final-*.log`, main `shared-guidance-main-baseline.log`, quick và density validator qua; ảnh đã xem. Next player-visible: truy nguyên góc bên không đúng composition ở `build/visual-evidence/onboarding-blockout/guidance-final-mobile/arrival.png`, sau đó mảng cyan chữ nhật trong `build/visual-evidence/profiles/mobile/near-training-stone-prompt.png`. Chưa visual PASS/gesture scroll/model; khảo sát rig ghi trong design, chưa import pack lệch mẫu.

- Đá blockout đã có mesh 80 tam giác + inset, nhìn cạnh không còn thành lát ảnh; collider/flow giữ nguyên. Demo `lgo-training-stone-volume-draft-v1.jpg` 114704 byte ngoài runtime. Ba `stone-volume-final-*.log` và quick cùng prefix qua; đã xem mobile bên đá/PC mép ngõ/tablet hoàn tất. Camera bốn mép ngõ qua, không chỉnh thêm camera. Next: tìm model/rig có nguồn/license rõ theo turnaround nhập môn để thay capsule bằng nhân vật có chuyển động; chưa NPC 3D/production art/visual PASS, không mở main scene hoặc systems ngoài roadmap. Detail trong design nhập môn.

- Camera/feedback blockout: Cinemachine chọn góc tránh nhà, giới hạn camera trong phố; giữ input không tự bẻ hướng khi đổi shot. Ba `blockout-camera-final-*.log` qua; main `cinemachine-main-flow-baseline.log` kết thúc capture ba profile, quick `camera-checkpoint-quick.log` PASS. Đã xem desktop ngõ/tablet ra phố; demo trang phục góc sau 154008 byte ở reference-ui, chưa model/runtime art. Next player-visible: kiểm tra đi sát hai mép ngõ/vòng góc nhà, sửa camera nếu che người trước khi tích hợp main. Chưa universal camera coverage/visual PASS; NPC/đá còn ảnh phẳng. Chi tiết và provenance trong design nhập môn; không ZIP/push.

- Feedback đá trong blockout đã có vòng focus theo điều kiện, pulse 1.2s và nhãn hoàn tất; reuse procedural glow/WorldLabelPresenter, không ảnh mới. Red thật `blockout-stone-red-player.log`; bản cuối ba profile `blockout-stone-label-*.log` và quick cùng prefix qua, ảnh mobile pulse/tablet settled đã xem. Bản đầu gán text trực tiếp làm bóng chữ cũ còn sót, đã sửa qua base Set và assertion đồng bộ. Source feedback chưa commit, gom với presentation tiếp; checkpoint NPC trước là `c04e6ca`, không push. Next player-visible: design nhân vật nhìn từ sau/góc camera phố theo reference Kiếm trước khi thay capsule; không lấy sprite mặt trước giả third-person, không tự đổi class/canon hoặc main scene. Chưa visual PASS, model 3D hoặc quest persistence.

- NPC trong blockout đã dùng `NpcDialogueSession` và `RuntimeNpcDialogueView` chung với flow chính; Gặp/Luyện kiểm tra tầm, đóng không hoàn tất, mở lại từ đầu, đọc hết mới mở đá. EditMode 3/3 (`npc-session-green.xml`), main ba profile (`npc-shared-view-baseline.log`), quick (`npc-shared-flow-quick.log`) qua. Blockout `blockout-npc-font-{desktop,tablet,mobile}.log` exit 0; ảnh `build/visual-evidence/onboarding-blockout/npc-font-*/` đã xem desktop/mobile thoại và tablet hoàn tất. Modal có assertion căn giữa/không tràn; chưa gesture thiết bị thật, quest persistence hoặc visual PASS. Tiếp: feedback focus/hoàn tất tại Đá Luyện trong blockout theo storyboard, reuse presentation có sẵn; không chỉ đổi nhãn nút, không thêm reward/quest hoặc nhân đôi modal.

- Blockout nhập môn đã đi thật qua CharacterController tới NPC rồi bên đá trong ba profile; chín PNG tại `build/visual-evidence/onboarding-blockout/lit-{desktop,tablet,mobile}/`, ảnh đã xem. Log `blockout-lit-*.log`; flow chính `shared-lit-surface-baseline.log` và quick `onboarding-blockout-quick.log` qua. Preview chỉ development flag, không account/save/quest; design ghi cách mở và giới hạn. Đã sửa root shader bị strip bằng material reference URP Lit chung, lượt build development tăng 42888 byte, không texture mới. Task tiếp: reuse state/UI hội thoại nhập môn trong cảnh mới để thử gặp/đóng/mở lại rồi tới đá; không tạo flow NPC riêng, không đổi main scene hoặc tọa độ hồ sơ. Chưa visual PASS; proxy geometry/camera vẫn cần review theo demo.

- Demo tiếp theo đã có: `docs/reference-ui/lgo-linh-mon-arrival-composition-draft-v1.jpg` (1440x810, 404279 byte, đã xem bản nén). Mặt bằng/điểm đứng/camera và trạng thái ở `docs/design/LINH-THANH-ONBOARDING-DESIGN.md`; DRAFT chưa duyệt, không runtime. Source checkpoint `4e95dab` đã local, không push. Task cụ thể tiếp: blockout kiểm chứng cảnh nhập môn tách khỏi scene/hồ sơ đang dùng, đi thật NPC -> điểm bên đá và review che khuất; chưa thay camera main, tọa độ lưu hoặc mở city systems. Demo còn sai tỷ lệ nhân vật và trạng thái nút Gặp ngoài tầm, đã ghi rõ; không lấy pixel ảnh làm thông số.

- World grounding/nhãn 2026-09-07: actor và đạo cụ đứng qua helper chung, không còn dùng orientation decal; phần nhãn không ghi đè vị trí NPC. Nhãn vật thể/prompt xếp theo projected bounds, bỏ offset riêng PC/tablet/mobile. Red thật: NPC bottom=-0.432 (`standing-actor-owner-red-player.log`), nhãn đá gap=-70px (`world-label-bounds-settled-red-player.log`), cổng còn nằm phẳng (`world-prop-grounding-red-player.log`). `build/dev-loop/world-standing-props-final.log` hoàn tất ba profile; quick cùng prefix pass; đã xem mobile world, desktop gần NPC, tablet gần đá. Không thêm texture, không thay collider/luật tương tác; chưa visual PASS hoặc 3D city. Task tiếp: cụ thể hóa demo sân Linh Môn nối phố từ storyboard thành mặt bằng/camera/điểm đứng tương tác; giải quyết việc người chơi che đá và đạo cụ rời rạc bằng staging có design, không thêm offset hoặc NPC menu giả.

- Checkpoint tổng hợp nhập môn/combat/input và ba slot: đã review diff chính, server shared/API chạy lại bằng JDK25/Maven Homebrew (`playable-checkpoint-server-homebrew.log`, BUILD SUCCESS); wrapper repo thiếu .toolchains, không phải source blocker. Package hygiene/diff check pass, runtime gần nhất tái dùng `application-focus-input-reset.log` vì source không đổi sau capture. Change budget WARN 50 file/1002 dòng trước ledger, không claim budget pass. Chỉ commit local, không ZIP/push. Task player-visible tiếp: đối chiếu world với storyboard nhập môn để chọn một lát cảnh/đường đi rõ, không tiếp tục chuỗi wording/callback tests.

- Focus app: world dùng chung ResetMovementInput cho overlay và OnApplicationFocus; UI reset pad/vector ngay callback, không đợi Update nền. `build/dev-loop/application-focus-input-reset.log` hoàn tất ba profile, marker loss/gain callback simulated pass; quick `application-focus-input-quick.log` exit 0. Chưa Alt-Tab/background thực hoặc gesture thiết bị thật. Task tiếp: review/gom coherent checkpoint của input/combat/UI đang tích lũy, sau đó chọn world-readability slice theo storyboard thay vì kéo dài chuỗi callback tests. Chưa visual PASS.

- Overlay resume đã qua ba profile (`build/dev-loop/world-overlay-input-resume-final.log`): xóa vector cũ, chặn frame đóng, input mới di chuyển lại; không đổi luật combat. Fixture gọi riêng world Update bằng reflection vì SendMessage broadcast cả UI và làm sai input, giữ log `world-input-resume-broadcast-fixture-failure.log`; evidence cũ chỉ dựa SendMessage không đủ chứng minh chặn movement. Pointer pad smoke vẫn chạy. Chưa phím vật lý/gesture thiết bị thật. Task tiếp: kiểm tra mất focus ứng dụng rồi quay lại có giữ input cũ không, dùng cùng owner input, không thêm lớp chặn riêng; chưa visual PASS.

- Input focus root fix: world tách đọc input khỏi presentation, không đọc movement/QE/F/Space khi overlay giữ focus hoặc app mất focus; mở overlay xóa TouchMovement. Callback Chém chặn Menu/thoại/sảnh. `build/dev-loop/world-overlay-input-focus.log` hoàn tất ba profile, marker Menu movement-injected/combat-callback pass; ảnh mobile menu đã xem; quick `world-overlay-input-quick.log` exit 0. Chưa test phím vật lý giữ xuyên overlay. Task tiếp: kiểm tra đóng overlay khôi phục điều khiển, không giữ input cũ; ưu tiên hành vi input trước checkpoint. Chưa visual PASS/server combat.

- Combat status toàn chiều rộng: gỡ nút legacy và icon cooldown khỏi controller/presentation, bỏ parser text cooldown cũ; HP/tầm/feedback dùng label chung. `build/dev-loop/combat-hud-status-width-current.log` hoàn tất ba profile; ảnh mobile ngoài tầm đã xem, bảng thấp/dễ đọc hơn và bounds pad pass. Hai lượt gate trước dừng do marker legacy, đã cập nhật hai validator hiện có theo Chém và nguồn cooldown thật; không bỏ non-claim/frozen gate. Chưa giảm build bytes/visual PASS. Task tiếp: gom checkpoint coherent cho luồng nhập môn/combat sau review diff và focused validation; sau đó chuyển sang design/input camera/world-readability có tác động rõ, không thêm vòng polish wording nhỏ.

- Combat HUD đã bỏ heading lặp và action thi triển trùng khỏi cây UI, giữ Chém ở cluster chung. `build/dev-loop/combat-hud-single-heading-action.log` hoàn tất ba profile; assertion đo khe hở bảng/pad sau layout pass; ảnh mobile ngoài tầm đã xem, không chạm pad. Quick gate `combat-hud-compact-quick.log` exit 0. Control probe legacy vẫn tồn tại ngoài panel, chưa cleanup dependency. Task tiếp: gỡ dependency presentation/probe của nút combat không còn hiển thị, giữ coverage qua Chém thật; không thêm UI hoặc giảm assertion để hợp thức hóa cleanup. Chưa visual PASS/scene final.

- Ngoài tầm -> vào tầm -> đánh -> hồi tự nhiên -> đánh lần hai đã qua ba profile (`build/dev-loop/combat-range-rejection-approach.log`); quick gate `combat-feedback-batch-quick.log` exit 0. Ảnh mobile combat-out-of-range đã xem: Chém/Lại gần, feedback rõ và HP 120/120 không đổi khi từ chối. Fixture đặt vị trí bằng harness, không claim đi bộ toàn tuyến. Lỗi visible tiếp: bảng combat dài chạm vùng movement pad, còn nút Tấn công thử trùng Chém và tiêu đề Bia luyện lặp. Sửa qua shared combat/HUD presentation theo grouping demo, không dịch pad hoặc tạo modal riêng. Chưa visual PASS/server combat.

- Range/readiness: nút Chém dùng bool range từ world, ngoài tầm/hết hồi hiện Lại gần; bảng trong tầm nhưng cooldown hiện đang hồi, không còn sẵn sàng sai. `build/dev-loop/combat-range-readiness-feedback.log` hoàn tất ba profile, ảnh mobile world-hub/target-dummy đã xem. World-hub đang trong tầm nên chưa có ảnh chứng minh nhãn ngoài tầm. Task tiếp: kiểm tra ngoài tầm -> lại gần -> thi triển bằng nút, giữ luật từ chối combat và so HP không đổi khi bị từ chối; không gọi screenshot trong tầm là coverage ngoài tầm. Chưa visual PASS/server combat.

- Cooldown tự hồi khi đứng yên và lần Chém thứ hai qua nút thật đã qua ba profile (`build/dev-loop/combat-cooldown-natural-recovery.log`, marker `LGO_WIND_SLASH_RECOVERY_PASS` trong từng player log). Không force-ready hoặc di chuyển; nút hiển thị tối thiểu 0.1s khi còn khóa, không đổi luật combat. Ảnh mobile target-dummy đã xem; chưa ảnh chụp đúng cuối 0.1s, visual PASS hoặc server combat. Task tiếp: thống nhất feedback ngoài tầm trên nút/HUD để người chơi biết vì sao Chém không trúng; dùng range state thật, không thêm skill hay art mới.

- Chém hiện đếm giây ngay trên nút và khóa trong cooldown; dùng thời gian combat state, shared presentation refresh theo phần mười giây kể cả đứng yên. `build/dev-loop/combat-cooldown-shared-refresh.log` hoàn tất ba profile; ảnh mobile target-dummy đã xem: nút và bảng cùng 5.8s. Lượt `wind-slash-button-cooldown.log` lộ bảng cũ đứng ở 6.0s đã được sửa. Diff check pass; chưa visual PASS/server combat. Tiếp theo kiểm chứng hết cooldown tự bật nút khi đứng yên và thi triển lần hai qua nút thật, không force-ready thay cho thời gian thực.

- Đã bỏ nhánh combat trùng từ nút ngữ cảnh: Gặp/Luyện -> Đã xong (disabled), Chém vẫn thi triển skill thật của prototype. `build/dev-loop/context-action-no-duplicate-combat.log` hoàn tất ba profile; ảnh mobile hoàn tất đã xem, assertion callback ngữ cảnh không phát combat pass. Quick gate: `build/dev-loop/guided-interaction-batch-quick.log`, exit 0. Task tiếp: đối chiếu combat loadout/design Kiếm hiện có và cooldown feedback trên chính nút Chém để người chơi biết khi nào dùng lại được; không thêm đánh thường giả hoặc skill mới ngoài roadmap. Chưa visual PASS/server combat.

- Menu save feedback reset: `build/dev-loop/session-save-feedback-reset.log` hoàn tất ba profile; assertions kiểm tra nhãn và tooltip sau lỗi/mở lại và thành công/re-entry, ảnh mobile menu đã xem. Bắt đầu lưu cũng bỏ tooltip lỗi cũ. Chưa hover-tooltip capture hoặc visual PASS. Task player-visible tiếp: đối chiếu hai nút Đánh/Chém sau nhập môn vì cùng gọi combat prototype, tránh mô tả nhầm đánh thường thành skill; kiểm tra design/loadout hiện có trước đổi control.

- Kiểm tra cuộn/mở lại đã hoàn tất ba profile (`build/dev-loop/dialogue-scroll-reopen-final.log`, marker trong từng player log): nội dung thực vượt viewport, scrollbar thay offset, footer trong shell, nút đóng/mở lại trả 1/3 và đầu trang. Lượt đầu tablet có fixture chưa đủ dài; giữ `dialogue-scroll-tablet-fixture-short.log`. Đây là verification cho sửa đọc thoại trước đó, không tính tính năng mới; chưa gesture thiết bị thật hay visual PASS. Tiếp theo chuyển sang phản hồi phiên: kiểm tra và xóa tooltip lưu thành công/thất bại cũ khi mở Menu mới, tránh trạng thái nhãn và tooltip mâu thuẫn; không tiếp tục mở rộng harness thoại nhỏ lẻ.

- `guided-buttons-reading-position.log` hoàn tất ba profile: Gặp/Tiếp tục/Hoàn tất/Luyện đi qua NavigationSubmitEvent trên nút thật; save/back/re-entry vẫn nằm trong harness. Dialogue reset scroll khi mở/đổi câu, không reset cùng câu. Đã xem mobile dialogue/training-complete. Assertion offset là gán bằng code, chưa kiểm tra gesture cuộn dài. Task tiếp: kiểm tra cuộn dài thực sau layout, giữ vị trí khi refresh cùng câu và reset khi mở lại; sửa base đọc nếu phát hiện lỗi, không claim visual PASS.

- Nút ngữ cảnh nhập môn giữ Gặp/Luyện theo tiến trình, khóa ngoài tầm, handler không chuyển sang combat trước hoàn tất. `build/dev-loop/guided-action-purpose.log` hoàn tất ba profile; đã xem mobile ngoài/gần đá, nút mờ/sáng đúng trạng thái; assertion kiểm tra ngoài tầm không gây damage/hoàn tất và vào tầm bật lại. Nút Chém riêng giữ nguyên. Tiếp theo kiểm tra toàn vòng bằng nút ngữ cảnh thật, gồm hoàn tất và trở lại sảnh, thay vì chỉ gọi trực tiếp world method; không claim scene final/visual PASS.

- Approach/action guidance: `build/dev-loop/onboarding-approach-action-final.log` hoàn tất ba profile, exit 0. Đã xem mobile `training-stone-approach.png` và `near-training-stone-prompt.png`: ngoài tầm hiện đường đá, gần đích hiện Chọn Luyện khớp nút. PC thêm lựa chọn nút cạnh phím F. Lượt đầu lỗi fixture mở lại dialogue đã hoàn tất; giữ `onboarding-approach-mobile-fixture-failure.log`, không dùng lượt đó làm pass. Task tiếp: kiểm tra action ngoài phạm vi đang chuyển sang Đánh trong chặng nhập môn, đối chiếu trạng thái combat hiện có và design trước đổi hành vi. Chưa visual PASS hoặc scene city hoàn chỉnh.

- Chỉ dẫn nhập môn đã đổi từ mạch lam sang đường đá ở thoại, feedback và hai nhánh HUD. Ba validator M5 liên quan pass; runtime ba profile exit 0 (`build/dev-loop/onboarding-stone-route-guidance.log`). Đã xem PC/mobile gần Đá Luyện, chưa có screenshot riêng đoạn đang đi để chứng minh câu chỉ đường mới. Task tiếp: bổ sung trạng thái đang đi giữa NPC và đá vào evidence hiện có, đối chiếu đường đi/HUD theo storyboard; không thêm validator mới cho wording.

- Kịch bản NPC/Lộ/skill/trang bị/trang phục: xem `docs/design/LGO-GAME-SYSTEMS-NARRATIVE-DESIGN.md`, đã nối các chặng chơi và ranh giới trách nhiệm; chưa phải hệ thống đã triển khai. Task player-visible kế tiếp: đối chiếu tuyến Người Giữ Cổng -> Đá Luyện với storyboard, sửa chỉ dẫn còn nói mạch sáng lam dù đường đã lát đá. Save rejection/retry hoàn tất ba profile (`build/dev-loop/menu-save-rejection-retry.log`, exit 0), ảnh mobile menu sau re-entry đã xem; lỗi API có kiểm soát, chưa mất mạng/timeout hoặc visual PASS.

- Focus nhãn world: `modal-world-label-focus.log` hoàn tất ba profile, ảnh mobile menu/dialogue/training-complete đã xem. UI truyền focus sang world; nameplate/prompt ẩn sau modal/sảnh, trở lại theo điều kiện gameplay. Không đổi offset hoặc input. Tiếp theo kiểm tra lỗi lưu/retry tại Menu để feedback lỗi và nút không bị kẹt; chưa fault-injection mạng, visual PASS hoặc quest persistence.

- Save/back/re-entry: `menu-save-back-reentry.log` hoàn tất ba profile; evidence bấm Lưu/Về điện qua menu, vào lại bằng API load và so vị trí sai lệch <=0.01. Nhãn thành công được reset khi mở menu mới, không reset lúc request đang chạy; ảnh mobile menu đã xem. Quest vẫn local/reset khi Enter. Tiếp theo xử lý world nameplate/prompt còn nổi phía sau modal để menu/dialogue có focus rõ, qua cùng cơ chế visibility, không đổi vị trí từng label.

- Menu save feedback: `menu-save-visible-feedback.log` hoàn tất ba profile; evidence bấm nút thật, đợi request và kiểm tra enabled + nhãn Đã lưu vị trí. Ảnh mobile menu đã xem. Nút hiện trạng thái đang lưu/thành công/thử lại, không thêm panel; lỗi mạng chưa fault-injection. Tiếp theo kiểm tra save/back/re-entry và reset nhãn thành công khi thay phiên/nhân vật để tránh feedback cũ; chưa claim quest persistence hoặc visual PASS.

- Completion flow: `guidance-completion-menu-final.log` hoàn tất ba profile; assertion kiểm tra đã xong dialogue/stone và các lệnh lặp không đổi mục tiêu. Ảnh mobile hoàn tất đã xem: nhắc Mở Menu đúng nơi có lưu/về sảnh. Đã sửa cả nhánh hint rút gọn/toast; không còn câu khẳng định ghi nhận luyện tập lâu dài. Tiến trình vẫn local. Tiếp theo kiểm tra save/back/re-entry từ nút Menu thật và ghi rõ phần nào được persist, trước khi mở progression mới.

- Sửa lỗi flow: `CloseDialogue` chỉ đóng giữa chừng, không đánh dấu hoàn tất/mở Đá Luyện; `ContinueDialogue` tới cuối gọi nhánh hoàn tất riêng. `dialogue-cancel-keeps-objective.log` hoàn tất ba profile: bấm nút đóng thật, đá còn khóa, NPC mở lại từ1/3; ảnh mobile đã xem. Validator lightweight dialogue pass. Tiếp theo kiểm tra nhánh hoàn tất/repeated interaction và feedback không claim lưu quest khi chỉ có local state.

- Guidance group đã bỏ nền/viền lồng qua skin chung; `world-guidance-unframed.log` hoàn tất ba profile, ảnh PC/mobile đã xem, mục tiêu/hint vẫn rõ. Task kế tiếp: kiểm tra toàn flow NPC -> Đá Luyện và phản hồi hoàn tất với storyboard nhập môn, ưu tiên lỗi hành vi/mục tiêu thực nếu có; không tiếp tục chuỗi chỉnh viền nhỏ. Chưa scene final hoặc visual PASS.

- Header HUD dùng compact mode của `NewSectionHeaderBlock`, bỏ ornament, canh trái và wrap; Character Hall mặc định không đổi. `world-hud-compact-header.log` hoàn tất ba profile, ảnh PC/mobile đã xem; hai validator header/boundary pass sau cập nhật marker cũ. Tiếp theo bỏ khung lồng khung guidance qua base, giữ nội dung mục tiêu/hint; chưa visual PASS hoặc đủ design HUD cuối.

- HUD mới nhất: `world-hud-single-actions.log` hoàn tất ba profile; ảnh PC world/menu đã xem. Ẩn dòng chỉ hướng lặp và footer lưu/về sảnh qua layout chung; hai action vẫn có trong Menu phiên với callback cũ. HUD thấp hơn, còn mục tiêu/gợi ý. Tiếp theo gom khối khu vực/tên nhân vật theo grouping reference để bớt chiếm sân, không thêm card lồng nhau; giữ input/interaction hiện có. Chưa visual PASS.

- Mạch lát đã chuyển từ mask nhị phân sang coverage theo khoảng cách texel; `courtyard-mortar-coverage.log` hoàn tất ba profile, ảnh PC/mobile đã xem: khe liên tục hơn, không tăng texture256. Tiếp theo ưu tiên HUD nhiệm vụ PC đang che nhiều sân và lặp chỉ dẫn; đối chiếu grouping HUD reference gốc, thu gọn qua base chung nhưng giữ mục tiêu/action thật. Không tiếp tục đánh bóng ground nhỏ lẻ khi flow/HUD còn vấn đề lớn.

- Tuyến đá procedural mới: `courtyard-stone-route-final.log` hoàn tất ba profile, ảnh PC/mobile đã xem. Giữ texture256, đường rộng khoảng0.9 world unit, nối gate/NPC/đá; nhìn rõ tuyến hơn vệt glow. Lượt `courtyard-stone-route.log` không dùng làm provenance cuối vì sửa công thức mask giữa build; bản final build lại đầy đủ. Còn mạch lát răng cưa và mép đường chưa hòa nền, chưa scene final. Tiếp theo xử lý sampling/mortar bằng công thức ổn định theo texel trước thêm chi tiết hoặc ảnh lớn.

- Đường ground đã nhận tọa độ NPC/Đá Luyện từ controller và dùng chung `GroundUv` với mesh; không còn UV đầu mút vẽ riêng. `ground-landmark-paths.log` hoàn tất ba profile, ảnh mobile đã xem: tuyến nối gate/trung tâm/NPC/đá hiện đúng vùng hơn nhưng còn mờ, chưa bằng đường đá trong storyboard. Tiếp theo thiết kế và triển khai bề mặt đường đá đọc rõ theo tuyến này, không thêm ảnh nặng; giữ phân biệt prototype và city thật.

- Ground 2026-09-07: sửa UV theo world x/z, footprint sân 18x18 trên collision plane 80x80; không tăng texture 256x256. `build/dev-loop/ground-world-scale.log` hoàn tất ba profile; ảnh PC/mobile World Hub đã xem, vòng sân/ô nền đọc rõ hơn quanh người chơi. Chưa khớp storyboard city: path procedural còn điểm UV hardcoded, task kế tiếp là dẫn đường từ tọa độ thật Linh Môn/NPC/Đá Luyện để tránh lệch đầu mút; không đổi collider hoặc camera để che lỗi.

- Portrait candidate đã vào runtime: 128x128 JPEG 8156 byte, vẽ mới theo NPC V3B world, không crop. `build/dev-loop/gatekeeper-small-portrait.log` hoàn tất ba profile, ảnh PC/mobile đã xem; ô thoại nhìn rõ mặt thay ảnh toàn thân thu nhỏ. Tiếp theo ưu tiên đường đi/mục tiêu nhập môn theo storyboard, không thêm menu/hệ thống mới: đối chiếu ground/camera/mốc NPC và Đá Luyện để người chơi hiểu hướng đi. Chưa claim Linh Thành playable, art final hoặc visual PASS.

- Cập nhật mới nhất: `dialogue-shared-gold-reading.log` hoàn tất ba profile; đã xem PC thoại ngắn/mobile dài. Tên speaker không còn lặp trong body; scroller dùng DeepGlass/Gold của skin chung. Sizing Auto/Center, bỏ minHeight đã có evidence trước đó. Tiếp theo: thống nhất thiết kế NPC world/portrait trước tạo asset chân dung riêng đúng kích thước; không đổi riêng mặt trong modal thành nhân vật khác với world. Chưa khớp demo hoặc visual PASS; cảnh sân luyện vẫn prototype kỹ thuật. Ghi chú root cause bên dưới là lịch sử đã xử lý.

- Dialogue 2026-09-07: bỏ title chung qua shell hiện có, giữ một speaker header/body/footer; runtime assertion kiểm tra ba vùng. `build/dev-loop/dialogue-single-speaker.log` hoàn tất ba profile; đã xem PC ngắn/mobile dài. Chưa khớp demo: `RuntimeWorldHudResponsiveLayout.cs` gán height bằng maxHeight nên thoại ngắn quá cao. Task kế tiếp: sizing theo nội dung có trần tại base, giữ scroll/căn giữa và kiểm chứng resize, không chỉnh offset. Demo: `docs/reference-ui/lgo-gatekeeper-dialogue-draft-v1.jpg`, chưa duyệt, không runtime asset.

- Ưu tiên owner 2026-09-07: thiết kế theo kịch bản gốc trước mọi hệ thống mới. Xem `docs/design/LGO-GAME-SYSTEMS-NARRATIVE-DESIGN.md` (đề xuất NPC/Lộ/skill/trang bị/trang phục và thứ tự slice) cùng `docs/design/LINH-THANH-ONBOARDING-DESIGN.md`. Task cụ thể kế tiếp: đối chiếu reference NPC, thiết kế demo Người Giữ Cổng và dialogue/đường đi nhập môn; chưa tự mở shop/economy/phái mới hoặc coi storyboard là runtime. M5 sân luyện vẫn là prototype kỹ thuật, không phải chương truyện hoàn chỉnh. Năm Lộ dài hạn khác phạm vi Kiếm/Võ Founder Alpha. Các ghi chú cũ bên dưới là lịch sử, không lấn ưu tiên này.

- Tiếp demo 2026-09-07: TextField có placeholder thật `Danh xưng`, mở Tạo thêm xóa tên cũ; runtime red/green trong `name-placeholder-red.log` / `name-placeholder-green.log`. Hoa văn góc vector dùng chung Character Hall/menu/section shell, không thêm ảnh; base preview dùng DenseGlass để đọc dễ hơn. PC/tablet khung có 23 checkpoint và ảnh đã xem (`shared-shell-ornaments.log`); mobile lần đầu mất focus giữa pointer test, không coi pass. Retry `ornaments-mobile-focus-retry.log` và lượt nền đậm `shared-shell-reading-contrast.log` đủ 23 checkpoint, ảnh menu/hội thoại dài đã xem; quick cùng tên pass. Chưa khớp demo toàn bộ; tiếp theo slot/gender phải gắn với dữ liệu nhân vật thật, kiểm tra tương thích persistence trước khi triển khai. Tránh mở ảnh trong lúc Player đang capture.

- Batch hiện tại 2026-09-07: font serif/body 197464 byte source, avatar trống 128x128 (14254 byte), nền Linh Thành riêng 1672x941 JPEG (431721 byte). Login/preview/HUD/control chuyển qua base neutral/gold; header/status và nhãn nút theo demo. Shared overlay căn giữa kích thước thực, Character Hall cùng root viewport như menu, max-height 85%; bounds red/green. `build/dev-loop/night-city-budgeted-theme.log`: 23 checkpoint/profile; ảnh PC Login/selected, tablet empty, mobile selected/menu đã xem; quick cùng tên và focused validators pass. Change budget WARN số file do font/asset/meta; không thêm validator mới. Pointer smoke từng intermittent, chưa rõ root cause. Chưa visual PASS hoặc giống demo hoàn chỉnh; tiếp theo selection ô trống, hoa văn khung dùng chung và giới tính có persistence, không UI giả.

- Tiến độ 2026-09-07: ba slot, silhouette giữa, form/footer thực sự thuộc cột phải; bỏ nhánh dock cũ và hai nút dùng cùng tier/chiều rộng qua base. Skin chung bắt đầu chuyển nền trung tính/viền vàng theo demo mới owner duyệt. `build/dev-loop/three-slots-neutral-gold.log` hoàn tất 23 checkpoint/profile, ảnh empty ba profile, selected PC/mobile và menu tablet đã xem; bounds resize pass. Quick gate `three-slots-new-skin-quick.log` và validator panel-density pass. Chưa giống demo hoàn chỉnh: thiếu thumbnail/selection ô trống, giới tính, font serif, nền đúng hướng; mobile selected còn khoảng trống lớn vì ẩn header. Tiếp tục các phần này và chuyển các skin cũ còn cyan, không claim visual PASS.

- Account cap: API từ chối nhân vật thứ tư, test reload/độc lập account pass (`build/dev-loop/three-character-limit-green.log`, 15 tests). UI khóa tạo khi đủ 3, harness tạo tối đa 3; không xóa hồ sơ legacy. Cần xử lý minh bạch account legacy >3 và assertion trạng thái ô trống. Nút Chém đã gọi combat local thật đang có, evidence đi qua nút và kiểm tra cooldown; hướng dẫn ẩn khi combat để không che controls. Không claim server-authoritative combat hoặc production auth/DB.

- HUD theo reference gốc `docs/reference-art/linh-gioi-world-event-ui.png`: movement và skill dùng chung base trên PC/tablet/mobile; Menu tách khỏi combat, thêm nút preview Hộ Linh đang có. Red bắt khóa desktop và Menu chiếm slot combat; lượt cuối 23 checkpoint/profile, pointer mô phỏng di chuyển/thả/menu pass, ảnh world-hub ba profile đã xem (`build/dev-loop/navigation-skill-separation.log`); quick gate pass (`build/dev-loop/shared-controls-quick.log`). Roster 8 hồ sơ cũng qua scroll/resize/selection. Tiếp theo: thu gọn panel nhiệm vụ tablet đang che NPC, tách trạng thái nhân vật khỏi nhiệm vụ và tạo icon riêng đúng kích thước theo reference gốc. Chưa claim visual PASS, thiết bị cảm ứng thật, hoặc đủ kịch bản game. Năm class theo concept/GDD, không thu thiết kế tổng thể về Kiếm tu; auth/DB production chưa mở.

- Roster selection: tạo hồ sơ thứ hai không còn tự quay về hồ sơ đầu; refresh giữ ID đang chọn. Base list button có selected/unselected, chỉ áp dụng cho hàng roster; bỏ hướng dẫn lặp. Runtime red xác nhận lỗi cũ, lượt cuối 21 checkpoint/profile hoàn tất, tạo/refresh/đổi hàng qua event đều pass, ảnh desktop/tablet/mobile đã xem (`build/dev-loop/roster-selection-final.log`); quick gate pass. Tiếp theo: roster nhiều hồ sơ, scroll trong vùng giới hạn và giữ hàng vừa chọn nhìn thấy. Chưa claim visual PASS hoặc mọi kích thước.

- Danh xưng sai hiện lỗi ngay trong form, sửa đúng xóa lỗi, không gửi request sai. Đã sửa body hàng ngang chiếm hết width làm footer tràn; bounds test có lượt red rồi green. Account tests 15/15, quick gate pass; đã xem form lỗi đầu tiên/Tạo thêm ở ba profile (`build/dev-loop/name-feedback-final.log`, `build/visual-evidence/profiles/*/character-create-invalid.png`). Một ảnh PC trước bị input đổi trước capture đã loại khỏi bằng chứng lỗi; smoke focus từng fail, lượt sau pass, chưa kết luận nguyên nhân. Tiếp theo: tạo nhân vật thứ hai phải giữ selection mới và roster chỉ nhấn mạnh hàng đang chọn. Không claim visual PASS/giống demo 100%.

- Batch hủy form + mobile guided actions: Tạo thêm có Tạo tu sĩ/Hủy, Escape dùng chung handler và không hủy lúc nút bị khóa; hủy giữ selection. Nút mobile đổi Đánh/Gặp/Luyện theo world state, dùng interaction hiện có; smoke đi qua NPC và Đá Luyện. Evidence cuối: `build/visual-evidence/mobile-guided-actions/player-unity.log`, ảnh near-gatekeeper/near-training-stone đã xem; quick gate pass. Một lượt smoke movement trước đó fail, hai lượt sau pass; chưa kết luận nguyên nhân transient. Tiếp theo: phản hồi danh xưng không hợp lệ trong form theo rule server hiện có (3-16 ASCII chữ/số/gạch dưới), không đổi contract.

- Batch form/resize/tên dài đã validate: Player resize 1382x972, 984x922, 692x486 và khôi phục đều qua bounds form/input/nút; tên tối đa 16 ký tự rộng xuống dòng ở PC/tablet, không cắt chữ. Đã xem screenshot probe và long-name cả ba profile; quick gate pass (`build/dev-loop/character-create-name-final.log`, `build/dev-loop/character-create-name-quick.log`). Tiếp theo: thao tác hủy/quay lại form Tạo thêm qua base hiện có. Chưa kiểm tra toàn bộ kích thước, thiết bị cảm ứng thật hoặc claim visual PASS.

- Character Hall central stage: nhân vật V3B đã ra cột giữa qua factory/layout chung; roster và thông tin ở hai bên. Base list button bỏ min-width cứng, dock PC giới hạn theo parent, shell PC/tablet có max-height theo phần viewport còn lại. Đã xem selected cả ba profile và empty PC; quick gate pass (`build/dev-loop/character-central-stage.log`, `build/dev-loop/character-central-stage-quick.log`). Tiếp theo: resize trong cùng phiên, tên dài và form Tạo thêm; chưa claim visual PASS hoặc giống demo 100%.

- Batch 2026-09-06: pad mobile đã nối movement cục bộ qua base `RuntimeTouchMovementPad`. 5/5 UI tests có graphics pass; quick gate pass. Player smoke thực hiện pointer mô phỏng: đi 1,441 đơn vị, thả/menu đều dừng (`build/visual-evidence/touch-movement/player-unity.log`). Đã xem world-hub sau smoke. Chưa test thiết bị cảm ứng thật hoặc claim visual PASS. Tiếp theo: bố cục hero Character Hall theo demo, dùng base chung và kiểm tra cả resize giữa các profile.

- Current phase: runtime UI/visual quality hardening and execution workflow cleanup.
- Active task: `LGO-RUNTIME-QUALITY-NEXT-COMPACT-BATCH-v1.0`.
- Current reason: responsive viewport work now has a canonical UI `RuntimeViewportMetrics` owner, north-star and mobile/tablet UI reference sheets anchor layout decisions, and `RuntimeUiOverflowGuard` centralizes bounded action rows, scroll regions, compact scroll chrome, overlay placement, modal body/footer regions, and responsive action columns. World dialogue now routes body/footer through shared modal primitives with body-only scrolling, viewport-centered overlay placement, equal top/bottom insets, styled compact scrollbar chrome, a long-text evidence checkpoint, and a shared responsive speaker header with the V3B Gate Keeper portrait. Session Menu now uses a title-only demo-aligned pause shell, centered modal placement, gold-only frame, hidden action-only scroll chrome, and one bounded vertical action column across desktop/tablet/mobile. Character Hall has begun moving from screen-specific composition to shared demo-aligned contracts, and mobile Login now uses the shared centered/top overlay base for its CTA cluster instead of a left/top absolute branch; screenshots were reviewed without claiming `VISUAL_RUNTIME_PASS`.
- Current batch scope: continue with player-visible layout/quality fixes, controller hotspot extraction, or dependency-driven V2 fallback retirement planning without opening new systems.
- Fast validation: `git --no-pager diff --check`; `bash -n tools/lgo_codex_git_checkpoint.sh tools/lgo_codex_autopilot.sh tools/lgo_continue_dev_loop.sh`; `python3.12 tools/report_lgo_change_budget.py`; `python3.12 tools/validate_package_hygiene.py`; `LGO_DEV_LOOP_GATE_PROFILE=quick LGO_DEV_LOOP_CONTEXT_MODE=quick ./tools/lgo_continue_dev_loop.sh`.
- Runtime validation: run `LGO_DEV_LOOP_VISUAL_RUNTIME=force ./tools/lgo_continue_dev_loop.sh` or `./tools/lgo_visual_runtime_review.sh` only when the next code change affects visible runtime UI. Do not claim `VISUAL_RUNTIME_PASS` from capture alone.
- Next implementation task after this fix: keep following the target case matrix in `docs/design/RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0.md`; next likely player-visible gap is auditing remaining absolute/floating Character Hall panels and Login mobile/tablet composition against the shared overlay/button/container bases, using bounded containers and profile evidence instead of one-off raw `Screen.width/height` tuning.
- Historical marker registry stays in this file for validator compatibility until a dedicated registry migration is implemented and validated.

Current focus update: Session Menu demo shell is ready under `LGO_SESSION_MENU_DEMO_SHELL_BASE_READY`; runtime now uses a title-only pause modal shell instead of the generic section shell, centers it via the shared overlay base, keeps actions in a bounded vertical column, hides action-only scroll chrome, and desktop/tablet/mobile screenshots were reviewed without claiming final visual pass.

Current focus update: Character Hall empty list boundary is ready under `LGO_CHARACTER_HALL_EMPTY_LIST_BOUNDARY_READY`; empty-state duplicate step rows were removed, the character list now uses a bounded scroll body with profile max height/width, and desktop/tablet/mobile screenshots confirm the empty lobby no longer clips through the bottom edge.

Current focus update: Character Hall selected dock clearance is ready under `LGO_CHARACTER_HALL_SELECTED_DOCK_CLEARANCE_READY`; desktop selected actions now use a bottom-safe dock without pushing panel flow, non-mobile selected duplicate status rows collapse to avoid CTA overlap, and tablet/mobile selected screenshots remain readable without final visual pass claim.

## Current focus

Post-login visual runtime hardening plus device-profile asset governance. Login has been upgraded to V3B-aligned runtime presentation, source-level post-login readability polish is implemented, and mobile/tablet/PC runtime asset profile budgets are now documented and validated. The standalone visual evidence harness now captures all seven screenshots, can continue in background, and auto-finishes the Unity Player after manifest completion so the operator should not need to click or close the player by hand. Character lobby usability, in-world HUD presentation, world hub scene readability, and desktop/tablet/mobile responsive evidence are now verified with fresh runtime screenshots. Login-to-character copy has been cleaned of player-facing dev wording, status chips now read correctly in runtime screenshots, the session menu/settings shell is responsive without tablet/mobile clipping, in-world interaction affordance now has stateful target labels, runtime asset size inventory is documented/validated, V3B runtime candidates now carry platform-specific Unity import profiles for Standalone/Android/iPhone, the in-world HUD is more compact/touch-oriented across desktop/tablet/mobile and now groups world guidance/action content into V3B-styled shell cards, the world hub camera now uses viewport-aware orthographic framing so mobile/tablet actors read larger than the fixed desktop view, and refreshed profile screenshots confirm mobile actor scale is improved without harmful cropping. The world hub ground now uses a lightweight procedural cultivation-platform texture instead of a debug-like grid, the login first screen now uses a V3B composition with a centered text logo/CTA cluster plus right-side Gate Keeper on desktop/tablet and a compact logo/CTA layout on mobile, the Character Hall now uses a V3B cultivator portrait with a mobile-specific two-zone lobby layout, lighter panel density, a game-facing create form with framed `Danh xưng` input and `Tạo tu sĩ` CTA, and refreshed desktop/tablet/mobile runtime screenshots, build-size budget reporting now separates Unity runtime payload from repository/reference/tooling weight, the visual evidence loop now writes PNG heuristics for checkpoint presence/dimensions/byte size/pixel variation/duplicate-frame detection, world-hub labels now show by guided state/proximity instead of cluttering the whole scene, the login CTA stack now uses a lighter dark-glass panel to reduce cyan/gold glare while retaining the V3B logo/button language, and shared runtime UI styling now lives in `RuntimeUiSkin` so repeated glass panel, framed row, compact button, and login backing rules can be reused instead of copy-written per screen. The World Hub now has lightweight procedural grounding shadows under the player, interactables, target dummy, warning slime, Spirit Gate, and key props. The visual review script now has a build-once/reuse-player profile wrapper for desktop/tablet/mobile screenshot refreshes. Source-only gates now preserve runtime evidence directories under `LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_READY`. Character Hall post-login composition now routes list, preview, profile hero, portrait, and responsive row setup through `RuntimeUiFactory` under `LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY`, and fresh runtime screenshots confirm the helper extraction did not break post-login layout under `LGO_POST_LOGIN_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY`. Character Hall V3B polish and evidence refresh are tracked under `LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_READY` and `LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_EVIDENCE_REFRESH_READY`. The unused controller-local Character Hall list-density wrapper has been removed under `LGO_RUNTIME_UI_FACTORY_CHARACTER_HALL_CLEANUP_FOLLOWUP_READY`. World HUD hidden pose/VFX/skin-source evidence labels now route through `RuntimeUiFactory.NewHiddenStatusLabel` under `LGO_WORLD_HUD_RUNTIME_UI_REUSE_AUDIT_READY`, fresh runtime evidence confirms World Hub/NPC Dialogue remain stable under `LGO_WORLD_HUD_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY`, and the World HUD root/group frames now use a calmer V3B/fantasy panel hierarchy under `LGO_WORLD_HUD_FANTASY_PANEL_HIERARCHY_POLISH_READY`.

Current focus update: refreshed screenshots confirm the corrected no-stretched-texture World HUD panel version under `LGO_WORLD_HUD_FANTASY_PANEL_EVIDENCE_REFRESH_READY`.

Current focus update: hidden status/note composition now routes through reusable factory helpers under `LGO_RUNTIME_UI_STATUS_COMPOSITION_CLEANUP_READY`.

Current focus update: focused runtime screenshots confirm hidden status composition remains stable under `LGO_RUNTIME_UI_STATUS_COMPOSITION_EVIDENCE_REFRESH_READY`.

Current focus update: local combat cooldown button now uses a lighter code-styled glass state under `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_LIGHTNESS_READY`.

Current focus update: target-dummy runtime evidence confirms the lighter cooldown button state under `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_READY`.

Current focus update: execution state now has a Quick Resume block under `LGO_RUNTIME_UI_STATE_DOC_COMPACTION_AUDIT_READY` so future continuous sessions can identify the active task without losing historical marker coverage.

Current focus update: append-only task history now has a compact rollup view at `docs/execution/TASK-LEDGER-ROLLUP.md` under `LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY` so future sessions can scan recent closures without reading the full ledger.

Current focus update: autopilot commit cadence now defaults to no auto-commit under `LGO_COMMIT_CADENCE_POLICY_HARDENING_READY`; coherent checkpoint commits remain opt-in with `LGO_AUTOPILOT_COMMIT=1`, and push remains opt-in with `LGO_AUTOPILOT_PUSH=1`.

Current focus update: visual debt triage selected Login NPC grounding plus CTA panel polish under `LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY`; no visual PASS is claimed from the triage alone.

Current focus update: login NPC grounding and CTA panel source polish is ready under `LGO_LOGIN_NPC_GROUNDING_CTA_PANEL_POLISH_READY`; runtime screenshot refresh is required before any visual claim.

Current focus update: refreshed desktop/tablet/mobile login screenshots confirm the source polish is visible; desktop and tablet are improved but still not production-final, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World Hub now has a more readable procedural cultivation stage, directional path glows, and focus glows under key interactables under `LGO_RUNTIME_UI_QUALITY_DEBT_FIRST_FIX_READY`; desktop/tablet/mobile screenshots were reviewed, the scene is improved but still placeholder-quality, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Character Hall no longer stretches the ornate V3B panel texture across the full shell under `LGO_CHARACTER_HALL_ACTION_DENSITY_FIRST_FIX_READY`; desktop/tablet/mobile screenshots are cleaner and more readable, still not production-final, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World procedural ground texture, actor shadows, focus glows, and path glows now live in `WorldProceduralVisuals` under `LGO_RUNTIME_UI_CONTROLLER_SIZE_REDUCTION_READY`; `PlayableWorldController` is smaller and runtime screenshot capture remains stable, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: Dev-loop/autopilot output now defaults to compact context and fast visual runtime server build logs under `LGO_WORKFLOW_FAST_GATE_NOISE_REDUCTION_READY`; visual capture also snapshots/restores Unity ProjectSettings so evidence runs do not pollute source diffs.

Current focus update: Login responsive layout now lives in `RuntimeLoginResponsiveLayout` under `LGO_RUNTIME_CODE_HOTSPOT_REDUCTION_READY`; `M4PlayableClientController` is smaller, historical validators follow the new ownership boundary, and runtime login capture remains stable with no visual PASS claim.

Current focus update: World-space label creation, text refresh, shadow, and active toggling now live in `WorldLabelPresenter` under `LGO_WORLD_CONTROLLER_INTERACTION_PRESENTATION_SPLIT_READY`; gameplay state ownership remains in `PlayableWorldController`, and world-hub runtime capture remains stable with no visual PASS claim.

Current focus update: `prepare_unity_local_assets.sh` now supports `LGO_UNITY_LOCAL_ASSETS_QUIET=1`, and `lgo_visual_runtime_review.sh` uses it under `LGO_PREPARE_UNITY_ASSETS_QUIET_PROFILE_READY`; visual review logs are shorter while protocol generation/build/capture still fail honestly.

Current focus update: visual runtime review now re-requests player focus with bounded progress logging and the Unity evidence runner configures background capture earlier under `LGO_VISUAL_RUNTIME_CAPTURE_FOCUS_ROBUSTNESS_READY`; latest desktop evidence captured all 10 checkpoints without a manual click in the runner log, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World Hub oversized procedural ground rings are toned down, lightweight runtime mist support is available, and the stale M6 readiness docs-only validator no longer blocks current non-frozen implementation work under `LGO_WORLD_HUB_VISUAL_DEPTH_WEIGHT_READY`; fresh desktop screenshots were reviewed as cleaner but still not final-production world art.

Current focus update: changeset noise audit removed obsolete M6 readiness allowlist code and split visual evidence hooks into `M4PlayableClientController.Evidence.cs` under `LGO_CHANGESET_NOISE_HOTSPOT_AUDIT_READY`; main UI controller is smaller, source-only passes, and visual evidence still captures all checkpoints.

Current focus update: Character Hall selected CTA hierarchy now prioritizes `Vào sân luyện` before `Tạo thêm` on desktop/tablet/mobile under `LGO_CHARACTER_HALL_SELECTED_CTA_PRIORITY_READY`; fresh desktop screenshot is cleaner, no visual PASS is claimed.

Current focus update: continuous workflow, task selection, quick/full gate strategy, and autopilot prompt now include change-budget rules under `LGO_CONTINUOUS_WORKFLOW_CHANGE_BUDGET_READY`; future batches should avoid new one-off docs/validators/markers unless they protect a real gate or handoff need.

Current focus update: World Hub ground texture now uses a smaller, softer procedural background and the desktop camera frame is closer under `LGO_WORLD_HUB_SOFT_BACKGROUND_CAMERA_READY`; fresh runtime screenshot is cleaner and actor/prop scale reads better, no visual PASS is claimed.

Current focus update: Character Hall selected state now collapses the create form under `LGO_CHARACTER_HALL_SELECTED_CREATE_COLLAPSE_READY`; fresh runtime screenshot shows a cleaner CTA band with `Vào sân luyện` first and no always-visible create input, no visual PASS is claimed.

Current focus update: local generated handoff archives were cleaned from `build/chatgpt-handoff`, reducing `build/` from 507MB to 169MB; `tools/lgo_local_artifact_cleanup.sh` now provides a dry-run/apply path under `LGO_LOCAL_ARTIFACT_CLEANUP_READY` while preserving source, evidence screenshots, and dev-loop logs.

Current focus update: enter-world evidence now captures a distinct Linh Môn transition state and world-hub resets to steady objective state under `LGO_ENTER_WORLD_EVIDENCE_DISTINCT_CHECKPOINT_READY`; fresh runtime screenshots were reviewed, duplicate-frame evidence is gone, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: session menu now owns focus on every profile, centers by viewport ratio, hides HUD/header while open, and dims the world behind it under `LGO_SESSION_MENU_CENTERED_FOCUS_POLISH_READY`; fresh runtime screenshot is cleaner, no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: selected mobile Character Hall now labels the action state as `Sẵn sàng` and lowers the CTA panel slightly under `LGO_MOBILE_CHARACTER_HALL_READY_COPY_POLISH_READY`; mobile screenshot was reviewed, no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: repeated session menu placement, responsive padding, background, and focus scrim rules now live in `RuntimeSessionMenuLayout` under `LGO_SESSION_MENU_LAYOUT_HELPER_READY`; source-only passes and runtime capture remained stable.

Current focus update: autopilot checkpoint staging now uses an allowlist and skips generated/cache/build artifacts under `LGO_AUTOPILOT_SAFE_CHECKPOINT_STAGING_READY`; commit/push remains opt-in and frozen surfaces remain blocked.

Current focus update: responsive root-cause fix is ready under `LGO_RUNTIME_VIEWPORT_METRICS_ROOT_CAUSE_READY`; old mistakes were mixing screenshot pixels, UI Toolkit panel units, safe-area pixels, and serialized PanelSettings enum values. Runtime now records screen pixels, panel viewport, safe panel rect, layout/input class, and active PanelSettings per visual checkpoint. Latest profile evidence: `build/visual-evidence/profiles/desktop`, `build/visual-evidence/profiles/tablet`, `build/visual-evidence/profiles/mobile`; screenshots reviewed, no `VISUAL_RUNTIME_PASS` claim.

Current focus update: focus-mode World HUD visibility is ready under `LGO_WORLD_HUD_FOCUS_MODE_STEADY_FOOTPRINT_READY`; steady desktop/tablet world-hub screenshots now hide the skill preview and combat panels while active Shadow Bind preview and mobile target-dummy combat-focus evidence still show the relevant panels. No gameplay/combat semantics changed and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: mobile World HUD child-panel constraints are ready under `LGO_MOBILE_WORLD_HUD_CHILD_PANEL_CONSTRAINT_READY`; dialogue, skill preview, combat, and guidance children now collapse their minimum width to the HUD parent on mobile instead of inheriting `NewPreviewPanel`'s wider min-width. Latest mobile `target-dummy-state.png` and `npc-dialogue.png` show the panels no longer protrude beyond the HUD edge; no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: mobile interaction prompt readability is ready under `LGO_MOBILE_INTERACTION_PROMPT_READABILITY_READY`; `F Gặp` and `F Luyện` prompts are slightly larger, raised away from object labels, and text shadows now sync to the prompt TextMesh size. Latest mobile near-gatekeeper and near-training-stone screenshots were reviewed; no gameplay logic changed and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: combat cooldown active sprite/texture now prefer V3B then lightweight `CombatPlaceholders` instead of V2 `CooldownFull` fallbacks under `LGO_RUNTIME_COMBAT_COOLDOWN_V2_DEPENDENCY_CLEANUP_READY`; target-dummy screenshot was reviewed as readable, V2 registry references are down to 24 and fallback-only properties to 8, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Character Hall portrait fallback no longer references V2 `IconAccountTexture` under `LGO_CHARACTER_HALL_PORTRAIT_V2_FALLBACK_CLEANUP_READY`; fresh Character Hall screenshots still show the V3B cultivator portrait, V2 registry references are down to 23 and fallback-only properties to 7, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: target dummy idle/selected/hit art now prefer V3B then lightweight `CombatPlaceholders` instead of V2 dummy fallbacks under `LGO_RUNTIME_TARGET_DUMMY_V2_FALLBACK_CLEANUP_READY`; target-dummy screenshot still renders the training dummy and combat panel clearly, V2 registry references are down to 20 and fallback-only properties to 4, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Shadow Slime runtime sprite and marker fallback now use V3B/null checks instead of V2 `ShadowSlimeAlt` under `LGO_RUNTIME_SHADOW_SLIME_V2_FALLBACK_CLEANUP_READY`; world-hub and target-dummy screenshots still show the shadow slime clearly, V2 registry references are down to 19 and fallback-only properties to 3, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: combat target marker and warning telegraph now use lightweight `CombatPlaceholders` directly instead of V2 fallback sprites under `LGO_RUNTIME_COMBAT_MARKER_V2_FALLBACK_CLEANUP_READY`; target-dummy screenshot remains readable, V2 registry references are down to 17 and fallback-only properties to 1, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: V2 dependency snapshot now uses exact property-name matching instead of prefix counting under `LGO_RUNTIME_ASSET_V2_DEPENDENCY_EXACT_SCAN_READY`; report now shows 16 remaining V2 references, all V3B-covered by exact-name coverage, with 0 fallback-only properties.

Current focus update: combat ready cooldown, wind slash, and impact spark now prefer V3B then lightweight `CombatPlaceholders` instead of V2 fallbacks under `LGO_RUNTIME_COMBAT_VFX_V2_FALLBACK_CLEANUP_READY`; source validators pass and report shows 12 remaining V2 references with 0 fallback-only properties.

Current focus update: Login background and Gate Keeper NPC texture now use V3B directly instead of V2 texture fallbacks under `LGO_LOGIN_V2_FALLBACK_CLEANUP_READY`; fresh login screenshot still renders background/logo/CTA/NPC clearly, report shows 10 remaining V2 references with 0 fallback-only properties, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World actors and set dressing now use V3B registry assets directly instead of V2 fallbacks under `LGO_RUNTIME_WORLD_V2_FALLBACK_CLEANUP_READY`; refreshed world-hub and target-dummy screenshots still render key actors/props clearly, and exact V2 dependency scan now reports 0 V2 registry references with 0 fallback-only properties.

Current focus update: login CTA and server row sizing/tint were reduced under `LGO_LOGIN_CTA_FIT_FOR_PURPOSE_READY`; fresh desktop/tablet/mobile screenshots were reviewed as calmer and still readable, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: World Hub procedural floor colors, rings, and guide paths were tuned under `LGO_WORLD_GROUND_READABILITY_TUNING_READY`; fresh screenshot was reviewed as less flat but still not production-final world art.

Current focus update: next-task advisor fallback is ready under `LGO_NEXT_TASK_ADVISOR_ACTIVE_FALLBACK_READY`; if backlog has no safe standalone task but `NEXT-ACTION.md` has an active task, the advisor returns it instead of stopping.

Current focus update: compact state loading and Vietnamese visual-runtime owner notes are ready under `LGO_COMPACT_STATE_BRIEF_AND_VI_RUNTIME_NOTES_READY`; routine dev/autopilot loops should use `tools/lgo_state_brief.py` before opening long state files.

Current focus update: World Hub lightweight depth pass is ready under `LGO_WORLD_HUB_LIGHTWEIGHT_DEPTH_PASS_READY`; procedural ground now uses a smaller texture and extra runtime-generated mist/path depth without adding image payload, with desktop/tablet/mobile screenshots reviewed but no final visual PASS claim.

Current focus update: combat dummy state asset priority now prefers V3B selected/hit/recover sprites under `LGO_COMBAT_DUMMY_V3B_STATE_PRIORITY_READY`; target-dummy runtime screenshot was reviewed as more consistent with V3B, still placeholder-quality, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: compact state brief now stays under the routine 90-line budget under `LGO_STATE_BRIEF_TOKEN_BUDGET_READY`; routine resume should use this before opening long state/ledger files.

Current focus update: mobile Character Hall bottom-safe overlay handling is ready under `LGO_MOBILE_CHARACTER_HALL_BOTTOM_SAFE_OVERLAY_READY`; selected action overlays now route through `RuntimeUiOverflowGuard.ApplyViewportBottomSafeOverlaySurface`, panel height is bounded from a profile-derived top/bottom inset, and fresh mobile screenshots were reviewed without claiming `VISUAL_RUNTIME_PASS`.

Current focus update: login NPC composition stage placement is ready under `LGO_LOGIN_STAGE_OVERLAY_BASE_READY`; the stage now routes through `RuntimeUiOverflowGuard.ApplyViewportOverlaySurface` and refreshed desktop/tablet/mobile login screenshots show the logo, CTA, background, and NPC remain visible without claiming `VISUAL_RUNTIME_PASS`.

Current focus update: mobile Character Hall shell/reflow is ready under `LGO_CHARACTER_HALL_MOBILE_SHELL_REFLOW_READY`; `_mainShell` sizing now comes from `RuntimeUiLayoutProfile`, mobile non-world screens use safe viewport width instead of a fixed `720` cap, and selected-character state forces responsive reflow so the cultivator hero is visible in mobile evidence.

Current focus update: mobile Character Hall selected hero readability is ready under `LGO_CHARACTER_HALL_MOBILE_HERO_READABILITY_READY`; selected preview width, portrait size, and selected-name size use profile metrics so the V3B cultivator/profile read clearer on mobile without moving buttons into a one-off layout.

Current focus update: mobile Character Hall light shell is ready under `LGO_CHARACTER_HALL_MOBILE_LIGHT_SHELL_READY`; `RuntimeUiSkin.ApplyCharacterHallPanelFrame(panel, layout.IsMobile)` now owns the lighter mobile shell alpha so runtime art reads through the panel without screen-specific card/button styling.

Current focus update: World objective pulse readability is ready under `LGO_WORLD_OBJECTIVE_PULSE_READABILITY_READY`; the Gate Keeper objective pulse is larger/brighter in the guided first step, and mobile `world-hub`/near-object screenshots were reviewed without changing interaction semantics or claiming `VISUAL_RUNTIME_PASS`.

Current focus update: combat cooldown button countdown is ready under `LGO_COMBAT_COOLDOWN_BUTTON_COUNTDOWN_READY`; the target-dummy combat button now shows compact remaining cooldown text such as `Hồi 6.0s`, backed by existing local cooldown state and mobile runtime screenshot review.

Current focus update: first-time Character Hall create flow now stays visible in the desktop safe viewport under `LGO_CHARACTER_HALL_CREATE_VIEWPORT_FLOW_READY`; desktop uses a horizontal create row before the selection grid, tablet/mobile evidence remains readable, duplicate empty-state objective copy is hidden, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: steady World Hub HUD footprint is smaller on desktop under `LGO_WORLD_HUD_DESKTOP_FOOTPRINT_TUNE_READY`; desktop/tablet/mobile profile screenshots were reviewed as readable, gameplay/dialogue/combat semantics are unchanged, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: mobile Gate Keeper prompt spacing is cleaner under `LGO_MOBILE_GATEKEEPER_PROMPT_AIR_GAP_READY`; the near-NPC label/prompt now sits higher with more air above sprites, interaction logic is unchanged, desktop remains readable, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World dialogue speaker context is ready under `LGO_DIALOGUE_SPEAKER_CONTEXT_BASE_READY`; the dialogue modal now has a shared responsive speaker header with V3B Gate Keeper portrait, normal and long mobile dialogue screenshots keep footer actions visible and body scrolling contained, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Character Hall demo/base alignment is ready under `LGO_CHARACTER_HALL_DEMO_BASE_ALIGNMENT_READY`; mobile selected now uses a full-safe shell and hides repeated header/prose so roster/hero/actions match the target hierarchy more closely, desktop empty no longer renders a selected placeholder hero, and create form actions route through shared modal body/footer and responsive columns without overflow.

Current focus update: Login mobile centered control base is ready under `LGO_LOGIN_MOBILE_CENTERED_CONTROL_BASE_READY`; the mobile login logo/server/CTA cluster now uses `RuntimeUiOverflowGuard.ApplyViewportOverlaySurface` with a computed centered inset and larger readable clamps, and fresh mobile evidence no longer shows the CTA cluster tiny in the top-left.

Current focus update: active skill preview now hides the guidance card under `LGO_SKILL_PREVIEW_FOCUS_GUIDANCE_HIDE_READY`; desktop/tablet skill preview screenshots show the skill panel and buttons fitting without bottom clipping, target-dummy guidance/combat state is unchanged, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: active Gate Keeper dialogue now hides the World HUD footer under `LGO_DIALOGUE_FOCUS_FOOTER_HIDE_READY`; desktop/tablet/mobile dialogue screenshots no longer show clipped save/back footer controls, dialogue flow is unchanged, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World-space label/profile metrics are ready under `LGO_WORLD_VIEWPORT_LABEL_METRICS_READY`; camera sizing, narrow/mobile checks, prompt sizing, and landmark label typography now route through a single World viewport profile, while `WorldLabelPresenter.ApplyStyle` keeps TextMesh shadows synced. Fresh desktop/tablet/mobile screenshots were reviewed as more readable, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: mobile dialogue action row fit is ready under `LGO_MOBILE_DIALOGUE_ACTION_ROW_FIT_READY`; dialogue-visible mobile HUD width now uses a wider profile-owned clamp, and the dialogue action row/buttons use no-wrap/flex sizing so `Tiếp tục` and `Đóng` remain in one readable touch row. Fresh mobile/tablet screenshots were reviewed, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: active World HUD focus footer overflow is fixed under `LGO_WORLD_HUD_FOCUS_FOOTER_OVERFLOW_READY`; skill-preview/combat-focus HUD states now hide the save/back footer, leaving the active gameplay panel visible without bottom-edge clipping in desktop/tablet evidence. No gameplay semantics changed and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Character Hall lobby text-field inner skin is ready under `LGO_CHARACTER_HALL_LOBBY_TEXT_FIELD_INNER_SKIN_READY`; shared `RuntimeUiSkin.ApplyLobbyInputFrame` now styles the UI Toolkit TextField input child so the create-form name field stays dark/readable across desktop/tablet/mobile screenshots. No auth/character semantics changed and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Character Hall desktop/tablet hierarchy now uses wider metric-owned shell sizing and lighter glass under `LGO_CHARACTER_HALL_WIDER_GLASS_BALANCE_READY`; desktop/tablet/mobile screenshots were reviewed as cleaner and still not production-final, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: Character Hall responsive layout now lives in `RuntimeCharacterHallResponsiveLayout` under `LGO_CHARACTER_HALL_RESPONSIVE_LAYOUT_HELPER_READY`; the main playable UI controller is smaller while character flow and reviewed runtime screenshots remain stable.

Current focus update: World HUD responsive panel/top-status/dialogue layout now lives in `RuntimeWorldHudResponsiveLayout` under `LGO_WORLD_HUD_RESPONSIVE_LAYOUT_HELPER_READY`; source-only and runtime screenshot evidence remain stable, and quick dev loop now skips visual capture by default unless forced.

Current focus update: World Hub now has a slightly stronger procedural platform, back-ridge prop layer, and mist depth under `LGO_WORLD_HUB_LIGHTWEIGHT_BACK_RIDGE_DEPTH_READY`; screenshot evidence is improved but still below final reference quality, and local cleanup can now dry-run/remove rebuilt Unity player builds without touching source/evidence, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: mobile session menu actions now route responsive sizing through `RuntimeSessionMenuLayout.ApplyActions` under `LGO_MOBILE_SESSION_MENU_ACTION_GRID_READY`; profile screenshot shows a readable 2x2 action grid with no final visual PASS claim.

Current focus update: responsive UI sizing now uses resolved UI Toolkit root viewport and visual evidence records `uiViewportWidth/Height` under `LGO_RUNTIME_UI_PANEL_VIEWPORT_SIZING_READY`; mobile evidence shows `960x540` screenshots mapping to about `361x203` UI panel units, explaining prior scale/crop issues and preventing further raw-pixel tuning.

Current focus update: World Hub set-dressing and depth-lighting placement now live in `WorldHubSetDressing` under `LGO_WORLD_HUB_SET_DRESSING_HELPER_READY`; `PlayableWorldController` now only owns the gameplay hook and shared billboard creation, while fresh runtime screenshots confirm World Hub/NPC Dialogue/Session Menu/Login remain stable. No `VISUAL_RUNTIME_PASS` is claimed because World Hub art is still below final reference quality.

Current focus update: Character Hall create-form collapse/expand state and selected-action CTA hierarchy now live in `RuntimeCharacterHallResponsiveLayout` under `LGO_CHARACTER_HALL_STATE_ACTION_HELPER_READY`; `M4PlayableClientController` keeps only flow decisions and delegates reusable presentation state. Source-only gates pass; no visual PASS is claimed from this source-only ownership cleanup.

Current focus update: Local combat HUD cooldown/icon/button/accent presentation now lives in `RuntimeCombatHudPresentation` under `LGO_RUNTIME_COMBAT_HUD_PRESENTATION_HELPER_READY`; `M4PlayableClientController` keeps local combat flow calls while the helper owns text, texture, tooltip, compact status, and accent state. Source-only and refreshed target-dummy runtime evidence pass, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: World Hub procedural cultivation floor now has stronger lightweight stone seams, rings, guide paths, and platform mist under `LGO_WORLD_GROUND_LIGHTWEIGHT_DEPTH_CUES_READY`; refreshed desktop screenshot is less flat without adding image payload, still below final reference-quality world art, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: compact state brief now suppresses long `PROJECT-STATE.md` body output under `LGO_STATE_BRIEF_PROJECT_STATE_NOISE_REDUCTION_READY`; routine resume output is back under the 90-line target, reducing token/context spend without weakening gates.

Current focus update: Character Hall selected-state proportions now use a wider shell, narrower list, and larger cultivator preview under `LGO_CHARACTER_HALL_SELECTED_HERO_PROPORTION_READY`; refreshed desktop screenshots are less admin-like while still below final reference-quality UI, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: local artifact cleanup now reports `client/Unity/Library` size and adds explicit `--apply-unity-cache` under `LGO_LOCAL_UNITY_CACHE_CLEANUP_MODE_READY`; dry-run confirms the large workspace weight is local Unity cache/player build rather than runtime art source.

Current focus update: Character Hall first-time create state now centers a narrower form and uses `Khai mở tu sĩ` copy under `LGO_CHARACTER_HALL_CREATE_STATE_FORM_POLISH_READY`; refreshed desktop screenshot is cleaner, but the TextField chrome still needs a future reusable form-component pass.

Current focus update: workflow self-review is applied under `LGO_WORKFLOW_SELF_REVIEW_COMPACT_PROGRESS_READY`; state brief no longer prints truncation noise for the core state, commit cadence should group related small batches, and Character Hall first-time `Danh xưng` field alignment is centered while deeper TextField chrome remains a focused future form-component task.

Current focus update: Character Hall first-time input label chrome is cleaned under `LGO_CHARACTER_HALL_INPUT_LABEL_CHROME_CLEANUP_READY`; `Danh xưng` now uses game copy above the field and a label-free input, shared skin label styling stays API-compatible, runtime screenshot evidence was reviewed, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: lobby text field creation now routes through `RuntimeUiFactory.NewLobbyTextField` under `LGO_RUNTIME_UI_LOBBY_TEXT_FIELD_FACTORY_READY`; the playable controller no longer owns the create-form input skin/tooltip details, source gates pass, and visual evidence from the preceding UI change remains current.

Current focus update: Character Hall density-aware status label creation now lives in `RuntimeUiFactory.NewCharacterHallStatusLabel` under `LGO_CHARACTER_HALL_STATUS_LABEL_FACTORY_READY`; the playable controller owns less reusable UI construction while Character Hall validators and quick dev loop remain clean.

Current focus update: World Hub procedural ground contrast is tuned under `LGO_WORLD_GROUND_CONTRAST_READABILITY_TUNE_READY`; the 256x256 runtime-generated texture now gives tile seams, rings, and guide paths more readable contrast without adding image payload, refreshed screenshots were reviewed, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: runtime asset inventory now reports V2 fallback `Resources` payload separately under `LGO_RUNTIME_ASSET_V2_FALLBACK_PAYLOAD_INVENTORY_READY`; current V2 fallback weight is 2389.7 KB across 65 images, kept only while registry/code dependencies still need fallback coverage.

Current focus update: runtime asset inventory now reports referenced V2 registry dependencies under `LGO_RUNTIME_ASSET_V2_DEPENDENCY_SNAPSHOT_READY`; current source references 29 V2 registry properties, 13 of them fallback-only by exact V3B property-name coverage.

Current focus update: responsive UI now follows Unity UI Toolkit panel-space instead of raw pixel tuning under `LGO_RUNTIME_UI_PANEL_SCALE_MODEL_READY`; runtime loads a real `LGORuntimePanelSettings` resource, profile selection uses screen short/long-side bands while component sizing uses resolved root viewport units, visual evidence records both coordinate systems, mobile session menu evidence was reviewed, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: login first-screen fit is repaired under `LGO_LOGIN_PANEL_SPACE_FIRST_SCREEN_FIT_READY`; responsive layout cache now tracks resolved viewport width/height, auth panel height and mobile login metrics are panel-space aware, and refreshed desktop/tablet/mobile screenshots show logo, server row, and `Vào Thế Giới` CTA inside the viewport. No `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Character Hall first-time create form width balance is ready under `LGO_CHARACTER_HALL_CREATE_FORM_WIDTH_BALANCE_READY`; the form panel is narrower, input/CTA cluster is centered, validator ownership follows the current factory/helper split, and refreshed screenshots were reviewed without claiming final visual pass.

Current focus update: secondary button V2 dependency cleanup is ready under `LGO_RUNTIME_UI_SECONDARY_BUTTON_V2_DEPENDENCY_CLEANUP_READY`; `RuntimeUiFactory.NewSecondaryButton` now uses code-side V3B-style framing instead of V2 `ButtonSecondaryTexture`, and runtime screenshots remain readable.

Current focus update: primary button V2 dependency cleanup is ready under `LGO_RUNTIME_UI_PRIMARY_BUTTON_V2_DEPENDENCY_CLEANUP_READY`; primary login buttons use V3B gold texture when available and code-side fallback framing instead of V2 primary/disabled textures.

Current focus update: Character Hall first-time create form readability is polished under `LGO_CHARACTER_HALL_CREATE_HINT_RESPONSIVE_POLISH_READY`; desktop uses a muted cultivation hint instead of a second framed row, tablet/mobile keep the form compact, and refreshed screenshots were reviewed without claiming final visual pass.

Current focus update: World Hub guided-objective readability is improved under `LGO_WORLD_HUB_GUIDED_OBJECTIVE_LABEL_READY`; the current objective object is labeled directly in the scene while existing panel objectives remain unchanged, with refreshed desktop/tablet evidence and no final visual pass claim.

Current focus update: local combat hit feedback now includes a visible reward placeholder under `LGO_LOCAL_COMBAT_REWARD_PLACEHOLDER_FEEDBACK_READY`; `Tinh khí +1` appears during target hit evidence, improving action-result feel without adding economy, inventory, or server reward semantics.

Current focus update: mobile World Hub top status copy is compact under `LGO_MOBILE_TOP_STATUS_COMPACT_READY`; profile screenshots show `Bước 1/2`, `Bước 2/2`, and `Hoàn tất` in the upper-right chip while the left HUD keeps full guided objective text, with no final visual pass claim.

Current focus update: mobile skill preview top status is ready under `LGO_MOBILE_SKILL_PREVIEW_TOP_STATUS_READY`; `skill-shadow-bind-preview.png` now shows `Xem Trói Bóng` in the mobile top chip while the world telegraph remains visible, with no final visual pass claim.

Current focus update: mobile/tablet Character Hall ready top status is compact under `LGO_CHARACTER_HALL_MOBILE_TOP_STATUS_COMPACT_READY`; profile screenshots show `Sẵn sàng` in create/select header chips while desktop copy remains unchanged, with no final visual pass claim.

Current focus update: combat target stamina status is compact under `LGO_COMBAT_TARGET_STAMINA_STATUS_COMPACT_READY`; target-dummy evidence now shows `Sức bền: 108/120 mô phỏng.` inside the combat HUD, reducing repeated target-name copy without changing combat simulation or persistence.

Current focus update: combat range status is compact under `LGO_COMBAT_RANGE_STATUS_COMPACT_READY`; target-dummy evidence now shows `Tầm: 2.6m / sẵn sàng.` in the range row, improving mobile combat HUD scanability without changing range calculation or combat simulation.

Current focus update: mobile/tablet World Hub guidance copy is compact under `LGO_MOBILE_WORLD_GUIDANCE_COPY_COMPACT_READY`; the left panel now uses short action text (`Gặp Người Giữ Cổng.`, `Ổn định Đá Luyện.`) while world-space prompts keep the key hints, with no gameplay rule change or final visual pass claim.

Current focus update: guided objective pulse cues are active under `LGO_GUIDED_OBJECTIVE_PULSE_CUES_READY`; profile screenshots show a lightweight pulse on Người Giữ Cổng during step 1 and Đá Luyện during step 2, with no new image payload or final visual pass claim.

Current focus update: NPC dialogue evidence flow is reset under `LGO_NPC_DIALOGUE_EVIDENCE_FLOW_RESET_READY`; `npc-dialogue.png` now shows the Gate Keeper dialogue panel, speaker/progress, and action buttons after the runner resets from prior combat/skill checkpoints, with no final visual pass claim.

Current focus update: mobile dialogue action row is compact under `LGO_MOBILE_DIALOGUE_ACTION_ROW_COMPACT_READY`; `npc-dialogue.png` now shows `Tiếp tục` and `Đóng` on one row, reducing vertical panel pressure while keeping touchable controls, with no final visual pass claim.

Current focus update: runtime UI overlay/button baseline is ready under `LGO_RUNTIME_UI_OVERLAY_BASELINE_READY`; dialogue and session menu modal placement now route through shared viewport overlay placement, dialogue is no longer nested inside the World HUD, long dialogue keeps equal top/bottom viewport insets with body-only scroll and fixed footer actions, and session/dialogue buttons use shared semantic tiers. Fresh desktop/tablet/mobile screenshots were reviewed, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: Character Hall overlay placement baseline is ready under `LGO_CHARACTER_HALL_OVERLAY_PLACEMENT_BASE_READY`; mobile create form and selected action dock now route through shared overlay horizontal/vertical anchors with viewport-derived width/insets instead of one-off absolute top/right/bottom values. Fresh profile screenshots were reviewed for Character Hall, dialogue, and session menu stability, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: Character Hall action button tier baseline is ready under `LGO_CHARACTER_HALL_ACTION_BUTTON_TIER_BASE_READY`; selected/mobile enter-world and create actions now use `RuntimeUiButtonTier.Primary/Compact/Standard` instead of separate per-screen min-width/min-height/font constants. Fresh desktop/tablet/mobile Character Hall screenshots were reviewed, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: shared modal/action structure is ready under `LGO_RUNTIME_UI_MODAL_ACTION_BASE_READY`; dialogue body/footer now use `RuntimeUiFactory.NewModalBody/NewModalFooter` and `RuntimeUiOverflowGuard.ApplyModalBody/ApplyModalFooter`, while selected Character Hall actions use a lightweight floating action-bar frame instead of the full create-form card. Fresh profile screenshots were reviewed for Character Hall and long dialogue; Character Hall is cleaner but still below the north-star composition target, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Character Hall selected hero metrics are ready under `LGO_CHARACTER_HALL_SELECTED_HERO_PROFILE_METRICS_READY`; selected preview width, portrait size, and selected-name typography now come from `RuntimeUiLayoutProfile`, desktop/tablet portrait reads larger, and the redundant selected-card heading was removed after screenshot review showed it overlapping the portrait. Fresh profile screenshots were reviewed, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: mobile Character Hall selected hero is ready under `LGO_MOBILE_CHARACTER_HALL_SELECTED_HERO_READY`; selected-state mobile now keeps a compact roster and shows the V3B cultivator hero/profile beside it using profile-owned list/preview metrics instead of hiding the selected preview. Fresh mobile/tablet/desktop screenshots were reviewed; mobile is closer to the target sheet but still not final production UI, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: scroll body chrome baseline is ready under `LGO_SCROLL_BODY_CHROME_BASE_READY`; `RuntimeUiOverflowGuard.ApplyBoundedScroll` now skins runtime scrollbars and hides default arrow buttons so long dialogue no longer exposes platform-default white scroller controls. Fresh desktop/tablet/mobile long-dialogue screenshots were reviewed, with no `VISUAL_RUNTIME_PASS` claim.

Autopilot operating rule: when a task or phase is truly closed by its required gates, continue to the next roadmap-valid task/phase instead of stopping at the phase boundary. Stop only for a real blocker, unavailable runtime/tooling, required owner decision, or frozen contract/protocol/schema/ADR change.

## Next task

`LGO-RUNTIME-QUALITY-NEXT-COMPACT-BATCH-v1.0`

Continue with one compact runtime quality batch. Pick the next visible issue from latest evidence, prefer layout/scale/hierarchy fixes over new assets, keep file count proportional to player value, and run visual evidence only when the change affects runtime presentation. Latest closed marker: `LGO_MOBILE_INTERACTION_PROMPT_READABILITY_READY`.

## Current blocker

No active blocker for source work. Visual runtime capture is currently available in this environment and has completed multiple consecutive rounds without manual player close; latest focus-robustness run captured 10/10 checkpoints with progress telemetry.

Evidence:

- `tools/validate_lgo_device_profile_ui_budgets.py`
- `docs/tasks/LGO-MOBILE-TABLET-UI-PROFILE-HARDENING-v1.0.md`
- `docs/tasks/LGO-VISUAL-CAPTURE-TIMEOUT-HARDENING-v1.0.md`
- `docs/tasks/LGO-CHARACTER-LOBBY-VISUAL-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-PLAYABLE-PRESENTATION-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUB-SCENE-PRESENTATION-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-GROUND-VISUAL-QUALITY-PASS-v1.0.md`
- `docs/tasks/LGO-LOGIN-NPC-COMPOSITING-POLISH-v1.0.md`
- `docs/tasks/LGO-CHARACTER-HALL-V3B-COMPOSITION-POLISH-v1.0.md`
- `docs/tasks/LGO-BUILD-SIZE-BUDGET-AND-CLEANUP-PASS-v1.0.md`
- `docs/tasks/LGO-VISUAL-RUNTIME-REVIEW-HEURISTICS-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-HUB-PROP-LABEL-RESPONSIVE-PASS-v1.0.md`
- `docs/tasks/LGO-LOGIN-PANEL-VISUAL-BALANCE-PASS-v1.0.md`
- `docs/tasks/LGO-CHARACTER-HALL-PANEL-DENSITY-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-SCENE-DEPTH-LAYERING-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-RESPONSIVE-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-LOGIN-CTA-ORNAMENT-LIGHTWEIGHT-PASS-v1.0.md`
- `docs/tasks/LGO-CHARACTER-CREATE-FORM-PRESENTATION-PASS-v1.0.md`
- `docs/tasks/LGO-CHARACTER-HALL-RESPONSIVE-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-VISUAL-RUNTIME-FAST-PROFILE-REUSE-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-ACTION-SHELL-V3B-SKIN-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-ACTION-SHELL-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-MOBILE-CAMERA-FRAMING-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-MOBILE-CAMERA-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-LABEL-SAFE-AREA-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-LABEL-SAFE-AREA-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-TOP-STATUS-MOBILE-READABILITY-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-TOP-STATUS-MOBILE-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-ACTOR-HUD-OCCLUSION-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-ACTOR-HUD-OCCLUSION-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-DIALOGUE-PANEL-VIEWPORT-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-DIALOGUE-PANEL-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-MOBILE-HIERARCHY-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-MOBILE-HIERARCHY-EVIDENCE-REFRESH-v1.0.md`
- `build/visual-evidence/latest/player.log`
- `build/visual-evidence/latest/unity-build.log`
- `build/codex-autopilot/status.json`

Next allowed action: polish Character Hall V3B visual hierarchy while keeping generated captures and package artifacts out of source control. The visual evidence harness records explicit review checklist categories and machine-readable heuristics for every checkpoint under marker `LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY`; combat button mobile evidence is tracked under `LGO_COMBAT_BUTTON_MOBILE_RESPONSIVE_EVIDENCE_READY`; World HUD component boundary cleanup is tracked under `LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY`; runtime UI evidence-state helper is tracked under `LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY`; World HUD header block is tracked under `LGO_WORLD_HUD_HEADER_BLOCK_READY`; World HUD row/helper coverage is tracked under `LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY`; World HUD row/helper evidence is tracked under `LGO_WORLD_HUD_ROW_HELPER_EVIDENCE_REFRESH_READY`; runtime UI style ownership cleanup is tracked under `LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY`; runtime UI style ownership evidence is tracked under `LGO_RUNTIME_UI_STYLE_OWNERSHIP_EVIDENCE_REFRESH_READY`; runtime UI controller style constants cleanup is tracked under `LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_READY`; runtime UI controller style constants evidence is tracked under `LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_EVIDENCE_REFRESH_READY`; runtime UI responsive padding profile audit is tracked under `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY`; runtime UI responsive padding profile evidence is tracked under `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_EVIDENCE_REFRESH_READY`; runtime UI session-menu padding profile audit is tracked under `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY`; runtime UI session-menu padding evidence is tracked under `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_EVIDENCE_REFRESH_READY`; runtime UI factory padding helper coverage is tracked under `LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_READY`; runtime UI factory padding helper evidence is tracked under `LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_EVIDENCE_REFRESH_READY`; runtime UI controller padding profile candidates are tracked under `LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY`; post-login Character Hall UI reuse cleanup is tracked under `LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY`; world-hub label responsiveness is tracked under `LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY`; world hub visual staging is tracked under `LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_READY`; world hub interaction readability is tracked under `LGO_WORLD_HUB_INTERACTION_READABILITY_READY`; world hub interaction evidence refresh is tracked under `LGO_WORLD_HUB_INTERACTION_EVIDENCE_REFRESH_READY`; near-interaction capture coverage is tracked under `LGO_NEAR_INTERACTION_CHECKPOINT_CAPTURE_READY`; near-interaction evidence refresh is tracked under `LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_READY`; visual evidence upload packaging is tracked under `LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY`; runtime asset budget refresh is tracked under `LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY`; runtime asset watch queue/profile polish is tracked under `LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY`; visual debt triage is tracked under `LGO_WORLD_HUB_VISUAL_DEBT_TRIAGE_READY`; session-menu focus evidence is tracked under `LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_READY`; Character Hall mobile density is tracked under `LGO_CHARACTER_HALL_MOBILE_COPY_DENSITY_READY`; Character Hall mobile evidence is tracked under `LGO_CHARACTER_HALL_MOBILE_COPY_EVIDENCE_REFRESH_READY`; Character Hall mobile selected CTA hierarchy is tracked under `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_HIERARCHY_READY`; Character Hall selected CTA evidence is tracked under `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_EVIDENCE_REFRESH_READY`; Character Hall style adoption is tracked under `LGO_CHARACTER_HALL_STYLE_ADOPTION_READY`; World HUD style adoption is tracked under `LGO_WORLD_HUD_STYLE_ADOPTION_READY`; runtime UI skin adoption evidence refresh is tracked under `LGO_RUNTIME_UI_SKIN_ADOPTION_EVIDENCE_REFRESH_READY`; runtime UI skin usage guide is tracked under `LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY`; runtime UI style duplication audit is tracked under `LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY`; runtime UI factory split review is tracked under `LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY`; runtime UI primitive factory is tracked under `LGO_RUNTIME_UI_PRIMITIVE_FACTORY_READY`; runtime UI button factory adoption is tracked under `LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_READY`; login debug-dot cleanup is tracked under `LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_READY`; login debug-dot evidence is tracked under `LGO_LOGIN_CTA_DEBUG_DOT_EVIDENCE_REFRESH_READY`; login CTA backing balance is tracked under `LGO_LOGIN_CTA_BACKING_BALANCE_READY`; login CTA backing evidence is tracked under `LGO_LOGIN_CTA_BACKING_EVIDENCE_REFRESH_READY`; runtime UI skin foundation is tracked under `LGO_RUNTIME_UI_SKIN_FOUNDATION_READY`; runtime UI skin adoption audit is tracked under `LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_READY`; login NPC grounding shadow balance is tracked under `LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_READY`; login NPC grounding evidence is tracked under `LGO_LOGIN_NPC_GROUNDING_SHADOW_EVIDENCE_REFRESH_READY`; the project still refuses to claim visual PASS from capture/build alone.

## Ready Marker Registry

This registry keeps historical source gates discoverable while `Next task` points only at the current task.

- `LGO-LOGIN-PANEL-VISUAL-BALANCE-PASS-v1.0` / `LGO_LOGIN_PANEL_VISUAL_BALANCE_READY`
- `LGO-LOGIN-CTA-ORNAMENT-LIGHTWEIGHT-PASS-v1.0` / `LGO_LOGIN_CTA_ORNAMENT_LIGHTWEIGHT_READY`
- `LGO-LOGIN-RESPONSIVE-SCALE-CLEANUP-PASS-v1.0` / `LGO_LOGIN_RESPONSIVE_SCALE_CLEANUP_READY`
- `LGO-CHARACTER-HALL-PANEL-DENSITY-PASS-v1.0` / `LGO_CHARACTER_HALL_PANEL_DENSITY_READY`
- `LGO-CHARACTER-CREATE-FORM-PRESENTATION-PASS-v1.0` / `LGO_CHARACTER_CREATE_FORM_PRESENTATION_READY`
- `LGO-CHARACTER-HALL-RESPONSIVE-EVIDENCE-REFRESH-v1.0` / `LGO_CHARACTER_HALL_RESPONSIVE_EVIDENCE_REFRESH_READY`
- `LGO-VISUAL-RUNTIME-FAST-PROFILE-REUSE-PASS-v1.0` / `LGO_VISUAL_RUNTIME_FAST_PROFILE_REUSE_READY`
- `LGO-WORLD-HUD-ACTION-SHELL-V3B-SKIN-PASS-v1.0` / `LGO_WORLD_HUD_ACTION_SHELL_V3B_SKIN_READY`
- `LGO-WORLD-HUD-ACTION-SHELL-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_ACTION_SHELL_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-MOBILE-CAMERA-FRAMING-PASS-v1.0` / `LGO_WORLD_MOBILE_CAMERA_FRAMING_READY`
- `LGO-WORLD-MOBILE-CAMERA-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_MOBILE_CAMERA_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-LABEL-SAFE-AREA-PASS-v1.0` / `LGO_WORLD_LABEL_SAFE_AREA_READY`
- `LGO-WORLD-LABEL-SAFE-AREA-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_LABEL_SAFE_AREA_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-TOP-STATUS-MOBILE-READABILITY-PASS-v1.0` / `LGO_WORLD_TOP_STATUS_MOBILE_READABILITY_READY`
- `LGO-WORLD-TOP-STATUS-MOBILE-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_TOP_STATUS_MOBILE_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-ACTOR-HUD-OCCLUSION-PASS-v1.0` / `LGO_WORLD_ACTOR_HUD_OCCLUSION_READY`
- `LGO-WORLD-ACTOR-HUD-OCCLUSION-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_ACTOR_HUD_OCCLUSION_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-HUD-DIALOGUE-PANEL-VIEWPORT-POLISH-v1.0` / `LGO_WORLD_HUD_DIALOGUE_PANEL_VIEWPORT_POLISH_READY`
- `LGO-WORLD-HUD-DIALOGUE-PANEL-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_DIALOGUE_PANEL_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-HUD-MOBILE-HIERARCHY-POLISH-v1.0` / `LGO_WORLD_HUD_MOBILE_HIERARCHY_POLISH_READY`
- `LGO-WORLD-HUD-MOBILE-HIERARCHY-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_MOBILE_HIERARCHY_EVIDENCE_REFRESH_READY`
- `LGO-SOURCE-GATE-EVIDENCE-PRESERVATION-PASS-v1.0` / `LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_READY`
- `LGO-VISUAL-EVIDENCE-PROFILE-INDEX-PASS-v1.0` / `LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY`
- `LGO-VISUAL-RUNTIME-REVIEW-HEURISTICS-PASS-v1.0` / `LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY`
- `LGO-WORLD-HUB-PROP-LABEL-RESPONSIVE-PASS-v1.0` / `LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY`
- `LGO-WORLD-SCENE-DEPTH-LAYERING-PASS-v1.0` / `LGO_WORLD_SCENE_DEPTH_LAYERING_READY`
- `LGO-WORLD-RESPONSIVE-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_RESPONSIVE_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-HUB-VISUAL-READABILITY-CLEANUP-PASS-v1.0` / `LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_READY`
- `LGO-WORLD-HUB-INTERACTION-READABILITY-PASS-v1.0` / `LGO_WORLD_HUB_INTERACTION_READABILITY_READY`
- `LGO-WORLD-HUB-INTERACTION-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUB_INTERACTION_EVIDENCE_REFRESH_READY`
- `LGO-NEAR-INTERACTION-CHECKPOINT-CAPTURE-PASS-v1.0` / `LGO_NEAR_INTERACTION_CHECKPOINT_CAPTURE_READY`
- `LGO-NEAR-INTERACTION-EVIDENCE-REFRESH-v1.0` / `LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_READY`
- `LGO-POST-LOGIN-VISUAL-EVIDENCE-UPLOAD-PACKAGING-v1.0` / `LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY`
- `LGO-RUNTIME-ASSET-WEIGHT-BUDGET-REFRESH-v1.0` / `LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY`
- `LGO-RUNTIME-ASSET-WATCH-QUEUE-IMPORT-PROFILE-POLISH-v1.0` / `LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY`
- `LGO-POST-LOGIN-RUNTIME-UI-REUSE-CLEANUP-v1.0` / `LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY`
- `LGO-POST-LOGIN-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0` / `LGO_POST_LOGIN_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY`
- `LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-v1.0` / `LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_READY`
- `LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-EVIDENCE-REFRESH-v1.0` / `LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-FACTORY-CHARACTER-HALL-CLEANUP-FOLLOWUP-v1.0` / `LGO_RUNTIME_UI_FACTORY_CHARACTER_HALL_CLEANUP_FOLLOWUP_READY`
- `LGO-WORLD-HUD-RUNTIME-UI-REUSE-AUDIT-v1.0` / `LGO_WORLD_HUD_RUNTIME_UI_REUSE_AUDIT_READY`
- `LGO-WORLD-HUD-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-HUD-FANTASY-PANEL-HIERARCHY-POLISH-v1.0` / `LGO_WORLD_HUD_FANTASY_PANEL_HIERARCHY_POLISH_READY`
- `LGO-WORLD-HUD-FANTASY-PANEL-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_FANTASY_PANEL_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STATUS-COMPOSITION-CLEANUP-v1.0` / `LGO_RUNTIME_UI_STATUS_COMPOSITION_CLEANUP_READY`
- `LGO-RUNTIME-UI-STATUS-COMPOSITION-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_STATUS_COMPOSITION_EVIDENCE_REFRESH_READY`
- `LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-LIGHTNESS-PASS-v1.0` / `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_LIGHTNESS_READY`
- `LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-EVIDENCE-REFRESH-v1.0` / `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STATE-DOC-COMPACTION-AUDIT-v1.0` / `LGO_RUNTIME_UI_STATE_DOC_COMPACTION_AUDIT_READY`
- `LGO-EXECUTION-LEDGER-ROLLUP-VIEW-v1.0` / `LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY`
- `LGO-COMMIT-CADENCE-POLICY-HARDENING-v1.0` / `LGO_COMMIT_CADENCE_POLICY_HARDENING_READY`
- `LGO-RUNTIME-UI-QUALITY-DEBT-TRIAGE-v1.0` / `LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY`
- `LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0` / `LGO_LOGIN_NPC_GROUNDING_CTA_PANEL_POLISH_READY`
- `LGO-WORLD-HUB-VISUAL-DEBT-TRIAGE-v1.0` / `LGO_WORLD_HUB_VISUAL_DEBT_TRIAGE_READY`
- `LGO-SESSION-MENU-FOCUS-EVIDENCE-REFRESH-v1.0` / `LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_READY`
- `LGO-CHARACTER-HALL-MOBILE-COPY-DENSITY-PASS-v1.0` / `LGO_CHARACTER_HALL_MOBILE_COPY_DENSITY_READY`
- `LGO-CHARACTER-HALL-MOBILE-COPY-EVIDENCE-REFRESH-v1.0` / `LGO_CHARACTER_HALL_MOBILE_COPY_EVIDENCE_REFRESH_READY`
- `LGO-CHARACTER-HALL-MOBILE-SELECTED-CTA-HIERARCHY-PASS-v1.0` / `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_HIERARCHY_READY`
- `LGO-CHARACTER-HALL-MOBILE-SELECTED-CTA-EVIDENCE-REFRESH-v1.0` / `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_EVIDENCE_REFRESH_READY`
- `LGO-LOGIN-CTA-DEBUG-DOT-CLEANUP-PASS-v1.0` / `LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_READY`
- `LGO-LOGIN-CTA-DEBUG-DOT-EVIDENCE-REFRESH-v1.0` / `LGO_LOGIN_CTA_DEBUG_DOT_EVIDENCE_REFRESH_READY`
- `LGO-LOGIN-CTA-BACKING-BALANCE-PASS-v1.0` / `LGO_LOGIN_CTA_BACKING_BALANCE_READY`
- `LGO-LOGIN-CTA-BACKING-EVIDENCE-REFRESH-v1.0` / `LGO_LOGIN_CTA_BACKING_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SKIN-FOUNDATION-PASS-v1.0` / `LGO_RUNTIME_UI_SKIN_FOUNDATION_READY`
- `LGO-RUNTIME-UI-SKIN-ADOPTION-AUDIT-PASS-v1.0` / `LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_READY`
- `LGO-CHARACTER-HALL-STYLE-ADOPTION-PASS-v1.0` / `LGO_CHARACTER_HALL_STYLE_ADOPTION_READY`
- `LGO-WORLD-HUD-STYLE-ADOPTION-PASS-v1.0` / `LGO_WORLD_HUD_STYLE_ADOPTION_READY`
- `LGO-RUNTIME-UI-SKIN-ADOPTION-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_SKIN_ADOPTION_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SKIN-USAGE-GUIDE-PASS-v1.0` / `LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY`
- `LGO-RUNTIME-UI-STYLE-DUPLICATION-AUDIT-v1.0` / `LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY`
- `LGO-RUNTIME-UI-FACTORY-SPLIT-REVIEW-v1.0` / `LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-FACTORY-PASS-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_FACTORY_READY`
- `LGO-RUNTIME-UI-BUTTON-FACTORY-ADOPTION-PASS-v1.0` / `LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_READY`
- `LGO-RUNTIME-UI-CONTROLLER-RESPONSIBILITY-MAP-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_RESPONSIBILITY_MAP_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_LAYOUT_HELPER_REVIEW_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-CONSTANTS-AUDIT-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_CONSTANTS_AUDIT_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-SESSION-SHELL-HELPER-REVIEW-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_SESSION_SHELL_HELPER_REVIEW_READY`
- `LGO-RUNTIME-UI-FACTORY-ADOPTION-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_FACTORY_ADOPTION_EVIDENCE_REFRESH_READY`
- `LGO-SESSION-MENU-SETTING-ROW-VISUAL-POLISH-v1.0` / `LGO_SESSION_MENU_SETTING_ROW_VISUAL_POLISH_READY`
- `LGO-SESSION-MENU-SETTING-ROW-EVIDENCE-REFRESH-v1.0` / `LGO_SESSION_MENU_SETTING_ROW_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SCREEN-SHELL-COMPONENT-REVIEW-v1.0` / `LGO_RUNTIME_UI_SCREEN_SHELL_COMPONENT_REVIEW_READY`
- `LGO-WORLD-POSE-PULSE-VISUAL-CLEANUP-v1.0` / `LGO_WORLD_POSE_PULSE_VISUAL_CLEANUP_READY`
- `LGO-RUNTIME-UI-SCREEN-SHELL-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_SCREEN_SHELL_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-ACTION-ROW-COMPONENT-REVIEW-v1.0` / `LGO_RUNTIME_UI_ACTION_ROW_COMPONENT_REVIEW_READY`
- `LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_ACTION_ROW_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-INPUT-FIELD-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_INPUT_FIELD_BASE_READY`
- `LGO-RUNTIME-UI-INPUT-FIELD-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_INPUT_FIELD_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-FORM-SECTION-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_FORM_SECTION_BASE_READY`
- `LGO-RUNTIME-UI-FORM-SECTION-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_FORM_SECTION_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-LIST-CARD-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_LIST_CARD_BASE_READY`
- `LGO-RUNTIME-UI-LIST-CARD-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_LIST_CARD_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STATUS-CHIP-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_STATUS_CHIP_BASE_READY`
- `LGO-RUNTIME-UI-STATUS-CHIP-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_STATUS_CHIP_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-TOGGLE-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_TOGGLE_BASE_READY`
- `LGO-RUNTIME-UI-TOGGLE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_TOGGLE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-ICON-STATUS-ROW-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_ICON_STATUS_ROW_BASE_READY`
- `LGO-RUNTIME-UI-ICON-STATUS-ROW-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_ICON_STATUS_ROW_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMBAT-BUTTON-METRICS-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMBAT_BUTTON_METRICS_READY`
- `LGO-RUNTIME-UI-COMBAT-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMBAT_BUTTON_METRICS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMBAT-HUD-SPACING-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMBAT_HUD_SPACING_READY`
- `LGO-RUNTIME-UI-COMBAT-HUD-SPACING-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMBAT_HUD_SPACING_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-AUDIT-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-AUDIT-v1.0` / `LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_READY`
- `LGO-RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-LABEL-FONT-METRICS-AUDIT-v1.0` / `LGO_RUNTIME_UI_LABEL_FONT_METRICS_READY`
- `LGO-RUNTIME-UI-LABEL-FONT-METRICS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_LABEL_FONT_METRICS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-TYPOGRAPHY-OWNERSHIP-SPLIT-REVIEW-v1.0` / `LGO_RUNTIME_UI_TYPOGRAPHY_OWNERSHIP_SPLIT_READY`
- `LGO-RUNTIME-UI-TYPOGRAPHY-OWNERSHIP-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_TYPOGRAPHY_OWNERSHIP_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-DRIFT-SCAN-v1.0` / `LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_DRIFT_SCAN_READY`
- `LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-PANEL-HIERARCHY-SIMPLIFICATION-PASS-v1.0` / `LGO_RUNTIME_UI_PANEL_HIERARCHY_SIMPLIFICATION_READY`
- `LGO-RUNTIME-UI-PANEL-HIERARCHY-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_PANEL_HIERARCHY_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-POLISH-v1.0` / `LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_READY`
- `LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMPONENT-DENSITY-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMPONENT_DENSITY_BASE_READY`
- `LGO-RUNTIME-UI-COMPONENT-DENSITY-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPONENT_DENSITY_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-DENSITY-ADOPTION-SCAN-v1.0` / `LGO_RUNTIME_UI_DENSITY_ADOPTION_SCAN_READY`
- `LGO-M5-VISUAL-EVIDENCE-RUNNER-SKIN-ADOPTION-EVIDENCE-v1.0` / `LGO_M5_VISUAL_EVIDENCE_RUNNER_SKIN_ADOPTION_EVIDENCE_READY`
- `LGO-VISUAL-EVIDENCE-OUTPUT-ISOLATION-AUDIT-v1.0` / `LGO_VISUAL_EVIDENCE_OUTPUT_ISOLATION_READY`
- `LGO-QUICK-FULL-GATE-STRATEGY-v1.0` / `LGO_QUICK_FULL_GATE_STRATEGY_READY`
- `LGO-VISUAL-EVIDENCE-BLANK-SCREEN-DETECTION-v1.0` / `LGO_VISUAL_EVIDENCE_BLANK_SCREEN_DETECTION_READY`
- `LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-v1.0` / `LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_READY`
- `LGO-RUNTIME-ASSET-WEIGHT-ACTIONABLE-BUDGET-v1.0` / `LGO_RUNTIME_ASSET_WEIGHT_ACTIONABLE_BUDGET_READY`
- `LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-EVIDENCE-v1.0` / `LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_EVIDENCE_READY`
- `LGO-RUNTIME-ASSET-WATCH-QUEUE-PRIORITIZATION-v1.0` / `LGO_RUNTIME_ASSET_WATCH_QUEUE_PRIORITY_READY`
- `LGO-RUNTIME-ASSET-WATCH-QUEUE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_ASSET_WATCH_QUEUE_EVIDENCE_REFRESH_READY`
- `LGO-EVIDENCE-GATE-SEQUENTIAL-RUN-POLICY-v1.0` / `LGO_EVIDENCE_GATE_SEQUENTIAL_RUN_POLICY_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-STYLE-APPLICATION-AUDIT-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_STYLE_APPLICATION_AUDIT_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-STYLE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_STYLE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-FACTORY-COVERAGE-AUDIT-v1.0` / `LGO_RUNTIME_UI_FACTORY_COVERAGE_AUDIT_READY`
- `LGO-RUNTIME-UI-IMAGE-LAYER-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_IMAGE_LAYER_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STYLE-DEBT-FOLLOWUP-AUDIT-v1.0` / `LGO_RUNTIME_UI_STYLE_DEBT_FOLLOWUP_AUDIT_READY`
- `LGO-RUNTIME-UI-COMPACT-STATUS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPACT_STATUS_EVIDENCE_REFRESH_READY`
- `LGO-COMBAT-BUTTON-STATE-READABILITY-POLISH-v1.0` / `LGO_COMBAT_BUTTON_STATE_READABILITY_POLISH_READY`
- `LGO-COMBAT-BUTTON-STATE-EVIDENCE-REFRESH-v1.0` / `LGO_COMBAT_BUTTON_STATE_EVIDENCE_REFRESH_READY`
- `LGO-COMBAT-BUTTON-MOBILE-RESPONSIVE-EVIDENCE-v1.0` / `LGO_COMBAT_BUTTON_MOBILE_RESPONSIVE_EVIDENCE_READY`
- `LGO-WORLD-HUD-COMPONENT-BOUNDARY-AUDIT-v1.0` / `LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY`
- `LGO-RUNTIME-UI-EVIDENCE-STATE-HELPER-REVIEW-v1.0` / `LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY`
- `LGO-WORLD-HUD-HEADER-BLOCK-REVIEW-v1.0` / `LGO_WORLD_HUD_HEADER_BLOCK_READY`
- `LGO-WORLD-HUD-ROW-HELPER-COVERAGE-AUDIT-v1.0` / `LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY`
- `LGO-WORLD-HUD-ROW-HELPER-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_ROW_HELPER_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STYLE-OWNERSHIP-DRIFT-AUDIT-v1.0` / `LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY`
- `LGO-RUNTIME-UI-STYLE-OWNERSHIP-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_STYLE_OWNERSHIP_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-CONTROLLER-STYLE-CONSTANTS-AUDIT-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_READY`
- `LGO-RUNTIME-UI-CONTROLLER-STYLE-CONSTANTS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-PADDING-PROFILE-AUDIT-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-PADDING-PROFILE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SESSION-MENU-PADDING-PROFILE-AUDIT-v1.0` / `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY`
- `LGO-RUNTIME-UI-SESSION-MENU-PADDING-PROFILE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-FACTORY-PADDING-HELPER-COVERAGE-AUDIT-v1.0` / `LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_READY`
- `LGO-RUNTIME-UI-FACTORY-PADDING-HELPER-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-CANDIDATE-AUDIT-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY`
- `LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-ONE-EDGE-LAYOUT-HELPER-AUDIT-v1.0` / `LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_HELPER_READY`
- `LGO-RUNTIME-UI-ONE-EDGE-LAYOUT-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMPONENT-MARGIN-TOKEN-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_READY`
- `LGO-RUNTIME-UI-COMPONENT-MARGIN-TOKEN-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-AUDIT-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-AUDIT-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-STYLE-BOUNDARY-GUIDE-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_STYLE_BOUNDARY_GUIDE_READY`
- `LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-DRIFT-SCAN-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_DRIFT_SCAN_READY`
- `LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_EVIDENCE_REFRESH_READY`
- `LGO-LOGIN-CTA-COMPONENT-VISUAL-POLISH-v1.0` / `LGO_LOGIN_CTA_COMPONENT_VISUAL_POLISH_READY`
- `LGO-LOGIN-CTA-COMPONENT-EVIDENCE-REFRESH-v1.0` / `LGO_LOGIN_CTA_COMPONENT_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMPONENT-BASE-REUSE-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMPONENT_BASE_REUSE_READY`
- `LGO-RUNTIME-UI-COMPONENT-BASE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPONENT_BASE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SCREEN-SHELL-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_SCREEN_SHELL_BASE_READY`
- `LGO-RUNTIME-UI-SCREEN-SHELL-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_SCREEN_SHELL_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-ACTION-ROW-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_ACTION_ROW_BASE_READY`
- `LGO-LOGIN-NPC-GROUNDING-SHADOW-BALANCE-PASS-v1.0` / `LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_READY`
- `LGO-LOGIN-NPC-GROUNDING-SHADOW-EVIDENCE-REFRESH-v1.0` / `LGO_LOGIN_NPC_GROUNDING_SHADOW_EVIDENCE_REFRESH_READY`

## Allowed paths

- `AGENTS.md`
- `.vscode/tasks.json`
- `tools/lgo_continue_dev_loop.sh`
- `tools/lgo_visual_runtime_review.sh`
- `tools/lgo_codex_autopilot.sh`
- `tools/lgo_codex_write_status.sh`
- `docs/execution/CODEX-AUTOPILOT.md`
- `client/Unity/Assets/Game/UI/Runtime/**`
- `client/Unity/Assets/Game/Bootstrap/**`
- `client/Unity/Assets/Game/World/**`
- `docs/execution/**`
- `docs/art/**`
- `docs/tasks/**`

## Forbidden paths

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`
- production auth, DB, economy, social, liveops

## Validation commands

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py
python3.12 tools/validate_lgo_runtime_asset_weight.py
python3.12 tools/validate_lgo_runtime_asset_import_profiles.py
python3.12 tools/validate_lgo_runtime_asset_weight_budget_refresh.py
python3.12 tools/validate_lgo_runtime_asset_watch_queue_import_profile.py
python3.12 tools/validate_lgo_device_profile_ui_budgets.py
python3.12 tools/validate_lgo_login_npc_compositing_polish.py
python3.12 tools/validate_lgo_login_panel_visual_balance.py
python3.12 tools/validate_lgo_login_cta_ornament_lightweight.py
python3.12 tools/validate_lgo_login_cta_debug_dot_cleanup.py
python3.12 tools/validate_lgo_login_cta_debug_dot_evidence_refresh.py
python3.12 tools/validate_lgo_login_cta_backing_balance.py
python3.12 tools/validate_lgo_login_cta_backing_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_skin_foundation.py
python3.12 tools/validate_lgo_runtime_ui_skin_adoption_audit.py
python3.12 tools/validate_lgo_login_npc_grounding_shadow_balance.py
python3.12 tools/validate_lgo_login_npc_grounding_shadow_evidence_refresh.py
python3.12 tools/validate_lgo_login_responsive_scale_cleanup.py
python3.12 tools/validate_lgo_character_hall_v3b_composition.py
python3.12 tools/validate_lgo_character_hall_style_adoption.py
python3.12 tools/validate_lgo_character_hall_panel_density.py
python3.12 tools/validate_lgo_character_hall_mobile_copy_density.py
python3.12 tools/validate_lgo_character_hall_mobile_copy_evidence_refresh.py
python3.12 tools/validate_lgo_character_hall_mobile_selected_cta_hierarchy.py
python3.12 tools/validate_lgo_character_hall_mobile_selected_cta_evidence_refresh.py
python3.12 tools/validate_lgo_character_create_form_presentation.py
python3.12 tools/validate_lgo_character_hall_responsive_evidence_refresh.py
python3.12 tools/validate_lgo_visual_runtime_fast_profile_reuse.py
python3.12 tools/validate_lgo_world_hud_action_shell_v3b_skin.py
python3.12 tools/validate_lgo_world_hud_style_adoption.py
python3.12 tools/validate_lgo_runtime_ui_skin_adoption_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_skin_usage_guide.py
python3.12 tools/validate_lgo_runtime_ui_style_duplication_audit.py
python3.12 tools/validate_lgo_runtime_ui_factory_split_review.py
python3.12 tools/validate_lgo_runtime_ui_primitive_factory.py
python3.12 tools/validate_lgo_runtime_ui_button_factory_adoption.py
python3.12 tools/validate_lgo_runtime_ui_controller_responsibility_map.py
python3.12 tools/validate_lgo_runtime_ui_responsive_layout_helper_review.py
python3.12 tools/validate_lgo_runtime_ui_responsive_constants_audit.py
python3.12 tools/validate_lgo_runtime_ui_responsive_session_shell_helper_review.py
python3.12 tools/validate_lgo_runtime_ui_factory_adoption_evidence_refresh.py
python3.12 tools/validate_lgo_session_menu_setting_row_visual_polish.py
python3.12 tools/validate_lgo_session_menu_setting_row_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_screen_shell_component_review.py
python3.12 tools/validate_lgo_world_pose_pulse_visual_cleanup.py
python3.12 tools/validate_lgo_runtime_ui_screen_shell_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_action_row_component_review.py
python3.12 tools/validate_lgo_runtime_ui_action_row_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_responsive_style_application_audit.py
python3.12 tools/validate_lgo_runtime_ui_responsive_style_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_factory_coverage_audit.py
python3.12 tools/validate_lgo_runtime_ui_image_layer_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_style_debt_followup_audit.py
python3.12 tools/validate_lgo_runtime_ui_compact_status_evidence_refresh.py
python3.12 tools/validate_lgo_combat_button_state_readability_polish.py
python3.12 tools/validate_lgo_combat_button_state_evidence_refresh.py
python3.12 tools/validate_lgo_world_hud_action_shell_evidence_refresh.py
python3.12 tools/validate_lgo_world_mobile_camera_framing.py
python3.12 tools/validate_lgo_world_mobile_camera_evidence_refresh.py
python3.12 tools/validate_lgo_world_label_safe_area.py
python3.12 tools/validate_lgo_world_label_safe_area_evidence_refresh.py
python3.12 tools/validate_lgo_world_top_status_mobile_readability.py
python3.12 tools/validate_lgo_world_top_status_mobile_evidence_refresh.py
python3.12 tools/validate_lgo_world_actor_hud_occlusion.py
python3.12 tools/validate_lgo_world_actor_hud_occlusion_evidence_refresh.py
python3.12 tools/validate_lgo_world_hud_dialogue_panel_viewport_polish.py
python3.12 tools/validate_lgo_world_hud_dialogue_panel_evidence_refresh.py
python3.12 tools/validate_lgo_world_hud_mobile_hierarchy_polish.py
python3.12 tools/validate_lgo_world_hud_mobile_hierarchy_evidence_refresh.py
python3.12 tools/validate_lgo_source_gate_evidence_preservation.py
python3.12 tools/validate_lgo_visual_evidence_profile_index.py
python3.12 tools/validate_lgo_build_size_budget.py
python3.12 tools/validate_lgo_world_hud_density_mobile_touch.py
python3.12 tools/validate_lgo_world_ground_visual_quality.py
python3.12 tools/validate_lgo_visual_runtime_review_heuristics.py
python3.12 tools/validate_lgo_world_hub_prop_label_responsive.py
python3.12 tools/validate_lgo_world_scene_depth_layering.py
python3.12 tools/validate_lgo_world_hub_visual_readability_cleanup.py
python3.12 tools/validate_lgo_world_hub_visual_debt_triage.py
python3.12 tools/validate_lgo_session_menu_focus_evidence_refresh.py
python3.12 tools/validate_lgo_world_hub_interaction_readability.py
python3.12 tools/validate_lgo_world_hub_interaction_evidence_refresh.py
python3.12 tools/validate_lgo_near_interaction_checkpoint_capture.py
python3.12 tools/validate_lgo_near_interaction_evidence_refresh.py
python3.12 tools/package_lgo_visual_evidence_upload.py --verify-only
python3.12 tools/validate_lgo_post_login_visual_evidence_upload_packaging.py
python3.12 tools/validate_lgo_world_responsive_evidence_refresh.py
python3.12 tools/validate_m4_2_playable_ui.py
python3.12 tools/validate_m4_visible_ui.py
python3.12 tools/validate_m6_combat_visual_readability.py
python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py
python3.12 tools/validate_package_hygiene.py
./tools/lgo_continue_dev_loop.sh
./tools/lgo_codex_autopilot.sh --dry-run
```

## Runtime evidence command

```bash
./tools/lgo_visual_runtime_review.sh
```

Fast UI/visual iteration command after nearby full gates are already green:

```bash
./tools/lgo_visual_runtime_review_profiles.sh
```

Expected classifications:

- `PASS`
- `FIX_REQUIRED`
- `VISUAL_CAPTURE_TIMEOUT`
- `VIDEO_CAPTURE_BLOCKED_ENV`
- `RUNTIME_BLOCKED_ENV`

## Stop conditions

- A frozen contract/protocol/schema/ADR change is required.
- Unity/player tooling is unavailable or visual capture is blocked by environment.
- A gate fails and cannot be fixed within allowed paths.
- Owner product/art decision is required.
- No valid next action remains.

## Follow-up task after current task

`LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0` has refreshed visual evidence and manual review notes in this session. Continue with `LGO-RUNTIME-UI-QUALITY-DEBT-FIRST-FIX-v1.0`, using existing validators and avoiding new micro-task docs unless a reusable gate is genuinely needed.

Latest closed batch: `LGO_WORLD_HUD_MOBILE_TOUCH_AFFORDANCE_READY` adds shared mobile World Hub touch affordances through factory/profile/layout bases, with reviewed desktop/tablet/mobile profile screenshots and no `VISUAL_RUNTIME_PASS` claim.

Latest closed UI ratio batch: `LGO_CHARACTER_HALL_SELECTED_CTA_RATIO_READY` moves selected Character Hall action sizing into a shared responsive ratio so the enter-world CTA is visually primary and the create action is secondary across desktop/tablet/mobile evidence.

Recent visual passes improved scene depth, NPC staging, responsive HUD behavior, world staging density, label readability, mobile touch affordances, and evidence review scoring without new gameplay or frozen-surface changes.
