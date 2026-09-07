# Nhập môn Linh Thành: kịch bản và design nháp

Ngày: 2026-09-07. Trạng thái: DRAFT, chưa owner duyệt; không phải evidence runtime.

## Căn cứ

- Thiết kế liên kết NPC, Lộ, skill, trang bị và trang phục: `docs/design/LGO-GAME-SYSTEMS-NARRATIVE-DESIGN.md`; đây là đề xuất phát triển theo kịch bản gốc, không mở hệ thống production.
- Định hướng gốc owner cung cấp: nhân vật Người Thức Tỉnh, Linh Thành là trung tâm đời sống và chiến đấu; social city chuyển thành chiến trường trong Âm Giới Xâm Lăng. Không chọn class ngay đầu hành trình theo định hướng gốc; prototype hiện vẫn dùng class.sword và chưa triển khai progression mở Lộ.
- `docs/02-GDD.md`: city -> field/combat -> city/social/upgrade, `map.city.linh_thanh`, năm hướng phát triển với hai class trong Founder Alpha. Không mở social/economy chỉ vì xuất hiện trong concept.
- `docs/reference-art/linh-gioi-world-event-ui.png`: không gian Neo-Asian, grouping HUD và trục social city/world event; không sao chép VIP, tiền, cấp hoặc tính năng chưa có vào prototype.
- `docs/reference-ui/lgo-character-three-slots-draft-v1.jpg`: hướng skin đã duyệt, kính tối trung tính, vàng/ngà, base shell/button chung.
- Storyboard mới: `docs/reference-ui/lgo-linh-thanh-onboarding-storyboard-draft-v1.jpg`.

## Phân biệt hiện tại và đề xuất

M5 hiện là bài thử kỹ thuật: gặp Người Giữ Cổng -> tương tác Đá Luyện -> pulse/completed; lưu vị trí và về sảnh. Bia đánh/skill preview là bài thử riêng, không phải nhiệm vụ canon nối tiếp. Không có đủ địa hình, camera, thành phố, quest progression hay phần thưởng để gọi đây là onboarding hoàn chỉnh.

Đề xuất đặt vòng nhập môn ngắn tại khoảng sân sát Linh Môn, thuộc hướng vào Linh Thành, không tạo một map sân luyện tách khỏi sản phẩm. Tên khu vực, thoại và tuyến nhiệm vụ dưới đây là đề xuất, chưa tự thay GDD hoặc dữ liệu frozen.

| Cảnh | Ý định người chơi | Hành vi/feedback | Chuyển trạng thái | Khoảng cách với source |
|---|---|---|---|---|
| 1. Qua Linh Môn | Biết mình vừa đến đâu, ai đón mình | Đường đá dẫn tới NPC; một mục tiêu ngắn; movement trái, context action phải | Đến phạm vi NPC | Có input/proximity; chưa có sân nối city và camera như demo |
| 2. Người Giữ Cổng | Hiểu thao tác tương tác và hướng đi | Một header NPC, body thoại cuộn, footer Tiếp tục/Đóng; chặn input world khi modal mở | Kết thúc thoại -> mục tiêu Đá Luyện | Prototype đã bỏ header lặp, đóng giữa chừng không hoàn tất; thoại và scene còn cần đối chiếu design truyện |
| 3. Chạm Đá Luyện | Thử tương tác, nhận phản hồi rõ | Vòng focus tại vật thể, pulse ngắn, mục tiêu đổi trạng thái; không loot/EXP giả | Tương tác hợp lệ một lần | Có local pulse/completed; chưa có quest persistence |
| 4. Ra quảng trường | Biết bước tiếp theo và thấy thế giới mở ra | Hướng đi rõ về quảng trường, hết hướng dẫn nhập môn | Tới hub; nội dung tiếp theo theo roadmap | Chưa có tuyến đường/đích đến; không thay bằng popup thắng hoặc vòng luyện lặp |

## Trạng thái cần thiết kế trước khi code tiếp

