# LGO rigid joint and layered outfit authoring contract v1

Status: **FORMULA READY / BODY BIND CALIBRATION REQUIRED / RIG LOCKED**.

Tài liệu này khóa cách thiết kế body và outfit cho pilot rigid. Nó không công nhận ảnh composite v5.2 là source, không mở outfit #2/class/level và không thay visual review trong Player.

## Kết luận từ nguồn và audit dự án

- Unity tính position, rotation và scale của Transform con tương đối với Transform cha. Vì vậy một `SpriteRenderer` cứng đặt dưới bone có thể đi theo bone mà không cần đổi hình học: <https://docs.unity3d.com/2022.3/Documentation/Manual/class-Transform.html>.
- Unity mô tả `SpriteSkin` là component làm biến dạng sprite. Nó không thuộc pipeline này: <https://docs.unity3d.com/Packages/com.unity.2d.animation@10.0/api/UnityEngine.U2D.Animation.SpriteSkin.html>.
- Unity `SortingGroup` giữ các renderer của một nhân vật nhiều sprite trong một nhóm và vẫn cho phép order riêng bên trong nhóm: <https://docs.unity3d.com/2022.3/Documentation/Manual/class-SortingGroup.html>.
- Spine ghi rõ mỗi phần chuyển động độc lập cần một file ảnh riêng; slot gắn vào bone và draw order tách khỏi bone. Đây là đối chứng authoring, không phải lệnh đổi runtime sang Spine: <https://us.esotericsoftware.com/spine-images>, <https://us.esotericsoftware.com/spine-slots>.
- Spine Tips về chuẩn bị cutout yêu cầu vẽ cả phần bị che, bo tròn đầu mảnh tại joint, tách front/back thành layer riêng và giữ outline ở hai phía vùng giao để phần xoay vẫn đọc liền. Đây là nguồn trực tiếp củng cố lựa chọn cap/underlap: <https://esotericsoftware.com/blog/2019/1>.

Các nguồn trên xác nhận cấu trúc transform/attachment/draw-order. Công thức cap tròn dưới đây là thiết kế kỹ thuật của LGO để đáp ứng ràng buộc rigid và được kiểm bằng alpha sweep; đây không phải quy tắc được Unity tự bảo đảm.

Audit source cũ phát hiện `tools/build_lgo_rigid_pilot_source_v2.py` cắt alpha theo một đường ngang và khai báo overlap chữ nhật. Cách đó không chứng minh vùng quanh pivot kín khi sprite xoay. Output `rigid-source-v2` chỉ được giữ làm evidence của method/runtime cũ, không được dùng làm source authority mới.

## Một joint có hai contract độc lập

Mỗi joint dùng đúng **một pivot** `P` chung cho bone, body và mọi outfit tương thích.

### Body contract — luôn hoàn chỉnh khi không mặc đồ

1. Parent body kéo qua `P` thành vùng underlap.
2. Child body chứa cap tròn tâm `P`, render trên parent ở cùng depth chain.
3. Cap nối liền với shaft của child; mép bị chồng không có contour nội bộ.
4. Body-only phải kín và có silhouette tự nhiên trên toàn safe rotation range.
5. Equipment không chứa da để vá body. Tắt toàn bộ equipment không được làm mất pixel body.

Ví dụ: upper arm đi vào socket vai của torso; lower arm có cap khuỷu phủ đầu upper arm; hand có cap cổ tay phủ forearm. Pelvis–thigh, thigh–shin và shin–foot dùng cùng quy tắc.

### Outfit contract — cover riêng, không thay body contract

1. Mỗi visual part thuộc đúng một bone và giữ một form duy nhất.
2. Parent garment có hidden underlap quanh `P`.
3. Child garment có cover/cap tròn quanh cùng `P`, render trên parent garment.
4. Thiết kế seam-cover tự nhiên: shoulder cap, bracer lip, glove cuff, belt, tunic side panel, knee guard, boot cuff.
5. Một sprite không được bắc qua hai bone chuyển động tương đối. Nếu silhouette cần hai bone thì tách tại interface hoặc `ASSET_SEGMENTATION_REQUIRED`.

Body và outfit chồng alpha tại cùng tọa độ là đúng. Pixel da nằm trong file equipment là sai.

## Công thức hình học

Mọi số đo nằm trong common canvas `1024×1536`, origin X `512`, ground Y `1484`. Không đo theo crop/atlas và không normalize từng item.

Với một joint:

```text
P = pivot source-pixel đã đăng ký
Wb = bề rộng body vuông góc trục xương tại P
Wg = bề rộng garment vuông góc trục xương tại P
O  = bề dày outline nguồn
F  = guard cho bilinear/downsample
T  = sai số registration cho phép
C  = khoảng hở vải so với da

M  = O + F + T
Rb = ceil(Wb / 2 + M)
Rg = ceil(max(Wg / 2, Wb / 2 + C) + M)
```

- Child body phải chứa trọn đĩa `B(P,Rb)` và parent body phải underlap qua pivot ít nhất `Rb` theo trục nối.
- Child garment phải chứa trọn cover `B(P,Rg)` và parent garment phải underlap qua pivot ít nhất `Rg`.
- Hình tròn tâm pivot bất biến dưới phép quay, nên vùng che trung tâm không phụ thuộc góc. Điều này chỉ bảo đảm không hở pivot; alpha sweep vẫn phải bắt clipping với torso, tà, tay/chân khác.
- Mặc định pilot dùng `O=3 px`, `F=2 px`, `T=2 px`, sample mỗi `5°`. Đây là source-resolution margin, không phải offset runtime.
- Rotation range lấy từ bộ pose/motion authority sau khi fit skeleton; không suy từ bbox frame. Sample count:

