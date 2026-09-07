# Bộ ghép kiến trúc Linh Thành — authoring draft

Mẫu đích đã xem: `docs/reference-art/linh-gioi-concept-board.png`, `linh-gioi-key-art.png`, `linh-gioi-world-event-ui.png`, cùng `docs/reference-art/v0.16.5/lgo-world-hub-2d5-v0165.png` và `docs/reference-art/v0.20.0/lgo-environment-prop-sheet-v0200.png`. Lấy nhịp mái cong, phố quanh sân trống, đèn ấm, gỗ/đá, cây và dấu nhận diện cổng. Giữ Nhân Gian sáng theo North Star; không lấy giao diện/nhãn hệ thống, nền đêm tím hay mở shop/PvP/crafting từ ảnh. Không ảnh tham khảo nào được đưa vào mesh/material.

`kit-layouts.json` là dữ liệu authoring chạy được bằng `build_preview.py`, **chưa có runtime consumer**. Ba layout khác nhau được dựng từ cùng chín collection mesh và chín material; instance không nhân bản mesh trong Blender. Preview dùng hình học riêng phỏng theo family hiện có, không phải export các mesh Unity hiện hành. Các kích thước mới là đề xuất bố cục, không đổi collider hoặc scale gameplay đã khóa.

| Module | Hiện có / thiết kế mới | Vai trò và mốc ghép |
|---|---|---|
| Nhà 4×4 m | Family `CityArchitectureVisuals.House/Roof`; chuẩn hóa draft | Chân giữa; cửa hướng −Z; nhịp phố 6–8 m, cho mái nhô mỗi bên |
| Cổng rộng 12 m | Family `Landmarks`; bản ghép draft | Chân giữa; lối đi giữa ≥6 m; crest và mái là silhouette chính |
| Đèn cao 3 m | Family `CreateStreetLanterns` | Chân trụ; đặt ngoài luồng đi, nhịp 6–8 m |
| Ghế rộng 2 m | Family ghế sân nghỉ hiện có | Chân giữa; mặt ngồi hướng −Z; khoảng tiếp cận trước ≥1 m |
| Cây hoa | Family `Tree`; canopy preview đơn giản | Chân gốc; tán cho phép ngoài footprint; không chặn cổng/NPC |
| Sạp 3×2 m | Thiết kế mới, chưa runtime | Trang trí phố; ghép mái/gỗ/đèn chung, không mở economy |
| Café 6×4 m + sân bàn ghế | Thiết kế mới, chưa runtime | Mặt mở, quầy, mái bạt sọc, biển hình tách, ban công; terrace nhô4 m phía trước |
| Nhà phố cao tầng | Thiết kế mới, chưa runtime | Ba tầng cửa dài, nhịp slab và một mái cong trên cùng; xen nhà thấp để tránh cả phố giống đền |
| Hoa văn sân Ø8 m | Thiết kế mới, chưa runtime | Nằm phẳng mặt đất; chỉ mốc bố cục, không altar/reward |

Đơn vị mét; JSON dùng trục Unity Y-up, script đổi sang Blender Z-up. Lưới phụ0.5 m, lưới chính2 m, xoay90°. Pivot ở chân giữa. Socket `left/right/front/back/top` ghi trong JSON là mốc layout theo hộp danh nghĩa; phần mái/tán nhô phải được tính riêng khi duyệt clearance. Không tự nối roof qua socket top nếu chưa kiểm tra độ cao eave. Các module dùng chín family: đá, ngà, gỗ, mái navy, đồng cũ, vải teal, đèn hổ phách, hoa, lá. Không dựng texture riêng cho mỗi nhà.

Ba bố cục: **gate** dẫn đường vào SCN-001 với skyline cao/thấp xen kẽ; **plaza** giữ tâm trống, café và chỗ nghỉ quanh rìa, vùng `future_world_event_anchor` chỉ giữ chỗ trong JSON; **market** có quầy café, bàn ghế ngoài trời và sạp trang trí. Hướng cửa các module quay vào đường; terrace phải được tính khi kiểm tra khoảng trống. Kịch bản/player/NPC/Đá Luyện vẫn thuộc runtime hiện có. Preview không chứa hệ thống mới hoặc giả lập nghiệm thu gameplay.

Đối chiếu `docs/02-GDD.md`: kit này dành riêng **`map.city.linh_thanh`**, social hub và địa điểm world event dài hạn; không dùng nguyên xi cho `map.field.mist_forest` (field cận chiến), `map.field.spirit_river` (di chuyển/hazard/tài nguyên) hoặc `map.dungeon.shadow_gate` (dungeon4 người). Hình concept gốc có café, nhà phố hiện đại xen kiến trúc Á Đông và năm hướng Võ/Kiếm/Pháp/Cơ/Linh; thiết kế thành phố phải chứa được sự đa dạng đó, không biến tất cả thành sân/đền tu luyện. Đây là định hướng hình ảnh; quyền mở từng class/hệ thống vẫn theo GDD/roadmap, không phát sinh từ kit. Không chép VIP, currency, HUD, event timer hay economy trong board vào runtime.

Hướng học từ sản phẩm thật: Epic mô tả City Sample dùng các bộ modular asset, luật theo kiểu nhà, điểm bố trí và instance để dựng thành phố lớn trong [City Sample](https://dev.epicgames.com/documentation/en-us/unreal-engine/city-sample-project-unreal-engine-demonstration). Áp dụng ở đây là **asset family + dữ liệu bố trí + instance**, không sao chép yêu cầu Nanite/Lumen hay suy ra hiệu năng Unity/mobile từ demo Unreal. Bước tích hợp phù hợp: adapter đọc catalog, tái dùng mesh/material theo family và chia batch theo cụm phố; giữ collision/navigation tách khỏi phần trang trí. Catalog hiện tại đủ để review hình khối/bố cục; chưa phải importer hoặc thiết kế final đã duyệt.

Chạy từ repo:

```sh
build/toolchains/blender/Blender.app/Contents/MacOS/Blender --background --python client/art-source/linh-thanh-kit/build_preview.py -- --output build/asset-staging/linh-thanh-kit
```

Output: `gate.png`, `plaza.png`, `market.png`, `LinhThanhKit.blend`, `preview-receipt.json`. Render offline để xem khả năng ghép; không phải runtime PASS, draw-call/FPS/memory hay chất lượng asset final. Cần tích hợp adapter và đo Player thật trước khi công bố các số đó.