- Chưa vào phạm vi / trong phạm vi / rời phạm vi: không để action nhắm nhầm NPC hoặc vật thể.
- Dialogue mở / dài / cuộn đến cuối / đóng giữa chừng / mở lại: chung base modal, body-only scroll, footer không tràn; world input bị chặn khi mở.
- Tương tác thành công / không hợp lệ / nhấn nhiều lần: không phát nhiều reward hoặc tự tiến quest. Hiện chưa có reward.
- Hoàn tất / quay lại hub / reconnect: chỉ hiển thị tiến trình đã lưu thật; local completed không được quảng cáo là quest persistence.
- PC rộng/hẹp, tablet 4:3, mobile landscape, resize trong cùng phiên: vị trí nhóm chức năng thống nhất; text/button không đè scene trọng tâm. Storyboard không phải chứng minh responsive hoặc kích thước pixel.
- Combat HUD, bốn active skill/ultimate/spirit, né và world-event chuyển trạng thái cần design riêng trước triển khai; không suy ra đã có từ các icon trong reference.

## Quy tắc asset và nghiệm thu

- Demo hội thoại riêng: `docs/reference-ui/lgo-gatekeeper-dialogue-draft-v1.jpg`, DRAFT chưa duyệt. Hai trạng thái ngắn/dài minh họa một speaker header, body cuộn, footer hai nút cùng tier không bọc thêm card. Cảnh nền và portrait mới là concept, không tự thay NPC runtime. Ảnh sinh chưa đúng tỷ lệ 16:9 và body còn thụt theo cột portrait; không dùng tọa độ ảnh làm kích thước code hoặc coi đó là viewport evidence. Khi triển khai, body dùng vùng còn lại của base modal, header/footer không co và không lọt khỏi shell.
- Nhãn `Đã hiểu` trong demo chỉ phù hợp nếu action thật xác nhận hướng dẫn; không đổi nhãn của action hoàn tất nhiệm vụ sang xác nhận rồi giữ hành vi cũ. Thoại dài trong ảnh là đề xuất lời văn, chưa là dữ liệu canon.
- Storyboard chỉ review kịch bản/composition, không crop/slice/import composite. Bốn khung chưa phải viewport matrix chuẩn; chi tiết chữ trang trí trong ảnh không phải nội dung canon.
- Tách brief cho ground, kiến trúc, NPC, nhân vật, icon và VFX; mỗi asset ghi kích thước hiển thị thực, texture/import profile, source bytes và memory/build cần đo. Không phát sinh ảnh lớn chỉ để dùng như icon.
- Chỉ bắt đầu vertical slice world khi có layout/demo cho cảnh được chọn, acceptance trạng thái và mapping tới hành vi được roadmap cho phép. So ảnh runtime với đúng cảnh/viewport/state, không coi capture hoặc concept đẹp là PASS.
- Không mở production auth/DB, economy, social hay event thật trong batch thiết kế này.

## Demo bố cục một cảnh: sân Linh Môn nối phố

`docs/reference-ui/lgo-linh-mon-arrival-composition-draft-v1.jpg`: DRAFT chưa duyệt, 1440x810, 404279 byte. Đây là composition cho scene, không phải đặc tả HUD đầy đủ hoặc ảnh runtime. Không crop/slice ảnh thành asset. NPC/đá/kiến trúc và nhân vật trong ảnh chưa là model đã có.

Chọn đường phố nhìn về phía trước theo reference gốc, thay vì coi sân tròn với sprite là đích sản phẩm. Không mở toàn city: chỉ một khoảng sân nối phố, một NPC và một vật thể đã có hành vi prototype. Cổng phát sáng phía xa là chi tiết concept, không tự mở teleport/event. Chữ khắc, trang phục và khuôn mặt trong ảnh chưa chốt canon.

Mặt bằng blockout đề xuất, đơn vị thử 1 unit = 1 m; KHÔNG thay tọa độ hoặc hồ sơ đang lưu ở bước design:

| Điểm | Tọa độ X/Z đề xuất | Ý nghĩa và khoảng trống |
|---|---|---|
| Sân | X từ -7 đến 7, Z từ -5 đến 15 | Một cụm kiểm chứng nhỏ, không phải kích thước city |
| Lối chính | X từ -2 đến 2 | Trục đi thông; không đặt đá/cây/NPC giữa lối |
| Vào sân | 0 / -3 | Nhìn thấy NPC và khoảng sân bên phải ngay khi vào |
| Người Giữ Cổng | -3 / 1 | Đứng cạnh lối, không sau bảng nhiệm vụ; điểm thử tương tác -1.8 / 1 |
| Đá Luyện | 3.5 / 4 | Khoảng sân bên phải cùng cao độ; điểm thử tương tác 2.3 / 4 |
| Hướng ra phố | 0 / 14 | Đích thị giác; chưa tự mở map transition hoặc quest mới |

Hai điểm đứng thử cách mục tiêu 1.2 m, nằm trong tầm prototype 1.45 m. Cùng Z và lệch X giúp thử cách tiếp cận từ bên, thay vì đứng giữa camera và vật thể. Đây là giả thuyết staging cần kiểm chứng bằng hình chiếu/collision thực, không phải bằng chứng hết che khuất. Phải có đường đi liên tục từ vào sân -> NPC -> đá; không teleport trong nghiệm thu tuyến đi.

Camera: mục tiêu là thấy phố phía trước và toàn thân người chơi, không chuyển sản phẩm thành top-down chỉ vì prototype đang dùng orthographic. Blockout thử perspective cố định hướng vào phố trước, chưa thêm free orbit. Nhân vật mục tiêu chiếm khoảng 20-25% chiều cao vùng chơi; ảnh sinh hiện lớn hơn mức này, nên không suy FOV/distance từ pixel ảnh. Đo lại hai mục tiêu khi đến gần, lùi và đi ngang; cột cổng/nhà/cây không chắn camera. Chưa chọn thông số FOV hoặc thay camera main trước khi có thử nghiệm và đối chiếu.

Nghiệm thu trước thay scene chính:
- Arrival: nhận ra NPC, lối chính và đá; nút Gặp ngoài tầm phải disabled. Nút Gặp sáng trong ảnh là thiếu sót của mockup, không phải behavior được duyệt.
- Đến NPC từ lối chính: tên/prompt không đè silhouette hoặc HUD; mở/đóng/hoàn tất thoại vẫn dùng base và state hiện có, không hoàn tất vì đóng modal.
- Sau thoại: đi thật tới điểm bên cạnh đá; player không che phần nhận diện chính của đá; Luyện đúng tầm, ra ngoài tầm khóa lại; không cấp EXP/đồ giả.
- PC rộng/hẹp, tablet, mobile landscape và resize: giữ tuyến đi và điểm tương tác trong vùng quan sát, kiểm tra world bounds chiếu camera cùng UI bounds. Một ảnh 16:9 không chứng minh mọi viewport.
- Lưu/re-entry: không diễn giải lại tọa độ hồ sơ cũ theo mặt bằng mới mà chưa có kế hoạch tương thích. Không đổi map/protocol/schema trong blockout.

Asset đầu tiên cần cho blockout: mặt đường tileable, một module tường/mái lặp lại, hai cột cổng, NPC/player đúng chiều cao, đá riêng; chưa sản xuất phố đầy đủ hoặc crowd. Dùng geometry/proxy hợp lệ và asset đã có để kiểm chứng trước, không dùng ảnh nền thay môi trường có thể đi được. Texture source/build/memory chỉ chốt sau đo kích thước trên màn hình; demo 404KB không nói lên budget của scene 3D.

Provenance demo một cảnh: imagegen trong chat, tham chiếu world-event gốc và storyboard nhập môn; nguồn `~/.codex/generated_images/01a0748f-76a8-7be2-bd55-33fe5e41c403/exec-5aa1c4b3-dda0-484a-ad62-6bf5d9a310fb.png`, 1672x941. Sips giữ tỷ lệ, cạnh dài 1440, JPEG quality76; đã xem bản nén, không crop. Demo chưa có chuỗi ảnh tiếp cận hoặc blockout Unity.

## Blockout runtime đầu tiên, chưa thay scene chính

