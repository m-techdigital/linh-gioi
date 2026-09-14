## Quick Resume

- `OPERATIONAL_GOAL_CURRENT`: hoàn thiện Character Hub 5 tab trên một shared base theo bộ canonical `redesign-v4-five-tabs`; class chỉ truyền profile/data, không tạo layout, topology hoặc renderer riêng.
- Tiềm năng v17 giữ một topology 600×520. Linh thú v18d giữ một hero, roster bốn slot, năm stat row, hai skill row và detail/actions dựng đúng một lần; Võ/Kiếm/Pháp/Cơ/Linh chỉ bind text/icon/state vào cùng object tree.
- Player hiện hành: `build/character-hub-spirit-template-player-v18d/LinhGioiOnline.app`.
- Evidence hiện hành: `build/character-hub-spirit-template-runtime-v18d/{pc,mobile,tablet}/`, đúng 24 frame mỗi viewport. Có đủ năm tab, 10 frame Tiềm năng theo class và 5 frame Linh thú theo class; capture nội bộ không dùng chuột/phím OS và không đổi renderer authority.
- Không phát triển class/pose/wardrobe/source art trong task này; không phục hồi renderer cũ hoặc dựng UI theo class.

## Next task

1. Audit đối chiếu tổng thể năm tab với năm canonical approved trên cùng Player v18d; ghi rõ fidelity gap còn lại theo component dùng chung, không căn riêng từng class/viewport.
2. Nếu có gap sửa an toàn bằng asset/base/shared helper hiện hữu, xử lý theo từng screen rồi capture lại đủ PC/tablet/mobile. Không tạo placeholder hoặc art ngẫu nhiên để lấp thiếu asset.
3. Sau audit nội bộ, owner review bộ evidence hiện hành; feedback visual chỉ được sửa ở shared component/data contract tương ứng.

## Current status

`CONTINUE`: Linh thú đã qua gate nội bộ và không còn tràn/chồng ở ba viewport; toàn goal Character Hub vẫn chưa được claim hoàn tất cho tới khi audit fidelity đủ năm tab và owner review.

Evidence:
`build/character-hub-spirit-template-runtime-v18d/{pc,mobile,tablet}/{character-info,bag,skills-default,skills,potential-default,potential,spirit-pet}.png`

## Checkpoint detail

- Asset portrait 192×192 được crop xác định từ hero Linh thú có provenance; hero wide vẫn dùng ở main preview. Không cắt canonical và không sinh art class mới.
- Validator khóa năm helper Linh thú dùng chung, topology ổn định và cấm rebuild theo class. Class-specific evidence chỉ đổi nhãn `Đồng hành Võ/Kiếm/Pháp/Cơ/Linh` trên cùng layout.
- Player build `Succeeded`, `errors=0`, `warnings=50`; Python/shared tests `35/35`; full Unity EditMode `289 total / 288 passed / 0 failed / 1 ignored`; no-3D/no-source/frozen diff pass.
- Chưa claim owner visual acceptance, art/wardrobe đủ năm class hoặc interaction progression đã mở khóa.
