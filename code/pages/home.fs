module FsHue.Pages.Home

open Suave
open System
open System.Web
open FsHue.hueUtils

type Home = {
    Lights: seq<string>
    }

let delay (f:unit -> Suave.Types.WebPart) ctx = 
  async { return! f () ctx }



let showHome = delay (fun () -> 
  let lights = FsHue.hueUtils.getLights()
  let data = lights |> Array.map(fun f-> f.Name) 
  DotLiquid.page "home.html" (data))