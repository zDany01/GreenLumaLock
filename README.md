# GreenLumaLock

**GreenLumaLock** is a C# utility designed to act as a state-aware "watcher" and launcher for GreenLuma 2025 (specifically `DLLInjector.exe`).

It is designed to be integrated into **Playnite** scripts to ensure Steam is running in the correct "patched" state before launching a game. It manages the Steam lifecycle, prevents double-injection, and uses a lockfile to track the patched session.

## 🚀 How It Works

GreenLumaLock checks the state of the **Steam process** and a local **lockfile** (`greenluma.lock`) to determine the correct course of action.

### The Logic Matrix

The program determines its action based on two conditions: **Is Steam Running?** and **Does the Lockfile Exist?**

| Steam Status | Lockfile Status | Action Taken | Result |
| :--- | :--- | :--- | :--- |
| **Running** | **Exists** | None. Steam is already patched. | **Success** (Steam Ready) |
| **Running** | **Missing** | 1. Kill Steam.<br>2. Launch `DLLInjector.exe`.<br>3. Launch Watcher. | **Success** (Steam Restarted) |
| **Stopped** | **Missing** | 1. Launch `DLLInjector.exe`.<br>2. Launch Watcher. | **Success** (Steam Started) |
| **Stopped** | **Exists** | None. Previous crash detected. | **Error** (Invalid State) |

### The Watcher Process
When GreenLumaLock starts Steam, it spawns a hidden clone of itself (Argument: `lock`). This background process:
1.  Creates `greenluma.lock`.
2.  Waits silently until the `steam` process exits.
3.  Deletes `greenluma.lock` upon Steam closure.

## 📦 Installation

1.  Compile the project or download the release.
2.  Place `GreenLumaLock.exe` in the **same directory** as `DLLInjector.exe`.
3.  Ensure `DLLInjector.ini` is configured correctly (usually for Stealth Mode).

## 🎮 Playnite Integration

To use this with Playnite, add the following script to your game's **Script** tab under **"Execute before starting a game"**.

This script runs the locker, waits for the result, and either lets the game start or halts execution if an error occurs.

```powershell
# Setup the Process Info
$GreenLumaLock = New-Object System.Diagnostics.ProcessStartInfo
$GreenLumaLock.FileName = "C:\Path\To\Your\GreenLumaLock.exe"
$GreenLumaLock.WorkingDirectory = "C:\Path\To\Your\GreenLuma_Folder\"
$GreenLumaLock.UseShellExecute = $true
$GreenLumaLock.Arguments = "noninteractive"

# Launch the process
$p = New-Object System.Diagnostics.Process
$p.StartInfo = $GreenLumaLock
$p.Start()
$p.WaitForExit()    

# Check the Exit Code to decide next steps
switch ($p.ExitCode) {
    15 { 
        throw "Invalid GreenLuma state! Steam is closed but lockfile exists. Check manually." 
    }
    32 { 
        # Success code. Exit script to allow Playnite to launch the game.
        exit 
    }
    63 { 
        throw "Internal Error! DLLInjector.exe not found." 
    }
}
```

## ⚙️ Exit Codes

If you are writing your own scripts, here is the reference for the Exit Codes returned by the executable:

| Exit Code | Meaning | Description |
| :--- | :--- | :--- |
| `32` | **Success** | Steam is ready (either it was already running patched, or it was successfully restarted). |
| `15` | **Invalid State** | Lockfile exists but Steam is closed. Indicates a previous crash or improper shutdown. |
| `63` | **Internal Error** | `DLLInjector.exe` was not found in the directory. |

### Note on Interactive Mode
If you run the program manually (without arguments), it will pause at the end with the message: `Interactive mode, press a key to exit`. 
When using it in scripts, always pass a dummy argument (e.g., `noninteractive`) to skip this pause.

## 🛠 Troubleshooting

* **Invalid State (Exit 15):** This happens if your PC crashed and the `greenluma.lock` file wasn't deleted.
    * **Fix:** Manually delete `greenluma.lock` in the folder.
* **Permissions:** If Steam is installed in a protected folder (e.g., `Program Files`), ensure Playnite or the script has Administrator privileges to kill/restart the Steam process.

## 📝 Disclaimer
This software is a third-party utility. It is not affiliated with Valve or Steam. Use at your own risk. The developer is not responsible for any bans or issues resulting from the use of Steam injection tools.
