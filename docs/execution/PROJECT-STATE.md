## Hiện hành — reject atlas Võ nhiều level bị mờ/lệch; dựng lại Võ Lv1 HD — 2026-09-11

Owner đã reject trực tiếp Player `build/vo-pose-pairwise-player-v1/LinhGioiOnline.app`: trang bị mờ, layer ghép sai và không đọc được hình dáng. Các pack `ten-slot-pose-authoring-v1` Lv1–Lv100 chỉ chứng minh loader/slot/level hoạt động; **không còn là visual candidate** và không được dùng để mở class/level tiếp theo. Nguyên nhân đã xác nhận: nguồn item nhỏ và material-transfer bị đẩy qua mask chồng lớp rồi sampling div4; validator kỹ thuật không đo chất lượng mỹ thuật.

Đã quay lại design gốc và attachment sheet sắc nét, dựng đồng thời sáu source pose Lv1 trên canvas 1024×1536. Bộ mới `ten-slot-pose-authoring-v2/registered-surface-lv001-hd-v2` giữ nguyên 60/60 alpha registration của geometry v3; RGB chỉ lấy từ donor cùng action ở pixel thực sự thuộc slot trên cùng, các underlayer giữ source riêng để tháo đồ không lộ mảng donor trùng. Board source `six-pose-full-compose.jpg` và `idle-ten-slot-toggle-review.png` đã được xem; một actor, đúng idle + bốn nhịp + tuck, hình Lv1 rõ hơn và đủ 10 trạng thái tháo.

Pack mới `legacy-base-run-contact-jump-v3-div4-ten-slot-lv001-hd-review-v2` giữ nguyên body atlas/manifest v3 div4 và dùng overlay div2; round-trip atlas 60/60, alpha bất biến 60/60. Runtime chỉ mở rộng loader cho overlay divisor 1/2/4, không đổi body, camera, scale, pivot hoặc timeline. Unity SourcePose `12/12` gồm test divisor2 trên body div4; Player build `build/vo-lv1-hd-player-v1/LinhGioiOnline.app` thành công 171950122 byte, 0 error/13 warning; capture `build/vo-lv1-hd-runtime-v1/pc` đạt 154 frame, 40 toggle, `errors=[]`. Board runtime `lv1-hd-runtime-review.jpg` đã được xem; vẫn giữ `REVIEW_ONLY` cho tới phản hồi owner trên Player đang mở.

## Hiện hành — Võ đủ progression Lv1–Lv100 trên cùng actor/pose contract — 2026-09-11

Đã audit lại design gốc: `detail/15-male-equipment-grid-redraw-source.png` có đúng 10 slot × 11 mốc Lv1/10/20/30/40/50/60/70/80/90/100; `detail/11-male-outfit-progression-redraw-source.png` có silhouette trước/sau cùng 11 mốc. Vì vậy không sinh một hệ level khác. Tool external tách 110 donor theo cell cố định và hash source; background removal chỉ tạo donor, không tự cấp runtime status.

Lv40–Lv100 được author theo batch trên sáu pose v3. Pixel body gốc, pose, pivot, camera và scale không đổi. Surface trong body dùng mask slot hiện hành với material đúng cột level; phần silhouette tóc/giáp vai/vạt áo chỉ vào component slot tương ứng. Ba sheet Lv40/Lv70/Lv100 do image generation tạo được giữ làm `SOURCE_STUDY_ONLY` để định hình chuyển cấp; không dùng full character thay body. Pose lộn loại bỏ silhouette transfer vì visual zoom cho thấy viền rối, quay về exact registered surface của tuck. Tất cả pack vẫn `REVIEW_ONLY`.

Player mới `build/vo-pose-all-tier-player-v1/LinhGioiOnline.app` build `171950122` byte, 0 error/13 warning. Evidence `build/vo-pose-all-tier-runtime-v3/pc` nạp 11 full pack vào một actor: 165 frame, 40 toggle, 149 switch thực, `poseReviewFullLevelsVerified=[1,10,20,30,40,50,60,70,80,90,100]`, mixed verified, `errors=[]`. Capture lưu riêng 11 frame full-set `[2..12]`; board `full-level-player-review.jpg` cho phép so trực tiếp mọi tier trên cùng idle/camera/body. Provenance giữ 11 fingerprint pack, mỗi pack 22 file, và log có 10 đường variant. Đã xem thêm Lv20 walk, Lv10 bốn nhịp run, mixed jump và Lv100 jump-diagonal. Lv100 tuck sau cleanup giữ đúng whole-pose v3, không còn silhouette extension chưa đủ ổn.

Kiến trúc dài hạn đã đối chiếu với runtime skin/attachment: animation giữ slot placeholder ổn định, mỗi item có thể gồm nhiều component và runtime ghép item theo slot trên một skeleton/body. LGO hiện đi cùng hướng `slotId -> itemId -> pose components`, preflight body hash/fit family/level trước apply nguyên tử. Điều này tránh tạo full-outfit cho mọi tổ hợp và cho phép phối chéo level. Gate kỹ thuật toàn tier đã qua; art tier cao vẫn cần polish source component ở kích thước Player trước khi gọi production-final hoặc mở class khác.

## Hiện hành — Võ Lv1/10/20/30 dùng chung một wardrobe actor — 2026-09-11

`TwoDSourcePoseReview` hiện nạp lặp lại mọi pack level, giữ một body atlas/hash, `fitFamily=vo_male_v3`, pose ID, pivot, scale và camera. Danh sách tier đủ 10 slot được suy ra động; capture kiểm full-set từng tier rồi kiểm một loadout phối vòng theo slot. Cơ chế không hardcode số tier, nên Lv40–Lv100 sẽ đi cùng đường nạp/đổi/verify khi có source item hợp lệ, không thêm controller hay actor theo level.

Source review đã mở rộng theo lô từ cùng geometry v3 sang Lv20 và Lv30: external `ten-slot-pose-authoring-v1/registered-surface-lv020-v1`, `registered-surface-lv030-v1` và hai pack sibling `legacy-base-run-contact-jump-v3-div4-ten-slot-lv020-review-v1`, `...lv030-review-v1`. Mỗi tier đủ 10 slot × sáu pose; root body atlas/manifest giữ đúng hash owner đã chốt. Art vẫn `REVIEW_ONLY`; khác biệt vật liệu Lv20/Lv30 còn nhẹ và chưa được gọi là production-final.

Player `build/vo-pose-four-tier-player-v2/LinhGioiOnline.app` build thành công `171949610` byte, 0 error/13 warning. Capture `build/vo-pose-four-tier-runtime-v2/pc`: 154 frame, 40 toggle, `errors=[]`, full levels `[1,10,20,30]`, mixed bốn level, 75 lần đổi item thực và fingerprint đầy đủ `22×4` ổn định. Đã xem trực tiếp walk Lv20, bốn nhịp run Lv10, jump mixed và jump-diagonal Lv30: một actor, body/action v3, không đổi camera/base/scale. Scoped Unity: SourcePose `11/11`, Map preview `17/17`; Python capture/loadout `18/18`.

Hướng kỹ thuật được đối chiếu với mô hình skin/attachment phổ biến: animation tham chiếu slot ổn định, item có thể gồm nhiều attachment, runtime ghép item theo slot trên cùng skeleton. Vì vậy contract dài hạn vẫn là `slotId -> itemId -> pose components`, preflight body/fit/source registration trước apply nguyên tử; không dựng full-outfit riêng cho từng tổ hợp. Source canonical thực tế có đủ grid và front/back progression Lv1–Lv100; ghi chú giới hạn Lv30 trước đó đã được sửa. Batch kế tiếp dùng chính nguồn này để hoàn thiện component silhouette, không nội suy offset/scale runtime và chưa mở class khác.

## Hiện hành — đổi item Lv1/Lv10 trực tiếp trên actor POSE THỬ — 2026-09-11

`TwoDSourcePoseReview` hiện nạp đồng thời nhiều item variant của cùng slot theo `unlockLevel`, bắt buộc cùng body atlas/manifest và `fitFamily=vo_male_v3`. Mỗi variant vẫn dùng chung body renderer, frame ID, facing và whole-pose somersault root; đổi item chỉ tắt renderer cũ, bật renderer mới và giữ nguyên trạng thái slot đã tháo. `CycleVoAvatarLevel` đổi toàn bộ level chỉ qua các tier đủ 10 slot; `CycleVoSelectedEquipmentItemLevel` đổi riêng item đang chọn. UI/runtime vẫn một actor và dùng `_voEquipmentLevels` hiện có, không thêm hệ nhân vật hay camera/scale theo tier.

Scoped EditMode: `TwoDSourcePoseReviewTests 11/11`, gồm switch Lv1↔Lv10 và giữ unequipped; `DongMonIllustratedPreviewTests 17/17` ở lượt đúng namespace (một lượt filter sai chạy 0 test đã bị loại, không dùng làm evidence). Python capture/loadout `17/17`. Player mới `build/vo-pose-item-variants-player-v3/LinhGioiOnline.app` build thành công `171948586` byte, 0 error/0 warning.

Capture `build/vo-pose-item-variants-runtime-v3/pc` nạp full pack Lv1 và full pack Lv10 vào cùng actor: 154 frame, 40 toggle, `errors=[]`, fingerprint 22+22 file ổn định. Runtime thực hiện đúng 20 chuyển renderer cần thiết: bốn frame run dùng full Lv10; jump dùng năm slot Lv1 + năm slot Lv10; manifest xác nhận `poseReviewLv10Verified=true` và `poseReviewMixedVerified=true`. Đã xem frame 08 và 18: một nhân vật, bốn nhịp/lộn giữ pose v3; HUD cho thấy base Lv1 với item đang chọn Lv10 ở run và Lv1 trong mixed jump. Art vẫn `REVIEW_ONLY`.

Next: mở rộng cùng item-variant contract sang Lv20/Lv30 bằng source material hiện có, nhưng chỉ đăng ký trên geometry v3 đã dùng cho Lv1/Lv10. Sau đó chạy ma trận đại diện theo slot/tier trên Player mới; không cần dựng trước mọi tổ hợp Cartesian. Chưa mở class khác cho tới khi Võ Lv1/10/20/30 có loadout evidence và các item lỗi hình học đã quay lại source mask.

## Hiện hành — Võ 10 slot cùng level và phối chéo Lv1/Lv10 — 2026-09-11

Đã author theo lô đủ 10 slot Võ nam Lv1 trên đúng body/action `legacy-base-run-contact-jump-v3-div4`: garment lấy silhouette từ pixel pose đã chốt, material chỉ ghi trong mask; `main_weapon` và `class_accessory` dùng source-space anchor; `outer_top` giữ surface đã review. Không sửa sáu body PNG, pivot, scale, camera hoặc registered wardrobe WIP. Source compose sáu pose đã được xem tại external `ten-slot-pose-authoring-v1/registered-surface-v1/six-pose-full-compose.jpg`. Pack `legacy-base-run-contact-jump-v3-div4-ten-slot-review-v1` có đủ 10 thư mục slot, mỗi slot sáu pose div4 và metadata `itemId/fitFamily/unlockLevel`; vẫn là `REVIEW_ONLY`.

Player evidence mới `build/vo-lv1-ten-slot-surface-runtime-v3/pc`: 154 frame, 40 toggle, `errors=[]`, log đủ `idle/run_contact_a/run_a/run_contact_b/run_b/jump_tuck`, fingerprint đủ 22 file body + 10 slot trước/sau capture. Đã xem idle, bốn nhịp chạy, lộn và 10 trạng thái tháo: một actor, nhãn `10/10 slot`, không có nhân vật/hệ thứ hai. Đây là agent visual review của source-review art; không tự gán production hoặc owner approval.

Cùng geometry/anchor đó đã được áp cho 10 item Võ nam Lv10 từ source progression hiện có, không sinh base/action mới. Source board external `registered-surface-lv010-v1/six-pose-full-compose.jpg`; Player evidence `build/vo-lv10-ten-slot-surface-runtime-v2/pc` cũng đạt 154 frame/40 toggle/`errors=[]` và đủ 22 fingerprint. Lv10 hiện là material/surface review, giữ silhouette starter để các action không lệch; chưa phải art production cuối.

