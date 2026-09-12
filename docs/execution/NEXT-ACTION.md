## Quy tắc chung — shared UI skin/base Map01A — 2026-09-13

Khi tiếp tục login/chọn nhân vật/hành trang/rương đồ/HUD/dialog, phải bám bộ demo owner gửi nhưng redesign cho game 2D hiện tại và dùng base chung `CongDongLamArrivalHud.Skin.cs` cho glass panel, modal shell, CTA, tab, label và frame. Không tạo modal/tab/card/detail panel thứ hai nếu vai trò giống nhau. Chạy `python3.12 tools/validate_lgo_ui_shared_skin.py` cùng Unity EditMode trước checkpoint; validator này chặn local skin constants/helper cũ như `InventoryGlass`, `StyleFrame`, `InventoryLabel` quay lại. Checkpoint rule/base hiện tại đã có test overlay entry và validator shared-skin; bước tiếp theo sau commit là visual/player review sâu hơn cho các screen còn lại, không coi đây là nghiệm thu toàn bộ UI.

Entry/login hiện có capture nội bộ không dùng OS input tại `build/map01a-entry-form-runtime/entry-login.png`; HUD chơi đã bị ẩn sau modal và form tài khoản/mật khẩu đã được khóa bằng Unity EditMode. Next UI hợp lệ: character select/HUD/dialog và polish hành trang/rương đồ bằng cùng shared skin/base; vẫn phải capture Player thật trước khi bàn giao. Không mở production auth hoặc tạo hệ modal/tab/card/detail panel thứ hai.

## Bổ sung mục tiêu owner — 2026-09-13

Giữ nguyên mục tiêu wardrobe/pose còn tồn đọng; thứ tự hiện hành owner chốt: **hoàn thiện Map01A → màn đăng nhập/chọn nhân vật → hành trang và rương đồ/kho → các screen, nút chơi và hội thoại NPC liên quan**. Phân tích/thích nghi bộ demo vừa gửi cho game 2D, không mặc định đó là design chuẩn; tham khảo các game/hệ thống khác. Code xong tự kiểm Player trước, owner review chỉnh sửa sau. Không mở auth/frozen contract trái phép hoặc tạo nút/số liệu giả. Rương đồ là kho gửi/rút, không đánh đồng với rương nhiệm vụ đang có.

Đã lưu đủ 8 PNG gốc, tên rõ từng màn, hash và nhãn REFERENCE_ONLY tại `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/` (ảnh 05 trùng 02). Phân tích, nguồn tham khảo và ma trận các màn: `docs/design/LGO-2D-UI-OWNER-DEMO-ANALYSIS-v0.1.md`. Mục tiêu chưa hoàn thành; bổ sung này phải được đọc cùng goal cũ khi resume.

Owner gửi thêm 4 ảnh ưu tiên tại `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/preferred-v2/`, status `OWNER_PRIORITY_UIUX_REFERENCE`, hash đã lưu trong manifest. Chỉ đạo mới nhất: thiết kế trong ảnh vẫn chưa hoàn thiện, cần phân tích/redesign cho hợp game 2D hiện tại, **không follow 100%**. Batch hành trang đã chuyển sang modal hai tab cấp chính: `Hành trang` là grid/túi + chi tiết món, `Thông tin` là nhân vật + 10 slot, cả hai dùng chung panel chi tiết bên phải; không sinh actor/base/item/chỉ số giả và không đổi wardrobe source/camera/scale.

## Audit chống nhầm validator UI cũ — 2026-09-13

`tools/report_lgo_legacy_ui_validator_refs.py` phân loại 73 validator `validate_lgo_*` còn trỏ `M4PlayableClientController.cs` là `LEGACY_STALE_NOT_CURRENT_GATE` vì controller này đã bị gỡ khỏi branch hiện tại. Không dùng các validator M4/V3B đó làm bằng chứng cho UI 2D mới và không phục hồi hệ cũ chỉ để làm xanh chúng. Chi tiết: `docs/design/LGO-2D-UI-LEGACY-VALIDATOR-AUDIT-v0.1.md`.

## Quick Resume

`CONTINUE`. Owner chuyển ưu tiên sang hoàn thiện Map01A sau checkpoint an toàn; dừng mở rộng/redraw character. Worktree `/private/tmp/lgo-vo-pose-div4-clean`, upstream `origin/feature/2d`. Giữ nguyên Võ div4/body/motion/camera/scale, registered WIP và art hiện có. Không Meshy/3D/frozen surfaces.

## Next task

Batch chức năng hiện tại: hành trang 2D theo demo `docs/design/demos/map01a-inventory-2d-layout.svg`; modal tách `Hành trang` và `Thông tin` theo góp ý owner, thêm `Rương đồ` dưới dạng gate an toàn. `Hành trang` mặc định: grid túi bên trái + panel chi tiết bên phải. `Thông tin`: nhân vật/10 slot bên trái + panel chi tiết bên phải. `Rương đồ`: chưa có model/API thật nên hiển thị thông báo rõ, không tạo item giả và khóa gửi/rút. Chọn item ở grid hoặc slot trang bị đều cập nhật cùng panel chi tiết. Giữ catalog/actor/art; không đụng base/camera/scale.

Checkpoint hành trang/storage tabs có evidence tại `build/map01a-detail-right-player/quest-capture/{pc,tablet,mobile}/07-q04-inventory-open.png` và đã được xem: modal Hành trang mặc định không bị minimap/action bar chồng, detail/grid đọc được, tab Rương đồ hiển thị ở header. EditMode `client/Unity/Logs/m0-editmode-results.xml` 265 total/264 pass/0 fail/1 ignored, gồm test tách tab Hành trang/Thông tin và test Rương đồ gate không đổi loadout. Build detail-right `errors=0 warnings=13`; capture Q01–Q09 đủ 18 frame/profile + 38 thoại + 6 NPC revisit. Không dùng click hệ điều hành/chuột thật cho evidence; nếu cần ảnh tab Thông tin/Rương đồ thì dùng runner/capture nội bộ.

Next sau checkpoint: mở character select/HUD/dialog theo ảnh ưu tiên v2 và API dev hiện có, giữ tách rõ `Đăng nhập`/`Bắt đầu`, không mở production auth giả; không dùng validator M4/V3B stale làm gate hoặc phục hồi hệ cũ. Rương đồ/kho đã có gate UI; chỉ bật gửi/rút thật khi có model/API hoặc task contract hợp lệ, không sửa frozen contract. Không quay lại character redraw/wardrobe class art trong batch UI này trừ khi owner đổi ưu tiên.

Hoàn thiện Map01A theo contract Q01–Q09 hiện có. Đổi class bằng `F` hoặc nút trong hành trang đã kiểm đủ Pháp/Võ/Kiếm/Cơ/Linh trên một actor. Pháp chỉ có nam Lv1 v7; Võ nam Lv1/Lv10; giới/cấp chưa có không được rơi về renderer cũ. Các pack vẫn REVIEW_ONLY, không suy diễn rằng toàn bộ design/pose đã nghiệm thu.

Player hiện hành: `build/legacy-3d-cleanup-player/LinhGioiOnline.app`; mở bằng `tools/launch_lgo_source_pose_review.py`, giữ catalog đủ năm class, không bật renderer cũ. Kế thừa HUD/hội thoại ở `b930acea`/`f7d5ba11`; giữ body/pose/camera/scale. Lỗi mái/đèn cắt sang ô khác đã sửa bằng raw source phục hồi, không imagegen lại.

Gate: 3 packer test + 20 EditMode map; build 0 error/0 warning. Evidence `build/map01a-restored-landmarks-player/quest-capture/{pc,tablet,mobile}` có Q01–Q09 và 38 ảnh thoại/profile; đã xem cả raw sheet và Player. Báo cáo `build/map01a-source-recovery-audit/repair-report.json`. Giới hạn: local playable slice, chưa nghiệm thu toàn bộ character/production hoặc thiết bị thật.

Batch dọn đã kiểm: gỡ 56 file presentation/metadata hết dùng, không có Unity C#/GUID consumer còn lại; no-3D guard chặn nhóm đã loại quay lại. 71 EditMode + 1 pointer test có graphics đạt; build 0 error/13 cảnh báo deprecated. PC capture Q01–Q09 + 38 ảnh thoại đã xem tại `build/legacy-3d-cleanup/pc/`; fixture chỉ Pháp, lệnh mở owner phải dùng launcher catalog năm class. Giữ M4/M6 gameplay còn dùng và toàn bộ art/registered WIP/camera/base/scale. Audit `build/legacy-3d-cleanup/audit.json`.

Next: tạo design/demo **mới cho game 2D hiện tại**, theo `docs/design/LGO-MAP01A-PLAYABLE-UI-v0.1.md`; không dùng nền login Linh Thành đêm/3D hoặc layout V3B cũ. Owner có thể cung cấp design khi cần. Chưa có design UI mới được duyệt. Sau demo nối vào entry 2D, dùng state/catalog và API dev sẵn có; hành trang header/footer cố định, body cuộn. Không mở lại redraw character.

## Yêu cầu tiếp nối của owner — 2026-09-12

Sau khi xử lý map, thiết kế và triển khai màn đăng nhập, hành trang, các nút cần thiết khi chơi theo design mới phù hợp game 2D hiện tại. Hoàn thiện hội thoại NPC như chơi thật: nội dung đầy đủ, lựa chọn/tiếp tục, nhận/trả nhiệm vụ, trạng thái trước/trong/sau nhiệm vụ và khi quay lại NPC. Trước implementation chỉ rõ design/demo từng màn và các tương tác; dùng base chung, kiểm input/UI trên Player, không chỉ test xanh. Không tự mở auth backend/frozen contract; audit flow đăng nhập hiện có để tái sử dụng.

## Current blocker

Không còn blocker source kiến trúc: đã phục hồi raw magenta sheet (không phải alpha cũ byte-identical) từ generated_images; raw và regions được lưu external tại `map-01a-cong-dong-lam/runtime-source-recovery/landmarks-v1`. Không dùng lại crop grid 512. Nguồn PNG gốc nằm ngoài Unity; runtime chỉ có atlas đã pack và provenance. Các màn đăng nhập/hành trang còn cần demo và implementation, không phải đã hoàn thành.

## Evidence và giới hạn

- Lỗi capture thiếu pack nữ đã sửa tận gốc: capture chỉ giới thực sự có, không tự bật Võ nữ legacy; báo cáo ghi `capturedGenders`. Pháp nam có 99 frame/16 tổ hợp tại `build/source-classes-stable-player/phap-male-verified/pc`, không còn exception. Đây là gate kỹ thuật, không phải nghiệm thu design.
- Ảnh và log đổi năm class: `build/source-classes-stable-player/actor-*.png`, `interactive.log`.
- Map Q01–Q09: `build/map01a-stable-player/quest-ui-final/{pc,tablet,mobile}` đủ 18 frame/profile, 9 nhiệm vụ, dùng bình/nhận thưởng/mở cổng. Các profile là mô phỏng tỷ lệ trên macOS, chưa phải thiết bị thật. Lượt `quest-ui-verified` đã sửa cuộn ngang và xem lại cả ba profile. Bản cuối ẩn nhãn POSE THỬ khi mở hành trang; `quest-ui-labels-pc` đã capture 18 ảnh và review, chữ item không bị nhãn debug đè.
- `--quest-only --pose-review-dir` tách gate map khỏi 60 frame wardrobe lịch sử; không phải bỏ assertion để claim wardrobe pass. Capture cũ `quest-capture/mobile` ghi nhầm scope nhưng chạy 78 frame đã bị helper từ chối, giữ để truy lỗi.
- Python pack/launcher/capture: 37 test; Unity `build/map-ui-final-tests.xml`: 26 test pass, gồm switch class/giới và nút bình máu Q04→Q05. Sửa UI chỉ giới hạn/định dạng lại hành trang hiện có.

## Lịch sử — không thay action hiện hành

Các action/PASS dưới đây được giữ để truy nguồn. Chỉ các mục Quick Resume, Next task và Current blocker ở trên định hướng batch hiện tại.

## Lịch sử — sửa Pháp trên đúng source-pose/base cũ sau visual reject — 2026-09-12

Checkpoint shared-rig v5 đã bị owner reject và hoàn nguyên bằng `eeeb1898`. Không dùng sheet 5×3 hoặc legacy `--lgo-phap-review` để mở Player; launcher review chỉ được dùng registered source-pose và có regression test chặn nhầm đường. Technical PASS cũ không còn giá trị visual.

Action: audit design/source Pháp canonical-v2 theo từng pose và từng slot ở full resolution; sửa trọn batch nam/nữ Lv1/Lv10 trên common canvas/pivot, bắt đầu từ jump để trả đúng chiều cao base thay vì scale `2/3`. Sau đó tạo board full, off từng slot, mixed và bốn nhịp chạy + jump; chỉ build/capture/mở Player khi silhouette liền, tháo đúng toàn món và tỷ lệ body giữ nguyên. Chưa mở Lv20/Lv30 hoặc class khác trước gate Pháp này.

## Lịch sử — owner kiểm Pháp canonical-v2 sau khi thu hồi semantic-v3 — 2026-09-12

Pháp semantic-v3 đã bị reject vì chia pixel theo anchor tạo layer chắp vá. Launcher hiện chỉ dùng candidate `canonical-v2`: surface tách trước semantic-v3 và `jump_tuck` được bake `2/3` quanh pivot cho body + đủ 10 layer. Evidence `build/phap-canonical-v2-runtime-v1/pc` có 190 frame, root scale 1, đúng bốn nhịp, đủ Lv1/Lv10/mixed/từng món tháo/32 tổ hợp và `errors=[]`; ảnh Player đã được agent xem nhưng chưa thay quyền review của owner.

Action: owner dùng `F` tới Pháp, `G` đổi giới, chọn từng slot rồi tháo/mặc/đổi cấp, `Shift` chạy và `W/↑` nhảy lộn. Nếu còn lệch, sửa source của toàn slot trên cả sáu pose rồi capture lại một lượt. Không dùng lại semantic-v3, không căn bằng camera/offset/runtime scale, không mở promotion/tier/class mới trước visual gate này.

## Lịch sử — Player kiểm Cơ Lv1/Lv10 và khóa một presentation — 2026-09-12

Player hiện hành: `build/source-pose-cross-class-player-v1/LinhGioiOnline.app`, build 0 error/0 warning từ source mới. Cơ nam/nữ Lv1/Lv10 dùng bốn source-pose pack, một actor, 10 slot, sáu pose và cùng body/canvas/pivot. Evidence `build/co-lv10-source-pose-review-v2/runtime-pc/pc` có 190 frame, 32 tổ hợp tháo/mặc, full `[1,10]`, mixed, 60 switch, `maxBodyVariants=1`, `errors=[]`; ảnh lớn đã review. Linh cũng đã tái capture đủ ma trận tại `build/linh-lv10-source-pose-review-v3/runtime-pc/pc`. Audit chéo 16 pack/160 item Kiếm–Pháp–Cơ–Linh ở `build/source-pose-cross-class-audit-v1.json` là PASS.

Code đã khóa presentation loại trừ nhau: nếu class-preview cũ được kích hoạt thì source-pose nam/nữ bị ẩn trong cùng refresh path; test hồi quy và full Unity EditMode đạt 250 pass/0 fail/1 ignored. Action hiện tại là để owner thao tác Player Cơ: `G` đổi giới, chọn slot trong hành trang rồi `Đổi cấp món`, `Shift` chạy, `W/↑` lộn. Giữ `REVIEW_ONLY`; không claim owner/production approval, không mở lại static-fit hoặc đổi camera/base/scale. Feedback hình phải sửa theo source surface/ownership cả batch.

## Lịch sử — Cơ Lv1 hai giới đã qua source board và Player evidence — 2026-09-12

Cơ nam/nữ Lv1 đã thay static-fit bằng source-pose 10 slot × sáu pose trên một actor. Evidence `build/co-source-pose-review-v1/runtime-pc/pc`: 186 frame, 20 item load, `maxBodyVariants=1`, `errors=[]`; đã xem bốn nhịp chạy, lộn và tháo cannon/áo/giày trên Player. Gate pack/capture/no-3D/no-source/frozen pass. Trạng thái `REVIEW_ONLY / AGENT_VISUAL_PASS`.

Action tiếp theo: author Cơ Lv10 nam/nữ theo progression gốc, giữ body/canvas/pivot byte-identical; review full Lv1/full Lv10 và mixed trên một Player. Không đổi camera/base/scale hoặc tạo hệ thứ hai.

## Lịch sử — Pháp Lv1/Lv10 hai giới và mixed đã có Player evidence — 2026-09-12

Bốn pack Pháp source-pose nam/nữ Lv1/Lv10 dùng đúng hai body authority hiện hành và một actor. Evidence `build/phap-lv10-source-pose-review-v1/runtime-pc/pc`: 190 frame, full `[1,10]`, 60 switch, mixed verified, `maxBodyVariants=1`, `errors=[]`; đã xem bốn nhịp chạy, lộn phối cấp và tháo pháp khí/áo trên Player. Pack `10/10`, capture `16/16`, no-3D/no-source/frozen audit pass. Đây là `REVIEW_ONLY / AGENT_VISUAL_PASS`; app được mở lại với cả hai cấp để owner thao tác.

Action tiếp theo: audit design Cơ và author trọn source-pose Lv1 nam/nữ, 10 slot × sáu pose, trên cùng body/canvas/pivot; review source full/toggle trước Player. Sau Cơ Lv1 mới làm Lv10/mixed. Không dùng static-fit đã thu hồi, không đổi camera/base/scale hoặc tạo hệ thứ hai.

## Lịch sử — Pháp Lv1 hai giới đã qua source board và Player evidence — 2026-09-12

Pháp nam/nữ Lv1 hiện dùng cùng source-pose contract với Võ/Linh/Kiếm: 10 slot × sáu pose, một actor, body authority bất biến. Evidence `build/phap-source-pose-review-v1/runtime-pc/pc` có 186 frame, 20 item load, `maxBodyVariants=1`, `errors=[]`; đã xem đứng, bốn nhịp chạy, lộn và tháo pháp khí/áo/giày trên Player thật. Pack `10/10`, capture `16/16`, no-3D/no-source/frozen audit pass. Đây là `REVIEW_ONLY / AGENT_VISUAL_PASS`; app vẫn mở cho owner.

Action tiếp theo: author Pháp Lv10 nam/nữ từ progression/grid gốc trên đúng source/canvas/pivot và body hash Lv1; review từng món hai chiều, full Lv1/full Lv10 và mixed trên một Player. Không đổi camera/base/scale, không tạo actor hay hệ wardrobe thứ hai. Sau gate Pháp Lv10 mới chuyển Cơ.

