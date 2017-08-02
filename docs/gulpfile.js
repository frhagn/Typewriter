"use strict";

var gulp = require("gulp");
var font = require("./gulp/font.js");
var css = require("./gulp/css.js");
var js = require("./gulp/js.js");
var dts = require("./gulp/dts.js");
var bower = require('gulp-bower');
 
gulp.task('bower', function() {
  return bower()
    .pipe(gulp.dest('bower_components/'))
});

gulp.task("copy-content", function () {

    font("font-awesome/fonts/*");

    css("font-awesome/css/*.css");
    // css("bootstrap/dist/css/bootstrap.*");
    css("bootswatch/superhero/bootstrap.css");
    css("bootswatch/superhero/bootstrap.min.css");
    
    //css("toastr/*.css");
    //css("awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css", "bootstrap_awesome-checkbox.css");
    //css("awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css", "bootstrap_awesome-checkbox.min.css", true);

    js("bootstrap/dist/js/bootstrap.*");
    //js("foundation/*.js");
    js("jquery/dist/*.js");
//    js("knockout/dist/knockout.debug.js", "knockout.js");
//    js("knockout/dist/knockout.js", "knockout.min.js");
//    js("knockout-extensions/*.js");
//    js("q/q.js");
//    js("q/q.js", "q.min.js", true);
//    js("toastr/*.js");
//    js("fastclick/lib/fastclick.js");
//    js("fastclick/lib/fastclick.js", "fastclick.min.js", true);
//
//    dts("foundation/foundation.d.ts", "foundation.d.ts");
//    dts("googlemaps.TypeScript.DefinitelyTyped/index.ts", "google-maps.d.ts");
//    dts("jquery.TypeScript.DefinitelyTyped/index.ts", "jquery.d.ts");
//    dts("knockout.TypeScript.DefinitelyTyped/index.ts", "knockout.d.ts");
//    dts("knockout-extensions/knockout-extensions.d.ts", "knockout-extensions.d.ts");
//    dts("q.TypeScript.DefinitelyTyped/index.ts", "q.d.ts");
//    dts("toastr.TypeScript.DefinitelyTyped/index.ts", "toastr.d.ts");

});