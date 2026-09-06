# Linh Giới: kịch bản liên kết các hệ thống

Ngày: 2026-09-07. Trạng thái: đề xuất thiết kế, chưa duyệt để triển khai các hệ thống mới. Không phải specification protocol/schema hoặc evidence runtime.

## Căn cứ và giới hạn

- Kịch bản gốc owner: Người Thức Tỉnh -> mở Lộ; Linh Thành là nơi sống và nơi cần bảo vệ; Âm Giới Xâm Lăng là sự kiện nhận diện. Năm Lộ: Võ, Kiếm, Pháp, Cơ, Linh. Bản gốc dự kiến Soft Launch Võ/Kiếm/Pháp, Cơ/Linh sau đó.
- `docs/02-GDD.md`: Founder Alpha chỉ Kiếm/Võ, level 1-20; bốn active, một ultimate, một spirit skill, thêm đánh thường/né. Không đánh đồng Founder Alpha với Soft Launch.
- `LGO-CONTENT-TAXONOMY-v1.0.md`, `LGO-DIALOGUE-CONTENT-PIPELINE-v1.0.md`, `LGO-MAP-ZONE-MODEL-v1.0.md`, `LGO-SKILL-EFFECT-CONTENT-PIPELINE-v1.0.md` là tài liệu liên quan đã có; không tạo bộ schema song song.
- `LINH-THANH-ONBOARDING-DESIGN.md` và storyboard nhập môn mới là bản nháp cho chặng đầu. M5 guided training hiện là thử nghiệm kỹ thuật, không đủ để gọi là chương truyện hoặc thành phố hoàn chỉnh.
- Tên M4/M5/M6 trong các task prototype không tự chứng minh milestone cùng số trong `MILESTONE-ROADMAP.md` đã đóng. Mỗi slice phải đối chiếu gate thực trước khi mở phạm vi.

## Phương án phát triển

Chọn tuyến truyện ngắn đi xuyên các hệ thống thay vì làm toàn bộ NPC, rồi toàn bộ đồ, rồi toàn bộ skill. Làm các hệ thống đồng thời ở quy mô lớn sẽ tăng dữ liệu và asset trước khi kiểm chứng vòng chơi; làm các màn độc lập sẽ tiếp tục thiếu liên kết.

Tuyến đề xuất: đến Linh Thành -> được hướng dẫn tương tác -> hiểu lựa chọn Lộ -> thử một hành vi chiến đấu -> đi vùng ngoài -> trở về với một thay đổi có ý nghĩa -> nhận ra vì sao cần bảo vệ thành phố. Không nhồi mọi bước vào một buổi tutorial; các chặng sau chỉ mở khi milestone cho phép.

## NPC là người trong thế giới

Các vai trò dưới đây là đề xuất; chưa tự đặt tên riêng, lịch sử môn phái hay canon mới. Chỉ Người Giữ Cổng có vòng tương tác prototype hiện tại.

| Vai trò | Nơi gặp / ý nghĩa | Hành vi bình thường | Khi có xâm lăng, hướng dài hạn | Demo phải có |
|---|---|---|---|---|
| Người Giữ Cổng | Lối vào Linh Thành; đón Người Thức Tỉnh | Giới thiệu nơi đến, chỉ một việc kế tiếp | Chuyển từ đón khách sang hướng dẫn sơ tán | Dáng toàn thân, portrait, gần/xa, thoại ngắn/dài, đã gặp |
| Người hướng dẫn Lộ | Khu luyện tập gắn với thành phố | Giải thích lối chơi; cho thử trước khi quyết định | Tổ chức phòng thủ theo vai trò | So sánh Kiếm/Võ, chưa đủ điều kiện, thử, quay lại, xác nhận |
| Thợ rèn | Khu chế tác | Giúp hiểu trang bị đang mặc và thay đổi chỉ số | Vai trò hậu cần, chưa chốt sửa đồ hoặc durability | So sánh đồ, ô trống, không tương thích, kết quả thao tác |
| Thợ may | Khu sinh hoạt/thương phố | Thử ngoại hình mà không đổi sức mạnh | Phản ứng theo tình trạng thành phố | Thử/hủy/áp dụng, chưa sở hữu, che mũ, clipping tư thế |
| Người trông coi Linh Thú | Địa điểm cần thiết kế trong city | Giới thiệu khác biệt hành vi của Linh Thú | Hỗ trợ theo luật event khi được mở | Đồng hành chưa có/đã có, kỹ năng hỗ trợ và giới hạn |
| Cư dân / chủ quán | Quảng trường/café | Cho thấy đời sống và lý do trở về, không chỉ là menu | Trú ẩn rồi trở lại sinh hoạt | Bình thường/cảnh báo/phục hồi; đường đi không chặn người chơi |

