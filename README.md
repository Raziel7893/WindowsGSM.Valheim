# WindowsGSM.Valheim
🧩WindowsGSM plugin that provides Valheim Dedicated server support!

# The Game
https://store.steampowered.com/app/892970/Valheim/

# Notes
- Moved default save game location to inside the serverfiles, as c/valheim is stupid :D
- if you want to change that, adjust it by clicking Edit Config in WGSM and modify the savedir parameter in Server Start Param

# Requirements
WindowsGSM >= 1.21.0

# Server Files
The administrator files are located in the host folder C:\Users\YORNAME\AppData\LocalLow\IronGate\Valheim\

# Config
Edit in Edit Config (click "Edit Config" button, look in the last box "Server Start Param")

-name "CHANGEME" -port 2456 -world "CHANGEME" -password "CHANGEME" -savedir "c:\directory where you want to save" -Public 1

*-savedir You can create a folder inside the directory of your valheim server and you can make backups

*-Public 1 only for Dedicated Servers

*-Public 0 only for Lans

# Important
For now the server works with a password, you have to put 5 numbers or 5 letters minimum, it is mandatory !!!

*-WindowsGSM.Valheim not compatible with Umod or other mods, for now

# Installation
  1. Download the latest release (or click Code => download zip)
  3. Move Valheim.cs folder to plugins folder
  4. Click [RELOAD PLUGINS] button or restart WindowsGSM

### Support
[WGSM](https://discord.com/channels/590590698907107340/645730252672335893)

### Give Love!
[Buy me a coffee](https://ko-fi.com/raziel7893)

[Paypal](https://paypal.me/raziel7893)
# License
This project is licensed under the MIT License - see the <a href="https://github.com/dkdue/WindowsGSM.Valheim/blob/main/LICENSE">LICENSE.md</a> file for details
