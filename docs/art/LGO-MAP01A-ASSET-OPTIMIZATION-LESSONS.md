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

### Body nhảy/lộn và authoring native — 2026-09-13

Owner cho phép sửa nguồn/pipeline sai trong giai đoạn đầu, kể cả body khi có bằng chứng; khóa cũ không phải lý do giữ lỗi. Mỗi sửa đổi cần giữ provenance, đối chiếu tác động và ghi kinh nghiệm để tái dùng. Quyền sửa không biến candidate hoặc technical test thành visual acceptance.

| Lỗi/giả định đã gặp | Kết luận và cách làm kế thừa | Trigger kiểm lại |
| --- | --- | --- |
| Hash body/root scale đúng được hiểu là anatomy khớp 1:1 | Tách anatomy nguồn, registration, root/world transform và fit trang phục. Sáu pose atlas hiện cùng `1.70/1536`, nhưng vai/tay/đầu jump vẫn cần review. Không đo chiều cao bbox pose co người để normalize về idle. | Đổi body/pose/hash, hierarchy/camera hoặc mapping liên quan; không chạy lại vì đổi status. |
| Dùng số đo audit cũ cho bộ pose mới | Audit run/idle `0.756` thuộc run-ab-v5 khác SHA current, đã loại khỏi kết luận. Tra source/hash trước reuse số liệu. Landmark guide DRAFT chỉ là ước lượng chiếu 2D, chưa phải chiều dài xương/dung sai đã duyệt. | SHA hoặc phương pháp đo thay đổi. |
| Native giữ raw body nhưng reference hiển thị có thể scale | Export revision kiểm cả raw pixel, clone source UUID, lock và projection của body reference. Kiểm master A/B xuyên các pose, không chỉ từng cặp. Giữ KRA revision/hash và input job. | Sửa cấu trúc layer, master, mask hoặc exporter; không cần Unity cho kiểm native này. |
| PNG export Krita bật dialog dù document batch mode | `Node.save()` cần `Krita.instance().setBatchmode(True)`; khôi phục trạng thái app sau thao tác. Save KRA, đóng/mở lại rồi đối chiếu PNG với projection. | Đổi phiên bản/tool API/exporter; không cài hoặc thử lại từ đầu khi bản cài còn hoạt động. |
| Yêu cầu transparent nhưng output có caro vẽ thật | Hai lần built-in ImageGen trả RGB 1024×1536; không có kênh alpha. Giữ cả hai output lỗi; dừng lặp cùng prompt alpha, chuyển cleanup bằng mask native và review mép. Không key màu xám toàn ảnh vì sẽ phá tóc/áo. | Thay phương pháp tạo alpha; không gọi generation lần nữa chỉ vì prompt đã nhấn mạnh transparency. |
| Chỉnh trang phục bằng mắt theo từng pixel | Tạo gate đo anchor từ `pose-registration-guide.json`: cổ, vai gần/xa, hông gần/xa, trục thân, đường vai, đường đai và vùng ngực/eo cho từng pose. Kết quả là dữ liệu review/phom, không phải auto-warp hoặc offset runtime. Output có sanity flag; dữ liệu thật hiện báo `run_b/shoulderWidthPx=60.08px`, chỉ bằng `0.34x` median `176.18px`, nên guide phải được review/sửa trước khi fit asset tiếp. | Đổi landmark guide, body source/hash, pose canvas hoặc bắt đầu fit `outer_top`/`waist_belt`/`shoulder_chest_guard` mới; không tiếp tục sửa contour nếu số đo anchor đang bất thường. |

Evidence tái dùng: `build/pose-matched-layer-authoring-v1/body-proportion-audit.json`; `native-revision-test-v3/test-report.json` trong cùng root (72 re-export PNG, master edit cập nhật A/B, hai dạng sửa body bị reject); 11 unit test của `tools/test_lgo_krita_layer_authoring.py`. Native body candidate ngoài repo tại `LGO-Selected-2D-Source-v1/class-work-in-progress/vo-lv001/jump-anatomy-candidate-v1`: có KRA mở lại kiểm ba source layer, còn alpha/anatomy WIP. Unity build/capture batch này: 0 vì chưa có source đạt để ingest. Các thử nghiệm sửa một pixel là fixture kiểm tool, không phải asset mới.

