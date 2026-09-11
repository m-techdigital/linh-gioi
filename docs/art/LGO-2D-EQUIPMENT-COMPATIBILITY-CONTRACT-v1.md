# Hợp đồng tương thích trang bị 2D v1

Ngày 2026-09-10. Hợp đồng này áp dụng cho Võ/Kiếm/Pháp/Cơ/Linh Lv1–30 và mở rộng được về sau. Nó phân biệt rõ ba lớp: **design source**, **attachment đã fit**, và **runtime item**. Một crop đẹp hoặc đúng tên chỉ là source candidate; không tự trở thành attachment hay runtime asset.

## Quyết định kiến trúc

Mỗi nhân vật dùng một `skeletonVersion`, một `bodyProfile` và `classId` ổn định. Mỗi món đồ có `itemId` độc lập, đúng một trong 10 `slotId`, `unlockLevel`, allow-list class/body profile, trạng thái fit, các attachment component, coverage/occlusion tag và provenance. Loadout là ánh xạ `slotId → itemId`; không có `setLevel` hoặc tier chung ép cả bộ. Đồ common/cosmetic muốn mặc nhiều class phải khai báo rõ allow-list, không suy ra từ chữ `unisex`.

`unlockLevel` chỉ là điều kiện tiến trình. Khi nhân vật đủ level, họ được phối tự do đồ Lv1/Lv10/Lv20/Lv30 ở các slot khác nhau. Compatibility không được suy từ level, tên file, class palette, kích thước crop hoặc vị trí ô trong board.

## Trạng thái asset bắt buộc

- `candidate`: đã chọn/cắt nguồn, chưa kiểm khớp base; `runtimeEligible=false`.
- `approved`: đã khớp skeleton/body, đủ component/bone/anchor, coverage/occlusion và qua motion evidence; mới được đóng atlas/runtime.
- `redraw-required`: dính body/da, sai contour/góc/tỷ lệ, gộp slot, thiếu vùng che khớp hoặc không thể chuyển động ổn định. Thiết kế/cắt lại ở canvas template; không cứu bằng scale/offset tùy ý trong runtime.

Chuyển `candidate → approved` cần người review ảnh và evidence Player. Validator tên/hash/alpha không có quyền cấp trạng thái này.

## Attachment và layering

Mỗi item có một hoặc nhiều component, mỗi component khai báo `componentId`, `boneId`, sort band và template canvas. Cặp trái/phải là component của cùng item. Item chỉ tương thích khi skeleton version trùng, body profile nằm trong allow-list và mọi bone tồn tại.

Coverage tag mô tả vùng item vẽ; hide tag mô tả vùng bị món đó che. Ví dụ `outer_top` có thể che `torso_inner`, boots có thể che `lower_shin`, tóc trước/sau dùng component và sort band riêng. Quy tắc che được hợp thành từ loadout hiện tại, không bake một bộ full outfit.

## Gate theo lô

1. Batch crop đủ ma trận, giữ SHA/provenance và trạng thái `candidate`.
2. Review contact sheet chỉ loại lỗi lớn: sai style, nhầm row, dính da/body, chữ/grid/artefact. Không claim fit.
3. Fit trên neutral base nam/nữ ở authored scale; item sai được redraw theo template base.
4. Kiểm ma trận phối chéo theo pairwise coverage: mỗi level xuất hiện cùng từng slot khác, gồm hair/outer/lower/waist/guard/footwear/weapon có nguy cơ va chạm; item class-specific phải bị chặn khi class không nằm trong allow-list.
5. Chạy idle/walk/run/jump/basic attack/class skill, equip/unequip và nền sáng/tối.
6. Pack atlas theo residency sau khi approved. Logical item ID không phụ thuộc atlas; mobile/tablet/PC dùng cùng source resolution và thay Max Size/compression theo platform.

Không thử toàn bộ tích Descartes `4^10`. Pairwise matrix cộng các bộ worst-case silhouette cho xác suất bắt collision cao hơn với chi phí kiểm chứng hữu hạn. Mọi lỗi mới phải thêm case tái hiện vào matrix hoặc validator trước batch class tiếp theo.

## Hiệu năng và dung lượng

Thiết kế theo kích thước hiển thị thật tại camera chơi; không tạo canvas lớn rồi thu nhỏ. Attachment template có vài nhóm hình học cố định như short/wide/long weapon, short/long coat và hair front/back thay vì một canvas cực đại. Atlas ưu tiên 512–1024 cho prototype mobile; chỉ tăng khi báo cáo occupancy/quality chứng minh cần. Không đánh giá RAM bằng byte PNG: phải tính texture format, kích thước giải nén, mipmap và residency.

