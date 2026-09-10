# Checklist tách asset trước Unity v1.0

Hiện trạng 23 ảnh Võ: REFERENCE_ONLY, chưa có pack item sạch; không production art, không runtime asset. Hoàn thành checklist cho từng file, không ký đạt theo dòng chữ “không dính da thịt” in trong ảnh.

- [ ] Có nguồn, SHA-256, class/gender/level/slot, người review và tình trạng DRAFT/REFERENCE_ONLY/clean candidate rõ ràng.
- [ ] Tên/folder đúng chuẩn; một level/slot duy nhất, đủ đôi đối với giày/quyền khí nếu bản preview quy định đôi.
- [ ] Không dính da thịt: zoom cổ/nách, lòng găng, ngón tay, cổ chân; không đầu/mặt/torso hoặc mannequin ẩn trong đồ.
- [ ] Tóc chỉ ở head_hair, không mặt/cổ; có kế hoạch front/back riêng trên cùng base.
- [ ] Không gộp bốn cặp cấm; tháo thử từng layer không kéo theo món khác hoặc để lại phần lặp.
- [ ] Vạt, ngọc, dây có một chủ sở hữu rõ ràng; không gộp class_accessory vào đai.
- [ ] Không chữ/logo/khung board, nền trắng hoặc nền caro vẽ giả trong sprite candidate.
- [ ] Kiểm alpha thật trên nền sáng/tối, không viền trắng, fringe hoặc rỗng nhầm vùng vật liệu.
- [ ] Có bản sạch không aura; VFX riêng, hình dáng không bị hiệu ứng che lấp.
- [ ] Camera side-view/góc secondary, scale, pivot, anchor và mapping vào hai base thống nhất; front-view board chưa đáp ứng gate này.
- [ ] Bản cutout có phần trái/phải và overlap vùng khớp phù hợp; không mirror mù khi áo/vũ khí bất đối xứng.
- [ ] Đối chiếu lắp đủ bộ và trộn mốc lv001/lv050/lv100; idle, walk, interact, ngồi và các pose vận động trong task animation riêng không xuyên/đứt lớp.
- [ ] Đo pixel hiển thị trước chọn kích thước texture; ghi sRGB/alpha, filter, compression, mipmap, atlas padding theo use case. Không coi dung lượng PNG là RAM texture.
- [ ] Task ingest giải quyết gate no-source-images của branch; không tự bỏ gate hoặc nhập board.
- [ ] Unity import/smoke thật và ảnh ở camera chơi thật được xem trước claim runtime/visual PASS.
- [ ] Trạng thái vẫn là `candidate` sau crop; chỉ `approved` khi skeletonVersion/bodyProfile/bone/anchor/coverage/occlusion/motion đều pass. Lỗi contour hoặc fit chuyển `redraw-required`, không bù bằng scale/offset runtime.
- [ ] Mixed-level loadout dùng item ID độc lập theo 10 slot; kiểm ít nhất một bộ phối Lv1/Lv10/Lv20/Lv30 trên cùng base sau khi nhân vật đủ unlock level.

Checker tự động chỉ kiểm text, tên và độ đầy đủ. Không phát hiện da bằng màu: vải ngà có thể giống da và da bị che vẫn là body. Không suy ra asset sạch từ alpha channel hoặc đuôi PNG.
