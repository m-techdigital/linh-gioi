## Quick Resume

- `OPERATIONAL_GOAL_CURRENT`: hoàn thiện Character Hub 5 tab trên một shared base theo bộ canonical `redesign-v4-five-tabs`; class chỉ truyền profile/data, không tạo layout, topology hoặc renderer riêng.
- Tiềm năng v20b giữ một topology 600×520 có sẵn toàn bộ vòng/đường nối/khung/ô/dấu cộng. Năm overlay chỉ bind icon/tên/value/selection; đã bỏ add-marker rỗng từng node. Linh thú v18d giữ một hero, roster bốn slot, năm stat row, hai skill row và detail/actions dựng đúng một lần. Võ/Kiếm/Pháp/Cơ/Linh chỉ bind data/state vào cùng object tree; Skill và Tiềm năng là hai component riêng.
- Player hiện hành: `build/character-hub-potential-base-guard-player-v20b/LinhGioiOnline.app`.
- Evidence hiện hành: `build/character-hub-potential-base-guard-runtime-v20b/{pc,mobile,tablet}/`, đúng 29 frame mỗi viewport. Có đủ năm tab, 5 frame Kỹ năng, 10 frame Tiềm năng và 5 frame Linh thú theo class; capture nội bộ không dùng chuột/phím OS và không đổi renderer authority.
- Không phát triển class/pose/wardrobe/source art trong task này; không phục hồi renderer cũ hoặc dựng UI theo class.

## Next task

1. Audit đối chiếu tổng thể năm tab với năm canonical approved trên cùng Player v20b; ghi rõ fidelity gap còn lại theo component dùng chung, không căn riêng từng class/viewport. Với Tiềm năng, mọi sửa tiếp theo phải nằm ở asset topology/helper chung; không thêm geometry vào overlay.
2. Nếu có gap sửa an toàn bằng asset/base/shared helper hiện hữu, xử lý theo từng screen rồi capture lại đủ PC/tablet/mobile. Không tạo placeholder hoặc art ngẫu nhiên để lấp thiếu asset.
3. Sau audit nội bộ, owner review bộ evidence hiện hành; feedback visual chỉ được sửa ở shared component/data contract tương ứng.

## Current status

`CONTINUE`: Tiềm năng đã khóa một base duy nhất và overlay chỉ còn data/tương tác; Kỹ năng dùng graph 4–3–2 chung và Linh thú không còn tràn/chồng. Toàn goal Character Hub vẫn chưa được claim hoàn tất cho tới khi audit fidelity đủ năm tab và owner review.

Evidence:
`build/character-hub-potential-base-guard-runtime-v20b/{pc,mobile,tablet}/`

## Checkpoint detail

- Asset portrait 192×192 được crop xác định từ hero Linh thú có provenance; hero wide vẫn dùng ở main preview. Không cắt canonical và không sinh art class mới.
- Validator khóa topology Kỹ năng 4–3–2, năm helper Linh thú, topology Tiềm năng ổn định và cấm rebuild theo class. Evidence theo class chỉ bind dữ liệu trên cùng layout.
- Player build `Succeeded`, `errors=0`, `warnings=50` (CS0618 hiện hữu); Python/shared/capture tests `36/36`; full Unity EditMode `290 total / 289 passed / 0 failed / 1 ignored`; frozen/no-3D/no-source gate pass.
- Chưa claim owner visual acceptance, art/wardrobe đủ năm class hoặc interaction progression đã mở khóa.
