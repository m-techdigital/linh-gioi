# Linh Giới Online — Project State

Last updated: `2026-09-08`

## Continuous workflow status

Current operating mode: `LGO_CONTINUOUS_WORKFLOW_ACTIVE`.

Current active next action is tracked in `docs/execution/NEXT-ACTION.md`.

- Checkpoint macro REMAKE keeper v4: đã thay toàn thân, đầu/mặt/tóc, nón, giáp, robe/cape và portrait bằng mesh 3D mới bám sheet; source `client/art-source/gate-keeper/KeeperReconstruction.blend`, 24.898 triangles, 65 bones, 2 atlas. Import/build `keeper-clothed-*`, 19/19 tests guide/dialogue `keeper-reconstruction-tests.xml`, quick gate và toàn tuyến runtime 960x540 `keeper-clothed-mobile.log` qua; ảnh keeper-side/dialogue/keeper-guide-direction đã xem. CHƯA VISUAL PASS: cape vẫn tạo tam giác khi IK chỉ đường (P1), hoa văn/nón/mép vải chưa sắc như design. NEXT: tách và gắn xương cape theo cấu trúc trang phục thực, giữ model remake; xử lý cả vùng biến dạng trong một macro pass rồi capture pose IK thật. Không quay lại primitive, không coi tool là blocker, không đổi frozen surfaces. Evidence tại `build/visual-evidence/onboarding-blockout/keeper-clothed-mobile/`; capture timeout cũ được giữ nguyên.

Sân nghỉ nay có ghế gỗ rõ mặt ngồi/chân/tựa; build/full-route960x540/quick `pavilion-rest-final-*` qua, ảnh đã xem. Một mesh nhỏ thay khối đặc; chưa chức năng ngồi hoặc art final. Tiếp kiến trúc đình/phố theo demo.

- Đá Luyện có bề mặt xanh xám sáng hơn và dấu xoắn cộng hưởng theo reference SCN-002; nét dày hơn sau review960x540, căn giữa/rộng20cm trên hai mặt. Một mesh128tri dùng chung và material pulse cũ, không texture/đèn/collider mới; giữ đá1.1m và completion/revisit. `build/dev-loop/stone-inlay-readable-mobile.log` toàn tuyến/mesh hướng ngoài/bounds/pulse/phục hồi/đi sân/quay lại NPC qua; ảnh stone-side/complete/complete-settled đã xem trong `build/visual-evidence/onboarding-blockout/stone-inlay-readable-mobile/`. Build `stone-inlay-readable-build.log`, quick `stone-inlay-readable-quick.log` qua. Chưa art final, thiết bị thật, mọi viewport hoặc VISUAL_RUNTIME_PASS. NEXT: tiếp phố/sân và flow khám phá theo storyboard/design sẵn có; không dừng ở polish biểu tượng hoặc validator, giữ base chung và frozen surfaces.

- Checkpoint Người Giữ Cổng trẻ v4 candidate: model/portrait cùng identity nón/tóc đen/áo xanh-ngà đã vào runtime; garment tách vạt trước/tà bên/cape, tay áo liên tục, sửa mặt bị cull và chuyển màu Blender→Unity. Reuse rig/controller/thoại/IK chỉ đá, không đổi gameplay contract. Hai FBX arrival/keeper có65 xương/rest matrix khớp tuyệt đối; keeper13.074 vertices/24.764 triangles sau round-trip,1renderer/6material, dùng lại Skin512/Eyes128. Chưa phải final art/modular assembler, chưa body occlusion/LOD/áo choàng chạy hoặc GPU/mobile-device proof.

- Người Giữ Cổng nay là Humanoid3D theo turnaround DRAFT, portrait128 render từ cùng model; shared generator/importer/instantiation và base thoại nhận portrait tùy chọn. Idle/quay về người chơi khi thoại/trở về hướng đón khách; collider/range giữ nguyên. Red sprite rồi sửa grounding bằng mesh bake (không dùng culling bounds), tà áo chuyển sang hip support sau review, nâng origin4cm theo số đo. `keeper-robe-{mobile,desktop,tablet}` full route/ground/idle/facing/portrait qua, ảnh đã xem; `keeper-hall-mobile` hai visit và `keeper-shared-import-regression` qua; quick qua. FBX1199548B, portrait20599B, không texture da/mắt/clip mới; đây là source bytes, không build delta. Gom cả seal pulse; 22 file gồm13 asset/meta/reference vượt advisory18, không đổi ngưỡng chung. Chưa art final/visual PASS, tư thế chờ còn cứng. Next: Kiểm chứng Idle_Talking_Loop CC0 và thiết kế cử chỉ giao tiếp Người Giữ Cổng theo turnaround/dialogue demo; tư thế chờ hiện còn cứng. Reuse importer/rig, chỉ thêm clip cần dùng nếu ảnh runtime chứng minh hợp vai trò; giữ root/collider/range, không mở canon/quest/production systems.

- Cử chỉ Đá Luyện dùng Interact Humanoid CC0 dài2s, clip+meta239571B; root motion tắt, tự về locomotion và di chuyển hủy. Red `stone-gesture-early-trace` bắt lệnh crossfade chưa xuất hiện trong Animator state; sửa hủy theo quyền sở hữu yêu cầu, `stone-gesture-early-green` qua. `stone-gesture-final-mobile` full route/hủy giữa động tác, `stone-gesture-final-desktop` full route/tự kết thúc qua; ảnh đã xem. Tablet `stone-gesture-green-tablet` chỉ reuse hình/pose trước sửa early-input, không final-tablet cancellation claim. `stone-gesture-hall-mobile` hai visit/profile/lifecycle qua; importer rerun cùng SHA, quick qua. Không IK/tay chạm mặt đá, combat animation, art final hoặc visual PASS. Next: Đối chiếu khung3 storyboard để làm phản hồi linh khí tại Đá Luyện rõ hơn: nối nhịp sáng dấu đá với cử chỉ và pulse hiện có, giữ asset nhẹ, không thêm đèn/texture lớn hoặc mở skill/quest mới.

