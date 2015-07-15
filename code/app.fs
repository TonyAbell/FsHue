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



let app =
  choose
    [ GET >>= choose
        [ browseStaticFiles
          path "/lights" >>= Home.allLights
          path "/groups" >>= Home.allGroups ]
      PUT >>= choose
        [ path "/turnallon" >>= Home.turnAllOn >>= NO_CONTENT
          path "/turnalloff" >>= Home.turnAllOff >>= NO_CONTENT
          path "/turnon" >>= Home.turnOn >>= NO_CONTENT
          path "/turnoff" >>= Home.turnOff >>= NO_CONTENT ]
      ]