Không mở shop, giao dịch, crafting hoặc AI crowd chỉ vì NPC xuất hiện trong demo. Vai trò chưa có backend chỉ được trình bày là concept, không có nút mua hoặc phần thưởng giả trong game.

### Hợp đồng hành vi NPC cần mô tả trước khi code

- Mỗi nội dung ghi NPC, vùng, điều kiện gặp, mục tiêu người chơi, câu thoại, điều kiện kết thúc, quyền sở hữu trạng thái và demo tương ứng.
- Tách trạng thái thế giới dùng chung khỏi tiến trình riêng nhân vật. Một người đóng hội thoại không được đổi trạng thái NPC cho mọi người.
- Luồng cơ bản: ngoài phạm vi -> có thể tương tác -> đang thoại -> kết thúc/đóng; rời phạm vi, disconnect, NPC không còn khả dụng phải có kết quả xác định.
- Không tiến nhiệm vụ chỉ vì mở popup. Tương tác phải kiểm tra điều kiện; thao tác lặp/retry không được nhận thưởng nhiều lần khi hệ thống thưởng được mở.
- NPC event dùng trạng thái event có thẩm quyền; không chạy timer riêng trên từng client để quyết định sơ tán hoặc thắng/thua.
- UI dùng cùng base speaker header, body scroll, footer action. Portrait và nội dung thay đổi, không tạo mỗi NPC một modal riêng.

## Lộ, môn phái và kỹ năng

Lộ là bản sắc chiến đấu. Môn phái là tổ chức trong truyện, nếu được duyệt. Chưa có căn cứ để tự gán năm Lộ thành năm bang/phái có lãnh địa, danh vọng hoặc quan hệ thù địch. Những tên riêng và cơ chế gia nhập/đổi phái cần một quyết định thiết kế riêng.

| Lộ | Bản sắc từ kịch bản | Điều cần thể hiện trong demo | Phạm vi |
|---|---|---|---|
| Võ | Cận chiến nhanh, phản đòn, áp lực | Tư thế, nhịp phản đòn, tín hiệu thời điểm | Founder Alpha theo GDD |
| Kiếm | Cơ động, crit, combo | Đường kiếm, dịch chuyển, kết thúc combo đọc được | Founder Alpha theo GDD |
| Pháp | Diện rộng, khống chế, nguyên tố | Vùng tác động và ranh giới nguy hiểm | Thiết kế dài hạn; chưa mở runtime |
| Cơ | Súng, drone, gadget | Silhouette công nghệ hợp Neo-Asian, mục tiêu/đạn rõ | Sau giai đoạn đầu |
| Linh | Triệu hồi, hồi phục, debuff | Chủ thể/triệu hồi phân biệt, tín hiệu hỗ trợ | Sau giai đoạn đầu |

Gốc dự kiến khoảng 10-14 skill mỗi Lộ nhưng loadout giới hạn. Đây không phải lệnh sản xuất ngay 50-70 icon hoặc kỹ năng. Chọn một tương tác chiến đấu có thể kiểm chứng trước, rồi mở rộng khi animation, input và luật server đã đủ.

Mỗi skill phải có: ý định chiến thuật; điều kiện dùng; mục tiêu/tầm/hình vùng; bắt đầu -> tác động -> hồi phục; cooldown/tài nguyên; hủy/ngắt; trạng thái không hợp lệ; icon/VFX/SFX; input PC/tablet/mobile; nguồn thẩm quyền; test trúng/trượt/lặp/reconnect phù hợp scope. Không tự chốt số damage/cooldown từ tranh concept.

Prototype local và server-authoritative combat phải ghi riêng. Không gọi nút pulse hoặc preview là một skill hoàn chỉnh. Linh Thú thay đổi hành vi build theo GDD, không chỉ là ngoại hình và không khóa sức mạnh độc quyền sau trả tiền.

## Trang bị, hành trang và trang phục

Ba khái niệm tách biệt: hành trang giữ vật phẩm; trang bị quyết định công dụng/chỉ số; trang phục quyết định ngoại hình. Đề xuất thử trang phục không thay chỉ số, hủy preview trả đúng bộ đang mặc. Quyền sở hữu theo account hay nhân vật chưa được chốt, không suy ra từ giới hạn ba ô nhân vật.

| Nhóm | Luồng đề xuất | Trạng thái bắt buộc trong design |
|---|---|---|
| Hành trang | Nhận -> xem -> dùng/trang bị theo loại | Trống, đầy, lọc không kết quả, item đã thay đổi, số lượng dài |
| Trang bị | Chọn -> so sánh -> mặc -> thấy kết quả | Ô trống, không đủ điều kiện, không tương thích, đang chờ server, thất bại không mất đồ |
| Trang phục | Xem trên nhân vật -> thử -> hủy hoặc áp dụng | Chưa sở hữu, tương thích cơ thể, che/xuyên mesh, tải asset thất bại, giữ chỉ số |
| Vật phẩm nhiệm vụ/nguyên liệu | Xem nguồn và công dụng thật | Chưa dùng được, không được giao dịch nếu luật quy định, không hiện action giả |

