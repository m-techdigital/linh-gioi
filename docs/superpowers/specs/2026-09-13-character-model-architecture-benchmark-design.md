# Character Model Architecture Benchmark Design

**Gate:** `LGO_CHARACTER_MODEL_ARCHITECTURE_REVIEW_01`

**Current execution status, 2026-09-13:** This benchmark is no longer the active sandbox path after owner fallback to the six-pose registered outfit pipeline. Keep it as guard/history. Do not resume the current skeletal generated-cutout work without a new accepted neutral layered body/rig blueprint.

## Mục tiêu

Chọn đơn vị sản xuất nhân vật có thể mở rộng cho tủ đồ Linh Giới bằng một benchmark hữu hạn, có thể lặp lại. Pipeline ảnh theo sáu pose hiện hành là baseline chi phí và chất lượng; nó không còn là kiến trúc mặc định. Hai candidate duy nhất là:

- `skeletal_2d`: nhân vật modular chạy trực tiếp bằng skeleton 2D. Probe đầu dùng Unity 2D Animation 13 đã có trong project; Spine chỉ được thay vào khi có bằng chứng license, exporter và runtime package.
- `modular_3d`: nhân vật modular 3D chạy trực tiếp trong Unity với camera gameplay ngang/orthographic. Blender là công cụ authoring; không xuất lại thành sprite trong candidate này.

Benchmark không thay gameplay/server/protocol/schema/ADR, không promote candidate trang phục hiện hành và không sửa body/pose cũ để làm đẹp kết quả.

## Kết quả probe skeletal 2D hiện tại

`bind-authority-candidate-v1` và Player probe từ generated cutout source bị owner reject sau review thực tế. Lý do sản phẩm: chân tay đọc như rời rạc, tỷ lệ cơ thể không khớp design 1:1, jump/flip không thể dùng làm nền mặc đồ, và technical pass không phản ánh chất lượng hình ảnh cuối. Candidate này đã chuyển sang `OWNER_REJECTED_VISUAL` và có marker `DO-NOT-PACK.md` trong selected-source external tree.

Điều này không đóng toàn bộ hướng `skeletal_2d`, nhưng đóng nguồn body/cutout hiện tại. Planner phải trả `AUTHOR_SKELETAL_2D_SOURCE_BLUEPRINT` khi manifest có `probeStatus=OWNER_REJECTED_SOURCE`. Next action hợp lệ là author hoặc nhận một neutral layered body/rig blueprint mới theo `docs/art/LGO-SKELETAL-2D-SOURCE-BLUEPRINT-SPEC-v1.md`, rồi chạy lại source admission và visual body gate trước garment. Nếu không có blueprint mới đủ chuẩn, benchmark phải chuyển sang `modular_3d` hoặc ghi owner decision, không tiếp tục cứu source cũ bằng animation curve, overlap, mask hay pixel edits.

## Đơn vị đối chứng chung

Ba hướng `pose_sprite_baseline`, `skeletal_2d` và `modular_3d` dùng cùng một brief hình ảnh, cùng chiều cao hiển thị và cùng tập hành vi:

- neutral swappable body;
- `upper`, `lower`, `footwear`, `waist` và một rigid hand item;
- `idle`, `run`, `jump`, `attack`, `return_to_idle`;
- tháo từng nhóm, phối item giữa hai mốc hình ảnh;
- sau khi khóa tooling/template, đưa vào một `unseen_item` không có chỉnh sửa pixel theo pose;
- chạy lại các tổ hợp cũ để phát hiện hồi quy.

Kích thước nguồn 2D kế thừa profile `lgo_character_canvas_1024x1536_v1`: canvas 1024×1536, origin X 512, ground Y 1484 và `u = 1.70 / 1536`. Pose co người hoặc nhảy được đổi bounding box tự nhiên; root/body không bị normalize về cùng chiều cao. Candidate 3D phải khớp cùng chiều cao hiển thị trong gameplay, nhưng không giả metadata source-space 2D cho mesh 3D.

Mọi component nguồn skeletal đi qua `LGO_SKELETAL_SOURCE_CONTRACT_01` trước layer import: đúng canvas/profile đăng ký, có alpha thật và có cả pixel trong suốt lẫn pixel hiển thị, slot hợp lệ, ownership chỉ thuộc vùng được phép của slot. Checkerboard RGB, crop riêng từng component, hoặc garment gộp vùng của slot khác đều bị reject trước khi rig. Validator này là admission gate kỹ thuật; pass không đồng nghĩa art đẹp hoặc ownership đã được mắt người duyệt.

## Evidence contract

Mỗi candidate có một record JSON với các trường bắt buộc:

- `toolchain.status`: `READY`, `BLOCKED` hoặc `UNVERIFIED`, cùng phiên bản và provenance;
- `commonTask`: các danh sách slot/action đã chạy và boolean cho neutral body, unequip, mixed loadout, unseen item, regression;
- `manualIntervention`: số thao tác source, số lần sửa pixel theo pose, số vòng làm lại và phút cho unseen item;
- `changeSurface`: số file source, code và generated artifact phải đổi để thêm unseen item;
- `automation`: replay từ input đã khóa có xác định hay không, output có provenance/hash hay không;
- `motionContinuity`: ba chuỗi `start_stop_direction_change`, `run_jump_fall_land_run`, `run_attack_run`; review sample giữa keyframe; đổi đồ giữa chuyển động mà không reset animation time; root scale ổn định; một thành phần sở hữu transform; locomotion theo vận tốc thực;
- `deformationIntegrity`: drift chiều dài xương cơ thể tối đa 0,1%; drift cạnh của item cứng tối đa 0,1%; socket rigid lệch tối đa một source pixel; không lật tam giác mesh mềm; seam gap tối đa hai source pixel; phần deform phải dùng authored weights; review body-only/full/mixed tại cùng timestamp;
- `visualReview.status`: `APPROVED`, `REJECTED` hoặc `PENDING`, kèm artifact đã xem;
- `performance.pc` và `performance.mobile`: trạng thái đo cùng frame-time p95, memory và draw/batch count khi có. Ảnh macOS theo tỷ lệ mobile không được ghi là đo thiết bị mobile thật;
- `hardFailures`: lỗi cấu trúc như xuyên đồ không khắc phục trong family, mất silhouette, không tháo/phối được hoặc cần sửa từng pose.

Record baseline dùng cùng trường chi phí và regression khi có thể, nhưng không được chọn làm kiến trúc thắng; nó chỉ chứng minh candidate mới có giảm lặp hay không.

## Gate và quyết định

Planner chỉ cho phép đi theo các trạng thái sau:

1. Thiếu baseline định lượng → `BASELINE_EVIDENCE_REQUIRED`.
2. Candidate chưa có toolchain/probe → `RUN_SKELETAL_2D_PROBE` hoặc `RUN_MODULAR_3D_PROBE`.
3. Source/body của skeletal probe bị owner reject → `AUTHOR_SKELETAL_2D_SOURCE_BLUEPRINT`.
4. Có probe nhưng thiếu task/evidence → `COMPLETE_COMMON_TASK_EVIDENCE`.
5. Evidence kỹ thuật đủ nhưng chưa có review mắt → `NEED_HUMAN_VISUAL_REVIEW`.
6. Hai candidate đủ gate → `BENCHMARK_READY_FOR_DECISION`.
7. Một candidate có hard failure đã tái hiện và candidate kia đủ gate → `BENCHMARK_READY_FOR_DECISION`, giữ failure làm evidence, không chạy vô hạn để cứu candidate.

Một candidate chỉ `eligible` khi hoàn thành toàn bộ task, không sửa pixel theo pose cho unseen item, không làm hỏng tổ hợp cũ, automation có provenance, không còn hard failure, có review hình ảnh `APPROVED`, và có đo PC thật. Mobile thật là gate trước production promotion, không ngăn quyết định kiến trúc thử nghiệm nếu được ghi `DEFERRED_DEVICE_REQUIRED`.

Có clip `idle/run/jump/attack` chưa đủ cho motion gate. Video Player tốc độ thật phải chứng minh các chuỗi chuyển trạng thái, rồi sample chậm các khoảng giữa keyframe để bắt rời khớp/xuyên đồ. Đổi item khi đang chạy hoặc nhảy phải giữ animation time và áp toàn bộ visual loadout trong cùng frame; asset chưa sẵn sàng thì giữ loadout cũ. Candidate không được dùng cùng một mix duration cho mọi transition để đạt test hình thức.

Input → movement state → animation state → body/equipment là một chiều dữ liệu. Vận tốc/collision thực quyết định locomotion; animation không cùng lúc kéo world transform với movement controller. Camera/frame interpolation là phần integration phải đo trong Player, không phải thuộc tính tự có của skeletal runtime.

## Đánh giá attachment b01f — chuyển động mượt

Nhận xét được chấp nhận làm phần mở rộng gate, không làm bằng chứng `skeletal_2d` đã đạt. Tài liệu Spine xác nhận `AnimationState` hỗ trợ update/apply theo thời gian, mix theo từng cặp và nhiều track; điều này chứng minh runtime có primitive phù hợp, chưa chứng minh source rig, transition policy hay controller Linh Giới đúng. Physics constraint là công cụ cho chuyển động phụ, không phải collision cloth hoặc bảo đảm chống xuyên.