```text
N = ceil((thetaMax - thetaMin) / 5°) + 1
```

Fit runtime vẫn dùng công thức source-space chung:

```text
u = 1.70 / 1536
worldPoint(x,y) = ((x-512)*u, (1484-y)*u)
localAttachment = worldPoint(P) - boneBindPivot
localScale = (1,1,1)
```

`P` chỉ được author một lần trong body bind profile. Mọi item dùng lại đúng `P`; item không có pivot, scale hoặc offset theo animation.

## Chuỗi body và lớp render cố định

Mỗi depth chain render parent trước, child sau để cap child phủ seam parent:

```text
FAR BODY:    torso/pelvis → upper limb → lower limb → hand/foot
FAR OUTFIT:  torso/lower garment → upper cover → lower cover → glove/boot
CORE BODY:   pelvis → torso → neck/head
CORE OUTFIT: inner → lower body → outer torso → belt → chest guard
NEAR BODY:   upper limb → lower limb → hand/foot
NEAR OUTFIT: upper cover → lower cover → glove/boot
HAIR/HEADGEAR/WEAPON/FX: canonical roles riêng
```

Một gameplay item có thể sở hữu nhiều visual part. Equip/unequip diễn ra theo bundle/slot; animation không đổi slot, sprite hoặc order. Tà dài, tóc dài và ribbon nếu có phải là chuỗi segment cứng, mỗi segment một bone phụ và một sprite; pilot đầu chưa mở physics.

## Source package bắt buộc

Layered source phải mở lại được và chứa hai nhánh tách biệt:

```text
BODY_MALE / BODY_FEMALE
  head, neck, torso, pelvis
  upper_arm_near/far, lower_arm_near/far, hand_near/far
  upper_leg_near/far, lower_leg_near/far, foot_near/far

OUTFIT_SLOT
  torso/collar
  shoulder + upper_sleeve near/far
  bracer near/far
  glove near/far
  lower_body + belt + tunic panels
  knee_guard + boot_shaft + boot_foot near/far
```

Mỗi layer xuất PNG RGBA trên common canvas hoặc giữ `sourceCanvasRect` đầy đủ. Trim chỉ bỏ pixel rỗng. Mỗi layer có `partId`, `targetBone`, `pivotPx`, `sortRole`, `sourceHash` và interface ID. Không crop ảnh composite thành part vì phần bị che không tồn tại trong ảnh đó.

## Gate theo đúng thứ tự để tránh redesign lặp

1. **Bind silhouette:** duyệt góc nhìn, tỷ lệ, trục người, vị trí pivot và đường may/cuff/belt. Chưa duyệt thì không vẽ layer chi tiết.
2. **Calibrate body:** đo `P`, `Wb` một lần trên body-only source; chạy tool sinh cap/overlay guide. Profile này được version theo body, không theo item.
3. **Body-only sweep:** sample toàn ROM. Không có background hole, chi rời, bump tròn lộ rõ hoặc contour nội bộ.
4. **Author outfit từ interface guide:** đo `Wg`, dùng cùng `P`, vẽ đầy đủ phần bị che. Không lấy body pixel.
5. **Unequip proof:** `body_only`, `all_on`, tắt từng slot và các cặp seam-critical. Body phải còn nguyên; item còn lại không phụ thuộc item vừa tắt.
6. **Motion key proof:** 15 key Player cho Idle/Walk/Run/Jump/Attack/Roll. Chỉ khi không còn lỗi cấu trúc mới render combo đầy đủ.
7. **Player review:** review liên tục ở gameplay speed. Machine PASS chỉ mở visual review.

Không cần thử toàn bộ `2^n` loadout. Phải test body-only, mỗi slot-off và các cặp giao nhau tại cổ/vai, eo/hông, cổ tay, gối/cổ chân. Khi thêm item mới, item dùng lại body interface profile; chỉ author hình dáng ngoài và `Wg` của chính item.

## Stop/reject rule

- Body-only hở: `BODY_CAP_OR_UNDERLAP_FIX_REQUIRED`.
- Body có cục tròn/lộ đường trong khi xoay: `BODY_SILHOUETTE_REAUTHOR_REQUIRED`.
- Outfit hở tại joint: `OUTFIT_COVER_OR_UNDERLAP_FIX_REQUIRED`.
- Một ảnh cần đi theo hai bone: `ASSET_SEGMENTATION_REQUIRED`.
- Part va vào torso/part khác trong safe ROM dù pivot kín: `SKELETON_RANGE_REVIEW_REQUIRED` hoặc `ASSET_REAUTHOR_REQUIRED`.
- Cùng lỗi cấu trúc xuất hiện sau hai lượt source: bác topology đó, không tăng overlap hoặc chỉnh pixel tiếp.

Tool công thức: `tools/build_lgo_rigid_joint_authoring_template.py`. Alpha gate: `tools/audit_lgo_rigid_joint_alpha_sweep.py`; gate quay child PNG thật quanh pivot, đo coverage trong `R - filterGuard` và xuất contact sheet. Control evidence tại `build/rigid-outfit-pilot/joint-alpha-sweep-control-v1`: cap tròn đạt `miss=0` từ `-150°..150°`, horizontal/rectangular split bị reject với pixel thiếu ở mọi frame mẫu.

Profile hiện hành tại `docs/art/data/lgo-rigid-joint-interface-profile-v1.json` cố ý mang trạng thái `FORMULA_PROFILE_AWAITING_BODY_BIND_CALIBRATION`; các pivot/width trong đó chỉ là layout target để review công thức, chưa được dùng rig. Alpha gate chỉ chứng minh coverage hình học; contour tự nhiên, va chạm silhouette và layer xuyên sai vẫn cần review trực tiếp.
