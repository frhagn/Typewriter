"use strict";

var gulp = require("gulp");
var rename = require("gulp-rename");

module.exports = function (name, newName) {

    var file = gulp.src("./bower_components/" + name).pipe(rename(newName));
    var folder = newName.substring(0, newName.length - 5);

    file.pipe(gulp.dest("./content/scripts/typings/" + folder));
};