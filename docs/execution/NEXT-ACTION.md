# NEXT ACTION — Character Hub / nội dung trang bị đúng demo

## Active — hoàn thiện bốn nguồn Võ còn thiếu và độ rõ mỹ thuật theo nhóm
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`; không reset/restore/đổi branch.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`; check pause/recovery/job/claims trước batch. Không tác động phiên khác; kết thúc WAITING_USER/BLOCKED.
- Canonical layout vẫn `redesign-v4-five-tabs`. Class demo là nguồn hình, không tự đổi ID/tên/cấp/chức năng gameplay hoặc actor/pose/wardrobe/renderer/frozen.

## v39 — ba nguồn đã loại cơ thể, cùng base và registry exact
- Baseline1f90b2cd (v38) có3icon quần/đai/giày. Thêm tóc, áo trong, hộ uyển cho đúngVõ–male–Lv1; tổng6/10binding. Chưa thêm găng/áo ngoài/giáp/phụ kiện.
- Sửa alpha trên nguồn384px v38: bỏ mặt/cổ giữa tóc; cắt tay và đai riêng khỏi áo; bỏ cẳng tay/ngón khỏi hộ uyển. Nguồn/mask/hash/rectdemo được giữ; không đưa nguyên bản bịloại vào game, không sinh ảnh mới.
- Cuối batch giữ nguyên tọa độ native384, một viewport256→120trongcell128; không bbox-fit hoặc offsetUI. LượtPlayer đầu cho thấy thu0.875theonhóm là thừa nên đã bỏ, build/capture lần2 có trigger rõ. Găng còn họa tiết mặttrời lệchdemo; áo ngoài clone tạo tieslặp nên không nhập.
- Packer hiện hành thêm encodingOPT-IN rgbStep4: sai sốtừngkênhRGB≤2/255, alpha/hình học không đổi; lặp lại không cộng dồn sai số. Default vẫn khôngnénRGB. Đây KHÔNG phải losslessRGB, KHÔNG claim13module cũ byte-identical.
- Atlas640×512/218150byte,16module,6binding; trước là640×384/238156byte. Giữ budget250000. PNGnhỏ hơn không đồng nghĩaGPUmemory giảm vì heighttăng.
- Fullgraphics313/313noskip;72Python (69repo+3sourceprobes). RED→GREEN: encoding/metadata, knownbodypixels và spriteavailability. Missing-state test dùng fixture rỗng rõ ràng, không mặc định tóc luôn thiếu khi catalog tăng.
- Hai build/capture tronglượt; final là `player-reviewed/` và `runtime-reviewed/`,132ảnh. Đãxem12ảnh cuối:Nhânvật+3mónselected ở3viewport. Build cuối0error0warning; không cóinference/model mới.
- Không thấy cắt/chồng hoặc notice sai trên3món. Tóc/vải còn tối trongôtablet; chưa owner/whole-art acceptance. Actor hiện hành không thay, không claimđồ trênngười đãkhớp toànbộdemo.

## Bước tiếp theo
1. Găng/áo ngoài phải redraw đúngdemo, không dínhbody hoặc ghépđai/ties; giáp/phụkiện phải đúngloạiđồLv1. Không dùng bảnbịloại đểlấp hoặc bỏ metadataownership.
2. Cải thiện ánhsáng/độrõ cảbộ nguồn ở kíchthước hiểnthị, không phóngtừngicon bằngUI. Giữ nguồnfullprecision, chỉencodingatlas theo profile cókiểmsaisố.
3. Tiếp tục itemId/class/slot/gender/level và thamchiếudemo bằng pipeline chung; bổsung biếnthể khác chỉkhi đúngnguồn. Bốnclass/giớikhác không mượniconVõ.
4. Skill vẫn44/45sprite và nhiềuhình lệchdemo; HỏaTuyến/ThanhTẩy và bản sắcclass còn việc. Không coi sốlượngsprite hoặc testPASS lànghiệmthu.

## Evidence / source package
`build/character-hub-bodyfree-items-v39/`: runtime-review.json, encoding-proof-reviewed.json, reviewed-editmode.xml, source-repairs-native.json, masks-final/, prepared-native/, registry-native.json, runtime-reviewed/, player-reviewed/LinhGioiOnline.app.
`runtime-final/` và `player-final/` là lượtĐẦU trước bỏshrink, KHÔNG dùng làm bằngchứngcuối. Lỗi proofnative lầnđầu đọc nhầmmanifest đãgiữlog, không bỏhashassertion.
`lgo-bodyfree-item-icons-v39-authoring.zip` +`.zip.sha256` chứa nguồnđãsửa, mask, raw-reference-only, atlasbase và registryportable; khônggiảinénvàoResources. RGBsource không bịround; PNGtáixuất phải trùnghash. Không kèmfont/model/toolchain.
`CONTINUE / PARTIAL_BODYFREE_ITEM_ART / VISUAL_FIX_REQUIRED` — chưa nghiệmthu toànCharacterHub hoặc thiếtbịmobile/tabletthật.
