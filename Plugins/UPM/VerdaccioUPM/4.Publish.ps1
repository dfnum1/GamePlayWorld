param(
  [ValidateSet('','patch','minor','major')]
  [string]$bump = '',
  [switch]$tag                 # 加 -tag 才推送 git tag
)

cd D:\VerdaccioUPM\Assets\gameplay
$ErrorActionPreference = 'Stop'

# 1. 升版（可选）
if ($bump) {
    Write-Host "========== 版本自增 ($bump) ==========" -ForegroundColor Green
    npm version $bump --no-git-tag-version
}

# 2. 发布
Write-Host "========== 发布到私服 ==========" -ForegroundColor Green
npm publish --registry http://10.0.16.124:4873

# 3. 打 tag / 推送（可选）
if ($tag) {
    Write-Host "========== 打 tag 并推送 ==========" -ForegroundColor Green
    $ver = (Get-Content package.json | ConvertFrom-Json).version
    git add package.json
    git commit -m "chore: release v$ver"
    git tag "v$ver"
    git push
    git push origin "v$ver"
}

Write-Host "===== 发布完成！=====" -ForegroundColor Cyan