module FsHue.hueUtils

open Q42.HueApi.NET
open System
open Q42.HueApi

let private ip =
     (new SSDPBridgeLocator()).LocateBridgesAsync(TimeSpan.FromSeconds(5.))
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> Seq.head


let private client() = 
    new HueClient(ip, "newdeveloper")

let getLights() = 
    client().GetLightsAsync()
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> Seq.toArray
