

module FsHue.LightCommandExtensions 
    open FSharpx.Extras
    open FSharpx.Functional
    open FSharp.Data.JsonExtensions

    open Colourful.Conversion
    open FSharp.Data.HttpContentTypes
    open FSharp.Data
    open System

    
    type JsonValue with
        static member colour = 
             { Get = fun (x: JsonValue) -> 
                 match x.TryGetProperty "xy" with
                  | Some v -> let converter = new ColourfulConverter()
                              let xyArray = v.AsArray()
                              let xCoord  = xyArray.[0].AsFloat()
                              let yCoord  = xyArray.[1].AsFloat()
                              let xyYColor = Colourful.xyYColor(xCoord,xCoord,1.)
                              converter.ToRGB(xyYColor).ToColor()
                  | None -> System.Drawing.Color.White
               Set = fun v (x: JsonValue) ->
                     let converter = new ColourfulConverter()
                     let color = Colourful.RGBColor(v)
                     let xyY = converter.ToxyY(color) 
                     let data = [| xyY.x; xyY.y|] |> Array.map JsonValue.Float
                     match x.TryGetProperty "xy" with
                        | Some _ -> x.Properties
                                        |> Array.filter (fun (p,_)-> p <> "xy")
                                        |> Array.append [|"xy", JsonValue.Array data |]
                                        |> JsonValue.Record 
                        | None -> x.Properties
                                    |> Array.append [|"xy", JsonValue.Array data |]
                                    |> JsonValue.Record    
              }
        static member on = 
             { Get = fun (c: JsonValue) -> 
                match c.TryGetProperty "on" with
                  | Some v -> v.AsBoolean()
                  | None -> false
                                            
               Set = fun v (x: JsonValue) ->  
                  match x.TryGetProperty "on" with
                  | Some _ -> x.Properties
                              |> Array.filter (fun (p,_)-> p <> "on")
                              |> Array.append [|"on", JsonValue.Boolean v|]
                              |> JsonValue.Record  
                  | None -> x.Properties
                              |> Array.append [|"on", JsonValue.Boolean v|]
                              |> JsonValue.Record           
              }
        static member brightness = 
             { Get = fun (c: JsonValue) -> 
                match c.TryGetProperty "bri" with
                  | Some v -> Convert.ToByte( v.AsInteger())
                  | None -> 0uy
                                            
               Set = fun v (x: JsonValue) ->  
                    let d = Convert.ToDecimal(Convert.ToInt16(v))
                    match x.TryGetProperty "bri" with
                        | Some _ -> x.Properties
                                    |> Array.filter (fun (p,_)-> p <> "bri")
                                    |> Array.append [|"bri", JsonValue.Number d|]
                                    |> JsonValue.Record
                        | None -> x.Properties
                                    |> Array.append [|"bri", JsonValue.Number d|]
                                    |> JsonValue.Record           
              }
      