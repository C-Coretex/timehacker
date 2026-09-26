# One-time setup for the Docker stack: provides a trusted localhost certificate in src/certs/ (git-ignored),
# which docker-compose.override.yml mounts into the API so it can serve HTTPS on 8081. Re-run it when the
# certificate expires.
$ErrorActionPreference = 'Stop'

# Resolved from the script's own location (resources/Init), so it works from any working directory.
# GetFullPath drops the `..` segments, which Docker would otherwise receive in the bind-mount path.
$certs = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\src\certs'))
$pem = Join-Path $certs 'timehacker.pem'
New-Item -ItemType Directory -Force $certs | Out-Null

# `dotnet` alone is not enough: dev-certs is an SDK command, and a runtime-only install has no SDKs.
$hasSdk = (Get-Command dotnet -ErrorAction SilentlyContinue) -and (dotnet --list-sdks)

if ($hasSdk) {
    # Reuses (or creates) the machine's dev certificate, so VS and `dotnet run` serve the same trusted one.
    # Windows asks for confirmation the first time it is trusted.
    dotnet dev-certs https --trust
    if ($LASTEXITCODE -ne 0) { throw 'Trusting the development certificate failed or was declined; the browser will reject the API.' }

    # PEM without a password, so compose needs no secret: produces timehacker.pem and timehacker.key.
    dotnet dev-certs https --export-path $pem --format PEM --no-password
    if ($LASTEXITCODE -ne 0) { throw 'Exporting the development certificate failed.' }
}
else {
    # No .NET SDK: generate the certificate in the SDK image instead (Docker is required for the stack
    # anyway), keeping a still-valid one so re-runs do not pile up trusted certificates. dev-certs writes
    # the files as root with mode 600, which the bind mount preserves and the API's non-root user cannot read.
    $valid = (Test-Path $pem) -and ((New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 $pem).NotAfter -gt (Get-Date).AddDays(30))
    if (-not $valid) {
        docker run --rm -e DOTNET_NOLOGO=1 -v "${certs}:/certs" mcr.microsoft.com/dotnet/sdk:10.0 `
            sh -c 'dotnet dev-certs https --export-path /certs/timehacker.pem --format PEM --no-password && chmod 644 /certs/timehacker.pem /certs/timehacker.key'
        if ($LASTEXITCODE -ne 0) { throw 'Generating the development certificate in Docker failed. Is Docker running?' }
    }

    # Windows asks for confirmation before adding a trusted root.
    Import-Certificate -FilePath $pem -CertStoreLocation Cert:\CurrentUser\Root | Out-Null
}

Write-Host "Development certificate ready in $certs"
