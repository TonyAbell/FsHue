module FsHue.Utils

open Suave
open System
open System.Web

open Suave.Types
open FSharp.Data

open Suave.Http.Successful
open Suave.Json
open Suave.Http
open Suave.Web
open Newtonsoft.Json


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

let doWork (idName)(getId: string -> Choice<string,string>) (work:int->unit) =
      match getId idName with
      | Choice1Of2 id ->
          let intId = System.Convert.ToInt32(id)
          work intId
      | Choice2Of2 msg -> ()
      ()
