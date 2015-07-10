// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open Suave
open Suave.Http.Successful
open Suave.Web 
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Topshelf
open System
open System.Threading

[<EntryPoint>]
let main argv = 
    let cancellationTokenSource = ref None
    
    let start hc = 
        let cts = new CancellationTokenSource()
        let token = cts.Token
        let config = { defaultConfig with cancellationToken = token}
        startWebServerAsync config app
        |> snd
        |> Async.StartAsTask 
        |> ignore

        cancellationTokenSource := Some cts
        true

    let stop hc = 
        match !cancellationTokenSource with
        | Some cts -> cts.Cancel()
        | None -> ()
        true

    Service.Default 
    |> display_name "ServiceDisplayName"
    |> instance_name "ServiceName"
    |> with_start start
    |> with_stop stop
    |> with_topshelf

    0 // return an integer exit code
