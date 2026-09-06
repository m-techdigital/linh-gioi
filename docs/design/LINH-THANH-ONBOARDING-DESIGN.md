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

## Provenance storyboard và dialogue

Imagegen trong chat, tham chiếu world-event và Character Hall đã duyệt; nguồn ngoài repo `~/.codex/generated_images/01a0748f-76a8-7be2-bd55-33fe5e41c403/exec-69276ad5-037e-449c-8a6f-e671b1473e0d.png`. JPEG quality78 bằng sips, không crop. Demo mới chưa owner duyệt.

Demo dialogue: cùng thư mục generated_images, `exec-0d2fe0d9-4b9e-4b83-b783-603fe51850ee.png`, tham chiếu Character Hall đã duyệt và NPC direction v0.20.0; JPEG quality78, không crop/slice/import runtime.