Unity mô tả Rigidbody2D interpolation và Cinemachine update mode để giảm judder khi nhịp physics/render khác nhau. Benchmark phải đo frame pacing và camera trong Player; không bật một setting rồi tự ghi PASS. Mục tiêu 60 FPS/16,7 ms là budget ban đầu, còn pass/fail phải dựa trên p95 và spike thực tế của scenario.

Nguồn chính thức đã kiểm:

- [Spine — Applying Animations](https://esotericsoftware.com/spine-applying-animations): `AnimationState`, mix time theo cặp và animation tracks.
- [Spine — Physics Constraints](https://esotericsoftware.com/spine-physics-constraints): constraint chuyển động phụ cần author/configuration riêng.
- [Unity — Rigidbody 2D interpolation](https://docs.unity3d.com/6000.0/Documentation/Manual/2d-physics/rigidbody/interpolation.html): interpolation/extrapolation xử lý khoảng giữa physics update.
- [Unity Cinemachine Brain](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineBrain.html): update method phải phù hợp cách target được cập nhật.

Network prediction/reconciliation và remote interpolation là gate của vertical slice sau khi chọn kiến trúc local. Hai candidate sẽ dùng cùng controller/network contract ở phase đó; đưa network rewrite vào benchmark authoring hiện tại sẽ thêm biến nhiễu và đụng phạm vi bị khóa.

## Đánh giá attachment 488a — giữ phom khi deform

Nhận xét được chấp nhận và làm thay đổi gate: motion liên tục không còn đủ để đạt. Mix and Match/Stretchyman chỉ là bằng chứng tổ chức attachment; tính đàn hồi trong demo không phải chuẩn anatomy Linh Giới. Candidate phải tắt body-bone stretch trong chuyển động thường và chứng minh chiều dài segment giữ ổn định; bounding-box nhân vật được co tự nhiên khi tuck, không normalize chiều cao pose.

Không dùng một deformation policy cho mọi vật liệu. Vũ khí, khóa đai, huy hiệu và plate giáp dùng rigid transform và phải giữ edge length/socket. Thân/tay/vạt áo được tách theo cấu tạo, dùng authored weights và seam contract; mesh mềm được nén/uốn nhưng không lật triangle. Khi đổi mặt nhìn thật như lòng bàn tay, đế giày hoặc mặt trong vạt, timeline attachment swap được phép. Số attachment đặc biệt của unseen item được ghi trong cost vector; nếu mỗi item cần thêm hàng loạt ảnh pose-specific thì candidate tự mất lợi thế, không được che bằng cách gọi đó là skeletal.

Review deformation lấy cùng timestamp/camera cho ba lớp `body_only`, `full_outfit`, `mixed_loadout`. So sánh này tách lỗi anatomy, garment và occlusion. Script được phép đánh dấu drift/gap/inversion; chỉ video Player tốc độ thật, slow review và mắt người mới quyết định chất lượng hình ảnh.

Planner không dùng điểm tổng hợp. Nó xuất vector chi phí/hiệu năng và quan hệ Pareto. Nếu một candidate không tệ hơn ở mọi thước đo chính và tốt hơn ít nhất một thước đo, nó là `dominantCandidate`; nếu hai hướng đánh đổi nhau, trạng thái là `OWNER_TRADEOFF_REQUIRED` với đúng các chiều xung đột. Chất lượng hình ảnh là gate, không đổi thành điểm để bù bằng tốc độ.

## Chống vòng lặp

- Không sửa từng pixel/pose sau khi template khóa; phát hiện một lần là candidate fail gate unseen-item.
- Không chạy lại probe khi input hash, tool version và code hash không đổi.
- Không color-key/inpaint/crop để cứu source đã fail alpha/canvas/ownership; đổi source method và giữ candidate lỗi với provenance.
- Cùng hard failure lặp hai lần thì đóng candidate hoặc thay thiết kế nguồn/family; không tiếp tục dò thông số.
- Mỗi candidate có tối đa hai vòng source-level correction trước unseen item. Vòng thứ ba là bằng chứng quy trình chưa đủ ổn định.
- Chỉ build/capture sau khi một candidate đã hoàn thành cả common task; một batch capture cho PC và một batch thiết bị mobile khi có.

## Phạm vi triển khai batch đầu

Batch đầu tạo planner có test và một inventory evidence từ trạng thái thực: Unity 2D Animation 13 hiện diện, Blender được probe, Spine runtime chưa hiện diện, cả hai candidate chưa hoàn thành common task. Kết quả hợp lệ phải yêu cầu chạy `skeletal_2d` probe trước, vì dependency đã nằm trong project và không phát sinh license mới. Đây là thứ tự thử, không phải tuyên bố candidate A thắng.