## Lịch sử — Kiếm Lv1/Lv10 hai giới và mixed đã có Player evidence — 2026-09-12

Kiếm nam/nữ Lv1/Lv10 hiện dùng bốn pack source-pose cùng body authority và một actor. Evidence `build/kiem-lv10-source-pose-review-v1/runtime-pc/pc`: 190 frame, full `[1,10]`, 60 switch, mixed verified, `maxBodyVariants=1`, `errors=[]`; đã xem bốn nhịp chạy, lộn phối cấp, tháo kiếm/áo ngoài của cả hai giới trên Player thật. Pack `10/10`, capture `16/16`, no-3D/no-source/frozen audit pass. Đây là `REVIEW_ONLY / AGENT_VISUAL_PASS`; Player vẫn mở để owner thao tác.

Action tiếp theo: audit design Pháp và thay static-fit đã thu hồi bằng một batch source-pose Lv1 đủ nam/nữ, 10 slot × sáu pose. Dùng đúng common body/canvas/pivot hiện hành, review full-compose và toggle board ở kích thước lớn trước một lượt Player; không đổi camera/base/scale, không tạo actor hoặc hệ wardrobe thứ hai. Sau Pháp Lv1 mới làm Lv10/mixed rồi chuyển Cơ.

## Lịch sử — Linh Lv1/Lv10 hai giới và mixed đã có Player evidence — 2026-09-12

Linh nam/nữ dùng cùng source-pose runtime, body authority cố định và 10 slot cho Lv1/Lv10. Evidence cuối `build/linh-lv10-source-pose-review-v2/runtime-pc-v2/pc` đạt 158 frame, full Lv1/Lv10 cho cả hai giới, 60 switch, mixed verified và `errors=[]`; ảnh đã review ở kích thước lớn. Nữ v1 hai-linh-cầu đã bị loại, chỉ v2 được pack. Đây là `REVIEW_ONLY / AGENT_VISUAL_PASS`, chưa phải owner/production approval.

Action tiếp theo: audit Kiếm theo turnaround/grid gốc và thay static-fit đã thu hồi bằng một batch source-pose 10 slot × 6 pose trên đúng body authority; review source board và toggle board trước Player. Không mở lại Võ, không đổi camera/base/scale, không chạm frozen surfaces; Map01A giữ nguyên target.

## Lịch sử — Linh Lv1 hai giới đã vào cùng source-pose runtime — 2026-09-12

Giữ Võ Lv1/Lv10 HD checkpoint đã chốt; không làm lại Võ và không dùng static-fit. Linh nam dùng `ten-slot-pose-authoring-v1/registered-surface-lv001-hd-v3`; Linh nữ dùng common body `common-female-v1/.../registered-body-v1` và 10 slot `linh-lv001/female-ten-slot-pose-authoring-v1/registered-surface-lv001-hd-v3`. Cả hai dùng một state/actor path, sáu pose, 10 slot; runtime chỉ render giới tính đang chọn. Player/evidence mới: `build/linh-female-source-pose-review-v2/` (154 frame PC, SourcePose 17/17, Python 10/10 + 15/15). Trạng thái vẫn `REVIEW_ONLY`, chưa hoàn thành goal.

Action tiếp theo: làm Linh Lv10 cho cả nam/nữ theo đúng hai body fit family hiện có, rồi kiểm full Lv1/full Lv10, tháo từng slot và phối chéo Lv1/Lv10 trên Player. Pixel ngoài ownership của món nâng cấp phải kế thừa nguyên byte; không sharpen, warp, offset, đổi camera/base/scale. Chỉ sau visual gate này mới thay lần lượt pack Kiếm/Pháp/Cơ đã bị thu hồi. Map01A tiếp tục là target.

## Lịch sử — Linh source-pose thay static-fit bị reject — 2026-09-12

Không dùng Player/pack `TwoDClassMixedLoadoutFitPreview` để tiếp tục căn Kiếm/Pháp/Cơ/Linh. Owner đã reject Linh static-fit vì ghép vỡ; kết luận hoàn tất cũ đã được thu hồi trong `PROJECT-STATE.md`.

Candidate hiện hành là Linh nam Lv1 external `class-work-in-progress/linh-lv001/ten-slot-pose-authoring-v1/registered-surface-lv001-hd-v3`: cùng body/motion Võ v3 div4, 10 slot × sáu pose, overlay div2, một actor. Player `build/linh-source-pose-review-v3/player/LinhGioiOnline.app`; evidence `build/linh-source-pose-review-v3/runtime-pc/pc` có 154 frame. Không đổi camera/base/scale.

Action tiếp theo: dựng Linh nữ Lv1 theo cùng contract, review source full/toggle trước một Player; sau đó Linh Lv10 và ma trận phối chéo Lv1/Lv10. Khi hai giới và phối chéo đạt visual gate mới áp lại pipeline này cho Kiếm/Pháp/Cơ. Trạng thái vẫn `CONTINUE`, chưa hoàn thành goal.

## Đã thu hồi — đủ Kiếm/Pháp/Cơ/Linh Lv1–30 static-fit — 2026-09-12

Giữ bốn Player/evidence class hiện hành và cùng `TwoDClassMixedLoadoutFitPreview`; không tạo controller, actor, camera hoặc hệ equip thứ hai. Linh cuối ở `client/Unity/build/linh-ten-slot-player-v1/LinhGioiOnline.app`, evidence `build/linh-lv1-30-ten-slot-runtime-v1/pc`, pack vẫn `DRAFT_RUNTIME_FIT`.

Audit ma trận chung đã PASS tại `build/linh-runtime-v1/cross-class-contract-audit.json`: cùng 10 slot canonical, skeleton, hai giới, cấp 1/10/20/30 và 104 component; không có contract mismatch. Gate tiếp theo là owner xem Player Linh cùng các Player Kiếm/Pháp/Cơ. Chỉ mở Lv40+ nếu review không phát hiện lệch/mờ; tier mới phải dùng cùng slot/component contract, không dựng full-outfit hoặc base riêng cho từng level.

## Lịch sử — Cơ đã đóng batch; chuyển source gate Linh — 2026-09-12

Giữ Player Cơ `client/Unity/build/co-ten-slot-player-v1/LinhGioiOnline.app` và evidence `build/co-lv1-30-ten-slot-runtime-v1/pc`; pack vẫn `DRAFT_RUNTIME_FIT`. Action kế tiếp là audit design Linh, tạo đủ nam/nữ 4×10 ở source trước rồi mới mở rộng cùng `TwoDClassMixedLoadoutFitPreview`. Không copy controller/camera/actor hoặc dùng crop board nhỏ. Sau Linh mới đánh giá lại ma trận ba class và thứ tự level cao hơn.

## Lịch sử — Pháp Lv1–30 đã đóng batch; tiếp theo source gate Cơ — 2026-09-12

Giữ Player Pháp `client/Unity/build/phap-ten-slot-player-v5/LinhGioiOnline.app` và evidence `build/phap-lv1-30-ten-slot-runtime-v5/pc`. Pack Pháp đủ 10 slot nam/nữ ở Lv1/10/20/30, tháo-mặc/phối cấp trên cùng actor và motion Võ đã khóa; vẫn `DRAFT_RUNTIME_FIT`. Không tạo lại controller Kiếm riêng hoặc hiển thị hai hệ song song.

Action tiếp theo: audit design Cơ tại external `classes-lv001-030/co`, xác nhận nguồn nam/nữ và bốn cấp trước khi tạo sheet 4×10. Dùng `pack_lgo_class_equipment_sheet.py` và `TwoDClassMixedLoadoutFitPreview` hiện có; mở rộng danh sách class/resource thay vì copy controller/capture. Nếu source Cơ thiếu progression hay item rõ, ghi source gate và chuyển audit Linh, không đưa crop board nhỏ/mờ vào Player. Sau mỗi class chỉ build/capture khi cả batch đã đủ. Map01A giữ nguyên target; không Meshy/3D, không frozen surfaces.

## Lịch sử — sau Kiếm Lv1–30, chuyển sang source gate Pháp — 2026-09-12

Checkpoint Kiếm hiện hành dùng Player `client/Unity/build/kiem-ten-slot-player-v5/LinhGioiOnline.app` và evidence `build/kiem-lv1-30-ten-slot-runtime-v1/pc`. Giữ module duy nhất `kiem-lv1-30-equipment-runtime-v1`, không khôi phục proof 4 slot hoặc recovered pack cũ. Art đang ở `DRAFT_RUNTIME_FIT`; feedback owner mới trên Player được ưu tiên sửa source/slot-box theo cả lô, không chỉnh camera/base/scale.

Action kế tiếp theo thứ tự owner: audit design/demo Pháp trong `LGO-Selected-2D-Source-v1/classes-lv001-030/phap`, chọn identity nam/nữ và xác nhận nguồn đủ 4 cấp × 10 slot trước khi import. Tái dùng shared rig, canonical slot mapping, hành trang và capture matrix Kiếm; không thêm actor/controller/equip flow thứ hai. Nếu source chứa turnaround hoặc nhiều góc nhìn trong một file, tách/chọn candidate tại source plan trước atlas. Chỉ build/capture sau khi đủ cả batch; sau Pháp mới tới Cơ rồi Linh. Map01A vẫn là target tổng thể; không Meshy/3D và không sửa frozen surfaces.

## Lịch sử — Võ Lv1/Lv10 HD đã qua agent visual audit — 2026-09-11

Dùng Player `client/Unity/build/vo-lv1-inventory-player-v6/LinhGioiOnline.app` với pack Lv1 `legacy-base-run-contact-jump-v3-div4-ten-slot-lv001-hd-review-v2` và alt Lv10 `legacy-base-run-contact-jump-v3-div4-ten-slot-lv010-hd-review-v2`. Hành trang hiển thị đủ 10 ô; chọn món để xem thông tin/tháo-mặc, bấm `Đổi cấp món` để đổi riêng Lv1/Lv10. Evidence cuối `build/vo-lv1-lv10-hd-inventory-runtime-v1/pc`: 156 frame, full `[1,10]`, mixed, bốn nhịp chạy, lộn, `errors=[]`.

Source Lv10 hiện hành là external `ten-slot-pose-authoring-v2/registered-surface-lv010-hd-v5`. Giữ quy tắc đã bắt được bằng audit: pixel ngoài vùng top-visible của slot phải kế thừa nguyên byte, không sharpen/filter lại underlayer; alpha/pivot/canvas dùng chung giữa các cấp. Không dùng v1-v4 hoặc bất kỳ pack nhiều level cũ bị reject. Sau review tương tác, bước roadmap kế tiếp là lấy design/demo Kiếm cụ thể rồi áp cùng pipeline theo một batch 10 ô × 6 pose; nếu mở thêm tier Võ thì cũng phải qua board từng món hai chiều và phối chéo trước Player. Không chỉnh camera/base/scale và không chạm frozen surfaces.

## Lịch sử — owner đang review Võ Lv1 HD trên Player — 2026-09-11

Owner đã reject toàn bộ atlas nhiều level trước vì mờ/lệch. Không dùng kết quả count/switch cũ làm visual PASS. Player hiện hành là `build/vo-lv1-hd-player-v1/LinhGioiOnline.app`, chỉ nạp pack Lv1 `legacy-base-run-contact-jump-v3-div4-ten-slot-lv001-hd-review-v2`: body/motion v3 div4 bất biến, overlay 10 slot div2, một actor. Source board và runtime board lần lượt ở external `ten-slot-pose-authoring-v2/registered-surface-lv001-hd-v2/` và `build/vo-lv1-hd-runtime-v1/lv1-hd-runtime-review.jpg`.

Action tiếp theo phụ thuộc đúng feedback hình ảnh đang diễn ra: sửa theo lô mọi lỗi source Lv1 được owner chỉ ra, không chỉnh camera/scale/offset runtime và không build từng item. Chỉ khi Lv1 đứng/chạy/lộn + 10 trạng thái tháo được owner chấp nhận mới áp cùng quy trình sang Lv10 và mặc chéo; chưa khôi phục pack Lv20–Lv100 cũ, chưa mở class khác. Map01A vẫn là target tổng thể.

## Lịch sử — full-level Võ đã chạy; gate còn lại là source-art polish — 2026-09-11

Đã author và pack đủ 10 slot cho 11 mốc Lv1–Lv100 trên body/action v3, rồi nạp tất cả vào một Player. Evidence `build/vo-pose-all-tier-runtime-v3/pc`: 165 frame, full levels `[1,10,20,30,40,50,60,70,80,90,100]`, 11 frame full-set riêng, mixed verified, 149 item switch, 11×22 fingerprint, `errors=[]`. Board `full-level-player-review.jpg` so trực tiếp mọi tier trên cùng idle/camera/body. Không đổi camera/base/scale, không thêm actor/controller theo level. Pose lộn tier cao đã bỏ silhouette transfer gây rối và giữ exact tuck surface.

Action tiếp theo: polish component source Lv40–Lv100 ở kích thước Player, ưu tiên độ đọc của `outer_top`, `shoulder_chest_guard`, `waist_belt` và `head_hair`; so trực tiếp với grid/progression canonical. Giữ vạt/giáp mở rộng trong đúng slot ownership để tháo riêng và phối chéo không kéo theo món khác. Chạy pairwise representative giữa tier thấp/trung/cao và trạng thái tháo từng slot trong một capture cuối. Chỉ sau visual gate này mới promotion khỏi `REVIEW_ONLY` và mở class tiếp theo. Map01A vẫn là target.

## Lịch sử — gate wardrobe Võ bốn tier và đường mở rộng mọi level — 2026-09-11

Đã nạp đồng thời Lv1/10/20/30 vào một actor POSE THỬ, đủ 10 slot mỗi tier. Runtime và capture lấy danh sách complete level động, kiểm full-set từng level và mixed loadout trên cùng body/action v3. Evidence `build/vo-pose-four-tier-runtime-v2/pc`: 154 frame, 40 toggle, 75 switch thực, full levels `[1,10,20,30]`, mixed verified, `errors=[]`, fingerprint `22×4`. Đã xem walk Lv20, run bốn nhịp Lv10, jump mixed và jump-diagonal Lv30; không có actor thứ hai hay thay camera/base/scale. Test Unity `11/11 + 17/17`, Python `18/18`.

Action tiếp theo: hoàn thiện progression Võ còn lại Lv40/50/60/70/80/90/100 theo cùng 10 slot và source-space geometry. Nguồn selected hiện chỉ có material canonical đến Lv30, nên trước khi pack phải lập một progression board đủ bảy tier còn lại bám turnaround/grid Võ, kiểm silhouette/ownership theo lô rồi mới register sáu pose. Khi có pack, chỉ thêm các `--pose-review-alt-dir`; code hiện tại tự phát hiện/verify mọi tier. Chạy một Player capture full-level + mixed đại diện, sửa item lỗi tại source mask/component. Chỉ sau gate Võ đầy đủ mới mở class khác. Map01A vẫn là target.

## Lịch sử — item variant Lv1/Lv10 chạy trực tiếp trên một actor — 2026-09-11

Đã nối hai tier vào `TwoDSourcePoseReview`: cùng slot có variant theo level, chung body hash/fit family/root pose; đổi toàn bộ tier hoặc riêng item bằng flow level/slot sẵn có. Không còn phải đổi cả review directory để xem từng loadout và không tạo renderer body thứ hai. Test đúng scope: SourcePose `11/11`, Map preview `17/17`, Python `17/17`. Player `build/vo-pose-item-variants-player-v3/LinhGioiOnline.app` build `171948586` byte, 0 error/0 warning.

Evidence `build/vo-pose-item-variants-runtime-v3/pc`: 154 frame, 40 toggle, `errors=[]`, full fingerprint Lv1+Lv10 `22+22`, đủ sáu pose. Capture đã chuyển đúng 20 item renderer: full Lv10 ở bốn frame chạy và mixed 5×Lv1 + 5×Lv10 ở jump; manifest xác nhận cả hai trạng thái. Đã xem frame 08/18 trên Map01A, một actor, không đổi camera/base/scale.

Action tiếp theo: author Lv20/Lv30 theo cùng surface geometry và thêm chúng làm variant, rồi capture ma trận pairwise theo slot/tier. Dùng `TwoDEquipmentCompatibilityCatalog` cho điều kiện level/body/skeleton ở đường production; pose reviewer chỉ trình bày item đã preflight. Item nào tạo đường cắt/hở hoặc đổi silhouette sai phải sửa source mask, không thêm offset runtime. Chưa mở class khác trước gate Võ nhiều level.

## Lịch sử — 10 slot Lv1, Lv10 và phối chéo cùng body/action — 2026-09-11

Đã đóng batch review đủ 10 slot Võ nam Lv1 trên source v3 div4 bất biến. Evidence `build/vo-lv1-ten-slot-surface-runtime-v3/pc` có 154 frame, 40 toggle, `errors=[]`, đủ sáu pose và fingerprint 22 file; đã xem một actor, idle, bốn nhịp chạy, lộn và tháo từng slot. External pack: `legacy-base-run-contact-jump-v3-div4-ten-slot-review-v1`. Không đổi camera/base/scale và không rollback registered WIP. Trạng thái art vẫn `REVIEW_ONLY`, chưa tự gán owner/production approval.

Đã áp cùng source geometry cho 10 item Lv10 và một loadout phối xen kẽ năm item Lv1/năm item Lv10. Evidence lần lượt `build/vo-lv10-ten-slot-surface-runtime-v2/pc` và `build/vo-mixed-lv1-lv10-surface-runtime-v2/pc`, mỗi bộ 154 frame/40 toggle/`errors=[]`, đủ 22 fingerprint và sáu pose. Tool mới `tools/compose_lgo_pose_review_loadout.py` resolve `slotId -> itemId`, preflight đủ 10 slot/body hash/fit family rồi mới copy nguyên atlas; test `3/3`. Đây là review-pack composer, không thay runtime catalog hiện có.

Action tiếp theo: dùng `TwoDEquipmentCompatibilityCatalog` làm resolver production duy nhất và nối item atlas theo pose vào loadout hiện có để đổi riêng từng slot ngay trong Player. Kiểm full Lv1, full Lv10, cặp phối chéo đại diện và tháo từng slot trong một batch. Sau đó tái dùng đúng body profile/geometry cho Lv20/Lv30; item sai silhouette phải quay lại source mask, không thêm offset/scale/controller theo level. Chưa mở class khác cho đến khi Võ qua ma trận này. Map01A giữ nguyên target.

## Lịch sử — sửa nguồn trang bị theo design gốc — 2026-09-11

