module FsHue.hueUtils


open System
open FSharp.Data


type LightId = int

type LightName = string

type GroupId = int
type GroupName = string


type Light = { LightId : LightId
               LightName : LightName }

type Group = { GroupId : LightId
               GroupName : LightName
               IsOn : bool }

let private ip =
    "192.168.1.8"
let private setLightStateUri lightId=
    sprintf "http://%s/api/newdeveloper/lights/%d/state" ip lightId

let private setGroupStateUri groupId=
    sprintf "http://%s/api/newdeveloper/groups/%d/action" ip groupId


let private getLightsUri =
    sprintf "http://%s/api/newdeveloper/lights" ip
let private getGroupUri =
    sprintf "http://%s/api/newdeveloper/groups" ip


let getGroups() =
    let uri = getGroupUri
    let response = FSharp.Data.Http.Request(url=uri,httpMethod="GET")
    if response.StatusCode = 200 then
        match response.Body with
         | Text body -> let json = JsonValue.Parse body
                        json.Properties()
                            |> Array.map(fun (id,f) ->
                                let intId = Convert.ToInt32(id)
                                let name = f.GetProperty("name").AsString()
                                let isOn = f.GetProperty("action").GetProperty("on").AsBoolean()
                                { GroupId = intId ; GroupName = name; IsOn = isOn })


         | _ -> [||]
    else [||]
let getLights() =
    let uri = getLightsUri
    let response = FSharp.Data.Http.Request(url=uri,httpMethod="GET")
    if response.StatusCode = 200 then
        match response.Body with
         | Text body -> let json = JsonValue.Parse body
                        json.Properties()
                            |> Array.map(fun (id,f) ->
                                let intId = Convert.ToInt32(id)
                                let name = f.GetProperty("name").AsString()
                                { LightId = intId ; LightName = name})


         | _ -> [||]
    else [||]


let setGroupState (cmd:JsonValue) groupId =
    let uri = setGroupStateUri groupId
    let body = TextRequest (cmd.ToString())
    let response =
        FSharp.Data.Http.Request(url=uri,httpMethod="PUT",body=body)
    response.StatusCode



let setLightState (lightCmd:JsonValue) lightId =
    let uri = setLightStateUri lightId
    let body = TextRequest (lightCmd.ToString())
    let response =
        FSharp.Data.Http.Request(url=uri,httpMethod="PUT",body=body)
    response.StatusCode

open FsHue.LightCommandExtensions
let offCmd = JsonValue.Null |> JsonValue.on.Set false

let onCmd =
        JsonValue.Null
        |> JsonValue.on.Set true
        |> JsonValue.brightness.Set 255uy
        |> JsonValue.colour.Set System.Drawing.Color.White

let turnGroupOn = function
        | id -> setGroupState onCmd id |> ignore

let turnGroupOff = function
        | id -> setGroupState offCmd id |> ignore


let turnLightOn = function
        | id -> setLightState onCmd id |> ignore
let turnLightOff = function
        | id -> setLightState offCmd id |> ignore
