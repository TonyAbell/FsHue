module FsHue.Pages.Home

open Suave
open System
open System.Web
open FsHue.hueUtils
open Suave.Types
open FSharp.Data
open FsHue.LightCommandExtensions
open Suave.Http.Successful
open Suave.Json
open Suave.Http
open Suave.Web
open Newtonsoft.Json

type HomeView =
    { Lights : list<Light>
      Groups : list<Group> }

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

let jsonSettings =
    let settings = new  JsonSerializerSettings()
    settings.ContractResolver <- new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
    settings


let allLights (ctx : HttpContext) =
    async {
        let lgihts = FsHue.hueUtils.getLights()
        return! JsonConvert.SerializeObject(lgihts,Formatting.Indented,jsonSettings)
                   |>  System.Text.Encoding.UTF8.GetBytes
                   |>  Response.response HTTP_200
                   >>= Writers.setMimeType "application/json"
                   <|  ctx

    }

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