Đã đóng presentation một actor cho pose review: v3 div4 + `outer_top` chạy chung một stack tại vị trí actor chính; registered WIP vẫn giữ state nhưng không render song song. Scoped EditMode `28/28`; Player `build/vo-single-stack-player-v1/LinhGioiOnline.app` build thành công `171941930` byte, 0 error/13 warning. Capture `build/vo-single-stack-runtime-v1/pc` có 154 frame, `errors=[]`; đã xem idle, bốn pha chạy, lộn và bật/tắt áo. Không đổi camera/base/scale/source motion.

Runtime hiện đã sẵn đường 10 slot/multi-component trong đúng stack này. Test cuối SourcePose `10/10`, equipment `7/7`, locomotion `12/12`. Player `build/vo-ten-slot-loader-player-v1/LinhGioiOnline.app` build `171943978` byte, 0 error/13 warning; capture `build/vo-ten-slot-loader-runtime-v1/pc` đạt 154 frame/40 toggle/`errors=[]` và visual review vẫn chỉ một actor. Python pack `10/10`, capture helper `13/13`, no-3D/no-source PASS.

Action hiện hành: hoàn thiện đủ 10 item/slot Võ nam Lv1 trên cùng body/action v3. Author theo lô từ design gốc, mỗi item có `itemId`, `fitFamily`, component theo pose/view, coverage/occlusion và source registration; review compose full-set + tháo từng slot trước một lượt Player. Sau khi cả 10 slot Lv1 đạt mới mở Lv10, triển khai resolver `slotId -> itemId` và kiểm phối chéo level; sau đó tái dùng đúng pipeline cho các level tiếp theo. Không tạo hệ nhân vật, skeleton, controller, scale hoặc offset riêng theo level.

Source batch external `ten-slot-pose-authoring-v1/raw-candidates` đã được audit và **chưa đủ điều kiện runtime**: nền caro bake ở hai sheet, pixel da ở tay/giày, study underclothes đổi anatomy. Bước kế tiếp là dùng mask/khớp đăng ký từng component lên đúng sáu pose v3, giữ pixel ngoài mask bất biến; ưu tiên đóng cùng lúc nhóm `lower_body + arm_guard + footwear + waist_belt`, rồi nhóm đầu/vũ khí/phụ kiện/giáp. Không build Player cho từng item; chỉ capture sau khi compose đủ 10 slot và các trạng thái tháo đã qua source QA.

Owner bác hai sheet tách generic và yêu cầu xem kỹ design gốc. Đã mở turnaround03, equipment05, progression11 và grid15: board sáu pose gần đúng hành động nhưng đổi áo vạt chéo thành vest mở giữa, đổi giày/hoa văn và tự thêm băng trán/giáp vai. Không dùng board này làm chuẩn đồ. External `registered-equipment-authoring-v1/original-design-reference-v1` giữ reference nguyên pixel, SHA và brief sửa; hai sheet lỗi được giữ riêng, không runtime. Turnaround03 là chuẩn ngoại hình đề xuất; grid15 chỉ định ownership vì áo Lv1 trong hai ảnh mâu thuẫn. Chưa tự gán owner approval.

Đã vẽ lại áo trực tiếp theo sáu visible surface của v3, dùng turnaround03 giữ vạt chéo/viền/vải ngà/biểu tượng. External `original-design-reference-v1/pose-guide/registered-surface-candidate/six-pose-source-compose.png` đã review: run_b giữ vùng tay che, tuck dùng lưng/sườn thay mặt trước. Không warp/scale từng pose; source v3 không đổi. Pack áo rời `div4-overlay-review`: div4, atlas128×512, PNG55895byte; 10test pack đạt, 6/6sprite round-trip đúng sampled source. Mới outer_top, REVIEW_ONLY; chưa Player/wardrobe PASS. Còn mép cổ/eo và ownership sát nút đai, mặt khuất/hành động khác. Study bốn áo trước đó chỉ tham chiếu, không dùng tuck mặt trước.

Bước tích hợp overlay và single-stack đã hoàn tất; không khôi phục presentation hai actor trong các capture tiếp theo.

Next batch: chuyển chín slot Lv1 còn lại theo cùng contract đã chứng minh với `outer_top`, rồi review cả bộ 10 slot và từng trạng thái tháo. Giữ body/action v3 div4 và tỷ lệ cũ; không quay lại warp đồ đứng hoặc sinh full-body mới. Giữ registered WIP. Chưa Lv10/class khác hoặc wardrobe PASS.

Nghiên cứu dài hạn trong `docs/art/LGO-2D-EQUIPMENT-COMPATIBILITY-CONTRACT-v1.md` vẫn áp dụng: body/action chung, item nhiều component, hình riêng khi đổi mặt, chuẩn đường nối và layering. Registered hiện chỉ bật/tắt slot và level chung, chưa resolve itemId khác cấp; sau Lv1 mới bổ sung resolver/preflight và kiểm mặc chéo Lv10. Recovery runtime giữ evidence `build/vo-native-run-recovery-v1/recovery-report.json`; lượt sửa brief/source không đổi C# hoặc chạy lại Unity. Map01A vẫn là target tổng thể.

## OWNER REJECT — motion mới sai bản đã chốt, dừng mở Lv10 — 2026-09-11

Batch khắc phục đang làm: bỏ riêng retarget nam v5/v7 và các assertion khóa dáng đã bị bác; lưu bản trước sửa tại `build/vo-native-run-recovery-v1/rejected-code`. Khôi phục gait registered nam từ sandbox cũ, giữ trang bị và sửa bốt nữ. Build một Player, kiểm đứng/chạy/lộn cùng nguồn whole-pose v3 div4 bất biến; registered là candidate, không được gọi là motion nguồn đã duyệt. Không sửa camera/base/scale.

Đường capture chính đã chặn việc dùng nhầm nguồn: `validate_owner_pose_source()` kiểm đúng fingerprint packv3 đã chốt trước khi resolve/launch Player. Kiểm trên file thật: v3được nhận, v5/v7bị từ chối; bản copy nguyên byte được nhận.10test helper đạt. Không đổi runtime/ảnh/đồ, không chạy lại Player hoặc mở thêm gate. Đây chỉ là bảo vệ nguồn review, chưa sửa retarget và không đủ để đóng motion.

Khắc phục bước đầu: đã chạy lại nguyên pack `legacy-base-run-contact-jump-v3-div4` bằng Player hiện có, không sửa6PNG, source rect, scale hoặc camera. Evidence `build/vo-owner-source-restored-v1/loop`:180frame, source hash trước/sau không đổi; đã xem `original-source-six-poses.png`. Clip `original-approved-source-player.webp` crop đúng actor whole-pose bên phải; `full-player-original-source.png` giữ ngữ cảnh hai actor. Registered bên trái vẫn là candidate retarget bị bác, chưa sửa hoặc coi là đạt. Sourcev3 là chuẩn cố định cho wardrobe theo pose; không dùngv5/v7 thay lại chuẩn này. Không commit/push batch bind tooling đang treo hoặc mởLv10.

Đã đọc đúng session `Polish NPC Người Giữ Cổng 2D` (01a0882c-6082-7b80-8ddd-3e7d7657a5e5), không suy luận từ handoff. Owner04:37UTC: “Tôi thấy khá mượt rồi”;04:48 yêu cầu thêm2contact và tăng lộn chéo;05:04: “Ok, tạm như này đi… vấn đề thực sự là khi thay đồ”. Bản chốt là whole-pose review trong `/private/tmp/lgo-vo-lv1-30-FaSFxE`, sau đó chỉ sửa blur lên `legacy-base-run-contact-jump-v3-div4`. Phải giữ source motion này và làm wardrobe khớp theo pose.

Sai lệch đã xác minh: clean branch thay cả4source chạy từv3 sangv5/v7 (hash khác), thêm authored run retarget trong `TwoDAuthoredVoRun`/`ApplyAuthoredMaleRun`. Clip bị owner phản đối `build/vo-four-beat-review-v1/loop/four-beat-player.webp` crop actor registered bên trái, không phải whole-pose review đã chốt bên phải. Đếm4key và fit ở bind không chứng minh giữ motion đã được owner chấp nhận. Thu hồi kết luận motion đủ ổn/mởLv10 ở các mục dưới. Không dùngv7 làm chuẩn mới.

Bản cũ/source/evidence còn nguyên, chưa rollback runtime hoặc registered wardrobe. Batch bind-test/tool/docs hiện chưa commit; tạm giữ để không gây thêm churn. Next chỉ khôi phục đúng chuẩn reviewv3div4 và flow của phiên cũ, đối chiếu source đã chốt; chỉnh đồ theo pose cố định, không redraw/IK sửa ngược pose đã chốt, không camera/base mới. Wardrobe hợp lệ giữ nguyên, không revert cảcommit hoặc mang close-review/camera tangent. Chưa Lv10/class khác, chưa được claim motionPASS từ count/test.

## Võ Lv1: đóng gate bind của pack registered — 2026-09-11

Đã nối `review_lgo_paper_doll_pack.py --registered-bind-dump` với geometry Unity thực: vertices sau skin/attachment, UV, indices, sorting và atlas của27layer nam/26layer nữ. Reference ghép ảnh nguồn canonical theo rect sở hữu; candidate raster từ mesh Unity và atlas, không normalize bbox và không dùng cùng ảnh làm hai phía. Dùng nguyên ngưỡng IoU≥0,90/area0,90–1,10/MAE≤22. Final `build/vo-registered-bind-fit-final/{male,female}-review`: nam IoU1,00000/area1,00000/MAE0,016; nữ0,99898/0,99898/0,129; registration sạch. Đã mở hai ảnh source–runtime: không thấy tách khớp/scale hoặc lệch đồ ở bind. Đây là fit projection/material; sorting/motion dựa trên Player/matrix đã review ở batch trước, không tự tạo owner approval.

Unity6/6 equipment test gồm2test mới kiểm mọi vertex khớp sourceUV dưới0,02sourcepx; Python17/17, cả direct entrypoint. Test bắt lệch vị trí, mất tam giác, đổi source/atlas/runtime code và thiếu fingerprint. Dump khóa SHA atlas/manifest, mọi TwoDRegistered*.cs, authored run, exporter, rig/anatomy và packages-lock. Review phát hiện dump cũ chưa khóa code + main guard test đặt sớm; đã sửa cả hai. Lượt compile đầu thiếu `using UnityEngine.U2D`, giữ logv1; không gọi đây là testRED. Finaltestslog/XML và CLI `--require-fit` đã chạy, không nới assertion. Chỉ sửa C# test assembly, không runtime C#; giữ Player evidence186frame/profile của81b945af, không build/capture lại cùng runtime.

Gate kỹ thuật Võ Lv1 hiện đủ để tiếp **Lv10/mặc chéo trên cùng base/rig/10slot**; art vẫn DRAFT, chưa product/physical-device/owner visual approval. Next: dùng quy tắc Lv10 (viền đồng nhỏ, nút đai rõ hơn, giữ silhouette starter) và board Võ đã chọn để author item rời có source canonical; ưu tiên thay đổi kết cấu đai/quyền khí cùng điểm nhấn vật liệu, không chỉ đổi hue. Giữ base cũ, source motion v7 div4; không copy progression board vào atlas. Sau source fit nối loadout theo từng item/tier vào registered path, rồi kiểm fullLv1/fullLv10/mặc chéo/tháo đồ trên Player. Chưa class khác; Map01A là target.

## Wardrobe: cặp vạt nữ liền vật liệu và ma trận slot — 2026-09-11

Audit tiếp theo `build/vo-garment-completeness-v1`: đã xem nguồn10slot nam/nữ;51part có hash/registration đúng.33mảnh gear có atlas crop pixel-identical với nguồn, không mất pixel alpha≥32 ngoài crop; cả8jointmesh phủ đủ pixel alpha≥32 ở bind. Đây là kiểm vật liệu/crop/mesh, không thay full-outfit fit. `review_lgo_paper_doll_pack.py` hiện đọc manifest cũ (`levels/atlases/rigParts/equipmentComponents`), chưa hỗ trợ registered (`parts/bodyMesh`); chưa chạy hoặc claim full-fit PASS cho pack mới. Next cụ thể: bổ sung đường đọc registered vào gate so bind hiện có, dùng source canonical và geometry đã bind làm hai phía đối chứng, giữ ngưỡng IoU≥0,90/area0,90–1,10/MAE≤22; không dùng cùng một ảnh render làm cả reference lẫn candidate. Sau gate này mới kết luận fit Lv1 và mở Lv10/mặc chéo.

Đã thay đúng hai source vạt nữ ngắn bằng vạt đen/ivory dài theo mẫu Võ nữ, trên canonical canvas cũ; vải liền từ mép may đến gấu, không chứa body/đai/quần. External `registered-equipment-authoring-v1/female-material-v3` giữ raw, mask, recipe, mesh, before/candidate và evidence. BodyMesh cùng26part khác giữ nguyên descriptor/pixel; atlas512²/141523byte, SHA031564b6be99d3ec3a722af9b88129741bb2e9dba715e5cb1a69f090c9fb6b4c. Giữ10slot/17attachment, binding vạt và giới hạn30° đã có; không base/scale/camera/frozen.

Capture tùy chọn `--wardrobe-matrix` kiểm16tổ hợp inner/outer/pants/belt cho mỗi giới, ghi enabledCore và filename theo frame/giới/bits. Review bắt lỗ hổng tham chiếu cùngảnh; helper đã buộc đúng `{frame:02d}-{gender}-wardrobe-{bits:02d}.png`, có test ảnh trùng/sai. Baseline matrix chỉ ra vạt cũ quá ngắn; candidate Player build171942282byte,0error/0warning. `build/vo-female-panels-v1/capture/{mobile,tablet,pc}`186frame/profile,32tổ hợp/profile,errors=[]; đã xem matrix nam/nữ PC và đứng/chạy/lộn/tháo vạt/base-after-jump ba tỷ lệ trong `panel-action-review.png`. Không thấy vạt còn lại sau tháo hoặc tách khỏi hông ở các ảnh đã kiểm. Equipment4/4, body10/10, pack10/10,capturehelper9/9; no3D/no-source/frozen/diff sạch. Budget mặc định700line WARN do thay mesh cũ pretty JSON (~2193 dòng xóa) bằng mesh compact; batch này dùng cap3000line, không nới validator sản phẩm.

Bốn nhịp Võ nam đã có Player evidence mới trong `build/vo-four-beat-review-v1/loop`: vào → [contactA/bayA/contactB/bayB] → dừng,3vòng đủ4×5frame/30fps; giữ nguồn v7 div4 và base. Agent đã review strip, chưa gán owner approval cảm giác chuyển động. Batch hiện tại chỉ đóng vạt và công cụ kiểm tổ hợp; toàn wardrobe vẫn DRAFT/runtimeEligible=false/productionEligible=false. Next: kiểm độ hoàn chỉnh vật liệu và fit áo trong/áo ngoài còn lại theo matrix hiện có, gom sửa nguồn nếu có lỗi cụ thể; feedback motion mới được ưu tiên trước. Chưa Lv10/mặc chéo/class khác, Map01A vẫn target. Không dùng đủ10slot để claim hoàn thiện wardrobe.

## Ưu tiên hiện hành: xác nhận bốn nhịp Võ nam — 2026-09-11

Owner nhắc lại: chạy cần4nhịp lặp, không tính nhịp đầu/cuối. Capture mới bằng Player `build/vo-female-panels-v1/LinhGioiOnline.app` (171942282byte,0error/0warning), base nam cũ + nguồn v7 div4: `build/vo-four-beat-review-v1/loop`.180frame/30fps; frame15–18 vào,19–38 là contactA→bayA→contactB→bayB, mỗi nhịp5frame; vòng39–58 và59–78 lặp đúng thứ tự. Registered/source cùngID; vào/dừng nằm ngoài loop. Không sửa tiếp source/rig/camera chỉ để tăng số tên pose. `six-beat-strip.png` và `four-beat-player.webp` crop nguyên pixel, giữscreenY; đã xem strip bốn dáng. Xác nhận cấu trúc nhịp, không tự coi cảm giác motion được owner duyệt.

Giữ WIP wardrobe đã có, không rollback: capture matrix/helper và cặp vạt nữ dài đang chưa commit. Finalcapture `build/vo-female-panels-v1/capture/{mobile,tablet,pc}`186frame/profile,32tổ hợp/profile,errors=[]; đã xem ma trận nữ PC, chưa hoàn tất review mọi ảnh action/profile nên không claim full wardrobe fit. Pack10/10,capturehelper9/9,no3D/no-source pass. Next: đối chiếu clip bốn nhịp với feedback owner trước khi phát triển thêm wardrobe; phần WIP chỉ hoàn tất kiểm evidence đang có. Chưa Lv10/mặc chéo/class khác; Map01A vẫn target. Không commit/push batch WIP chưa đóng visual gate.

## Wardrobe nữ: bốt cao/tóc đã vào Player — 2026-09-11

Đã tích hợp source `registered-equipment-authoring-v1/female-material-v2`: bốt cao theo mẫu Võ nữ, hai joint mesh shin→foot giữ17attachment/10slot, thay đúng foot gốc; tóc bỏ42pixel mảnh da rời.25part atlas còn lại pixel-identical, body/base/rig không đổi. Atlas512²/132828byte, SHA0a6fc8c9774a1f2b52362d74138f48cb93288aa28a9af4c0ba20bfff3e4200a9; mesh sinh từ alpha và serialize compact, không chứa source board.

Test Unity đầu bắt tam giác bốt nữ đảo ở run/9 (3pass/1fail, giữ `tests.xml`). Nguyên nhân: bàn chân nữ ở pha thu chân vẫn world-flat khiến cổ chân xoắn mạnh; replay cũ lấy mẫu theo2,5Hz nên bỏ sót một phần vòng1,5Hz. Đã sửa test sweep theo cadence hiện hành và cho bàn chân nữ đang bay theo hướng cẳng chân khi góc vượt60°, blend theo độ nhấc chân. Không đổi đường đi cổ chân, chân planted, scale, base hoặc motion Võ nam v7. Không nới ngưỡng tam giác; capture đếm đúng female boots-on6joint/8rigid, boots-off/base giữ counts cũ.

Kết quả18/18 locomotion+equipment, body pixel10/10; full-cycle offline96pose minJacobian gần0,476/xa0,514, mọi tam giác cùng hướng. Build171936314byte,0error/0warning. `build/vo-boot-articulation-v1/registered/{mobile,tablet,pc}` đều154frame,errors=[],maxReturnError<0,000008. Đã xem đứng/chạy/lộn/tháo bốt/base-after-jump cả ba tỷ lệ trong `boot-review.png`; chưa thấy hở cổ chân hoặc bốt còn lại sau tháo. Hai lượt test do lỗi cụ thể, một build/một capture batch; không build lại cho metadata evidence.