Đá Luyện đổi từ cột nhọn1.5m sang khối thấp1.1m/vai rộng, dấu vàng mặt trước và mặt hướng đường bám slope mesh; chung cube/material, không texture/light mới. Collider0.65x1.5x0.65 giữ nguyên, không claim khớp bề mặt vật lý/art final. Red `stone-silhouette-red` bắt khối cao/mỏng; `stone-silhouette-green-{mobile,desktop,tablet}` silhouette/full route/focus/pulse/camera qua, ảnh đã xem; `stone-silhouette-hall-mobile` hai visit/profile/lifecycle qua, ảnh về sảnh đã xem; quick qua. Không visual PASS/GPU benchmark/khắc chữ canon hoặc production systems. Main không đổi, reuse evidence trước đúng scope. Next: Kiểm chứng clip Interact trong build/asset-staging/quaternius-animation/UAL1_Standard.fbx (CC0), reuse pipeline Humanoid hiện có để nhân vật có cử chỉ khi chạm Đá Luyện theo storyboard. Chỉ import clip cần dùng, root motion tắt, input di chuyển hủy cử chỉ; giữ quest/range/pulse và kiểm tra kích thước clip/lifecycle.

Guidance main/phố dùng cột và text primitive chung, không chip lồng; phố dùng NewWorldHudRoot và bounded-scroll skin chung, trần30% safe height. Red `guidance-group-red` bắt frame từng dòng. Bản `guidance-group-green-mobile` bị loại sau review vì scrollbar trắng/đệm dư; bỏ group padding và reuse overflow guard, không nới trần. Final `guidance-group-final-{mobile,desktop,tablet}` full route, nội dung thường không cuộn,40 dòng cuộn thật/không che pad qua; ảnh đã xem. `guidance-group-main-desktop` capture/manifest qua, ảnh world-hub đã xem; quick qua. Không asset mới/physical touch, resize trong khi cuộn-thoại, mọi viewport hoặc visual PASS/art final. Next: Đối chiếu Đá Luyện trong storyboard nhập môn, cải thiện hình khối và dấu nhận diện trên đá thay bia nhọn blockout; ưu tiên procedural mesh/material nhẹ, giữ collider/range/feedback hiện có, không mở skill hoặc quest mới.

Tên hồ sơ nay hiện trên nhân vật bằng WorldLabelPresenter; HUD ghi Linh Môn. Helper chung neo theo bounds/camera và tránh nhãn NPC/đá bằng screen bounds, ẩn/khôi phục trong thoại. Red thiếu nhãn ở `player-identity-red`, red chồng NPC ở `player-identity-green-mobile`; build lỗi accessibility đã sửa tại API World/UI. `player-identity-separated-{mobile,desktop}` full route/tên16 ký tự/thoại qua; `player-identity-confirm-tablet` thêm đá/camera-alley/completion qua; ảnh đã xem. Lượt `player-identity-verified-tablet` thoát sớm, không tính full route, chưa kết luận nguyên nhân. `player-identity-hall-mobile` hai visit/profile/cleanup nhãn-bóng qua; `player-identity-main-regression` technical capture/manifest qua; ảnh đã xem, quick qua. Không asset mới, không mọi vị trí/physical Escape/visual PASS/art final hoặc mapping outfit/gender. Next: Đối chiếu storyboard để gom địa điểm/mục tiêu/chỉ dẫn thành một nhóm HUD chung, bỏ ba dải status-chip rời hiện tại; giữ scroll/safe bounds và trạng thái thoại/completion cho main/phố, không thêm asset hoặc skin riêng.

Build development mặc định vào Linh Môn từ nút Vào game, không cần cờ onboarding; `--lgo-technical-yard` giữ fixture cũ và luôn ưu tiên, policy release giữ main. `hall-default-route-green.xml`8/8 (red2/8). Review tìm race chọn lại slot khi tải: `hall-entry-reselect-red.log` tái hiện; đã khóa entry/roster, snapshot ID, finally mở khóa. Final `hall-entry-guard-green-{mobile,desktop,tablet}.log` không cờ: lỗi hồ sơ thiếu phục hồi, hai vòng UI Submit/reselection guard/movement/profile/ownership qua, ảnh đã xem; `hall-default-technical-regression.log` harness cũ capture/manifest qua, ảnh đã xem. Quick qua. Không asset mới, không release-binary/cancellation/physical-key/full-lag hoặc visual PASS/art final; model chung chưa mapping outfit/gender, không quest persistence/combat production trong phố. Next: Đối chiếu reference world rồi làm rõ nhận diện người chơi tại Linh Môn: tên hồ sơ trên nhãn nhân vật, tên khu vực trong HUD, dùng base nhãn chung; kiểm tra tên dài/cạnh NPC/thoại, không thêm model hoặc texture nặng.

