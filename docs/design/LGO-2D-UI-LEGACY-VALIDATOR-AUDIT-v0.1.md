# Audit validator UI legacy M4/V3B

Ngày: 2026-09-13. Phân loại: **LEGACY_STALE_NOT_CURRENT_GATE**.

`client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs` không còn tồn tại trong branch hiện tại. Audit bằng `tools/report_lgo_legacy_ui_validator_refs.py` phát hiện 73 validator lịch sử trong `tools/validate_lgo_*.py` vẫn nhắc file này. Các validator đó được giữ như provenance của UI M4/V3B cũ, không phải gate hiện hành cho Map01A, hành trang, đăng nhập hoặc chọn nhân vật 2D mới.

Quy tắc áp dụng:

- Không phục hồi `M4PlayableClientController.cs`, login/sảnh V3B, registry V2/V3B, hoặc layout 3D chỉ để làm xanh validator cũ.
- UI hiện hành dùng `CongDongLamArrivalHud` và runtime Map01A đang chạy; evidence chức năng hành trang/detail phải dựa trên Player capture mới, Unity EditMode mới và tài liệu design 2D hiện hành.
- Login/chọn nhân vật tiếp theo phải thiết kế theo bộ reference owner v2 và `docs/design/LGO-MAP01A-PLAYABLE-UI-v0.1.md`, tách rõ `Đăng nhập`/`Bắt đầu`, dùng API dev hiện có nếu nối được, không tự mở production auth/contract.
- Nếu cần thay validator cũ, làm theo từng màn đang triển khai: tạo validator mới trỏ đúng runtime/UI hiện hành và evidence mới; không sửa hàng loạt 73 file trong một batch chỉ để đổi tên symbol.

Lệnh audit:

```bash
python3.12 tools/report_lgo_legacy_ui_validator_refs.py
```

Kết quả hiện tại: target `M4PlayableClientController.cs` missing, 73 legacy validator refs, owner note yêu cầu không dùng chúng làm gate cho UI 2D mới.
