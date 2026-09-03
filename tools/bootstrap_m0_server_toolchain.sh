#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
# shellcheck disable=SC1091
source "$ROOT/tools/runtime/toolchain.env"
TOOLCHAIN_ROOT="${M0_TOOLCHAIN_ROOT:-$ROOT/.toolchains}"
DOWNLOAD_ROOT="$TOOLCHAIN_ROOT/downloads"
JDK_HOME="$TOOLCHAIN_ROOT/temurin-${M0_JDK_VERSION}"
MAVEN_HOME="$TOOLCHAIN_ROOT/apache-maven-${M0_MAVEN_VERSION}"
mkdir -p "$DOWNLOAD_ROOT"

fail(){ printf 'M0 TOOLCHAIN BOOTSTRAP FAILED: %s\n' "$*" >&2; exit 2; }
fetch(){
  local url="$1" out="$2"
  if [[ -s "$out" ]]; then return 0; fi
  if command -v curl >/dev/null 2>&1; then
    curl --fail --location --retry 2 --connect-timeout 15 --output "$out.part" "$url" || { rm -f "$out.part"; fail "download failed: $url"; }
  elif command -v wget >/dev/null 2>&1; then
    wget -O "$out.part" "$url" || { rm -f "$out.part"; fail "download failed: $url"; }
  else
    fail 'curl or wget is required'
  fi
  mv "$out.part" "$out"
}

[[ "$(uname -s)" == Linux && "$(uname -m)" == x86_64 ]] || fail 'automatic bootstrap currently supports Linux x86_64 only'

jdk_archive="${M0_JDK_ARCHIVE_PATH:-$DOWNLOAD_ROOT/$M0_JDK_ARCHIVE}"
jdk_sha_file="$DOWNLOAD_ROOT/$M0_JDK_ARCHIVE.sha256.txt"
if [[ -z "${M0_JDK_ARCHIVE_PATH:-}" ]]; then fetch "$M0_JDK_URL" "$jdk_archive"; fi
if [[ -n "${M0_JDK_SHA256:-}" ]]; then
  expected_jdk_sha="$M0_JDK_SHA256"
else
  fetch "$M0_JDK_SHA256_URL" "$jdk_sha_file"
  expected_jdk_sha="$(awk 'NR==1 {print $1}' "$jdk_sha_file")"
fi
[[ "$expected_jdk_sha" =~ ^[0-9a-fA-F]{64}$ ]] || fail 'invalid JDK SHA256 evidence'
echo "$expected_jdk_sha  $jdk_archive" | sha256sum -c - >/dev/null || fail 'JDK SHA256 mismatch'

maven_archive="${M0_MAVEN_ARCHIVE_PATH:-$DOWNLOAD_ROOT/$M0_MAVEN_ARCHIVE}"
if [[ -z "${M0_MAVEN_ARCHIVE_PATH:-}" ]]; then fetch "$M0_MAVEN_URL" "$maven_archive"; fi
echo "$M0_MAVEN_SHA512  $maven_archive" | sha512sum -c - >/dev/null || fail 'Maven SHA512 mismatch'

if [[ ! -x "$JDK_HOME/bin/java" ]]; then
  tmp="$TOOLCHAIN_ROOT/.jdk-extract-$$"; rm -rf "$tmp"; mkdir -p "$tmp"
  tar -xzf "$jdk_archive" -C "$tmp"
  extracted="$(find "$tmp" -mindepth 1 -maxdepth 1 -type d | head -1)"
  [[ -n "$extracted" ]] || fail 'JDK archive did not contain an extracted directory'
  rm -rf "$JDK_HOME"; mv "$extracted" "$JDK_HOME"; rm -rf "$tmp"
fi
if [[ ! -x "$MAVEN_HOME/bin/mvn" ]]; then
  tmp="$TOOLCHAIN_ROOT/.maven-extract-$$"; rm -rf "$tmp"; mkdir -p "$tmp"
  tar -xzf "$maven_archive" -C "$tmp"
  extracted="$(find "$tmp" -mindepth 1 -maxdepth 1 -type d | head -1)"
  [[ -n "$extracted" ]] || fail 'Maven archive did not contain an extracted directory'
  rm -rf "$MAVEN_HOME"; mv "$extracted" "$MAVEN_HOME"; rm -rf "$tmp"
fi

JAVA_HOME="$JDK_HOME" PATH="$JDK_HOME/bin:$MAVEN_HOME/bin:$PATH" java --version
JAVA_HOME="$JDK_HOME" PATH="$JDK_HOME/bin:$MAVEN_HOME/bin:$PATH" javac --version
JAVA_HOME="$JDK_HOME" PATH="$JDK_HOME/bin:$MAVEN_HOME/bin:$PATH" mvn --version
printf 'M0_SERVER_TOOLCHAIN_READY JAVA_HOME=%s MAVEN_HOME=%s\n' "$JDK_HOME" "$MAVEN_HOME"
