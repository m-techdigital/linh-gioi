# Đánh giá đề xuất Single Image → Modular Rigid 2D Character

Ngày đánh giá: 2026-09-15
Nguồn được phản biện: file đính kèm `07cf0822-c65f-478a-8f83-9b1706baf55c/pasted-text.txt`
Đề xuất bổ sung được phản biện: file đính kèm `c242ea8d-048e-4c51-8d61-f6589509889c/pasted-text.txt`
Phạm vi: pilot rigid outfit nam/nữ hiện hành; chưa mở class, level hoặc migration production.

## Quyết định

Giữ **kiến trúc** của đề xuất, không chọn nguyên xi **chuỗi công cụ** của đề xuất.

Các nguyên tắc được nhập vào pilot:

- một master reference cố định kèm hash, canvas, origin, ground và landmark;
- một item logic có thể gồm nhiều sprite cứng theo bone;
- hidden overlap phải tồn tại trong editable source trước khi animation;
- một skeleton contract và một fit profile cho mọi animation;
- equip-time replacement được phép, pose/frame/angle replacement bị cấm;
- PNG RGBA + manifest là đầu vào Unity;
- validator chỉ mở visual review, không tự công nhận hình ảnh đạt.

Đường triển khai hiện hành vẫn là **Blender editable volumetric rigid source → PNG RGBA → Unity SpriteRenderer/Transform**. SAM2, Grounded-SAM-2, Krita AI Diffusion, ComfyUI và Sprite Library không nằm trên critical path của pilot này.

## Phần đúng và có giá trị lâu dài

### Master reference và đăng ký tọa độ

Đề xuất đúng khi yêu cầu khóa ảnh nguồn và overlay mọi đầu ra về cùng master. LGO đã có profile mạnh hơn một metadata chiều cao đơn lẻ:

- canvas `1024×1536`;
- origin X `512`;
- ground Y `1484`;
- `u = 1.70 / 1536`;
- landmark nam/nữ và SHA-256 của candidate v9.

Các giá trị này dùng để đăng ký camera, pivot và tỷ lệ một lần. Chúng không được biến thành offset riêng theo pose hoặc item.

### Logical item gồm nhiều rigid visual part

Đây là phần phù hợp nhất với mục tiêu LGO. Một áo vẫn là một gameplay item nhưng có thể sở hữu `torso`, `upper_arm_near`, `upper_arm_far`, `lower_arm_near`, `lower_arm_far`, `waist_front` và `waist_back`. Mỗi part có đúng một sprite form, một bone, một pivot và một render role. Animation chỉ điều khiển bone.

### Hidden geometry và overlap

File đúng khi nhấn mạnh phải dựng phần bị che. Mask của pixel đang nhìn thấy không đủ để xoay vai, khuỷu, hông hoặc gối. Pilot chỉ cho phép rig sau khi editable source chứa surface ẩn và contact sheet xoay khớp không hở.

### PNG + manifest và kiểm chứng trong Player

PNG RGBA riêng cùng manifest dễ kiểm hash, alpha, pivot, atlas và ownership hơn việc phụ thuộc cấu trúc layer của một ứng dụng. Đây là boundary phù hợp giữa source authoring và Unity. Runtime phải chụp lại sprite reference, local fit và scale trước/sau các state để chứng minh bất biến.

## Phần phải sửa

### SAM2 chỉ là mask bootstrap

SAM2 nhận point/box/mask prompt và trả mask của vùng ảnh nhìn thấy. Nó không biết bone ownership của LGO, không phân biệt chắc chắn near/far limb ở vùng chồng và không suy ra surface bị che. Grounded-SAM-2 bổ sung text grounding, nhưng repository chính thức mô tả nó là chuỗi Grounding DINO + SAM2 và cài đặt local khuyến nghị CUDA/Linux. Máy hiện tại là Apple Silicon, vì vậy không được coi đây là capability đã sẵn sàng.

Nếu dùng trong tương lai, output của SAM2 chỉ được mang vai trò `MASK_CANDIDATE`; nó phải qua semantic ownership, hidden-surface và whole-board review. Mask không được tự động trở thành source part.

### Inpainting không phải reconstruction có thể chứng minh