Đã thêm `tools/compose_lgo_pose_review_loadout.py` để dựng review pack nguyên tử theo `slotId -> itemId`. Tool bắt buộc đúng 10 canonical slot, chung body hash và `fitFamily=vo_male_v3`; lỗi một item thì xóa toàn output, không để loadout nửa chừng. Test mới `3/3`. Pack phối xen kẽ năm item Lv1 + năm item Lv10 chạy trên Player tại `build/vo-mixed-lv1-lv10-surface-runtime-v2/pc`: 154 frame/40 toggle/`errors=[]`, đủ bốn nhịp và lộn. Composer chỉ tạo evidence đã resolve; runtime production tiếp tục dùng `TwoDEquipmentCompatibilityCatalog` hiện có, không tạo hệ equip song song.

Next: nối atlas item theo pose vào catalog/runtime loadout hiện có để chuyển từng `itemId` trực tiếp thay vì đổi cả review directory; giữ atomic preflight theo body profile/skeleton/level. Sau đó mở rộng cùng authoring geometry sang Lv20/Lv30 theo batch, ưu tiên khác biệt cấu tạo/material nhưng không đổi tỷ lệ hoặc source motion. Chưa mở class khác trước khi ma trận Võ theo level và các trạng thái tháo/phối chéo có evidence đủ rõ. Map01A vẫn là target tổng thể.

## Hiện hành — sửa nguồn trang bị theo design gốc — 2026-09-11

Player hiện chỉ trình bày **một nhân vật**: body/motion v3 div4 đã chốt và layer `outer_top` mới dùng chung frame, facing, pivot, root lộn, base và scale. Registered wardrobe WIP vẫn được giữ nguyên state/code/source để tiếp tục khai thác, nhưng renderer của nó bị ẩn khi pose-review stack hoạt động; không còn hai nhân vật/hai hệ hiển thị song song. Test scoped cuối `28/28`; build `build/vo-single-stack-player-v1/LinhGioiOnline.app` thành công, `171941930` byte, 0 error/13 warning. Capture PC `build/vo-single-stack-runtime-v1/pc` đạt 154 frame, `errors=[]`; đã xem idle, bốn pha chạy, lộn và cặp bật/tắt `outer_tunic`, chỉ có một actor và tắt áo trả đúng body bên dưới. Đây là evidence review của agent, chưa tự gán toàn bộ wardrobe Lv1 PASS.

Loader single-stack sau đó đã được tổng quát cho đủ danh sách 10 slot và nhiều component của cùng một slot (ví dụ front/back), mỗi component vẫn lấy cùng pose ID/source registration và cùng root actor. Adapter ánh xạ tên runtime cũ sang slot chuẩn tại một chỗ. Test cuối: SourcePose `10/10`, equipment `7/7`, locomotion `12/12`; không có suite `executed=0`. Player `build/vo-ten-slot-loader-player-v1/LinhGioiOnline.app` build `171943978` byte, 0 error/13 warning; capture `build/vo-ten-slot-loader-runtime-v1/pc` 154 frame, 40 toggle, `errors=[]`. Đã xem lại idle/run/jump/tắt `outer_tunic`: một actor, nhãn `1/10 slot`, tắt áo trả đúng lớp dưới.

Owner chốt thứ tự mở rộng: hoàn thiện đủ **10 item/slot của cùng một level** trước; sau đó mới chuyển cùng cơ chế sang level khác. Phối item khác level vẫn phải khớp trên một body/action profile chung. Không tạo controller, skeleton, scale, offset hoặc atlas logic riêng theo level. Contract hiện hành dùng `slotId -> itemId`, `fitFamily`, component theo pose/view và apply nguyên tử; Lv1 là batch đầu, Lv10 là gate phối chéo đầu tiên, rồi mới nhân tiếp các mốc còn lại.

Đã tạo ba sheet thiết kế theo lô cho chín slot còn lại và một study underclothes tại external `class-work-in-progress/vo-lv001/ten-slot-pose-authoring-v1/raw-candidates`. Audit giữ trạng thái `SOURCE_STUDY_ONLY`: sheet tay/chân còn pixel da; hai sheet còn lại bake nền caro; study underclothes đổi anatomy/placement so với v3. Không asset nào trong bốn file này được nối runtime. Chúng chỉ cung cấp vật liệu/cấu tạo để đăng ký lại theo mask/khớp v3; `STATUS.md` ghi hash và lỗi, tránh dùng nhầm candidate gần đúng.

Owner bác hai sheet tách generic và yêu cầu xem kỹ design gốc. Đã mở turnaround03, equipment05, progression11 và grid15: board sáu pose gần đúng hành động nhưng đổi áo vạt chéo thành vest mở giữa, đổi giày/hoa văn và tự thêm băng trán/giáp vai. Không dùng board này làm chuẩn đồ. External `registered-equipment-authoring-v1/original-design-reference-v1` giữ reference nguyên pixel, SHA và brief sửa; hai sheet lỗi được giữ riêng, không runtime. Turnaround03 là chuẩn ngoại hình đề xuất; grid15 chỉ định ownership vì áo Lv1 trong hai ảnh mâu thuẫn. Chưa tự gán owner approval.

Đã vẽ lại áo trực tiếp theo sáu visible surface của v3, dùng turnaround03 giữ vạt chéo/viền/vải ngà/biểu tượng. External `original-design-reference-v1/pose-guide/registered-surface-candidate/six-pose-source-compose.png` đã review: run_b giữ vùng tay che, tuck dùng lưng/sườn thay mặt trước. Không warp/scale từng pose; source v3 không đổi. Pack áo rời `div4-overlay-review`: div4, atlas128×512, PNG55895byte; 10test pack đạt, 6/6sprite round-trip đúng sampled source. Mới outer_top, REVIEW_ONLY; chưa Player/wardrobe PASS. Còn mép cổ/eo và ownership sát nút đai, mặt khuất/hành động khác. Study bốn áo trước đó chỉ tham chiếu, không dùng tuck mặt trước.

Bước tích hợp overlay đã hoàn tất theo single-stack ở trên. Capture kiểm fingerprint base/overlay, đủ sáu pose và log bật/tắt áo; không truyền atlas áo làm pack body hoặc nới owner-source guard.

Next batch: chuyển đủ 10 slot Võ nam Lv1 theo design gốc thành component cùng pose/view, gom lỗi source theo cả lô rồi mới build/capture. Giữ body/action v3 div4 và tỷ lệ cũ. Compose/tháo từng món phải khớp trước Player; không quay lại warp đồ đứng hoặc sinh full-body mới. Giữ registered WIP. Chỉ sau full Lv1 mới mở Lv10 và ma trận phối chéo; chưa class khác hoặc wardrobe PASS.

Nghiên cứu dài hạn trong `docs/art/LGO-2D-EQUIPMENT-COMPATIBILITY-CONTRACT-v1.md` vẫn áp dụng: body/action chung, item nhiều component, hình riêng khi đổi mặt, chuẩn đường nối và layering. Registered hiện chỉ bật/tắt slot và level chung, chưa resolve itemId khác cấp; sau Lv1 mới bổ sung resolver/preflight và kiểm mặc chéo Lv10. Recovery runtime giữ evidence `build/vo-native-run-recovery-v1/recovery-report.json`; lượt sửa brief/source không đổi C# hoặc chạy lại Unity. Map01A vẫn là target tổng thể.

## OWNER REJECT — motion mới sai bản đã chốt, dừng mở Lv10 — 2026-09-11

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

## Võ POSE THỬ div4 — checkpoint tooling, motion chưa đóng — 2026-09-11

Chỉ đạo owner hiện hành: Võ motion/source review → wardrobe/10 slot Lv1 → Lv10/mặc chéo → class khác. Map01A vẫn là target sản phẩm; các Next sang Kiếm ở lịch sử bên dưới không còn là action hiện hành.

Worktree sạch `/private/tmp/lgo-vo-pose-div4-clean`, nhánh `codex/vo-pose-div4-review`, fork `origin/feature/2d@cc684828`. Chỉ port bốn file Python pack/test/capture và ghi chú này; không C#/camera/close-review/resources outfit/map WIP. Registered outfit/motion gốc tại `/private/tmp/lgo-vo-lv1-30-FaSFxE` giữ nguyên, chưa được coi là đã tích hợp vào nhánh sạch.

Packer mặc định divisor4/max1024, xuất trực tiếp `atlas-review.json/png`, yêu cầu pivot nguồn rõ cho jump_tuck. Test cũ trong WIP thực tế chưa có case divisor4; nay có test reconstruction4/8, default4, CLI/pivot. Pack tái tạo `build/vo-pose-div4-repacked-v1` trùng byte external `legacy-base-run-contact-jump-v3-div4`: 512×1024, PNG373834byte, RGBA8 2097152byte, SHA `27630a5ceece2500e412620b70cf43e61a80bbef5d4391ae450d3d19d6829010`, REVIEW_ONLY. Source hash/reconstruction6/6, registration sạch.

Evidence mới `build/vo-pose-div4-verified-runtime-v2/{mobile,tablet,pc}`: 154 frame/profile, đủ6pose, hash/đường dẫn pack và Player trong provenance; ảnh đứng/chạy/lộn đã xem. Đây là Player binary WIP kế thừa, không phải build HEAD sạch; batch0C#/0build. Không claim motion/production PASS: contact A/B còn cách ground65px nguồn. Hai redraw candidate bị loại vì alpha/registration; giữ source cũ, không bù offset/camera. Chi tiết trong `build/vo-pose-div4-verified-runtime-v2/REVIEW.md`.

Kiểm cuối tooling: pack9/9, capture7/7; no-3D/no-source-image và frozen diff PASS. Code review độc lập đã bắt/sửa provenance khi pack bị thay giữa capture; regression manifest/atlas mutation PASS. 0build,3lượt capture có lý do; chưa có video transition review hoặc source contact pass.

## Võ Lv1–30 functional class slice gate — 2026-09-10

Võ là class mẫu đầu tiên đã qua functional vertical-slice gate trên Map01A: base nam/nữ, bốn mốc Lv1/10/20/30, 10 slot, cởi/mặc từng slot, rig 10 bone, 112 equipment component và idle/walk/run/jump/basic attack/`Liên Quyền` có hit timing/HP feedback. Batch này bổ sung item level độc lập theo slot: owner có thể giữ base/loadout Lv1 nhưng đổi riêng `outer_tunic` sang Lv30, chuyển nam/nữ, chạy/nhảy/đánh/skill rồi cởi/mặc lại mà item vẫn bám shared rig.

Capture `build/vo-mixed-level-v1/capture/` có 78 artifact/profile trên mobile/tablet/PC simulation. Manifest cả ba profile xác nhận mixed loadout `outer_tunic=Lv30`, chín slot còn lại Lv1, motion và toggle đều pass; frame 35/38, 28/34 và 67/71/72/78 đã review trực tiếp. EditMode `207/206/0/1`; build macOS thành công; smoke/matrix/no-3D/no-source-image pass.

Trạng thái: `VO_LV1_30_FUNCTIONAL_VERTICAL_SLICE_PASS / ART_PRODUCTION_DRAFT`. Đây là bằng chứng hệ thống và hướng hình đủ để nhân pipeline; chưa claim animation nhiều frame production, balance, persistence/backend hoặc art final. Theo Goal, tiếp theo kế thừa Kiếm draft hiện có vào cùng base/10-slot/mixed-level contract, chưa mở Pháp/Cơ/Linh đồng thời.

## Map01A playable visual slice gate — 2026-09-10

Cổng Đông Lâm Map01A giữ nguyên functional gate Q01–Q09: route 10 khu, 6 NPC, 4 quái chỉ ở combat edge, Linh Thảo/rương, ba hit combat, loot, inventory/potion/equip, minimap và portal Suối Thanh Minh. Visual audit theo sheet `01-gameplay-screen-16x9.png` đã mở lại gate vì bản cũ dành quá nhiều khung hình cho trời và làm nhân vật/NPC quá nhỏ.

