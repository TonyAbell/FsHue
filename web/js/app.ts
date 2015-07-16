/// <reference path="d\angularjs\angular.d.ts" />
/// <reference path="d\lib.d.ts" />


module lightsApp {
  'use strict';

  export class LitghtItem {
    constructor(
      public lightId: string,
      public lightName: string
      ) { }
  }


  export interface ILightScope extends ng.IScope {
    lights: LitghtItem[];
    lightOn(lightId: string): void;
    lightOff(lightId: string): void;
    allLightsOn(): void;
    allLightsOff(): void;
    status: boolean;
    changeStatus() : void;
  }
  export class LightController {
    public static $inject = ['$scope', '$http'];

    constructor(private $scope: ILightScope, private $http: ng.IHttpService) {
      $http.get("/lights").then((response: ng.IHttpPromiseCallbackArg<LitghtItem[]>) => {
        $scope.lights = response.data;
      });
      $scope.status = true;
      $scope.changeStatus = () => {
        $scope.status = !$scope.status;
      };
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
  export class GroupItem {
    constructor(
      public groupId: string,
      public groupName: string,
      public isOn : boolean
      ) { }
  }
  export interface IGroupScope extends ng.IScope {
    groups: GroupItem[];
    groupOn(groupId: string): void;
    groupOff(groupId: string): void;
  }
  export class GroupController {
    public static $inject = ['$scope', '$http'];

    constructor(private $scope: IGroupScope, private $http: ng.IHttpService) {
      $http.get("/groups").then((response: ng.IHttpPromiseCallbackArg<GroupItem[]>) => {
        $scope.groups = response.data;
      });
      $scope.groupOn = (groupId) => {

        for (let i = 0; i < $scope.groups.length; i++) {
            if ($scope.groups[i].groupId == groupId){
              $scope.groups[i].isOn = true;
            }
        }

        var data = "groupid=" + groupId;
        this.$http.put("/turnon", data).then(_ => { });
      };
      $scope.groupOff = (groupId) => {
        for (let i = 0; i < $scope.groups.length; i++) {
            if ($scope.groups[i].groupId == groupId){
              $scope.groups[i].isOn = false;
            }
        }
        var data = "groupid=" + groupId;
        this.$http.put("/turnoff", data).then(_ => { });
      };
    }

  }
  var app = angular.module('app', ["kendo.directives"])
                   .controller('LightController', LightController)
                   .controller('GroupController', GroupController);
}
