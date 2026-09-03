#!/usr/bin/env bash
set -u

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LOG="${ROOT}/local-macos-setup-$(date +%Y%m%d-%H%M%S).log"

say() { printf '%s\n' "$*"; }
fail() { say "ERROR: $*"; say "log=$LOG"; return 1; }
run() { say "+ $*"; "$@"; }

choose_python() {
  if command -v python3.12 >/dev/null 2>&1; then
    command -v python3.12
    return 0
  fi
  if command -v brew >/dev/null 2>&1; then
    local py312_prefix
    py312_prefix="$(brew --prefix python@3.12 2>/dev/null || true)"
    if [ -n "$py312_prefix" ] && [ -x "$py312_prefix/bin/python3.12" ]; then
      printf '%s\n' "$py312_prefix/bin/python3.12"
      return 0
    fi
  fi
  if command -v python3 >/dev/null 2>&1; then
    command -v python3
    return 0
  fi
  return 1
}

main() {
  say "== Linh Gioi Online local macOS setup =="
  say "ROOT=$ROOT"
  say "LOG=$LOG"

  if ! command -v brew >/dev/null 2>&1; then
    fail "Homebrew is required for coreutils timeout. Install Homebrew, then run: brew install coreutils python@3.12" || return 1
  fi

  local coreutils_prefix
  coreutils_prefix="$(brew --prefix coreutils 2>/dev/null || true)"
  if [ -z "$coreutils_prefix" ] || [ ! -x "$coreutils_prefix/libexec/gnubin/timeout" ]; then
    fail "coreutils is missing. Run: brew install coreutils" || return 1
  fi

  mkdir -p "$HOME/.local/bin"
  ln -sf "$coreutils_prefix/libexec/gnubin/timeout" "$HOME/.local/bin/timeout"
  export PATH="$HOME/.local/bin:$coreutils_prefix/libexec/gnubin:$PATH"
  hash -r

  cd "$ROOT" || return 1

  say
  say "== Python runner =="
  local py312_prefix python_runner
  py312_prefix="$(brew --prefix python@3.12 2>/dev/null || true)"
  if [ -n "$py312_prefix" ] && [ -x "$py312_prefix/bin/python3.12" ]; then
    export PATH="$py312_prefix/libexec/bin:$py312_prefix/bin:$PATH"
  fi
  python_runner="$(choose_python)" || { fail "No Python runner found. Install: brew install python@3.12"; return 1; }
  say "PYTHON_RUNNER=$python_runner"
  "$python_runner" --version || return 1

  say
  say "== Local dependency mode =="
  say "M2 validators can run with external PyYAML/jsonschema when available, or with the repo stdlib fallback when pip/Homebrew Python is broken. No pip install is required for source validation."
  "$python_runner" - <<'PY' || return 1
import importlib.util
print("yaml_available=", importlib.util.find_spec("yaml") is not None)
print("jsonschema_available=", importlib.util.find_spec("jsonschema") is not None)
PY

  say
  say "== timeout =="
  timeout --version | head -n 1 || return 1

  say
  say "== Pinned protoc 3.13.0 =="
  if [ -n "${PROTOC_BIN:-}" ] && [ -x "${PROTOC_BIN:-}" ]; then
    say "Using existing PROTOC_BIN=$PROTOC_BIN"
  else
    PROTOC_BIN="$(find "$ROOT/.toolchains" "$HOME/.local/lgo-toolchains" "$HOME/lgo-unity-player-evidence-v0312" "$HOME/lgo-unity-player-evidence-v0313" -type f -path '*protoc-3.13.0-osx-x86_64/bin/protoc' 2>/dev/null | head -n 1)"
  fi

  if [ -z "${PROTOC_BIN:-}" ]; then
    local tool_dir zip_file
    tool_dir="$ROOT/.toolchains/protoc-3.13.0-osx-x86_64"
    zip_file="$ROOT/.toolchains/protoc-3.13.0-osx-x86_64.zip"
    mkdir -p "$ROOT/.toolchains"
    rm -rf "$tool_dir" "$zip_file"
    run curl -L -o "$zip_file" "https://github.com/protocolbuffers/protobuf/releases/download/v3.13.0/protoc-3.13.0-osx-x86_64.zip" || return 1
    mkdir -p "$tool_dir"
    run unzip -q "$zip_file" -d "$tool_dir" || return 1
    PROTOC_BIN="$tool_dir/bin/protoc"
  fi

  PROTOC_SHA256="$(shasum -a 256 "$PROTOC_BIN" | awk '{print $1}')"
  export PROTOC_BIN
  export PROTOC_SHA256

  say "PROTOC_BIN=$PROTOC_BIN"
  say "PROTOC_SHA256=$PROTOC_SHA256"
  "$PROTOC_BIN" --version || return 1

  if [ "$PROTOC_SHA256" != "397cf42b9c11b9ad4e49f40310c6c0deaba2ceec6e97ab2ecaa5a1bd58db0429" ]; then
    fail "protoc SHA256 mismatch" || return 1
  fi
  if [ "$("$PROTOC_BIN" --version)" != "libprotoc 3.13.0" ]; then
    fail "protoc version mismatch" || return 1
  fi

  local python_dir python_path_prefix
  python_dir="$(dirname "$python_runner")"
  python_path_prefix="$python_dir"
  if [ -n "${py312_prefix:-}" ]; then
    python_path_prefix="$py312_prefix/libexec/bin:$py312_prefix/bin:$python_path_prefix"
  fi
  cat > "$ROOT/.lgo-local-env" <<ENVEOF
export PATH="$python_path_prefix:$HOME/.local/bin:$coreutils_prefix/libexec/gnubin:\$PATH"
export PROTOC_BIN="$PROTOC_BIN"
export PROTOC_SHA256="$PROTOC_SHA256"
ENVEOF

  say
  say "== chmod scripts =="
  find tools -type f -name '*.sh' -exec chmod +x {} \;

  say
  say "== Run M2 source validation =="
  source "$ROOT/.lgo-local-env"
  ./tools/validate_m2_source.sh || return 1

  say
  say "== LOCAL_SETUP_PASS =="
  say "To reuse this terminal setup later: source \"$ROOT/.lgo-local-env\""
}

main 2>&1 | tee "$LOG"
rc=${PIPESTATUS[0]:-0}
printf '\nexit_code=%s\nlog=%s\n' "$rc" "$LOG"
exit "$rc"