Khi resume, đọc manifest/evidence hiện hành và kiểm đúng input/hash/version trước reuse. Chỉ chạy lại gate bị tác động bởi sửa mới hoặc có bằng chứng cũ không còn áp dụng; giữ lỗi thật và lý do chạy lại trong cùng batch, không nới assertion hoặc tạo báo cáo mới thay cho sửa lỗi.

Cleanup alpha tiếp nối: đo vùng nền candidate cho RGB spread ≤4 và maxRGB tối thiểu 113–121; từ đó tạo transparency mask native với spread ≥10 hoặc maxRGB <105, giữ raw source. Đây là điều kiện riêng của source đã đo, không preset chung cho mọi ảnh xám. Review phát hiện lỗ nhỏ vạt đai; vá 37 vùng kín ≤64px, tổng 102px, không đóng các khe lớn. KRA save/reopen và PNG/projection khớp; đã xem nền sáng/tối. Evidence ngoài repo `jump-anatomy-candidate-v1/native-alpha-v2/{alpha-report.json,creation-script.py,jump-alpha.kra}`. Khi input/hash đổi phải đo lại phân bố màu và review mép; không mặc định reuse threshold. Alpha đạt xử lý không đóng anatomy/registration gate.

Review anatomy tiếp nối: dùng board head 1:1 và upper-arm rotate-only, không normalize crop. Chưa có căn cứ thu toàn đầu/jump; vấn đề quan sát rõ hơn ở khối deltoid. Vá cục bộ trong KRA giữ 232.908 pixel đục ngoài vùng vá; alpha boundary cleanup riêng. Hai lỗi export đã tìm nguyên nhân: layer `Background` mặc định đang bật gây PNG nền trắng; thử `removeChildNode/addChildNode` đổi parent làm mất transparency masks trong KRA lưu lại. Cách đã kiểm trong `shoulder-repair-v3`: giữ cây layer, ẩn/khóa Background, save/reopen rồi kiểm đủ mask, alpha ngoài figure=0 và PNG=projection. Không suy từ việc export thành công hoặc pixel=projection rằng alpha đúng. Giữ v1/v2 lỗi; v3 là candidate, chưa anatomy/Player PASS.

Outer-top material idle tiếp nối: dùng phom/lineart/mask làm control giúp output đi đúng Lv1 hơn donor coat cũ, nhưng built-in ImageGen vẫn trả RGB có nền caro giả. Không dùng thẳng output; đưa vào Krita native, áp alpha phom, save/reopen, xuất PNG từ projection và review trên nền sáng/tối. `outer-top-material-idle-v2` technical PASS, nền giống caro còn khoảng 0,3% trong vùng alpha, nhưng visual chưa accept vì collar/neck lệch phom và chi tiết xanh-vàng ở gấu trái chưa rõ ownership. `outer-top-material-idle-v1` tạo được file nhưng mất report do cleanup/report order, nên script sau phải ghi report trước khi đóng document.

Repair material idle tiếp nối: technical PASS không đủ; v3/v4/v5/v6/v7 đều bị giữ làm bài học. V3 tô polygon collar thô làm hỏng neckline; v4 cắt alpha hem tạo lỗ và mất continuity; v5 generation mới sửa ý tưởng collar/hem nhưng dịch vai/tay áo và đưa nền tối/glow vào vùng alpha; v6 composite chéo v5 vào v2 trong mask tạo ghost patch vì hai generation không cùng placement/material; v7 repaint hem native giữ alpha nhưng thành mảng vá lộ. Kết luận kế thừa: chọn `outer-top-material-idle-v2` làm best current, tạo `outer-top-idle-repair-targets-v1` với mask collar/neck và hem ownership trong KRA để sửa source-level có vùng tác động rõ. Sau nhiều lỗi cùng giả định, không tiếp tục polygon/cut/composite/repaint lớn; vòng sau chỉ nên dùng source-paint/inpaint nhỏ theo mask hoặc review lại ownership hem. Trigger kiểm lại: mọi đổi phom, ImageGen source, anatomy occlusion hoặc ownership collar/hem; không pack khi mới có idle hoặc khi revision chỉ pass kỹ thuật mà visual reject.

Small inpaint tiếp nối: ngay cả khi prompt yêu cầu sửa chỉ vùng mask, output v8 vẫn có alpha nhưng phóng/dịch toàn áo và mang nền/glow. Reject trước native compose để tránh lặp lỗi v6. Bài học: với built-in edit/generation, phải kiểm cùng canvas/placement trước khi dùng làm patch; có alpha không chứng minh output là patch hợp lệ.

