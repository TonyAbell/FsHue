

module FsHue.LightCommandExtensions 
    open FSharpx.Extras
    open FSharpx.Functional
    open Q42.HueApi
    open Colourful.Conversion
    
    type LightCommand with
        static member colour = 
             { Get = fun (c: LightCommand) -> 
                let converter = new ColourfulConverter()
                let xCoord  = c.ColorCoordinates.[0]
                let yCoord = c.ColorCoordinates.[0]
                let xyYColor = Colourful.xyYColor(xCoord,xCoord,1.)
                converter.ToRGB(xyYColor).ToColor()
               
               Set = fun v (x: LightCommand) ->
                     let converter = new ColourfulConverter()
                     let color = Colourful.RGBColor(v)
                     let xyY = converter.ToxyY(color) 
                     x.ColorCoordinates <- [| xyY.x; xyY.y|] 
                     x
                    
               
              }
        static member on = 
             { Get = fun (c: LightCommand) -> if c.On.HasValue then c.On.Value else false
               Set = fun v (x: LightCommand) ->                   
                    x.On <- System.Nullable<bool>.op_Implicit v
                    x
              }
        static member brightness = 
             { Get = fun (c: LightCommand) -> if c.Brightness.HasValue then c.Brightness.Value else 0uy
               Set = fun v (x: LightCommand) ->                                       
                    x.Brightness <- System.Nullable<byte>.op_Implicit v
                    x
              }