Batch composition v1 hạ camera từ 4,6 xuống 3,8 world unit, đặt gameplay lane ở offset 1,5 và thêm 21 placement cây/trúc/hoa/đá từ chính `modules-atlas.png`; không thêm texture, không resize ảnh và không tăng texture budget. Player capture có 78 artifact/profile trên mobile/tablet/PC simulation; đã review trực tiếp arrival, chợ/thoại, combat edge và portal ở cả ba tỷ lệ. Cổng, kiến trúc, NPC và nhân vật nay chiếm vùng chơi rõ ràng, UI không che objective/interact chính.

Trạng thái: `MAP01A_PLAYABLE_VISUAL_SLICE_PASS / ART_PRODUCTION_DRAFT / PRODUCTION_SERVICES_DEFERRED`. Evidence: `build/map01a-composition-v1/capture/`. Non-claim: chưa đạt illustration final như target screenshot, chưa chứng nhận thiết bị vật lý, persistence/backend hoặc scene Map01B. Theo Goal, task tiếp theo là đóng Võ Lv1–30; Kiếm draft giữ nguyên và chưa được tiếp tục.

## Map01A playable quest flow Q01–Q09 — 2026-09-10

Cổng Đông Lâm hiện có state machine local cho đủ tuyến Q01–Q09 trong Player: thoại bốn NPC, quan sát cổng, kiểm tra hành trang, nhận consumable, hái Linh Thảo, mở rương tùy chọn, nhận combat quest, hạ mục tiêu bằng ba lần skill, nhặt loot và mở portal. Trạng thái world phản hồi trực tiếp: herb ẩn, chest dim, enemy chết đổi tint, portal mở. Capture v4 tạo 72 ảnh (24 trạng thái × mobile/tablet/PC); cả ba manifest xác nhận 9/9 quest, 3 cast/3 hit và target HP 0. Visual review đã kiểm thoại, gather/chest, combat và portal.

Trạng thái: `MAP01A_Q01_Q09_PLAYABLE_FLOW_PASS / MAP01A_FUNCTIONAL_UI_INCOMPLETE`. Evidence: `build/map01a-playable/q01-q09-three-profiles-final/`. Chưa claim Map01A hoàn tất vì Q04 mới ghi nhận flag và reward text; cần panel inventory thật, dùng potion, equip class item và minimap/route unlock nhìn thấy được. Đây là macOS aspect simulation, chưa phải chứng nhận mobile/tablet vật lý; quest state chưa nối persistence/backend.

## Võ modular equipment motion alignment — 2026-09-10

Runtime không còn tráo paper-doll đang mặc sang full-frame Lv1 khi nhân vật ở `base`, `modular` hoặc tier Lv10/20/30. Các layer cùng đi theo root pose; full-frame key pose chỉ chạy ở `Lv1 + full`. EditMode `179/178/0/1`; macOS Player `166.975.891` byte, 0 error/13 warning; 54 ảnh tại `build/map01a-art/vo-aligned-modular-final-three-profiles-v2/` technical pass và các frame 11/12/14/15/18 đã review trên ba profile. Frame 15 xác nhận nữ modular còn đúng `base + 9 slot` sau khi tháo `inner_top`.

Trạng thái: `MODULAR_EQUIPMENT_MOTION_ALIGNMENT_PASS / ARTICULATED_RIG_INCOMPLETE`. Đây là root-pose compatibility, chưa phải limb animation; bước tiếp theo là một batch rig/attachment và đủ state Lv1–30 trước khi mở class khác.

> Batch mới nhất: Map 01A đã dựng đủ tuyến nhìn thấy 10 khu bằng atlas/module tái sử dụng, 6 NPC, 6 landmark, 4 quái combat-edge-only và hai interactable. Runtime PNG giảm từ 4.526.172 xuống 1.049.392 byte sau palette optimization và giữ review tương đương. Unity EditMode cuối 175 tổng/174 pass/0 fail/1 skip; macOS Player build 160.624.595 byte, 0 lỗi/13 warning; capture 24 ảnh/3 profile technical pass và đã review. Trạng thái `MAP_ART_FOUNDATION_PASS / CLASS_PC_VISUAL_FIX_REQUIRED`; Q01–Q09 và combat loop chưa được claim.

> Bắt buộc đọc trước batch art/scale: `docs/art/LGO-MAP01A-ASSET-OPTIMIZATION-LESSONS.md`. Owner yêu cầu runtime giảm chất lượng/dung lượng, chia tile dùng lại và lưu bài học. Batch đang gom camera/HUD scale + downsample/tile; budget mới 4 MiB PNG, chưa dùng số 7 MiB cũ làm mục tiêu.

# PROJECT STATE — Linh Giới Online 2D

## Võ skill hit proof — 2026-09-10

`Liệt Phong Kích` trong Map01A hiện có range gate 2,4 world unit, active window 0,42 giây, hit timing, damage 35, target HP `100→65`, hit counter, target tint và HUD feedback. EditMode `179/178/0/1`; macOS Player `166.975.379` byte, 0 error/13 warning. Capture `build/map01a-art/vo-combat-final-three-profiles/{mobile,tablet,pc}/12-vo-skill.png` đã review: pose/VFX/target/HP feedback đều xuất hiện, foot vẫn ở combat lane.

Trạng thái skill: `ONE_VO_SKILL_RUNTIME_PROOF_PASS`; chưa claim combo, enemy AI, knockback, death/drop hoặc balance. Class gate còn thiếu tier-matched modular motion.

## Võ Lv1/10/20/30 visual progression — 2026-09-10

Runtime hiện có bốn mốc trang bị Võ Lv1/10/20/30, mỗi mốc gồm nam/nữ, base/full và 10 slot paper-doll: 96 part, bốn atlas tier tách vòng đời tải. Hai giới đều có sáu pose Lv1 thật; tổng sáu atlas indexed 1024² là 643.722 byte. L/touch đổi tier trong Player mà không lệch foot anchor; 54 ảnh ba profile tại `build/map01a-art/vo-lv1-30-final-v2-three-profiles/` technical pass và các frame 12/15–18 đã review trực tiếp.

Trạng thái: `VO_LV1_30_VISUAL_PROGRESSION_PASS / TIER_MATCHED_MOTION_AND_COMBAT_INCOMPLETE`. Motion hiện chỉ mang outfit Lv1. Ở Lv10/20/30 runtime giữ đúng paper-doll tĩnh thay vì tráo sang outfit Lv1; cần batch tier-matched motion hoặc rig attachment trước khi đóng class. Build macOS `166.974.355` byte, 0 error/0 warning; EditMode cuối `179/178/0/1`.

## Võ Lv1 equipment + real-motion checkpoint — 2026-09-10

Đã kế thừa base WIP đã audit và tạo đồng bộ Võ nam/nữ Lv1 theo board owner, không dùng board phẳng làm runtime asset. Runtime có 10 slot `main_weapon/head_hair/inner_top/outer_tunic/lower_garment/waist/arm_guard/boots/light_armor/accessory`, cùng canvas và foot anchor; C/G/V/B hoặc touch kiểm được full/base/modular, giới tính, chọn slot và tháo/lắp. Võ nam có sáu pose thật idle/walk/dash/punch; Võ nữ hiện dùng static/fallback và được ghi rõ trong manifest.

Hai atlas indexed 1024² tổng 215.938 byte PNG; source 1024×1536 nằm ngoài Unity và runtime chỉ giữ kích thước đủ cho actor khoảng 150 px. EditMode cuối `178/177/0/1`; Player build `162.751.299` byte, 0 error/13 warning; 42 ảnh ba profile ở `build/map01a-art/vo-lv1-motion-final-three-profiles/` đã review. Baseline onboarding, smoke/matrix, no-3D và no-source-image pass. Trạng thái: `VO_LV1_EQUIPMENT_MOTION_CHECKPOINT_PASS / VO_LV1_30_PROGRESSION_INCOMPLETE`.

Tiếp theo xử lý một batch Võ Lv10/Lv20/Lv30 nam/nữ và motion nữ, rồi nối progression selector + frame animation. Chưa mở class thứ hai; không claim combat loop hoàn chỉnh hoặc chất lượng production cuối.

## Võ Lv1–30 runtime workflow proof — 2026-09-10

Map 01A hiện hiển thị PC Võ nam từ đúng WIP owner đã duyệt thay cho hình cyan. Runtime pack 512²/23.370 byte giữ `base/full` và ba slot tách trên chung canvas; HUD PC/mobile cho đổi `full → base → modular`, đi ngang và dùng `Liệt Phong Kích`. EditMode mới nhất `177 total / 176 passed / 0 failed / 1 skipped`; macOS build `160.896.915` byte, 0 error/13 warning; capture Map 01A `12 × 3` profile technical pass và đã review các frame base/modular/walk/skill. Baseline onboarding smoke/build/capture 20 frame cùng runtime/visual matrix, no-3D và no-source-image đều pass; ảnh baseline vẫn là blockout cũ và không được dùng để claim mỹ thuật Map 01A.

Trạng thái sản phẩm: `VO_RUNTIME_WORKFLOW_PROOF_PASS / VO_10_SLOT_AND_FRAME_MOTION_INCOMPLETE`. Transform motion hiện chỉ chứng minh state/input/VFX; chưa phải frame animation production. Võ còn thiếu bảy slot, nữ, run/jump/basic attack và `Liên Quyền` nhiều frame. Batch sau xử lý cả sheet cùng tỷ lệ rồi mới test/capture, không vá từng item.

## Goal hiện hành — Cổng Đông Lâm Map 01A — 2026-09-10

Batch đa màn hình đã kiểm kỹ thuật: importer nén, grounding, HUD safe-area, joystick dùng lại, thoại Hạ Vân; EditMode 167 pass/0 fail/1 skipped; build 0 errors, 161.426.371 byte. Capture 12 ảnh mobile/tablet/PC ở `build/map01a-art/batch-final-three-profiles/`; xem ba ảnh dialogue, HUD không che NPC. Vẫn **VISUAL_FIX_REQUIRED** vì PC placeholder và map mới có đoạn arrival, chưa đủ route/Q01–Q09. Chưa chứng nhận touch/GPU thiết bị thật.

MAP01A-02 có preview pack nguồn riêng chạy trong macOS Player; camera đã sửa cắt mái. EditMode 167 pass/0 fail/1 skipped, build 0 errors. Ảnh `build/map01a-art/review/player-arrival-camera.png` còn PC blockout và thiếu HUD/marker/flow Map01A: **VISUAL_FIX_REQUIRED**, chưa chuyển class.

Audit sandbox class đã đóng: lịch sử `efa46a8` được bảo toàn bằng bundle, diff uncommitted có backup, 26 ảnh chi tiết đã thêm vào source pack và 28 unit test tool class PASS. Không lấy nguyên nhánh lệch hoặc output procedural làm mỹ thuật. Báo cáo: `docs/art/LGO-CLASS-STANDARDIZATION-REUSE-AUDIT-v1.md`.

MAP01A-01 đã có contract runtime data-driven và field evidence riêng. EditMode, Editor smoke, macOS build/capture và matrix đã chạy; contract hiện diện trong Player. Visual review chưa đạt source owner vì scene cũ vẫn là blockout rectangle tối. Trạng thái sản phẩm vì vậy là `CONTRACT_PASS / VISUAL_FIX_REQUIRED`; tiếp theo chỉ làm visible slice spawn–đại cổng.

Goal vận hành đã được realign theo source owner mới: trước tiên hoàn thiện **Cổng Đông Lâm Map 01A Lv1–3** thành vertical slice 2D có thể chơi và kiểm bằng macOS Player. Map gồm 10 khu từ Spawn/Hạ Vân tới portal Suối Thanh Minh, 12 lớp render/parallax, safe-zone + khu dân cư + combat edge nhẹ, sáu NPC chính, bốn quái Lv1–3 và quest Q01–Q09. Linh Thành chỉ xuất hiện xa và chưa mở.

