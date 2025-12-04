param(
  [ValidateSet('','patch','minor','major')]
  [string]$bump = ''
)
cd D:\VerdaccioUPM\Assets\gameplay
$ErrorActionPreference = 'Stop'

if ($bump) {
    Write-Host "========== 版本自增 ($bump) ==========" -ForegroundColor Green
    npm version $bump --no-git-tag-version
}

Write-Host "========== 发布到私服 ==========" -ForegroundColor Green
npm publish --registry http://10.0.16.124:4873

Write-Host "===== 发布完成！=====" -ForegroundColor Cyan