Gặp/Luyện/Đã xong dùng glyph hội thoại/bàn tay/check qua một helper chung ở main và phố; tooltip runtime dùng font/skin chung vì Unity tooltip chỉ hỗ trợ Editor. Ba PNG64 tổng3937B, license3208B, không mipmap/readable; generator khóa Lucide1.21.0/resvg2.6.2, tái tạo cùng SHA. Red icon/tooltip và bounds giữ ở `interaction-icon-red`, `interaction-tooltip-red`, `interaction-icon-bounds-red`; đã bỏ padding nút chữ và đổi phép đo rounding sang pixel thực. Final `interaction-tooltip-green-{mobile,desktop,tablet}.log` toàn tuyến/icon/hover-event/safe-bounds qua, ảnh đã xem; `interaction-tooltip-main-desktop.log` capture/manifest qua, ảnh đã xem. Main mobile trước tooltip ở `interaction-icon-main-mobile.log` giữ provenance riêng. Quick qua. Không physical pointer/touch, missing-asset fault injection, GC/GPU benchmark hoặc visual PASS/final art. Next: Kiểm tra gate để sảnh vào Linh Môn mặc định trong build development; giữ đường vào sân kỹ thuật qua cờ và không đổi production. Bám storyboard, test hai vòng vào/về sảnh, lựa chọn nhân vật và cleanup trước chuyển.

Người Giữ Cổng có vòng focus vàng khi trong tầm, reuse helper/sprite Đá Luyện; ẩn ngoài tầm, trong thoại và sau hoàn tất, đóng giữa chừng trả lại focus. Red `keeper-focus-red.log` thiếu vòng; `keeper-focus-green-mobile.log` giữ lỗi test thiếu bước chờ Update, đã sửa test không nới assertion. Final `keeper-focus-verified-{mobile,desktop,tablet}.log` toàn tuyến/focus qua (960x540,1920x1080,1366x1024), ảnh keeper-side đã xem; thoại mobile đã xem; `keeper-focus-hall-return-mobile.log` hai visit/ownership qua, ảnh về sảnh đã xem; quick qua. Không ảnh import mới, không visual PASS/final art; chưa test focus khi quay lại NPC sau completion hoặc Escape vật lý. Next: nút Gặp/Luyện có biểu tượng hội thoại/bàn tay theo storyboard, dùng base touch action chung và asset nhỏ phù hợp, không mở gameplay mới.

Sân nối phố: năm nhà xa dùng module/mặt tiền chung, xoay về sân và không collider; bồn cây thêm đất/bóng tiếp xúc bằng helper có sẵn; guidance xác nhận đến sân rồi ẩn, quay lại không phát lại. Bản tháp trắng `street-scenery-green-mobile` đã loại sau review. Red tại `street-scenery-red`, `garden-grounding-red`, `forecourt-arrival-red`. Final `forecourt-arrival-green-{mobile,desktop,tablet}.log` full route/scenery/grounding/feedback/reentry qua, ảnh đã xem; `street-garden-hall-return-mobile` hai vòng lifecycle qua, ảnh về sảnh đã xem; quick qua. Không ảnh import mới/quest persistence/art final/visual PASS/GPU benchmark. Next: vòng focus báo Người Giữ Cổng trong tầm tương tác, reuse base focus của Đá Luyện; tắt khi thoại mở hoặc rời tầm, không đổi range/quest.

Batch presentation: Trói Bóng bỏ cube hồng trùng, giữ một telegraph cho alert/preview; `shadow-warning-owner-green-{desktop,mobile}` main capture/manifest và lifecycle qua, ảnh đã xem. Phố reuse Skybox/Procedural đã serialize, chỉ đổi camera clear từ SolidColor sang Skybox; `street-sky-red` xác nhận root, `street-sky-green-{mobile,desktop}` full route qua/ảnh đã xem; `street-sky-hall-return-mobile` hai vòng giữ nguyên skybox/ambient/camera, ảnh về sảnh đã xem. Quick/M5 pose/M6 skill qua; không texture mới, không visual PASS/final sky/clouds hoặc missing-sprite fault-injection claim. Next: cảnh mái nhà ngoài tường sân theo vùng scenery của `lgo-street-forecourt-draft-v1.jpg`, reuse module/mesh/material, không mở lối đi hoặc gameplay mới.

Trói Bóng: Player red `shadow-warning-owner-red.log` xác nhận cube Alert Warning bật cùng telegraph, URP/Lit/màu hồng chứ không shader missing. Đã gom cảnh báo vào vòng hiện có, bỏ cube trùng; fallback chỉ tạo khi thiếu sprite. `shadow-warning-owner-green-{desktop,mobile}.log` exit0/capture/manifest, cảnh báo còn sau timeout và ẩn khi reset qua; ảnh đã xem, quick/M5 pose/M6 skill qua. Chưa commit để gom batch presentation; không ảnh mới/visual PASS, chưa fault-inject missing sprite. Next: nền trời ban ngày nhẹ cho preview phố theo `lgo-street-forecourt-draft-v1.jpg`, giữ camera/lighting và phục hồi môi trường khi về sảnh.

Panel kỹ năng đã bỏ khung lồng/tiêu đề lặp và override cyan; reuse nút compact chung, ba cột đều. HUD chung cuộn dọc, max-height đo từ mép trên thực tới pad trong safe-panel units. Red `skill-panel-red` bắt cyan; `skill-panel-bounded-mobile` bắt overflow do chỉ trừ header minimum. Final `skill-panel-measured-{desktop,tablet,mobile}.log` exit0/capture/manifest, cuộn nội dung dài qua; ảnh đã xem, quick và hai validator liên quan qua. Mobile giữ skill ở cụm đáy. Shared Menu/Về sảnh lên góc phải, resize/main ba profile qua; phố có sáu đèn reuse mesh/material, `street-hud-final-{mobile,desktop,tablet}` route qua/ảnh đã xem, `street-hud-hall-return-mobile` hai vòng phục hồi header/lifecycle qua. Không ảnh import mới, không physical Escape/mobile-device/visual PASS/final art. Next: truy renderer tạo ô hồng ở Trói Bóng; kiểm chứng fallback/telegraph ownership rồi sửa theo demo, không tăng payload.

