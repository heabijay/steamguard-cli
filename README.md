# steamguard-cli (fork) [beta]

A cross-platform CLI app for setting up and using Steam Mobile Authenticator. This is a fork of [dyc3/steamguard-cli](https://github.com/dyc3/steamguard-cli), refactored and rewritten on .NET 5. Works on any plaform using dotnet.


# Disclaimer

**Use this software at your own risk.**


# Usage

`steamguard-cli` uses `maFiles` folder in own app folder by default, but you can change it using argument `-m <path>`.
Your `maFiles` can be created with [Steam Desktop Authenticator][SDA]. You can create `maFiles` with steamguard-cli using the `setup` action (`steamguard-cli setup`).

**REMEMBER TO MAKE BACKUPS OF YOUR `maFiles`, AND TO WRITE DOWN YOUR RECOVERY CODE!**

[SDA]: https://github.com/Jessecar96/SteamDesktopAuthenticator


```
usage: steamguard-cli (verb) (options)

Verbs:
  add, setup                  Set up Steam Guard for 2 factor authentication.
  decrypt                     Remove encryption from your maFiles.
  encrypt                     Encrypt your maFiles or change your encryption passkey.
  generate-code, code, 2fa    (Default Verb) Generate a Steam Guard code for the specified user (if any) and exit. (default).
  trade, setup                Set up Steam Guard for 2 factor authentication.
  help                        Display more information on a specific command.
  version                     Display version information.

Options:
  -c, --copy                  Copies codes to clipboard
  -s, --steam-username        Selected steam username.
  -m, --mafiles-path          Input file to read.
  -p, --pass-key              Specify your encryption passkey.
  --help                      Display this help screen.
  --version                   Display version information.
```