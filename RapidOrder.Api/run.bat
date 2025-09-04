@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

REM Maximum file size = 1MB
SET maxSize=1048576
SET i=0

:loop
SET fileToWrite=orders_!i!.txt

REM Check if file exists and rotate if bigger than maxSize
IF EXIST !fileToWrite! (
    FOR %%A IN (!fileToWrite!) DO (
        SET size=%%~zA
    )
    IF !size! GEQ %maxSize% (
        SET /A i+=1
        GOTO loop
    )
)

REM Capture RF signal and append to current file
rtl_fm.exe -f 433.92M -s 22050 -g 20 | multimon-ng.exe -a POCSAG1200 -t raw - >> !fileToWrite!

pause