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

Nguồn tham khảo:
- https://docs.unity3d.com/Packages/com.unity.2d.animation@13.0/manual/SLAsset.html
- https://docs.unity3d.com/Packages/com.unity.2d.animation@13.0/manual/SpriteSwapIntro.html
- https://en.esotericsoftware.com/spine-unity-mix-and-match