Waist-belt phom idle: kiểm belt trên body trần chưa đủ, phải layer cùng outer_top vì belt nằm phía trên áo ngoài. V1 fit eo nhưng quá dày và tail che hạ y; v2 mảnh hơn, tail ngắn hơn và được chọn làm current draft. Đây chỉ là phom/mask, chưa phải material hay source acceptance. Trigger kiểm lại: đổi outer_top baseline, đổi belt contour, hoặc bắt đầu nhân pose/material; không dùng preview body-only để tự approve belt.

Waist-belt material idle: generation v1 đạt kỹ thuật mask/export nhưng visual reject vì nền tối/glow còn nằm trong alpha và che mất buckle/chi tiết khi đặt trên outer_top. V2 dựng deterministic bằng Krita trên phom v2 giữ đúng placement, có belt xanh đậm, viền vàng và khóa xanh, hiện là current best candidate review-required. Bài học: với món mảnh nằm chồng trên áo, generation toàn mảng dễ biến nền/thân áo thành vật liệu; nếu chưa có local mask rất chặt và kiểm board layered, ưu tiên material native/controlled paint. Trigger kiểm lại: thay outer_top, đổi phom/tail/buckle, hoặc nhân sang pose có xoay/che khuất; không pack từ một idle material chỉ vì native export pass.

Waist-belt run draft: có thể nhân style deterministic từ idle sang bốn pose chạy mà chưa đụng jump, nhưng phải ghi rõ đây là draft placement. `waist-belt-run-four-material-v1` phủ `run_contact_a/run_a/run_contact_b/run_b` bằng KRA native và board body+belt; chưa có outer_top run context nên tail/buckle có thể cần sửa sau khi áo ngoài chạy tồn tại. Bài học: nhân pose theo anchor thủ công giúp tiến nhanh, nhưng coverage phải phân biệt “candidate pose covered” với “A/B source accepted”; không cho phép pack chỉ vì đã đủ PNG draft. Trigger kiểm lại: chỉnh anchor/angle, thêm outer_top run, duyệt/sửa jump anatomy hoặc tạo biến thể B.

Outer_top run polygon failure: hai lượt `outer-top-run-four-material-v1/v2` chứng minh cách đặt polygon torso thủ công theo vài điểm trên body tạo hình như tấm khiên nổi trên vai/lưng, không phải áo bám thân. Đây là dấu hiệu làm việc manh mún: sửa từng contour làm tăng file/evidence nhưng chưa tạo kết quả người chơi kiểm được. Bài học: với slot chi phối silhouette như outer_top, phải review theo stack bốn pose và chọn phương pháp bám silhouette/source-paint trước; chỉ khi outer bám thân mới chỉnh waist/guard theo nó. Trigger kiểm lại: mọi outer run candidate mới phải có board body+outer và body+outer+waist; nếu lại floating-shield thì dừng hướng đó thay vì tạo v3/v4 bằng cùng giả định.

Feedback owner 2026-09-13: nếu sau nhiều giờ chỉ có validator, provenance và candidate reject, đó là tín hiệu quy trình sai nhịp. Validator giúp chặn promote sai nhưng không thay thế kết quả người chơi kiểm được. Từ đây mọi batch trang phục phải có một artifact stack-level rõ ràng hoặc một decision/proof thay đổi phương pháp; không tiếp tục tạo nhiều thư mục candidate nhỏ chỉ để có tiến độ.

Measurement lock tiếp nối: vòng sau không được tạo thêm `outer_top` bằng polygon/body-mask hoặc “dịch vài pixel” trước khi anchor guide sanity pass. Công cụ hiện hành `tools/measure_lgo_pose_garment_anchors.py` xuất `garment-anchor-measurements-v1.json` và overlay đo tại selected source; status cố ý là `GARMENT_ANCHOR_MEASUREMENTS_REVIEW_REQUIRED`, `runtimeEligible=false`, `guideSanity.status=ANCHOR_GUIDE_SANITY_REVIEW_REQUIRED`. Report có `directComparison.fixTargets`: ví dụ `run_b` hiện giữ `near_shoulder=[568,542]`, mục tiêu vai `176.18px` ở góc `18.56°`, gợi ý review `far_shoulder≈[735.02,598.08]` thay vì dịch áo. Khi sanity flag hoặc overlay cho thấy pose nào lệch như `run_b`, sửa dữ liệu landmark/body candidate trước rồi mới sinh/fill trang phục theo lineart/mask đã đo. Đây là cách thay đổi phương pháp, không phải thêm một validator để thay thế review visual.