Vẫn DRAFT/runtimeEligible=false/productionEligible=false cho toàn bộ wardrobe. Next: hoàn thiện vật liệu áo/vạt độc lập và kiểm16tổ hợp inner/outer/pants/belt trên base cũ; không dùng count10 hoặc riêng bốt đạt để claim full outfit fit. Giữ pack Võ nam v7 div4 hiện hành; chưa Lv10/mặc chéo/class khác, Map01A là target. Source/mesh/replay/tests/ảnh quan trọng giữ external female-material-v2/runtime-candidate-v1.

## Võ run: bổ sung hai đỉnh bay trên bốn nhịp — 2026-09-11

Feedback thiếu nhịp đã được đối chiếu với Player: vòng có bốn key riêng nhưng đầu/hông giữ nguyên độ cao, headY=335,458 ở cả bốn key. Bản v7 giữ contact A/B, entry/exit, idle/jump nguyên file; dịch hai source bay lên48px bằng phép dịch nguyên pixel, không resample/crop nét. Rig dịch hông/thân/target tay cùng lượng, không đổi base/chiều dài chân/scale/camera. Tham khảo nguyên tắc contact–peak tại https://blog.animschool.edu/tag/body-mechanics/; đây là điều chỉnh nguồn hiện có, không thêm pipeline hoặc asset3D.

Nguồn hiện hành: external `class-work-in-progress/vo-lv001/legacy-base-run-weight-v7-div4`. Pack `build/vo-run-weight-v1/pack`: div4, REVIEW_ONLY,512×1024,439382byte,2MiB; PNG byte-identical với v6, chỉ source registration/manifest đổi theo hai pose bay. V6 được giữ nguyên để đối chiếu. Nhịp vẫn vào → [contact A → bay A → contact B → bay B] → dừng,1,5vòng/s; entry/exit nằm ngoài loop.

Evidence mới `build/vo-run-weight-v1/loop`:180frame/30fps, ba vòng đủ bốn nhịp; head chạm335,550/bay340,400, chênh4,850px. Đã xem strip nguồn–rig đứng/vào/bốn nhịp/dừng/lộn, lưu clip crop nguyên pixel `player-run.webp`. Registered ba tỷ lệ trong `build/vo-run-weight-v1/registered` đều154frame,errors=[]; đã xem run/off-inner/base-after-jump ba tỷ lệ. Đây là visual review của agent, không tự gán owner approval hay toàn bộ class/wardrobe PASS.

Kiểm chứng batch:18/18 EditMode locomotion+equipment (không claim toàn suite hoặc SourcePoseReviewTests), pack10/10, body10/10, capturehelper8/8; build171889418byte,0error/13warning. Một test run, một build và một batch capture loop+ba tỷ lệ; không retry. No3D/no-source/frozen/diff audit sạch. Recipe, pack, test và evidence nhỏ được giữ external v7/runtime-review.

WIP wardrobe đã kế thừa và đóng lỗi body nữ: inner_top không còn thay pixel da trong bốn tổ hợp tóc/quần; atlas khác giữ nguyên. Audit51part source/hash/registration đúng chỉ chứng minh provenance, không chứng minh đồ hoàn chỉnh. Các lớp garment còn thiếu vật liệu kín, fidelity boots/tails nữ và bind-fit tổ hợp vẫn chưa đạt; giữ DRAFT/runtimeEligible=false. Không rollback registered outfit.

Next: đối chiếu cảm giác chạy ở tốc độ thật trong clip v7 với feedback bốn nhịp; không sửa tiếp chỉ vì tên pose/test. Nếu còn lỗi nhịp, sửa source/rig cùng batch. Khi motion ổn mới tiếp material-completeness và bind-fit wardrobe10slotLv1 trên base cũ, rồi Lv10/mặc chéo, rồi class khác. Map01A vẫn target tổng thể; không mở camera/frozen/new-base.

## Võ nam đã retarget bốn nhịp vào registered rig — 2026-09-11

Nhịp hiện hành: vào chạy → [contact A → bay A → contact B → bay B] lặp → dừng → đứng. Source review và rig cùng1,5 vòng/s; chuyển động2,4 world unit/s. `TwoDAuthoredVoRun` lấy mốc hông/gối/cổ chân từ audit nguồn v5 và entry/exit v6, nội suy góc giữa bốn key để giữ chiều dài đùi/cẳng chân. Test đo đủ tám cổ chân ở cả hai hướng, sai số<0,0001world; base và tỷ lệ không đổi. Tay chạy dùng target đọc từ source, target tay xa sau được giới hạn tầm với; không claim pixel-exact với hình vẽ full-frame.

Đã sửa nhánh gập khuỷu khi lộn: tay chạm gối nhưng khuỷu từng văng sau đầu; test mới chứng minh RED rồi GREEN với gập dưới vai. Pivot lộn theo pack(512,820), hông co xuống theo pose, giữ base/skeleton và không camera. Entry/exit ngoài loop, refresh không tăng thời gian; bước tiến nhanh của capture được truyền cho cả hai timeline để về bind ngay khi đã qua transition.

Evidence: base comparison `build/vo-source-retarget-base-runtime-v3` có180frame/30fps, ba vòng4nhịp, tám pose và ID registered/source trùng khi chạy/vào/dừng. Đã xem strip/ảnh đứng,chạy,lộn; clip `player-motion-comparison.webp` crop không scale và giữ raw BMP. Clip thuộc build v3; build v4 chỉ sửa xử lý seek thời gian, có provenance riêng. `build/vo-source-retarget-registered-v4/{mobile,tablet,pc}` mỗi profile154frame,20action transitions,40toggles,4held-jump restarts,30base pose frames,errors=[] và return-to-bind đạt; ảnh run PC/jump tablet/mobile đã xem. Không đồng nghĩa đã duyệt mỹ thuật mọi slot hoặc chứng nhận mobile thật.

Test focused20/20; pack10/10,capturehelper8/8. Build cuối171889274byte,0error/13warning. No3D/no-source/frozen audit sạch. Các retry có bằng chứng: mirror world-rotation sai → chuyển bind-space; một test oracle cổ chân phải contactB sửa1339→1368 đúng audit; khuỷu sai nhánh IK → flip; capture v3 mobile return drift sau walk → sửa seek, giữ log FAIL. Không hạ assertion. Đã gom thành một checkpoint; không commit từng sửa nhỏ.

Pack nguồn giữ nguyên v6 div4 REVIEW_ONLY512×1024/439382byte/2MiB, SHA d52ac7992b1eea742c2e76168eb551f2209389529447a0da1379bc157c0a7cfa. Không redraw/source mới, không rollback WIP/main, không frozen/camera tangent. Recipe/provenance/clip và ảnh quan trọng được copy external `legacy-base-run-entry-exit-v6-div4/registered-retarget-review`.

Phần nhịp/chuyển động base đã có bằng chứng ổn định để tiếp tục **audit wardrobe/10 slot Lv1**. Toàn bộ registered art vẫn DRAFT; không đánh dấu runtime-approved hoặc class/tier visual PASS khi fit/semantics chưa đủ. Next: audit source/registration/bind-fit và ảnh từng slot nam/nữ đã capture, lập nhóm lỗi thực sự rồi sửa một batch. Chưa Lv10/mặc chéo hoặc class khác; Map01A vẫn target tổng thể. Chỉ quay lại motion nếu ảnh/feedback mới chỉ ra lỗi cụ thể; không mở camera.

Bắt đầu bằng gate trong `docs/art/LGO-CLASS-2D-MODULE-STANDARD-v1.0.md`: profile/rect nguồn, same-bind outfit fit, item không dính body, đủ10slot tách biệt và tháo/mặc không đổi base. Kế thừa registered resources đã audit, không copy cả WIP. Nếu helper fit chưa có ở clean branch thì đọc/audit helper WIP rồi port dependency cần thiết; không tắt guard hoặc tự gán runtimeEligible. Ưu tiên dùng154ảnh/profile sẵn có trước khi quyết định cần build/capture mới.

## Lịch sử checkpoint (không thay action hiện hành)

> **Gate Võ Lv1–30 — 2026-09-10:** functional class slice đã pass với base nam/nữ, 4 tier, 10 slot, cởi/mặc từng slot, rig 10 bone, 112 component, full-frame Lv1 và rig motion toàn tier cho idle/walk/run/jump/basic/`Liên Quyền` + hit feedback. Batch mixed-level cho mặc riêng `outer_tunic` Lv30 trên loadout Lv1, giữ qua nam/nữ và action. Evidence `build/vo-mixed-level-v1/capture/` có 78 artifact/profile; trạng thái `VO_LV1_30_FUNCTIONAL_VERTICAL_SLICE_PASS / ART_PRODUCTION_DRAFT`.
>
> **Next:** chuyển sang Kiếm Lv1–30 bằng cùng shared base/rig/10-slot contract, kế thừa draft atlas đã có và chỉ đưa item qua production equip khi fit gate pass. Sau Kiếm mới tới Pháp, Cơ, Linh; mỗi class theo một batch riêng và vẫn giới hạn Lv1–30.

> **Checkpoint Võ modular motion alignment — 2026-09-10:** base/modular và các tier Lv10/20/30 giữ nguyên toàn bộ layer đang mặc khi walk/skill bằng một root pose chung; full-frame motion chỉ dùng cho `Lv1 + full`. Capture mobile/tablet/PC đã chứng minh nữ modular sau khi tháo `inner_top` vẫn chỉ còn `base + 9 slot`, slot đã tháo không xuất hiện lại; frame skill vẫn có hit `100→65 HP`. Evidence: `build/map01a-art/vo-aligned-modular-final-three-profiles-v2/`. Đây là compatibility fix, chưa phải limb rig production hoặc chứng nhận thiết bị vật lý.
>
> **Next:** làm một batch rig/attachment Võ theo các anchor đầu-ngực-hông-tay-chân, rồi nối idle/walk/run/jump/basic attack/`Liên Quyền` cho outfit Lv1–30. Chỉ capture lại sau khi trọn batch asset + motion + equipment đã nối; chưa mở class thứ hai.

> **Checkpoint Võ combat hit — 2026-09-10:** `Liệt Phong Kích` chỉ kích hoạt khi mục tiêu còn sống và trong 2,4 world unit; hit được áp tại key timing, trừ 35 HP, tăng hit counter, đổi tint mục tiêu và ghi feedback lên HUD. Capture 12 trên cả mobile/tablet/PC xác nhận `100→65 HP`; spam trong active window bị chặn. Đây là một skill runtime kiểm chứng được, chưa phải combo `Liên Quyền`, AI/knockback hoặc combat production.
>
> **Next:** đóng phần motion/trang bị khớp nhau cho Võ Lv10/20/30. Ưu tiên rig/attachment theo limb để item tùy chọn đi cùng pose; chỉ dùng full-frame tier sheet nếu batch QA chứng minh không làm nổ tổ hợp asset. Sau gate này mới cân nhắc class thứ hai.

> **Checkpoint Võ progression Lv1/10/20/30 — 2026-09-10:** một sheet chung đã được tách theo batch thành bốn tier, hai giới và 10 slot/tier. Runtime có 96 paper-doll part trong bốn atlas 1024²; motion Lv1 có sáu pose thật cho mỗi giới trong hai atlas riêng. Tổng sáu PNG 643.722 byte. Player cho đổi tier bằng L/touch; capture 18 trạng thái × 3 profile đã technical pass và review trực tiếp frame female walk + Lv10/20/30.
>
> **Next:** hoàn thiện Võ class gate bằng motion khớp từng tier (hoặc rig attachment production) và combat `Liên Quyền` có hit timing/target feedback. Hiện Lv10/20/30 cố ý giữ paper-doll tĩnh khi walk/skill để không tráo ngược sang bộ Lv1; vì vậy chưa claim class Lv1–30 hoàn chỉnh. Gom asset/motion/combat rồi test ba profile một lượt, chưa mở class thứ hai.

> **Checkpoint Võ Lv1 — equipment + motion — 2026-09-10:** runtime hiện có Võ nam/nữ cùng art direction, `base/full` và 10 slot paper-doll có thể chọn/bật/tắt trong Player. Hai atlas 1024² chứa 24 part tĩnh (91.553 byte) và 6 frame Võ nam (124.385 byte); walk và `Liệt Phong Kích` dùng pose thật thay transform giả. Capture cuối có 42 ảnh (14 trạng thái × mobile/tablet/PC) và đã review trực tiếp. EditMode `178 total / 177 passed / 0 failed / 1 skipped`; build macOS `162.751.299` byte, 0 error/13 warning; smoke, matrix và branch guards đều pass.
>
> **Next coherent batch:** tạo theo lô một sheet tiến cấp Võ Lv10/Lv20/Lv30 cho cả nam/nữ và một sheet motion nữ, cùng canvas/foot anchor hiện tại; sau đó nối selector cấp và frame animation rồi mới chạy một lượt test/build/capture ba profile. Đây mới là checkpoint Lv1: chưa claim đủ progression 1–30, motion nữ hoặc combat class hoàn chỉnh; chưa mở Kiếm/Pháp/Cơ/Linh.

> **Checkpoint Võ Lv1–30 trên Map 01A — 2026-09-10:** PC cyan đã được thay trong preview bằng hình Võ nam gold/black kế thừa từ WIP đã audit. Một atlas 512²/23.370 byte chứa base, full, ba slot `inner_top/arm_guard/main_weapon` và slash VFX; cùng canvas/ground anchor nên `full/base/modular` đổi bằng C hoặc touch mà không lệch chân. Walk có transform motion và `Liệt Phong Kích` dùng X/touch. Capture macOS Player 36 ảnh (12 trạng thái × mobile/tablet/PC) đã xem trực tiếp; đây là **workflow proof**, chưa phải animation frame/skeletal production hoặc Võ đủ 10 slot.
>
> **Next:** tiếp tục `CLASS-VO-01` theo một batch asset lớn: tạo/tách đủ bảy slot còn thiếu và bộ nữ trên cùng canvas, rồi làm sprite-frame hoặc skeletal motion thật cho idle/walk/run/jump/basic attack/`Liên Quyền`. Giữ atlas theo cấp dùng lại, target 512–1024, ASTC 6×6 mobile; capture lại ba profile một lần khi cả asset + motion đã nối xong. Không mở class thứ hai hoặc map 01B.

## Batch source props + phủ nền — 2026-09-10

Goal vẫn Map01A Cổng Đông Lâm trước, sau đó một class Lv1–30; không dùng wording Đông Môn/Người Giữ Cổng cũ để mở scope khác. Batch trước là tiến triển thực: cắt89 mảnh/hash/pixel. Batch này cleanup16 props cùng sheet, ghép4 vào opt-in Player bằng atlas256²/33.209 byte và sửa phủ nền cả hai chiều sau parallax.

Evidence `build/map01a-art/source-props-three-profiles/`: 12 frame/3 aspect technical pass; đã xem ba frame village-lane, hết viền đen trên tablet/PC, props đúng vị trí. EditMode167 pass/0 fail/1 skip sau sửa import. Có3 lần test lỗi trước đó (log riêng),1 macOS build159.293.123 byte/0 errors/13 warnings,1 capture batch3profile; không giấu retry. Baseline smoke +20frame capture + matrices pass chỉ chứng minh regression prototype cũ, không chứng nhận Map01A Q01–Q09. Đã xem baseline01/02/03/20 bằng contact sheet, vẫn blockout.

Map **VISUAL_FIX_REQUIRED**: skyline quá nổi và phóng lớn, ground lặp rõ, player primitive, mới arrival/thoại Hạ Vân; thiếu route đầy đủ/NPC/quest/items/combat. Chưa push/claim map pass hoặc mở class. WIP code/main khác giữ nguyên; hai GUID settings tự sinh đã lưu patch rồi restore trong worktree này.

Đã rà contact sheet toàn bộ35 ảnh Đông Lâm: `build/map01a-art/source-audit/index.json`, page01/02. Sheet parallax ảnh26 đúng bộ xanh–vàng nhưng các lớp trong board chỉ vài trăm pixel, không upscale thành nền full-screen. Board34 có sáu NPC toàn thân nhưng nét cartoon/kiểu cổng khác nên chưa trộn. Tiếp theo hoàn thiện nguồn parallax/terrain/NPC thống nhất ở đúng pixel budget rồi mở route Map01A theo contract; giữ group crop/cleanup/atlas, không tạo từng asset lẻ tùy ý. Bảng alpha nhóm16 vẫn có matte dưới bàn trà/trong kệ/chậu: giữ ngoài ingest, không claim sạch cả16.

> Chỉ đạo nguồn mới nhất: dùng bộ đồng nhất, không tin tên canonical cũ. Đã batch-crop 89 mảnh từ sheet08/06; nguồn hiện hành `/Users/minhdc/Projects/Design/LGO-Extracted-2D-Items-v1/map-01a-reviewed`, plan `docs/art/LGO-MAP01A-BATCH-CROP-PLAN.json`. 04/07 bị giữ ngoài batch do sai chức năng/khác style. Tiếp theo cleanup alpha/duplicate theo nhóm, chọn NPC đúng bộ, rồi ghép Map01A; không tự tạo lại từng item. Runtime WIP vẫn VISUAL_FIX_REQUIRED (cover nền tablet/PC + player placeholder); chỉ chạy bộ Unity/build/3-profile capture khi gom xong batch runtime.

> Quy tắc batch và bài học đã commit tại `57b83f0` (AGENTS.md + docs/art/LGO-MAP01A-ASSET-OPTIMIZATION-LESSONS.md). Batch scale/downsample đã chạy 1 EditMode + 1 build + 1 capture 12 ảnh sau khi gom thay đổi cuối. Capture kỹ thuật pass; review ảnh `build/map01a-art/reduced-three-profiles/` phát hiện hở nền phía trên ở tablet/PC: cover nền mới tính theo width, chưa bao cả height và camera offset. Gom sửa này cùng các lỗi visual tiếp theo; không báo visual PASS, không lặp build chỉ để đổi tài liệu.

> Quy tắc thực thi mới: đọc mục “Quy tắc owner — làm theo batch, tránh vòng lặp nhỏ” trong `AGENTS.md`. Chốt kết quả/batch trước sửa; gom lỗi review; chỉ rerun gate theo trigger cụ thể; lưu số lượt và lý do cuối batch.

