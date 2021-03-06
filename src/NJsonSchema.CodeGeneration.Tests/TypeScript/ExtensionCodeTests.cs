﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema.CodeGeneration.TypeScript;

namespace NJsonSchema.CodeGeneration.Tests.TypeScript
{
    [TestClass]
    public class ExtensionCodeTests
    {
        [TestMethod]
        public void When_extension_code_is_processed_then_code_and_classes_are_correctly_detected_and_converted()
        {
            //// Arrange
            var code =
@"
/// <reference path=""../../typings/angularjs/angular.d.ts"" />

import generated = require(""foo/bar"");
import foo = require(""foo/bar"");
import bar = require(""foo/bar"");

export class Bar extends generated.BarBase {

}

var clientClasses = {clientClasses};
for (var clientClass in clientClasses) {
    if (clientClasses.hasOwnProperty(clientClass)) {
        angular.module('app').service(clientClass, ['$http', clientClasses[clientClass]]);
    } 
}

class Foo extends generated.FooBase {

}

export class Test {

}

var x = 10;";

            //// Act
            var ext = new TypeScriptExtensionCode(code, new []{ "Foo", "Bar" });

            //// Assert
            Assert.IsTrue(ext.ExtensionClasses.ContainsKey("Foo"));
            Assert.IsTrue(ext.ExtensionClasses["Foo"].StartsWith("export class Foo extends FooBase {"));

            Assert.IsTrue(ext.ExtensionClasses.ContainsKey("Bar"));
            Assert.IsTrue(ext.ExtensionClasses["Bar"].StartsWith("export class Bar extends BarBase {"));

            Assert.IsTrue(ext.ImportCode.Contains("<reference path"));
            Assert.IsTrue(ext.ImportCode.Contains("import foo = require(\"foo/bar\")"));
            Assert.IsTrue(ext.ImportCode.Contains("import bar = require(\"foo/bar\")"));

            Assert.IsTrue(ext.BottomCode.StartsWith("var clientClasses"));
            Assert.IsTrue(ext.BottomCode.Contains("if (clientClasses.hasOwnProperty(clientClass))"));
            Assert.IsTrue(ext.BottomCode.Contains("export class Test"));
            Assert.IsTrue(ext.BottomCode.EndsWith("var x = 10;"));
        }
    }
}
