# Viva

Viva is ETL project written in [F#](https://learn.microsoft.com/en-us/dotnet/fsharp/what-is-fsharp)

## Table of Contents

* [Getting Started](#getting-started)
* [Features](#features)
* [Requirements](#requirements)
* [Installation](#installation)
* [Usage](#usage)
* [Contributing](#contributing)
* [License](#license)

## Getting Started

[ Briefly describe how to get started with the project, including any dependencies or setup required ]

## Features

* [ List the key features of the project ]

## Requirements
- Requires Dotnet 9.0

## Installation

[ Provide step-by-step instructions for installing the project, including any commands or scripts to run ]

## Usage

```fsharp
open Microsoft.Extensions.Logging
open Microsoft.Playwright

open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions
open Viva.Playwright

let factory = LoggerFactory.Create(fun builder -> builder.AddSimpleConsole() |> ignore)
let context =
    PlaywrightContext.createPlaywrightContext(
        MsEdge,
        BrowserTypeLaunchOptions(Headless = false),
        BrowserNewContextOptions(ViewportSize = ViewportSize.NoViewport),
        factory
    )
```

## Contributing
Here's a guide to setting up your development environment:

### Docker
- Docker is used for testing Playwright code. Podman isn't recommended due to lack of BuildKit support.
- The [container image](lscr.io/linuxserver/msedge:latest) includes browser-based KASMVnc for GUI development.

### VS Code
#### Dev Container Extension:

- Automatically installs .NET (dotnet).
- Sets up necessary tools and restores packages needed for your project.
- Configures required extensions.
*Note:* This setup has not been tested with other IDEs like Visual Studio or Rider.

## License Information for Viva Project

**Copyright** © [2025]
The Viva project is distributed under [Unlicense](https://unlicense.org). See the [LICENSE](https://github.com/getkks/Viva/blob/main/LICENSE) file for more information.