@echo off
color 03

:: 删除obj目录
for /f  "delims="  %%i in ('dir /s /ad /b obj') do (
      echo. & echo = "%%i" =
      rmdir /s /q "%%i"
      )
)
:: 删除bin目录
for /f  "delims="  %%i in ('dir /s /ad /b bin') do (
      echo. & echo = "%%i" =
      rmdir /s /q "%%i"
      )
)
echo.
::pause