> Bắt buộc đọc trước batch art/scale: `docs/art/LGO-MAP01A-ASSET-OPTIMIZATION-LESSONS.md`. Owner yêu cầu runtime giảm chất lượng/dung lượng, chia tile dùng lại và lưu bài học. Batch đang gom camera/HUD scale + downsample/tile; budget mới 4 MiB PNG, chưa dùng số 7 MiB cũ làm mục tiêu.

## Owner Goal Override — Cổng Đông Lâm Map 01A — 2026-09-10

Audit sandbox “Chuẩn hóa module 2D class” đã hoàn tất trước khi triển khai map. Không cherry-pick nguyên nhánh vì diff có 48 file xóa và nhánh hiện hành đã đi trước 54 commit riêng. Đã giữ bundle/patch, chọn 26 source chi tiết, xác nhận 28 unit test PASS và phân loại rõ tool/contract/WIP tại `docs/art/LGO-CLASS-STANDARDIZATION-REUSE-AUDIT-v1.md`. Class code chỉ port chọn lọc sau gate Map 01A.

`MAP01A-01` đã khóa bằng runtime JSON: 10 khu, 12 parallax/render layer, 6 NPC, 4 quái chỉ ở combat edge, Q01–Q09, UI safe area và portal Suối Thanh Minh; Player manifest có `runtimeCongDongLamMap01AContractSnapshot`. Review ảnh xác nhận prototype hiện vẫn tối/rectangle và còn label Đông Môn/Linh Thành cũ, nên đây là contract PASS chứ chưa phải visual Map 01A PASS.

Nguồn sản phẩm mới nhất thay các mục Đông Môn/Linh Thành cũ bên dưới:

- Việc đang làm: **Cổng Đông Lâm = Map 01A**, tutorial chung Lv1–3 sau năm class intro riêng.
- Source hình canonical: `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/map-01a-cong-dong-lam/01..10`; catalog và giới hạn dùng tại `docs/art/LGO-SELECTED-2D-SOURCE-CATALOG-v1.md`.
- World flow khóa: Class Intro → Cổng Đông Lâm → Suối Thanh Minh → Rừng Ngoại Vi → Đồi Phong Linh → Miếu Linh Sơn → Sơn Thạch Thủ → Linh Thành. Batch này chỉ Map 01A; Linh Thành chỉ là silhouette xa.
- Không tiếp tục lấy primitive hiện có làm art direction. Giữ state machine, input, anchor, Tilemap/atlas contract và evidence tooling nếu phù hợp; thay nội dung/cảnh theo source canonical.
- Không đưa nguyên design board vào runtime. Tách/redraw/crop thành asset riêng theo layer với provenance, alpha/pivot/anchor/sort và review Player.
- Công sức class cũ không bị bỏ: source pack giữ 12 file Võ WIP gồm base nam/nữ đã căn ground, mask plan, ba slot alpha và atlas/toggle review. Chưa import runtime vì còn lỗi cổ tay/alpha và thiếu slot/motion.

### Batch đa màn hình / tối ưu asset — đã kiểm kỹ thuật

Đã gom importer desktop BC1/BC3 + Android/iOS ASTC 6x6, hai texture dùng chung, terrain lặp sprite, camera, grounding, HUD safe-area, joystick có sẵn và hội thoại Hạ Vân. Input legacy bị suspend khi mở preview; trò chuyện này không chạy quest Shadow Slime/Linh Thành cũ. Pack PNG 6.764.005 byte trong budget 7 MiB; GPU byte trong manifest là **ước tính**, chưa đo thiết bị thật. Build Player mới 161.426.371 byte, trước nén 177.317.779 byte.

Một lượt tích hợp và một lượt sửa lỗi review: EditMode cuối 168 total/167 pass/0 fail/1 skipped; build 0 errors/13 warnings. `tools/capture_lgo_map01a_art.py --profile all` tạo 12 ảnh (arrival/dialogue/gate/village × mobile 1600×720, tablet 1024×768, PC 1280×720), manifest xác nhận grounding/parallax và mở/đóng thoại. Evidence: `build/map01a-art/batch-final-three-profiles/`, log `build/map01a-art/review/batch-final-*`. Đã xem ba ảnh dialogue: marker đúng vị trí, hộp thoại không che NPC. Đây là mô phỏng tỷ lệ macOS, chưa kiểm touch/notch/GPU mobile thật. PC vẫn placeholder; map chưa đạt visual/product gate.

Next batch: mở rộng Map01A theo route/quest source với Quan Thủ và tương tác chung, tiếp tục giữ atlas dùng chung và budget; dùng base/slot WIP đã audit khi cần thay PC placeholder, không phát triển cả năm class. Gom đủ một đoạn gameplay rồi test/build/capture chung, không build sau từng chỉnh nhỏ. Bổ sung các trạng thái cần thiết vào cùng capture; chưa chuyển sang hoàn thiện class trước khi đóng map.

### Actions theo thứ tự

1. **MAP01A-01 — World strip contract:** thay route cũ bằng 10 khu `Spawn/Hạ Vân → Đại Cổng → Quan Thủ → Quảng trường → Tổng Phú → Thanh Nhi → Giếng/Cầu → Lão Trần → Combat edge → Portal Suối Thanh Minh`; khóa 12 parallax/render layer, collision và UI safe area từ source.
2. **MAP01A-02 — Visible art slice:** dựng một màn gameplay 16:9 từ đúng palette/architecture/terrain/vegetation source; loại cảm giác rectangle blockout ở góc spawn–đại cổng; Player và NPC đứng đúng ground.
3. **MAP01A-03 — Lv1–3 playable flow:** Q01–Q09, NPC chính, inventory/potion/gather/loot/equip/chest, bốn quái chỉ ở rìa làng và portal Map 01B. Không boss, không monetization, không skill thứ hai.
4. **MAP01A-04 — Runtime gate:** Unity EditMode, smoke, macOS Player build/capture, matrix, no-3D/no-source-image, frozen audit và review ảnh so với 10 source canonical.
5. **CLASS-VO-01 — sau khi map pass:** tiếp tục WIP Võ, không làm lại; hoàn thiện Võ nam/nữ Lv1–30 với 10 slot, thay đồ, idle/walk/run/jump/basic attack và `Liên Quyền` trong Player.
6. Chỉ sau CLASS-VO-01 mới nhân cùng contract sang Kiếm/Pháp/Cơ/Linh Lv1–30.

MAP01A-02 đã có pack draft tái tạo bằng `tools/pack_lgo_map01a_art.py` (Pillow venv class cũ). Nguồn và SHA ở `build/map01a-art/source/manifest.json`; atlas/nền ở `build/map01a-art/pack/`. Cổng và Hạ Vân có alpha thật. Terrain bị bake checkerboard trong cả hai lần export, nên chỉ dùng vùng đá đặc `(0,330,2172,724)` của bản v1; không coi ảnh đó là alpha sprite. Script kiểm hash nguồn/reference và alpha trước pack.

Đã bổ sung `village-midground-draft-v1.png` (alpha thật, review trên trắng/xanh tối), pack bốn part `gate/ha-van/terrain/village` và năm layer placement có order/parallax. Review mới `build/map01a-art/review/authored-layer-assembly-v2.png` dựng trực tiếp từ manifest: chân cổng đã chạm mặt đường, lớp làng che phần khoảng trống dưới cổng. Đây vẫn là composite, chưa Player evidence. `build/map01a-art/review/pack-qa.txt` xác nhận rect bounded/nonoverlap, alpha RGBA giữ nguyên sau resize, source hash mutation bị từ chối; Python compile PASS.

Renderer `CongDongLamMap01AArtPreview` đã tích hợp Resource riêng qua `--lgo-map01a-art-preview`; atlas bốn part/năm layer, parallax và khôi phục renderer/camera được kiểm bằng EditMode. Lượt đầu phát hiện OnDestroy EditMode chưa restore, đã sửa bằng ExecuteAlways; test cuối 168 total, 167 passed, 0 failed, 1 skipped. Build macOS thành công (0 errors, 13 warnings); ảnh cửa sổ Player thật `build/map01a-art/review/player-arrival-camera.png` xác nhận camera đã hết cắt mái. Log ở `unity-editmode-camera.log`, `player-build-camera.log`, `player-camera.log`. Đây là preview art, **không phải Map01A playable/visual PASS**.

Next action ngay: căn PC foot/contact shadow trên lane (ảnh hiện vẫn PC blockout sát mép), nối marker/HUD và tương tác Map01A đúng Hạ Vân/Quan Thủ/route. Preview hiện ẩn HUD cũ; chưa thay flow state cũ. Không dùng Editor smoke Shadow Slime/Linh Thành để claim Q01–Q09. Bổ sung capture tự động cho preview mới, kiểm movement/parallax trong Player; sau đó tiếp Map01A-03. Class WIP giữ nguyên đến gate map.

## Current Owner Goal Override — 2026-09-10

- Map đầu Đông Môn là vertical-slice grounding/proof surface; không mở map thứ hai.
- Class workflow chỉ xử lý **một class trước: Võ Lv1-30**. Không làm đồng loạt 5 class, không mở nội dung 31+.
- Việc tiếp theo sau checkpoint cell-map: refine alpha matte/pivot/scale/head-arms-legs coverage và pose/frame motion Võ Lv1-30 trong Player; kiểm chứng thay đồ, tách layer, skill cue, contact shadow và route grounding cùng scene.
- Chỉ khi Võ Lv1-30 có runtime evidence đủ rõ mới nhân pattern sang Kiếm/Pháp/Cơ/Linh Lv1-30.
- Không Meshy/3D, không dùng ảnh thiết kế cũ làm runtime asset, không sửa frozen surfaces.

`LGO_VO_LV1_30_SKILL_CUE_ART_READY`: Skill cue Võ Lv1-30 đã có approved runtime art cho trail/edge/impact (`cells=13`); tiếp theo refine alpha/pivot/body-head-arms coverage và frame motion Võ Lv1-30 trước khi nhân class.

`LGO_VO_LV1_30_CELLMAP_COVERAGE_READY`: Cell-map approved runtime art Võ Lv1-30 đã mở rộng từ 8 lên 11 cell, thêm torso underlay và hai mảnh pants để giảm fallback primitive trong Player. Evidence nằm ở checkpoint commit tương ứng; đây vẫn là art/probe giai đoạn đầu, chưa phải production final.

# NEXT ACTION — Linh Giới Online 2D

## Goal kỹ thuật hiện tại — 2026-09-10

### Base-first override mới nhất

Checkpoint mới nhất: manifest v9 dùng base head không bake tóc; evidence `build/vo-ten-slot-matrix-v2/three-profiles/` đạt 66 frame × 3 profile và đã review đủ 20 trạng thái tháo từng slot cho nam Lv1/nữ Lv30. Gate `CLASS_VO_LV1_30_VERTICAL_SLICE_PASS` chỉ xác nhận map/avatar/equip/action local trong Player, chưa phải production-final. Next: tách loadout, rig pose và action orchestration khỏi `CongDongLamMap01AArtPreview` thành character base dùng chung; giữ Map01A làm consumer và chạy lại cùng evidence trước khi mở class thứ hai.

Checkpoint mới hơn: manifest Võ v8 đã thay delta trang phục nhỏ bằng batch 8 sheet rig-compatible, đóng 112 attachment vào 7 atlas/885.234 byte. Player mobile/tablet/PC tại `build/vo-garment-v1/three-profiles/` đã review: Lv30 nữ có đủ silhouette trong run/jump/basic/Liên Quyền. Next duy nhất trước khi đóng `CLASS-VO-01`: thêm capture matrix cởi/mặc đủ 10 slot cho nam/nữ ở Lv1 và Lv30, kiểm layer order/pivot; không tạo thêm artwork nếu matrix không chỉ ra lỗi cụ thể.

Gate: `VO_LV1_30_GARMENT_BATCH_PLAYER_PASS / TEN_SLOT_VISUAL_MATRIX_PENDING`. Sau gate này mới tách orchestration/controller base khỏi Map01A và chỉ sau đó mới mở class thứ hai.

Đã bỏ hướng mỗi slot tự dịch/xoay. Võ hiện là consumer đầu tiên của `TwoDSkeletalPaperDollRig`: skeleton cha-con, local pose và 96 attachment metadata dùng chung. Next không được copy `CongDongLamMap01AArtPreview` sang class khác. Việc tiếp theo của `CLASS-VO-01` là tạo/chuẩn hóa garment art tương thích chính skeleton này, kiểm full outfit và equip/unequip từng slot qua idle/run/jump/basic/`Liên Quyền`; chỉ khi visual đạt mới tách controller orchestration ra khỏi Map01A và nhân manifest sang class thứ hai.

Gate hình ảnh hiện tại: `VO_BASE_FIRST_RIG_TECHNICAL_PASS / GARMENT_ART_VISUAL_FIX_REQUIRED`. Evidence mới nhất `build/vo-base-first-v3/three-profiles/`; không dùng capture split-only hoặc independent-pivot trước đó để claim. Quy tắc/lỗi đã ghi tại `LGO-2D-PRODUCTION-WORKFLOW-RESEARCH-v0.1.md`.

Owner bổ sung: mỗi class chỉ xử lý dải **Lv1-30** theo giai đoạn phát triển; không mở skill/trang bị high-tier 31-100 trong batch class hiện tại.

Owner đã giao Codex tự quyết định hướng kỹ thuật cho task này. Quyết định hiện tại: **đóng map đầu Đông Môn thành vertical slice 2D có thể kiểm chứng trong Player trước, rồi mới chuyển sang hoàn thiện 5 class**. Các checkpoint Linh Thành/Quảng Trường/district bên dưới là lịch sử runtime preview local-only; không dùng chúng làm lý do mở map thứ hai hoặc tiếp hub trước khi Đông Môn đạt gate.

Hướng kỹ thuật map đầu:

1. Đông Môn dùng authored Tilemap/Grid + Sprite Atlas/prop atlas/parallax layer, đọc từ Resource JSON khi còn ở giai đoạn draft. Không tiếp tục polish bằng rectangle primitive vô hạn.
2. Scene phải phục vụ flow kiểm chứng thật: spawn -> Người Giữ Cổng/thoại -> Bia Luyện Khí -> jump/dash/class skill -> Shadow Slime -> quay lại/hoàn tất. Mỗi trạng thái có capture Player và manifest, không claim từ Editor-only.
3. Art runtime phải là asset mới, có provenance/hash/allowlist rõ ràng. Không dùng ảnh thiết kế cũ, không crop board/reference, không Meshy/3D.
4. NPC/props/terrain đi theo data-driven placement, atlas rect, anchor/pivot và layer order để sau này thay asset production mà không đổi state machine.
5. Gate map đầu là owner nhìn được trong Player: hình có chiều sâu, silhouette đọc được, không chồng HUD/label/player, collision/route khớp hình, thao tác E/I/Tab/T/Y và movement không bị lệch.

Hướng kỹ thuật sau gate map đầu:

1. Tiếp nhận luồng 5 class từ tab riêng bằng commit/asset đã rõ ownership; không ghi đè worktree hoặc file đang dở của tab đó.
2. Chuẩn hóa base chung nam/nữ, item slot alpha thật, anchor/pivot, sorting layer, compatibility rules và snapshot trang bị.
3. Mỗi class Võ/Kiếm/Pháp/Cơ/Linh phải kiểm được trong Player: chọn class, mặc/tháo từng slot, phối bộ tương thích, idle/walk/run/jump và đòn/skill preview trong scope hiện có.
4. Gate 5 class là nhìn thấy đồ/vũ khí đi theo thân khi đổi hướng/chuyển động, không hở cổ tay/chân/thân, không sai layer, không dùng ảnh source/reference làm runtime asset.

Next action ngay: tiếp tục **Võ Lv1-30** làm class mẫu theo function probe vừa thêm. Probe đã chứng minh trong Player manifest: paper-doll slots, try-on/apply, motion states, skill target và grounding Đông Môn đều đi qua cùng một contract. Bước kế tiếp là refine runtime art cell-map thật: làm sạch alpha matte, tách thêm cells đầu/tay/chân/boots/waist/weapon từ `VoLv1ApprovedRuntimeArt`, chỉnh world scale/pivot/sort để nhìn rõ trong Player ở các ảnh 07/08/09/10, rồi mới nhân pipeline sang Kiếm/Pháp/Cơ/Linh Lv1-30. Map đầu vẫn cần art/parallax/prop pass riêng để tiến gần reference; không dùng checkpoint grounding/probe này để claim map hoặc class đã production-final.

## Đông Môn illustrated draft — 2026-09-10

Đã tạo art mới và pack skyline + atlas cổng/NPC/terrain; preview opt-in trong Player qua `--lgo-dongmon-art-preview`, không sửa controller/state/5 class hoặc frozen surfaces. `tools/capture_lgo_dongmon_art.py` capture 5 trạng thái vào thư mục riêng. EditMode 139 pass, 0 fail, 1 skipped; guard 5 test pass; smoke/build/baseline 20 frame và art 5 frame đã chạy, ảnh đã review. Art vẫn DRAFT, player còn placeholder; không claim giống hoàn toàn ảnh owner.

Next/gate: owner xem capture `build/dongmon-art/player-final/03-dialogue.png` (bản copy bền ở `build/dongmon-art-checkpoint/`) và duyệt bố cục/palette/tỷ lệ trước khi nhân rộng. Sau duyệt chỉ hoàn thiện map đầu Đông Môn; xong gate map mới chuyển sang tích hợp và hoàn thiện 5 class từ tab riêng. Hướng/plan/evidence: `docs/design/dong-mon-illustrated/DESIGN.md`. Không tiếp polish primitive; không ghi đè checkout chính hoặc source tab 5 class. Không có blocker runtime; gate còn lại là duyệt mỹ thuật.


## Gate Keeper silhouette — 2026-09-10

Đã kiểm Người Giữ Cổng 13 part đọc từ JSON với `rect/ellipse/diamond/tapered`; shape lạ fallback rectangle, cache dùng lại texture, snapshot đọc style từ source. Capture tiếp cận NPC ở khoảng cách 0.85 trong FocusRange 1.15 để không chồng silhouette. Không đổi gameplay 5 class hoặc frozen surfaces.

Evidence trong `build/gatekeeper-polish-checkpoint/`: EditMode 138 pass, 0 fail, 1 skipped (pointer capture cần UI panel); onboarding smoke Complete; macOS build Succeeded (0 errors, 13 warning API UI cũ); Player capture 20 BMP; smoke/visual matrix, no-3D/no-source-image và frozen diff audit pass. Đã xem ảnh 01/02/03/20: mũ, áo thuôn, bóng ellipse, gậy/ngọc đọc được; player đứng riêng khi thoại. Đây chỉ là checkpoint silhouette blockout, chưa đạt chuẩn illustrated owner gửi.