Sau gate Map 01A, class mẫu là **Võ Lv1–30** vì batch trước đã có base nam/nữ căn ground, mask/anchor, ba slot alpha và atlas/toggle evidence có thể tái sử dụng. Hoàn thiện Võ đủ 10 slot, thay đồ và motion/skill `Liên Quyền` trước khi nhân sang bốn class còn lại. Không làm lại asset đã dùng được; không coi primitive hoặc ảnh tự sinh cũ là nguồn mỹ thuật.

Source pack đã chuẩn hóa ngoài repo tại `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1`: 10 ảnh Map 01A canonical, 37 ảnh class canonical và 12 file Võ WIP. Catalog trong repo: `docs/art/LGO-SELECTED-2D-SOURCE-CATALOG-v1.md`. Ảnh gốc không đổi. Board chỉ là source; runtime dùng asset tách lớp có provenance.

Các đoạn dưới phản ánh checkpoint trước realignment. Nếu mâu thuẫn, Goal hiện hành và source catalog thắng.

Ưu tiên owner mới nhất 2026-09-10: Codex tự quyết hướng kỹ thuật, nhưng phải phục vụ sản phẩm 2D owner có thể tự kiểm chứng trong Player. Goal hiện tại là **hoàn thiện map đầu Đông Môn trước**, sau đó làm **một class đầu tiên** đủ base body, paper-doll slot, tách đồ, thay đồ, chuyển động và skill trong Player thật để kiểm chứng workflow trước khi nhân rộng sang 5 class. Góc cổng illustrated hiện mới là draft, không đồng nghĩa map đầu đã xong. Không mở map thứ hai hoặc mở rộng hub/district trước khi Đông Môn đạt gate.

Quyết định kỹ thuật đang khóa:

- Đông Môn chuyển dần sang authored Tilemap/Grid + Sprite Atlas/prop atlas + parallax nhiều lớp, data-driven bằng Resource JSON trong giai đoạn draft.
- Runtime map phải chứng minh flow spawn -> Người Giữ Cổng -> Bia Luyện Khí -> jump/dash/class skill -> Shadow Slime -> complete trong macOS Player thật, có capture/manifest và review ảnh.
- Không polish primitive/rectangle vô hạn, không crop/dán ảnh concept/reference, không Meshy/3D, không thay frozen surfaces.
- Sau gate map đầu, làm một class đầu tiên theo base chung nam/nữ, paper-doll slots, anchor/pivot, alpha item thật, layer order, motion sync và Player verification cho chọn class/mặc/tháo đồ/chuyển động/skill.
- Sau khi class đầu pass mới nhân rộng sang 5 class; tab song song đã dừng nhưng vẫn không sửa trực tiếp main checkout hoặc ghi đè worktree cũ.

Gate thao tác kiểm chứng chi tiết nằm ở `NEXT-ACTION.md`.


## Đông Môn PC route-lane grounding — 2026-09-10

`LGO_DONG_MON_PC_ROUTE_LANE_GROUNDING_READY`: map đầu Đông Môn có source runtime `DongMonPlayerGrounding.json` cho 5 anchor `gatekeeper/training-stone/jump/dash/shadow-slime`, mỗi anchor khai báo lane, foot/contact shadow, player/shadow sort order và shadow size. `TwoDOnboardingController` đọc source này để đổi contact shadow + sort band theo `CurrentRouteNodeId`; manifest giữ `currentGrounding` cho trạng thái cuối và `visitedGrounding=shadow-slime lane=combat-lane playerSort=6 shadowSort=5` để chứng minh Player thật đã đi qua combat lane. Đây là checkpoint khớp PC vào mặt phẳng map đầu, không phải polish art production giống ảnh reference. Evidence 2026-09-10: EditMode `total=164 passed=163 failed=0 skipped=1`; onboarding smoke PASS; macOS Player build Succeeded; Player capture 20 frame PASS; runtime/visual matrix PASS; no-3D/no-source-image PASS; frozen diff audit PASS. Đã xem ảnh `01-initial`, `02-gate-focus`, `03-dialogue`, `08-complete`, `20-district-harbor-preview`: PC/NPC đọc được, bóng chân hiện đúng, combat-lane foreground sort rõ hơn; overlay vẫn blockout, chưa claim thành phẩm.

## Đông Môn illustrated draft — 2026-09-10

Đã tạo art mới và pack skyline + atlas cổng/NPC/terrain; preview opt-in trong Player qua `--lgo-dongmon-art-preview`, không sửa controller/state/5 class hoặc frozen surfaces. `tools/capture_lgo_dongmon_art.py` capture 5 trạng thái vào thư mục riêng. EditMode 139 pass, 0 fail, 1 skipped; guard 5 test pass; smoke/build/baseline 20 frame và art 5 frame đã chạy, ảnh đã review. Art vẫn DRAFT, player còn placeholder; không claim giống hoàn toàn ảnh owner.

Next/gate: owner xem capture `build/dongmon-art/player-final/03-dialogue.png` (bản copy bền ở `build/dongmon-art-checkpoint/`) và duyệt bố cục/palette/tỷ lệ trước khi nhân rộng. Sau duyệt mới mở thêm foreground/prop và ghép asset 5 class đã được tab riêng chuẩn hóa. Hướng/plan/evidence: `docs/design/dong-mon-illustrated/DESIGN.md`. Không tiếp polish primitive; không ghi đè checkout chính hoặc source tab 5 class. Không có blocker runtime; gate còn lại là duyệt mỹ thuật.





## Võ Lv1-30 function probe checkpoint — 2026-09-10

`LGO_VO_LV1_30_SKILL_CUE_ART_READY`: Võ Lv1-30 skill cue đã dùng approved runtime art cho đủ 3 cue `trail`, `edge`, `impact`; manifest `VoLv1ApprovedRuntimeArt` tăng từ 11 lên 13 cell và bắt `skill_vo_lv1_palm_burst` cho impact. Runtime test tìm trực tiếp `vo_lv1_first_skill_edge` và `vo_lv1_first_skill_impact` GameObject, bắt sprite `ApprovedRuntimeArt`, và kiểm impact world size để không phình/che PC. Player frame 08 cho thấy vệt skill sáng rõ hơn ở tay phải; vẫn là art/probe giai đoạn đầu, chưa claim production final. Evidence 2026-09-10: TDD red fail đúng vì `edge` còn `LGO 2D Solid Sprite` và manifest `cells=11`; GREEN EditMode `total=165 passed=164 failed=0 skipped=1`; smoke PASS; macOS Player build Succeeded errors=0 warnings=7; visual capture 20 frame PASS; runtime/visual matrix PASS; no-3D/no-source-image PASS.

`LGO_VO_LV1_30_CELLMAP_COVERAGE_READY`: Võ là class mẫu đầu tiên, phạm vi vẫn khóa **Lv1-30**. Cell-map approved runtime art đã mở rộng từ 8 lên 11 cell: thêm `inner_shadow/base_torso_male`, `pants_shadow_l/pants_vo_lv1`, `pants_shadow_r/pants_vo_lv1` để thân trong và hai chân không còn fallback primitive trong Player. Test runtime tìm trực tiếp GameObject paper-doll và bắt sprite name chứa `ApprovedRuntimeArt`; runtime/visual matrix cũng bắt `cells=11` cùng 3 cell mới. Đây là checkpoint chức năng/tách đồ/pose trong Player, chưa claim production art giống ảnh tham chiếu; bước sau vẫn là refine alpha, pivot, scale và frame motion Võ Lv1-30 trước khi nhân sang class khác. Evidence 2026-09-10: TDD red fail đúng vì manifest chỉ `cells=8` và `inner_shadow` render `LGO 2D Shape Sprite tapered`; GREEN EditMode `total=165 passed=164 failed=0 skipped=1`; smoke PASS; macOS Player build Succeeded errors=0 warnings=13 UI obsolete cũ; visual capture 20 frame PASS; runtime/visual matrix PASS; no-3D/no-source-image PASS; frozen diff audit PASS.

`LGO_VO_LV1_30_FUNCTION_PROBE_READY`: Võ là class mẫu đầu tiên trong scope **Lv1-30**; chưa nhân sang 5 class và không mở tier 31-100. `TwoDOnboardingController` expose `runtimeVoLv1LevelBandFunctionProbeSnapshot` vào Player manifest để owner kiểm cùng một chỗ: `classId=vo`, `levelBand=1-30`, `highTier31Plus=False`, paper-doll slots `OuterShirt/PantsOrSkirt/Waist/Gloves/Boots/Weapon`, try-on/apply equipment, motion states `Idle/Jump/Dash/ClassSkill/TrainingCompletePose`, skill `vo_lv1_first_skill` đánh `shadow-slime`, và grounding đã đi qua combat lane Đông Môn. Probe này là contract kiểm chức năng class trong Player, chưa claim art production. Evidence 2026-09-10: TDD red thiếu property; EditMode sau implement `total=165 passed=164 failed=0 skipped=1`; onboarding smoke PASS; macOS Player build marker PASS qua runtime matrix; Player capture 20 frame PASS; runtime/visual matrix PASS; no-3D/no-source-image PASS.

## Võ Lv1 one-class runtime slice checkpoint — 2026-09-10

