#!/bin/sh
# One-time setup for the Docker stack: provides a trusted localhost certificate in src/certs/ (git-ignored),
# which docker-compose.override.yml mounts into the API so it can serve HTTPS on 8081. Re-run it when the
# certificate expires.
set -e

# Resolved from the script's own location (resources/Init), so it works from any working directory.
dir="$(cd "$(dirname "$0")" && pwd)"
certs="$(cd "$dir/../.." && pwd)/src/certs"
pem="$certs/timehacker.pem"

# Git Bash / MSYS on Windows: the Windows trust store and Docker path handling belong to the PowerShell script.
case "$(uname -s)" in
  MINGW*|MSYS*|CYGWIN*) exec powershell.exe -NoProfile -ExecutionPolicy Bypass -File "$(cygpath -w "$dir/setup-dev-certs.ps1")" ;;
esac

mkdir -p "$certs"

# `dotnet` alone is not enough: dev-certs is an SDK command, and a runtime-only install has no SDKs.
if command -v dotnet >/dev/null 2>&1 && [ -n "$(dotnet --list-sdks 2>/dev/null)" ]; then
  # Reuses (or creates) the machine's dev certificate, so `dotnet run` serves the same trusted one. Trust is
  # only partially supported on Linux, so a failure there is a warning rather than an error.
  if ! dotnet dev-certs https --trust; then
    echo "warning: could not trust the certificate; open https://localhost:8081 once and accept it" >&2
  fi

  # PEM without a password, so compose needs no secret: produces timehacker.pem and timehacker.key.
  dotnet dev-certs https --export-path "$pem" --format PEM --no-password
else
  # No .NET SDK: generate the certificate in the SDK image instead (Docker is required for the stack
  # anyway), keeping a still-valid one so re-runs do not pile up trusted certificates. Running as the
  # host user keeps the files owned by (and chmod-able for) them.
  if ! { [ -f "$pem" ] && command -v openssl >/dev/null 2>&1 && openssl x509 -checkend 2592000 -noout -in "$pem" >/dev/null; }; then
    docker run --rm --user "$(id -u):$(id -g)" -e HOME=/tmp -e DOTNET_NOLOGO=1 -v "$certs:/certs" \
      mcr.microsoft.com/dotnet/sdk:10.0 \
      sh -c 'dotnet dev-certs https --export-path /certs/timehacker.pem --format PEM --no-password && chmod 644 /certs/timehacker.pem /certs/timehacker.key'
  fi

  case "$(uname -s)" in
    # Adds it to the login keychain; macOS asks for the user's password.
    Darwin) security add-trusted-cert -r trustRoot -k "$HOME/Library/Keychains/login.keychain-db" "$pem" ;;
    *) echo "warning: trust $pem manually (e.g. copy it to /usr/local/share/ca-certificates/ as .crt and run" \
            "update-ca-certificates, then import it in the browser), or open https://localhost:8081 once and accept it" >&2 ;;
  esac
fi

# The API runs as a non-root user inside the container and must be able to read the key.
chmod 644 "$pem" "$certs/timehacker.key"

echo "Development certificate ready in $certs"