Development `--lgo-onboarding-from-lobby` nối Vào game ở sảnh tới phố candidate, có Về sảnh qua button/handler Escape. Hai vòng/profile ở `hall-onboarding-verified-{mobile,desktop,tablet}.log` qua, ảnh/manifest đúng kích thước đã xem; profile API, account/client/slot không đổi, camera/ambient/nhãn/material thu hồi. Main `hall-onboarding-main-confirm.log` capture/manifest exit0, ảnh world/menu đã xem; lượt main đầu mất focus giữ log fail. Root HUD ẩn do UIDocument ancestry đã sửa; Escape OS bị quyền1002, chỉ claim handler; movement fail cũ chưa đủ nguyên nhân, telemetry sau~1.8m/0.5s. Không đổi default main, không asset mới/final art/quest persistence/visual PASS. Next: slot điều hướng phiên ở góc phải theo storyboard, sửa shared HUD layout cho main và phố, không nút riêng từng màn.

Sau dcda6a3, preview quay model về NPC khi thoại; red62.5572° -> green0°, full route/quick qua và ảnh dialogue đã xem. Không đổi vị trí/camera/UI/luật thoại, chưa commit để gom interaction. Kiểm tra mapping ngón đã đủ nên không sửa importer. Tiếp hướng nhìn/tư thế khi tương tác đá theo storyboard; chưa final art/main integration.

Lượt băng tay sau mở màn hình đã hoàn tất (`arrival-wrist-wrap-unlocked.log`,192frames/exit0), ảnh chạy đã xem; không còn blocker capture hiện tại. Gom rig/đai/wrap thành checkpoint. Tiếp candidate theo turnaround, kiểm tra mapping/tư thế bàn tay; chưa final art/main integration. Owner không cần chủ động xuất video bàn giao hoặc đề nghị chuyển sandbox nữa.

Băng tay candidate đã sửa từ mesh cẳng tay thay ống cố định/32 mặt phải đảo; Blender front/rear đã xem, Unity import/pose/build/quick qua. Video final chưa thực hiện: foreground loginwindow, Player không tạo frame; đã dừng đúng Player19412. Không visual PASS; nhóm rig/đai/wrap chưa commit. Tiếp source/asset không cần foreground, capture lại khi môi trường khả dụng.

Batch trang phục đang gom: rig width đã sửa; đai dùng convex support envelope để bắc qua khe áo/vạt, không lõm theo skin. 21342tris/1158188byte, Unity import/pose/build/quick qua; Player video8s `arrival-sash-envelope/sash-envelope.mp4`, đã xem idle/chạy. Chưa final art; next băng tay hở mặt sau khi gập khuỷu theo turnaround. Worktree còn dirty sau f727735.

Sau checkpoint `f727735`: sửa recipe scale rig nối xương bằng snapshot, tránh nhân tỷ lệ lặp gây tay ngắn; model/prefab đang dirty để gom cùng trang phục. Red thật ở `rig-width-red.log`; Blender, Unity pose và quick qua; video8s `arrival-rig-width/rig-width-fixed.mp4` đã xem khung idle/chạy. Không tăng texture/tam giác; next vạt/đai theo turnaround, chưa main integration/final art.

Checkpoint motion đã có quick pass, ảnh preview ba profile và main screenshot review. Main lượt đầu fail menu input; lượt telemetry hoàn tất flow, `arrival-motion-main-diagnostic.log` exit0, không thay assertion. Nguyên nhân fail trước chưa xác định, giữ log/trace. Next: biến dạng vạt/tỷ lệ nhân vật và dáng chạy theo turnaround trước main integration; chưa final art/visual PASS.

Candidate nhập môn có idle/walk/jog trong preview opt-in, prefab Resources góp payload build. Lỗi giơ tay đã có regression red/green, dùng T-pose chuẩn hóa Unity thay cho transform FBX thô. Route ba profile `arrival-canonical-*` exit0, ảnh mobile đã xem. Hai video 8s trước/sau tại `build/visual-evidence/motion-comparison/`; bản trước tái hiện lỗi, asset/source đã trả về bản chuẩn (`arrival-video-restored-canonical.log` exit0). Chưa final art/visual PASS/main integration; tiếp review PC/tablet và quick/main regression trước checkpoint.

Checkpoint mới nhất camera/pulse: `held-clock-{desktop,tablet,mobile}.log` cả ba exit 0 và quick `camera-pulse-checkpoint-quick.log` qua, không đổi movement/deadline; đã xem desktop arrival/tablet camera-exit. Main pulse ba profile đã được review, không đổi main từ evidence đó. Cold start 3 red -> 3 green; lỗi held travel 2.4m cũ chưa xác định nguyên nhân, không gộp thành lỗi môi trường. Tiếp model/rig nhập môn theo turnaround có sẵn, không mở hệ thống ngoài roadmap. Chưa art cuối/visual PASS/CPU benchmark. Các đoạn dưới ghi diễn biến trước checkpoint.

Camera cold start: 3/3 red tại 2 FPS -> 3/3 green bằng cập nhật standby mỗi frame, ảnh đúng trục phố đã xem; quick qua. Chưa full-route pass: giữ input thiếu quãng đường ở lượt PC, lần chẩn đoán sau bị foreground cản và process đã kết thúc. Không gộp lỗi chưa đủ dữ liệu thành môi trường, không commit/visual PASS; tiếp phân biệt timing/focus/input trước chốt batch camera + pulse.