Anti-repeat gate 2026-09-13: khi một hướng đã fail hai lần cùng nguyên nhân, mọi candidate tiếp theo cùng họ phương pháp bị coi là **quy trình sai**, không phải “cần thêm một chỉnh nhỏ”. Với trang phục pose hiện tại, các họ phương pháp đã bị khóa là polygon torso vẽ tay, body-mask fill lớn, composite generation lệch placement và chỉnh contour/pixel không có anchor đo. `tools/audit_lgo_pose_method_repeats.py` scan candidate root và trả `METHOD_REPEAT_BLOCKED`/exit 2 nếu đề xuất lặp họ đã khóa; evidence thật `source-review-v1/method-repeat-audit-v1.json` đang khóa cả bốn họ cũ và chỉ cho hướng `measured_anchor_stack` đi tiếp. Batch sau chỉ được tiếp tục nếu có một trong ba bằng chứng mới: (1) guide/body đã sửa và sanity pass, (2) artifact stack-level bốn pose chứng minh outer_top bám thân tốt hơn rõ rệt, hoặc (3) proof authoring khác có contract đo/so sánh trước khi sinh asset. Nếu không có bằng chứng này, agent phải dừng sinh asset, mở evidence lỗi gần nhất, cập nhật decision/lessons, rồi chọn phương án khác; không được tạo thêm v4/v5/v6 để “thử xem”.

Hiệu quả batch phải được tự audit trước khi chạy tiếp: liệt kê artifact người chơi/reviewer sẽ kiểm được, số candidate đã reject theo họ phương pháp, số gate đã chạy lại và lý do, phần evidence còn reuse được, điều kiện khiến batch tiếp theo khác batch fail. Nếu câu trả lời chỉ là “đổi vài tọa độ/mask/prompt” thì chưa đủ khác phương pháp. Validator, provenance hoặc file status không được tính là kết quả nếu không làm rõ được quyết định sản phẩm hoặc đường sửa tiếp theo.

Nguyên tắc kế thừa lỗi: giai đoạn đầu được phép sửa rộng body, asset và pipeline khi thấy sai, nhưng mỗi hướng bị reject phải ghi rõ nguyên nhân, evidence, điều kiện tái kiểm và phần còn được reuse. Nếu evidence nguồn/tool vẫn cùng hash/canvas/profile, không kiểm lại từ đầu; chỉ chạy lại gate bị ảnh hưởng bởi thay đổi mới. Mục tiêu là biến sai lầm thành guardrail vận hành, tránh lặp lại cùng giả định trong batch sau hoặc dự án khác.

Shoulder/chest guard idle: deterministic v1 chứng minh native Krita round-trip và placement sạch, nhưng visual reject vì guard vai quá rộng và tấm ngực cạnh tranh với charm/ornament phía trước. V2 là partial WIP/tooling failure trong Scripter, giữ lại để biết export có thể có trước khi provenance/board hoàn chỉnh. V3 thu gọn guard nhưng vẫn cạnh tranh charm và selection đầu thiếu full-stack waist context. V4 chừa vùng charm/collar rõ hơn, được chọn làm current best candidate review-required; guard-only silhouette còn hơi rời/rỗng nên chưa source-accept. Bài học: món giáp nằm trên outer_top phải được review với toàn stack body + outer + waist + ornament, không review guard-only; cần giữ readability của collar/charm trước khi nhân pose. Trigger kiểm lại: thay collar/outer_top, đổi accessory/charm ownership, đổi waist layer stack, hoặc chuyển sang pose có vai xoay; không dùng technical KRA/export pass làm source acceptance.

Coverage gate cho pipeline nhiều slot: sau khi có vài idle candidate, cần sinh coverage JSON/board tổng thể trước khi nghĩ tới pack. Tool `tools/audit_lgo_pose_layer_authoring_coverage.py` và test tương ứng biến kiểm tra này thành lệnh lặp lại được; report `pose-layer-coverage-v3.json` cho thấy `inner_top` và `class_accessory` đủ file sáu pose A/B, `waist_belt` phủ candidate 5/6 pose, còn `outer_top` và `shoulder_chest_guard` mới idle-only. Bài học: một board idle đẹp hoặc một draft 5/6 pose không chứng minh pipeline khớp pose; promotion chỉ được xem xét khi coverage theo slot/pose/variant, anatomy jump và ownership collar/hem đều có evidence. Trigger kiểm lại: thêm pose mới, đổi current-best slot, sửa body authority/candidate hoặc cập nhật packer/composer.