macOS: sau launch từ Contents/MacOS, Player có thể chờ foreground do runInBackground=0; dùng `open <đúng app vừa build>` để kích hoạt app đang chạy, không mở ảnh giữa capture. Worktree cũ được giữ vì có thay đổi đồng thời ngoài lượt này; chỉ dọn worktree riêng của batch. Tab 5 class giữ ownership source của họ.

Next: design/demo draft góc Đông Môn bằng art mới theo phần hiệu chỉnh owner trong workflow research; capture Player và duyệt mỹ thuật trước khi nhân rộng. Không tiếp tục polish primitive vô hạn.


## Ưu tiên owner mới — 2026-09-09: chuẩn hóa art Võ

Scope hiện tại chỉ Võ, docs/reference/checker; không tiếp gameplay hoặc triển khai class khác từ batch này. Đã review 23 PNG và chọn nguồn trong `docs/art/classes/vo/LGO-VO-2D-MODULE-SPEC-v1.0.md`; handoff: `HANDOFF-LGO-CLASS-2D-MODULE-STANDARD-v1.0.md`. Các mục runtime phía dưới là trạng thái trước yêu cầu này, không phải quyền mở rộng batch art.

Việc tiếp theo: tạo pack clean transparent cho Võ `lv001` + `lv050`: dùng base candidate nam/nữ đã có trong `build/class-2d-review/generated-drafts/`, sinh từng item rời thay vì sheet checkerboard, lắp đủ/tháo từng slot trên base chung, phối chéo level, motion sheet idle/walk/run/jump/basic_attack/skill_preview và mockup rương/paper doll để thử đồ. Draft cross-level đã chứng minh hướng phối `lv001`/`lv050` trên cùng base; hai base candidate có alpha thật và white-check, nhưng sheet 10 slot `lv001` bị bake checkerboard nên chưa đạt transparent item gate. Gate asset hiện thiếu: `python3.12 tools/validate_class_2d_module_spec.py --require-assets` báo thiếu 220 item + 6 board. Sau khi Võ đạt gate này, owner yêu cầu xử lý tiếp class Cơ và Linh theo folder tương tự `/Users/minhdc/Projects/2D/Vo`. Evidence: `build/class-2d-review/validation.log`; lỗi trực quan từng nguồn và SHA-256 lưu trong spec Võ.

Ingest vào Unity còn bị gate `tools/validate_2d_branch_no_source_images.py` chặn ảnh source; cần task asset/ingest riêng giải quyết gate, không tự tắt validator, không import/crop board. Bản chọn vẫn REFERENCE_ONLY; chưa production/runtime art.

## Lịch sử — trạng thái 2026-09-09

Branch hiện tại: `feature/2d`. Owner đã khóa hướng mới qua `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md`: **2D Side-Scrolling Social Action MMORPG**, HD 2D anime / illustrated character, không pixel-art, không Meshy/3D, side-view parallax, social hub + action combat. Kịch bản mới đã được đưa vào `docs/02-GDD.md` để làm nguồn chính trước khi design map: giữ thế giới/class/progression/story/backend, chuyển pipeline sang base sprite/cutout body, layer trang phục 2D, skeleton 2D, anchor point, sprite atlas và runtime test. Cleanup 3D đã commit/push ở `d97a3c8`, cleanup trace dịch vụ 3D ở `6bf905e`, xoá ảnh source cũ ở `646492e`, Đông Môn procedural blockout ở `eeaf19d`, direction/map catalog ở `1fa3231`. Tutorial Jump/Dash/Skill đã checkpoint ở `e330c58`; Shadow Slime combat micro-slice ở `ad9cd52`; map layer budget ở `4bb77f0`; route progress/minimap ở `986b0e5`; 2D-04 Kiếm Lv1 module catalog, Đông Môn landmark/parallax và inventory try-on strip đã checkpoint; batch hiện tại sẵn sàng chuyển sang tileset/terrain collision hoặc panel inventory input thật. Roadmap khóa: 2D-00 Direction Lock → 2D-01 Male/Female Base Character → 2D-02 Modular Runtime → 2D-03 Võ Lv1 → 2D-04 Kiếm Lv1 → 2D-05 Pháp Lv1 → 2D-06 Cơ Lv1 → 2D-07 Linh Lv1 → 2D-08 Animation Foundation → 2D-09 Linh Thành Map → 2D-10 Gate Keeper Tutorial → 2D-11 Shadow Slime Combat → 2D-12 Vertical Slice, trong Zone Network thay vì open-world liên tục.

## Gate hiện tại

- `python3.12 tools/validate_2d_branch_no_3d.py` phải pass trước khi tiếp tục gameplay 2D.
- `python3.12 tools/validate_2d_branch_no_source_images.py` phải pass để bảo đảm ảnh thiết kế/source cũ đã bị loại khỏi source tree.
- Unity batch compile/test phải chạy thật sau các thay đổi source.
- Player smoke phải chứng minh flow hoàn tất trong Player, không chỉ Editor.
- Runtime visual capture phải có manifest và ảnh đã xem bằng mắt nếu có thay đổi player-visible.
- Không sửa frozen surfaces nếu chưa có owner decision riêng.

## Lịch sử — việc tiếp theo 2026-09-10

Chỉ đạo owner mới nhất 2026-09-10: **map đầu trước → hoàn thiện 5 class sau**, ưu tiên chức năng owner tự kiểm chứng trong game. Không mở map thứ hai.

Cập nhật Goal kỹ thuật: map Đông Môn đã là mặt phẳng kiểm chứng anchor/grounding; batch hiện tại tiếp tục Võ Lv1 trước để chứng minh paper-doll slot, tách đồ, pose/frame motion và skill có thể chạy trong Player thật trên cùng scene. Chưa nhân sang 5 class khi Võ chưa có runtime evidence đủ rõ.

Batch tiếp theo của Võ Lv1 đang khóa theo hướng slot compatibility trước: mọi item runtime phải báo selected slot/anchor/fit/profile/provenance trong manifest để owner kiểm thay đồ không lệch trước khi nâng art atlas/spritesheet.

Batch Võ Lv1 đã có anchor/pivot gizmo, runtime fit contract và production atlas contract trong Player: owner có thể kiểm Chest/Hips/Hand/Foot, slot bounds/sort, skill Hand_R, required atlas cells, rig joints và replacement gates trên mặt phẳng Đông Môn. Bước kế tiếp là tạo/nhập spritesheet hoặc layered PSB sạch cho Võ Lv1 theo contract này, rồi mới nhân pattern sang Kiếm/Pháp/Cơ/Linh.

1. Owner xem góc Đông Môn illustrated trong capture Player `03-dialogue.png`; bản này mới là draft góc cổng, chưa coi cả map đầu hoàn thiện.
2. Hoàn thiện Đông Môn theo kịch bản đã có: cảnh/parallax/terrain, đường đi và tương tác không lệch hình; kiểm spawn → Người Giữ Cổng/thoại → Bia Luyện Khí → hướng dẫn hiện có. Bàn giao Player chạy được, phím điều khiển và bằng chứng trước/sau từng thao tác. Không mở hệ thống mới hoặc map tiếp theo.
3. Sau gate map đầu, tiếp nhận commit/asset từ tab 5 class rồi triển khai theo base/slot chung: Võ, Kiếm, Pháp, Cơ, Linh; không ghi đè worktree hoặc source chưa commit của tab đó. Mỗi class có nguồn item rời alpha thật, anchor/pivot, thứ tự layer và quy tắc tương thích rõ ràng.
4. Gate kiểm chứng 5 class trong Player: chọn class → mặc/tháo từng slot → phối các bộ tương thích → idle/walk/run/jump và đòn/skill preview trong scope đã có. Kiểm đồ đi theo chuyển động, không lệch tay/chân/vũ khí, không hở/cắt thân hoặc sai layer khi đổi hướng; snapshot trang bị phải khớp phần đang thấy. Đây là task tiếp theo, chưa phải chức năng đã hoàn thiện.
5. Bàn giao một bản game với hướng dẫn thao tác và expected result ngắn cho từng chức năng; test/capture thực, owner nhìn và thao tác được. Không dùng ảnh concept hoặc test xanh thay bằng chứng gameplay.

## Lịch sử — blocker 2026-09-09

Chưa có blocker. Visual capture hiện bắt được world stage, HUD world-space, minimap/route overlay, scene-beat metadata và character base snapshot bằng camera render; manifest lưu `hudSnapshot`, `productionSceneBeatSnapshot`, `runtimeMapSnapshot`, `runtimeCharacterBaseSnapshot`, `runtimeEquipmentSnapshot`, `runtimeAnimationSnapshot` để kiểm copy/trạng thái. Ảnh source cũ đã bị loại khỏi source tree và có validator riêng để ngăn tái nhập nhầm. Shadow Slime micro-slice, map layer budget, route progress, Kiếm Lv1 module runtime, Đông Môn landmark pass và inventory try-on/input runtime hiện được kiểm bằng catalog check, controller snapshot, smoke runner và visual manifest; chưa có blocker.

## Map runtime checkpoint — 2026-09-09

Song song với luồng class Võ, map Đông Môn vừa có tilemap runtime spine riêng: `DongMonTileDefinitions`, runtime `RuntimeTilemapSnapshot`, manifest `runtimeTilemapSnapshot`, Player capture 10 frame đã review. Next map-safe action: nâng procedural tile strip thành authored tilemap chunks/parallax spacing cho Đông Môn, nhưng chỉ làm khi không đụng scope class/art đang xử lý ở tab song song.

## Map chunk checkpoint — 2026-09-09

Đông Môn đã có `ChunkFlow` runtime: `chunk_gate_entry -> chunk_training_stone -> chunk_jump_bridge -> chunk_dash_lane -> chunk_slime_arena`, visual capture PASS. Next map-safe action: parallax spacing/foreground polish hoặc authored Unity Tilemap asset khi asset sạch sẵn sàng; tránh chạm luồng class/art ở tab song song.

## Runtime smoke matrix checkpoint — 2026-09-09

`LGO-TASK-047` đã có closure `LGO_RUNTIME_SMOKE_MATRIX_READY`: `python3.12 tools/lgo_runtime_smoke_matrix.py --phase two-d` kiểm onboarding smoke JSON, macOS Player build log và visual manifest hiện tại, gồm `runtimeTilemapSnapshot` + `ChunkFlow`. Next safe action: tiếp roadmap map/runtime hoặc task advisor kế tiếp, vẫn tránh chạm luồng class/art ở tab song song.

## Visual evidence matrix checkpoint — 2026-09-09

`LGO-TASK-048` đã có closure `LGO_VISUAL_EVIDENCE_MATRIX_READY`: `python3.12 tools/lgo_visual_evidence_matrix.py --verify-current` kiểm 5 frame visual 2D hiện tại và manifest fields, marker `LGO_VISUAL_EVIDENCE_MATRIX_2D_CURRENT_PASS`. Next safe action: tiếp task advisor kế tiếp hoặc map/runtime nhỏ, không claim production art từ evidence này.

## Crash/error reporting checkpoint — 2026-09-09

`LGO-TASK-049` đã có closure `LGO_CRASH_REPORTING_PLAN_READY`: local plan/validator PASS, không thêm production crash service hoặc telemetry backend. Next safe action: task advisor kế tiếp trong docs/tools hoặc map/runtime nhỏ khi không đụng luồng class/art.

## Release checklist checkpoint — 2026-09-09

`LGO-TASK-050` đã có closure `LGO_RELEASE_CHECKLIST_READY`: release checklist validator PASS; trạng thái vẫn pre-alpha, không mở implementation hoặc production release claim. Next safe action: kiểm advisor mới, ưu tiên task không xung đột tab class hoặc map/runtime 2D nhỏ có evidence thật.

## Đông Môn parallax/foreground checkpoint — 2026-09-09

`LGO_DONGMON_PARALLAX_POLISH_READY`: runtime Đông Môn đã có `ParallaxDepthSnapshot` và visual capture mới PASS 10 frame với cloud-drift, mountain-silhouette, mist/foreground grass thấp; ảnh đã review không che HUD, combat, minimap hoặc inventory. Build app thực tế khoảng 109MB, nhưng Unity Player build log có bước `prepared ~36238M`, nên next safe action ưu tiên kiểm/tối ưu nguyên nhân build prepared-size hoặc tiếp map/runtime nhỏ không đụng luồng class/art đang mở ở tab song song.

## World/Linh Thành zone-network checkpoint — 2026-09-09

`LGO_WORLD_ZONE_NETWORK_RUNTIME_READY`: runtime catalog và minimap overlay đã có World Map/Linh Thành network (`WorldMapNetwork: hub=linh-thanh`, `LinhThanhHubRuntime`), Player capture mới PASS 10 frame, smoke matrix 2D đã bắt token này. Map tổng thể vẫn chưa production-complete; next safe map action là Linh Thành hub shell/Quảng Trường hoặc authored Đông Môn Tilemap khi có asset sạch, tránh đụng luồng class/art tab song song.

## Linh Thành hub shell checkpoint — 2026-09-09

`LGO_LINHTHANH_HUB_SHELL_RUNTIME_READY`: runtime catalog/scene đã có `HubShell: linh-thanh` với Đông Môn, Quảng Trường, Học Viện và Thương Phố; Player capture mới PASS 10 frame và smoke matrix 2D đã bắt token HubShell. Map tổng thể vẫn chưa production-complete; next safe map action là mở Quảng Trường hub shell chi tiết hơn hoặc authored Đông Môn Tilemap khi asset sạch, tránh đụng luồng class/art tab song song.

## Quảng Trường plaza shell checkpoint — 2026-09-09

`LGO_LINHTHANH_PLAZA_SHELL_RUNTIME_READY`: runtime catalog/scene đã có `PlazaShell: district=plaza`, social-spawn/event-board/guild-bulletin preview và guard `safe-no-trade-backend`; Player capture mới PASS 10 frame và smoke matrix 2D đã bắt PlazaShell. Map tổng thể vẫn chưa production-complete; next safe map action là hub runtime riêng cho Quảng Trường hoặc authored Đông Môn Tilemap khi asset sạch.

## Linh Thành unlock presentation checkpoint — 2026-09-09

`LGO_LINHTHANH_UNLOCK_PRESENTATION_READY`: sau khi người chơi hoàn tất Đông Môn/Shadow Slime, runtime state bật `LinhThanhUnlocked=true`, HUD đổi mục tiêu sang `Mở Linh Thành: Quảng Trường`, scene bật banner/path local preview và visual manifest ghi `runtimeLinhThanhUnlockSnapshot` với `unlock=plaza`, `safe-local-no-teleport`. Đây là unlock presentation local-only: chưa mở teleport, shop, giao dịch, bang hội hoặc backend xã hội. Map tổng thể vẫn chưa production-complete; next safe map action là làm Quảng Trường hub runtime riêng với NPC/board local-only hoặc chuyển Đông Môn procedural chunk sang authored Tilemap khi asset sạch sẵn sàng.

## Quảng Trường hub runtime preview checkpoint — 2026-09-09

`LGO_LINHTHANH_PLAZA_HUB_RUNTIME_READY`: Quảng Trường đã có runtime preview local-only sau unlock Đông Môn: `RuntimeLinhThanhPlazaHubSnapshot`/visual manifest chứa `PlazaHubRuntime`, `npc=gate-guide`, `board=event-local-preview`, `guild-bulletin=locked`, `safe-local-no-backend`; scene bật NPC/board/label sau unlock và không hiện sớm ở frame initial. Next map-safe action: thêm interaction local-only cho bảng sự kiện/NPC Quảng Trường hoặc authored Đông Môn Tilemap khi asset sạch sẵn sàng.
## Next after Quảng Trường board interaction — 2026-09-10

Map/runtime checkpoint mới đã có board interaction local-only cho Quảng Trường sau unlock Đông Môn, visual frame `11-plaza-board-preview`, inventory panel ẩn mặc định và Unity EditMode wrapper đã được sửa để không pass giả khi thiếu XML. Next map-safe action: thêm NPC local interaction cho Người Giữ Cổng/Thương Nhân ở Quảng Trường hoặc authored Đông Môn Tilemap asset khi có asset sạch; vẫn tránh đụng luồng class/art tab song song và không mở teleport/shop/giao dịch/bang hội/backend nếu chưa có gate riêng.
## Next after Quảng Trường NPC interaction — 2026-09-10

`LGO_LINHTHANH_PLAZA_NPC_INTERACTION_READY`: Quảng Trường đã có NPC interaction local-only cho Người Giữ Cổng và Thương Nhân preview sau unlock Đông Môn, visual frame `12-plaza-npc-preview`, dialogue speaker đúng NPC và manifest bắt `safe-local-no-shop-backend`. Next map-safe action: chuyển Đông Môn procedural chunk sang authored Tilemap asset sạch hoặc thêm Plaza NPC selector/input thật; chưa mở shop/economy, teleport, giao dịch, bang hội hoặc backend nếu chưa có gate riêng.

## Quảng Trường target selector checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_TARGET_SELECTOR_READY`: Quảng Trường sau unlock Đông Môn có selector runtime local-only: `P` đổi mục tiêu giữa Bảng Sự Kiện, Người Giữ Cổng, Thương Nhân; `E/Enter` tương tác mục tiêu đang chọn. Evidence cần giữ khi resume: `runtimePlazaHubInputSnapshot`, frame `12-plaza-target-selector`, frame `13-plaza-npc-preview`, smoke/matrix PASS.

## Next map-safe action

1. Nâng Quảng Trường social layout để ba mục tiêu hub có spacing/độ đọc tốt hơn và chuẩn bị route vào Học Viện/Thương Phố/Bang Hội ở mức shell local-only; hoặc
2. Chuyển Đông Môn procedural chunks sang authored Tilemap/tileset sạch nếu asset/source 2D đã sẵn; hoặc
3. Thêm transition shell `Đông Môn -> Linh Thành/Quảng Trường` không mở teleport backend, chỉ preview route local.

Không mở shop/economy, teleport thật, giao dịch, bang hội, HP/loot/server-authoritative combat hoặc frozen surfaces trước gate riêng.

## Quảng Trường social layout checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_SOCIAL_LAYOUT_READY`: selector Quảng Trường đã có layout token `spaced-social-triangle`, spacing ba target hub rõ hơn trong runtime capture. Debt còn lại: blockout procedural vẫn có text nhỏ/chồng nhẹ ở marker xa, nên batch map tiếp theo nên ưu tiên authored Tilemap/label layout hoặc transition shell thay vì mở chức năng xã hội thật.

