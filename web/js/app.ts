/// <reference path="d\angularjs\angular.d.ts" />
/// <reference path="d\lib.d.ts" />


module lightsApp {
  'use strict';

  export class LitghtItem {
    constructor(
      public id: string,
      public name: string
      ) { }
  }

  export interface IMainScope extends ng.IScope {
    lights: LitghtItem[];
    vm: MainController
  }
  export class MainController {
    public static $inject = ['$scope', '$http'];

    constructor(private $scope: IMainScope, private $http: ng.IHttpService) {
      $scope.vm = this;
      $http.get("/lights").then((response: ng.IHttpPromiseCallbackArg<LitghtItem[]>) => {
        $scope.lights = response.data;
      });
    }
    public lightOn(lightId: string) {
      var data = "lightid=" + lightId;
      this.$http.put("/turnon", data).then(_ => { });
    }
    public lightOff(lightId: string) {
      var data = "lightid=" + lightId;
      this.$http.put("/turnoff", data).then(_ => { });
    }
    public allLightsOn() {
      this.$http.put("/turnallon", "").then(_ => { });
    }
    public allLightsOff() {
      this.$http.put("/turnalloff", "").then(_ => { });
    }
  }
  var app = angular.module('app', []).controller('MainController', MainController);
}
