/// <reference path="d\jquery\jquery.d.ts" />
/// <reference path="d\angularjs\angular.d.ts" />
function greeter(person) {
    return "Hello, " + person.firstname + " " + person.lastname;
}
var user = { firstname: "Jane", lastname: "User" };
document.body.innerHTML = greeter(user);