## Checkerboard nhìn như alpha không phải source trong suốt — 2026-09-13

Built-in ImageGen được yêu cầu tạo một sheet cutout skeletal có nền trong suốt nhưng output `generated-cutout-sheet-v1-rejected.png` là RGB 1254×1254, không có alpha; checkerboard chỉ là pixel nền. Sheet còn lặp tay/chân và gộp belt/tail vào thân áo. Gate phải đọc mode/alpha trước mọi segmentation. Không cắt checkerboard theo màu, inpaint hoặc sửa pixel để cứu sheet này; giữ provenance ngoài repo và đổi đơn vị tạo nguồn sang native/vector layer rõ hoặc từng component độc lập có alpha được kiểm.

Thử đổi từ whole sheet sang đúng một component `upper_torso` vẫn tái hiện lỗi: RGB, không alpha, zero transparent pixel; hình còn lẫn waist/lower panels ngoài ownership yêu cầu. Sau hai lần cùng lớp lỗi, phương pháp raster ImageGen hiện tại bị đóng cho probe này. Quy tắc kế thừa: thay prompt hoặc thu nhỏ đầu ra không được tính là phương pháp mới nếu output contract vẫn không được máy kiểm; lần tiếp theo phải đổi sang SVG/native layer có dimension/origin/ownership rõ hoặc source layer do họa sĩ cung cấp.

## Không tiếp tục chỉnh trang phục từng pixel khi chưa có proof tool — 2026-09-13

Feedback owner về pipeline trang phục Pháp Lv1 cho thấy vòng sửa `outer_top` lineart/mask từng chút một không đủ hiệu quả: có measurement gate và stack board nhưng vẫn dễ biến thành nắn hình thủ công không có hồi kết. Từ thời điểm này, trước khi sinh/chỉnh thêm candidate trang phục mới phải có một proof tool-assisted hoặc một batch brief chứng minh phương pháp khác hẳn họ đã fail. Hướng ưu tiên là kiểm Unity/Krita local: source-space landmarks → shared skeleton/weight field → SpriteLibrary component categories → report không pose-local scaling. Spine/Comfy chỉ được mở như feasibility riêng khi có tool/model/workflow chạy được; không dùng tài liệu hay preview để claim production path.

### Tái dùng evidence đúng phạm vi — 2026-09-13

Trước một spike mới, kiểm output đã có. Audit 24 cặp Krita A/B xác nhận alpha giữ nguyên, 12 cặp có nội dung đổi RGB; vì vậy lặp thử nghiệm đổi màu không giải quyết thiếu phom. Envelope chỉ có mốc cơ thể chưa phải mapping trang phục. Planner đã bỏ quyền authoring tự động từ số đo sạch, regression bảo vệ lỗi mở gate này. Không suy anatomy chính xác từ median hoặc số test, không tính mẫu hình học cùng công thức là hai thiết kế production. Chi phí setup phom và chi phí item kế tiếp phải ghi riêng; chưa đo thì chưa hứa tốc độ.

### Blender/tool proof phải kiểm output thật — 2026-09-13

PID ứng dụng còn sống không chứng minh toolchain còn dùng được: Blender PID 81705 vẫn chạy từ path cũ nhưng executable không còn mở được. Sau khi owner cho phép tải lại, Blender 4.5.5 LTS được phục hồi vào `build/toolchains/blender/Blender.app` và `--version` pass. Probe render đầu tiên còn một lỗi khác: Blender log `Saved` nhưng PNG alpha rỗng vì camera clip cắt mất mesh. Chỉ sau khi kiểm alpha/bbox và xem board mới được ghi `NARROW_TECHNICAL_PASS_TOOLCHAIN_ONLY`.

Bài học áp dụng rộng: proof pipeline phải chứng minh đúng chuỗi dữ liệu, không chỉ chứng minh công cụ chạy. Với trang phục, số đo có giá trị khi nó trở thành input sinh mesh/UV/pose export; nếu chỉ dùng để người kéo pixel thì vẫn là vòng thủ công cũ. Trigger kiểm lại: đổi Blender/toolchain, đổi script render/camera/pass, đổi source-space profile, hoặc bắt đầu dùng proof synthetic để quyết định production. Evidence `build/structured-garment-method-verification/blender-discovery.json`, `build/structured-garment-method-verification/blender-parametric-proof-v1/report.json`, `review-board.png`.

