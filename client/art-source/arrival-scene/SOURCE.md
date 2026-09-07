# Nhân vật nhập môn — candidate cảnh Linh Thành

Mẫu đích: nhân vật trẻ tóc đen/navy–ngà trong `04_scenario_001_gate_keeper_storyboard.png` và palette của `01_world_art_direction_board.png`; SCN-001/002, đi/chạy/chạm đá dùng controller hiện hữu. Chưa art final.

`ArrivalScene.blend` là source chỉnh sửa có rig65 xương, tối đa4 weights. Thân/trang phục kế thừa arrival study từ Quaternius CC0, đầu/tóc và texture tái sử dụng Keeper (nguồn/provenance tại `../gate-keeper/`). Phần tóc đỉnh đầu dựng bổ sung, vai/thân được thu gọn trong pose rồi inverse skin, giữ rest rig.

Xuất bằng Blender với `tools/art/export_arrival_scene.py -- --output <staging>`, sau đó nhập bằng `ArrivalOutfitImporter.Import`. Runtime dùng chung `NpcAppearancePreset`, mesh nén Medium,3 material; palette riêng48×8, hai texture Keeper dùng chung. Không tính texture dùng chung thành dung lượng mới cho mỗi nhân vật, không suy ra mọi bộ trang phục dưới100KB.

Source Blender giữ các texture đóng gói để chỉnh sửa; exporter chỉ xuất palette mới. FBX/source không thuộc dependency runtime prefab. Preview Blender không thay evidence animation/camera trong Player thật. Tóc/mặt và mép trang phục vẫn là phần cần art review tiếp.