Main gần đá không còn mảng cyan đặc: dùng ground glow chung thay cube, ba profile/quick/pose validator qua, ảnh PC/mobile đã xem. Fixture inactive đã sửa; trước đó không phải red đáng tin cho loại component. Camera arrival vẫn chưa tái hiện bằng probe, chưa thay cấu hình; tiếp khởi tạo/focus/warmup. Source đang gom, chưa commit/visual PASS.

Guidance nhập môn có view chung main/blockout; chữ mobile dễ đọc hơn, mục tiêu theo tiến trình và ẩn khi thoại. Ba profile preview, main regression, quick/density validator qua; đã xem ảnh. Còn lỗi camera arrival chọn góc bên và mảng cyan sau nhân vật ở main gần đá, tiếp truy nguyên bằng evidence; chưa visual PASS/model phù hợp hoặc cuộn gesture.

Đá Luyện riêng blockout đã chuyển từ sprite sang mesh nhẹ theo demo thể tích; giữ collider/feedback, ánh sáng môi trường Flat giúp vùng bóng dễ đọc. Ba profile `stone-volume-final-*` và quick qua, ảnh đã xem; camera bốn mép ngõ không tái hiện che nhân vật. Tiếp model/rig có license theo turnaround nhập môn; chưa main integration/visual PASS hoặc art cuối.

Camera blockout đã chuyển sang Cinemachine tránh che khuất và giữ hướng input qua đổi góc; ba profile bản cuối qua, ảnh ngõ PC/ra phố tablet đã xem. Hồi quy main hoàn tất capture ba profile, quick PASS. Demo trang phục nhập môn trước/bên/sau có trong reference-ui, chưa model; tiếp kiểm tra mép ngõ/góc nhà, không mở main camera hoặc hệ thống ngoài roadmap. Không visual PASS.

Blockout có focus/feedback tại đá, pulse kết thúc và nhãn xác nhận; `blockout-stone-label-*` ba profile cùng quick qua, ảnh đã xem. Reuse glow và label base, không texture mới; source giữ để gom presentation batch tiếp. Kế tiếp cần design nhân vật nhìn từ sau/góc camera phố trước khi thay capsule, không coi sprite mặt trước là giải pháp third-person final.

NPC blockout đã nối vòng thoại -> đá bằng session/view chung với màn chính, không clone modal. EditMode 3/3, main ba profile và quick qua; blockout ba profile `npc-font-*` có kiểm tra tầm/hủy/mở lại/input/bounds và ảnh đã xem. Đây vẫn là development prototype local; bước tiếp là focus/feedback đá theo storyboard, chưa city final, quest persistence hoặc visual PASS.

Blockout development nhập môn đã có tuyến đi bằng CharacterController và chín ảnh ba profile; không account/save hoặc scene main mới. Shared material URP Lit sửa fallback sprite do thiếu shader reference trong Player; main flow ba profile và quick qua. `docs/design/LINH-THANH-ONBOARDING-DESIGN.md` ghi evidence/cách mở/non-claims; bước tiếp là reuse hội thoại hiện có cho cảnh mới, không nhân đôi UI/state.

Demo một cảnh nhập môn đã có tại `docs/reference-ui/lgo-linh-mon-arrival-composition-draft-v1.jpg` (404279 byte); design hiện có bổ sung mặt bằng, camera và điểm tiếp cận từ bên. DRAFT, chưa blockout hoặc thay scene chính; mục đích nối prototype về kịch bản Linh Thành, không mở shop/social/event từ tranh.

2026-09-07 world presentation: sprite actor/đạo cụ đứng trên sân phẳng qua helper chung; nhãn/prompt theo bounds chiếu camera, không còn ghi đè vị trí NPC trong label refresh. Runtime ba profile `build/dev-loop/world-standing-props-final.log`, quick `world-standing-props-quick.log` hoàn tất, ảnh đã xem. Không thêm texture; vẫn prototype 2.5D, chưa city/visual PASS. Thiết kế NPC/Lộ/skill/đồ vẫn theo `docs/design/LGO-GAME-SYSTEMS-NARRATIVE-DESIGN.md`; bước tiếp cần mặt bằng/camera và điểm đứng nhập môn từ storyboard trước thay cảnh lớn.

2026-09-07 nhập môn: chỉ đường đá và tên nút Gặp/Luyện khớp HUD; nút ngữ cảnh giữ mục đích khi ngoài tầm, không tự chuyển sang đánh. `build/dev-loop/guided-action-purpose.log` hoàn tất ba profile; ảnh mobile ngoài/gần Đá Luyện đã xem. Nút Chém riêng không đổi; chưa claim visual PASS hoặc city theo demo hoàn chỉnh.

2026-09-07 cập nhật hiện tại: thiết kế hệ thống trong `docs/design/LGO-GAME-SYSTEMS-NARRATIVE-DESIGN.md` đã nối NPC/Lộ/skill/trang bị/trang phục thành các chặng có điều kiện mở, không thay GDD hoặc mở production. Save rejection/retry hoàn tất ba profile (`build/dev-loop/menu-save-rejection-retry.log`), đã xem ảnh mobile menu sau re-entry; chưa kiểm tra mất mạng/timeout hoặc claim visual PASS. Task tiếp theo ở đầu NEXT-ACTION; các đoạn dưới là lịch sử batch, không phải toàn bộ lỗi còn tồn tại.

Dialogue sizing mới nhất: `build/dev-loop/dialogue-natural-body.log` hoàn tất ba profile; ảnh PC/tablet ngắn và mobile dài đã xem. Overlay Center + height Auto + bỏ sàn vùng đọc thay cấu hình kéo đầy trần. Chưa visual PASS: portrait toàn thân thu nhỏ, body lặp tên NPC và scroller cyan còn cần xử lý theo demo.

