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
open FsHue.Utils

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
