# Map01A — tối ưu ảnh, tỷ lệ và bài học cần kế thừa

Chỉ đạo owner: giảm chất lượng/dung lượng runtime theo kích thước thực sự hiển thị; asset phải tách ghép dùng lại được; xử lý mobile/tablet/PC trong batch, kiểm sau khi gom xong. Source HD giữ ngoài Unity để tái xuất, không tạo lại ảnh chỉ vì đổi độ phân giải.

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
