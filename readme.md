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

[ Provide examples of how to use the project, including any commands or APIs ]

## Contributing

- ### Docker
	- You need Docker for the time being to test playwright related code. Podman fails due to issue with (or missing support for) BuildKit.
	- The base image includes browser based KASMVnc support
- ### VS Code
	- #### Dev Container
		- Creates everything necessary on top of docker
		- Installs dotnet post creation of the container for the first time
		- Install all the dotnet tools and restores all the packages
		- Sets up all the extension
	- Other IDEs (Visual Studio or Rider) were not tested.

## License

Copyright (c) [2025]

This project is licensed under the terms of [Unlicense](https://unlicense.org) license. See the [LICENSE](https://github.com/getkks/Viva/blob/main/LICENSE) file for more information.