`LGO_VO_LV1_APPROVED_RUNTIME_ART_CELLMAP_READY`: Võ Lv1/Lv1-30 đã có cell-map runtime đầu tiên từ `VoLv1ApprovedRuntimeArt` vào Player: manifest thêm `levelBand=1-30`, `starterTexture`, `skillTexture`, và `cellMap`; `chest_panel`, `hand_wrap_r`, `vo_lv1_first_skill_trail` bật `approvedRuntimeArt=true`. Renderer tạo sprite từ atlas rect bằng `Sprite.Create`, scale theo world target size, còn part chưa map tiếp tục fallback primitive để tránh chồng/nhân sai. Visual manifest/matrix bắt `approvedRuntimeArt=True`, `VoLv1ApprovedRuntimeArt`, `approvedCell=...`. Evidence 2026-09-10: TDD red fail đúng vì thiếu loader; EditMode sau implement `total=162 passed=161 failed=0 skipped=1`; 2D onboarding smoke PASS; macOS Player build Succeeded errors=0 warnings=0; Player visual capture PASS; runtime/visual matrix PASS; no-3D/no-source-image PASS; frozen diff audit PASS. Đã review ảnh `07-skill-ready`, `08-complete`, `10-inventory-applied`: art thật thấy rõ nhất ở torso/skill trong `08-complete`; alpha/cell slicing còn thô nên chưa claim production final.
`LGO_VO_LV1_APPROVED_RUNTIME_ART_PACK_READY`: đã thêm pack art runtime mới cho Võ Lv1/Lv1-30 foundation tại `VoLv1ApprovedRuntimeArt`: `vo-lv1-starter-atlas.png` 2048x2048 cho body/equipment paper-doll cells và `vo-lv1-skill-atlas.png` 1024x1024 cho skill VFX atlas, kèm `manifest.json` status `APPROVED_RUNTIME_ART`, hash SHA-256, role, path và provenance `image_gen`. Đây là art gốc mới, không dùng ảnh thiết kế cũ/reference làm runtime asset. Alpha hiện là automated matte v0 nên còn cần refine/cell slicing trước khi claim production; batch này chỉ mở đường thay primitive bằng atlas thật có guard. Scope class giữ Lv1-30, chưa mở tier 31-100.
`LGO_VO_LV1_APPROVED_RUNTIME_ART_POLICY_READY`: validator `validate_2d_branch_no_source_images.py` không còn là chặn tuyệt đối mọi runtime art; nó dùng registry manifest allowlist chặt cho pack image runtime được duyệt. Pack Võ Lv1 được chuẩn bị ở `client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1ApprovedRuntimeArt` với manifest id `vo-lv1-approved-runtime-art-v1`, status `APPROVED_RUNTIME_ART`, hai PNG dự kiến `vo-lv1-starter-atlas.png` và `vo-lv1-skill-atlas.png`, role/hash/dimension/generator/referenceOnly bắt buộc. Test guard tạo pack Võ hợp lệ trong temp root và xác nhận được allow; ảnh rogue ngoài manifest vẫn fail. Chưa thêm PNG/PSB thật trong batch này; đây là cổng kỹ thuật để batch sau thay atlas cell runtime-generated bằng art Võ thật mà vẫn chặn ảnh concept/source cũ.
`LGO_VO_LV1_ATLAS_CELL_BINDING_READY`: Võ Lv1 hiện bind từng paper-doll part/skill cue vào production atlas cell để chuẩn bị thay primitive bằng spritesheet/rig thật mà không phá anchor/pivot/fit: `part=chest_panel cell=torso_outer_vo_lv1`, `part=hand_wrap_r cell=glove_r_vo_lv1`, `skillCue=vo_lv1_first_skill_trail cell=skill_vo_lv1_palm_trail`, và snapshot có `cellSource=runtime-generated-atlas-cell`. Runtime object name chứa `AtlasCell {cellId}` cho part và skill cue, nên Player/test có thể kiểm đúng cell nào đang hiển thị. Evidence 2026-09-10: TDD red fail đúng vì model thiếu `cell`; EditMode sau implement `total=160 passed=159 failed=0 skipped=1`; 2D onboarding smoke PASS; macOS Player build Succeeded errors=0 warnings=13; Player visual capture PASS 20 ảnh; runtime/visual matrix PASS với marker cell binding; no-3D/no-source-image PASS; frozen diff audit PASS. Đã review ảnh `07-skill-ready`, `08-complete`, `09-inventory-try`, `10-inventory-applied`: chức năng anchor/slot/try-on/apply/skill đi đúng hướng trong scene thật, nhưng vẫn là scaffold primitive, chưa claim production illustrated art giống ảnh owner gửi.
`LGO_VO_LV1_PRODUCTION_ATLAS_CONTRACT_READY`: Võ Lv1 PaperDollAtlas hiện có production atlas contract để thay primitive bằng art thật mà không vỡ khớp: `productionAtlas=vo-lv1-starter-atlas-v1`, `importMode=layered-psb-or-spritesheet`, `texturePolicy=approved-original-2d-art-only`, required cells cho base body/torso/pants/waist/gloves/boots/weapon/skill, rig joints `Root/Chest/Hips/Hand_L/Hand_R/Foot_L/Foot_R`, motion clips Võ Lv1 và replacement gates `preserve-runtime-fit-contract`, `preserve-paper-doll-slot-ids`, `preserve-anchor-gizmo-semantic-points`, `preserve-player-collision-plane`, `visual-review-in-player`. Evidence 2026-09-10: TDD red fail đúng vì thiếu `productionAtlas`; EditMode sau implement `total=158 passed=157 failed=0 skipped=1`; 2D onboarding smoke PASS; macOS Player build Succeeded errors=0 warnings=19; Player visual capture PASS 20 ảnh; runtime/visual matrix PASS; no-3D/no-source-image PASS; frozen diff audit PASS. Đã review ảnh `08-complete` và `09-inventory-try`: visual chưa đổi khỏi scaffold, nhưng manifest Player đã khóa contract để thay spritesheet/rig thật cho một class trước khi nhân sang 5 class.
`LGO_VO_LV1_RUNTIME_FIT_READY`: Võ Lv1 hiện có runtime fit contract để kiểm đồ rời/skill có khớp PC trong cảnh thật hay không: `runtimeVoLv1RuntimeFitSnapshot` expose `fitStatus=ANCHOR_ALIGNED`, `scenePlane=dong-mon-gameplay-plane`, `playerPivot=bottom-center`, bounds/sort cho `OuterShirt`, `PantsOrSkirt`, `Waist`, `Gloves`, `Boots`, và `skill=vo_lv1_first_skill anchor=Hand_R`. Đây là lớp kiểm chứng trước art production: thay spritesheet/rig sau này phải giữ item/anchor/pivot/bounds không lệch. Evidence 2026-09-10: TDD red fail đúng vì thiếu property; EditMode sau implement `total=157 passed=156 failed=0 skipped=1`; 2D onboarding smoke PASS; macOS Player build Succeeded errors=0 warnings=13; Player visual capture PASS 20 ảnh; runtime/visual matrix PASS; no-3D/no-source-image PASS; frozen diff audit PASS. Đã review ảnh `07-skill-ready` và `10-inventory-applied`: skill cue bám hướng tay phải, đồ/inventory vẫn kiểm được slot/anchor; chưa claim production illustrated art.
`LGO_VO_LV1_ANCHOR_GIZMO_READY`: Võ Lv1 có runtime anchor/pivot gizmo bám vào player cho `Chest`, `Hips`, `Hand_L`, `Hand_R`, `Foot_L`, `Foot_R`, bật trong training/inventory để kiểm đồ rời đang khớp thân/chân/tay trên mặt phẳng Đông Môn. Visual manifest expose `runtimeVoLv1AnchorGizmoSnapshot` với `pivotPolicy=bottom-center-foot-anchor`, `visibleWhen=inventory-or-class-training`, `safe-runtime-gizmo=True`; đây là scaffold kiểm alignment trước khi thay primitive bằng spritesheet/rig thật, không phải UI/art production. Evidence 2026-09-10: EditMode total=155 passed=154 failed=0 skipped=1; 2D onboarding smoke PASS; macOS Player build Succeeded errors=0 warnings=7; Player visual capture PASS 20 ảnh; runtime/visual matrix PASS; no-3D/no-source-image PASS; frozen diff audit PASS. Đã review ảnh 05-jump-ready, 08-complete, 09-inventory-try, 10-inventory-applied, 20-district-harbor-preview: anchor Chest/Hips/Hand/Foot đọc được trong Player, không claim production art.
`LGO_VO_LV1_SLOT_COMPAT_READY`: Võ Lv1 class slice/runtime inventory hiện expose slot compatibility/provenance để kiểm chứng đồ rời khớp base trước khi thay bằng spritesheet thật: `anchorSet=Chest,Hips,Hand_L,Hand_R,Foot_L,Foot_R`, `compat=base_male_torso->OuterShirt`, `source=runtime-authored-json`, và inventory input ghi `selectedCompat=True`, `selectedSlot`, `selectedAnchor`, `fitProfile`, `source=runtime-authored-catalog`. Mục tiêu là tránh lỗi thay đồ sai anchor khi nhân sang 5 class; vẫn không dùng ảnh ngoài và vẫn là primitive/blockout. Evidence: TDD red fail đúng 2 test mới, Unity EditMode `total=153 passed=152 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS, frozen diff audit PASS; đã review ảnh inventory try/apply trong Player.
`LGO_VO_LV1_FRAME_SAMPLER_READY`: Võ Lv1 locomotion profile đã có frame clip/timing data-driven cho `Idle`, `Jump`, `Dash`, `ClassSkill` và `TrainingCompletePose`; controller sample frame theo animation phase rồi expose `motionClip`, `motionFrame`, `paperDollPose` trong `runtimeAnimationSnapshot`. Đây là bước khớp nhân vật vào cảnh bằng cùng hệ anchor/pose đã dùng cho Đông Môn: PC không chỉ đổi màu/slot tĩnh mà có frame intent để sau này thay bằng spritesheet/rig thật. Vẫn là primitive/blockout, chưa claim giống concept owner gửi. Evidence: Unity EditMode `total=151 passed=150 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS, frozen diff audit PASS; đã review ảnh `01-initial`, `03-dialogue`, `05-jump-ready`, `06-dash-ready`, `07-skill-ready`, `08-complete`, `09-inventory-try`, `20-district-harbor-preview`. Next action sau checkpoint: nâng Võ Lv1 sang art atlas/spritesheet approved hoặc rig slot production hơn, rồi mới nhân pattern sang Kiếm/Pháp/Cơ/Linh.
`LGO_VO_LV1_ONE_CLASS_RUNTIME_SLICE_READY`: Class đầu tiên để kiểm chứng workflow là Võ Lv1, chưa nhân rộng sang 5 class. Thêm runtime resource `LGOClasses/VoLv1ClassSlice.json` mô tả base nam/nữ, 5 paper-doll slots, anchor/fit profile, motion Idle/Walk/Jump/Dash/ClassSkill và skill `vo_lv1_first_skill` đánh `shadow-slime`. Controller expose `runtimeVoLv1ClassSliceSnapshot` trong visual manifest tại frame `08-complete` trước khi thử đồ inventory, nên snapshot giữ loadout Võ `top_vo_lv1_male`, motion `TrainingCompletePose` và `runtimeSkillCue=True`; inventory applied vẫn là snapshot riêng để kiểm thử thay đồ. Runtime scene có cue `VÕ Q`/trail nhỏ để owner thấy skill class đầu tiên đã đi qua Player thật, nhưng đây vẫn là cue blockout, chưa phải VFX production. Evidence: Unity EditMode `total=145 passed=144 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS. Next action: thay primitive cue/slot màu bằng sprite atlas/paper-doll asset sạch cho Võ trước, rồi mới nhân pattern sang Kiếm/Pháp/Cơ/Linh.
`LGO_VO_LV1_PAPERDOLL_MOTION_POSE_READY`: Võ Lv1 paper-doll atlas đã có `poseOffsets` data-driven cho `vo_idle`, `vo_jump_lift`, `vo_dash_stretch`, `vo_skill_cast`, `vo_lv1_training_complete`; controller áp pose theo animation intent và expose `currentPose` trong `runtimeVoLv1PaperDollAtlasSnapshot`. Overlay Võ hiện từ lesson Bia Luyện Khí để Player capture kiểm được đồ đi theo Jump/Dash/Skill/Complete, skill cue chỉ bật lúc cast/complete. Đây vẫn là primitive/blockout atlas để kiểm chứng anchor/pivot/motion và thay đồ runtime, chưa phải art production giống concept. Evidence: Unity EditMode `total=149 passed=148 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS, frozen diff audit PASS. Next action sau checkpoint: thay primitive atlas Võ bằng bitmap/spritesheet approved hoặc nâng locomotion frame sampling trước khi nhân pattern sang Kiếm/Pháp/Cơ/Linh.


## Đông Môn interaction marker + PC grounding checkpoint — 2026-09-10

