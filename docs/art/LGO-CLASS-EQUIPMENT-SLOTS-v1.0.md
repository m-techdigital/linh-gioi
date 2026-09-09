# Equipment slots v1.0 — áp dụng Võ

Giữ nguyên 10 ID cho nam/nữ và mọi level. Không tăng số slot để cứu một board sai.

| # | ID | Label | Phạm vi sở hữu của Võ |
| --- | --- | --- | --- |
| 1 | `main_weapon` | Vũ khí chính / Class weapon | Quyền khí bao mu bàn tay và khớp đấm; cổ tay là mép nối, không kéo lên cẳng tay. |
| 2 | `head_hair` | Đầu / Tóc / Mũ / Băng trán | Tóc thay thế, băng trán, trâm; không mặt, tai, cổ hoặc da đầu. Tóc chỉ được nằm trong slot này. |
| 3 | `inner_top` | Áo trong | Áo sát người bằng vải, không da, giáp, đai hoặc áo khoác. |
| 4 | `outer_top` | Áo ngoài / Chiến y / Robe / Jacket | Áo khoác/vạt gắn từ thân áo; không áo trong, giáp vai hay đai. |
| 5 | `lower_body` | Quần / Váy / Hạ y | Quần/váy và đường may cạp cố hữu; không đai tháo rời hoặc tua treo từ đai. |
| 6 | `waist_belt` | Đai lưng | Đai, khóa, tua vải và ngọc gắn cố định vào đai; không quần/váy. |
| 7 | `arm_guard` | Bảo hộ tay / Cẳng tay / Găng phụ trợ | Riêng Võ: ống bảo hộ từ dưới khuỷu tới trước cổ tay; hai đầu rỗng, không bàn tay/ngón tay hoặc khối đấm. |
| 8 | `footwear` | Giày / Ủng | Giày chiến đế thấp, mũi kín, cổ rỗng; không bàn chân, cẳng chân hoặc tất của base. |
| 9 | `shoulder_chest_guard` | Giáp vai / Ngực nhẹ | Mảnh giáp vai/ngực rời cùng dây cố định; không áo vải hoàn chỉnh hoặc tay áo. |
| 10 | `class_accessory` | Phụ kiện / Linh ấn / Trang sức / Class emblem | Một món charm/huy chương biểu tượng nắm đấm; không chép lại ngọc đã thuộc đai, không aura. |

Quyền khí và bảo hộ nối ở cổ tay với vùng overlap nhỏ có chủ đích trong bản cutout sau này; không vẽ hai lớp găng trùng lên bàn tay. Cả hai cùng tháo/lắp độc lập trên base. Ngọc đai là một phần đai nếu cố định; cùng ngọc đó không xuất hiện lại trong accessory.

Không gộp `main_weapon` với `arm_guard`.
Không gộp `inner_top` với `outer_top`.
Không gộp `lower_body` với `waist_belt`.
Không gộp `outer_top` với `shoulder_chest_guard`.

Trang bị không dính da thịt. Lỗ cổ, nách, cổ tay, cổ giày để rỗng; mặt trong vải/kim loại có thể vẽ, không tô màu da thay khoảng rỗng. Base tóc mặc định chỉ được ẩn khi equip tóc thay thế rồi phục hồi khi tháo; đây là tiêu chí thiết kế, chưa thay behavior runtime.
