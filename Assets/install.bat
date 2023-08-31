@echo off

REM Check if pip is installed
where pip3 >nul 2>&1
if %errorlevel% neq 0 (
    echo pip not found. Installing pip...
    python -m ensurepip --default-pip
)

REM Install vdf package using pip
echo Installing vdf package...
pip3 install vdf

REM Run your Python script
echo Running Bonelab Study mod installer
python3 Install.py