`LGO_DONG_MON_PC_GROUNDING_MARKERS_READY`: Đông Môn map đầu có thêm runtime resource `DongMonInteractionMarkers.json` cho 5 mốc route `gatekeeper -> training-stone -> jump -> dash -> shadow-slime`, render marker/label trực tiếp trong Player để kiểm điểm tương tác. Runtime manifest/matrix bắt `runtimeDongMonInteractionMarkerSourceSnapshot` và `runtimeDongMonPlayerSceneFitSnapshot`; PC được kiểm theo `footAnchor=bottom-center`, contact shadow, ground band, sort order và route anchors để batch class tiếp theo ghép nhân vật/đồ/motion trên cùng hệ tọa độ, không chỉnh tay theo ảnh chụp. Evidence: Unity EditMode `total=143 passed=142 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS. Visual vẫn là blockout kỹ thuật, chưa phải art giống concept; next action là làm một class đầu tiên có base/paper-doll/tách đồ/thay đồ/motion/skill trong Player thật trước khi nhân rộng sang 5 class.

## Gate Keeper silhouette — 2026-09-10

Đã kiểm Người Giữ Cổng 13 part đọc từ JSON với `rect/ellipse/diamond/tapered`; shape lạ fallback rectangle, cache dùng lại texture, snapshot đọc style từ source. Capture tiếp cận NPC ở khoảng cách 0.85 trong FocusRange 1.15 để không chồng silhouette. Không đổi gameplay 5 class hoặc frozen surfaces.

Evidence trong `build/gatekeeper-polish-checkpoint/`: EditMode 138 pass, 0 fail, 1 skipped (pointer capture cần UI panel); onboarding smoke Complete; macOS build Succeeded (0 errors, 13 warning API UI cũ); Player capture 20 BMP; smoke/visual matrix, no-3D/no-source-image và frozen diff audit pass. Đã xem ảnh 01/02/03/20: mũ, áo thuôn, bóng ellipse, gậy/ngọc đọc được; player đứng riêng khi thoại. Đây chỉ là checkpoint silhouette blockout, chưa đạt chuẩn illustrated owner gửi.

macOS: sau launch từ Contents/MacOS, Player có thể chờ foreground do runInBackground=0; dùng `open <đúng app vừa build>` để kích hoạt app đang chạy, không mở ảnh giữa capture. Worktree cũ được giữ vì có thay đổi đồng thời ngoài lượt này; chỉ dọn worktree riêng của batch. Tab 5 class giữ ownership source của họ.

Next: design/demo draft góc Đông Môn bằng art mới theo phần hiệu chỉnh owner trong workflow research; capture Player và duyệt mỹ thuật trước khi nhân rộng. Không tiếp tục polish primitive vô hạn.


## Current branch

`feature/2d`

## Current direction

Scenario production spine mới: `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md`.

Linh Giới Online tiếp tục theo North Star Social Action MMORPG, nhưng branch này khóa hướng **2D Side-Scrolling Social Action MMORPG**: HD 2D anime / illustrated character, không pixel-art, side view, map parallax nhiều lớp, combat nhanh có walk/run/jump/dash/skill, hub xã hội đông người. Khóa kịch bản mới đã được đưa vào `docs/02-GDD.md`: giữ thế giới/class/progression/story/backend, đổi cách hiện thực sang sprite layer, skeleton 2D, anchor, atlas và runtime visual evidence. Linh Thành vẫn là hub xã hội; Đông Môn tutorial là lát cắt ưu tiên đầu: Người Giữ Cổng, Bia/Đá Luyện, movement/jump/dash, skill class, Shadow Slime, quay NPC và mở Linh Thành.

## Current source state

Đã dọn pipeline/source/asset/tool cũ liên quan hướng dựng nhân vật/cảnh 3D khỏi `feature/2d` và loại bỏ toàn bộ ảnh thiết kế/source cũ khỏi source tree để tránh kéo lại hướng art đã bỏ. Runtime 2D nhập môn hiện có một slice player-visible: di chuyển bằng WASD/phím mũi tên, focus NPC, mở thoại, nhận hướng dẫn tới Bia Luyện Khí, kích hoạt bia, hoàn tất chuỗi Jump/Dash/ClassSkill, đánh tan Shadow Slime bằng kỹ năng Võ Lv1 và hoàn tất nhập môn. Visual hiện là sprite/layer procedural gọn để kiểm flow, có HUD world-space, minimap/route overlay, snapshot base character nam/nữ, modular equipment và locomotion animation được camera capture; chưa phải art final. Kịch bản mới yêu cầu production map đi theo Zone Network và Chapter 1 `Vết Nứt Đông Môn`, không biến sân luyện thành toàn game và không triển khai map bằng cách crop/dán board.

## Validation spine

- `python3.12 tools/validate_2d_branch_no_3d.py` bảo vệ branch khỏi việc kéo lại pipeline cũ.
- `python3.12 tools/validate_2d_branch_no_source_images.py` bảo vệ branch khỏi ảnh thiết kế/source cũ; evidence trong `build/` được bỏ qua.
- Unity batch compile/test là gate trước khi báo checkpoint.
- `tools/run_lgo_2d_onboarding_smoke.sh` kiểm flow nhập môn bằng Unity Editor command-line.
- `python3.12 tools/lgo_runtime_smoke_matrix.py --phase two-d` kiểm evidence runtime 2D đã sinh: onboarding smoke JSON, macOS Player build log và visual manifest có `runtimeTilemapSnapshot`/`ChunkFlow`.
- `python3.12 tools/lgo_visual_evidence_matrix.py --verify-current` kiểm các frame visual 2D trọng yếu: initial, gate focus, skill ready, inventory try và inventory applied; mọi claim vẫn là runtime evidence, không phải production art.
- `python3.12 tools/validate_lgo_crash_error_reporting_plan.py` bảo vệ crash/error plan local: phân loại `FIX_REQUIRED`, `UNVERIFIED_ENVIRONMENT`, `CONTRACT_CHANGE_REQUIRED`, `HUMAN_REVIEW_REQUIRED` mà không tích hợp production service/telemetry.
- Player smoke và visual capture trong `build/2d-onboarding-player/` + `build/2d-onboarding-visual/` là evidence runtime thật cho slice 2D, gồm HUD snapshot trong manifest và ảnh PNG review.

## Latest checkpoint evidence

- Cleanup commit: `d97a3c8 Remove 3D asset pipeline from 2D branch`.
- Meshy/service trace cleanup commit: `6bf905e Remove obsolete 3D service traces from 2D branch`.
- Source image cleanup commit: `646492e Remove legacy images and keep 2D HUD procedural`.
- Procedural Đông Môn blockout commit: `eeaf19d Improve 2D onboarding map blockout details`.
- Direction/map catalog checkpoint: `1fa3231 Lock 2D social action direction and map catalog`.
- Character base checkpoint: `0d31cc8 Add 2D character base catalog`.
- Modular equipment checkpoint: `091a2e3 Add 2D modular equipment runtime spine`.
- Võ Lv1 seed checkpoint: `81000ed Add 2D Vo Lv1 starter outfit seed`.
- Locomotion animation checkpoint: `c04e40f Add 2D locomotion animation spine`.
- Tutorial movement/skill checkpoint: Bia Luyện Khí mở chuỗi Jump → Dash → ClassSkill → Complete; visual capture 8 frame và manifest kiểm `screenshotCount=8`, `finalStep=Complete`, animation tokens Jump/Dash/ClassSkill/TrainingCompletePose.
- Shadow Slime combat micro-slice checkpoint: sau Dash, Shadow Slime xuất hiện ở `LearnClassSkill`; ClassSkill đánh tan slime, manifest có `runtimeCombatSnapshot=ShadowSlimeVisible=False ShadowSlimeDefeated=True`, ảnh review `07-skill-ready.png`/`08-complete.png`.
- Map layer budget checkpoint: `TwoDMapDesignCatalog` expose `LayerBudget: Chapter 1: Vết Nứt Đông Môn` với Sky/Fog, Far, Mid, Near, Gameplay Plane, Foreground; minimap overlay hiển thị Chapter/Layer marker và manifest `runtimeMapSnapshot` chứa layer budget.
- Route progress checkpoint: `TwoDOnboardingState.CurrentRouteNodeId` bám các node spawn → gatekeeper → training-stone/movement → jump → dash → shadow-slime → return-gate; minimap hiển thị `Node:` hiện tại và manifest có `runtimeRouteProgressSnapshot`.
- Kiếm Lv1 module checkpoint: catalog có module áo Kiếm nam/nữ, quần/đai/găng/boots, vũ khí kiếm starter với `class=Kiem`, `level=1`, `sword_trail_seed`; loadout mix được áo/kiếm Kiếm với quần Võ qua cùng fit profile. Runtime Đông Môn có rack preview `KIẾM LV1` để chứng minh module class thứ hai đi vào scene mà không cần Meshy/3D hay ảnh source.
- Đông Môn landmark checkpoint: `DongMonLandmarks` khóa Cổng Linh Thành, Bia Luyện Khí, Cầu Gỗ, Thác Nước, Sóng Linh và Rừng Ngoại Thành với layer/route node; runtime scene có silhouette cầu/rừng/thác/sóng linh và manifest chứa `Landmarks: Chapter 1`.
- Inventory try-on checkpoint: runtime Đông Môn có strip `HÀNH TRANG` cho icon Võ/Kiếm/kiếm starter; `RuntimeInventoryTryOnSnapshot` và visual manifest khóa flow `select_icon -> inspect_item -> try_on -> cancel_or_apply`, selected `top_kiem_lv1_male`, preview `status=TRYING_ON`.
- Scenario production spine checkpoint: `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md` đưa kịch bản game mới của owner vào source chính: HD 2D anime/illustrated, không pixel-art, Zone Network, Chapter 1/2/3, class identity, item/map pipeline và roadmap 2D-00 → 2D-12.
- Terrain collision spine checkpoint: Đông Môn có contract `DongMonCollisionBands` cho ground-main, training-platform, jump-gap, dash-lane và slime-arena; runtime có cue `JUMP GAP`/`DASH LANE`, visual manifest chứa `runtimeTerrainCollisionSnapshot` và ảnh review không che HUD/action chính.
- Inventory input spine checkpoint: panel `HÀNH TRANG` hỗ trợ `I` mở/đóng, `Tab` chọn, `T` thử, `Y` áp dụng, `Esc` hủy; `RuntimeInventoryInputSnapshot` ghi state Applied/Trying/Cancelled và visual capture có frame `09-inventory-try`, `10-inventory-applied`.
- Đông Môn tilemap runtime spine checkpoint: map catalog có `DongMonTileDefinitions` cho ground grass/stone, wood platform, jump gap, dash lane và slime arena; runtime scene render tile chunks Gate/Training/Jump/Dash/Slime cùng platform/gap/dash/slime cues, visual manifest ghi `runtimeTilemapSnapshot` + `ChunkFlow`, Player capture 10 frame đã review không che HUD/action chính.
- Runtime smoke matrix 2D checkpoint: `tools/lgo_runtime_smoke_matrix.py --phase two-d` báo `LGO_RUNTIME_SMOKE_MATRIX_2D_PASS`, xác minh smoke JSON, Player build log và visual manifest của onboarding 2D hiện tại.
- Visual evidence matrix 2D checkpoint: `tools/lgo_visual_evidence_matrix.py --verify-current` báo `LGO_VISUAL_EVIDENCE_MATRIX_2D_CURRENT_PASS`, xác minh 5 screenshot evidence quan trọng và các manifest field tương ứng, đồng thời giữ non-claim production art.
- Crash/error reporting plan checkpoint: `tools/validate_lgo_crash_error_reporting_plan.py` báo `LGO_CRASH_ERROR_REPORTING_PLAN_VALIDATION_PASS`; `tools/lgo_error_report_summary.py` phân loại local closure summary thiếu là `UNVERIFIED_ENVIRONMENT`, chưa thêm production crash/telemetry service.
- Quảng Trường plaza shell checkpoint: map catalog expose `LinhThanhPlazaShellSnapshot` với `PlazaShell: district=plaza`, `social-spawn`, `event-board`, `guild-bulletin-preview`, `safe-no-trade-backend`; runtime scene có preview marker social/event/guild trong nền; Player build/capture mới PASS và smoke matrix 2D bắt PlazaShell.
- Linh Thành hub shell checkpoint: map catalog expose `LinhThanhHubShellSnapshot` với `HubShell: linh-thanh`, `district=plaza`, `district=academy`, `district=market`; runtime scene thêm marker/silhouette hub phía sau Đông Môn; Player build/capture mới PASS, smoke matrix 2D bắt buộc token HubShell.
- World/Linh Thành zone-network checkpoint: map catalog expose `ZoneNetworkSnapshot` với `WorldMapNetwork: hub=linh-thanh` và `LinhThanhHubRuntime`; runtime minimap overlay hiển thị node LT ↔ Đông Vực/Âm Giới; Player build/capture mới PASS, smoke matrix 2D giờ bắt buộc `runtimeMapSnapshot` giữ zone network.
- Linh Thành unlock presentation checkpoint: sau Shadow Slime/Complete, `LinhThanhUnlocked=true`, HUD hiện “Mở Linh Thành: Quảng Trường”, runtime scene bật banner/path local preview và manifest có `runtimeLinhThanhUnlockSnapshot` với `unlock=plaza`, `safe-local-no-teleport`; Player build/capture mới PASS và ảnh complete/inventory đã review không che HUD/minimap/inventory.
- Quảng Trường hub runtime preview checkpoint: sau unlock Đông Môn, runtime expose `RuntimeLinhThanhPlazaHubSnapshot`/manifest field với `PlazaHubRuntime`, `npc=gate-guide`, `board=event-local-preview`, `guild-bulletin=locked`, `safe-local-no-backend`; scene bật cụm NPC/board/label sau unlock và frame initial đã review không hiện sớm.
- Đông Môn parallax/foreground polish checkpoint: runtime map catalog có `ParallaxDepthSnapshot` cho `Layer5=Sky/Fog:cloud-drift`, `Layer4=Far Background:mountain-silhouette`, `Layer0=Foreground:grass-leaf-motes`; Player build/capture mới PASS, manifest 10 frame chứa `ParallaxDepth`, ảnh review không che HUD/combat/inventory; app build thực tế khoảng 109MB dù Unity log có bước prepared ~36GB.
- Alpha/Beta/Live checklist checkpoint: `tools/validate_lgo_release_checklist.py` báo `LGO_RELEASE_CHECKLIST_VALIDATION_PASS`; release state vẫn là pre-alpha development, không claim alpha/beta/live hoặc production art.

## Next

Tiếp tục roadmap 2D bằng parallax spacing/foreground polish cho Đông Môn hoặc chuyển chunk procedural sang authored Unity Tilemap asset khi asset sạch sẵn sàng hoặc nâng inventory panel sang inspect detail/icon grid đẹp hơn sau khi có art asset sạch. Map work phải bám `docs/02-GDD.md`, `docs/design/LGO-2D-SOCIAL-ACTION-DIRECTION-LOCK-v0.1.md`, `docs/design/LGO-2D-MAP-AZ-DESIGN-v0.1.md` và không mở HP/loot/economy/server-authoritative combat khi chưa có gate riêng.
## 2026-09-10 — Quảng Trường board interaction + truthful Unity test gate

`LGO_LINHTHANH_PLAZA_BOARD_INTERACTION_READY`: sau khi hoàn tất Đông Môn/Shadow Slime và unlock Linh Thành, state runtime cho phép mở preview local-only của `Bảng sự kiện Quảng Trường`. HUD chuyển đúng sang `Khu vực: Quảng Trường`, visual manifest ghi `runtimeLinhThanhPlazaHubSnapshot` với `interaction=board-preview-open`, và visual capture có frame `11-plaza-board-preview`. Inventory panel được ẩn mặc định và chỉ hiện khi người chơi mở hành trang để board preview không bị che.

Đồng thời sửa `tools/unity_batch_test.sh`: bỏ `-quit` khỏi `-runTests`, bắt buộc có XML results, `total>0`, `passed>0`, `failed=0` trước khi in `UNITY_EDITMODE_PASS`. Evidence mới: Unity EditMode thật `total=98 passed=97 failed=0`, Editor smoke PASS, macOS Player build PASS (`totalSize=114657763`), `tools/lgo_runtime_smoke_matrix.py --phase two-d` PASS và `tools/lgo_visual_evidence_matrix.py --verify-current` PASS. Scope vẫn local-only: chưa mở teleport/shop/giao dịch/bang hội/backend.

## LGO_LINHTHANH_PLAZA_NPC_INTERACTION_READY — 2026-09-10

Quảng Trường có NPC interaction local-only sau unlock Đông Môn: `TryTalkPlazaGateGuide()` và `TryTalkPlazaMerchantPreview()` tách `DialogueSpeaker` khỏi `DialogueLine`, visual frame `12-plaza-npc-preview`, manifest `runtimeLinhThanhPlazaHubSnapshot` có `npc=merchant-preview`, `interaction=npc-merchant-preview`, `safe-local-no-shop-backend`. Đây vẫn là social hub preview an toàn: chưa mở shop/economy, teleport, giao dịch, bang hội hoặc backend.

## LGO_LINHTHANH_PLAZA_TARGET_SELECTOR_READY — 2026-09-10

Quảng Trường sau unlock Đông Môn nay có target selector runtime local-only: người chơi dùng `P` để đổi mục tiêu giữa `Bảng Sự Kiện`, `Người Giữ Cổng` và `Thương Nhân`, dùng `E/Enter` để tương tác mục tiêu đang chọn. Manifest visual có `runtimePlazaHubInputSnapshot` với `controls=P select, E interact`, `selected=merchant-preview`, `interaction=npc-merchant-preview`, `safe-local-no-shop-backend`; visual capture tăng lên 13 frame, gồm `12-plaza-target-selector` và `13-plaza-npc-preview`. Scope vẫn an toàn: chưa mở teleport, shop/economy, giao dịch, bang hội hoặc backend.

## LGO_LINHTHANH_PLAZA_SOCIAL_LAYOUT_READY — 2026-09-10

Quảng Trường selector được nới thành layout `spaced-social-triangle`: Người Giữ Cổng ở trái, Bảng Sự Kiện ở giữa/phía sau, Thương Nhân ở phải để giảm cảm giác chắp cụm và giúp frame selector/NPC dễ đọc hơn. Manifest `runtimePlazaHubInputSnapshot` bắt `layout=spaced-social-triangle`; visual matrix vẫn giữ scope local-only, chưa mở shop/economy/teleport/bang hội/backend.

## LGO_LINHTHANH_HUB_TRANSITION_PREVIEW_READY — 2026-09-10

Đã thêm transition shell local-only `Đông Môn → Quảng Trường`: controller expose `RuntimeHubTransitionSnapshot`, input/runtime method `PreviewEastGateToPlazaTransition()`, visual capture có frame `14-plaza-transition-preview`, manifest bắt `HubTransition: unlocked=True`, `from=east-gate`, `to=plaza`, `mode=local-route-preview`, `safe-local-no-teleport-backend`. Đây là route preview để map A-Z có xương sống đi từ tutorial sang hub; chưa mở teleport thật, map streaming, server travel hoặc backend xã hội.

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
## 2026-09-10 — Quảng Trường layout anchors

`LGO_LINHTHANH_PLAZA_LAYOUT_ANCHORS_READY`: runtime Quảng Trường sau unlock Đông Môn đã có `PlazaHubLayout` với 5 anchor social-spawn, event-board, gate-guide, merchant-preview và guild-locked. Đây là layout/social hub local-only cho Player capture, chưa phải map production art và chưa mở backend event/shop/guild.
## 2026-09-10 — Quảng Trường anchor detail inspect

`LGO_LINHTHANH_PLAZA_ANCHOR_DETAIL_READY`: runtime Quảng Trường đã expose `PlazaHubDetail` cho target đang chọn và visual manifest/matrix bắt merchant preview `role=starter-gear-preview`, `detail=try-before-shop`. Đây là inspect local-only, chưa mở shop/event/guild/backend.
## LGO_LINHTHANH_DISTRICT_PREVIEW_READY — 2026-09-10

`feature/2d` đã có district preview rail local-only cho Linh Thành sau khi hoàn tất Đông Môn. Controller/manifest xuất `runtimeLinhThanhDistrictPreviewSnapshot`; Player capture có 15 frame, trong đó `15-district-preview-rail` hiển thị ring/label Học Viện và guard `safe-no-district-backend`. Trạng thái vẫn là pre-alpha blockout map, chưa production map toàn thế giới.
## LGO_LINHTHANH_DISTRICT_DETAIL_READY — 2026-09-10

`feature/2d` đã có `RuntimeLinhThanhDistrictDetailSnapshot` và Player capture 16 frame. Frame `16-district-market-preview` cho thấy district rail chọn Thương Phố sau Học Viện, manifest guard `safe-no-trade-backend`, `safe-no-economy-backend`, `safe-no-district-backend`. Map tổng thể vẫn pre-alpha blockout, chưa production-complete.

## 2026-09-10 — Linh Thành Đền Linh preview rail

- Marker: `LGO_LINHTHANH_SPIRIT_TEMPLE_PREVIEW_READY`.
- Trạng thái: runtime 2D đã có Player capture riêng `17-district-spirit-temple-preview` cho Đền Linh trong district preview rail; manifest selected `spirit-temple`, route `plaza->spirit-temple`, detail `altar-local-only`, next `quest-buff-gate`.
- Giới hạn: preview local-only, chưa phải production map/art hoàn thiện, chưa mở buff/story/backend/teleport.
- Next: tiếp tục map-safe bằng polish visual/readability Linh Thành hoặc nâng Đông Môn Tilemap/atlas sạch.

## 2026-09-10 — Linh Thành district rail coverage

- Marker: `LGO_LINHTHANH_DISTRICT_RAIL_COVERAGE_READY`.
- Trạng thái: Player visual capture đã tăng lên 20 frame và cover rail Học Viện, Thương Phố, Đền Linh, Khu Rèn, Khu Bang Hội, Cảng Linh Thuyền.
- Evidence cuối giữ Cảng: `selected=harbor`, `label=Cảng Linh Thuyền`, `route=plaza->harbor`, `detail=spirit-boat-locked`, `next=world-route-gate`.
- Giới hạn: vẫn là runtime blockout/local-only; chưa mở crafting, guild, travel, teleport hoặc district backend.
- Next: polish visual density/layout Linh Thành hoặc nâng Đông Môn Tilemap/atlas sạch.

## 2026-09-10 — Linh Thành district rail readability

- Marker: `LGO_LINHTHANH_DISTRICT_RAIL_READABILITY_READY`.
- Trạng thái: district preview callout/backplate đi theo selected node, manifest có `DistrictRailReadability` và Player visual capture 20 frame đã review frame Khu Rèn/Bang Hội/Cảng.
- Giới hạn: polish UX cho runtime blockout local-only; chưa thay production art, chưa mở district backend.
- Next: polish minimap/district rail density hoặc nâng Đông Môn Tilemap/atlas sạch.

## 2026-09-10 — 2D minimap compact readability

- Trạng thái: `LGO_MINIMAP_COMPACT_READABILITY_READY`; overlay/minimap giảm mật độ chữ, route `Đông Môn → Linh Thành`, district chips `HV TP ĐL KR BH CẢ`, world links ngắn và selected-node progress.
- Evidence cần giữ: `runtimeMinimapReadabilitySnapshot` trong visual manifest, Player capture frame `01-initial` và `20-district-harbor-preview`, smoke/visual matrix PASS.
- Giới hạn: local-only, chưa mở teleport/travel/backend; map chưa production-complete toàn bộ A-Z.
- Next: Đông Môn authored Tilemap/atlas sạch hoặc mở chi tiết khu Linh Thành tiếp theo với guard local-only.

## 2026-09-10 — Đông Môn authored detail resource

- Trạng thái: `LGO_DONG_MON_AUTHORED_DETAILS_RESOURCE_READY`; detail pass Đông Môn đã tách sang `Resources/LGOMaps/DongMonAuthoredDetails.json`.
- Evidence cần giữ: `runtimeDongMonAuthoredDetailSourceSnapshot` trong manifest với `details=7`, `moss/step/rope/spirit-dust/rune`, `safe-no-source-image`, `safe-no-3d`.
- Giới hạn: đây là data/runtime foundation, chưa phải production concept art.
- Next: atlas/sprite pipeline sạch cho props/NPC hoặc Linh Thành detail layout resource.

## 2026-09-10 — Gate Keeper NPC sprite source

- Trạng thái: `LGO_DONG_MON_GATEKEEPER_NPC_SPRITE_SOURCE_READY`; Gate Keeper dùng `Resources/LGOMaps/DongMonNpcSprites.json` để render 13 sprite parts.
- Evidence cần giữ: `runtimeDongMonNpcSpriteSourceSnapshot` trong manifest với `role=tutorial-guide`, `silhouette=elder-robed-guardian-staff`, `safe-no-source-image`, `safe-no-3d`.
- Giới hạn: vẫn là runtime blockout/stylized sprite parts, chưa phải final concept art/NPC animation.
- Next: mở atlas/sprite thật cho NPC hoặc áp pattern này cho Training Stone/Shadow Slime/hub NPC.
- LGO_2D_PRODUCTION_WORKFLOW_RESEARCH_READY: đã ghi `docs/execution/LGO-2D-PRODUCTION-WORKFLOW-RESEARCH-v0.1.md`; batch sau ưu tiên pipeline Tilemap/Sprite Atlas/paper-doll, dùng `tools/capture_lgo_2d_onboarding_visual.py`, không polish bằng rectangle primitive kéo dài.
- 2026-09-10: `LGO_VO_LV1_APPROVED_RUNTIME_ART_SLOT_EXPANSION_READY` — Võ Lv1-30 đã bật thêm approved runtime art cells cho glove L/R, waist, boots và staff qua manifest/cellMap; Player capture pass nhưng alpha/slicing vẫn là checkpoint kỹ thuật, chưa production-final như reference screenshot.
## Võ Lv1 full-frame motion gate — 2026-09-10

Võ nam/nữ Lv1 đã có đủ state nhìn thấy trong Map01A: idle, walk, run, jump rise/apex, basic attack và hai key pose `Liên Quyền`. Nguồn candidate đầu 8×2 bị gate mới từ chối vì 16/16 silhouette chạm cell; batch 4×2 riêng từng giới được khử checker bằng border flood, căn margin 32 px, tách 16 frame và đóng lại hai atlas 1024². Action bar chạy bằng touch hoặc Shift/J/Z/X; hit thường trừ 12 HP, `Liên Quyền` trừ 35 HP và có VFX/feedback.

Trạng thái: `VO_LV1_FULL_FRAME_MOTION_PASS / ANIMATED_PAPER_DOLL_ATTACHMENT_INCOMPLETE`. Evidence: `build/vo-motion-v8/three-profiles/`, 114 ảnh (38×3), manifest xác nhận Q01–Q09 và bốn state mới ở cả hai giới. Chưa claim Võ Lv1–30 hoàn chỉnh: full-frame chỉ dùng cho Lv1/full; modular và Lv10/20/30 vẫn giữ slot đúng nhưng dùng root pose. Next là attachment/rig 10 slot, không mở Kiếm/Pháp/Cơ/Linh.

## Võ modular skeletal base gate — 2026-09-10

`VO_SKELETAL_BASE_RIG_PASS / EQUIPMENT_COMPONENT_BINDING_INCOMPLETE`: runtime có 10 body segment cho mỗi giới và pose profile data-driven xoay quanh joint pivot; modular không còn dùng một base sprite đứng phẳng. Source QA đúng 10 connected component/sheet, atlas rig 1024² chỉ 24.232 byte; tổng bảy atlas 724.795 byte. Player capture 46×3 đã review: thân/limb chuyển động rõ hơn, nhưng slot đôi vẫn chưa tách theo bone và gear Võ nữ Lv30 còn lệch. Next bắt buộc là split/bind equipment component trước khi đóng Võ hoặc nhân pipeline.
## Võ base-first skeletal hierarchy — 2026-09-10

Đã chuyển prototype Võ từ sprite pivot độc lập sang bone hierarchy cha-con và đưa phần dựng rig/attachment vào base `TwoDSkeletalPaperDollRig`. Manifest v7 giữ 7 atlas/724.795 byte, thêm 96 attachment metadata cho đủ 10 slot ở Lv1/10/20/30, nam/nữ; pose world được chuyển sang local rotation để không cộng dồn sai. EditMode gần nhất 181 total/180 pass/0 fail/1 ignored; Player capture ba profile technical pass. Visual review xác nhận tay/chân nối tốt hơn, nhưng garment nữ Lv30 còn chưa khớp silhouette production. Trạng thái: `VO_BASE_FIRST_RIG_TECHNICAL_PASS / GARMENT_ART_VISUAL_FIX_REQUIRED`; chưa mở class khác.

## Võ garment attachment v8 — 2026-09-10

Đã chọn/tạo đủ một lượt 8 sheet Võ Lv1/10/20/30 nam-nữ theo layout 4×3, xử lý nền liên thông từ viền và đóng 112 attachment vào 7 atlas 1024²/885.234 byte. EditMode 180 pass/0 fail/1 ignored; macOS Player build v18 không lỗi/cảnh báo; capture 46×3 và review mắt xác nhận Lv30 nữ có trang phục rõ trong bốn action modular. Trạng thái `VO_LV1_30_GARMENT_BATCH_PLAYER_PASS / TEN_SLOT_VISUAL_MATRIX_PENDING`; chưa claim class hoàn chỉnh và chưa mở class khác.

## Võ ten-slot matrix + base head v9 — 2026-09-10

Matrix đầu bắt lỗi tóc bake trong base head; đã thay base đầu không tóc cho nam/nữ và giữ toàn bộ tóc ở attachment. Manifest v9: 112 equipment component, 7 atlas/884.660 byte. EditMode 182 total/181 pass/0 fail/1 ignored; Player v20 và capture 66×3 tại `build/vo-ten-slot-matrix-v2/three-profiles/` technical pass. Review 20 slot-off xác nhận đủ 10 slot ở nam Lv1/nữ Lv30, gồm slot nhiều bone. Trạng thái `CLASS_VO_LV1_30_VERTICAL_SLICE_PASS`; next tách character/action orchestration base, chưa mở class khác.
## Shared character runtime state gate — 2026-09-10

Đã tách selection/loadout/action timing Võ khỏi `CongDongLamMap01AArtPreview` vào `TwoDCharacterRuntimeState` dùng chung. EditMode `184/183/0/1`; macOS Player v22 `168.488.627` byte, 0 error; capture `build/vo-shared-state-v2/three-profiles/` đạt 66 frame cho mobile/tablet/PC, `voSharedRuntimeStateVerified=True`, 10-slot matrix/Q01–Q09/combat giữ nguyên và đã review contact sheet. Trạng thái `SHARED_CHARACTER_RUNTIME_STATE_PLAYER_PASS`. Next là một batch source/atlas Kiếm Lv1–30 nam/nữ trên base này; chưa mở class thứ ba.
## Kiếm Lv1–30 source candidate + compatibility gate — 2026-09-10

Đã audit và batch-extract 80 ô `2 giới × Lv1/10/20/30 × 10 slot` thành source-v4 ngoài repo. Contact review xác định 12 crop nữ phải redraw vì còn dính body/da/tay/chân; 68 crop còn lại vẫn cần fit base, tất cả runtime eligible bằng 0. Runtime contract mới kiểm `approved + class allow-list + skeletonVersion + bodyProfile + attachment bone`, loadout map item độc lập theo slot và cho phối chéo level sau unlock. TDD cuối `191 total / 190 pass / 0 fail / 1 ignored`; macOS Player build `168.494.995` byte, 0 error; onboarding smoke PASS. Trạng thái `KIEM_LV1_30_SOURCE_CANDIDATES_EXTRACTED / BASE_FIT_UNVERIFIED`. Next fit/redraw theo lô rồi mới pack atlas/runtime.

Kiểm góc nhìn xác nhận grid cũ không phù hợp paper-doll side-view. Batch-v2 đã tạo hai redraw sheet 4×10 cho nam/nữ; nữ v1 fail da/tay, nữ v2 đã sửa giới hạn hai row. Extractor thêm magenta feather/despill và tạo `source-v3` đủ 80 cell, 1.837.558 byte. Trạng thái `KIEM_SIDEVIEW_REDRAW_SOURCE_READY / SPRITESKIN_FIT_UNVERIFIED / RUNTIME_ELIGIBLE_0`. Next là SpriteSkin fit spike có benchmark, không pack atlas hoặc claim runtime art trước motion capture.

Unity 2D Animation 13.0.0 đã compile trên Unity 6000.3.2f1. Sprite Library adapter map `componentId/itemId`, mixed-level apply theo transaction và tách Rigid/Skinned; Skinned sprite rỗng bone data bị chặn. EditMode `195/194/0/1`; benchmark Player cuối 168.812.074 byte, tăng 317.079 byte (0,188%). Trạng thái `KIEM_SPRITE_LIBRARY_COMPATIBILITY_PASS / VISUAL_FIT_UNVERIFIED`; next weight/fit đúng bốn item mixed-level cho cả nam/nữ.

Prefit bốn item phát hiện hair/outer source-v3 còn nhiều view; đã redraw cục bộ và normalize PPU 208, không sinh lại weapon/inner hoặc 72 item ngoài proof. Contact idle nam/nữ đã review, kích thước normalized 23–33 KB/file. Trạng thái `KIEM_MIXED_LOADOUT_IDLE_PREFIT_PASS / MOTION_UNVERIFIED / RUNTIME_ELIGIBLE_0`; next import/weight đúng 8 file proof và capture sáu state.

Đã import đúng 8 proof item vào một atlas 512×512: 6 Skinned có Sprite bones/weights do authoring tool tái tạo, 2 kiếm Rigid; PPU 208, PNG 173.683 byte. Validator khóa atlas/hash/provenance; EditMode `204/203/0/1`, Player 169.088.250 byte/0 error, tổng delta 0,352% so baseline và giảm 202.672 byte so bản 8 texture rời. Player motion chưa kiểm nên trạng thái giữ `KIEM_PROOF_ASSET_AUTHORING_PASS / PLAYER_MOTION_UNVERIFIED / RUNTIME_ELIGIBLE_0`.

Kiếm fit preview đã nối 4 món mixed-level nam/nữ vào skeleton chung bằng bone-proxy adapter, sau khi review và sửa lỗi bind trực tiếp làm garment tụt xuống chân. Production equip vẫn chặn `DraftRuntimeFit`; EditMode `206/205/0/1`, Player 169.099.226 byte/0 error và capture 78×3 kiểm idle/walk/run/jump/basic/shared-skill pose; UI ghi DRAFT. Trạng thái `KIEM_MIXED_LOADOUT_SHARED_RIG_FIT_PASS / KIEM_FULL_10_SLOT_INCOMPLETE / KIEM_SKILL_PRODUCTION_INCOMPLETE / RUNTIME_ELIGIBLE_0`.

Đã pack ngoài runtime toàn bộ 48 candidate thuộc 6 slot Kiếm còn thiếu thành atlas nam/nữ 1024, không resize và có rect/hash/provenance. Visual batch review xác nhận art direction đồng nhất; chưa fit base nên không nhập Unity và vẫn eligible 0. Trạng thái `KIEM_REMAINING_SIX_SLOT_SOURCE_BATCH_READY / BASE_FIT_REQUIRED`.
## Hiện hành — Võ Lv1 HD được owner chấp nhận sơ bộ; thêm hành trang 10 món thao tác trực tiếp — 2026-09-11

Owner đánh giá Player Lv1 HD “có vẻ ổn định hơn” và yêu cầu khóa lại để tiếp tục. Bộ khóa vẫn là `legacy-base-run-contact-jump-v3-div4-ten-slot-lv001-hd-review-v2`: body/motion v3 div4, overlay div2, cùng canvas/pivot/scale/camera và một actor. Source art giữ trạng thái review đã chấp nhận sơ bộ; không khôi phục các pack nhiều level từng bị reject.

Player mới bổ sung Hành trang Võ vào HUD Map01A: panel tự mở trong phiên POSE THỬ tương tác, hiển thị đủ 10 slot theo lưới hai cột, item ID, level, fit/base, sáu pose và trạng thái đang mặc/đã tháo. Mỗi hàng chọn trực tiếp bằng chuột; nút mặc/tháo cập nhật đúng slot trên actor. `Đổi cấp món` chỉ bật khi slot thực sự có variant, chuẩn bị cho Lv10/mặc chéo. Nhãn world POSE THỬ được chuyển sang cạnh actor để không che nút Phụ kiện Võ. Test SourcePose + state/UI `16/16`; test callback riêng dựng Map01A + `UIDocument`, đếm 10 hàng và xác nhận chọn/tháo Áo ngoài. Player `client/Unity/build/vo-lv1-inventory-player-v6/LinhGioiOnline.app` build thành công `171955242` byte, 0 error/13 warning; capture `build/vo-lv1-inventory-runtime-v6/pc` đạt 154 frame, 40 toggle, sáu pose và `errors=[]`. Visual evidence panel: `build/vo-lv1-inventory-window-v5.png` và `build/vo-lv1-inventory-window-v5-key-toggle.png`; đã review đủ 10 hàng, không cắt/che và trạng thái tháo cập nhật rõ.

Tham khảo độc lập cho hướng dài hạn: Unity Sprite Swap dùng category/label để nhiều ngoại hình dùng chung skeleton/mesh; Spine mix-and-match kết hợp item skins trên cùng skeleton. LGO tiếp tục frame-by-frame HD vì cần giữ nét cel/source pose, nhưng khóa cùng nguyên tắc: slot ID ổn định, một body authority, source canvas/pivot chung và variant theo level; không tạo controller/offset riêng theo item.
