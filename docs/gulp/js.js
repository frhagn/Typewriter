"use strict";

var gulp = require("gulp");
var rename = require("gulp-rename");
var uglify = require("gulp-uglify");

module.exports = function (name, newName, min) {

    var file = gulp.src("./bower_components/" + name);

    if (newName) {
        file.pipe(rename(newName));
    }

    if (min) {
        file.pipe(uglify());
    }

    file.pipe(gulp.dest("./content/scripts"));
};