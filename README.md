# PLRPC

[![Build Artifacts](https://github.com/LBPUnion/PLRPC/actions/workflows/build.yml/badge.svg)](https://github.com/LBPUnion/PLRPC/actions/workflows/build.yml)

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
                                   
**GUI Installation Steps (recommended)**

1. Navigate to the Releases Tab
2. Download the latest GUI build (or major version if you like somewhat-stability)
3. Extract the build to any folder
4. Run the client by double clicking on the `PLRPC.GUI.Windows.exe` executable

**CLI Installation Steps (advanced)**

1. Navigate to the Releases Tab
2. Download the latest CLI build (or major version if you like somewhat-stability)
3. Extract the build to any folder
4. Run the client
    - **Configuration Mode:** `./path/to/PLRPC --config` (use `--config` each time)
    - **Manual Mode:** `./path/to/PLRPC --server https://lighthouse.instance.url --username instanceusername`

**Post Install:**

Please create an Issue if you encounter any bugs or weird errors.

## Helpful Information

* You can use the `--applicationid` command line argument, change the `applicationId` entry in your configuration,
  or if using the GUI, unlock defaults and change the `Application ID` entry in the options to override the default
  Discord Application ID. This can be useful if your Lighthouse instance or other service is compatible with the PLRPC 
  protocol and you want to display your own application name.