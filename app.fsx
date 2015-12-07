#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "packages/FSharpx.Extras/lib/40/FSharpx.Extras.dll"
#r "packages/Colourful/lib/net45/Colourful.dll"
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#load "packages/FSharp.Formatting/FSharp.Formatting.fsx"
open System
open System.Web
open System.IO
open Suave
open Suave.Web
open Suave.Http
open Suave.Types
open FSharp.Data
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Web


open Suave.Sockets
open Suave.Sockets.Control
open Suave.WebSocket

open System
open System.Net
open Suave
open Suave.Types
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Http.Files
open Suave.Http.RequestErrors
open Suave.Logging
open Suave.Web
open Suave.Utils


#load "code/common/utils.fs"
#load "code/common/lightCommandExtensions.fs"
#load "code/common/hueUtils.fs"
#load "code/pages/home.fs"
#load "code/app.fs"


open FsHue.LightCommandExtensions
open FsHue.Pages
open FsHue.Main



let cfg =
  { defaultConfig with
      bindings = [ HttpBinding.mk' HTTP  "127.0.0.1" 8010 ]
      homeFolder = Some __SOURCE_DIRECTORY__ }
let _, server = startWebServerAsync cfg app
Async.Start(server)
System.Diagnostics.Process.Start("http://localhost:8010")
System.Console.ReadLine() |> ignore

//#if INTERACTIVE
//#else
//let cfg =
//  { defaultConfig with
//      bindings = [ HttpBinding.mk' HTTP  "127.0.0.1" 8011 ]
//      homeFolder = Some __SOURCE_DIRECTORY__ }
//let _, server = startWebServerAsync cfg app
//Async.Start(server)
//System.Diagnostics.Process.Start("http://localhost:8011")
//System.Console.ReadLine() |> ignore
//#endif
