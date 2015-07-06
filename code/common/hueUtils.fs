module FsHue.hueUtils


open System
open FSharp.Data

type Light = { Id : int
               Name : string}

let private ip =
    "192.168.1.8"
let private setStateUri lightId=
    sprintf "http://%s/api/newdeveloper/lights/%d/state" ip lightId

let private getLightsUri =
    sprintf "http://%s/api/newdeveloper/lights" ip

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
                                { Id = intId ; Name = name})
                            

         | _ -> [||]
    else [||]

let setLightState (lightCmd:JsonValue) lightId = 
    let uri = setStateUri lightId
    let body = TextRequest (lightCmd.ToString())
    let response = 
        FSharp.Data.Http.Request(url=uri,httpMethod="PUT",body=body)
    response.StatusCode