Krita AI Diffusion hỗ trợ inpaint, ControlNet và IP-Adapter qua ComfyUI, nhưng output vẫn là pixel được suy đoán. Plugin yêu cầu backend, model và dung lượng đáng kể; tài liệu của plugin cũng nêu Apple Silicon dùng MPS và có thể chậm hoặc lỗi tùy workflow. Quan trọng hơn, inpaint đẹp ở bind pose không chứng minh joint giữ kín qua toàn range of motion.

Vì vậy inpaint chỉ có thể tạo candidate cho vùng ẩn. Candidate phải tồn tại trong editable source, giữ pixel gốc ngoài mask, có provenance và qua rotation sweep. Pilot hiện tại không dùng capability này vì Krita GUI/Scripter automation đã không chạy ổn định trong sandbox và chưa có model/workflow được kiểm chứng.

### SpriteResolver không phải điều kiện để fit once

Unity Sprite Library/Resolver thực sự có thể đổi sprite theo Category + Label. Điều đó phù hợp cho equip-time swap, nhưng cũng tạo thêm một surface có thể bị animation hoặc state logic điều khiển sai. Repo đã có `RigidOutfitVisualBundle` và binder trực tiếp bằng sprite reference. Pilot giữ abstraction hiện có để giảm biến số; validator cấm mọi object-reference curve và state/angle/frame dependency. Có thể đánh giá Sprite Library như adapter authoring sau khi pilot đạt, không đưa vào Final #1.

### Không cho phép “scale cực nhỏ”

File cho phép scale nhỏ khi cần; điều này xung đột trực tiếp với contract owner. Body, bone và equipment giữ `(1,1,1)` trong bind và toàn bộ animation. Chuyển động chỉ dùng position/rotation và root translation hợp lệ.

### Không mặc định 2K–4K

Repo yêu cầu chốt pixel hiển thị và texture budget trước khi tạo asset. Pilot dùng source profile `1024×1536`; Blender render trực tiếp đúng budget và cùng camera. Chỉ tăng độ phân giải khi visual review chứng minh chi tiết bị mất và budget được cập nhật có chủ đích.

## Vì sao không chọn pipeline trong file làm critical path

Chuỗi trong file có hai bước thủ công/không xác định chất lượng: sửa mask và vá occlusion. Nếu chạy tự động hoàn toàn, hai bước này có thể tạo hình đẹp ở một pose nhưng sai ownership hoặc thiếu hidden surface. Nếu sửa tay, kế hoạch không còn là Codex tự xử lý toàn bộ. Thêm Grounded-SAM-2, ComfyUI, Krita và Unity Sprite Library cùng lúc cũng làm tăng bốn nguồn lỗi trước khi chứng minh được một nhân vật.

Blender source route không bảo đảm art sẽ đạt. Ưu điểm của nó là tạo được geometry ẩn, pivot và source mở lại bằng một toolchain có command line; khi thất bại, lỗi được quy về `DESIGN_SOURCE` thay vì bị che bởi mask/inpaint/runtime. Pilot giới hạn hai revision thiết kế. Nếu revision 02 vẫn mang dáng mannequin/capsule hoặc sai identity, giả thuyết tự động bị bác và trạng thái phải là `BLOCKED_BY_ASSET`; Codex không tiếp tục rig hay Unity để tạo pass giả.

## Đánh giá đề xuất Apple Vision + LGO Cutout Builder + Moho

### Apple Vision: chỉ giữ như intake accelerator

API là có thật. Apple Vision cung cấp body pose joints và foreground instance mask native trên macOS. Apple cũng ghi rõ pose detection giảm chất lượng khi người cúi, lộn, bị che hoặc mặc đồ rộng. API trả joint observation và whole-subject mask; nó không trả semantic pixel layer cho từng upper arm/thigh, không dựng surface bị che và không xác lập render ownership.

Probe đã chạy trên chính hai clean master v9:

| Kết quả | Male | Female |
|---|---:|---:|
| Body observation | 1 | 1 |
| Recognized points | 19 | 19 |
| Foreground instances | 1 | 1 |
| Core landmark RMS so với profile đã đăng ký | 44.00 px | 43.37 px |
| Head confidence | 0.1069 | 0.1511 |

Vision nhận bố cục cơ thể khá tốt ở vai và một số cổ chân, nhưng sai 40–60 px tại nhiều cổ tay, hông và gối; head confidence rất thấp. Với base hiện tại, profile v9 đã đăng ký chính xác hơn nên Vision không giảm công việc. Với base mới trong tương lai, Vision có thể đề xuất điểm lần đầu; kết quả phải được so với profile và visual gate trước khi ghi contract.

