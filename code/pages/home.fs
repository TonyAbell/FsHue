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

let turnGroupOn = function
    | id -> FsHue.hueUtils.setGroupState onCmd id |> ignore

let turnGroupOff = function
    | id -> FsHue.hueUtils.setGroupState offCmd id |> ignore


let turnLightOn = function
    | id -> FsHue.hueUtils.setLightState onCmd id |> ignore
let turnLightOff = function
    | id -> FsHue.hueUtils.setLightState offCmd id |> ignore

let jsonSettings =
    let settings = new  JsonSerializerSettings()
    settings.ContractResolver <- new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
    settings

let buildJson<'a> (getData: unit -> 'a) ctx =
    async {
      let data = getData()
      return! JsonConvert.SerializeObject(data,Formatting.Indented,jsonSettings)
      |>  System.Text.Encoding.UTF8.GetBytes
      |>  Response.response HTTP_200
      >>= Writers.setMimeType "application/json"
      <|  ctx
    }

let allGroups ctx =
    buildJson FsHue.hueUtils.getGroups ctx

let allLights ctx =
    buildJson FsHue.hueUtils.getLights ctx
    
let iterAllLights cmd =
    FsHue.hueUtils.getLights()
        |> Array.map (fun f -> f.LightId)
        |> Array.iter cmd


let turnAllOn (ctx : HttpContext) =
    async {
        iterAllLights turnLightOn
        return Some(ctx)
    }
let turnAllOff (ctx : HttpContext) =
    async {
        iterAllLights turnLightOff
        return Some(ctx)
    }

let private doWork (idName)(getId: string -> Choice<string,string>) (work:int->unit) =
  match getId idName with
  | Choice1Of2 id ->
      let intId = System.Convert.ToInt32(id)
      work intId
  | Choice2Of2 msg -> ()
  ()

let doWorkLight = doWork "lightid"
let doworkGroup = doWork "groupid"

let turnOn (ctx : HttpContext) =
    async {
        doWorkLight ctx.request.formData turnLightOn
        doworkGroup ctx.request.formData turnGroupOn
        return Some(ctx)
    }

let turnOff (ctx : HttpContext) =
    async {
        doWorkLight ctx.request.formData turnLightOff
        doworkGroup ctx.request.formData turnGroupOff
        return Some(ctx)
    }
