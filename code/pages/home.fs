module FsHue.Pages.Home

open Suave
open System
open System.Web
open FsHue.hueUtils
open Suave.Types
open FSharp.Data
open FsHue.LightCommandExtensions
type Home = {
    Lights: seq<string>
    }

let delay (f:unit -> Suave.Types.WebPart) ctx = 
  async { return! f () ctx }

let showHome = delay (fun () -> 
  let lights = FsHue.hueUtils.getLights()
  match lights with 
   | Some d -> DotLiquid.page "home.html" (d |> Array.toList)
   | None -> DotLiquid.page "home.html" Array.empty<Light> )


let turnOn (ctx:HttpContext)   = 
    async {
        
        let onCmd = 
            JsonValue.Null 
                |> JsonValue.on.Set true
                |> JsonValue.brightness.Set 255uy
                |> JsonValue.colour.Set System.Drawing.Color.White
        match ctx.request.formData "lightid" with
        | Choice1Of2 id -> 
            let intId = System.Convert.ToInt32(id)

            FsHue.hueUtils.setLightState intId onCmd |> ignore
        | Choice2Of2 msg -> ()

       
        return! showHome ctx 
    }
let turnOff (ctx:HttpContext) =
    async {
         let cmd = 
            JsonValue.Null 
                |> JsonValue.on.Set false
         match ctx.request.formData "lightid" with
            | Choice1Of2 id -> 
                let intId = System.Convert.ToInt32(id)

                FsHue.hueUtils.setLightState intId cmd |> ignore
            | Choice2Of2 msg -> 
                printfn "%s" msg
                ()
         return! showHome ctx
    }