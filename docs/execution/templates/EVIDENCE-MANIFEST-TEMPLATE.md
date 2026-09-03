# Evidence Manifest Template

Use this as Markdown or convert to JSON/YAML when a task needs machine-readable evidence.

```yaml
task_id:
source_baseline:
source_sha256:
runtime:
  os:
  java:
  maven:
  unity:
  node:
  python:
commands:
  - id:
    cwd:
    command:
    started_at_utc:
    finished_at_utc:
    exit_code:
    executed_count:
    result:
    log:
artifacts:
  - path:
    sha256:
    purpose:
limitations:
  - gate:
    reason:
    result: UNVERIFIED_ENVIRONMENT
next_allowed_step:
```
