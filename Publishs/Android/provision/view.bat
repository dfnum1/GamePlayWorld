@echo off
set curDir=%cd%
set keytool = C:/"Program Files"/Java/jre1.8.0_181/bin/

C:/"Program Files"/Java/jre1.8.0_181/bin/keytool.exe -v -list -keystore key.keystore

@pause