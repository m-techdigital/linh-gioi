# Map01A Character Select screen realignment plan

**Mục tiêu:** thay modal chọn class kỹ thuật bằng màn chọn hồ sơ nhân vật bám canonical đã khóa, dùng đúng một hồ sơ runtime thật và không đụng class/pose/wardrobe/source.

**Nguồn:** `docs/design/LGO-MAP01A-CHARACTER-SELECT-SCREEN-CONTRACT-v1.0.md`.

## Batch 1 — khóa behavior bằng test

- Sửa `TwoDCharacterRuntimeStateTests` để yêu cầu stage trái, panel phải, một profile `LụcThiên`, hai empty slot, action/server/footer và không còn năm legacy class card.
- Chứng minh chọn profile không đổi `ActiveEquipmentClassId`; Vào game đóng overlay; Quay lại mở Entry.

## Batch 2 — dựng layout bằng shared base

- Thay hierarchy trong `CongDongLamArrivalHud.CharacterSelect.cs`.
- Thêm helper style Character Select vào `CongDongLamArrivalHud.Skin.cs`; các control cùng loại dùng chung helper/class.
- Dùng `GetVoAvatarThumbnailSprite()` và HUD icon atlas hiện hành. Không thêm asset giả.

## Batch 3 — Player evidence và gate

- Build Player sau C# change.
- Capture `character-select.png` tại PC `1600×900`, mobile landscape `1600×720`, tablet `1024×768` bằng capture flag nội bộ.
- Xem trực tiếp cả ba ảnh; sửa sai lệch cấu trúc trước khi chạy full edit-mode suite và validators.
- Cập nhật `PROJECT-STATE.md`, `NEXT-ACTION.md`, review catalog, commit/push một checkpoint coherent.

## Stop/gate

Không mở màn Chọn máy chủ, Đăng ký hoặc Quên mật khẩu trước khi Character Select đạt visual gate. Không sửa frozen surfaces và không resume class/pose/wardrobe/source.
