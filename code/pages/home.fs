module FsHue.Pages.Home

open Suave
open System
open System.Web
open FsHue.hueUtils
open Suave.Types
open FSharp.Data
open FsHue.LightCommandExtensions
open Suave.Http.Successful

type Home = 
    { Lights : seq<string> }

let delay (f : unit -> Suave.Types.WebPart) ctx = async { return! f () ctx }

let showHome = 
    delay (fun () -> 
        let lights = FsHue.hueUtils.getLights()
        DotLiquid.page "home.html" (lights |> Array.toList))

let offCmd = JsonValue.Null |> JsonValue.on.Set false

let onCmd = 
    JsonValue.Null
    |> JsonValue.on.Set true
    |> JsonValue.brightness.Set 255uy
    |> JsonValue.colour.Set System.Drawing.Color.White

let turnLightOn = function 
    | id -> FsHue.hueUtils.setLightState onCmd id |> ignore
let turnLightOff = function 
    | id -> FsHue.hueUtils.setLightState offCmd id |> ignore

let turnAllOn (ctx : HttpContext) = 
    async { 
        FsHue.hueUtils.getLights()
        |> Array.map (fun f -> f.Id)
        |> Array.iter turnLightOn
        return Some(ctx)
    }
let turnAllOff (ctx : HttpContext) = 
    async { 
        FsHue.hueUtils.getLights()
        |> Array.map (fun f -> f.Id)
        |> Array.iter turnLightOff
        return Some(ctx)
    }

let turnOn (ctx : HttpContext) = 
    async { 
        match ctx.request.formData "lightid" with
        | Choice1Of2 id -> 
            let intId = System.Convert.ToInt32(id)
            turnLightOn intId
        | Choice2Of2 msg -> ()
        return Some(ctx)
    }

let turnOff (ctx : HttpContext) = 
    async { 
        match ctx.request.formData "lightid" with
        | Choice1Of2 id -> 
            let intId = System.Convert.ToInt32(id)
            turnLightOff intId
        | Choice2Of2 msg -> ()
        return Some(ctx)
    }