Evidence: `build/rigid-outfit-pilot/capability-evidence/apple-vision-v1/vision-probe.json` và `landmark-comparison.json`.

### Deterministic overlap từ ảnh phẳng: không đủ tạo source tự nhiên

Ý tưởng đẩy upper arm vào dưới torso đúng ở cấp topology, nhưng ảnh phẳng không chứa màu, nét và thể tích của đoạn ẩn. Kéo dài pixel biên hoặc gắn cap tròn chỉ tạo topology kỹ thuật; các thử nghiệm flat-card/capsule cũ đã cho thấy kết quả có thể không hở alpha nhưng vẫn đọc như chân tay rời và sai cơ thể. Current Blender source route tạo surface ẩn trong cùng editable volume trước khi render, sau đó mới kiểm overlap bằng rotation sweep.

### Angle-dependent elbow sprite: bác vì vi phạm contract

`elbow.straight/mid/high` được chọn theo góc vẫn là angle/pose-dependent sprite swap, dù upper arm và forearm là rigid. Rule owner cấm đúng cơ chế này. Nếu cần che seam, pilot dùng một seam-cover sprite cố định luôn tồn tại và parent vào một bone hoặc redesign silhouette/overlap. Không có state sprite theo khoảng góc.

### Sáu pose là key reference, không phải giới hạn runtime

Sáu pose cũ có giá trị làm authority cho contact, high-knee và tuck. Final #1 phải chạy clip liên tục ở gameplay speed qua Idle/Walk/Run/Jump/Attack/Roll/Hit và transition. Qua sáu ảnh rời không chứng minh interpolation, foot sliding, transition pop hoặc seam ở giữa key.

### Moho không giải quyết blocker hiện tại

Moho có rig, IK và scripting, phù hợp khi đã có layered artwork. Máy hiện không cài Moho; thêm license/tool mới chỉ thay Blender ở đoạn downstream đã được chứng minh. Nó không biến flattened PNG thành layered source đúng hoặc sinh hidden geometry. Không tải, mua hoặc thử Moho trong pilot này.

### Rive và Cartoon Animator bị loại khỏi player-character path

Rive hỗ trợ Unity 6 nhưng runtime chính thức render qua RenderTexture/Rive renderer và tài liệu nói không phải drop-in SpriteRenderer. Điều đó không phù hợp bundle nhiều SpriteRenderer, SortingGroup và atlas contract hiện tại. Cartoon Animator 5 không có bản macOS theo thông báo chính thức của Reallusion. Hai nhánh này không được nghiên cứu tiếp cho pilot nhân vật.

## Kế hoạch áp dụng

Execution authority: `docs/superpowers/plans/2026-09-15-rigid-outfit-pilot-production.md`.

1. Bootstrap Blender 4.5.13 LTS có checksum/codesign và chạy background probe.
2. Tạo neutral male/female từ profile v9 bằng object volume có contour riêng, hidden overlap và toon material; xuất `.blend`, clean board và dimension board.
3. So camera, landmark, ground và silhouette envelope với master reference; Codex mở board để review. Tối đa hai grouped revision.
4. Chỉ sau `DESIGN_VISUAL_PASS`, tạo canonical bones và body-only Idle/Walk/Run/Jump/Attack/Roll/Hit. Xuất 15-key sheet và clip gameplay-speed.
5. Tạo outfit #1 + weapon theo interface bone/pivot cố định; kiểm all-on, slot-off, seam-pair và alpha sweep trong Blender.
6. Render từng rigid part từ cùng camera thành PNG RGBA, sinh manifest/hash/pivot và mở lại `.blend` để audit.
7. Tích hợp vào vertical slice Unity v3 hiện có, chạy invariant tests và graphics Player capture cho male/female. Codex review clip/contact sheet. Đây là Final #1; dừng chờ owner review.
8. Sau khi Final #1 được duyệt, tạo outfit #2 cùng họ bằng đúng interface/fingerprint, không đo lại body và không thêm offset. Sau đó test mix slot và một animation skeleton-only mới.

## Evidence thật đã chạy trong lần đánh giá

Evidence index: `build/rigid-outfit-pilot/capability-evidence/evidence-index-v1.json`.

