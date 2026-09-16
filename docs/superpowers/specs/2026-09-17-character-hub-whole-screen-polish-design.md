# Character Hub whole-screen polish design

## Goal
Hoàn thiện UI/UX Character Hub 5 tab theo canonical `redesign-v4-five-tabs`, ưu tiên lỗi thật và base dùng chung thay vì pixel patch theo từng screen.

## Constraints
- Giữ branch/worktree hiện hành, actor/pose/wardrobe/renderer và frozen surfaces.
- Không mở gameplay contract chưa có; không bịa stat, item, pet hoặc progression.
- PC, mobile landscape và tablet giữ cùng composition; chỉ điều chỉnh density/scale/touch target.
- Shared modal, tabs, detail inspector, action state, icon frame và button base phải tái sử dụng.
- Build/capture chỉ khi runtime source đổi; audit bằng mắt trên Player trước khi chốt.

## Evidence-backed findings
- Character: equipment rail/detail hierarchy còn thưa; `+1` đang lấy từ item level nên gây hiểu nhầm enhancement.
- Bag: supply rows render `x0`; empty inventory semantics chưa trung thực; detail generic chiếm nhiều chỗ khi không có stat.
- Potential: write actions thực sự disabled nhưng skin vẫn gần active.
- Spirit Pet: tablet detail bị chật/scroll; disabled action chưa đủ rõ; locked roster không nên nhận dead-click.
- Skill: recent icon alignment đã có checkpoint riêng; chỉ regression audit, không rewrite pipeline.
