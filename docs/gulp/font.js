"use strict";

var gulp = require("gulp");

module.exports = function (name) {

    gulp.src("./bower_components/" + name).pipe(gulp.dest("./content/fonts"));
};