Chưa chốt số slot đồ, rarity, chỉ số, tỷ lệ rơi, crafting, giao dịch hay giá. Những quyết định này cần combat/progression và data contract; không vẽ UI đầy chỉ số tưởng tượng rồi ép source theo ảnh.

## Design/demo và khoảng trống hiện tại

| Phần | Reference đang có | Còn thiếu trước triển khai mới |
|---|---|---|
| Character Hall | `docs/reference-ui/lgo-character-three-slots-draft-v1.jpg` | Hoàn thiện states/giới tính và đối chiếu runtime, không coi mọi chi tiết đã đạt |
| Nhập môn / NPC | Storyboard nhập môn mới; `docs/reference-art/v0.20.0/lgo-npc-direction-sheet-v0200.png` | Demo riêng Người Giữ Cổng và dialogue theo skin mới, bản đồ đường đi, states |
| Năm Lộ | `docs/reference-ui/lgo-five-paths-character-reference-v1.jpg` | Turnaround/animation, lựa chọn và mở Lộ; concept không phải spec skill |
| Trang phục / skill Kiếm | `docs/reference-ui/lgo-sword-costume-skill-concepts-v1.jpg` | Compatibility, preview/apply, keyframes VFX, kích thước icon thật |
| Hành trang / đồ | `docs/reference-art/future-reference-v0.36.0/lgo-extra-character-inventory-ui-v0360.png` | Kiểm tra lại nội dung và chuyển skin mới; slot/state/so sánh đồ chưa được chốt |
| City / event | `docs/reference-art/linh-gioi-world-event-ui.png` | Layout playable từng khu, camera, đường đi, tất cả state event và asset riêng |

Danh sách trên ghi reference có sẵn, không xác nhận toàn bộ ảnh đã được duyệt hoặc từng chi tiết phù hợp. Không bỏ reference cũ; dùng làm đầu vào đối chiếu, không crop/slice composite để import.

Mỗi demo mới cần đi kèm luồng và state matrix, không chỉ một ảnh đẹp: PC rộng/hẹp, tablet, mobile, resize, nội dung dài, empty/loading/error. Cùng loại UI phải cùng base; shell giới hạn viewport và cân trên/dưới, header/footer giữ trong shell, chỉ body cuộn. Đo bằng bounds và tương tác runtime, không đoán từ ảnh concept.

Asset brief phải ghi kích thước hiển thị và số lượng xuất hiện cùng lúc, alpha, animation, texture format/mips, dung lượng source/build và memory cần đo. Không dùng ảnh portrait lớn cho icon nhỏ. Định budget theo asset role và thiết bị mục tiêu; JPEG nhẹ không chứng minh texture RAM nhẹ.

## Thứ tự slice và tiêu chí mở

### Kịch bản nối hệ thống, chưa phải nhiệm vụ đã triển khai

| Chặng | Người chơi cần hiểu / thực hiện | NPC và hệ thống tham gia | Kết quả phải nhìn thấy | Điều kiện trước implementation |
|---|---|---|---|---|
| Đến thành | Mình là người mới đến, đang ở đâu, gặp ai | Người Giữ Cổng, chỉ dẫn và dialogue chung | Nhận ra NPC, tiếp cận, đọc/đóng/mở lại, tìm đúng điểm tương tác | Demo đường đi và hội thoại; không khẳng định city đã tồn tại |
| Hiểu Lộ | Kiếm và Võ khác nhau qua hành động, không chỉ chỉ số | Người hướng dẫn, lựa chọn thử và combat fixture | Thử một tình huống né/đánh hoặc phản đòn; quay lại so sánh được | Demo hai tình huống, animation, luật combat và điều kiện mở Lộ; chưa khóa lựa chọn vĩnh viễn |
| Ra ngoài | Dùng điều vừa học trong mục tiêu có thể thắng/thua | NPC giao việc, field, mục tiêu chiến đấu | Đọc nguy hiểm, thực hiện mục tiêu, biết vì sao thành công/thất bại | Encounter/map demo và gate combat; không tự phát item/EXP ở client |
| Trở về | Kết quả chuyến đi thay đổi build ra sao | NPC hậu cần, hành trang và trang bị | So sánh một món đồ với món đang mặc, thay đúng slot, thấy thay đổi thật | Quyền sở hữu, giao dịch dữ liệu và rollback; không mở crafting/shop giả |
| Tạo bản sắc | Ngoại hình là lựa chọn cá nhân, không mua sức mạnh | Thợ may, trang phục và preview nhân vật | Thử một bộ, hủy về bộ cũ hoặc áp dụng; chỉ số không đổi | Demo trước/sau trên các tư thế, asset tương thích, luật sở hữu được chốt |
| Bảo vệ thành | Những người từng gặp phản ứng với nguy hiểm | Cư dân, Người Giữ Cổng, event dùng chung | Nhận biết cảnh báo, vai trò của mình và trạng thái phục hồi | Chỉ mở khi roadmap event cho phép; không dùng timer client thay trạng thái server |