- `OnboardingBlockoutWorld` dựng geometry/collider, camera perspective và player capsule 1.8m; `OnboardingBlockoutPreview` dùng movement pad/layout/PNG writer có sẵn. Không có account client, save, quest hoặc hội thoại mới trong preview.
- Chỉ mở bằng `--lgo-onboarding-blockout` trong development build/Editor. Builder hiện có nhận `LGO_PLAYER_DEVELOPMENT=1`; không có biến này vẫn build mặc định. Test `blockout-opt-in-green.xml`: 1 executed, 1 passed; red trước đó bắt preview chưa tồn tại. Chưa chạy riêng release Player để thử cờ.
- Lệnh mở bản development hiện tại: `build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity --lgo-onboarding-blockout`. Thêm `--lgo-blockout-evidence-dir /absolute/path` để đi tuyến và chụp arrival/keeper-side/stone-side tự động, rồi thoát. Không chạy cùng lúc với capture khác.
- Tuyến CharacterController đi từ spawn tới -1.8/1, qua 0/2.5 rồi 2.3/4; không teleport. Ba log `build/dev-loop/blockout-lit-{desktop,tablet,mobile}.log` kết thúc exit 0 và có `LGO_ONBOARDING_BLOCKOUT_ROUTE_PASS`. Đây là input vector mô phỏng qua controller, chưa gesture/phím vật lý hoặc quest completion.
- Chín PNG trong `build/visual-evidence/onboarding-blockout/lit-{desktop,tablet,mobile}/`: đúng 1920x1080, 1366x1024, 960x540; analyzer PNG có sẵn xác nhận variation, mỗi profile ba hash khác nhau. Đã xem desktop arrival, tablet gần NPC, mobile gần đá; điểm bên đá không bị player che trong frame đó. Chưa chứng minh mọi đường đi/góc camera/resize.
- Camera có vùng đệm phía sau: floor/collider biên kéo tới Z=-12 để không có tường chắn giữa camera và player lúc spawn; vùng tương tác vẫn giữ các điểm design. Nhãn dùng base quay theo camera, không giữ nghiêng 55 độ của prototype cũ.
- Probe `blockout-shader-probe.log` xác nhận material cũ fallback `Sprites/Default` trong Player. `LGOSharedLitSurface.mat` giữ reference URP Lit cho `RuntimeArtCatalog` chung; log mới xác nhận Lit và ảnh có shading/bóng. Build development tăng từ 340542518 lên 340585406 byte (+42888 byte cho lượt thay đổi), không thêm texture; không suy ra release budget từ số này. Căn cứ stripping: [Unity Shader.Find](https://docs.unity3d.com/ScriptReference/Shader.Find.html).
- Flow chính ba profile vẫn qua `shared-lit-surface-baseline.log`, ảnh mobile world đã xem; quick `onboarding-blockout-quick.log` pass. Blockout còn hình hộp, bóng răng cưa, label xa nhỏ, player proxy và không có phố phía xa; không visual PASS hoặc giống demo final. Bước tiếp: reuse state/UI hội thoại nhập môn cho cảnh mới, không sao chép flow hoặc đổi hồ sơ đang lưu.

## Cập nhật blockout: vòng NPC dùng chung

- Main world và blockout dùng `NpcDialogueSession` cho mở/đọc/hủy/hoàn tất; cùng `RuntimeNpcDialogueView` dựng shell/header/body-scroll/footer qua factory và responsive layout đang có. Nội dung blockout riêng theo đường phía phải, không nhắc bóng/slime của scene cũ. Đây là thoại draft local, không quest hoặc canon mới.
- Gặp chỉ hoạt động trong 1.45m; đóng không mở bước đá, mở lại từ dòng đầu; đọc hết ba dòng và xác nhận mới có Luyện. Đang thoại chặn keyboard/vector movement và ẩn nhãn world. Đá chỉ hoàn tất trong tầm; chưa có pulse/focus trong blockout, mới đổi nút thành Đã xong, nên cần tiếp presentation theo cảnh 3.
- Unit `build/dev-loop/npc-session-green.xml`: 3 executed/3 passed; red trước đó là thiếu type gây compile failure, không phải ba NUnit test đã chạy fail. Main flow ba profile `npc-shared-view-baseline.log` và quick `npc-shared-flow-quick.log` qua sau extraction; chưa rerun main sau thay đổi chỉ trong preview font/bounds fixture.
- Blockout mới: `build/dev-loop/blockout-npc-font-{desktop,tablet,mobile}.log`, cả ba exit 0 và có route/NPC flow marker. Fixture dùng movement vector qua CharacterController, submit event vào nút thật, thử ngoài tầm, hủy/mở lại, input-lock và bounds căn giữa. Không chứng minh gesture/phím thiết bị thật hoặc resize liên tục.
- 15 ảnh tại `build/visual-evidence/onboarding-blockout/npc-font-{desktop,tablet,mobile}/`: arrival, keeper-side, dialogue, stone-side, complete. Đã xem thoại desktop/mobile và complete tablet; body/footer trong shell, top/bottom cân bằng; player proxy/geometry/bóng chưa đạt demo final. Không thêm ảnh runtime hoặc claim visual PASS. Các log/ảnh blockout đời đầu ở phần trên là lịch sử trước khi nối thoại.

## Feedback đá và khoảng trống nhân vật

- Cảnh 3 đã thêm focus vàng khi trong tầm và đã hoàn tất thoại; tương tác một lần phát pulse linh khí nở/tắt 1.2s, nhãn Đã ổn định còn lại. Dùng `GetWorldPlatformGlowSprite` 192x192 đã có và `CreateGroundGlowSprite`, không thêm ảnh/import mới. Không chứng minh giảm build/memory; đây là texture procedural cached, không asset miễn phí về RAM.
- Red `build/dev-loop/blockout-stone-red-player.log` exit 1 vì chưa có focus. Ba bản cuối `blockout-stone-label-{desktop,tablet,mobile}.log` exit 0; kiểm tra focus, pulse kết thúc, nhãn/shadow đồng bộ. `blockout-stone-label-quick.log` qua; ảnh `build/visual-evidence/onboarding-blockout/stone-label-*/`, đã xem mobile complete và tablet complete-settled. Lượt green trước sửa nhãn không phải evidence visual cuối: shadow còn text cũ, đã thay gán trực tiếp bằng `WorldLabelPresenter.Set`.
- Tiếp theo cần đối chiếu reference Kiếm/trang phục và demo composition để có hình mẫu góc sau/ba phần tư phù hợp camera phố trước khi thay capsule. Candidate nam hiện là sprite nhìn trước; không dùng nó để tuyên bố có model/animation third-person. Không chốt class của Người Thức Tỉnh hoặc ngoại hình canon từ asset proxy.

## Camera phố và mẫu trang phục nhập môn

- Camera blockout dùng Cinemachine 3.1.7: ClearShot chọn góc sau/hai bên, Deoccluder tránh vật cản, Confiner3D giữ camera trong phạm vi phố. Một Brain cập nhật trước nhãn world; không còn hai nơi ghi transform camera. Input giữ hướng tại lúc bắt đầu nhấn, chỉ lấy hướng camera mới khi nhả/nhấn lại; đóng thoại hoặc mất focus xóa input.
- Red thật: `blockout-camera-red-player.log` bị nhà che; `blockout-camera-hold-red.log` đổi hướng đang giữ nút. Các bản chỉ kiểm tra một tia đã từng qua nhưng ảnh vẫn sai; không dùng làm visual pass. Bản cuối `blockout-camera-final-{desktop,tablet,mobile}.log` kiểm tra đầu/thân/chân, framing, camera switch và input giữ/nhấn mới; cả ba qua. Đã xem ảnh desktop `camera-alley.png` và tablet `camera-exit.png` trong `build/visual-evidence/onboarding-blockout/camera-final-*/`.
- Hồi quy main ba profile: `build/dev-loop/cinemachine-main-flow-baseline.log` kết thúc EVIDENCE_CAPTURED_FOR_REVIEW; quick `camera-checkpoint-quick.log` PASS. Build development cuối 341618105 byte, tăng 1020591 byte so với bản trước dependency; không quy toàn bộ delta cho thư viện hoặc coi là budget release.
- Chưa kiểm chứng mọi vị trí sát tường/góc nhà hoặc từng frame chuyển camera; chưa orbit, model nhân vật, NPC 3D hay visual PASS. NPC/đá nhìn cạnh vẫn lộ ảnh phẳng. Bước tiếp: đi sát hai mép ngõ và vòng góc nhà để kiểm tra che khuất trước khi cân nhắc đưa camera vào main flow.
- Demo `docs/reference-ui/lgo-arrival-outfit-turnaround-draft-v1.jpg`: 1280x853, 154008 byte, JPEG76, chỉ reference ngoài runtime. Mẫu trước/bên/sau/ba phần tư lấy hướng bộ TRAINING trong reference Kiếm và composition nhập môn; không chốt class/canon. Thumbnail camera trong tranh có nhân vật quá lớn, không dùng làm chuẩn framing. Chưa có model/rig tương ứng; không cắt tranh composite làm sprite/model giả.
- Nguồn imagegen: `~/.codex/generated_images/01a0748f-76a8-7be2-bd55-33fe5e41c403/exec-9793cbe1-ef37-4611-83b2-f69b82a6dd9b.png`; đã xem bản nén. NPC/Lộ/skill/trang bị/trang phục tiếp tục theo `LGO-GAME-SYSTEMS-NARRATIVE-DESIGN.md`, không mở hệ thống ngoài roadmap vì có concept.

## Đá Luyện có chiều dày trong blockout

- Bản cuối: `build/dev-loop/stone-volume-final-{desktop,tablet,mobile}.log`, cả ba exit 0; vòng thoại/đá, mesh volume, bốn mép ngõ và giữ hướng qua camera đều đi qua. Quick `stone-volume-final-quick.log` PASS. Ảnh ở `build/visual-evidence/onboarding-blockout/stone-volume-final-*/`; đã xem mobile bên đá, desktop cạnh ngõ phải, tablet hoàn tất. Mesh có mặt bên thật, vùng bóng dễ đọc hơn; vẫn proxy và chưa visual PASS. Không rerun main vì chỉ thay preview development, tái dùng baseline `cinemachine-main-flow-baseline.log` của main không đổi.
- Build development cuối 341623453 byte (+5348 so với checkpoint camera), không suy ra release/memory budget; ảnh V3B vẫn ở Resources, không claim đã bỏ payload. Tiếp: tìm nguồn model/rig có license rõ cho nhân vật theo turnaround nhập môn, kiểm tra silhouette/góc sau và chuyển động trước khi thay capsule; không lấy ảnh mặt trước hoặc mannequin không đúng design làm final.

- Demo `docs/reference-ui/lgo-training-stone-volume-draft-v1.jpg`: 1280x853, 114704 byte, JPEG76; tham chiếu đá V3B, chỉ bản mẫu hình khối ngoài runtime. Imagegen nguồn `~/.codex/generated_images/01a0748f-76a8-7be2-bd55-33fe5e41c403/exec-524f5b93-0974-4025-9c5d-f485c2c03222.png`; đã xem bản nén, không crop/slice. Không thay art main hoặc coi là production art.
- Proxy cao 1.5m, bệ 0.65x0.45m, thân thu nhỏ/đỉnh lệch và dấu ngọc phía trước; mesh đá 80 tam giác, inset cube riêng, không texture mới. Dùng Lit chung và giữ collider 0.65x1.5x0.65m như trước để không đổi collision/tầm tương tác. Đây là khối giản lược theo demo, chưa sao chép chi tiết mặt đá hoặc rune V3B.
- Red `stone-volume-red-player.log` exit 1 vì đá còn SpriteRenderer không có chiều dày mesh. `camera-edge-mobile.log` trước thay đá đã qua bốn điểm sát vách ngõ trái/phải; không đổi camera vì không tái hiện lỗi ở bốn điểm đó. Chưa kiểm chứng mọi góc nhà hoặc toàn bộ frame chuyển camera.
- Ảnh mesh đầu (`stone-volume-green-mobile/stone-side.png`) cho thấy đá quá tối. Bootstrap có ambientMode=0; blockout trước chỉ gán ambientLight. Chuyển riêng preview sang Flat rồi tăng mức fill để đọc khối trong bóng râm; không thêm đèn từng vật thể. Căn cứ: [Unity ambientLight](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/RenderSettings-ambientLight.html). Bản Flat đầu vẫn tối, không lấy làm evidence hình ảnh cuối.

## Guidance và khảo sát model

- Bản cuối: `shared-guidance-final-{desktop,tablet,mobile}.log` exit 0; assertions mục tiêu/ẩn khi thoại/safe bounds/không đè pad qua. `shared-guidance-main-baseline.log` hoàn tất capture ba profile; quick `shared-guidance-final-quick.log` và validator density qua. Đã xem main mobile gần đá, blockout mobile arrival, tablet hoàn tất, PC thoại. Chưa thử cuộn bằng gesture/nội dung dài hoặc mọi kích thước.
- PC có lần mất foreground, Player vẫn sống; đưa đúng process lên foreground có thời hạn rồi chạy tiếp, không restart. Ảnh `guidance-final-mobile/arrival.png` chọn góc bên sai composition; main `profiles/mobile/near-training-stone-prompt.png` lộ mảng cyan chữ nhật sau nhân vật. Hai lỗi visual này chưa được giải quyết bởi guidance; tiếp truy nguyên, không claim visual PASS hoặc camera mọi góc.

- Storyboard cảnh 1/3 có mục tiêu bên trái; blockout trước chỉ có nút Gặp/Luyện nên thiếu ngữ cảnh khi mới vào. Dùng `RuntimeWorldGuidanceView` chung với main, tái sử dụng factory/typography; controller main giữ alias phục vụ refresh/evidence, không dựng label guidance riêng nữa.
- Preview hiện Gặp Người Giữ Cổng -> Chạm Đá Luyện -> Linh khí đã ổn định; gợi ý đổi khi vào tầm, ẩn trong hội thoại. Chưa mở chặng quảng trường hoặc quest persistence. Vùng riêng theo safe viewport: bên trái, top 18%, max-height 30%, width không quá 42% hoặc giới hạn HUD hiện có; ScrollView dọc giữ nội dung dài trong vùng. Đây là guidance, không phải modal hoặc layout dialog mới.
- Ảnh mobile đầu cho thấy cỡ chữ 11 của base quá nhỏ sau panel scale; tăng hai cỡ guidance mobile lên 18 trong typography chung, không override preview. Red thiếu mục tiêu: `blockout-guidance-red-player.log`; bản green đầu chưa là evidence typography cuối. Validator density cũ tìm marker trong controller được chuyển sang owner view, không thêm validator.
- Khảo sát model: repo chưa có FBX/GLB/Blend; chưa có Blender trên PATH. [Quaternius Universal Base Characters](https://quaternius.com/packs/universalbasecharacters.html) công bố Humanoid rig, trung bình 13k triangles và CC0; có thể làm nền rig nhưng không phải trang phục nhập môn đã duyệt. [Modular Character Outfits Fantasy](https://quaternius.itch.io/modular-character-outfits-fantasy) có bộ đồ modular CC0, chưa xác minh một bộ khớp turnaround Neo-Asian. [Kenney Protagonists](https://kenney.nl/assets/animated-characters-protagonists) có CC0, chưa chọn vì chưa chứng minh phù hợp design. Không tải/import cả pack hoặc dùng bản mirror không rõ quyền; chưa có model thay capsule.

## Nguồn ảnh storyboard

Imagegen trong chat, tham chiếu world-event và Character Hall đã duyệt; nguồn ngoài repo `~/.codex/generated_images/01a0748f-76a8-7be2-bd55-33fe5e41c403/exec-69276ad5-037e-449c-8a6f-e671b1473e0d.png`. JPEG quality78 bằng sips, không crop. Demo mới chưa owner duyệt.

Demo dialogue: cùng thư mục generated_images, `exec-0d2fe0d9-4b9e-4b83-b783-603fe51850ee.png`, tham chiếu Character Hall đã duyệt và NPC direction v0.20.0; JPEG quality78, không crop/slice/import runtime.