2026-09-07: dialogue chỉ còn speaker header qua shell chung; `build/dev-loop/dialogue-single-speaker.log` hoàn tất ba profile, ảnh PC ngắn/mobile dài đã xem, không visual PASS. Root cause tiếp theo: dialogue ép height bằng maxHeight nên thoại ngắn vẫn quá cao. Design nhập môn/hệ thống mới là DRAFT, không mở production hoặc thay canon.

Demo tiếp theo: placeholder thật và draft tên trống khi Tạo thêm; khung hoa văn chung bằng vector cho shell/menu, preview nền đậm hơn. Runtime red/green tên, PC/tablet khung và mobile retry 23 checkpoint; ảnh đã xem, quick gate pass (`build/dev-loop/shared-shell-reading-contrast.log`). Có một lượt mobile mất focus được giữ nguyên evidence thất bại; chưa visual PASS hoặc đủ slot/gender theo demo.

Typography/skin: font heading/body ~193 KiB source qua `unityFontDefinition`, avatar trống 128x128 ~14 KB, nền Linh Thành riêng ~422 KiB; Login/preview/HUD dùng base neutral/gold. Shared column bỏ margin gây cắt nút; shared overlay căn giữa chiều cao thực, Character Hall cùng root viewport và max-height 85%. Bounds red/green, 23 checkpoint/profile (`build/dev-loop/night-city-budgeted-theme.log`), ảnh đã xem, quick/focused pass. Chưa visual PASS hoặc giống demo hoàn chỉnh: còn hoa văn, selection ô trống và giới tính. Nền cũ còn fallback Resources; không claim giảm tổng build.

Batch 2026-09-07: chuyển Character Hall sang ba slot và form/footer trong cột phải, bỏ dock cũ, hai nút cùng base/tier; API cap 3 với test reload. Demo `docs/reference-ui/lgo-character-three-slots-draft-v1.jpg` là hướng visual mới owner duyệt; skin bắt đầu chuyển neutral/gold. Capture 23 checkpoint/profile (`build/dev-loop/three-slots-neutral-gold.log`), ảnh đã xem, quick/focused gate pass. Chưa visual PASS: thiếu thumbnail, giới tính, font serif và nền mới; các màn cũ chưa chuyển hết. Chém dùng combat local/cooldown hiện có, không phải combat server-authoritative.

Batch 2026-09-06: roster dài có scroll chọn hàng/resize; movement và skill dùng chung base trên ba profile, Menu ngoài cụm combat, preview Hộ Linh được đưa vào cụm. Runtime 23 checkpoint/profile và smoke movement pass; quick gate pass, ảnh đã xem. Reference chính là concept board gốc và `linh-gioi-world-event-ui.png`, không phải demo sân luyện mới. Tablet còn panel che NPC; icon/presentation chưa đạt reference. Không claim visual PASS hoặc production auth/DB. Năm hướng Võ/Kiếm/Pháp/Cơ/Linh vẫn là đích thiết kế; phạm vi triển khai theo GDD/roadmap.

Roster selection đã sửa: tạo mới/refresh giữ đúng ID, hàng chọn có viền vàng qua base chung, preview đồng bộ khi đổi hàng. Runtime ba profile và quick gate pass, screenshot đã xem (`build/dev-loop/roster-selection-final.log`). Next: danh sách dài phải cuộn có giới hạn và giữ lựa chọn trong viewport của roster; không claim visual PASS.

Batch danh xưng: validation tại form theo rule server, lỗi không đẩy footer ra ngoài; 15/15 tests, quick gate và capture ba profile hoàn tất, ảnh lỗi đã xem (`build/dev-loop/name-feedback-final.log`). Tiếp theo: giữ đúng nhân vật vừa tạo và phân biệt selection trong roster qua base chung. Chưa claim visual PASS hoặc mọi viewport.

Gameplay/UI batch: form Tạo thêm có Hủy giữ nhân vật; cùng base nút đổi vai trò chính/phụ. Mobile action chuyển Đánh/Gặp/Luyện theo mục tiêu gần, mở hội thoại và hoàn thành Đá Luyện qua handler runtime. Evidence `build/visual-evidence/mobile-guided-actions/` đã xem, quick gate pass. Chưa test thiết bị cảm ứng thật hoặc claim visual PASS.

Form Tạo thêm và tên dài: anchor/visibility ổn định, input không co cắt; tên 16 ký tự rộng wrap trong base preview. Player resize thật và khôi phục pass trên ba profile (1382x972, 984x922, 692x486); ảnh probe/long-name đã xem, quick gate pass. Tiếp theo hoàn thiện hủy/quay lại form; chưa claim toàn bộ kích thước hoặc visual PASS.

Character Hall central stage: dùng lại ảnh V3B ở giữa bố cục ba cột chung; sửa min-width nút roster, ràng buộc dock theo parent và max-height shell PC/tablet. Đã xem ảnh selected PC/tablet/mobile và empty PC; quick gate pass. Còn cần evidence resize liên tục, tên dài và form Tạo thêm; không claim visual PASS.

Batch 2026-09-06: pad mobile nhận pointer và nối movement hiện có; 5/5 UI tests có graphics pass. Player smoke xác minh di chuyển 1,441 đơn vị rồi dừng khi thả/mở menu; log `build/visual-evidence/touch-movement/player-unity.log`. Quick gate pass; đã xem world-hub. Chưa test cảm ứng trên thiết bị thật, chưa claim giống demo 100%.

Latest player-visible batch: `LGO_CHARACTER_HALL_SELECTED_CTA_RATIO_READY` gives selected Character Hall actions a shared responsive ratio so the primary enter-world CTA owns the row and the secondary create action no longer looks equally dominant. Fresh desktop/tablet/mobile Character Hall screenshots were reviewed without claiming `VISUAL_RUNTIME_PASS`.

