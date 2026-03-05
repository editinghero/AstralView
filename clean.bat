@echo off
echo Cleaning build artifacts...
if exist obj rmdir /s /q obj
if exist bin rmdir /s /q bin
if exist publish rmdir /s /q publish
echo Clean completed!
