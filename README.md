# GraphQL Dotnet Parser
[![AppVeyor](https://img.shields.io/appveyor/ci/graphql-dotnet-ci/parser.svg)](https://ci.appveyor.com/project/graphql-dotnet-ci/parser)
[![Coverage Status](https://coveralls.io/repos/github/graphql-dotnet/parser/badge.svg?branch=master)](https://coveralls.io/github/graphql-dotnet/parser?branch=master)
[![NuGet](https://img.shields.io/nuget/v/GraphQL-Parser.svg)](https://www.nuget.org/packages/GraphQL-Parser/)
[![MyGet Pre Release](https://img.shields.io/myget/graphql-dotnet/vpre/GraphQL-Parser.svg)](https://www.myget.org/F/graphql-dotnet/api/v3/index.json)

This library contains a lexer and parser classes as well as the complete GraphQL AST model.

## Lexer
Generates token based on input text.
### Usage
```csharp
var lexer = new Lexer();
var token = lexer.Lex(new Source("\"str\""));
```
Lex method always returns the first token it finds. In this case case the result would look like following.
![lexer example](/Doc/lexer-example.png)

## Parser
Parses provided GraphQL expression into AST (abstract syntax tree).
### Usage
```csharp
var lexer = new Lexer();
var parser = new Parser(lexer);
var ast = parser.Parse(new Source(@"
{
  field
}"));
```
Json representation of the resulting AST would be:
```json
{
	"Definitions": [{
		"Directives": [],
		"Kind": 2,
		"Name": null,
		"Operation": 0,
		"SelectionSet": {
			"Kind": 5,
			"Selections": [{
				"Alias": null,
				"Arguments": [],
				"Directives": [],
				"Kind": 6,
				"Name": {
					"Kind": 0,
					"Value": "field",
					"Location": {
						"End": 50,
						"Start": 31
					}
				},
				"SelectionSet": null,
				"Location": {
					"End": 50,
					"Start": 31
				}
			}],
			"Location": {
				"End": 50,
				"Start": 13
			}
		},
		"VariableDefinitions": null,
		"Location": {
			"End": 50,
			"Start": 13
		}
	}],
	"Kind": 1,
	"Location": {
		"End": 50,
		"Start": 13
	}
}
```

## Printer
Accepts AST and converts it back into GraphQL query.
### Usage
```csharp
var lexer = new Lexer();
var parser = new Parser(lexer);
var printer = new Printer();

var ast = parser.Parse(new Source(@"
{
  field
}"));

/*
  Should result into:

  {
    field
  }
*/
var query = printer.Print(ast);
```