Mỗi chặng phải dẫn trở lại vòng city -> field -> city. Không thêm sáu quầy NPC cùng lúc; chỉ sản xuất NPC/asset cho chặng đang làm. Đá Luyện hiện chỉ kiểm chứng tương tác, không được coi là đã chứng minh combat hoặc lựa chọn Lộ.

### Ranh giới trách nhiệm dự kiến

- Nội dung NPC: danh tính, hội thoại và điều kiện tham gia; không tự sửa inventory hay cấp thưởng trong callback UI.
- Tiến trình nhân vật: khi được mở, phía có thẩm quyền quyết định bước đã hoàn tất. UI chỉ trình bày trạng thái; đóng modal không đồng nghĩa hoàn thành.
- Combat: kiểm tra điều kiện dùng skill và kết quả tác động; icon/VFX thể hiện kết quả hoặc dự đoán có quy tắc, không tự quyết định damage.
- Trang bị/trang phục: lưu riêng vật phẩm đang trang bị và lựa chọn ngoại hình; preview tạm không được ghi vào hồ sơ trước xác nhận thành công.
- Trạng thái city/event: dùng chung cho người trong vùng, tách khỏi thoại/tiến trình riêng. Chưa thêm service, schema hoặc controller mới chỉ từ bản thiết kế này.

Trước khi thêm nội dung cho một chặng: cần một bộ demo đủ trạng thái, danh sách asset có kích thước hiển thị thật và một tiêu chí nghiệm thu chơi được. Nếu chỉ có concept tổng quan, đánh dấu thiếu design chi tiết; không lấy concept làm bằng chứng hệ thống đã sẵn sàng.

1. Chốt demo Người Giữ Cổng + nhập môn: lý do xuất hiện, thoại, đường đi, action và kết thúc. Chỉ tái sử dụng hành vi local đã có sau khi demo được duyệt; không tuyên bố đã có Linh Thành.
2. Hoàn thiện vòng đến/gặp/tương tác/rời đi với cùng base UI; kiểm chứng input, dialogue dài, đổi kích thước và mục tiêu rõ. Người chơi phải hiểu đang ở đâu và làm gì tiếp.
3. Thiết kế rồi triển khai thử một lựa chọn chiến đấu Kiếm/Võ khi gate cho phép; xác nhận vai trò kỹ năng, animation và thẩm quyền trước progression hoặc thêm Lộ.
4. Thiết kế lát hành trang/trang bị/trang phục nhỏ, rồi chỉ mở implementation sau quyết định ownership/data/migration. Một lần thay đồ phải có kết quả thật và không mất dữ liệu.
5. Mở field -> về city và NPC hậu cần theo milestone; sau đó mới mở event có cảnh báo/phòng thủ/phục hồi. Không lấy NPC quầy hàng làm lý do mở economy/social sớm.

Mỗi chặng có demo trước, source sau, focused test và ảnh runtime cùng state để đối chiếu. Nếu chưa có design đủ rõ, làm rõ đúng slice đó; không sinh hàng loạt ảnh/tài liệu không phục vụ bước kế tiếp.

## Tham khảo ngoài, không thay thế kịch bản gốc

- [ArenaNet: Introducing the Wardrobe System](https://www.guildwars2.com/en/news/introducing-the-wardrobe-system/): tham khảo việc quản lý skin/ngoại hình. Áp dụng đề xuất tách ngoại hình khỏi chỉ số; không sao chép phí, currency hoặc account unlock vào Linh Giới.
- [FFXIV Game Manual: Navigating the Game Screen](https://na.finalfantasyxiv.com/game_manual/view/): hướng dẫn hiển thị nhiệm vụ cốt truyện và class/job tiếp theo. Bài học đề xuất: chỉ rõ bước truyện/Lộ kế tiếp, không biến HUD thành danh sách menu không có mục tiêu.
- [ArenaNet: Designing the Living World](https://www.guildwars2.com/en/news/designing-the-living-world/): phân biệt vai trò thế giới mở và tuyến cá nhân. Áp dụng đề xuất tách tiến trình nhập môn riêng khỏi trạng thái xâm lăng chung.

Các áp dụng trên là suy luận thiết kế cho dự án, không chứng minh runtime Linh Giới đã có các hệ thống đó. Không thay frozen contract hoặc tự đóng milestone trong batch này.