Theo tài liệu chính thức, Unity Sprite Library dùng Category/Label và variant để thay sprite, còn Sprite Swap skeletal yêu cầu cùng skeleton. Spine mix-and-match ghép item skin/attachment trên một skeleton; prepack phù hợp tủ đồ hữu hạn, atlas tải thêm phù hợp kho đồ lớn. Runtime repack là tối ưu tùy chọn sau benchmark, không phải cách sửa source sai fit.

Thiết kế sheet phải theo camera gameplay. Grid front/3/4 chỉ được làm inventory/reference; attachment của Player side-view phải được redraw side-view hoặc có mesh/weight được chứng minh trên cùng skeleton. ImageGen output luôn bắt đầu ở `candidate`; một edit sạch body bằng mắt vẫn chưa đủ để nâng `approved`.

Runtime dùng hybrid attachment. `Rigid` dành cho kiếm, phụ kiện và chi tiết không uốn; chúng theo bone bằng transform. `Skinned` dành cho tóc dài, áo và vạt cần biến dạng; sprite loại này phải có bone data trước khi được đăng ký. Sprite Library dùng `componentId` làm Category và `itemId` làm Label, nên loadout phối chéo level không cần atlas hoặc code riêng theo tier. Adapter preflight toàn bộ library entry/renderer trước khi apply để tránh trạng thái thay đồ nửa chừng.

Contact sheet toàn lô không thay thế review cận cảnh từng item proof. Một cell có thể nhìn như một slot nhưng thực tế chứa hai view; trường hợp đó là `REDRAW_REQUIRED_MULTI_VIEW_CELL`. Normalize chỉ thực hiện một lần từ working resolution xuống PPU/camera target, lưu hash và kích thước canonical; runtime không upscale để bù thiết kế sai.

Nguồn tham khảo:
- https://docs.unity3d.com/Packages/com.unity.2d.animation@13.0/manual/SLAsset.html
- https://docs.unity3d.com/Packages/com.unity.2d.animation@13.0/manual/SpriteSwapIntro.html
- https://en.esotericsoftware.com/spine-unity-mix-and-match

## Thiết kế theo hành động và phối chéo cấp — owner realignment 2026-09-11

**Trạng thái: phương án triển khai, chưa phải runtime đã hoàn thiện.** Owner bác hai sheet tách generic và yêu cầu bám design gốc. Reference/brief/SHA tại external `registered-equipment-authoring-v1/original-design-reference-v1`: turnaround03 làm chuẩn ngoại hình đề xuất, bảng detail15 chỉ tham chiếu ownership vì áo Lv1 không đồng nhất. Board sáu pose `action-designed-outfit-v1` chỉ gần đúng hướng động tác, không làm chuẩn thiết kế đồ. Giữ áo vạt chéo, viền và biểu tượng gốc; không đổi thành vest mở giữa, không tự thêm băng trán/giáp vai. Warp v1/v2 và sheet generic không dùng production.

### Đối chiếu hệ thống thực tế