Direct-3D readiness phải đi hết đường dữ liệu authoring → interchange → engine import. Probe modular mới xuất đồng thời GLB để kiểm cấu trúc và FBX vì project Unity hiện không có GLTF importer. Clean Blender re-import xác nhận skin/action/object; Unity import tạm xác nhận `SkinnedMeshRenderer`, clip và equipment names rồi xóa model khỏi `Assets`. Đây vẫn chỉ là toolchain proof: không dùng số object/clip để suy ra animation đẹp, anatomy đúng hoặc renderer 3D nên thay game 2D. Trigger tiếp theo là isolated Player common task với art/proportion đủ review; không đưa cube proxy vào Map01A.

Tên layer không chứng minh source đã được tách. ORA canonical nam có đủ 11 tên `AUTHOR` và đúng canvas, nhưng audit alpha cho thấy 11/11 layer rỗng; hình thật chỉ ở reference composite. Từ nay template layered phải qua content gate trước khi lập rig/job. Không dựng anatomy bị che bằng cách cắt ngược áo/quần khỏi composite; cần anatomy authority mới hoặc layer do họa sĩ author thật.

Occlusion proof nối tiếp: overlay flat-panel lên body thật nhìn được nhưng fail định lượng 18/18 vì phủ vùng da, jump nặng nhất hơn 31k pixel. Cắt theo skin-color heuristic xóa được conflict mà không sửa PNG từng pose, chứng minh mask/occlusion pass nên là dữ liệu máy dùng lại, không phải thao tác brush từng ảnh. Nhưng heuristic màu không phải semantic ownership; bước production cần body-part/slot mask hoặc weight/depth thật. Evidence `occlusion-conflict-audit.json`, `occlusion-clipped-v1/occlusion-clip-report.json` trong cùng thư mục Blender proof.

GarmentCode proof nối tiếp: core `test_garmentcode.py` chạy được trong venv riêng và sinh rập `t-shirt` JSON/SVG/PNG/PDF. Điều này xác nhận hướng pattern tham số là executable local, không chỉ tài liệu. Giới hạn vẫn rõ: chưa chạy drape/simulation vì cần NVIDIA Warp fork, chưa gắn với body/pose/occlusion/render LGO. Bài học: dùng GarmentCode làm nguồn cấu trúc/phom, còn sprite pose-matched cần bridge riêng qua Blender/rig/mask; không được coi rập 2D là item game đã xong.

Khuyến nghị modular e7129 được nhận có điều kiện: đúng hướng lâu dài là family trang phục + compiler + resolver, nhưng mỗi analog phải chuyển thành gate LGO cụ thể. Roblox/UMA/Spine giúp đặt câu hỏi về cage, hide data, weights và linked mesh; chúng không phải dependency production tự động. Bridge GarmentCode JSON sang Blender đã pass kỹ thuật nhưng fail occlusion 18/18 khi fit panel 2D trực tiếp; đây là ngõ cụt đã khóa. Bài học kế thừa: mọi pilot mới phải chứng minh một item chưa chuẩn bị trước trong family đã khóa, không sửa PNG từng pose; nếu không làm được thì fail family/module, không biến thành bảng ngoại lệ theo từng level.

Compiler pilot gate 2026-09-13: số đo chính xác chỉ có giá trị lâu dài khi đi vào planner/validator có thể chặn sai phương pháp. `plan_lgo_pose_pipeline_next_action.py --compiler-pilot` nay yêu cầu proof đủ sáu pose gồm `jump_tuck`, semantic occlusion hoặc weights, surface reuse, geometry-param regeneration, unseen item sau template lock, zero per-variant pixel edits và không runtime offset. Fixture từ bridge GarmentCode hiện bị chặn ở `build/structured-garment-method-verification/planner-current-gate-v1/next-action.json`: lỗi còn lại không phải thiếu ảnh mà là thiếu ownership semantic/unseen reuse và dùng direct panel fit. Trigger kiểm lại: chỉ khi có pilot mới với body-part mask/weight/depth thật hoặc source family compiler khác, không chạy lại bằng cùng rập affine.

