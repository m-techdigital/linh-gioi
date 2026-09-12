# Linh Gioi NPC/Dialogue Content Pipeline v1.0

Marker: `LGO_DIALOGUE_PIPELINE_READY`

## Purpose

Dialogue content should be authored with stable ids, Vietnamese runtime copy, and validation before it becomes production data.

## Planning Format

Planning-only dialogue entries should include:

- dialogue id;
- NPC id;
- zone id;
- Vietnamese player-facing line;
- trigger condition;
- next objective hint;
- tone tag;
- implementation status;
- reviewer note.

## Id Pattern

```text
dialogue.<npc_id>.<scene_or_state>
```

Examples:

- `dialogue.gate_keeper.intro`
- `dialogue.gate_keeper.training_hint`
- `dialogue.training_stone.pulse_ready`

## Runtime Copy Rules

- Player-facing dialogue must be Vietnamese.
- Do not bake dialogue text into images.
- Keep tutorial text short enough for mobile UI.
- Avoid lore dumps in runtime prompts.
- Keep local prototype lines separate from future production story review.

## Pipeline Stages

| Stage | Purpose | Output |
|---|---|---|
| `draft` | rough Vietnamese content | markdown/table draft |
| `reviewed` | tone/readability pass | reviewed text |
| `runtime_candidate` | ready for local UI wiring | source-owned config or code constant when approved |
| `production_candidate` | ready for schema/DB review | contract-change request |

## Future Entry Criteria

Before implementing a production dialogue pipeline:

- decide whether dialogue belongs in GameData, localization files, or DB;
- create contract-change request if schemas must change;
- add positive/negative validation;
- add runtime smoke for visible Vietnamese copy;
- add reviewer workflow for lore/tone.

## Map01A — luồng hội thoại dùng chung, 2026-09-12

Theo yêu cầu owner, bám sheet `map-01a-cong-dong-lam/05-dialogue-and-quest-flow.png` cho trình tự nói chuyện/lựa chọn và contract Q01–Q09 hiện hành cho quest/reward. Không đưa số EXP/vàng/quái minh họa trên board vào runtime khi contract chưa có. Dùng `NpcDialogueSession` hiện có để giữ chung cách đọc trang, hỏi thêm, hoàn tất và đóng.

Hạ Vân, Quan Thủ, Tổng Phú, Thanh Nhi, Tiểu Đồng, Lão Trần phải nói chuyện được khi quay lại. Mỗi NPC có lời giới thiệu/nhận việc, lời nhắc đang làm và lời sau hoàn tất; chỉ action nhận việc cuối hội thoại mới đổi quest hoặc nhận quà, đóng sớm không nhận. Tổng Phú không cấp lại tiếp tế. Tiểu Đồng dùng một lựa chọn nói chuyện bên cạnh tương tác hái/rương tại cùng khu vực. Hỏi đường/việc tiếp theo không làm tiến triển nhiệm vụ. UI giữ panel thoại hiện có, hiện số trang và các lựa chọn chữ Việt; kiểm cả nội dung và thao tác trên Player.
