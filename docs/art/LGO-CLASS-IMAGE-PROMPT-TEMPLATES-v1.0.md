# Prompt template module 2D v1.0 — chạy trước cho Võ

Trạng thái: brief thiết kế, chưa sinh ảnh mới. Tham chiếu quyết định trong spec Võ; truyền ảnh nguồn đã chọn cùng prompt. Không yêu cầu một lần sinh đủ 220 item: làm mẫu tách lớp lv001 và lv050 nam/nữ trước, review rồi mới nhân các mốc. Board hoàn chỉnh phải dàn từ thiết kế item đã kiểm, không dùng output 10×11 đầu tiên như bằng chứng sạch.

## Item rời

“Thiết kế lại một item {slot_id}, class Võ vo, {gender}, {level}, HD 2D anime illustrated, fantasy Á Đông hiện đại. Giữ identity từ ảnh reference đã chọn: đen/đỏ/vàng đồng/ngà, quyền lực cơ học nắm đấm, không xanh băng. Phạm vi item: {slot_ownership_from_equipment_slots}. Chỉ item rời; không dính da thịt, body, mannequin, mặt, cổ, tay, ngón tay, chân, torso. Nếu head_hair chỉ tóc/băng trán rời không da đầu; slot khác không tóc. Găng rỗng không ngón tay, ống bảo hộ chỉ cẳng tay và hai đầu rỗng. Không áo trong dính áo ngoài, không đai dính quần, không giáp vai dính áo ngoài. Không VFX/aura, không chữ, logo, shadow nền. Nền trong suốt thật cho item candidate. Góc {view}, scale và hướng sáng theo base tham chiếu; lỗ hở giữ alpha. Một item hoặc đôi item cùng slot, không món khác. DRAFT clean candidate cần review, không production.”

Xuất preview theo `assets/reference/classes/{class_id}/{gender}/equipment/{slot_id}/{class_id}_{gender}_{slot_id}_{level}.png` khi task ingest cho phép. Naming không biến ảnh thành runtime asset.

## Equipment grid

“Bảng reference Võ {gender}, đúng 10 hàng × 11 cột. Hàng theo thứ tự main_weapon, head_hair, inner_top, outer_top, lower_body, waist_belt, arm_guard, footwear, shoulder_chest_guard, class_accessory. Cột lv001, lv010, lv020, lv030, lv040, lv050, lv060, lv070, lv080, lv090, lv100. Mỗi hàng một slot, mỗi cột một level. Mỗi ô chỉ detached item; không chân dung/body/mannequin. Khung đơn giản, không banner trang trí chiếm ô. Giữ khoảng cách và góc item nhất quán. Không nhầm main_weapon với arm_guard; inner_top với outer_top; lower_body với waist_belt; outer_top với shoulder_chest_guard. REFERENCE_ONLY.”

## Module map

“Board DRAFT Võ nam/nữ, bên trái mannequin có trang phục nền kín đáo và callout vị trí 10 slot đúng ID; bên phải 10 item tách rời tuyệt đối không body. Chỉ mô tả quan hệ tháo/lắp, không lấy số slot làm sorting order. Phân biệt vị trí cổ tay main_weapon và cẳng tay arm_guard; lớp áo vải và giáp cứng; vạt áo và tua đai. Không ghi chuẩn production.”

## Full outfit progression

“11 mốc cùng base {gender}, cùng pose/scale/camera: lv001 đến lv100 theo spec. Khối gọn, silhouette rõ cho side-scrolling, Võ đen–đỏ–vàng–ngà. Nhân vật toàn thân chỉ trong board outfit, review không VFX trước. Đây là visual review only, không dùng để trích xuất item. Không tự đổi cơ thể hay tuổi theo level.”

## Skill/VFX progression

“Board nghiên cứu thị giác Võ, cam/vàng ấm, impact ngắn, bụi và shockwave; 11 mốc tăng độ phức tạp có kiểm soát. Có frame không hiệu ứng làm đối chứng silhouette. Không đặt damage, buff, cooldown, tên/unlock skill như thiết kế gameplay đã duyệt. REFERENCE_ONLY, không sprite sheet runtime.”

Bốn class kiem/phap/co/linh chưa áp dụng trong batch. Khi được giao phải thay identity và reference theo class, không chỉ recolor quyền khí Võ thành vũ khí class khác.
