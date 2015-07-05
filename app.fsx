#r "System.Xml.Linq.dll"
#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "packages/DotLiquid/lib/NET45/DotLiquid.dll"
#r "packages/Suave.DotLiquid/lib/net40/Suave.DotLiquid.dll"
#r "packages/FSharpx.Extras/lib/40/FSharpx.Extras.dll"
#r "packages/Q42.HueApi.Net/lib/net45/Q42.HueApi.NET.dll"
#r "packages/Q42.HueApi/lib/portable-net45+wp80+win81+wpa81/Q42.HueApi.dll"
#r "packages/Colourful/lib/net45/Colourful.dll"

#r @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\Facades\System.Runtime.dll"
#r @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\Facades\System.Threading.Tasks.dll"
#load "packages/FSharp.Formatting/FSharp.Formatting.fsx"
open System
open System.Web
open System.IO
open Suave
open Suave.Web
open Suave.Http
open Suave.Types
open FSharp.Data
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Web



#load "code/common/filters.fs"

#load "code/common/lightCommandExtensions.fs"

#load "code/common/hueUtils.fs"


#load "code/pages/home.fs"

open FsHue.Pages



// -------------------------------------------------------------------------------------------------
// Server entry-point and routing
// -------------------------------------------------------------------------------------------------

let browseStaticFile file ctx = async {
  let actualFile = Path.Combine(ctx.runtime.homeDirectory, "web", file)
  if System.IO.File.Exists(actualFile) then

      let mime = Suave.Http.Writers.defaultMimeTypesMap(Path.GetExtension(actualFile))
      let setMime =
        match mime with
        | None -> fun c -> async { return None }
        | Some mime -> Suave.Http.Writers.setMimeType mime.name
      return! ctx |> ( setMime >>= Successful.ok(File.ReadAllBytes actualFile) ) 
  else
      return None
       }
let browseStaticFiles ctx = async {
  let local = ctx.request.url.LocalPath
  let file = if local = "/" then "index.html" else local.Substring(1)
  return! browseStaticFile file ctx }

// Configure DotLiquid templates & register filters (in 'filters.fs')
[ for t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes() do
    if t.Name = "Filters" && not (t.FullName.StartsWith "<") then yield t ]
|> Seq.last
|> DotLiquid.registerFiltersByType

DotLiquid.setTemplatesDir (__SOURCE_DIRECTORY__ + "/templates")


let app =
  choose
    [ GET >>= choose
        [ //path "/" >>= Home.showHome
          path "/hello" >>= OK "Hello GET"
          path "/" >>= Home.showHome
          browseStaticFiles
         ]
      POST >>= choose
        [ path "/hello" >>= OK "Hello POST"
          path "/goodbye" >>= OK "Good bye POST" ] 
      ]


// -------------------------------------------------------------------------------------------------
// To run the web site, you can use `build.sh` or `build.cmd` script, which is nice because it
// automatically reloads the script when it changes. But for debugging, you can also use run or
// run with debugger in VS or XS. This runs the code below.
// -------------------------------------------------------------------------------------------------

#if INTERACTIVE
#else
let cfg =
  { defaultConfig with
      bindings = [ HttpBinding.mk' HTTP  "127.0.0.1" 8011 ]
      homeFolder = Some __SOURCE_DIRECTORY__ }
let _, server = startWebServerAsync cfg app
Async.Start(server)
System.Diagnostics.Process.Start("http://localhost:8011")
System.Console.ReadLine() |> ignore
#endif