Current visual/runtime evidence harness command:

```bash
./tools/lgo_visual_runtime_review.sh
```

This harness must classify failures honestly as `FIX_REQUIRED`, `VISUAL_CAPTURE_TIMEOUT`, `VIDEO_CAPTURE_BLOCKED_ENV`, or `RUNTIME_BLOCKED_ENV`; it must not claim `VISUAL_RUNTIME_PASS` from build/capture alone.

Current Codex autopilot supervisor command:

```bash
./tools/lgo_codex_autopilot.sh
```

Autopilot status is written to `build/codex-autopilot/status.json`. If a valid next task remains in `docs/execution/NEXT-ACTION.md`, the status must be `CONTINUE`, not `DONE`.

## Current milestone

`M6 combat foundation v0.55.0`

## Current decision

`M6_COMBAT_FOUNDATION_CLOSED_LOCAL_v0.55.0`

M0 final decision: `M0_RUNTIME_CLOSED`.

M1 final decision: `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` (`M1 Offline Combat Prototype` closed).

M2 has source-level implementation for the first online session scaffold. Runtime closure is still pending because this sandbox does not run Unity Editor and Java 25/Maven runtime together for M2 evidence.

## Authoritative source baseline

`linh-gioi-governance-roadmap-queue-v1.0`

Baseline ancestry:

- accepted M0 runtime closure;
- accepted M1 offline combat runtime closure;
- M3 server/API persistence runtime closure;
- M3-B Unity account/character source integration;
- M4-0 playable vertical slice foundation;
- M4-1 visual placeholder foundation;
- M4-2 playable UI redesign;
- M4-3 placeholder art quality pass;
- M4 closure automation and stabilization validation;
- M4 visible UI usability and manual review harness;
- M5 first playable loop foundation with local-only interaction feedback;
- M5 visual evidence UX review path.
- accepted visual reference pack v0.16.5.
- M5 guided training loop source hardening.
- M6 skill preview sandbox and target dummy readability closed locally.
- M6 combat readiness spec closed as docs-only at `M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0`.
- M6 combat foundation v0.55.0 closed locally through local prototype, runtime closure, server-authoritative pilot, Unity-to-Java E2E, UX/readability polish, visual evidence, and GameData adversarial validation.
- Code governance baseline v1.0 adds maintainability, ownership, duplication, validator, and handoff rules.

## Current source successor

`linh-gioi-governance-roadmap-queue-v1.0`

This source includes:

- existing M2 online session runtime candidate tooling;
- closed M3 server/API persistence;
- M3-B Unity account/character client integration source;
- M4 source status `M4_PLAYABLE_VERTICAL_SLICE_FOUNDATION_SOURCE_READY`;
- M4 visual status `M4_VISUAL_PLACEHOLDER_FOUNDATION_SOURCE_READY`;
- M4 UI/art quality status `M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY`;
- M4-2/M4-3 Playable UI And Art Quality Pass v0.12.0;
- M4 stabilization status `M4_PLAYABLE_SLICE_STABILIZATION_SOURCE_READY`;
- M4 visible UI status `M4_VISIBLE_UI_USABILITY_SOURCE_READY`;
- M4 playable UI shell, runtime art catalog, and in-world HUD shell;
- upgraded original placeholder SVGs under `client/Unity/Assets/Game/Art/**`;
- M4 source validators, runtime smoke command, closure automation, handoff, report, and manifest.
- M4 visible UI review harness and usability validator.
- M5 first playable loop source status `M5_FIRST_PLAYABLE_LOOP_SOURCE_READY`.
- M5 local-only interaction loop with Gate Keeper, Training Stone, non-combat Shadow Slime marker, proximity prompt, F/Space acknowledgement, and existing save/back flow preserved.
- M5 visual evidence source status `M5_VISUAL_EVIDENCE_UX_REVIEW_READY`.
- Unity-side visual evidence runner for Gate Entry, Character Hall, World HUD, and First Playable Loop Feedback.
- Visual reference pack status `LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5`.
- M5 guided training loop source status `M5_GUIDED_TRAINING_LOOP_SOURCE_READY`.
- Guided local sequence: talk to Gate Keeper, stabilize Training Stone, preserve Save Position and Back to Lobby.

## Closed M0 gates

- Source validation: PASS.
- Protocol codegen and tests: PASS.
- GameData tests and compiled manifest: PASS.
- Java 25 runtime: PASS.
- Maven 3.9.16 runtime: PASS.
- Server build/test: PASS, 25 executed / 0 skipped.
- Spring Boot `/health`: PASS.
- Netty TCP bind: PASS.
- Real TCP `ClientHello -> ServerHello`: PASS.
- Unsupported protocol / malformed payload survival: PASS.
- Unity Editor evidence: PASS, 6000.3.2f1.
- Unity project import/generate: PASS.
- Unity EditMode tests: PASS.
- Unity Linux player build: PASS.
- Unity Player -> Java Netty handshake: PASS.
- Graceful shutdown / no orphan server process: PASS.
- Frozen contract audit: PASS.

## Closed M1 gates

- M1 source implementation: PASS.
- M1 GameData-driven catalog mapping: PASS.
- M1 stricter catalog validation source: PASS.
- M1 invalid request rejection source: PASS.
- M1 deterministic combat test source: PASS.
- M1 HUD prototype source: PASS.
- M1 default camera/light scene generation: PASS.
- M1 offline smoke command: PASS.
- M1 runtime evidence scripts: PASS.
- Unity `6000.3.2f1` M1 EditMode runtime: PASS.
- Unity M1 player build: PASS.
- Sandbox M1 offline combat Linux player replay: PASS.

