# Map01A Tiềm năng — implementation plan

Canonical screen duy nhất: `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/04-tiem-nang-five-tab-APPROVED.png` (1672×941). Baseline Player: `build/map01a-skills-screen-runtime-v4/{pc,mobile,tablet}/potential.png`. Screen active duy nhất là Tiềm năng; Nhân vật, Rương đồ và Kỹ năng đã khóa, Linh thú chưa sửa.

## Audit toàn màn trước code

| Vùng | Canonical | Baseline hiện tại | Batch cần xử lý |
|---|---|---|---|
| Shell/tab | shared 1098×724, rail năm tab | đúng shared geometry | giữ nguyên base đã khóa |
| Diagram kinh mạch | vòng kép có hoa văn, nhân vật thiền trung tâm, 5 node lớn | vòng đơn thưa, tâm là icon người kỹ thuật, node nhỏ | tạo một diagram layer chung, icon trung tâm và 5 node theo cùng geometry |
| Node tiềm năng | icon fantasy tròn, tên + chỉ số + affordance cộng | icon HUD phẳng trong khung vuông nhỏ | atlas Tiềm năng riêng; node base tròn dùng chung, số liệu hiện hành giữ nguyên |
| Footer | preset Võ bên trái, điểm còn lại bên phải | một badge đề xuất chạy giữa đáy | dựng footer hai đầu theo canonical; chỉ là thông tin read-only |
| Detail-right | icon lớn, mô tả, current/next facts, chi phí, hai action | icon nhỏ, copy kỹ thuật, một action khóa | hierarchy detail Tiềm năng trên shared detail base; `Cộng 1 điểm` và `Đặt lại` đều disabled |
| Viewport | cùng landscape composition | không reflow nhưng diagram/icon quá thưa | scale toàn shell; không stack, không breakpoint đổi hierarchy |

## State và interaction

- Mặc định chọn `Sinh lực`; click từng node cập nhật icon, tên, giá trị và detail-right bằng callback hiện có.
- Điểm còn lại là `12`, chỉ dùng làm dữ liệu hiển thị. Chưa có contract ghi thuộc tính nên `+`, `Cộng 1 điểm` và `Đặt lại` phải disabled rõ, không tự đổi state.
- Capture bắt buộc gồm mặc định `Sinh lực` và trạng thái chọn `Công` ở PC, mobile landscape và tablet.

## Asset gate

- Cần một board 2×3 có alpha thật: `Công`, `Thủ`, `Sinh lực`, `Linh lực`, `Nhanh nhẹn`, `Tâm mạch`.
- Board lấy trực tiếp ngôn ngữ hình ảnh từ canonical screen; lưu source/design board ngoài repo và runtime atlas có manifest/hash/provenance, status `DRAFT_RUNTIME_REVIEW`.
- Không dùng emoji, icon hệ điều hành, HUD placeholder hoặc ảnh class/source art.

## Base-first implementation

1. Viết regression test cho shared potential-node base, atlas riêng, node mặc định/chọn, footer hai đầu và hai action disabled.
2. Thêm base diagram/node/icon trong `CongDongLamArrivalHud.Skin.cs`; CharacterHub chỉ dựng hierarchy và bind data/callback.
3. Dùng loader atlas chung trong `CongDongLamMap01AArtPreview.cs`; thêm no-source-image gate cho atlas.
4. Triển khai toàn bộ vùng lớn trong một batch, build một Player, capture hai state trên ba profile một lượt.
5. Chỉ khóa Tiềm năng khi Unity tests, catalog/shared validators, no-3D/no-source, frozen diff và visual audit ba viewport đều đạt.

## Gate result — 2026-09-14

- Player `build/map01a-potential-screen-player-v1/LinhGioiOnline.app` build thành công, `errors=0`.
- Evidence `build/map01a-potential-screen-runtime-v1/{pc,mobile,tablet}/` có default/selected state và manifest chín frame; đúng 1600×900, 1600×720, 1024×768.
- Visual audit: diagram năm node, core, footer hai đầu, detail-right và hai action disabled giữ cùng composition; không stack, cắt hoặc chồng.
- Atlas sáu icon RGBA có manifest/hash/provenance, status `DRAFT_RUNTIME_REVIEW`.
- Kết luận: Tiềm năng đạt layout gate; screen active tiếp theo là Linh thú, bắt đầu bằng audit/plan toàn màn.
