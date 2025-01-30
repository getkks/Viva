namespace Viva.Playwright.Services

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Playwright
open IcedTasks
open FsToolkit.ErrorHandling
open Viva.Runtime.Helpers
open Viva.Runtime.Extensions

type PlaywrightService(logger: PlaywrightService ILogger) =
    member val Playwright =
        try
            let play = Playwright.CreateAsync().Await()
            logger.LogDebug("Playwright instance created.")
            play
        with ex ->
            logger.LogError(ex, "Failed to create Playwright instance.")
            raise ex