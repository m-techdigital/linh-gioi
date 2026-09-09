# Linh Thành Onboarding Design — 2D Branch

Branch `feature/2d` chuyển onboarding về hướng 2D-first. Mục tiêu là dựng lát cắt có thể chơi được cho SCN-001/002: Cổng Linh Thành, NPC gác cổng, chuyển tới Bia Luyện Khí, feedback thao tác đầu tiên.

## Trạng thái đã duyệt cho nhánh này

- Dừng hướng dựng nhân vật/cảnh bằng pipeline 3 chiều.
- Giữ North Star Social Action MMORPG: Linh Thành là hub xã hội, Âm Giới Xâm Lăng là hướng dài hạn.
- Làm 2D runtime trước: rõ flow, rõ tương tác, dễ capture, dễ thay asset.

## SCN-001 — Cổng Linh Thành

Player-visible target:

- Nền cổng Linh Thành dạng 2D nhiều lớp, màu xanh đêm/ngọc/kim tuyến theo visual lock.
- Nhân vật người chơi đứng rõ trên sân, có pivot chân ổn định.
- NPC gác cổng có nhãn, vùng focus và thoại ngắn.
- HUD hướng dẫn: “Di chuyển tới NPC”, “Nói chuyện”, “Vào Linh Thành”.

Interaction target:

- Di chuyển ngang/dọc trong vùng an toàn.
- Khi vào vùng NPC, hiện action.
- Bấm action mở thoại.
- Chọn tiếp tục để chuyển sang SCN-002.

## SCN-002 — Bia Luyện Khí

Player-visible target:

- Sân luyện 2D đơn giản, bia đá nổi bật, glow nhẹ khi focus.
- HUD hướng dẫn thao tác luyện khí đầu tiên.
- Feedback khi kích hoạt: rung/glow/text trạng thái, không mở hệ thống thưởng hay combat mới nếu chưa tới gate.

Interaction target:

- Player di chuyển tới bia.
- Focus bia hiện action.
- Kích hoạt bia tạo feedback rõ và log evidence.

## Quy tắc asset 2D

- Sprite phải có tên/pivot/scale nhất quán để thay thế nhanh.
- Character, NPC, prop và UI icon tách riêng.
- Trang phục/phụ kiện sau này sẽ đi theo sprite-layer hoặc paper-doll 2D, chưa triển khai trong batch gỡ 3D.
- Nếu asset cuối chưa có, dùng placeholder 2D có silhouette đúng thay vì quay lại pipeline 3 chiều.

## Evidence cần có

- Unity batch compile/test thật.
- Runtime capture cho desktop ít nhất khi có thay đổi visual/player-visible.
- Ảnh phải được xem trước khi claim visual pass.