## Hub transition checkpoint — 2026-09-10

`LGO_LINHTHANH_HUB_TRANSITION_PREVIEW_READY`: sau khi hoàn tất Đông Môn, runtime có preview tuyến `Đông Môn -> Quảng Trường` ở mức local-only, evidence `runtimeHubTransitionSnapshot` và frame `14-plaza-transition-preview`. Next map-safe action: authored Đông Môn Tilemap/tileset sạch, hoặc Quảng Trường label/readability pass; không mở teleport/backend thật.

## Quảng Trường label readability checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_LABEL_READABILITY_READY`: Quảng Trường đã giảm mật độ chữ trong world-space sau unlock Đông Môn: nhãn chi tiết quanh NPC/board được thay bằng chip `01/02/03`, banner unlock lớn tự ẩn khi tutorial đã `Complete`, manifest có `runtimePlazaReadabilitySnapshot` với `mode=label-rail`, `world-label-density=reduced`, `target-chips=event-board,gate-guide,merchant-preview`. Evidence mới: Unity EditMode `total=102 passed=101 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame và visual matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là mở Học Viện/Thương Phố shell hoặc authored Đông Môn Tilemap khi asset sạch sẵn sàng, tránh đụng luồng class/art tab song song.

## Học Viện shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_ACADEMY_SHELL_RUNTIME_READY`: Linh Thành có thêm Học Viện shell local-only trong catalog/scene/runtime manifest. `AcademyShell: district=academy` ghi `skill-hall=preview-only`, `class-trainer=locked`, `lecture-board=local-preview`, `safe-no-skill-backend`, `safe-local-no-backend`; scene có silhouette Học Viện/skill board/trainer locked ở hậu cảnh, không mở skill backend hoặc class progression mới. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=104 passed=103 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là Thương Phố shell hoặc authored Đông Môn Tilemap khi asset sạch sẵn sàng, vẫn tránh đụng luồng class/art tab song song.

## Thương Phố shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_MARKET_SHELL_RUNTIME_READY`: Linh Thành có thêm Thương Phố shell local-only trong catalog/scene/runtime manifest. `MarketShell: district=market` ghi `vendor-row=preview-only`, `auction-board=locked`, `item-stall=local-preview`, `safe-no-trade-backend`, `safe-no-economy-backend`, `safe-local-no-backend`; scene có awning/stall/auction-board locked ở hậu cảnh trái, không mở shop, trade, economy hoặc backend. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=106 passed=105 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap hoặc Đền Linh/Khu Dân Cư shell nếu tiếp tục mở Linh Thành theo zone network.

## Đền Linh shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_SPIRIT_TEMPLE_SHELL_RUNTIME_READY`: Linh Thành có thêm Đền Linh shell local-only trong catalog/scene/runtime manifest. `SpiritTempleShell: district=spirit-temple` ghi `blessing-altar=preview-only`, `story-shrine=locked`, `incense-vfx=local-preview`, `safe-no-buff-backend`, `safe-no-story-backend`, `safe-local-no-backend`; scene có shrine/altar/incense VFX ở hậu cảnh phải, không mở buff, story quest backend hoặc progression mới. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=108 passed=107 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là Khu Dân Cư shell để mở social residential spine hoặc authored Đông Môn Tilemap khi asset sạch.

## Khu Dân Cư shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_RESIDENTIAL_SHELL_RUNTIME_READY`: Linh Thành có thêm Khu Dân Cư shell local-only trong catalog/scene/runtime manifest. `ResidentialShell: district=residential` ghi `npc-home-row=preview-only`, `social-chat-node=locked`, `ambient-citizen=local-preview`, `safe-no-housing-backend`, `safe-no-social-backend`, `safe-local-no-backend`; scene có nhà dân/NPC ambient/chat node locked ở hậu cảnh, không mở housing backend, social backend hoặc player-owned home. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=110 passed=109 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap/collision polish hoặc Khu Rèn shell nếu tiếp tục mở Linh Thành theo zone network.

## Khu Rèn shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_FORGE_SHELL_RUNTIME_READY`: Linh Thành có thêm Khu Rèn shell local-only trong catalog/scene/runtime manifest. `ForgeShell: district=forge` ghi `anvil-row=preview-only`, `craft-board=locked`, `forge-glow=local-preview`, `safe-no-crafting-backend`, `safe-no-upgrade-economy`, `safe-local-no-backend`; scene có workshop/anvil/forge glow/craft board locked ở hậu cảnh, không mở crafting backend, upgrade economy hoặc item mutation. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=112 passed=111 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap/collision polish hoặc Khu Bang Hội shell khi cần mở trục cộng đồng.

## Khu Bang Hội shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_GUILD_SHELL_RUNTIME_READY`: Linh Thành có thêm Khu Bang Hội shell local-only trong catalog/scene/runtime manifest. `GuildShell: district=guild` ghi `guild-hall=preview-only`, `guild-banner=local-preview`, `notice-board=locked`, `safe-no-guild-backend`, `safe-no-membership-backend`, `safe-local-no-backend`; scene có guild hall/banner/notice board locked ở hậu cảnh phải, không mở guild backend, membership, chat hoặc guild progression. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=114 passed=113 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap/collision polish hoặc gate/harbor shell để mở travel affordance local-only.


## Cảng Linh Thuyền shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_HARBOR_SHELL_RUNTIME_READY`: Linh Thành có thêm Cảng Linh Thuyền shell local-only trong catalog/scene/runtime manifest. `HarborShell: district=harbor` ghi `spirit-boat=preview-only`, `travel-board=locked`, `dock-lantern=local-preview`, `safe-no-travel-backend`, `safe-no-teleport-backend`, `safe-local-no-backend`; scene có dock/spirit boat/travel board locked ở vùng thấp, không mở travel backend, teleport hoặc world-route execution. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=116 passed=115 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap/collision polish hoặc gate shell để mở travel affordance local-only.


## Đông Môn authored detail pass runtime checkpoint — 2026-09-10

`LGO_DONG_MON_AUTHORED_DETAIL_PASS_READY`: Đông Môn có thêm authored detail pass bám route/collision thay vì trang trí ngẫu nhiên. Catalog/controller/visual manifest expose `runtimeDongMonAuthoredPassSnapshot` với `DongMonAuthoredPass`, `route-segments=5`, `collision-bands=5`, `detail-density=readable`, `collision-boundaries=from-bands`, `foreground-fringe=controlled`, `landmark-silhouettes=anchored`, `no-random-decoration`, `safe-no-procedural-spam`; runtime thêm moss/stone-step/bridge-rope/dash-dust/slime-rune nhỏ theo chunk và collision band để cải thiện hình ảnh mà không che HUD/player/combat. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=118 passed=117 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Tilemap asset/palette thật hoặc gate shell local-only khi không đụng luồng class/art song song.


## Đông Môn tile palette runtime checkpoint — 2026-09-10

`LGO_DONG_MON_TILE_PALETTE_RUNTIME_READY`: Đông Môn có runtime tile palette contract cho 6 tile chính, chuẩn bị thay procedural chunk bằng authored Tilemap asset/palette thật mà không đổi gameplay flow. Catalog/controller/visual manifest expose `runtimeDongMonTilePaletteSnapshot` với `DongMonTilePalette`, `tile_ground_grass:earth-green:soft-grass-edge`, `tile_ground_stone:spirit-cyan:stone-step-line`, `tile_platform_wood:warm-wood:rope-rail`, `tile_gap_marker:void-shadow:negative-space`, `tile_dash_lane:spirit-cyan:wind-streak`, `tile_slime_arena:violet-corruption:rune-boundary`, `safe-no-source-image`, `safe-runtime-palette-contract`; runtime thêm dải swatch thấp để kiểm màu/role mà không che HUD/player/combat. Evidence: RED Unity compile fail đúng vì thiếu palette snapshot, GREEN Unity EditMode `total=120 passed=119 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là chuyển palette contract này thành authored Tilemap asset/palette Unity thật hoặc gate shell local-only khi không đụng luồng class/art song song.
## Đông Môn authored source asset runtime checkpoint — 2026-09-10

`LGO_DONG_MON_AUTHORED_SOURCE_ASSET_READY`: Đông Môn có authored runtime source asset `client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/DongMonTilePalette.json` được đóng gói qua Unity Resources và load trong Editor/Player bằng `TwoDMapDesignCatalog.LoadDongMonTilePaletteSourceSnapshot()`. Runtime manifest expose `runtimeDongMonTilePaletteSourceSnapshot` với `DongMonTilePaletteSource`, `resource=LGOMaps/DongMonTilePalette`, `tile_dash_lane=True`, `safe-no-source-image=True`, `safe-runtime-resource=True`; scene beat ghi nguồn palette để future Tilemap asset/palette thay thế không đổi gameplay flow. Evidence: RED Unity compile fail đúng vì thiếu source snapshot API, GREEN Unity EditMode `total=122 passed=121 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là parse JSON này thành authored chunk placement hoặc tạo Unity Tilemap/Tile palette asset thật khi không đụng luồng class/art song song.
## Đông Môn authored chunk placement runtime checkpoint — 2026-09-10

`LGO_DONG_MON_AUTHORED_CHUNK_PLACEMENT_READY`: Đông Môn đã có authored runtime chunk placement source `client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/DongMonChunkPlacement.json`. Runtime load data bằng `TwoDMapDesignCatalog.LoadDongMonAuthoredChunkPlacements()` và render các chunk tile từ Resource thay vì giữ toàn bộ vị trí/count hardcoded trong scene builder. Visual manifest expose `runtimeDongMonChunkPlacementSourceSnapshot` với `DongMonChunkPlacementSource`, `resource=LGOMaps/DongMonChunkPlacement`, `chunk_gate_entry@-3.70,-2.02x4`, `chunk_dash_lane@1.82,-1.02x4`, `chunk_slime_arena@3.25,-1.36x2`, `authored-placement=True`, `safe-runtime-resource=True`, `safe-no-3d=True`. Evidence: RED Unity compile fail đúng vì thiếu chunk placement API/snapshot, GREEN Unity EditMode `total=124 passed=123 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là tách placement này thành Tilemap/Tile palette asset thật hoặc thêm label/layout pass cho Đông Môn/Quảng Trường khi không đụng luồng class/art song song.

## Đông Môn route label rail checkpoint — 2026-09-10

`LGO_DONG_MON_ROUTE_LABEL_RAIL_READY`: Đông Môn có readability pass player-visible cho route label: world-space collision labels `JUMP GAP`/`DASH LANE` được giảm thành chip `03`/`04`, scene thêm rail route với `01 Cổng`, `02 Bia`, `03 Jump`, `04 Dash`, `05 Slime`, và manifest expose `runtimeDongMonReadabilitySnapshot` với `mode=route-label-rail`, `world-label-density=reduced`, `chips=gate,stone,jump,dash,slime`, `avoids-hud-overlap`, `safe-local-no-backend`. Evidence: RED Unity compile fail đúng vì thiếu `RuntimeDongMonReadabilitySnapshot`, GREEN Unity EditMode `total=125 passed=124 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, 2D Player visual capture PASS 14 frame không dùng `-nographics`, runtime smoke matrix PASS và visual evidence matrix PASS. Visual vẫn là blockout/prototype art, chưa production-complete; next safe map action là chuyển placement/palette thành Unity Tilemap asset thật hoặc tiếp tục polish layout Đông Môn/Linh Thành.

## Đông Môn Unity Tilemap runtime checkpoint — 2026-09-10

`LGO_DONG_MON_UNITY_TILEMAP_RUNTIME_READY`: Đông Môn đã có Unity `Grid` + `Tilemap` + `TilemapRenderer` runtime layer thật, lấy cell từ `Resources/LGOMaps/DongMonChunkPlacement.json` và palette role từ `Resources/LGOMaps/DongMonTilePalette.json`. Visual layer này đang là underlay mảnh dưới procedural strip để chứng minh pipeline Tilemap sạch trước khi thay toàn bộ blockout art; manifest expose `runtimeDongMonUnityTilemapSnapshot` với `DongMonUnityTilemap`, `renderer=TilemapRenderer`, `grid=Grid`, `source=LGOMaps/DongMonChunkPlacement`, `cells=16`, `safe-no-source-image`, `safe-no-3d`. Evidence: RED Unity compile fail đúng vì thiếu `RuntimeDongMonUnityTilemapSnapshot`, GREEN Unity EditMode `total=126 passed=125 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, 2D Player visual capture PASS 14 frame không dùng `-nographics`, runtime smoke matrix PASS và visual evidence matrix PASS. Visual vẫn là blockout/prototype art; next safe map action là chuyển dần procedural strip sang Tilemap/atlas đẹp hơn hoặc polish Linh Thành layout, không mở shop/economy/teleport/backend.
## Quảng Trường layout anchor checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_LAYOUT_ANCHORS_READY`: Quảng Trường sau unlock Đông Môn có runtime `PlazaHubLayout` gồm 5 anchor social-spawn/event-board/gate-guide/merchant-preview/guild-locked, được scene render local-only và visual manifest/matrix bắt token. Next map-safe action: mở inspect detail cho từng anchor bằng UI local-only hoặc tiếp tục tile/atlas production khi có asset sạch; không mở event/shop/guild/backend.
## Quảng Trường anchor detail checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_ANCHOR_DETAIL_READY`: Quảng Trường sau unlock có `runtimePlazaHubDetailSnapshot` và dòng detail world-space cho target đang chọn, gồm merchant preview `try-before-shop` local-only. Next map-safe action: polish readability/spacing Plaza hoặc chuyển asset sạch sang atlas/tile khi có gate ingest riêng; không mở shop/event/guild/backend.
## Checkpoint — LGO_LINHTHANH_DISTRICT_PREVIEW_READY — 2026-09-10

Đã thêm preview rail chọn khu Linh Thành sau unlock Đông Môn: Học Viện là target đầu, route `plaza->academy`, control `M select-district`, frame visual `15-district-preview-rail`. Next map-safe action: polish readability/density của Linh Thành shell hoặc mở district preview chi tiết tiếp theo local-only; không mở teleport/shop/skill/crafting/guild/travel backend nếu chưa có gate riêng.
## Checkpoint — LGO_LINHTHANH_DISTRICT_DETAIL_READY — 2026-09-10

Đã thêm district detail snapshot và visual frame `16-district-market-preview`: rail Linh Thành cycle từ Học Viện sang Thương Phố, giữ local-only và không mở shop/trade/economy. Next map-safe action: polish density/visual label cho hub shell hoặc thêm detail preview cho Đền Linh/Khu Rèn/Guild/Harbor theo cùng guard, vẫn không mở backend.
## Đền Linh district preview checkpoint — 2026-09-10

`LGO_LINHTHANH_SPIRIT_TEMPLE_PREVIEW_READY`: Linh Thành district preview rail đã có frame Player riêng cho Đền Linh sau cycle Học Viện → Thương Phố → Đền Linh. Runtime manifest giữ `selected=spirit-temple`, `label=Đền Linh`, `route=plaza->spirit-temple`, detail `altar-local-only`, next gate `quest-buff-gate`, guard `safe-no-buff-backend`/`safe-no-district-backend`. Evidence: Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS, Player visual capture PASS 17 frame, visual matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là polish visual/readability Linh Thành hoặc chuyển thêm Đông Môn procedural strip sang Tilemap/atlas sạch.

## Linh Thành district rail coverage checkpoint — 2026-09-10

`LGO_LINHTHANH_DISTRICT_RAIL_COVERAGE_READY`: district preview rail Linh Thành đã có Player visual frames cho Khu Rèn, Khu Bang Hội và Cảng Linh Thuyền sau các frame Học Viện/Thương Phố/Đền Linh. Manifest cuối ở `selected=harbor`, route `plaza->harbor`, detail `spirit-boat-locked`, next `world-route-gate`, guard `safe-no-travel-backend`/`safe-no-teleport-backend`/`safe-no-district-backend`. Evidence: Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS, Player visual capture PASS 20 frame, visual matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là polish visual density/layout Linh Thành hoặc nâng Đông Môn Tilemap/atlas sạch.

## Linh Thành district rail readability checkpoint — 2026-09-10

`LGO_LINHTHANH_DISTRICT_RAIL_READABILITY_READY`: district preview rail đã polish callout để label/backplate đi theo node đang chọn, tăng size nhẹ và expose `runtimeLinhThanhDistrictReadabilitySnapshot` với `label-follows-selected=True`, `backplate=follows-selected`, `callout-size=readable`, `avoids-hud-overlap`. Evidence: Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS, Player visual capture PASS 20 frame và visual review frame Khu Rèn/Bang Hội/Cảng. Map tổng thể vẫn chưa production-complete; next safe map action là polish minimap/district rail density hoặc nâng Đông Môn Tilemap/atlas sạch.

## Next after minimap compact readability — 2026-09-10

`LGO_MINIMAP_COMPACT_READABILITY_READY`: world/minimap overlay đã rút route text, dùng district chips ngắn `HV TP ĐL KR BH CẢ`, world links dạng ngắn và manifest bắt `runtimeMinimapReadabilitySnapshot`. Map chưa xong production A-Z; nền tảng hiện có world map/Linh Thành/Đông Môn/district preview chạy trong Player. Next map-safe action: nâng Đông Môn authored Tilemap/atlas sạch hoặc mở layout chi tiết tiếp theo cho một khu Linh Thành theo local-only gate; không mở teleport/travel/shop/guild/backend.

## Next after Đông Môn authored detail resource — 2026-09-10

`LGO_DONG_MON_AUTHORED_DETAILS_RESOURCE_READY`: Đông Môn đã có resource detail pass riêng `DongMonAuthoredDetails.json` cho moss/step/rope/spirit-dust/rune, runtime đọc resource và manifest/matrix bắt snapshot. Next map-safe action: chuyển các props/NPC blockout tiếp theo sang atlas/sprite pipeline sạch hoặc nâng một khu Linh Thành detail layout bằng resource tương tự; không quay lại hardcode/dán tay, không mở backend.

## Next after Gate Keeper NPC sprite source — 2026-09-10

