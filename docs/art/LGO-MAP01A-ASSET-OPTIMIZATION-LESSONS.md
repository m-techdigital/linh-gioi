## Không chia sheet theo ô khi object vượt biên — 2026-09-12

Mái social-hall vượt x=512, đèn hunter-post lấn ô well; grid cố định đã tạo mảnh lẻ trong Player. Raw gốc còn ở generated_images dù alpha trung gian trong build đã mất: tìm theo thời điểm + contact sheet thay vì chỉ so SHA alpha cũ. Phục hồi raw và provenance ngoài repo tại selected source `map-01a-cong-dong-lam/runtime-source-recovery/landmarks-v1`.

Packer landmarks nay nhận sáu source rect có biên trong cột alpha rỗng; chặn object chạm biên, overlap và thiếu coverage sau key/noise cleanup. `sourceContentRect` ghi cả bbox đã trim theo tọa độ raw, không mất dữ kiện khi pack. Test kiểm màu mái xuất hiện trong atlas hall và không xuất hiện trong merchant. Không tuyên bố alpha mới byte-identical với file trung gian đã mất.

Atlas đổi từ 3×2 sang 2×3 ô để tăng số pixel công trình trong cùng 1024², không upscale và không tăng ước tính BC3 1 MiB. PNG tăng 1.003.688→1.637.421 byte; build cùng target đo tăng 1.184 byte (183.190.682→183.191.866). Không đồng nhất PNG với GPU/build. Player đã xem mái/đèn/giếng liền, Q01–Q09 vẫn chạy trên ba tỷ lệ; evidence `build/map01a-restored-landmarks-player/quest-capture/`.

## Mặt đi bộ phải nằm trên ảnh, không phải mép rect — 2026-09-12

Terrain cũ gán đỉnh Sprite.bounds = GroundY, nhưng đá/rêu/cỏ/cầu có padding và phối cảnh khác nhau, làm chân trông lơ lửng dù test bounds xanh. `modules-layout.json` hiện khai báo `walkSurfaceFromTop` cho bốn part (20/28/62/18 px); renderer dịch terrain đúng độ lệch source-pixel theo world height. Mọi instance cùng part dùng chung mốc, không chỉnh actor/camera/scale, không sửa pixel atlas.

Mốc được chọn trên mặt đi bộ trong module gốc, không lấy pixel alpha đầu tiên (hoa/cỏ có thể nhô cao). Test đọc PNG thật, kiểm hàng mặt đi opaque xuyên 490 pixel/part và tọa độ surface của cả 12 terrain trùng GroundY. Capture `build/map01a-grounded-player/quest-capture/{pc,tablet,mobile}` đủ Q01–Q09, đã xem chỗ nối cỏ–đá/cầu và chân nhân vật. Không đồng nhất kết quả này với nghiệm thu art toàn map.

## Bốn ID không bảo đảm bốn nhịp chạy — 2026-09-11

RunA/B nguồn cũ gần cùng thế chân; thêm contact vẫn chưa đủ cảm giác bốn nhịp. Sheet imagegen toàn vòng tiếp tục lặp1/3 và2/4 nên loại trước pack/Player. Hướng hiệu quả hơn trong batch này: khóa ownership chân gần/xa bằng mảnh base + IK, rồi chỉ dùng donor sửa đường nối quần trên mask. Source v5 giữ phần ngoài mask, tiếp đất0gap, nhịp bay được phép cách nền; không normalize mọi pose về ground. Pack div4 vẫn512×1024 và369348byte.