- Blender 4.5.13 LTS macOS ARM64 được tải từ release chính thức. SHA-256 local trùng manifest chính thức: `663ce944257c61ff1d6aa09e15c8f57bbd8d59023adb2fa7edde33a9ed960b53`; codesign hợp lệ.
- `tools/probe_lgo_blender_rigid_capabilities.py` đã tạo scene diagnostic có ba object cứng parent vào ba bone, lưu `.blend`, rồi một Blender process mới mở file và render lại. Audit không tìm thấy modifier, vertex group, shape key, non-unit scale hoặc scale curve; PNG có bốn channel và alpha thật. Decoded pixel hash trước/sau reopen giống nhau.
- Unity 6000.3.2f1 hiện có 24/24 targeted `RigidOutfitArchitectureTests` đạt trong lần chạy mới. Invariant gate mới chạy trả `PASS` cho năm rule: không SpriteSkin/mesh, không animated sprite, không animated scale, không animation binding equipment và không pose-dependent equipment logic.
- Apple Vision probe trên hai master v9 tìm được một body, 19 điểm và một foreground instance cho mỗi ảnh, nhưng core RMS là `44.00/43.37 px` và head confidence thấp. Evidence này giới hạn Vision ở vai trò gợi ý landmark cho intake tương lai.
- Contact sheet runtime v4 cũ đã được xem lại và vẫn là evidence bị bác: cơ thể/trang phục rời, pose và che khớp không đạt. Kết quả test xanh ở trên chỉ xác nhận code architecture; nó không nâng visual cũ thành source hợp lệ.

Phần chưa có evidence và vẫn là gate bắt buộc: neutral male/female production-quality, joint-safe body motion từ source đó, outfit #1 trong Player, outfit #2 reuse không remeasure. Bước tiếp theo vì vậy là `DESIGN_SOURCE_GATE_REVISION_01`, không phải viết thêm framework hoặc chạy lại Unity visual cũ.

## Điều kiện đánh giá lại quyết định

Chỉ mở lại SAM2/inpaint route khi có đủ cả ba bằng chứng:

1. toolchain chạy được headless trên máy này với model/hash cố định;
2. một ảnh LGO thật tạo được near/far semantic masks và hidden-surface candidates có provenance;
3. cùng source đó qua joint range sweep mà không cần pixel edit, pose offset hoặc sprite swap.

Thiếu một trong ba điều kiện thì segmentation vẫn là công cụ hỗ trợ selection, không phải pipeline production.

## Nguồn chính thức đã đối chiếu

- Meta SAM2: <https://github.com/facebookresearch/sam2>
- IDEA Research Grounded-SAM-2: <https://github.com/IDEA-Research/Grounded-SAM-2>
- Krita AI Diffusion: <https://github.com/Acly/krita-ai-diffusion>
- Unity Sprite Library Asset: <https://docs.unity3d.com/Packages/com.unity.2d.animation@13.0/manual/SLAsset.html>
- Unity SpriteResolver API: <https://docs.unity3d.com/Packages/com.unity.2d.animation@13.0/api/UnityEngine.U2D.Animation.SpriteResolver.html>
- Unity SpriteSkin API: <https://docs.unity3d.com/Packages/com.unity.2d.animation@13.0/api/UnityEngine.U2D.Animation.SpriteSkin.html>
- Blender command line: <https://docs.blender.org/manual/en/4.5/advanced/command_line/index.html>
- Blender Object API: <https://docs.blender.org/api/current/bpy.types.Object.html>
- Apple DetectHumanBodyPoseRequest: <https://developer.apple.com/documentation/vision/detecthumanbodyposerequest>
- Apple GenerateForegroundInstanceMaskRequest: <https://developer.apple.com/documentation/vision/generateforegroundinstancemaskrequest>
- Apple body-pose limitations: <https://developer.apple.com/videos/play/wwdc2020/10653/>
- Moho scripting manual: <https://www.lostmarble.com/moho/manual/scriptsmenu.html>
- Rive Unity FAQ: <https://rive.app/docs/game-runtimes/unity/faq>
- Reallusion Cartoon Animator macOS notice: <https://kb.reallusion.com/Product/53088/Why-is-Cartoon-Animator-5-only-available-for-Windows-OS-and-not-for-Mac-OS>
