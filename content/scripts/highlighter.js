var Highlighter = (function () {
    function Highlighter() {
        var _this = this;
        this.typePrefixes = ["class", "module", "namespace", "new"];
        this.keywords = [
            "any", "as", "boolean", "break", "case", "catch", "class", "const", "constructor", "continue", "declare", "default",
            "do", "else", "enum", "export", "extends", "delete", "debugger", "default", "false", "finally", "for", "from",
            "function", "get", "if", "implements", "import", "in", "instanceof", "interface", "let", "module", "namespace", "new",
            "null", "number", "of", "private", "protected", "public", "require", "return", "set", "static", "string",
            "super", "switch", "this", "throw", "true", "try", "typeof", "var", "void", "while", "with", "yield",
            "bool", "byte", "char", "decimal", "double", "dynamic", "object", "short", "int", "long", "sbyte", "float", "string", "uint", "ushort", "ulong",
            "as", "break", "case", "class", "const", "continue", "do", "else", "enum", "false", "finally", "for", "foreach", "if", "is", "new", "null",
            "out", "ref", "return", "static", "struct", "switch", "throw", "true", "try", "typeof", "using", "var", "while"
        ];
        this.properties = [
            "Attributes", "BaseClass", "Constants", "ContainingClass", "Classes", "Default", "Enums", "Fields", "FullName", "HasGetter",
            "HasSetter", "Interfaces", "IsDate", "IsEnum", "IsEnumerable", "IsFlags", "IsGeneric", "IsGuid", "IsNullable", "IsPrimitive",
            "IsTask", "IsTimeSpan", "Methods", "Name", "name", "Namespace", "NestedClasses", "NestedEnums", "NestedInterfaces",
            "OriginalName", "Parameters", "Parent", "Properties", "Type", "TypeArguments", "TypeParameters", "Value", "Values",
            "ClassName", "HttpMethod", "RequestData", "Route", "Unwrap", "Url"
        ];
        this.parse = function (element) {
            var isTst = element.className.indexOf("tst") > -1;
            var isCs = element.className.indexOf("cs") > -1;
            var isTs = isTst === false && element.className.indexOf("ts") > -1;
            var code = element.innerHTML.trim().replace(/</, "&lt;").replace(/>/, "&gt;");
            var properties = _this.properties.slice();
            if (isTs === false) {
                code = _this.formatCsharpMethods(code, properties);
                code = _this.formatCsharpProperties(code);
            }
            if (isCs === false) {
                code = _this.formatTypeScriptMethods(code);
                code = _this.formatTypeScriptProperties(code);
            }
            code = _this.formatTypes(code);
            code = _this.formatKeywords(code);
            code = _this.formatComments(code);
            code = _this.formatStrings(code);
            if (isTst) {
                code = _this.formatProperties(code, properties);
                code = _this.formatMatchingBraces(code, "[", "]");
                code = _this.formatMatchingBraces(code, "{", "}");
            }
            element.innerHTML = code;
        };
    }
    Highlighter.prototype.formatCsharpMethods = function (code, properties) {
        var _this = this;
        var pattern = /(\s+\w+\s+)?(\w+)(\()((\w[^\s]*\s+\w+,?\s*)*)(\))/g;
        return code.replace(pattern, function (match) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            properties.push(args[1]);
            var params = args[3].replace(/(\w[^\s]*)(\s+\w+,?\s*)/g, function (match) {
                var args = [];
                for (var _i = 1; _i < arguments.length; _i++) {
                    args[_i - 1] = arguments[_i];
                }
                return _this.formatType(args[0] || "") + args[1];
            });
            return "" + (args[0] || "") + args[1] + args[2] + params + args[5];
        });
    };
    Highlighter.prototype.formatTypeScriptMethods = function (code) {
        var _this = this;
        var pattern = /(\)\s*:\s*)([\w|$|\.|&lt;|&gt;]+)(\s*(?:=&gt;|{))/g;
        return code.replace(pattern, function (match) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            return args[0] + _this.formatType(args[1]) + args[2];
        });
    };
    Highlighter.prototype.formatCsharpProperties = function (code) {
        var _this = this;
        var pattern = /((?:public|private|protected|internal)\s+)(\w[^\s]*)(\s*\w+\s*\{\s*(?:get|set))/g;
        return code.replace(pattern, function (match) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            return args[0] + _this.formatType(args[1]) + args[2];
        });
    };
    Highlighter.prototype.formatTypeScriptProperties = function (code) {
        var _this = this;
        var pattern = /(\w+\s*:\s*)(\w[\w|\.|&lt;|&gt;]*)([^;,)]*[;,)])/g;
        return code.replace(pattern, function (match) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            return args[0] + _this.formatType(args[1]) + args[2];
        });
    };
    Highlighter.prototype.formatType = function (code) {
        var _this = this;
        if (this.keywords.indexOf(code) > -1)
            return code;
        return code.split(/(\.|&lt;|&gt;)/g).map(function (part) { return part !== "." && part.indexOf("&") !== 0 && _this.keywords.indexOf(part) < 0 ? "<span class='property'>" + part + "</span>" : part; }).join("");
    };
    Highlighter.prototype.formatTypes = function (code) {
        this.typePrefixes.map(function (p) {
            var pattern = new RegExp("(\\b" + p + "\\s+)([$\\w]+)", "g");
            code = code.replace(pattern, "$1<span class='property'>$2</span>");
        });
        return code;
    };
    Highlighter.prototype.formatKeywords = function (code) {
        this.keywords.map(function (k) {
            var pattern = new RegExp("(\\b" + k + ")([\\s\\.,;\\(\\)\\[\\&]+)", "g");
            code = code.replace(pattern, "<span class='keyword'>$1</span>$2");
        });
        return code;
    };
    Highlighter.prototype.formatProperties = function (code, properties) {
        properties.sort(function (a, b) { return b.length - a.length; }).map(function (p) {
            // filter + context
            var pattern = new RegExp("(\\$" + p + "\\()([^\\)]+)(\\)\\[)", "g");
            code = code.replace(pattern, function (match) {
                var args = [];
                for (var _i = 1; _i < arguments.length; _i++) {
                    args[_i - 1] = arguments[_i];
                }
                return args[1].indexOf("=>") > 0 ?
                    "<span class='property'>" + args[0] + "</span>" + args[1] + "<span class='property'>" + args[2] + "</span>" :
                    "<span class='property'>" + args[0] + "</span><span class='string'>" + args[1] + "</span><span class='property'>" + args[2] + "</span>";
            });
            // context
            code = code.replace(new RegExp("(\\$" + p + "\\[)", "g"), "<span class='property'>$1</span>");
            // no context
            code = code.replace(new RegExp("(\\$" + p + ")(?![\\(\\[]</span>)", "g"), "<span class='property'>$1</span>");
        });
        // code blocks
        code = code.replace(/\$\{/g, "<span class='property'>$&</span>");
        return code;
    };
    Highlighter.prototype.formatComments = function (code) {
        var _this = this;
        var pattern = /\/\/[^\n]*|\/\*[^(\*\/)+]*\*\//g;
        return code.replace(pattern, function (match) { return ("<span class='comment'>" + _this.removeFormatting(match) + "</span>"); });
    };
    Highlighter.prototype.formatStrings = function (code) {
        var _this = this;
        code = code.replace(/'[^']'/g, "<span class='string'>$&</span>");
        var pattern = /\$?"[^"]*"|`[^`]*`/g;
        code = code.replace(pattern, function (match) {
            if (match.length > 0 && match[0] !== "\"") {
                var pattern_1 = match[0] === "$" ? /(\{[^\}]*\})+/ : /(\$\{[^\}]*\})+/;
                var start = match[0] === "$" ? "{" : "${";
                return match.split(pattern_1).map(function (part) { return part.indexOf(start) === 0 ? part : "<span class='string'>" + _this.removeFormatting(part) + "</span>"; }).join("");
            }
            return "<span class='string'>" + match + "</span>";
        });
        return code;
    };
    Highlighter.prototype.removeFormatting = function (code) {
        return code.replace(/<span class='\w+'>/g, "").replace(/<\/span>/g, "");
    };
    Highlighter.prototype.escape = function (code) {
        return code.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    };
    Highlighter.prototype.formatMatchingBraces = function (code, open, close) {
        var index = -1;
        do {
            index = code.indexOf(open + "</span>", index + 1);
            if (index > -1) {
                var stack = 0;
                for (var i = index; i < code.length; i++) {
                    if (code[i] === open)
                        stack++;
                    if (code[i] === close)
                        stack--;
                    if (stack === 0) {
                        if (close === "]" && i < code.length - 1 && code[i + 1] === open) {
                            code = code.substring(0, i) + "<span class='property'>" + close + open + "</span>" + code.substring(i + 2);
                        }
                        else {
                            code = code.substring(0, i) + "<span class='property'>" + close + "</span>" + code.substring(i + 1);
                        }
                        break;
                    }
                }
            }
        } while (index > -1);
        return code;
    };
    return Highlighter;
})();
document.addEventListener("DOMContentLoaded", function (event) {
    var elements = document.querySelectorAll("pre.hl");
    var instance = new Highlighter();
    for (var i = 0; i < elements.length; i++) {
        instance.parse(elements[i]);
    }
});