Hướng joint + sprite cho phần đổi silhouette tham khảo [Unity Animated Swap](https://docs.unity3d.com/Packages/com.unity.2d.animation@10.0/manual/ex-sprite-swap.html) và [Spine weights](https://esotericsoftware.com/spine-weights); đây là nguyên tắc tác nghiệp, không thêm engine/đổi rig hoặc tự coi wardrobe đã đạt.

Capture bốn ảnh rời có thể thiếu nhịp hoặc sai đồng hồ. Capture mới ghi180frame/30fps, pose ID/state/time và3vòng đầy đủ. Nó bắt được HUD vẫn đọc input, ép run→walk và tăng phase hai lần: chặn input trong capture và kiểm delta1/30, không nới assertion. Chỉ chạy PC để xác minh lỗi nhịp; ba profile khi thay đổi hiển thị cần kiểm. Evidence `build/vo-four-phase-clean-runtime-v2/REVIEW.md`; hai nhịp vào/dừng còn pending.

## Lịch sử divisor8 và contact grounding

## POSE THỬ mờ và contact source — 2026-09-11

Feedback POSE THỬ mờ so base đã truy về pack divisor8; không mở camera/close-review. Review hiện hành `legacy-base-run-contact-jump-v3-div4` dùng cùng nguồn, divisor4, atlas512×1024, PNG373834byte/RGBA8 2097152byte, REVIEW_ONLY. Không tự dùng budget review làm production policy. CLI `pack_lgo_pose_review_atlas.py` mặc định4/1024, xuất tên Player đọc và cần `--jump-pivot-source 512 820` cho bộ6pose này. Capture helper chặn divisor8/promotion/hash sai, đối chiếu exact loaded path và từng pose trong Player log.

Repack đã trùng byte; capture mới `build/vo-pose-div4-verified-runtime-v2` đủ154frame ×3profile trên Player WIP kế thừa và đã xem ảnh. Không phải new-build evidence. Source contactA/B còn thấp hơn mốc chân65px nguồn; cần sửa source có anchor, không normalize/camera để che. Hai imagegen candidate bị loại (RGB/caro giả alpha; magenta nhưng sai ground và đổi shading thân). Giữ lỗi/provenance external để không lặp cùng prompt tọa độ; idle/A/B gốc giữ nguyên, wardrobe chờ motion gate.

# Map01A — tối ưu ảnh, tỷ lệ và bài học cần kế thừa

Chỉ đạo owner: giảm chất lượng/dung lượng runtime theo kích thước thực sự hiển thị; asset phải tách ghép dùng lại được; xử lý mobile/tablet/PC trong batch, kiểm sau khi gom xong. Source HD giữ ngoài Unity để tái xuất, không tạo lại ảnh chỉ vì đổi độ phân giải.

## Batch Võ Lv1: quy tắc tạo và tách nhân vật

- Tạo cả giới tính hoặc cả motion set trong một sheet/batch theo cùng art direction; không gọi sinh từng món. Board owner dùng để redraw và đối chiếu, không crop nguyên board vào runtime.
- Source làm việc giữ ngoài Unity ở canvas 1024×1536. Trước khi tách, căn bbox với base trong sai số tối đa 3 px và dịch toàn bộ ảnh về cùng ground; sau đó lấy delta theo base để phần da/cơ thể tiếp tục thuộc base.
- Nền magenta do công cụ sinh có thể không phải một màu tuyệt đối. Key theo độ chênh `min(red, blue) - green`, kiểm alpha/bbox bằng ảnh review nền tối, không chỉ so một mã RGB.
- Mười slot của một giới tính và sáu motion frame được xử lý cùng lượt. Runtime chỉ giữ hai atlas indexed 1024²: static 91.553 byte và motion 124.385 byte; manifest compact 12.113 byte. Ảnh nguồn độ phân giải cao không vào Resources.
- Actor hiện cao khoảng 150 px trong ba profile. Vì vậy cell runtime quanh 340 px đủ dự phòng, không xuất mỗi bộ thành texture 2K/4K. Importer giới hạn 1024; mobile dùng ASTC 6×6. Con số PNG không thay cho đo GPU/thiết bị thật.
- Sheet motion 3×2 giữ cùng nhân vật/trang phục cho idle, hai bước walk, dash, punch windup và punch impact. Đây là cách nhanh hơn và đồng nhất hơn sinh từng frame. Batch kế tiếp làm cùng cấu trúc cho nữ và tiến cấp Lv10/20/30.
- Slot mảnh có thể thay đổi rất nhẹ khi actor hiển thị nhỏ; kiểm bằng toggle trong Player nhưng chỉ giữ slot có giá trị phối đồ thật. Khi vào production animation dài, ưu tiên layered PSD/PSB hoặc skeletal attachment thay vì nhân số lượng full-frame cho mọi tổ hợp.
- Tiến cấp Lv10/20/30 được tạo thành một sheet 3×2 rồi tách đồng loạt; mỗi tier dùng atlas 1024² riêng để chuẩn bị tải theo tier. Sáu atlas equipment + motion hiện là 643.722 byte PNG, thay vì một atlas 2048 luôn tải toàn bộ.
- Full-frame motion chỉ dùng đúng outfit mà sheet đã vẽ. Không được dùng frame Lv1 khi nhân vật đang mặc Lv10/20/30; runtime hiện giữ paper-doll tier tĩnh cho đến khi có tier-matched sheet hoặc rig attachment. Đây là gate correctness, không che bằng transform hoặc claim animation hoàn chỉnh.

## Quyết định đang áp dụng

- Nguồn canonical: `LGO-SELECTED-2D-SOURCE-CATALOG-v1.md`; công cụ/mask/class WIP: `LGO-CLASS-STANDARDIZATION-REUSE-AUDIT-v1.md`. Không thay nguồn owner bằng procedural preview cũ.
- `tools/pack_lgo_map01a_art.py`: atlas 1536², nền xa 1024×576. Tổng PNG 3.774.048 byte, trước giảm 6.764.005 byte (giảm khoảng 44%). Budget slice 4 MiB PNG. Sprite được resize từ nguồn, giữ alpha, không nhân alpha lần hai khi paste.
- Bộ nhớ lý thuyết không mipmap: RGBA 11.796.480 byte; desktop BC1/BC3 2.654.208 byte; ASTC6x6 1.311.232 byte. Đây là ước tính block texture, không bao gồm driver/cache và không thay thế profiler thiết bị thật.
- Terrain nguồn được chia thành bốn source rect trong cùng atlas. Renderer lắp tile và lặp cả chuỗi cho màn rộng, dùng cùng texture. Không xuất ảnh riêng cho mỗi lần đặt. Vị trí/collision/ground vẫn độc lập pixel resolution.
- Camera thử mới orthographicSize 4.6 thay 3.54375: cùng vật thể nhỏ hơn khoảng 23%; giữ chân tại 73% chiều cao màn hình. HUD clone PanelSettings riêng, scale theo chiều cao, không sửa policy chung; joystick chỉ bật trên touch profile.
- Không có nghĩa toàn bộ game chỉ cần một atlas: phân nhóm theo vòng đời `shared terrain/props`, `map-specific background/landmark`, `actors/equipment`, `UI`. Không copy chung một prop vào atlas của mọi map. Chưa tách atlas hiện tại sang các nhóm này; đây là gate trước khi nhân nhiều map.
- Chưa triển khai streaming/Addressables. Resources hiện phục vụ một slice; cần task riêng thay cơ chế tải khi mở nhiều map, theo dõi reference count và giải phóng nhóm map cũ. Cắt sprite trong atlas không giảm memory của texture khi đã tải.

## Quy trình bắt buộc trước khi tạo asset mới

1. Chốt camera và render resolution mục tiêu của ba profile, kể cả zoom gần nhất được phép. Tính `pixelHeight = worldHeight / (2 * orthoSize) * renderHeight`; chiều ngang tương tự. Dùng kích thước framebuffer/render scale, không mặc định độ phân giải vật lý 4K của màn hình.
2. Ghi brief: asset ID, source owner, số pixel lớn nhất trong gameplay, kích thước yêu cầu, alpha/pivot, nhóm dùng chung/map riêng, số biến thể và vòng đời tải. Khoảng dự phòng 1.0–1.25 lần là lựa chọn ban đầu của dự án, không phải chuẩn Unity bắt buộc. Chỉ tăng khi zoom/animation chứng minh cần.
3. Ưu tiên tìm asset đã có theo catalog/hash. Prop lặp lại dùng cùng sprite/prefab; tách cấu trúc thành module có nghĩa (cột, mái, đèn, gạch), không cắt ngẫu nhiên ảnh toàn cảnh rồi coi là tileset. Cụm ba nhà hiện tại vẫn là midground cluster, chưa phải ba prop độc lập.
4. Yêu cầu tạo ngay gần kích thước mục tiêu. Không mặc định sinh 2K/4K từng item nhỏ. Nếu công cụ sinh không bảo đảm đúng kích thước, ghi rõ hạn chế trước batch; không âm thầm sinh hàng loạt ảnh lớn và gọi downsample là pipeline tối ưu. Source owner có sẵn được giữ nguyên, không tạo lại.
5. Pack theo vòng đời tải và định dạng nén; giữ gutter/pivot/source rect. So chất lượng tại tỷ lệ 1:1 trong game, không chỉ phóng to atlas để đánh giá. Chọn điểm dừng khi đọc được silhouette/vật phẩm, không tối đa chi tiết vô ích.
6. Đo build/download, CPU/GPU texture, draw calls/overdraw, frame time và thời gian tải/chuyển map. Các chỉ số này có gate riêng; giảm PNG không chứng minh FPS hoặc RAM thấp.

### Pixel dự kiến cho góc Map01A hiện tại

Camera `orthoSize=4.6`, capture cao tối đa 768px. Đây là brief cho asset tiếp theo theo ba profile hiện dùng; chưa cam kết render native 1440p/4K hoặc zoom gần khác. Khi mở cấu hình render cao hơn phải tính lại, không tự nâng toàn bộ source.

| Asset | Kích thước lớn nhất dự kiến trên màn hình | Mục tiêu tạo/xuất ban đầu |
|---|---:|---:|
| Hạ Vân, world 1.18×1.77 | 99×148 px | khoảng 128×192 px, đủ padding |
| Cổng, world 7.8×5.20 | 651×434 px | khoảng 768×512 px |
| Cụm nhà, world 8.6×4.3 | 718×359 px | khoảng 832×416 px |
| Một tile đường, world width 3.15 | 263 px ngang | khoảng 288–320 px ngang; chiều cao theo phần đá thực thấy |
| Nền xa | phủ màn ngang tới 1600px nhưng chi tiết thấp | 1024×576 đang thử; chỉ tăng khi review thấy blur khó chịu |

Bảng không có nghĩa tất cả ảnh phải power-of-two. Kích thước atlas và block compression cần được packer/importer quản lý; tránh padding vô ích chỉ để mỗi prop thành 1024².

### Tải asset khi có nhiều map

Theo [Unity Addressables memory management](https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/MemoryManagement.html), load/release phải đối xứng; release asset chưa chắc giải phóng bộ nhớ nếu bundle còn được giữ. Vì vậy nhóm shared assets phải sống qua các map dùng chung, nhóm map-specific giải phóng khi hết reference. Tránh unload rồi lập tức reload nhóm dùng chung khi chuyển map. Không gọi UnloadUnusedAssets mỗi frame: có thể gây hitch. Đây là hướng cần triển khai/đo, **chưa có streaming trong code hiện tại**.

## Lỗi đã gặp — không lặp lại

| Lỗi/bẫy | Cách xử lý đã xác nhận |
|---|---|
| Terrain có ô caro nhưng alpha hoàn toàn đặc, kể cả lần imagegen sửa | Giữ bản lỗi, chỉ dùng vùng đá đặc `(0,330,2172,724)` đã review; không chroma-key toàn ảnh hoặc claim alpha PASS |
| Double alpha khi paste atlas | `atlas.paste(image, position)` không truyền alpha làm mask lần nữa |
| PNG nhỏ bị hiểu thành GPU memory nhỏ | Báo riêng PNG, memory block ước tính, đo runtime thực tế khi có thiết bị |
| Camera cũ cắt mái / scene quá lớn | Quy định kích thước camera và foot lane; review ba aspect chung, không sửa riêng từng rectangle |
| HUD scale theo width khiến ultrawide phóng chữ/nút | Clone PanelSettings, match height, tách kích thước touch với PC; không thu nhỏ hit target cùng mỹ thuật một cách mù quáng |
| Marker sai ở frame đầu | Chờ layout ổn định, marker có width/height rõ; capture sau Update và EndOfFrame |
| Camera.Render capture mất UI Toolkit | Đọc framebuffer sau EndOfFrame để có cả HUD; ghi BMP và convert PNG bằng sips |
| World assembly thiếu EncodeToPNG | Reuse `DongMonIllustratedPreview.WriteBmp`, không thêm dependency chỉ để encode |
| OnDestroy EditMode không restore renderer | ExecuteAlways; test restore camera/renderer/controller và grounding không tích lũy |
| macOS app launch sai cwd / chờ foreground | Launch executable từ Contents/MacOS, activate đúng app; không mở review ảnh giữa capture |
| State cũ mở Shadow Slime/Linh Thành khi đang Map01A | Suspend controller input cũ; input/thoại arrival riêng; không lấy smoke cũ làm bằng chứng Q01–Q09 |
| Reimport Unity sửa pipeline GUID trong ProjectSettings | Lưu patch evidence, chỉ restore hai file do batch tự sinh sau kiểm; không reset main/worktree khác |

## Kiểm một batch

Gom asset/scale/HUD/input rồi EditMode → macOS build → `tools/capture_lgo_map01a_art.py --profile all` → review ảnh + guard/frozen diff. Chỉ chạy lại khi phát hiện lỗi liên quan. Ba profile mô phỏng tỷ lệ trên macOS không chứng nhận touch/notch/performance trên điện thoại/tablet thật. Không tuyên bố visual pass vì PC còn blockout hoặc chưa đủ route/quest.

## Tham khảo bên ngoài và phạm vi áp dụng

- [Unity Sprite Atlas workflow](https://docs.unity.cn/Manual/SpriteAtlasWorkflow.html): packing, max texture size và include in build. Áp dụng quản lý texture/atlas; repo hiện vẫn dùng atlas PNG + JSON, không nhầm với `.spriteatlas` Unity tự đóng gói.
- [Unity Sprite Atlas distribution](https://docs.unity.cn/Manual/SpriteAtlasDistribution.html): phân phối atlas; cơ sở nghiên cứu tải theo nhóm, chưa coi là streaming đã triển khai.
- [MapleStory UI guide](https://gi.maplestory.nexon.com/Guide/Beginner/Manual): quest/minimap có bố trí và mức hiển thị điều chỉnh. Tham khảo phân cấp UI, không lấy ảnh làm runtime art.
- [Dead Cells mobile controls](https://playdigious.com/news/sharpen-your-thumbs-dead-cells-is-now-slaying-foes-on-android): control tùy chỉnh/floating hoặc fixed pad. Tham khảo tách cấu hình input khỏi scale cảnh; không tự thêm auto-hit/auto-battle vào LGO.

Các phần camera/downsample/tile mới cần capture xác nhận trong batch hiện hành. Nguồn HD và bản cũ giữ tại `build/map01a-art/source/`, `build/map01a-art/pack/`; pack giảm tại `build/map01a-art/pack-reduced/`.

## Batch chọn nguồn và cắt đồng loạt — 2026-09-10

Owner yêu cầu đồng nhất giao diện; không sử dụng mọi ảnh chỉ vì đã đặt tên canonical.
Review thực tế phát hiện `04-npc-character-and-placement.png` và `07-environment-vegetation-ambient.png` là board tổng quan khác kiểu cổng/nét vẽ, không đúng chức năng tên file. Hai ảnh bị giữ ngoài batch cắt. Không xóa nguồn cũ.

Nguồn batch: sheet08 kiến trúc/props và sheet06 quái/item, đối chiếu palette mái xanh–vàng, đèn ấm, hoa đào và núi xa của gameplay01. Chỉ reuse ngôn ngữ hình ảnh; UI Lv80/premium, quest cũ, map top-down và vé hồi thành không thành scope Map01A. Terrain isometric cần chuyển đúng góc side-view; không tự bóp méo để giả thành tile hợp lệ. NPC/class chưa chọn lại bằng hai sheet này.

Plan đầy đủ: `docs/art/LGO-MAP01A-BATCH-CROP-PLAN.json`; runner `tools/extract_lgo_design_sheet_items.py`. Output hiện hành ngoài repo: `/Users/minhdc/Projects/Design/LGO-Extracted-2D-Items-v1/map-01a-reviewed`. Có 89 crop PNG, 1.761.077 byte, manifest nguồn/hash/tọa độ normalized và native; ba contact sheet. Bản `map-01a` là lượt trước review, không dùng tiếp.

Kiểm 89/89 crop tương đương pixel vùng ảnh gốc, hash đúng, không upscale; py_compile thành công. Review contact sheet phát hiện đường chia cột hàng rương khác hàng đèn và heading lẫn vào crop; đã gom sửa tọa độ một lượt. Đây là source staging còn nền/chữ/biến thể, không phải 89 asset production hoặc chứng nhận Player. Không chạy lại Unity vì batch này chỉ thêm công cụ và nguồn ngoài runtime. Tiếp theo xử lý alpha theo nhóm prop cùng nền, chọn một mẫu cho vật thể trùng; giữ các vùng chưa đủ pixel/góc nhìn ở trạng thái cần xử lý, không generate từng item mới tùy ý.

### Cleanup nhóm props và kiểm trong Player

Đã xử lý 16 props trong một lượt chung, output hiện hành ngoài repo `LGO-Extracted-2D-Items-v1/map-01a-props-player-batch`. Chọn 4 silhouette cho opt-in Player comparison: crate, bench, lamp-post, stone-lamp; atlas 256² chỉ 33.209 byte PNG, BC3 ước tính 65.536 byte. Không resize crop, không thay màu/vẽ mới. Nhóm còn matte bên trong (bàn trà/kệ/chậu) chưa ingest. File nguồn và các bản lỗi giữ nguyên, chưa gọi cả nhóm production-ready.

Lỗi mới cần tránh: (1) crop rộng 121px không vừa ô 128px nếu chừa padding4 hai bên; kiểm kích thước toàn batch trước tạo output, padding2 cho crop tối đa124; không downsample tùy ý. (2) Key màu toàn ảnh thủng đèn sáng nhưng vẫn giữ bóng nền xanh dưới bàn; đổi sang flood vùng matte nhạt nối biên. Không áp green despill của sandbox class lên nền trắng/xanh nhạt. (3) Texture meta tạo tối thiểu để Unity suy diễn shape sai (sheet-props shape2 so với atlas đang chạy shape1); importer phải khóa `TextureImporterType.Default` và `TextureImporterShape.Texture2D`. Giữ tên layout khác texture cho Resources rõ ràng. Hai log EditMode thất bại: `source-props-editmode.log`, `source-props-editmode-fixed.log`; log sau sửa importer: `source-props-editmode-import-fixed.log` trong `build/map01a-art/review/`. Không biến lần chạy lỗi thành PASS.

Nền đen tablet/PC: công thức cũ chỉ phủ chiều ngang; camera tăng height và lệch tâm dọc khiến phần trên ra ngoài ảnh. Fit cả hai camera extents cộng offset sau parallax; test bounds trên ba aspect trong test preview hiện có. Cần capture Player sau build để xác nhận ảnh, không dựa duy nhất vào test bounds.

Xác nhận từ lần diagnostic: layout=True, texture2D=False và meta vẫn shape2. Chỉ sửa AssetPostprocessor không tự ép reimport asset đã có trong cache; sửa meta asset sở hữu của batch về shape1 để kích hoạt import lại. Không xóa Library/reset checkout để chữa cache.

## Owner mở quyền chủ động tạo ảnh — batch module 2026-09-10

Kết hợp crop source đủ chất lượng với tạo bù module cần thiết. Đã dùng built-in imagegen tạo một bộ8 module theo sheet08/gameplay01 và nền xa theo sheet09/gameplay01. Không dùng API/CLI image generation. Ba call gồm: tạo atlas, một lần sửa toàn bộ nền ô caro sang magenta, tạo nền xa opaque. File/prompt/hash: `build/map01a-art/module-generation-v1/brief.json`, `prompts.txt`, `parallax-prompt.txt`.

Tool không bảo đảm size/alpha theo prompt: atlas yêu cầu1024² nhưng trả1254² RGB với ô caro; giữ bản lỗi, không coi là RGBA. Chỉ sửa nền một lần rồi reuse script main stopped-task `tools/chroma_key_vo_candidate.py` ở chế độ đọc, key255,0,255/tolerance36/feather72, output trong worktree này; không sửa code main. `tools/pack_lgo_map01a_modules.py` áp dụng edge-only despill của sandbox class cho magenta, giữ alpha trước pack, cắt8 vùng đã review và đóng atlas1024², 1.193.261 byte PNG; BC3 ước tính1MiB, ASTC6x6 467.856 byte. Không upscale crop. Nền xa vẫn opaque, chưa có12 layer độc lập.

Đã xem atlas trên nền tối: vật liệu/side-view phù hợp, không còn board/checkerboard; cây cỏ còn một ít viền/speck cần review khi ghép, tile chưa chứng nhận seam/collision. Status DRAFT_REQUIRES_PLAYER_REVIEW, không chạy lại Unity chỉ vì tạo source. Tiếp ghép trọn bộ terrain/vegetation/background và route camera rồi mới build/capture cả3profile một lượt. Không đánh dấu map pass hoặc mở class.

## Batch tuyến hoàn chỉnh, landmark, combat và palette PNG — 2026-09-10

- Tạo theo sheet thay vì từng item: một sheet 6 NPC, một sheet 6 landmark và một sheet 4 quái + rương + Linh Thảo. Mỗi sheet dùng atlas hiện hành làm chuẩn style, chroma magenta một màu, rồi key/trim/despill/pack một lượt. Runtime tương ứng là `npcs-atlas` 512², `landmarks-atlas` 1024² và `combat-atlas` 512².
- Tỷ lệ thực tế theo camera 4.6 và framebuffer cao nhất 768px: NPC khoảng 149–159px, quái khoảng 77–99px, landmark 180–235px. Bản NPC 1024² ban đầu lớn hơn nhu cầu nên đã repack 512² trước khi tích hợp.
- `tools/optimize_lgo_map01a_runtime_textures.py` chuyển cả batch sang indexed PNG 256 màu (small props 192 màu), không đổi kích thước. Contact sheet `build/map01a-art/review/optimization-comparison.jpg` đã được xem trên nền tối. Bảy texture runtime hiện 1.049.392 byte; RGBA lý thuyết 17.301.504 byte, desktop GPU estimate 4.030.464 byte, ASTC6x6 estimate 1.932.480 byte. Indexed PNG chỉ giảm source/download; Unity vẫn giải nén/nén theo importer khi chạy.
- Không quantize lặp trên bản đã quantize để kiếm thêm vài KB: input canonical phải là output RGBA của packer. Batch thử lại cho thấy hash/palette alpha đổi dù ảnh nhìn gần giống; runtime chỉ lấy combat mới từ lượt đó, sáu texture trước giữ bản review lần đầu.
- Terrain module overlap ngang 0,12 world unit để che khe bilinear giữa tile; không thay collision/route width. Architecture/landmark đặt theo ground anchor, không kéo tỷ lệ riêng ở từng profile.
- Evidence cuối: `build/map01a-art/final-three-profiles/` gồm 8 frame × mobile/tablet/PC. Đã xem arrival/gate/market/bridge/combat/portal; không thấy matte magenta hoặc nền đen. Player blockout vẫn lộ rõ, vì vậy checkpoint chỉ đóng map art foundation và chuyển sang Võ Lv1–30 để sửa PC; không gọi Q01–Q09/combat hoặc visual production hoàn tất.

### Atlas ít frame: tối ưu xếp hàng trước khi tăng cạnh — 2026-09-11

Tám sprite Võ div4 bị greedy shelf đẩy1024²/4MiB dù xếp được512×1024/2MiB. Exact row partition tối đa10sprite giữ pixel/divisor, giảm PNG503540→439382byte. Ưu tiên greedy khi hòa để pack cũ không đổi byte; regression reconstruct đủ tám sprite, repack v5 nguyên SHA. Không giảm sampling để chữa packing.

### Retarget phải kiểm cả nhánh khớp và clock — 2026-09-11

Võ bốn nhịp dùng mốc source + nội suy góc để giữ chiều dài, không chỉ đổi tên frame trên đường chân sin cũ. Đặt góc ở bind-space tránh mirror làm sai world rotation. Hand-target error nhỏ chưa chứng minh tay ôm gối: thêm điều kiện khuỷu dưới vai và continuity khi restart để bắt nhánh IK gập ra sau đầu. Tham khảo mô hình target+bend direction của [Spine IK](https://us.esotericsoftware.com/spine-ik-constraints), áp dụng `LimbSolver2D.flip` sẵn có, không thay engine hay sinh lại base. Capture tiến nhanh2giây phải kết thúc transition, không khởi tạo stop mới; source và rig nhận cùng step. Bằng chứng: build/vo-source-retarget-v1 và vo-source-retarget-registered-v4.
