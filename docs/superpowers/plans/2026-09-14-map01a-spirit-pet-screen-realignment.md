# Map01A Linh thú — implementation plan

Canonical screen duy nhất: `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/05-linh-thu-five-tab-APPROVED.png`. Baseline Player: `build/map01a-potential-screen-runtime-v1/{pc,mobile,tablet}/spirit-pet.png`. Bốn screen trước đã khóa; Linh thú là screen active duy nhất.

## Audit toàn màn trước code

| Vùng | Canonical | Baseline hiện tại | Batch |
|---|---|---|---|
| Shell/tab | shared 1098×724 | đúng shared geometry | giữ nguyên |
| Hero stage | Thanh Vân Hồ lớn, identity + hai progress bar | art lớn đúng tỷ lệ; progress gộp trong một badge | giữ art provenance-backed, tách thân mật/tăng trưởng thành progress bars chung |
| Roster | bốn portrait; selected/level rõ | selected có art thật, ba slot khóa dùng lock | giữ ba slot khóa trung thực vì chưa có source riêng; không tạo pet giả |
| Detail-right | portrait/name/type/deployed, năm stats, hai skill, hai action | portrait/name và hai skill text thưa, thiếu stats/action xuất chiến | dựng hierarchy detail đầy đủ bằng dữ liệu review hiện hành; hai action disabled/read-only |
| Viewport | cùng landscape composition | đã không reflow | giữ scale toàn shell ở ba profile |

## State, asset và gate

- Thanh Vân Hồ là selection hiện hành; ba slot còn lại khóa và không nhận dead click.
- Reuse duy nhất art provenance-backed `spirit-fox-preview.png`; không imagegen thêm pet hoặc dùng class/source art.
- `Xuất chiến` hiển thị trạng thái `Đang xuất chiến`; `Bồi dưỡng` disabled vì chưa có progression contract.
- Capture đủ PC 1600×900, mobile 1600×720, tablet 1024×768. Khóa screen khi full Unity/Python validators, no-3D/no-source, frozen diff và visual audit đều đạt.

## Gate result — 2026-09-14

- Player `build/map01a-spirit-screen-player-v1/LinhGioiOnline.app` build thành công, `errors=0`.
- Evidence `build/map01a-spirit-screen-runtime-v1/{pc,mobile,tablet}/` đã xem trực tiếp; không stack, cắt hoặc chồng.
- Hero art giữ nguồn hiện hành; progress, roster, stats/kỹ năng và hai action detail-right cùng nằm trong shared shell.
- Kết luận: Linh thú đạt layout gate; toàn bộ character hub năm screen đã khóa.
