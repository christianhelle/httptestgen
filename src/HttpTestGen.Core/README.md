# HttpTestGen.Core

Core library for HttpTestGen - A .NET source generator that converts `.http` files into C# test code.

## Overview

This package contains the core functionality for parsing `.http` files and generating test code. It provides the base classes and interfaces used by the framework-specific generators.

## Features

- **HTTP File Parsing**: Parse `.http` files with support for all HTTP methods
- **Request Processing**: Handle headers, request bodies, and assertions
- **Test Generation**: Base classes for generating test code
- **Multiple Frameworks**: Support for xUnit and TUnit test generation

## Components

- `ITestGenerator`: Interface for test generators
- `TestGenerator`: Base test generator implementation
- `HttpFileParser`: Parser for `.http` files
- `HttpFileRequest`: Represents an HTTP request from a `.http` file
- `HttpFileAssertions`: Handles response assertions

## Usage

This package is typically not used directly. Instead, use one of the framework-specific generators:

- `HttpTestGen.XunitGenerator` for xUnit tests
- `HttpTestGen.TUnitGenerator` for TUnit tests

## License

This project is licensed under the MIT License.
