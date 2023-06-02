# PLRPC

[![Build Artifacts](https://github.com/LBPUnion/PLRPC/actions/workflows/build.yml/badge.svg)](https://github.com/LBPUnion/PLRPC/actions/workflows/build.yml)
[![CodeQL Analysis](https://github.com/LBPUnion/PLRPC/actions/workflows/codeql.yml/badge.svg)](https://github.com/LBPUnion/PLRPC/actions/workflows/codeql.yml)

PLRPC (short for ProjectLighthouse Rich Presence Client) is a continuation of the LighthouseRichPresence client under
the same premise.

## Features

- [x] Obtain user, user status, room, and slot information from a Lighthouse server
- [x] Interact with a Discord Client to display retrieved information
- [x] Configuration file support
- [x] Make @Slendy cry
- [ ] Interactive buttons for user/slot
    - [x] View User Profile
    - [ ] View Current Slot
- [ ] Stability (no fires)

## Installation Instructions

**Installation Steps:**

1. Navigate to the Releases Tab
2. Download the latest build (or major version if you like somewhat-stability)
3. Extract the build to any folder
4. Run the client
    - **Configuration Mode:** `./path/to/PLRPC --config` (use `--config` each time)
    - **Manual Mode:** `./path/to/PLRPC --server https://lighthouse.instance.url --username instanceusername`

> **Warning** for **Windows Users**:
>
> Currently, you are unable to run the .exe file directly. You **must** open a Command Prompt or PowerShell
> window, navigate to the file path, and execute the binary manually. Refer
to [Installation Step #4](https://github.com/LBPUnion/PLRPC/blob/master/README.md#installation-instructions)
> for instructions on how to further configure and run the client.

**Post Install:**

Please create an Issue if you encounter any bugs or weird errors.
