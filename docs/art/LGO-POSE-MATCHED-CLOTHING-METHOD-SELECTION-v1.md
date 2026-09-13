# LGO pose-matched clothing method selection v1

Date: 2026-09-13
Scope: Pháp nam Lv1 pose-matched clothing sandbox in `feature-2d-latest`. This document must be read before creating another garment candidate.

## Kiểm chứng đề xuất GarmentCode / Blender — 2026-09-13

Owner yêu cầu kiểm chứng tệp `cde34d16-8dd5-4dc4-94da-8d5e4f2ed934/pasted-text.txt`. Kết luận: cơ chế sản xuất có cơ sở, chưa chứng minh chất lượng hoặc lợi ích trên asset LGO. Tạm dừng tiếp bước chỉnh bảng áo AI trong khi đánh giá hướng này. Đây là kiểm chứng phương pháp, chưa đổi body/rig/runtime production.

Nguồn gốc đã đọc:

- Thomas Vasseur, [Dead Cells production](https://www.gamedeveloper.com/production/art-design-deep-dive-using-a-3d-pipeline-for-2d-animation-in-i-dead-cells-i-): skeleton/model 3D → PNG + normal map; nhân vật khoảng 50 pixel, tác giả thừa nhận giới hạn detail. Chứng minh cơ chế, không chứng minh art HD LGO.
- [GarmentCode](https://github.com/maria-korosteleva/GarmentCode): framework rập có component, tham số, stitches; draping là một bước riêng. Không đồng nghĩa garment đã rigged, weighted hoặc có sáu action LGO. [GarmentMeasurements](https://github.com/mbotsch/GarmentMeasurements) đo từ mesh 3D (`measurements output.obj measurements.yaml`), không suy được số đo chính xác từ PNG bị che.
- [ChatGarment](https://github.com/biansy000/ChatGarment) công bố lỗi chiều dài/rộng và postprocessing; dùng GarmentCodeRC và ContourCraft-CG riêng. Không thêm model này làm dependency bắt buộc.
- [Blender Map UV](https://docs.blender.org/manual/id/4.1/compositing/types/transform/map_uv.html): ánh xạ texture mới sau render được hỗ trợ. Phải giữ UV/pass dạng float phù hợp (ví dụ multilayer EXR), coverage/alpha và shading phù hợp. Thay normal/roughness/specular/transparency hoặc geometry không thể mặc định chỉ đổi RGB. B là reuse mapping khi topology/visibility giữ nguyên; C cần cập nhật geometry và render mapping/coverage mới bằng cùng chương trình.
- [Blender passes](https://docs.blender.org/manual/id/4.0/render/layers/passes.html): mask là dữ liệu kết quả render. Bề mặt opaque bị che không được Cryptomatte phục hồi. Item cần render độc lập khỏi đồ tùy chọn, tránh bake bóng đồ tùy chọn; kiểm depth ordering với front/back LGO.
- [Qwen Layered](https://github.com/QwenLM/Qwen-Image-Layered/blob/main/README.md) không bảo đảm semantic từng layer bằng prompt. [CatVTON model](https://huggingface.co/zhengchong/CatVTON) công bố CC BY-NC-SA 4.0; không chọn làm dependency thương mại mặc định.

Kiểm máy ban đầu: PID 81705 còn chạy từ root project `build/toolchains/blender/Blender.app`, nhưng bundle/executable báo về không tồn tại nên CLI không dùng được. Sau khi owner cho phép tải lại nếu không còn bundle, Blender 4.5.5 LTS chính thức được tải từ `download.blender.org` vào root project `build/toolchains/blender/Blender.app` và `--version` chạy được. Evidence `build/structured-garment-method-verification/blender-discovery.json`.

Proof local hẹp đã chạy bằng Blender: `build/structured-garment-method-verification/blender-parametric-proof-v1/report.json`, `review-board.png` và `review-board-on-body.png`. Script nhận `pose-registration-guide.json`, giữ canvas nguồn 1024×1536, xuất 3 cấu hình A/B/C trên 6 pose. B đổi surface nhưng giữ nguyên mesh hash của A cho mọi pose; C đổi geometry bằng tham số và regenerate mesh. 18 PNG đều không rỗng sau khi sửa lỗi camera clip; bài học là render/probe phải kiểm alpha, board và overlay lên body authority, không chỉ log `Saved`.

Kết luận proof: `NARROW_TECHNICAL_PASS_TOOLCHAIN_ONLY` và overlay status `VISUAL_SOURCE_OVERLAY_REVIEW_REQUIRED`. Nó chứng minh Blender local có thể dùng số đo/landmark làm input sinh output nhiều pose/variant không cần sửa PNG từng pose. Overlay cũng cho thấy cách này chưa đủ: flat-panel synthetic còn đè tay/body/jump, thiếu occlusion split và không mang thiết kế Pháp. Nó **không** chứng minh GarmentCode draping, cloth simulation, skin weights, body anatomy, mỹ thuật LGO hoặc tháo/lắp runtime. Không lấy kết quả Unity EditMode cũ hoặc board synthetic này làm chứng minh production wardrobe.

Audit occlusion trên overlay synthetic: `build/structured-garment-method-verification/blender-parametric-proof-v1/occlusion-conflict-audit.json` báo 18/18 pose-variant fail skin/body conflict, jump nặng nhất. Đây là fail đúng: flat panel không được dùng làm phom production. Pass thử `occlusion-clipped-v1/occlusion-clip-report.json` xóa conflict theo skin-color heuristic và không sửa PNG từng pose, chứng minh occlusion pass có thể tự động hóa. Nhưng heuristic màu da không thay thế semantic body-part mask, depth/component split hoặc weights; kết quả vẫn không production-eligible.

GarmentCode local feasibility: upstream commit `d449629979028123a5c4dc9e732a2ec19b7fce31` đã clone vào `build/toolchains/GarmentCode`; venv riêng `build/toolchains/garmentcode-venv` cài được dependency, gồm `cgal` và `libigl` wheel trên macOS arm64. `test_garmentcode.py` chạy thành công và sinh `t-shirt` pattern gồm specification JSON, pattern PNG/SVG/PDF, body measurements và design params tại `build/structured-garment-method-verification/garmentcode-core-output/t-shirt__260913-11-27-46/`. Evidence `build/structured-garment-method-verification/garmentcode-core-feasibility.json`. Phạm vi đúng: GarmentCode core pattern generation chạy local được; drape/simulation chưa kiểm vì docs yêu cầu fork NVIDIA Warp-GarmentCode; chưa có bridge sang body/rig/render LGO.

Bridge GarmentCode → Blender đã kiểm bằng chính specification JSON: `build/structured-garment-method-verification/garmentcode-blender-bridge-v1/report.json` đạt technical pass, nhưng visual/occlusion audit fail 18/18. Điều này bác bỏ hướng fit trực tiếp panel rập 2D lên pose như production path. Rập 2D là input cấu trúc; bước production vẫn cần drape/rig/depth hoặc semantic occlusion trước render.

Khuyến nghị owner e7129 được đánh giá tại `build/structured-garment-method-verification/owner-recommendation-e7129-assessment.json`. Quyết định: nhận kiến trúc **modular garment family + equipment compiler + Unity resolver**, với proof gates bắt buộc. Các nguồn tham khảo được chấp nhận theo phạm vi: Roblox layered clothing là analog về giao diện lớp/cage, Spine mesh/linked mesh là analog về weights/UV/deform reuse, UMA là analog về hide-data theo recipe, Addressables là roadmap memory/residency sau khi asset source ổn. Không cài hoặc chuyển runtime theo các hệ này trong sandbox hiện tại. Pilot kế tiếp phải có một item mới trong family sau khi khóa template; nếu vẫn sửa từng pose thì pipeline fail.

Planner đã được nâng để biến khuyến nghị này thành gate máy đọc được thay vì ghi chú thủ công. `tools/plan_lgo_pose_pipeline_next_action.py` nhận thêm `--compiler-pilot` và chỉ trả `GARMENT_FAMILY_COMPILER_CANDIDATE_ALLOWED` khi pilot chứng minh đủ: semantic occlusion/weights, B reuse geometry/mapping, C regenerate từ tham số, một unseen item sau template lock, đủ pose gồm `jump_tuck`, không sửa PNG từng variant và không bù runtime offset. Kể cả khi Blender/GarmentCode xuất đủ ảnh, `fitStrategy=direct_2d_pattern_panel_affine_fit` vẫn bị reject. Evidence hiện tại `build/structured-garment-method-verification/planner-current-gate-v1/next-action.json` trả `COMPILER_PILOT_EVIDENCE_REQUIRED`, thiếu `semantic_occlusion_or_weights` và `unseen_item_after_template_lock`, đồng thời ghi `direct_panel_fit_rejected`. Đây là chốt chống quay lại hướng rập 2D phủ thẳng lên pose.

Technical compiler pilot mới tại `build/structured-garment-method-verification/semantic-compiler-pilot-v1/compiler-pilot-report.json` dùng body-part semantic masks, không dùng skin-color heuristic. Nó xuất bốn biến thể trên sáu pose: A template khóa, B đổi surface giữ geometry/mapping, C đổi tham số hình học, D unseen sau template lock. Planner với report này trả `GARMENT_FAMILY_COMPILER_CANDIDATE_ALLOWED` tại `build/structured-garment-method-verification/planner-semantic-compiler-pilot-v1/next-action.json`, nhưng vẫn `runtimePromotionAllowed=false` và report ghi `productionEligible=false`. Ý nghĩa đúng: compiler contract có thể mở candidate authoring có kiểm soát; chưa chứng minh art LGO, body authority hoặc Unity runtime.

Pilot tiếp theo thay body procedural bằng guide thật `common-male-v1/pose-registration-guide-v1` từ selected source, chỉ ghi output trong `build/structured-garment-method-verification/semantic-source-space-pilot-v1/`. Lượt đầu fail vì near-arm ownership dùng polygon khớp quá mảnh, không giao vùng áo ở nhiều pose; đây là lỗi mask semantic, không phải lỗi material. Sau khi đổi near-arm thành capsule có bề rộng theo vai, report pass và planner `build/structured-garment-method-verification/planner-semantic-source-space-pilot-v1/next-action.json` trả `GARMENT_FAMILY_COMPILER_CANDIDATE_ALLOWED`. Board đã xem: phom áo còn procedural và chưa production, nhưng dữ liệu source-space thật đã chứng minh pipeline có thể giữ hidden surface/tay trước, A/B/C/D và unseen item mà không sửa PNG từng pose. Bước kế tiếp phải thay phom procedural bằng garment family thật cho Lv1, không chuyển board này vào runtime.

Khi chạy lại bằng measurement thật thay vì fixture sạch, guide gốc bị chặn đúng: `build/structured-garment-method-verification/lv1-short-vest-family-candidate-v1/real-guide-gate/next-action-real-guide.json` trả `FIX_GUIDE_BEFORE_ASSET` vì `run_b` shoulder width 60.08px, chỉ 0.34× median 176.18px, đồng thời proportion sanity cũng review-required. Candidate override review-only `pose-registration-guide-candidate-run-b-shoulder.json` đặt `far_shoulder≈[735.02,598.08]` theo target đo được, không mutate selected source. Đo lại candidate cho `ANCHOR_GUIDE_SANITY_NO_OUTLIERS` và `PROPORTION_SANITY_NO_OUTLIERS`; planner nay phân biệt `guideStatus=CANDIDATE_REVIEW_ONLY`, nên `build/structured-garment-method-verification/planner-lv1-short-vest-family-candidate-v2-fixed-guide/next-action.json` trả `REVIEW_GUIDE_CANDIDATE_BEFORE_ASSET`, `assetAuthoringAllowed=false`, `candidateGuideAllowed=true`. Vì family v1 đã sinh từ guide gốc, current artifact review đúng là `build/structured-garment-method-verification/lv1-short-vest-family-candidate-v2-fixed-guide/`, vẫn review-only và `runtimePromotionAllowed=false`.

Review alternatives cho `run_b` nằm tại `build/structured-garment-method-verification/lv1-short-vest-family-candidate-v1/real-guide-gate/run-b-shoulder-alternatives-v1/`. Kết luận hiện tại: A giữ nguyên bị reject; B metric target sạch số nhưng visual nghi rơi vào vùng tay trước; C visual torso-edge và D rebalanced đều sạch guide sanity nhưng vẫn là candidate guide. Family preview C/D cho thấy C ít kéo áo ra tay hơn D, nên C là default review tiếp theo, không phải authority. Bất kỳ lựa chọn nào cũng cần promote guide/owner-accepted override trước khi source-paint garment art thật.

Audit continuity v2 trong cùng thư mục bổ sung hard flag near-arm corridor: B bị loại vì `far_shoulder_inside_near_arm_corridor` và `far_shoulder_reaches_forearm_zone`, dù score v1 từng thích B do width đẹp. Ranking v2: C default review, D dự phòng, A/B reject. Đây là cơ chế chống metric overfit cho các landmark bị che.

Điều chỉnh bài thử trước khi thực thi:

1. Gate đầu tiên là source body/rig/camera tái hiện design ở run khó và jump; cùng canvas/world-scale và pixel budget runtime. Giữ bản candidate riêng; không yêu cầu byte-identical với sáu PNG độc lập và đồng thời giả định shared rig làm được. Chỉ khi hình ảnh candidate đạt mới tiếp garment; không xây kho đồ trên body thô.
2. A phải có vùng biến dạng khó. Design outer_top Lv1 hiện không tay: có thể đưa inner_top tay áo vào phép thử cùng outer_top, hoặc ghi rõ họ áo thử nghiệm khác. Không âm thầm đổi design Lv1 thành áo có tay/vạt dài để đáp ứng benchmark.
3. B đổi bề mặt, giữ geometry/weights/mapping. C đổi một tham số phom trong miền công bố; regenerate geometry/pass tự động được phép, chỉnh PNG từng pose bị cấm. Reject cấu hình ngoài miền trước render.
4. Cùng sáu pose, cả tháo đai/giáp/áo ngoài. Chứng minh phần khuất vẫn còn và không tồn tại bóng phụ kiện đã tháo. Nếu depth không biểu diễn được bằng component hiện hành thì fail compatibility, chưa đổi renderer.
5. Một setup và tối đa hai lần hiệu chỉnh nguồn chung. Đo setup, sửa nguồn, compute/export, can thiệp từng variant riêng; không hứa phần trăm tiết kiệm. Dừng nếu A không đạt art/body gate, dù script xuất đủ PNG. Chỉ coi đạt khi B/C không cần sửa ảnh, art và tháo/lắp đều đạt.

Quyết định kiểm chứng: ưu tiên thử Blender ngoại tuyến có nguồn tham số, giữ game 2D; GarmentCode là reference kiến trúc trước, không là điều kiện bắt buộc. Đây là đề xuất có cơ sở để thử, chưa phải phương án đã được xác nhận tối ưu cho LGO.

## Kết luận audit hiện hành — 2026-09-13

Số đo sạch không phải quyền sản xuất. Planner nay trả `DIAGNOSTIC_STACK_BOARD_ALLOWED`, `assetAuthoringAllowed=false`, `reuseProven=false` trên input candidate v2. Các kết quả `RUN_STACK_BOARD_ALLOWED` bên dưới là lịch sử đã thu hồi về mặt quyền authoring. Bảng chẩn đoán vẫn được phép; không xem việc đổi nhãn rejected → template là cải thiện chất lượng mỹ thuật.

Sau Player probe skeletal, owner reject trực tiếp body/cutout visual vì tỷ lệ và chuyển động jump/flip không đạt. Do đó source-space garment method không được tiếp tục trên body hiện tại. Số đo có thể vẫn dùng làm chẩn đoán, nhưng action sản xuất kế tiếp là `AUTHOR_SKELETAL_2D_SOURCE_BLUEPRINT`: tạo hoặc nhận một neutral layered body/rig blueprint mới, rồi mới chạy lại guide, proportion, motion, deformation và garment gates. Nếu không có source body mới đủ chuẩn, benchmark phải chuyển sang `modular_3d` hoặc owner decision thay vì quay lại tạo candidate áo.

Owner steering sau đó chốt fallback vận hành: không tiếp tục phát triển skeletal/source-blueprint trong sandbox này khi chưa có blueprint mới được duyệt. Active route quay về six-pose registered outfit pipeline, dùng body/motion sáu pose hiện có làm authority tạm thời và tập trung khớp đồ khi phối level/pose khác nhau. Tài liệu khóa: `docs/art/LGO-SIX-POSE-REGISTERED-OUTFIT-PIPELINE-LOCK-v1.md`.

Đã đối chiếu lại output thật của `inner-top-native-v2` và `accessory-native-v1`: tổng 24 cặp A/B giữ alpha bằng nhau và mọi hash khớp report; 12 cặp có nội dung đều đổi RGB, 12 cặp back rỗng. Đây là bằng chứng tái sử dụng đổi màu trên source cũ, không phải nguồn hoàn chỉnh hoặc hai thiết kế độc lập. Evidence `build/unity-pose-tool-spike/existing-krita-reuse-audit.json`. Không chạy lại spike clone/recolor hoặc Unity tests chỉ để chứng minh cùng kết luận.

Envelope hiện chỉ chứa đường vai/eo/trục thân. Nó chưa chứa mesh/UV mapping, cổ/tay/vạt đầy đủ hoặc mặt khuất; không thể biến nó thành phom đạt chỉ bằng hai ảnh sinh từ cùng công thức. Đường tiếp theo là hoàn thiện phom theo design đã chọn và mapping sáu pose, sau đó dùng một thiết kế bề mặt thứ hai. Đổi silhouette/cấu tạo phải có fitFamily tương ứng và review lại; không hứa mọi item dùng nguyên một phom. Ghi thời gian setup riêng với thời gian item thứ hai, số can thiệp geometry và lỗi visual. Hiện chưa có dữ liệu để hứa thời gian một bộ đồ.

## Why this exists

The sandbox already proved that small candidate loops are inefficient. Polygon torso drafts, large body-mask fills, AI/composite patches, and contour nudges produced evidence but no player-checkable garment result. The next work must use one selected method with measurable entry and exit gates.

## Sources checked

Internal:

- `docs/art/LGO-CLASS-2D-MODULE-STANDARD-v1.0.md`
- `docs/art/LGO-2D-EQUIPMENT-COMPATIBILITY-CONTRACT-v1.md`
- `docs/art/LGO-POSE-MATCHED-CLOTHING-PIPELINE-DECISION-v1.md`
- current measurement and repeat-audit evidence in selected source

External primary sources:

- Unity 2D Animation Sprite Swap sample: part swapping uses Sprite Library categories/labels and Sprite Resolvers; skeletal sprite swap requires compatible setup. Source: <https://docs.unity3d.com/Packages/com.unity.2d.animation@10.0/manual/ex-sprite-swap.html>
- Spine skins: production wardrobe uses prepared skins, attachments, placeholders, linked meshes, and mix-and-match composition. Source: <https://esotericsoftware.com/spine-skins>
- Godot cutout animation: cutout animation works by preparing separate pieces and animating them as a layered hierarchy. Source: <https://docs.godotengine.org/en/stable/tutorials/animation/cutout_animation.html>
- Krita Python scripting: Krita supports Python automation, so source authoring/export can be scripted, but our installed version still requires save/reopen/export verification. Source: <https://docs.krita.org/en/user_manual/python_scripting.html>

The common pattern is clear: engines and authoring tools reuse prepared, registered parts. They do not make a bad source, bad landmark guide, or missing occlusion layer correct at runtime.

## Method and tool options

| Method / tool path | Expected value | Risk in this sandbox | Decision |
| --- | --- | --- | --- |
| Per-candidate polygon/body-mask edits | Fast to create one image | Already failed; no stable measurement; encourages pixel guessing | Rejected |
| AI-first generation per slot/pose | Can add surface material quickly | Repeated placement/background/alpha failures; AI decides geometry without ownership contract | Rejected as geometry authority |
| Krita Python source authoring/export | Local Krita is available and can automate layer documents, export, reopen checks, alpha checks, contact sheets, and provenance | Automation cannot decide anatomy, occlusion, mesh weights, or whether a garment is visually good | Use only as deterministic authoring/export support |
| Unity 2D Animation Sprite Skin + Sprite Library feasibility spike | Unity is installed and this repo already references `com.unity.2d.animation` 13.0.0 plus runtime/tests for registered SpriteSkin and SpriteLibrary equipment. It can prove whether one shared character-space skeleton/weight field plus resolver categories reduces per-pose repaint work | Unity will not fix bad source art. Skeletal Sprite Swap requires identical skeleton/mesh setup between swappable sprites, so source registration still gates success | **Immediate tool-first spike before more lineart candidates** |
| Spine skins / linked mesh authoring proof | Strong production model for prepared skins, attachments, placeholders, linked meshes, and mix-and-match composition | Spine is not installed locally; paid/export workflow and Unity runtime integration must be proven before relying on it | Best external authoring candidate after local Unity spike, not current default |
| ComfyUI / SAM / ControlNet mask or material assist | Could speed mask cleanup or material fill after geometry is locked | No local ComfyUI/model/workflow/MCP was found; AI cannot be source geometry authority | Separate feasibility spike only after tool/model/workflow proof |
| DragonBones / Creature / Live2D | Alternative 2D rigging/mesh tools | Not installed locally; uncertain fit with current Unity runtime and paper-doll equipment contract | Not selected now |
| Registered template stack in source-space | Uses one canvas/profile, measured anchors, layer ownership, stack board, and pack gates | Slower upfront, but failures are measurable and reusable | Still the source-art contract, but now paired with a tool-first feasibility gate |

## Tool feasibility lock — 2026-09-13

Owner feedback identified the main failure mode correctly: repeated manual lineart/pixel adjustment can consume hours without proving production value. Therefore the sandbox must pause further numbered `outer_top` lineart/material candidates until a tool-assisted workflow is checked against the current project.

Local findings:

- Unity is available locally and the project already has `com.unity.2d.animation` 13.0.0 in `client/Unity/Packages/manifest.json`. The repo also already has `TwoDRegisteredSpriteSkin`, `TwoDSpriteLibraryEquipmentAdapter`, and EditMode tests covering registered weights, SpriteLibrary item registration, draft-only preview, and missing skinning rejection.
- Krita 5.3.3 is available under `/Users/minhdc/Tools/lgo-krita-5.3.3/krita.app`; use it for scripted layer authoring/export verification, not for manual pixel tuning.
- Spine, DragonBones, Creature, Live2D Cubism, and ComfyUI were not found in the checked local application/tool directories. They cannot be claimed as the active production path until install/runtime proof exists.

The next effective proof is `UNITY_REGISTERED_SPRITESKIN_LIBRARY_SPIKE`: create or reuse one small layered garment/body stack and prove that the same source-space landmarks drive a shared skeleton/weight field and SpriteLibrary component categories across four run poses. The spike passes only if it produces a reviewable artifact and machine-readable report showing layer registration, skeleton/weight reuse, no pose-local scaling, and candidate/runtime separation. It fails usefully if Unity cannot author/import the needed skin data without PSD Importer, if source layers lack compatible topology, or if the visual result still requires per-pixel fixes.

First Unity gate result: after `python3.12 tools/prepare_unity_protocol.py` regenerated the missing Unity protocol C# files, targeted Unity EditMode passed with 12/12 tests and zero failures. Evidence: `client/Unity/build/unity-pose-tool-spike/editmode-results-after-protocol.xml` and `build/unity-pose-tool-spike/unity-registered-spriteskin-library-spike-v1.json`. This proves the existing runtime/tooling can enforce registered SpriteSkin/SpriteLibrary behavior, but it does **not** prove PSD/PSB import, artist-quality garment art, Spine, Comfy, or runtime visual acceptance. The next proof must measure reuse on a second source-space item, not create another hand-tuned one-off candidate.

A reusable slot-envelope contract candidate was then written outside the repo at `source-review-v1/pose-slot-envelope-contract-v1.json`. It covers six poses and three slots: `outer_top`, `waist_belt`, and `shoulder_chest_guard`. Its purpose is to make the measurement reusable: future items compare against the same body-derived slot guides instead of redefining shoulders, waist, hips, or pose scale per item. This contract remains review-only until the next gate proves at least two items in the same slot can reuse it without changing body landmarks, pose-local scale, or runtime offsets.

The first reuse audit is intentionally negative: `source-review-v1/two-item-reuse-audit-v1.json` records `TWO_ITEM_REUSE_NOT_PROVEN` because existing `outer_top` v1/v2 templates are iterations of one candidate family, not two independent items. They cannot prove long-term production value. This is the stop rule for the old loop: do not count repeated versions of one garment as reuse evidence.

Spine remains the strongest external authoring candidate if local Unity/Krita support cannot remove enough manual work. Its proof should be offline and small: one skeleton, one base, one outfit skin, one linked/reused mesh pattern, and a JSON/atlas export review. Do not buy, install, or switch runtime based on docs alone.

ComfyUI/SAM/ControlNet may help after masks are locked. It must not be used to decide body proportions, garment ownership, or slot geometry, and it must not run until the exact local workflow/model can be executed and compared against the measured source-space gates.

## Selected method: registered template stack

The next effective method is:

1. **Guide/body gate first.** Run `tools/measure_lgo_pose_garment_anchors.py` and `tools/plan_lgo_pose_pipeline_next_action.py`. If any pose has measurement outliers, fix/review the guide or body candidate before creating garment art.
2. **One source-space template per pose.** Every item layer uses the same 1024×1536 canvas, origin X 512, ground Y 1484, and existing pose authority. Do not normalize each pose or item by bbox.
3. **Outer_top lineart/mask before material.** Draw/review measured collar, shoulder, torso axis, sleeve/arm occlusion, waist hem, and hidden cloth regions before filling texture. AI may fill material only inside accepted masks.
4. **Stack board as the first visual artifact.** The first accepted review board must show four run poses with body + `outer_top` + `waist_belt` + `shoulder_chest_guard`. Single-slot boards are diagnostic only.
5. **Runtime only after source pass.** Unity Sprite Library/Resolver or current runtime resolver can ingest assets only after source registration, stack visual review, and coverage gates pass.

## Size standard

There is no safe way to compute a final, absolute body-part size from an arbitrary rendered 2D pose alone. Running and jumping introduce foreshortening, occlusion, cloth coverage, and hand-drawn exaggeration. A pixel bbox or a median between poses is therefore not the project standard.

The project standard is:

- source-space profile `lgo_character_canvas_1024x1536_v1`;
- body/action authority and hashes;
- origin X `512`, ground Y `1484`, world unit factor `1.70/1536`;
- reviewed skeleton/proportion contract derived from authority landmarks;
- stack visual review and Player evidence before runtime approval.

`tools/measure_lgo_pose_garment_anchors.py` now separates two roles:

- `guideSanity`: catches broken anchors such as collapsed shoulders before drawing clothing.
- `proportionSanity`: checks broad pose-family ratios such as head/torso, shoulder/torso, and hip/torso before asset authoring.

These measurements are gates and review aids. They must not auto-scale a pose, auto-warp a garment, or justify per-pixel nudging. If the broad proportion contract fails, the correct task is guide/body review, not clothing adjustment.

## Current gate result

Current evidence:

- `source-review-v1/garment-anchor-measurements-v1.json`
- `source-review-v1/garment-anchor-measurements-v1-overlay.png`
- `source-review-v1/method-repeat-audit-v1.json`
- `source-review-v1/pipeline-next-action-v1.json`

The original guide planner returns `FIX_GUIDE_BEFORE_ASSET`, `assetAuthoringAllowed=false`. `run_b` has `shoulderWidthPx=60.08px`, only `0.34x` the median `176.18px`. The overlay shows the calculated target is useful for review but not safe to auto-apply. Therefore the next task is not another `outer_top` candidate. It is a guide/body review and correction pass.

A review-only candidate guide now proves the selected method can improve the gate before asset work:

- `source-review-v1/run-b-shoulder-landmark-review-crop-v1.png` compares current red landmarks, median target magenta, and visually selected cyan candidate.
- `source-review-v1/run-b-shoulder-landmark-overrides-v1.json` records the candidate override with provenance.
- `source-review-v1/pose-registration-guide-candidate-run-b-shoulder-v1.json` keeps status `CANDIDATE_REVIEW_ONLY`.
- `source-review-v1/garment-anchor-measurements-candidate-run-b-shoulder-v1.json` removes the `run_b` shoulder outlier.
- `source-review-v1/pipeline-next-action-candidate-run-b-shoulder-v1.json` returns `RUN_STACK_BOARD_ALLOWED`.

This does not promote the candidate guide to authority. It proves that guide/body correction before garment authoring is measurable and more effective than repeating pixel-level garment candidates.

## First method proof result

The selected method was exercised without creating new AI material art:

1. `tools/compose_lgo_pose_stack_review_board.py` composed a four-run-pose stack using existing assets. The v1 board included known rejected `outer-top-run-four-material-v3-bodymask` only to expose failure at stack level. Report: `STACK_REVIEW_FIX_REQUIRED`, `rejected=4`, `missing=4`.
2. `tools/generate_lgo_outer_top_measured_template.py` generated review-only `outer-top` mask/lineart templates from the candidate guide measurements. No AI fill, material generation, runtime pack, or authority promotion occurred.
3. The v2 stack board replaced rejected `outer_top` with the measured template. Report: `STACK_REVIEW_FIX_REQUIRED`, `rejected=0`, `missing=4`.

This is a useful proof because it changed an objective metric in the stack report and removed the known body-mask artifact class. It is still not garment acceptance: the outer template reads as a mechanical trapezoid and needs source-paint/lineart refinement for collar, armhole, sleeve ownership, cloth silhouette, and hem before material fill.

Next method step is superseded by the tool feasibility lock above. Do not continue `outer_top` lineart/mask refinement until `UNITY_REGISTERED_SPRITESKIN_LIBRARY_SPIKE` either proves a reusable tool-assisted path or fails with a recorded reason. If the spike fails, resume source-space template work only with a revised batch brief that explains why the work is no longer repeating the rejected manual loop.

## Entry gate for the next asset batch

Asset authoring may resume only when all are true:

- `plan_lgo_pose_pipeline_next_action.py` returns `RUN_STACK_BOARD_ALLOWED`.
- Measurement overlay has no shoulder/torso/hip outliers requiring guide review.
- `audit_lgo_pose_method_repeats.py` does not block the proposed method family.
- The batch brief names one review artifact and the method family it avoids.

## Exit gate for the next asset batch

The next asset batch is successful only if it produces:

- a four-pose stack board;
- a grouped failure/success report;
- source KRA/export evidence if Krita is used;
- no claim of runtime readiness unless source registration, coverage, pack, Player capture, and visual review all pass.

If the stack board still fails, the batch must stop with a grouped root-cause decision. It must not continue by creating another numbered candidate inside the same method family.

## Audit donor sáu pose — 2026-09-13

Design progression Lv1 dùng áo ngắn nhưng donor `deterministic-preserve-authoring-v7` còn vạt dài. Script cắt theo dilation 132px quanh áo trong không phải mapping cấu tạo. Không tiếp tục trim donor này để ép thành phom Lv1. Board lịch sử normalize crop từng pose nên không kiểm được kích thước; board mới giữ canvas 1:1 và đưa nhãn ngoài ảnh. Đã xem `build/unity-pose-tool-spike/source-fit-family-audit/six-pose-unobscured.png`; findings cùng thư mục. Nguồn áo ngắn đúng thiết kế là dependency trước transfer mapping, còn anatomy jump chưa đạt.

## Source design có mặt khuất — 2026-09-13

Một generation tạo bảng áo rời front/side/back tại external `short-vest-construction-design-v1`. Đã xem: lưng/nách/cổ dùng làm tham khảo cấu tạo, nhưng AI vẫn đưa tam giác gấu và huy hiệu ngực từ reference; chưa đạt ownership. Không dùng kích thước hoặc silhouette của bảng làm chuẩn body. Giữ prompt/provenance/hash, không lặp cùng prompt; tách huy hiệu trong source layer và quyết định chi tiết gấu trước mapping. Đây là design draft, không mở gate authoring production.

## Đính chính phát hiện Blender — owner 2026-09-13

Owner xác nhận đã có Blender. `ps`/`lsof` xác nhận PID 81705 đang chạy từ root project `build/toolchains/blender/Blender.app`, resource path 4.5. Tuy nhiên bundle/executable báo về không còn tồn tại khi kiểm filesystem và CLI không khởi chạy được. Sau khi owner nói nếu không còn thì có thể tải lại, Blender 4.5.5 LTS đã được phục hồi vào đúng toolchain root project và chạy `--version` thành công. Evidence `build/structured-garment-method-verification/blender-discovery.json`.

Probe đầu tiên bị render rỗng dù Blender log `Saved`; nguyên nhân là camera đặt cách mesh xa hơn clip mặc định. Sau khi sửa `clip_end`, manifest kiểm `allVariantPngNonEmpty=true`. Bài học kế thừa: toolchain proof phải kiểm executable, output alpha/bbox và board đã xem; không được claim từ process còn sống hoặc file PNG được ghi.


## Đánh giá lại cấp mô hình nhân vật — owner 3ca3, 2026-09-13

Nhận xét trong attachment `3ca3d7a9-ed29-4fd5-a339-86697560fb68` được **chấp nhận ở cấp kiến trúc**, nhưng không được hiểu là đã chọn Spine hoặc 3D làm production ngay. Quyết định mới là mở gate `LGO_CHARACTER_MODEL_ARCHITECTURE_REVIEW_01` trước khi tiếp tục authoring trang phục: pipeline sáu pose/image-fit hiện tại chỉ còn là baseline đối chứng và evidence lịch sử, không còn là mặc định production foundation. Evidence JSON: `build/structured-garment-method-verification/owner-recommendation-3ca3-model-rebuild-assessment.json`.

Điểm nhận đúng: “authority” chỉ là trạng thái nguồn đang đăng ký, không phải thiết kế tối ưu bất biến. Body, pose set, renderer, layer order và cách pack nhân vật đều có thể bị thay nếu benchmark chứng minh cách mới sản xuất item tiếp theo tốt hơn, ít can thiệp hơn và không phá tổ hợp cũ. Các lỗi vừa gặp cũng ủng hộ nhận định này: direct GarmentCode/Blender panel fit pass kỹ thuật nhưng fail occlusion 18/18; guide `run_b` sửa bằng số đo vẫn cần review anatomy; lineart/mask từng pose dễ quay lại vòng pixel thủ công.

Hướng đánh giá được giới hạn còn hai nhánh, không mở thêm vòng nghiên cứu công cụ vô hạn:

- **A — 2D skeletal runtime modular character.** Baseline nội bộ là Unity 2D Animation/SpriteSkin/Sprite Library vì project đã có `com.unity.2d.animation 13.0.0`. Spine là candidate mạnh về authoring/runtime nếu license/export/tooling được chứng minh; tài liệu chính thức cho thấy Spine có runtime export, skins, linked meshes và weights, nhưng purchase mới cho phép save/export/use runtimes.
- **B — Direct 3D modular character trong gameplay 2D.** Unity chính thức cho phép 2D gameplay dùng 3D geometry/orthographic hoặc 3D character; Blender local chỉ là tool/proof input. Không dùng lại Blender→sprite hoặc direct 2D pattern panel fit làm mặc định vì các proof trước đã không giải được occlusion/ownership theo cách production.

Bài benchmark chung cho hai nhánh: cùng reference Linh Giới và kích thước gameplay; một base body trung tính; áo có tay/vạt, quần, giày, đai và một món cứng; idle/run/jump/attack/return; tháo từng slot và phối khác mốc; thêm một item unseen sau khi khóa template/tool; đo thao tác thủ công, sửa source, sửa code, regression và hiệu năng PC/mobile. Điều kiện thắng là item mới kế tiếp đạt chất lượng với ít can thiệp hơn và không phá tổ hợp cũ, không phải demo đầu tiên đẹp.

Giới hạn quan trọng: không đổi Unity/Java/protocol/UI/map cùng lúc; không promote candidate short-vest hoặc C/D shoulder khi guide còn review-only; không dùng clipping/mask làm lời giải chung cho mọi đè lớp; không mua/cài/chuyển Spine chỉ từ tài liệu. Nếu cả hai nhánh fail, bước đúng là sửa nguồn mỹ thuật hoặc giảm yêu cầu sản phẩm cụ thể, không mở pipeline thứ ba/thứ tư.