Semantic compiler pilot proof: `semantic-compiler-pilot-v1` cho thấy một family nhỏ có thể sinh A/B/C/D trên sáu pose từ mask bộ phận, giữ hidden surface dưới tay trước và kiểm reuse bằng hash geometry. Đây là bằng chứng khác hẳn vòng nắn pixel: unseen item D được tạo sau template lock và vẫn qua cùng rule. Nhưng output là body/procedural để test contract, chưa phải body/art LGO; không được chuyển thẳng sang runtime hoặc claim production. Trigger tiếp theo là thay procedural body bằng body authority/source-space thật và giữ cùng gate; nếu fail thì sửa mask/ownership, không quay lại fit panel trực tiếp.

Source-space guide pilot nối tiếp: khi dùng `common-male-v1/pose-registration-guide-v1` thật, mask near-arm dạng polygon khớp fail vì quá mảnh và không che áo ở nhiều pose. Root cause là sai biểu diễn ownership bộ phận, không phải thiếu thêm màu hoặc thêm polygon áo. Đổi near-arm thành capsule theo bề rộng vai làm `semantic-source-space-pilot-v1` pass cùng gate A/B/C/D/unseen. Bài học: semantic mask phải đại diện vùng chi thật, không chỉ nối landmark bằng đường mảnh; mọi compiler sau cần kiểm hidden pixels theo từng pose để bắt lỗi ownership trước khi sinh material.

Measurement fixture không được thay guide thật: `lv1-short-vest-family-candidate-v1` từng pass compiler nhưng khi đưa guide gốc qua measurement thì planner chặn `FIX_GUIDE_BEFORE_ASSET`. Lỗi cụ thể là `run_b` shoulder collapsed 60.08px so với median 176.18px. Candidate guide review-only sửa `far_shoulder` theo target đo được làm sanity pass và tạo `lv1-short-vest-family-candidate-v2-fixed-guide`, nhưng planner sau đó được sửa tiếp để không mở asset authoring khi `guideStatus=CANDIDATE_REVIEW_ONLY`; trạng thái đúng là `REVIEW_GUIDE_CANDIDATE_BEFORE_ASSET`. Bài học: compiler pass phải chạy cùng measurements thật của guide đang dùng; nếu đổi guide sau khi sinh asset thì phải regenerate candidate; nếu guide mới chỉ là candidate thì chỉ review/duyệt guide, không dùng board đó để đi tiếp sản xuất.

Run_b shoulder review không được chọn theo số đo duy nhất: target tự động preserve-near đạt 176.18px nhưng crop cho thấy điểm rơi sâu về vùng tay trước, dễ sai anatomy. Alternatives C/D được đo riêng và đều sạch guide sanity, nhưng D kéo vai/áo ra tay nhiều hơn; C `visual_torso_edge_candidate` giữ endpoint ở mép thân hơn nên là default review tiếp theo. Bài học: metric target là gợi ý điều tra, không phải tọa độ authority; mọi outlier fix cần board trước/sau và family preview để thấy tác động lên garment.

Continuity score cũng phải có hard anatomy flags. Audit v1 ưu tiên width/smoothness nên chọn nhầm B dù visual nằm trên tay trước. Audit v2 thêm corridor tay trước và loại endpoint rơi vào vùng đó trước khi xếp hạng; kết quả C đứng đầu, D thứ hai, A fail outlier, B fail `far_shoulder_inside_near_arm_corridor`/`far_shoulder_reaches_forearm_zone`. Bài học: score tổng hợp không được che điều kiện hình học bất khả xâm phạm; nếu một điểm thuộc vùng chi trước thì không được là shoulder authority chỉ vì metric đẹp.


## 2026-09-13 — Không để source authority khóa sai kiến trúc nhân vật

Feedback owner 3ca3 được nhận như một bài học cấp kiến trúc: nguồn/pose/renderer hiện tại là authority vận hành, không phải cam kết giữ nguyên. Khi một pipeline cần sửa body, shoulder, mask hoặc layer từng chút để qua một item, phải nâng vấn đề lên đơn vị sản xuất nhân vật/trang bị thay vì tiếp tục vẽ lại từng pose. Từ nay trước khi tạo thêm garment candidate production, chạy gate kiến trúc hữu hạn: A 2D skeletal runtime modular và B direct 3D modular runtime trên cùng bài test, đo item unseen sau khi khóa template/tooling. Pose-image pipeline cũ chỉ dùng làm baseline đối chứng cho đến khi nó thắng gate bằng evidence.
