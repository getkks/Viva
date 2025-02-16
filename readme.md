# Viva

Viva is an ETL project built using [F#](https://learn.microsoft.com/en-us/dotnet/fsharp/what-is-fsharp). It offers a comprehensive solution for data extraction, transformation, and loading, making it easier to manage and process data. This document will guide you through the various aspects of Viva, including its features, requirements, installation, usage, contribution guidelines, and license information.

## Table of Contents

* [Getting Started](#getting-started)
	* [Requirements](#requirements)
* [Features](#features)
* [Usage](#usage)
* [Contributing](#contributing)
* [License](#license)

## Getting Started

To get started with Viva, you will need the following:

### Requirements
- Requires .NET Sdk 9.0 or higher installed on your machine.
- You will also need to install the required NuGet packages.
	```powershell
	dotnet add package Viva
	dotnet add package Viva.Runtime
	dotnet add package Viva.Playwright
	```

## Features

- **Efficient data extraction**: Viva offers tools for extracting data from various sources.
- **Flexible transformation capabilities**: Offers tools for cleaning, manipulating, and enhancing extracted data.
- **Apply transformations**: Includes features like filtering, aggregating, joining data, etc.
- **Cross-Platform Compatibility**: Runs on Windows, macOS, and Linux.

## Usage
- Add the packages to your project
	```powershell
	dotnet add package Viva
	dotnet add package Viva.Runtime
	dotnet add package Viva.Playwright
	```
- Simplified example.
	1. Setup Logging.
		```fsharp
		open Microsoft.Extensions.Logging
		open Microsoft.Playwright

		open FsToolkit.ErrorHandling
		open Viva.Runtime.Helpers.Operator.Result
		open Viva.Playwright

		let factory =
			LoggerFactory.Create(fun builder ->
				builder
					.AddSimpleConsole(fun options ->
						options.SingleLine <- true
						options.IncludeScopes <- true
						options.TimestampFormat <- "hh:mm:ss tt - "
					)
					.SetMinimumLevel
		#if DEBUG
					LogLevel.Debug
		#else
					LogLevel.Information
		#endif

				|> ignore
			)
		```
	2. Use Playwright to interact with a website.
		```fsharp
		MsEdge.Create(factory, BrowserTypeLaunchOptions(Headless = false), BrowserNewContextOptions(ViewportSize = ViewportSize.NoViewport))
		>>= _.Open("https://www.youtube.com/")
		<!> _.SetDefaultTimeOut(TimeSpan.FromMinutes 30L)
		<!> _.GetByRole(AriaRole.Combobox, "Search")
		>>= _.Fill("linux brtfs")
		>>= _.Press("ArrowDown")
		>>= _.PressAndClose("Enter")
		<!> _.GetByLabel("Search filters")
		>>= _.ClickAndClose()
		<!> _.GetBySelector("ytd-search-filter-renderer", "This year")
		>>= _.ClickAndClose()
		<!> _.GetByRole(AriaRole.Link, "This year")
		>>= _.ClickAndClose()
		|> Result.tee(fun _ -> Async.Sleep 5000 |> Async.RunSynchronously)
		```
	3. Close the page and Playwright context, and finally dispose of the factory
		```fsharp
		>>= _.CloseAll()
		|> ignore

		factory.Dispose()
		```
## Contributing

We welcome contributions to the Viva project! Please follow these steps to contribute:

1. Set up development environment.
	1. Install .Net Sdk
	2. Docker (if Linux distro is not supported by Playwright)
		- Docker is used for testing Playwright code. Podman isn't recommended due to lack of BuildKit support.
		- The [container image](lscr.io/linuxserver/msedge:latest) includes browser-based KASMVnc for GUI development.
	3. VS Code
		- Dev Container Extension
			- Automatically installs .NET (dotnet).
			- Sets up necessary tools and restores packages needed for your project.
			- Configures required extensions.

		*Note:* This setup has not been tested with other IDEs like Visual Studio or Rider.
2. Fork the Viva repository.
3. Clone your forked repository to a local machine.
4. Create a new branch for your changes.
5. Make your changes and commit them with descriptive messages.
6. Submit a pull request, providing a clear description of your changes.
7. We will review your pull request and provide feedback or merge your changes.

## License Information for Viva Project

**Copyright** © [2025]
The Viva project is distributed under [Unlicense](https://unlicense.org). See the [LICENSE](https://github.com/getkks/Viva/blob/main/LICENSE) file for more information.