`LGO_DONG_MON_GATEKEEPER_NPC_SPRITE_SOURCE_READY`: Người Giữ Cổng đã có resource sprite-part riêng `DongMonNpcSprites.json` và runtime render từ slot/anchor. Next map-safe action: áp cùng pattern cho Training Stone/Shadow Slime/NPC hub hoặc thay slot bằng atlas sprite thật khi art sạch sẵn sàng; không mở backend/social/shop.
- LGO_2D_MACOS_PLAYER_CWD_CAPTURE_LESSON: khi chạy macOS Player evidence cho 2D onboarding, dùng `tools/capture_lgo_2d_onboarding_visual.py` hoặc launch executable từ `Contents/MacOS`; chạy trực tiếp từ repo root có thể init engine rồi thoát trước khi `GameBootstrap` ghi manifest/screenshot.
- LGO_2D_PRODUCTION_WORKFLOW_RESEARCH_READY: đã ghi `docs/execution/LGO-2D-PRODUCTION-WORKFLOW-RESEARCH-v0.1.md`; batch sau ưu tiên pipeline Tilemap/Sprite Atlas/paper-doll, dùng `tools/capture_lgo_2d_onboarding_visual.py`, không polish bằng rectangle primitive kéo dài.
## Đông Môn interaction marker + PC grounding checkpoint — 2026-09-10

`LGO_DONG_MON_PC_GROUNDING_MARKERS_READY`: Đông Môn map đầu có thêm runtime resource `DongMonInteractionMarkers.json` cho 5 mốc route `gatekeeper -> training-stone -> jump -> dash -> shadow-slime`, render marker/label trực tiếp trong Player để kiểm điểm tương tác. Runtime manifest/matrix bắt `runtimeDongMonInteractionMarkerSourceSnapshot` và `runtimeDongMonPlayerSceneFitSnapshot`; PC được kiểm theo `footAnchor=bottom-center`, contact shadow, ground band, sort order và route anchors để batch class tiếp theo ghép nhân vật/đồ/motion trên cùng hệ tọa độ, không chỉnh tay theo ảnh chụp. Evidence: Unity EditMode `total=143 passed=142 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS. Visual vẫn là blockout kỹ thuật, chưa phải art giống concept; next action là làm một class đầu tiên có base/paper-doll/tách đồ/thay đồ/motion/skill trong Player thật trước khi nhân rộng sang 5 class.
## Võ Lv1 one-class runtime slice checkpoint — 2026-09-10

`LGO_VO_LV1_ONE_CLASS_RUNTIME_SLICE_READY`: Class đầu tiên để kiểm chứng workflow là Võ Lv1, chưa nhân rộng sang 5 class. Thêm runtime resource `LGOClasses/VoLv1ClassSlice.json` mô tả base nam/nữ, 5 paper-doll slots, anchor/fit profile, motion Idle/Walk/Jump/Dash/ClassSkill và skill `vo_lv1_first_skill` đánh `shadow-slime`. Controller expose `runtimeVoLv1ClassSliceSnapshot` trong visual manifest tại frame `08-complete` trước khi thử đồ inventory, nên snapshot giữ loadout Võ `top_vo_lv1_male`, motion `TrainingCompletePose` và `runtimeSkillCue=True`; inventory applied vẫn là snapshot riêng để kiểm thử thay đồ. Runtime scene có cue `VÕ Q`/trail nhỏ để owner thấy skill class đầu tiên đã đi qua Player thật, nhưng đây vẫn là cue blockout, chưa phải VFX production. Evidence: Unity EditMode `total=145 passed=144 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS. Next action: thay primitive cue/slot màu bằng sprite atlas/paper-doll asset sạch cho Võ trước, rồi mới nhân pattern sang Kiếm/Pháp/Cơ/Linh.

`LGO_VO_LV1_PAPERDOLL_MOTION_POSE_READY`: Võ Lv1 paper-doll atlas đã có `poseOffsets` data-driven cho `vo_idle`, `vo_jump_lift`, `vo_dash_stretch`, `vo_skill_cast`, `vo_lv1_training_complete`; controller áp pose theo animation intent và expose `currentPose` trong `runtimeVoLv1PaperDollAtlasSnapshot`. Overlay Võ hiện từ lesson Bia Luyện Khí để Player capture kiểm được đồ đi theo Jump/Dash/Skill/Complete, skill cue chỉ bật lúc cast/complete. Đây vẫn là primitive/blockout atlas để kiểm chứng anchor/pivot/motion và thay đồ runtime, chưa phải art production giống concept. Evidence: Unity EditMode `total=149 passed=148 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS, frozen diff audit PASS. Next action sau checkpoint: thay primitive atlas Võ bằng bitmap/spritesheet approved hoặc nâng locomotion frame sampling trước khi nhân pattern sang Kiếm/Pháp/Cơ/Linh.
> **Checkpoint CLASS-VO-01A — motion Võ Lv1 hai giới — 2026-09-10:** nguồn mới được tạo theo đúng identity Võ đã audit, tách thành hai sheet 4×2, gate tự động loại sheet chạm ranh giới ô và chuẩn hóa alpha/margin theo cả batch. Runtime hiện có 14 frame cho mỗi giới: idle/walk/dash/punch legacy cùng run/jump/basic attack/`Liên Quyền`; HUD có nút touch và phím Shift/J/Z/X. Sáu atlas 1024² tổng 700.563 byte, tăng khoảng 57 KB dù thêm 16 frame. EditMode `180/179/0/1`; build macOS 166.998.867 byte; capture `build/vo-motion-v8/three-profiles/` có 38 frame/profile và đã review, không chồng action bar hoặc che portal.
>
> **Next:** tiếp tục `CLASS-VO-01B` trong cùng worktree: làm attachment/rig cho 10 slot để trang bị tùy chọn đi đúng pose khi run/jump/basic/`Liên Quyền`, ưu tiên Lv1 làm proof rồi áp metadata chung cho Lv10/20/30. Không mở class thứ hai. Trạng thái hiện tại là `VO_LV1_FULL_FRAME_MOTION_PASS / ANIMATED_PAPER_DOLL_ATTACHMENT_INCOMPLETE`; full-frame đã chạy thật nhưng modular/tier cao vẫn chỉ dùng root pose.

> **Checkpoint CLASS-VO-01B1 — base skeletal rig — 2026-09-10:** hai sheet rig nam/nữ 5×2 được tạo từ identity Võ đã tuyển, tách theo connected component thành 20 segment head/torso/arms/legs, đóng thêm một atlas 1024² 24.232 byte. Manifest v6 có 120 rig-pose profile và 120 equipment-attachment profile dùng chung Lv1/10/20/30. Runtime modular thay base phẳng bằng 10 segment, xoay quanh joint pivot cho idle/walk/run/jump/basic/`Liên Quyền`; capture hiện tại `build/vo-rig-v2/three-profiles/` có 46 ảnh/profile.
>
> **Next:** `CLASS-VO-01B2` tách các slot nhiều component (`arm_guard`, `boots`, và phần trang bị hai bên) rồi bind component vào hand/foot/thigh/torso bone. Review B1 xác nhận khớp thân đã chuyển động, nhưng slot đôi còn là sprite chung và Võ nữ Lv30 chưa khớp trang bị đủ sạch; không claim paper-doll hoàn chỉnh và chưa mở class thứ hai.
## Sau checkpoint Võ v9 — shared character/action base

Tách loadout, rig pose và action orchestration khỏi `CongDongLamMap01AArtPreview` sang runtime base dùng chung, không đổi hình ảnh/output đã capture. Giữ manifest contract tương đương Sprite Library `Category + Label`; chưa thêm package Unity mới. Sau extraction, build/capture lại cùng ma trận 66×3 để chứng minh không regression rồi mới bắt đầu class thứ hai. Asset loading về sau chia theo map/class/tier residency, không load toàn bộ 5 class hoặc tạo bộ atlas riêng cho ba profile.

## Sau shared character runtime state — Kiếm Lv1–30 source batch

`SHARED_CHARACTER_RUNTIME_STATE_PLAYER_PASS`: state mode/gender/level/loadout/action đã rời Map01A vào `TwoDCharacterRuntimeState`; Player v22 và capture 66×3 giữ nguyên Võ/Q01–Q09, manifest có `voSharedRuntimeStateVerified=True`. Next: audit toàn bộ source Kiếm đã tuyển trong `LGO-Selected-2D-Source-v1` và backup sandbox chuẩn hóa cũ, chọn một identity nam/nữ đồng nhất, lập một crop/attachment plan đủ Lv1/10/20/30 × 10 slot rồi mới pack/nối base chung trong một batch. Không tạo lại từng item, không mở Pháp/Cơ/Linh trước khi Kiếm có Player evidence.

## Sau Kiếm source-v4 — base-fit/redraw gate trước atlas

Đã tách một lượt 80 ô thành source-v4 với đúng 10 slot canonical, nhưng không mặc định crop là item sạch: 12 crop nữ đã xác định `redraw-required`, 68 còn lại `candidate`, runtime eligible bằng 0. Next: dựng template canvas/attachment cho base nam/nữ, sửa 12 món dính body và fit theo lô; sau đó chạy mixed-level pairwise matrix qua idle/walk/run/jump/basic attack/class skill. Chỉ item `approved` mới được pack atlas 512–1024 và nối shared runtime state. Không mở Pháp/Cơ/Linh trước Player evidence Kiếm.

## Sau Kiếm side-view redraw source-v3 — SpriteSkin fit spike

Đã tạo lại theo base side-view hai sheet nam/nữ đủ 80 cell; nữ v1 lỗi dính body/tay được giữ làm evidence, nữ v2 đã sửa theo row, source-v3 đã crop/despill sạch theo batch. Tất cả vẫn candidate. Next: cài `com.unity.2d.animation` 13.x trong một spike có benchmark, weight một bộ mixed-level tối thiểu `Lv1 weapon + Lv30 hair + Lv10 inner + Lv20 outer` lên skeleton chung nam và nữ, chạy sáu motion state và capture ba profile. Chỉ giữ package nếu compile/build-size/runtime evidence đạt; nếu SpriteSkin không phù hợp thì quay về split component từ cùng source-v3, không sinh lại art.

## Sau Sprite Library compatibility spike — weight/fit batch đầu

Package 13.0.0 compile/test PASS và Player delta khoảng 0,188%, nên giữ. Adapter đã chứng minh loadout `Lv1 weapon + Lv30 hair + Lv10 inner + Lv20 outer` resolve độc lập qua Category/Label và chặn Skinned sprite chưa có bones. Next: fit cùng bốn món trên template nam/nữ, để weapon là Rigid, weight hair/inner/outer theo skeleton, kiểm occlusion và sáu state `idle/walk/run/jump/basic_attack/class_skill`, rồi mới nâng đúng các item đã pass lên approved và capture ba profile. Chưa import cả 80 candidate, chưa mở Pháp/Cơ/Linh.

## Sau mixed-loadout idle prefit — import/weight/motion

Prefit đã sửa hair/outer multi-view, normalize đúng PPU 208 và ghép idle nam/nữ thành công; weapon/inner được kế thừa. Next: import đúng 8 file của proof vào draft runtime pack, weight hair/inner/outer trên skeleton chung và giữ weapon Rigid; đăng ký qua Sprite Library, nối một opt-in Kiếm fit preview, chạy/capture sáu motion state trên mobile/tablet/PC. Chỉ các item qua motion/occlusion mới đổi thành `approved`; toàn bộ 72 candidate còn lại vẫn ở ngoài runtime.

## Sau Kiếm proof asset authoring — opt-in Player motion preview

8 proof item đã vào một draft atlas 512×512 ở PPU 208; 6 Skinned có bone/weight thật, 2 weapon Rigid. Atlas giữ nguyên cell đã normalize và guard khóa hash/provenance. Next: dựng component preview dùng base/rig chung, Sprite Library mixed loadout và SpriteSkin transforms; expose gender + `idle/walk/run/jump/basic_attack/class_skill`, rồi mới build/capture một lượt ba profile và review occlusion/grounding. Chưa nâng `approved` trước evidence này.

## Sau Kiếm shared-rig fit preview — hoàn thiện theo batch, không chỉnh từng món

Fit proof 4 slot đã chạy trong Player qua bone proxy và 78×3 capture; production vẫn chặn draft. Next: xử lý một lượt 6 slot Kiếm còn thiếu cho Lv1/10/20/30 nam/nữ từ source-v3, redraw theo cùng side-view base khi crop không phù hợp, pack theo atlas 512–1024 có byte budget; sau đó nối skill Kiếm Lv1 riêng và mới chạy lại full test/build/capture ba profile. Không nâng `approved` hoặc mở Pháp/Cơ/Linh trước khi đủ 10 slot, tháo/mặc và visual motion review.

48 crop của 6 slot còn thiếu đã được pack thành hai atlas candidate ngoài runtime tại `generated-batch-v2/remaining-six-slot-batch-v1/`, không resize và eligible 0. Next cụ thể: chọn `Lv1 lower_body + Lv10 waist_belt + Lv20 arm_guard + Lv30 footwear + Lv20 shoulder_chest_guard + Lv30 class_accessory` cho cả nam/nữ; ghép contact 10-slot với proof hiện tại, sửa lệch theo base rồi author bone proxy trái/phải. Chỉ sau contact pass mới import hai atlas fit đã chọn.
## Lịch sử — khóa Lv1 + hành trang trực tiếp, sau đó author Lv10 HD — 2026-09-11

Hoàn tất gate và commit một checkpoint gồm Lv1 HD đã được owner chấp nhận sơ bộ cùng panel Hành trang 10 món có thể chọn/tháo/mặc bằng chuột. Visual gate phải xác nhận lưới hai cột không cắt hàng, item `accessory` không bị nhãn POSE THỬ che và thao tác đổi đúng state/render trên một actor.

Sau checkpoint, author Võ nam Lv10 theo cả sáu pose trong một batch, bám progression/grid canonical và tái dùng nguyên body atlas/hash, canvas 1024×1536, pivot, scale, camera, pose keys và slot order của Lv1. Nạp Lv1 + Lv10 vào cùng Player/Hành trang; kiểm full Lv10, tháo 10 slot, và loadout phối chéo đại diện. Slot nào lệch/hở/mờ phải sửa source component theo lô trước capture; không thêm offset/controller/camera riêng. Chỉ mở class kế tiếp sau khi Võ Lv10 và mặc chéo qua visual gate.
## Lịch sử — Võ đã khóa; Kiếm dừng tại source gate — 2026-09-11

Checkpoint Võ Lv1/Lv10 HD và hành trang 10 slot vẫn là bản Player hiện hành ở mục kế tiếp; commit `21a7415a` và `6d1a3018` đã push lên `origin/feature/2d`. Audit design-first Kiếm dùng đúng turnaround nam Lv1 và grid 10 slot. Pack Kiếm cũ đủ material nhưng runtime chỉ là proof 4 slot `DRAFT_RUNTIME_FIT`, không được nối vào hệ hành trang hiện hành.

Batch Kiếm 30 phút đã thử hai hướng source và **reject cả hai trước runtime**: donor toàn thân không khớp ranh giới ownership Võ; mười donor module độc lập sinh nền checker/trùng chi khi ép vào mask cũ. Evidence lỗi giữ ngoài repository tại `class-work-in-progress/kiem-lv001/ten-slot-pose-authoring-v1/registered-surface-lv001-hd-v1..v3`; tuyệt đối không pack/import các bản này. Bước hợp lệ tiếp theo là redraw Kiếm trực tiếp trên clean common-body pose template, tạo alpha riêng cho từng slot/chi và kiểm contact sheet full + tháo từng món trước khi build. Không đổi camera/base/scale và không lấy trang phục Võ làm alpha authority cho class khác.
## Lịch sử — Kiếm Lv1 nam/nữ đã qua source và Player visual gate — 2026-09-12

Không mở lại hoặc thay pipeline Võ; Kiếm kế thừa đúng body/motion authority đã chốt: nam dùng Võ v3 div4, nữ dùng common female sáu pose. Hai source ngoài repo là `class-work-in-progress/kiem-lv001/ten-slot-pose-authoring-v2/registered-surface-lv001-hd-v2` và `class-work-in-progress/kiem-lv001/female-ten-slot-pose-authoring-v2/registered-surface-lv001-hd-v1`. Mỗi giới có 10 slot × sáu pose; kiếm ở sorting order 23 phía sau body, các overlay div2 và body atlas giữ byte-identical.

Evidence Player `build/kiem-source-pose-review-v1/runtime-pc-both/pc`: 186 frame, nạp đủ 20 item, thực thi `idle/run_contact_a/run_a/run_contact_b/run_b/jump_tuck`, `errors=[]`. Board đã xem tại `kiem-lv1-male-female-actor-review-board.png`: một actor, đủ bốn nhịp chạy, lộn liền hình, tháo kiếm/áo ngoài không để mảnh nổi. Trạng thái `REVIEW_ONLY / AGENT_VISUAL_PASS`, chưa claim owner hoặc production approval.

Action tiếp theo: author Kiếm Lv10 nam/nữ từ design progression gốc trên chính hai body/canvas hiện hành, rồi nạp Lv1+Lv10 để kiểm full-set, tháo từng slot và mixed-level trên Player. Không đổi camera/base/scale, không dùng static-fit đã thu hồi, không tạo controller/actor thứ hai; sau Kiếm Lv10 mới sang Pháp.
## Lịch sử — FIX_REQUIRED sau audit đúng actor và semantic slot — 2026-09-12

Không dùng các kết luận `AGENT_VISUAL_PASS` cũ để nghiệm thu. Capture cũ đo nhầm bounds của registered actor ẩn; đường capture đã được sửa để ghi actor source-pose, pose/frame, bounds, root scale và rotation theo từng ảnh. Oracle đúng hiện nằm ở `build/kiem-source-pose-metric-audit-v2/pc/registered-manifest.json`; root scale giữ 1 nên không chỉnh camera/base/scale để xử lý cảm nhận nhân vật lớn khi nhảy.

Tám pack fail đã được thay bằng source/pack `semantic-v3`; audit tổng mới `build/source-pose-design-audit-v2/semantic-v3-all-classes.json` pass 16/16. Player hiện hành `build/source-pose-semantic-v4-player/LinhGioiOnline.app` có debounce đổi class; evidence cuối là Kiếm `runtime-v1`, Pháp/Cơ `runtime-v3`, Linh `runtime-v2`, mỗi bộ có 190 frame đúng actor, đúng bốn nhịp riêng, root scale 1, Lv1/Lv10/mixed và 32 tổ hợp wardrobe.

Action tiếp theo là owner kiểm trực tiếp Player ở cửa sổ lớn: `F` đổi class, `I` mở hành trang, đổi giới/cấp và tháo từng slot khi đứng/chạy/lộn. Nếu một vùng ownership hoặc anatomy còn sai, sửa lại source semantic-v3 theo cả sáu pose rồi capture lại đúng class; không căn camera/scale/offset. Chưa có pack Võ nữ thì không ghép giả hoặc mở presentation thứ hai. Sau owner gate mới áp cùng contract sang tier tiếp theo.
