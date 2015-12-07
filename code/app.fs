module FsHue.Main

open System
open System.Web
open System.IO
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Web
open Suave
open Suave.Web
open Suave.Http
open Suave.Types
open FsHue.Pages

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

open System
open System.Net

open Suave.Sockets
open Suave.Sockets.Control
open Suave.WebSocket



let browseStaticFile file ctx = async {
  let actualFile = Path.Combine(ctx.runtime.homeDirectory, "web", file)
  if System.IO.File.Exists(actualFile) then
      let mime = Suave.Http.Writers.defaultMimeTypesMap(Path.GetExtension(actualFile))
      let setMime =
        match mime with
        | None -> fun c -> async { return None }
        | Some mime -> Suave.Http.Writers.setMimeType mime.name
      return! ctx |> ( setMime >>= Successful.ok(File.ReadAllBytes actualFile) )
  else
      return None
       }
let browseStaticFiles ctx = async {
  let local = ctx.request.url.LocalPath
  let file = if local = "/" then "index.html" else local.Substring(1)
  return! browseStaticFile file ctx }


let echo (webSocket : WebSocket) =
  fun (cx:HttpContext) -> socket {
    
    let loop = ref true
    while !loop do
      let! msg = webSocket.read()
      match msg with
      | (Text, data, true) ->
        let str = UTF8.toString data
        do! webSocket.send Text data true
      | (Ping, _, _) ->
        do! webSocket.send Pong [||] true
      | (Close, _, _) ->
        do! webSocket.send Close [||] true
        loop := false
      | _ -> ()
  }



let app =
  choose
    [ GET >>= choose
        [ browseStaticFiles
          path "/websocket" >>= handShake echo
          path "/lights" >>= Home.allLights
          path "/groups" >>= Home.allGroups ]
      PUT >>= choose
        [ path "/turnallon" >>= Home.turnAllOn >>= NO_CONTENT
          path "/turnalloff" >>= Home.turnAllOff >>= NO_CONTENT
          path "/turnon" >>= Home.turnOn >>= NO_CONTENT
          path "/turnoff" >>= Home.turnOff >>= NO_CONTENT ]
      ]
