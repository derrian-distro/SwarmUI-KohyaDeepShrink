# SwarmUI Kohya Deep Shrink Extension

A SwarmUI extension that provides a user-friendly interface for the ComfyUI Kohya Deep Shrink component. This extension allows you to easily adjust model downscaling parameters during the image generation process.

## Description

Kohya Deep Shrink is a technique that can help improve image generation quality by dynamically adjusting the model's resolution during the generation process. This extension wraps the ComfyUI `PatchModelAddDownscale` node and exposes its parameters through SwarmUI's interface.

## Parameters

- **Block Number** (1-32): The U-Net block number where the downscaling will be applied
- **Downscale Factor** (0.1-9.0): How much to downscale the model's internal resolution
- **Start Percent** (0-1): When to begin applying the downscale effect during generation
- **End Percent** (0-1): When to stop applying the downscale effect during generation
- **Downscale Method**: Algorithm to use for downscaling. Options:
  - bicubic
  - nearest-exact
  - bilinear
  - area
  - bislerp
- **Upscale Method**: Algorithm to use for upscaling. Same options as downscale method
- **Downscale After Skip**: Whether to apply downscaling after skip connections

## Installation

### One-Click Install
You can now install the extension directly from the SwarmUI extension manager.
You can find the extension in the Server -> Extensions tab.
It's as simple as clicking install and then waiting for the server to restart.
After that you should see the section in the Generate tab.

### Manual
* Shutdown SwarmUI
* Update SwarmUI first, if you have an old version this extension may not work
* Open a cmd/terminal window in SwarmUI\src\Extensions
* Run `git clone https://github.com/derrian-distro/SwarmUI-KohyaDeepShrink.git`
* Run `SwarmUI\update-windows.bat` or `SwarmUI\update-linuxmac.sh` to recompile SwarmUI

## Usage

1. Enable the "Kohya Deep Shrink" parameter group in the SwarmUI interface
2. Adjust the parameters as needed
3. Generate images as normal - the deep shrink effect will be automatically applied

## Default Values

- Block Number: 8
- Downscale Factor: 2.0
- Start Percent: 0.0
- End Percent: 0.35
- Downscale Method: bicubic
- Upscale Method: bicubic
- Downscale After Skip: true