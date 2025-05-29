# ----- Configuration -----
$AppId = "1333160"
$PrivateKeyPath = ".\private-key.pem"
# -------------------------

# Prerequisites check
if (-not (Get-Command 'openssl' -ErrorAction SilentlyContinue)) {
    Write-Error "openssl is required but not installed. Please install OpenSSL."
    exit 1
}

# Step 1: Generate JWT
Write-Host "[INFO] Generating JWT..."

$iat = [int](Get-Date -UFormat %s)
$exp = $iat + 540
$header = '{"alg":"RS256","typ":"JWT"}'
$payload = "{""iat"":$iat,""exp"":$exp,""iss"":""$AppId""}"

function EncodeBase64Url([string]$input) {
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($input)
    [Convert]::ToBase64String($bytes) -replace '\+','-' -replace '/','_' -replace '='
}

$headerEncoded = EncodeBase64Url $header
$payloadEncoded = EncodeBase64Url $payload
$unsignedToken = "$headerEncoded.$payloadEncoded"

# Write the unsigned token to a temporary file
$tmpFile = [System.IO.Path]::GetTempFileName()
[System.IO.File]::WriteAllText($tmpFile, $unsignedToken)

# Sign the token using openssl
$signature = & openssl dgst -sha256 -sign $PrivateKeyPath -binary $tmpFile | openssl base64 -A
$signatureEncoded = $signature -replace '\+','-' -replace '/','_' -replace '='
Remove-Item $tmpFile

# Construct final JWT
$jwt = "$unsignedToken.$signatureEncoded"

Write-Host "[SUCCESS] JWT created."
Write-Host "`nJWT:"
Write-Output $jwt

# Optional: test it
Write-Host "[INFO] Testing JWT with https://api.github.com/app..."
Invoke-RestMethod -Headers @{ Authorization = "Bearer $jwt" } -Uri "https://api.github.com/app"


# Step 2: Get installation ID
Write-Host "[INFO] Getting installation ID for $Org..."
$installationResponse = Invoke-RestMethod -Headers @{Authorization = "Bearer $jwt"; Accept = "application/vnd.github+json" } `
    -Uri "https://api.github.com/orgs/$Org/installation"

$installationId = $installationResponse.id
if (-not $installationId) {
    Write-Error "Failed to get installation ID."
    exit 1
}

# Step 3: Get access token
Write-Host "[INFO] Getting installation access token..."
$tokenResponse = Invoke-RestMethod -Method Post `
    -Headers @{Authorization = "Bearer $jwt"; Accept = "application/vnd.github+json" } `
    -Uri "https://api.github.com/app/installations/$installationId/access_tokens"

$accessToken = $tokenResponse.token
if (-not $accessToken) {
    Write-Error "Failed to get access token."
    exit 1
}

# Step 4: Test GHCR access
$ghcrUrl = "https://ghcr.io/v2/$Org/$Repo/$ImageName/tags/list"
Write-Host "[INFO] Testing access to $ghcrUrl"

$response = Invoke-WebRequest -Uri $ghcrUrl -Headers @{ Authorization = "Bearer $accessToken" } -Method Get -ErrorAction SilentlyContinue

if ($response.StatusCode -eq 200) {
    Write-Host "[SUCCESS] Access to GHCR is working."
    Write-Output $response.Content
} else {
    Write-Host "[FAIL] Access denied. Response:"
    Write-Output $response.RawContent
}
