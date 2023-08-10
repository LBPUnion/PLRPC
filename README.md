# PLRPC

[![Build Artifacts](https://github.com/LBPUnion/PLRPC/actions/workflows/build.yml/badge.svg)](https://github.com/LBPUnion/PLRPC/actions/workflows/build.yml)

PLRPC (short for ProjectLighthouse Rich Presence Client) is a continuation of the LighthouseRichPresence client under
the same premise.

## Features

- [x] Retrieve remote configuration from Lighthouse or compatible API
- [x] Obtain user, user status, room, and slot information from a Lighthouse server
- [x] Interact with a Discord Client to display retrieved information
- [ ] Interactive buttons for user/slot
    - [x] View User Profile
    - [ ] View Current Slot
- [ ] Stability (no fires)

## Installation Instructions
                                   
**GUI Installation Steps (recommended)**

1. Navigate to the Releases Tab
2. Download the latest GUI build for Windows or Linux
3. Extract the build to any folder
4. Run the client
   * **Windows:** Run the GUI by double clicking on the `LBPUnion.PLRPC.GUI.Windows` executable
   * **Linux:** Run the GUI by double clicking on the `LBPUnion.PLRPC.GUI.Linux` executable
     * You may need to mark the program as executable first, or run it from the command line

**CLI Installation Steps (advanced)**

> **Warning**
> These steps are only for advanced users who are comfortable with the command line.
> If you are not comfortable with the command line, please use the GUI instead.

1. Navigate to the Releases Tab
2. Download the latest CLI build for Windows or Linux
3. Extract the build to any folder
4. Run the client
    - **Configuration Mode:** `./path/to/PLRPC.CLI --config` (use `--config` each time)
    - **Manual Mode:** `./path/to/PLRPC.CLI --server https://lighthouse.instance.url --username instanceusername`

**Post Install:**

Please create an Issue if you encounter any bugs or weird errors.

## Helpful Information

* You can use the `--applicationid` command line argument, change the `applicationId` entry in your configuration,
  or if using the GUI, unlock defaults and change the `Application ID` entry in the options to override the default
  Discord Application ID. This can be useful if your Lighthouse instance or other service is compatible with the PLRPC 
  protocol and you want to display your own application name.
* LittleBigPlanet 1 is not fully supported and will not show all of the information LBP2, 3, and Vita will. This is
  because LBP1 does not support rooms, therefore the client cannot determine user status or slot information. There
  are plans to implement expanded support for LBP1 in the future as noted by [#30](https://github.com/LBPUnion/PLRPC/issues/30).