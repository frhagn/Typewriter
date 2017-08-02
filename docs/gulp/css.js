"use strict";

var gulp = require("gulp");
var rename = require("gulp-rename");
var minify = require("gulp-minify-css");

module.exports = function (name, newName, min) {

    var file = gulp.src("./bower_components/" + name);

    if (newName) {
        file.pipe(rename(newName));
    }

    if (min) {
        file.pipe(minify());
    }

    file.pipe(gulp.dest("./content/stylesheets"));
}