| Nguồn chính thức đã đọc | Điều có bằng chứng | Áp dụng cho Linh Giới |
| --- | --- | --- |
| [MapleStory Worlds — Controlling Avatar Animations](https://maplestoryworlds-creators.nexon.com/en/docs/?postId=820) | Tách action của body và trang bị; hướng dẫn đồng bộ hai action để tránh thân đi nhưng đồ đứng. Item được xem theo action trong editor. | Một bộ điều khiển pose/thời gian cho body và mọi item. Đây là tài liệu MapleStory Worlds, không suy ra implementation nội bộ của MapleStory gốc. |
| [Spine — Mix and Match](https://esotericsoftware.com/spine-examples-mix-and-match) | Mỗi item là skin có nhiều attachment; vải có control riêng. Đường nối mesh cần cùng vị trí và weights để không hở. | Chia món theo cấu tạo, chung chuẩn đường may/khớp. Tà áo có chuyển động phụ riêng nhưng không kéo khớp cơ thể. |
| [Spine — Linked meshes](https://esotericsoftware.com/spine-meshes#Linked-meshes) | Mesh liên kết chia sẻ vertices, UV, weights và có thể kế thừa deformation. | Chỉ tái dùng mesh/animation khi cùng cấu trúc hình học; áo ngắn và áo choàng không bị ép vào một mesh. |
| [Unity 2D Animation 13 — Sprite Swap](https://docs.unity3d.com/Packages/com.unity.2d.animation@13.0/manual/SpriteSwapIntro.html) | Thay từng sprite qua Category/Label; skeletal swap yêu cầu skeleton giống nhau. | Giữ Unity hiện có; kiểm skeleton/profile trước apply. Đã đọc cả sample PartSwapUI và Skeleton Sharing trong PackageCache của project 13.0.0. |
| [Terraria/tModLoader — PlayerDrawLayers](https://docs.tmodloader.net/docs/stable/class_player_draw_layers.html) | Có các lớp riêng torso, skin, chân/phụ kiện trước/sau; vị trí lớp vũ khí thay đổi theo trạng thái. | Slot logic không phải một sortingOrder cố định. Một item có thể có nhiều phần nằm xen giữa các phần cơ thể. Đây là API tModLoader, không sao chép art hay code game. |
| [Spine — Runtime repacking và on-demand loading](https://esotericsoftware.com/spine-unity-main-components#Runtime-Repacking) | Repack tạo texture/material cần giải phóng; có chi phí và ràng buộc đọc/nén. Tải theo nhu cầu giúp giảm residency. | Ưu tiên đóng atlas offline và dùng chung texture; chưa thêm repack mỗi avatar hoặc mỗi lần mặc đồ khi chưa benchmark. |

Các dòng “Áp dụng” là kết luận thiết kế của dự án, không phải bảo đảm từ nhà cung cấp. Tham khảo Spine để học cách authoring, chưa cài/chuyển engine sang Spine. Không có thư viện nào tự biến ảnh phẳng thành bộ đồ đúng mọi góc nhìn.

### Lựa chọn

| Phương án | Chất lượng/chi phí | Quyết định |
| --- | --- | --- |
| Vẽ cả nhân vật cho mỗi bộ và động tác | Dễ review một bộ; khó tháo riêng và phối chéo, phải nhân ảnh theo tổ hợp | Chỉ dùng làm design/demo |
| Tự động uốn đồ dáng đứng bằng vài điểm khớp | Tiết kiệm ảnh nhưng thiếu mặt khuất, sai nếp gập/sole/tuck; thử nghiệm vừa rồi đã cho thấy giới hạn | Không dùng làm quy trình authoring chính |
| Body/action chuẩn + item nhiều phần + mesh và hình vẽ riêng khi đổi góc | Tốn công thiết kế chuẩn ban đầu; reuse theo họ cấu tạo và chỉ bổ sung góc thật sự cần | Phương án triển khai |

Một item phải được **thiết kế trực tiếp trong các động tác bắt buộc** trước khi quyết định phần nào tái dùng. Nếu mesh giữ được contour, đường may và chất liệu thì dùng chung. Nếu mặt trước chuyển sang mặt trong/đế giày hoặc bị co ngắn khi lộn thì vẽ component ở góc đó trong source gốc; đó là dữ liệu authoring có phiên bản, không phải vá ảnh Player hoặc chỉnh offset cho từng ảnh lỗi. Chỉ đổi màu/hoa văn trên một bộ hình học đã đạt mới được kế thừa mesh/weights/pose; silhouette mới cần kiểm lại hoặc vào họ cấu tạo mới.

### Chuẩn chung giữ mọi cấp khớp nhau

- `bodyProfile` và `skeletonVersion` không phụ thuộc level. Canvas 1024×1536, groundY1484 và phép chiếu 1,70/1536 giữ nguyên. Crop/atlas không thay tỷ lệ. Reference motion vẫn là sáu pose v3 div4; không lấy board sinh mới thay body hoặc tọa độ chuẩn.
- `actionProfile` sở hữu thứ tự bốn nhịp `run_contact_a → run_a → run_contact_b → run_b`, thời gian, facing, sockets và thứ tự che khuất. Mọi item nhận cùng pose/time. Đổi trang bị không reset animation và không tạo đồng hồ riêng theo tier.
- `fitFamily` mô tả cấu tạo như áo ngắn sát nách, quần võ bó gấu, bốt thấp. Mỗi family có chuẩn cổ/nách/eo/cổ tay/gấu quần/cổ giày, biên phủ và khoảng chừa chuyển động. Món mới phải bám chuẩn này ở toàn bộ action, không chỉ khớp một pivot khi đứng.
- Chỗ nối các mảnh của cùng bề mặt dùng cùng tọa độ và weights hoặc deformation tương đương đã kiểm. Chỗ hai món chồng nhau có vùng phủ và khoảng chừa được thiết kế sẵn. Không đòi biên áo ngoài dính cùng vertex với áo trong vì đó là hai lớp vải khác nhau.
- Loadout là `slotId → itemId`, ví dụ áo Lv1 + quần Lv10 + giày Lv1. `unlockLevel` không chọn pose, offset, body, hoặc toàn bộ atlas bộ đồ. Không blacklist tổ hợp khác cấp hợp lệ để che lỗi fit.
- Lớp cơ thể/fallback thuộc body, không thuộc món đồ. Không sao chép full body vào từng item hoặc sinh body variant cho mọi tổ hợp 10 slot. Phần bị che cần source đầy đủ theo body/action chung; tháo đồ phải trả đúng fallback. Không tự vẽ lại phần da đang nhìn thấy để vừa áo.
- Near/far là vai trò giải phẫu trong view đang author, không suy ra từ trái/phải màn hình khi flip. Flip áp dụng thống nhất lên body và attachment; layer crossing thuộc action profile.

### Phân rã theo cấu tạo bộ Võ nam gốc

Đây là danh sách source cần vẽ thành layer riêng, không phải chỉ dẫn crop board. Mỗi phần phải đủ vật liệu phía bị che để dùng được khi tháo món bên cạnh. Số phần tối thiểu được điều chỉnh khi bản source cho thấy cần thêm mặt trong/ngoài; số slot vẫn là10.

| Slot | Các phần source sở hữu | Cách hỗ trợ động tác |
| --- | --- | --- |
| `main_weapon` | Quyền khí gần/xa, mặt mu và lòng khi cần; không chứa ngón tay hoặc hộ uyển | Theo socket bàn tay; hình riêng cho nắm đấm/ôm gối nếu đổi mặt nhìn |
| `head_hair` | Tóc trước/sau; băng trán chỉ khi đúng thiết kế item, không tự thêm vào Lv1; không chứa mặt/đầu | Theo head; đuôi băng chuyển động phụ; hình cúi đầu riêng nếu cần |
| `inner_top` | Thân áo trong và phần lưng thực sự cần, mép cổ/nách | Thiết kế cổ/nách trong chạy và tuck; không dính áo ngoài hoặc da |
| `outer_top` | Vạt chéo liền thân, lưng, viền và hoa văn may trên áo; mặt trong khi lật, không tự cắt ngang eo | Mesh theo thân cho vùng ổn định; vạt và vùng nén/tuck có thiết kế riêng |
| `lower_body` | Hông/đũng, ống gần/xa, gấu và mặt nếp gập cần thiết | Có khoảng chừa gối/đũng; không uốn nguyên ảnh quần đứng thành tuck |
| `waist_belt` | Đai vòng, nút buộc, đuôi đai trước/sau | Giữ đường eo chung qua cấp; đuôi đai là của đai, không dính áo/quần |
| `arm_guard` | Hộ uyển gần/xa, mặt trong/ngoài cần thiết | Chừa khuỷu và cổ tay; hình co ngắn theo góc nhìn, không gộp quyền khí |
| `footwear` | Giày gần/xa, cổ giày, upper/đế khi đổi mặt | Khớp cổ chân/gấu; vẽ đúng đế khi chạy/lộn, không kéo giày đứng |
| `shoulder_chest_guard` | Miếng giáp gần/xa và dây thuộc giáp khi item có trong design; slot có thể rỗng ở Lv1 | Dưới/trên cánh tay theo pose; không khóa vai hoặc dính thân áo |
| `class_accessory` | Dây treo/tua thuộc phụ kiện; biểu tượng may trên áo vẫn thuộc outer_top | Một socket đai chuẩn; tháo phụ kiện không xóa nút thắt đai |

Nguồn làm việc cần cấu trúc `itemId / componentId / pose-or-view`, layer/material ownership và sockets rõ ràng. Ảnh full outfit chỉ là kết quả compose để review. Không gộp main_weapon với arm_guard, outer_top với giáp vai hoặc charm với waist_belt cho đủ hình đẹp. Tên slot authoring giữ chuẩn; adapter ánh xạ tên cũ như `outer_tunic`, `lower_garment`, `boots` ở một chỗ, không đổi frozen IDs.

### Thiếu hụt runtime đã xác minh

`TwoDCharacterRuntimeState` hiện giữ `HashSet<string> _equippedSlots` và một `_levelIndex` chung. `TwoDRegisteredOutfit.Equipment` load pack Lv1 theo gender và `ApplyEquipment` chỉ nhận predicate bật/tắt. Đây là proof 10 slot, **chưa có renderer phối itemId khác cấp**. Thêm art Lv10 vào cấu trúc này không tự tạo khả năng mặc chéo.

Cần bổ sung local visual loadout/registry trong đường trang bị hiện có: resolve `(itemId, componentId, actionPose)` sang asset/mesh, preflight profile/skeleton/mọi component rồi đổi toàn bộ visual state trong cùng frame. Thiếu pose hoặc chưa tải đủ thì giữ nguyên visual loadout trước đó và báo lỗi rõ; không hiện nửa bộ, không lấy sprite Lv1 thay âm thầm. Gameplay/server contracts vẫn giữ nguyên; đây chưa phải persistence/economy mới.

Draw order được chọn theo action và component, có phần cơ thể xen giữa áo/quần/giáp. Coverage là vùng do source thiết kế, dùng để che fallback/lớp dưới đúng quy tắc; không dùng mask tùy tiện để chữa contour sai. Chỉ resolver hoặc một cơ chế duy nhất sở hữu sprite của mỗi renderer, tránh hai animation path tranh quyền.

### Tối ưu có số đo

Đo file hiện tại: atlas nam512²/112843byte PNG, nữ512²/141523byte PNG; mỗi atlas là1MiB RGBA8 không mipmap, BC3 262144byte và ASTC6×6 118336byte theo công thức block. Đây chỉ là texture estimate, chưa phải tổng RAM/GPU residency đo trên thiết bị. Code hiện load cả hai pack gender; trước mở rộng cần cache/tải theo tập asset thực sự dùng.

- Body và animation dùng chung; item giữ texture riêng theo component. Hình góc đặc biệt chỉ bổ sung vùng đồ cần thiết, không bổ sung full body. Dung lượng tăng theo tổng component/view độc nhất, không theo tích số tổ hợp mặc đồ.
- Xuất theo pixel hiển thị thật đã đo, giữ chất lượng div4 hiện hành; atlas bắt đầu512–1024 và tăng chỉ khi occupancy/ảnh review chứng minh cần. Không ép mọi pose vào512 rồi làm mờ để đạt byte budget.
- Đóng atlas offline theo nhóm dùng chung/vòng đời tải; không mặc định một atlas cho mỗi cấp hoặc một atlas mới cho mỗi avatar. Cache texture/material/geometry dùng chung, chỉ state/pose là riêng. Không hứa một atlas đồng nghĩa một draw call.
- Trước áp dụng runtime repack, so với prepacked ở Map01A với1/10/30avatar thử nghiệm: tổng texture resident, material/draw call, thời gian CPU/GPU, p95 đổi đồ và allocation. Cache có giới hạn, giải phóng khi hết reference; không repack theo frame hoặc cấp phát texture riêng cho mọi avatar. Các con số actor là kịch bản benchmark đề xuất, chưa phải tải đã đạt.
- Xuất đồng thời báo cáo PNG, imported format, decoded/ước tính GPU, phần shared và riêng theo avatar. Có thiết bị thật mới kết luận hiệu năng mobile; capture tỷ lệ trên macOS không thay thế.

### Batch triển khai và tiêu chí dừng

1. Giữ motion v3 đã chốt; thiết kế toàn bộ đồ Lv1 theo sáu pose. Dùng cấu tạo turnaround gốc; board gần đúng hành động không là chuẩn ngoại hình hoặc tọa độ.
2. Author các layer riêng theo bảng trên, đủ mặt khuất, đường nối và góc cần vẽ mới. Compose source cả bộ và tháo từng món; không dùng ảnh compose làm nguồn cắt item. Đóng lỗi thiết kế theo nhóm trước build.
3. Đưa bộ Lv1 rời vào cùng body/action chuẩn trên Player; kiểm đứng/bốn nhịp chạy/lộn, flip, đổi/tháo giữa động tác, trở về base. Không thay rig motion nguồn để hợp thức hóa áo. Các action khác trong spec vẫn phải hoàn thiện trước tuyên bố wardrobe production.
4. Sau Lv1, author Lv10 trên cùng fit families rồi kiểm phối áo/quần/đai/giáp/giày khác cấp. Ma trận pairwise gồm rỗng/Lv1/Lv10 ở cả10slot, cộng nhóm tương tác ba món áo–đai–quần, tay–hộ uyển–quyền khí, quần–gấu–giày và full-set extremes. Pairwise không chứng minh mọi tổ hợp; giữ explicit regression cho lỗi từng gặp và mở rộng khi thêm family.
5. Chỉ sau source fit + runtime motion/mixed-loadout + budget thực đạt mới nhân tier/class. Registered WIP giữ để reuse vật liệu/pipeline tương thích; không rollback cả bộ. Mỗi batch gom một lần review lỗi, test liên quan, build/capture cần thiết và checkpoint chung.

Thiết kế này bổ sung phần còn thiếu cho contract hiện có; không thay frozen surfaces và không tự gán motion/wardrobe PASS. Thước đo là đồ rời khớp trong hành động và khi phối cấp, không phải số file cắt ra hoặc số test xanh.
