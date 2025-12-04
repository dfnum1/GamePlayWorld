# 1. 触发器
$trigger  = New-ScheduledTaskTrigger -AtStartup

# 2. 动作（注意引号，路径有空格必须双引号）
$nodeExe  = "`"C:\Program Files\nodejs\node.exe`""
$vbin     = "`"C:\Users\$env:USERNAME\AppData\Roaming\npm\node_modules\verdaccio\bin\verdaccio`""
$cfg      = "`"D:\VerdaccioUPM\config.yaml`""
$action   = New-ScheduledTaskAction -Execute $nodeExe `
                                     -Argument "$vbin -c $cfg" `
                                     -WorkingDirectory "D:\VerdaccioUPM"

# 3. 设置
$settings = New-ScheduledTaskSettingsSet -StartWhenAvailable `
                                          -DontStopOnIdleEnd `
                                          -AllowStartIfOnBatteries `
                                          -DontStopIfGoingOnBatteries

# 4. 关键：指定 SYSTEM 账户，并勾选“最高权限”
Register-ScheduledTask -TaskName "VerdaccioUPM" `
                       -Trigger $trigger `
                       -Action $action `
                       -Settings $settings `
                       -User "SYSTEM" `
                       -RunLevel Highest `
                       -Force