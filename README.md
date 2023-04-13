# PLRPC

PLRPC (short for ProjectLighthouse Rich Presence Client) is a continuation of the LighthouseRichPresence client under the same premise.

## Features

- [x] Obtain user, user status, room, and slot information from a Lighthouse server
- [x] Interact with a Discord Client to display retrieved information
- [x] Configuration file support
- [x] Make @Slendy cry
- [] Interactive buttons for user/slot
- [] Stability (no fires)

## Installation Instructions

**Prerequisites:**

- .NET Runtime 7.x.x (64-bit) *Required in order to run the application. Major version releases will most likely have the runtime packaged inside, though this doesn't happen often.*
  - [Windows](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-7.0.5-windows-x64-installer)
  - [Linux](https://learn.microsoft.com/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website)

**Installation Steps:**

1. Ensure you have all prerequisites properly installed
2. Navigate to the Releases Tab
3. Download the latest build (or major version if you like somewhat-stability)
4. Extract the build to any folder
5. Run the client
    - **Configuration Mode:** `./path/to/PLRPC --config`
    - **Interactive Mode:** `./path/to/PLRPC`

**Post Install:**

Please create an Issue if you encounter any bugs or weird errors.
