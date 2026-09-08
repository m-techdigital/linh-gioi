# Nhân vật nhập môn — candidate cảnh Linh Thành

Mẫu đích: nhân vật trẻ tóc đen/navy–ngà trong `04_scenario_001_gate_keeper_storyboard.png` và palette của `01_world_art_direction_board.png`; SCN-001/002, đi/chạy/chạm đá dùng controller hiện hữu. Chưa art final. Tóc đối chiếu dáng buộc cao và mái rủ ở `docs/reference-ui/lgo-arrival-outfit-turnaround-draft-v1.jpg`; không lấy bộ áo trắng của draft đó để thay trang phục navy/ngà đang theo storyboard.

Lỗi macro đang xử lý: mặt trước cắt nhọn và thiếu da bên má; cổ/gáy không liền; mép cổ áo nhô; tóc đọc thành mũ trơn và búi hình trứng. Dùng đầu kín + UV mặt riêng, mép cổ liên tục và tóc có mái/đuôi cong. So silhouette và góc nghiêng với mẫu trước khi build; build qua không thay kiểm tra fidelity.

`ArrivalScene.blend` là source chỉnh sửa có rig 65 xương, tối đa 4 weights. Trang phục kế thừa arrival study từ Quaternius CC0 và Keeper (nguồn/provenance tại `../gate-keeper/`). `tools/art/build_modular_player.py` dựng player từ Keeper; `tools/art/rebuild_player_head.py` thay đầu liền khối và chỉnh mép cổ áo trong source player hiện tại. Hai đường dùng cùng hàm dựng đầu và `rebuild_player_hair.py`, giữ rest rig chuẩn. `finish_player_neck.py` nối lớp lót trực tiếp vào 44 cạnh miệng cổ áo (88 tam giác), đồng thời skin phần đuôi tóc từ Head qua neck_01 đến spine_03; crown/mái giữ Head. Lớp lót có tag tác giả để tái tạo không nhân đôi.

Bảy phần tác giả: Head, Hair, UpperBody, LowerBody, Gloves, Boots, Shoulders. Baker chung ghép thành một SkinnedMeshRenderer, hai material; body dùng chung atlas Keeper 4096×2048, đầu dùng riêng `ArrivalFace.png` 1774×887. Texture mặt được tạo bằng imagegen ngày 2026-09-08 từ KeeperReconstructionFace để có da liên tục quanh má/cổ, thay atlas cũ có tóc/cổ áo lẫn vào UV. Đây là albedo phủ mesh đầu 3D, không phải ảnh nhân vật đặt vào scene. Prompt tại `face-prompt.txt`; không coi mặt mới là bản sao pixel của reference.

Xuất bằng Blender với `tools/art/export_arrival_scene.py -- --output <staging>`, rồi nhập bằng `ArrivalOutfitImporter.Import`. Export chỉ phát sinh FBX và texture mặt player; không nhân đôi body atlas. Unity dùng Avatar chuẩn, giữ mesh đầy đủ và chất lượng texture PC; chưa tối ưu mobile hoặc cam kết dung lượng mỗi bộ dưới 100 KB. FBX/module Editor không thuộc dependency runtime prefab.

Blender đóng gói texture để chỉnh sửa. Preview Blender không thay evidence animation/camera trong Player thật; trạng thái kiểm tra gần nhất xem `docs/execution/NEXT-ACTION.md`. Tóc/chất liệu/trang phục vẫn cần art review, không đánh dấu final chỉ vì build qua.

Ủng2026-09-08: `repair_boot_projection.py` tái ánh xạ mặt bên/sau từ dải giữa mặt trước sạch, giữ atlas/geometry/weights.8 ảnh PC tại `build/visual-evidence/boots-center-desktop/review.json` đã xem; mảng nền đen giảm nhưng còn kéo ngang màu, chưa final. Bước tiếp: bề mặt ủng riêng rồi bake vào atlas, không tiếp thử ngưỡng UV.
