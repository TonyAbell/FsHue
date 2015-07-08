/// <reference path="d\angularjs\angular.d.ts" />
/// <reference path="d\lib.d.ts" />


module lightsApp {
    'use strict';


      export interface IMainScope extends ng.IScope {
            name : string;
      }
      export class MainController {
            public static $inject = [ '$scope'];

  		      constructor(private $scope: IMainScope ) {
                $scope.name = 'Tony A';
            }
  	  }
      var app = angular.module('app', []).controller('MainController', MainController);


}
