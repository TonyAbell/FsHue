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
    lightOn(lightId: string): void;
    lightOff(lightId: string): void;
    allLightsOn(): void;
    allLightsOff(): void;

  }
  export class MainController {
    public static $inject = ['$scope', '$http'];

    constructor(private $scope: IMainScope, private $http: ng.IHttpService) {
      $http.get("/lights").then((response: ng.IHttpPromiseCallbackArg<LitghtItem[]>) => {
        $scope.lights = response.data;
      });
      $scope.allLightsOn = () => {
            this.$http.put("/turnallon", "").then(_ => { });
      };
      $scope.allLightsOff = () => {
            this.$http.put("/turnalloff", "").then(_ => { });
      };
      $scope.lightOn = (lightId) => {
        var data = "lightid=" + lightId;
        this.$http.put("/turnon", data).then(_ => { });
      };
      $scope.lightOff = (lightId) => {
        var data = "lightid=" + lightId;
        this.$http.put("/turnoff", data).then(_ => { });
      };

    }

  }
  var app = angular.module('app', []).controller('MainController', MainController);
}