## Current M2 source gates

- Existing protocol consumed without mutation: PASS source audit.
- Java online session source: PASS static validation.
- Java online session unit/integration tests added: PASS static validation.
- Unity online session client source: PASS static validation.
- Unity online smoke command source: PASS static validation.
- M2 runtime evidence tooling/docs: PASS static validation.

## M2 runtime gates still pending

- Java 25/Maven server build and tests with new M2 session classes: pending runtime verification.
- Server `online-session-smoke.py` against live Java Netty: pending runtime verification.
- Unity EditMode tests with M2 client serialization: pending runtime verification.
- Unity-built Linux player `--lgo-m2-online-session-smoke` against live Java Netty: pending runtime verification.
- Reconnect/failure path in runtime smoke evidence: pending runtime verification.

## Next allowed step

Run targeted playable closure verification on the current source. M2 runtime closure remains pending local Unity evidence and must not be inferred from M4/M5 work.

## Forbidden next step

Do not start full M5 social/guild, payment, marketplace, PvP ranking, production auth, production DB infra, progression/economy expansion, inventory expansion, combat expansion, or broad content expansion. Do not mutate `protocol/**`, `gamedata/schemas/**`, ADR, or design tokens without an S0 contract-change task.

## M5 First Playable Loop Foundation v0.15.0

Current M5 source status: `M5_FIRST_PLAYABLE_LOOP_SOURCE_READY`.

This owner-approved foundation adds only a lightweight local interaction loop to the existing M4 playable shell:

- enter world through the existing account/character flow;
- see player, Gate Keeper, Training Stone, and non-combat Shadow Slime markers;
- approach an interactable and press F or Space;
- receive concise objective/interaction feedback;
- preserve Save Position and Back to Lobby.

It does not claim full M5 social scope, full combat, economy, guild, chat, market, party, live ops, production auth, DB persistence, final production art, protocol changes, or GameData schema changes.

## M5 Visual Evidence UX Acceptance v0.16.0

Current source status: `M5_VISUAL_EVIDENCE_UX_REVIEW_READY`.

This review path adds Unity-side screenshot capture and deterministic metadata for:

- Gate Entry;
- Character Hall;
- World HUD;
- First Playable Loop Feedback.

It writes `build/visual-evidence/visual-evidence-summary.json` and `build/visual-evidence/visual-evidence-summary.txt`. Visual evidence remains review-ready and does not by itself claim explicit human visual acceptance.


## M3 owner override note

`OWNER_OVERRIDE_FROM_M2_RUNTIME_CANDIDATE` is recorded because M3 source work was explicitly opened by the project owner before `M2_ONLINE_SESSION_RUNTIME_CLOSED`. This does not claim the pending M2 Unity local evidence gates as PASS.

## Current M3 source gates

- Dev account login API source: PASS static/source validation.
- Dev key SHA-256 persistence hygiene: PASS static/source validation.
- Character create/list/load API source: PASS static/source validation.
- Character position save/load API source: PASS static/source validation.
- Local JSON persistence schema v1: PASS static/source validation.
- Unsupported future schema guard: PASS static/source validation.
- API persistence smoke tooling: PASS static/source validation.

## M3 runtime gates pending until execution

- Java 25 + Maven server build/test on M3 source.
- Spring Boot API runtime with `LG_API_PERSISTENCE_DIR`.
- Dev login/create/load/save HTTP smoke.
- API restart reload smoke.
- No raw dev key persisted.
- No orphan API process after smoke.

## M2 v0.6.2 hardening note

`M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY` supersedes v0.6.1 while keeping M2 locked to the same protocol and scope. v0.6.2 retains the one-command local runner and dependency-free GameData fallback, then adds server/client movement validation parity, multi-snapshot Unity smoke assertions, and no-false-ready local evidence classification.


## Previous M2 state marker for inherited M2 validators

`M2 Online Session Prototype` remains at `M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE`; M2 runtime evidence remains pending local Unity execution.


## Closed M3 server/API gates

- M3 final decision: `M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED`.
- Dev login API runtime smoke: PASS.
- Character create/list/load runtime smoke: PASS.
- Character position save/load runtime smoke: PASS.
- API restart/reload persistence smoke: PASS.
- Raw dev key persistence hygiene: PASS.

## Current M3-B source gates

- Unity `ClientRuntimeConfig` API endpoint configuration: PASS static/source validation.
- Unity `LinhGioi.Account` asmdef and dependency graph: PASS static/source validation.
- Unity `AccountApiClient` real HTTP implementation with `UnityWebRequest`: PASS static/source validation.
- Unity login/list/create/load/save-position model surface: PASS static/source validation.
- Unity command-line smoke path `--lgo-m3b-account-character-smoke`: PASS static/source validation.
- Restart-aware smoke path `--lgo-m3b-expect-existing`: PASS static/source validation.

## M3-B runtime gates pending until current Unity player execution

- Current Unity Linux player built from M3-B source.
- Unity player first-pass API smoke.
- API restart.
- Unity player restart-pass API smoke with persisted character reuse.
- Raw `m3b-unity-dev-key` absence in `players-v1.json`.

## M3-B owner override note

`OWNER_OVERRIDE_FROM_M3_SERVER_API_CLOSED` is recorded because the owner explicitly opened Unity client integration after the M3 server/API persistence closure while M2 Unity local evidence remains separately pending. This does not claim `M2_ONLINE_SESSION_RUNTIME_CLOSED`.


Historical milestone reference: `M3 Account / Character Persistence` is closed at server/API persistence level before M3-B.
