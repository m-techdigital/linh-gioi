# Map01A Chọn nhân vật — canonical screen contract v1.0

Ngày khóa design: 2026-09-14  
Trạng thái: **CANONICAL_DESIGN_LOCKED / LAYOUT_LOCKED**

## Nguồn quyết định duy nhất

Canonical design duy nhất:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v6-character-select/01-character-select-CANONICAL.png`

- Canvas: `1672 × 941`.
- SHA-256: `afdc76db54df285b02bd641b3588faa3e7a757d3cfc4eed56ca2fbd266e8d108`.
- `04-chon-nhan-vat-nam-class-kiem-demo.png` là design lịch sử vì trộn chọn class vào chọn nhân vật.
- Runtime screenshot là evidence đối chiếu, không phải design thứ hai.

## Scenario và phạm vi dữ liệu

Screen xuất hiện sau đăng nhập thành công và trước Map01A playable HUD. Mục đích là chọn hồ sơ nhân vật đã lưu, đọc tóm tắt class/cấp/máy chủ, tạo nhân vật ở slot trống và vào game.

Runtime hiện chỉ có một hồ sơ thật: `LụcThiên`. Hai slot còn lại phải hiện `Chưa có nhân vật`; không tạo hồ sơ, level, class hoặc progression giả. Art nhân vật lấy từ preview sprite hiện hành của Player. Batch UI này không sửa class, pose, wardrobe, source, camera, scale hoặc dữ liệu gameplay.

## Hierarchy và layout canonical

1. Nền Đông Lâm phủ canvas; logo và motto ở góc trái trên.
2. Stage nhân vật chiếm khoảng `64%` bề ngang, giữ full-body và ground line hiện hành.
3. Panel tài khoản bên phải chiếm khoảng `32%`, không phải modal debug ở giữa.
4. Panel gồm title, một selected character card, hai empty slot, tạo nhân vật, tóm tắt nhân vật, action row và server row.
5. `Quay lại` nằm trái dưới, không lẫn vào action chọn nhân vật.

| Vùng | Rect/tỷ lệ mục tiêu | Quy tắc |
|---|---:|---|
| Brand | x `44..430`, y `35..155` | không chạm stage/panel |
| Character stage | x `80..1085`, y `160..870` | sprite ScaleToFit; không đổi camera/base/scale |
| Account panel | x `1085..1635`, y `90..862` | một column cố định, scroll chỉ khi thật sự thiếu chiều cao |
| Selected profile | trong panel, cao `104..116` | avatar/icon thật, tên, Lv, class, server |
| Empty profiles | hai row, cao `70..82` | trạng thái trung tính; không giả nhân vật |
| Character detail | dưới list | class/title/cấp lấy state hiện có |
| Actions | đáy panel | `Chỉnh sửa`, `Xóa`, `Vào game`; CTA primary rõ |
| Server row | đáy màn | `S1 · Đông Lâm`, `Đổi máy chủ` |

Geometry được scale đồng nhất theo viewport. PC/mobile landscape/tablet giữ cùng composition trái/phải; không wrap panel xuống dưới stage.

## State và interaction

| State | Hiển thị | Interaction |
|---|---|---|
| Selected profile | `LụcThiên`, level/state hiện hành | chọn lại không đổi class/source |
| Empty slot | `Chưa có nhân vật` | route tới màn Tạo nhân vật riêng khi có contract; hiện tại phản hồi read-only |
| Tạo nhân vật | secondary action | không mở class selector cũ; phản hồi chưa khả dụng |
| Chỉnh sửa/Xóa | secondary/destructive | khóa hoặc phản hồi trung thực khi chưa có account backend |
| Vào game | primary | đóng character-select và vào Map01A hiện hành |
| Đổi máy chủ | secondary | route tới screen Chọn máy chủ riêng khi screen đó được design/contract |
| Quay lại | navigation | đóng character-select và mở lại Entry/Login |

Tất cả control nhận click phải đổi state hoặc hiển thị feedback. Không có dead click. Không callback nào được gọi `CycleSourcePoseClass()`.

## Viewport gate

| Profile | Evidence size | Bắt buộc |
|---|---:|---|
| PC | `1600 × 900` | full stage, panel, actions, server row |
| Mobile landscape | `1600 × 720` | cùng composition; không stack/wrap/cắt CTA |
| Tablet | `1024 × 768` | scale đồng nhất; panel vẫn ở phải |

## Base-first và asset budget

- Dựng hierarchy/state ở `CongDongLamArrivalHud.CharacterSelect.cs`; style tái sử dụng nằm trong `CongDongLamArrivalHud.Skin.cs`.
- Reuse shared modal/card/button/label/icon frame; không dựng hệ button/card riêng từng control.
- Character preview dùng source runtime hiện hành; empty slot dùng crest provenance-backed từ HUD atlas.
- HUD atlas `512 × 512`, 16 cell `96 px`, khoảng `85 KB`; icon Character Select hiển thị tối đa `48 px`, đủ density cho Player. Không tạo atlas trùng lặp.
- Không dùng emoji, ký tự icon tạm, ảnh sinh ngẫu nhiên hoặc asset lớn hơn pixel display không có lý do.

## Gate hoàn thành

- Test khóa hierarchy, một profile thật + hai empty slot, detail phải, shared style và không còn legacy class cards/callback.
- Build Player thành công.
- Capture và xem trực tiếp cả ba profile PC/mobile/tablet.
- Player `build/map01a-character-select-canonical-player-v6/LinhGioiOnline.app` build thành công, `errors=0`.
- Evidence `build/map01a-character-select-canonical-runtime-v6/{pc,mobile,tablet}/character-select.png` đã xem trực tiếp ở `1600×900`, `1600×720`, `1024×768`: cùng composition, không cắt/chồng/wrap.
- Stage tái sử dụng `far-background.png` của Map01A (`1024×576`, khoảng `124 KB`) thay vì nhân đôi asset; lớp nền sạch che world actors, nên chỉ còn đúng một selected-character preview.
- Full `TwoDCharacterRuntimeStateTests` đạt `25/25`; selected profile không đổi class/source, Enter/Back đổi screen đúng. Shared-skin guard đạt.
- Kết luận: `LAYOUT_LOCKED`. Art/portrait nhân vật vẫn kế thừa source hiện hành và không phải class-art approval.
