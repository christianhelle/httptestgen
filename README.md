# HttpTestGen - .http File Testing Framework

A powerful .NET source generator that automatically converts `.http` files into fully functional C# test code. This tool bridges the gap between API testing in IDEs (like Visual Studio Code with the REST Client extension) and automated testing in your .NET projects.

## 🚀 Features

- **Automatic Test Generation**: Transform `.http` files into xUnit or TUnit test code at compile time
- **Rich HTTP Support**: Parse GET, POST, PUT, DELETE, PATCH, HEAD, OPTIONS, TRACE, and CONNECT methods
- **Header Processing**: Full support for HTTP headers including custom headers
- **Request Bodies**: Support for JSON, XML, and text request bodies
- **Response Assertions**: Validate expected status codes, response headers, and response body content
- **Variables**: Declare and use variables with `@variable = value` and `{{variable}}` substitution
- **Dynamic Functions**: Built-in functions for generating random data (`{{guid()}}`, `{{name()}}`, `{{email()}}`, etc.)
- **Request Directives**: Control test behavior with `@name`, `@timeout`, `@dependsOn`, `@pre-delay`, `@post-delay`, `@if`/`@if-not`
- **Multiple Test Frameworks**: Generate tests for xUnit and TUnit
- **Source Generator**: Zero-runtime overhead with compile-time code generation
- **IDE Integration**: Works seamlessly with existing `.http` files in your IDE
- **Comment Styles**: Support for both `#` and `//` comment styles
- **IntelliJ Compatibility**: Ignores JetBrains HTTP Client script blocks (`> {% ... %}`)

## 📦 Installation

### xUnit Generator

```xml
<PackageReference Include="HttpTestGen.XunitGenerator" Version="1.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

### TUnit Generator

```xml
<PackageReference Include="HttpTestGen.TUnitGenerator" Version="1.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

## 🔧 Usage

### Basic .http File Syntax

Create a `.http` file in your test project with HTTP requests:

```http
# Simple GET request
GET https://api.example.com/users

# GET request with headers
GET https://api.example.com/users/123
Accept: application/json
Authorization: Bearer your-token-here

# POST request with JSON body
POST https://api.example.com/users
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com"
}

# Request with expected status code
GET https://api.example.com/nonexistent
EXPECTED_RESPONSE_STATUS 404

# Request with expected response headers
GET https://api.example.com/data
EXPECTED_RESPONSE_HEADER content-type: application/json
EXPECTED_RESPONSE_HEADER x-custom-header: custom-value
```

### Variables

Declare variables at the top of your `.http` file and reference them with `{{variable}}` syntax:

```http
@host = api.example.com
@baseUrl = https://{{host}}/v1
@token = my-auth-token

GET {{baseUrl}}/users
Authorization: Bearer {{token}}

###

POST {{baseUrl}}/users
Content-Type: application/json
Authorization: Bearer {{token}}

{
  "name": "John Doe"
}
```

Variables support chaining — a variable can reference another variable that was declared earlier.

### Dynamic Function Variables

Use built-in functions to generate dynamic values at parse time:

```http
POST https://api.example.com/users
Content-Type: application/json

{
  "id": "{{guid()}}",
  "name": "{{name()}}",
  "email": "{{email()}}",
  "job": "{{job_title()}}",
  "address": "{{address()}}",
  "created": "{{getdatetime()}}"
}
```

#### Available Functions

| Function | Description | Example Output |
|----------|-------------|----------------|
| `{{guid()}}` | UUID v4 (32 hex chars, no hyphens) | `a1b2c3d4e5f6789012345678abcdef01` |
| `{{string()}}` | 20-character random alphanumeric | `f47ac10b58cc4372a567` |
| `{{number()}}` | Random integer 0–100 | `42` |
| `{{name()}}` | Random full name | `James Smith` |
| `{{first_name()}}` | Random first name | `Emily` |
| `{{last_name()}}` | Random last name | `Johnson` |
| `{{email()}}` | Random email address | `james.smith@example.com` |
| `{{address()}}` | Random street address | `123 Main St, New York, NY 10001` |
| `{{job_title()}}` | Random job title | `Software Engineer` |
| `{{getdate()}}` | Current date (YYYY-MM-DD) | `2025-01-15` |
| `{{gettime()}}` | Current time (HH:MM:SS) | `14:30:45` |
| `{{getdatetime()}}` | Local datetime | `2025-01-15 14:30:45` |
| `{{getutcdatetime()}}` | UTC datetime | `2025-01-15 12:30:45` |
| `{{base64_encode('text')}}` | Base64 encode text | `dGV4dA==` |
| `{{upper('text')}}` | Convert to uppercase | `TEXT` |
| `{{lower('TEXT')}}` | Convert to lowercase | `text` |
| `{{lorem_ipsum(50)}}` | Lorem ipsum (N words) | `lorem ipsum dolor sit amet...` |

### Request Directives

Control request behavior using directives in comments before the request line:

```http
# @name loginRequest
# @timeout 5s
# @pre-delay 500
POST https://api.example.com/auth
Content-Type: application/json

{
  "username": "admin",
  "password": "secret"
}

###

# @name getProfile
# @dependsOn loginRequest
# @if loginRequest.response.status 200
# @timeout 10s
# @post-delay 1000
GET https://api.example.com/profile
Authorization: Bearer my-token
```

#### Available Directives

| Directive | Format | Description |
|-----------|--------|-------------|
| `@name` | `# @name requestName` | Names the request (used for test method naming and dependencies) |
| `@timeout` | `# @timeout 5s` | Sets request timeout (supports `ms`, `s`, `m` units) |
| `@connection-timeout` | `# @connection-timeout 3s` | Sets connection establishment timeout |
| `@dependsOn` | `# @dependsOn requestName` | Declares dependency on another request |
| `@if` | `# @if request.response.status 200` | Conditional execution (positive) |
| `@if-not` | `# @if-not request.response.status 401` | Conditional execution (negative) |
| `@pre-delay` | `# @pre-delay 500` | Delay in milliseconds before the request |
| `@post-delay` | `# @post-delay 1000` | Delay in milliseconds after the request |

All directives support both `#` and `//` comment prefixes.

### Generated Test Code

The source generator automatically creates test methods from your `.http` files. For example, the above `.http` file would generate:

#### xUnit Output
```csharp
public class ApiTestsXunitTests
{
    [Xunit.Fact]
    public async Task get_api_example_com_0()
    {
        var sut = new System.Net.Http.HttpClient();
        var response = await sut.GetAsync("https://api.example.com/users");
        Xunit.Assert.True(response.IsSuccessStatusCode);
    }

    [Xunit.Fact]
    public async Task get_api_example_com_1()
    {
        var sut = new System.Net.Http.HttpClient();
        var response = await sut.GetAsync("https://api.example.com/users/123");
        Xunit.Assert.True(response.IsSuccessStatusCode);
    }

    [Xunit.Fact]
    public async Task post_api_example_com_2()
    {
        var sut = new System.Net.Http.HttpClient();
        var content = new System.Net.Http.StringContent("{...}", System.Text.Encoding.UTF8, "application/json");
        var response = await sut.PostAsync("https://api.example.com/users", content);
        Xunit.Assert.True(response.IsSuccessStatusCode);
    }

    [Xunit.Fact]
    public async Task get_api_example_com_3()
    {
        var sut = new System.Net.Http.HttpClient();
        var response = await sut.GetAsync("https://api.example.com/nonexistent");
        Xunit.Assert.Equal(404, (int)response.StatusCode);
    }

    [Xunit.Fact]
    public async Task get_api_example_com_4()
    {
        var sut = new System.Net.Http.HttpClient();
        var response = await sut.GetAsync("https://api.example.com/data");
        Xunit.Assert.True(response.IsSuccessStatusCode);
        Xunit.Assert.True(response.Headers.GetValues("content-type").Contains("application/json"));
        Xunit.Assert.True(response.Headers.GetValues("x-custom-header").Contains("custom-value"));
    }
}
```

When `@name` is used, the test method uses the request name instead of the auto-generated pattern:

```csharp
    [Xunit.Fact]
    public async Task loginRequest()
    {
        var sut = new System.Net.Http.HttpClient();
        sut.Timeout = System.TimeSpan.FromMilliseconds(5000);
        await System.Threading.Tasks.Task.Delay(500);
        var content = new System.Net.Http.StringContent("{...}", System.Text.Encoding.UTF8, "application/json");
        var response = await sut.PostAsync("https://api.example.com/auth", content);
        Xunit.Assert.True(response.IsSuccessStatusCode);
    }
```

## 📋 Supported HTTP Methods

- `GET` - Retrieve data
- `POST` - Create new resources
- `PUT` - Update existing resources
- `PATCH` - Partial updates
- `DELETE` - Remove resources
- `HEAD` - Retrieve headers only
- `OPTIONS` - Check available methods
- `TRACE` - Diagnostic trace
- `CONNECT` - Establish tunnel

## 🎯 Assertion Syntax

### Status Code Assertions
```http
GET https://api.example.com/notfound
EXPECTED_RESPONSE_STATUS 404
```

### Header Assertions
```http
GET https://api.example.com/api/data
EXPECTED_RESPONSE_HEADER content-type: application/json
EXPECTED_RESPONSE_HEADER cache-control: no-cache
```

### Body Assertions
```http
GET https://api.example.com/health
EXPECTED_RESPONSE_BODY "healthy"
```

### Assertion Prefix

Assertions can optionally be prefixed with `> ` for compatibility with other HTTP clients:

```http
GET https://api.example.com/
> EXPECTED_RESPONSE_STATUS 200
> EXPECTED_RESPONSE_BODY "ok"
> EXPECTED_RESPONSE_HEADER content-type: application/json
```

## 🏗️ Request Separators

Separate multiple requests with `###` (recommended) or `#`/`##`:

```http
GET https://api.example.com/users

###

POST https://api.example.com/users
Content-Type: application/json

{
  "name": "Test User"
}

###

DELETE https://api.example.com/users/1
```

## 🏗️ Project Integration

1. **Add the NuGet package** to your test project
2. **Create `.http` files** in your test project
3. **Build your project** - tests are generated automatically
4. **Run tests** using your preferred test runner

### Example Project Structure
```
MyProject.Tests/
├── MyProject.Tests.csproj
├── api-tests.http
├── user-tests.http
└── integration-tests.http
```

The source generator will create corresponding test classes:
- `api-tests.http` → `ApiTestsXunitTests` or `ApiTestsTests` (TUnit)
- `user-tests.http` → `UserTestsXunitTests` or `UserTestsTests` (TUnit)
- `integration-tests.http` → `IntegrationTestsXunitTests` or `IntegrationTestsTests` (TUnit)

## 🛠️ Advanced Features

### Comments and Documentation

Both `#` and `//` comment styles are supported:

```http
# This is a hash comment
// This is a double-slash comment
GET https://api.example.com/users

// Test user creation
POST https://api.example.com/users
Content-Type: application/json

{
  "name": "Test User"
}
```

### Request Bodies
Support for various content types:

```http
# JSON body
POST https://api.example.com/data
Content-Type: application/json

{
  "key": "value"
}

# XML body  
POST https://api.example.com/data
Content-Type: application/xml

<root>
  <item>value</item>
</root>

# Plain text body
POST https://api.example.com/data
Content-Type: text/plain

This is plain text content
```

### IntelliJ HTTP Client Compatibility

JetBrains HTTP Client response handler scripts are automatically ignored:

```http
GET https://api.example.com/users
> {%
    client.test("Test", function() {
        client.assert(response.status === 200);
    });
%}
```

## 📸 Visual Examples

### Running Tests in Terminal
![dotnet test](images/http-testing.png)

### Visual Studio Integration
![visual studio](images/http-testing-vs.png)

## 🔄 Development Workflow

### Typical Development Flow
1. **Design your API** using `.http` files in your IDE
2. **Test manually** using REST Client extensions
3. **Add assertions** for expected behavior
4. **Build project** to generate automated tests
5. **Run tests** in CI/CD pipeline

### Best Practices
- **Organize by feature**: Create separate `.http` files for different API endpoints or features
- **Use descriptive comments**: Document what each request tests
- **Add assertions**: Always include expected status codes and important headers
- **Use variables**: Define base URLs and tokens as variables for reusability
- **Use `@name` directives**: Name your requests for meaningful test method names
- **Use `###` separators**: Clearly separate requests in multi-request files
- **Version control**: Commit your `.http` files alongside your code

## 🏷️ Naming Conventions

Generated test method names follow the pattern: `{method}_{hostname}_{index}`

Examples:
- `GET https://api.example.com/users` → `get_api_example_com_0`
- `POST https://localhost:5000/api/data` → `post_localhost_1`
- `DELETE https://my-api.azurewebsites.net/items/123` → `delete_my_api_azurewebsites_net_2`

When `@name` is used, the name becomes the test method name directly:
- `# @name loginRequest` → `loginRequest`
- `# @name getUserById` → `getUserById`

## 🚀 Performance Considerations

- **Compile-time generation**: Zero runtime overhead
- **Incremental builds**: Only regenerates when `.http` files change
- **Parallel execution**: Generated tests can run in parallel
- **Memory efficient**: No reflection or dynamic compilation at runtime

## 🔧 Configuration

### MSBuild Properties
You can customize the behavior using MSBuild properties:

```xml
<PropertyGroup>
  <!-- Include .http files in the project -->
  <AdditionalFileItemNames>$(AdditionalFileItemNames);HttpFile</AdditionalFileItemNames>
</PropertyGroup>

<ItemGroup>
  <HttpFile Include="**/*.http" />
</ItemGroup>
```

### Conditional Compilation
Use preprocessor directives to include/exclude specific tests:

```csharp
#if DEBUG
// Debug-specific test generation
#endif
```

## 🧪 Testing Strategies

### Unit Tests vs Integration Tests
- **Unit Tests**: Test individual endpoints with mocked dependencies
- **Integration Tests**: Test complete API flows with real HTTP calls
- **Contract Tests**: Verify API contracts match expectations

### Assertion Patterns
```http
# Success scenarios
GET https://api.example.com/users
EXPECTED_RESPONSE_STATUS 200
EXPECTED_RESPONSE_HEADER content-type: application/json

# Error scenarios  
GET https://api.example.com/users/999999
EXPECTED_RESPONSE_STATUS 404

# Authentication scenarios
GET https://api.example.com/protected
Authorization: Bearer invalid-token
EXPECTED_RESPONSE_STATUS 401

# Body content verification
GET https://api.example.com/health
EXPECTED_RESPONSE_BODY "healthy"
```

## 🔗 Integration with Popular Tools

### Visual Studio Code
Works seamlessly with the [REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)

### JetBrains IDEs
Compatible with built-in HTTP client in IntelliJ IDEA, WebStorm, and Rider

### Postman
Export Postman collections to `.http` format for automated testing

## 🏗️ Build and Development

### Prerequisites
- .NET SDK 8.0 or later
- Compatible IDE with `.http` file support (optional)

### Building from Source
```bash
git clone https://github.com/christianhelle/httptestgen.git
cd httptestgen/src
dotnet build HttpTestGen.Core/HttpTestGen.Core.csproj
dotnet build HttpTestGen.XunitGenerator/HttpTestGen.XunitGenerator.csproj
dotnet build HttpTestGen.TUnitGenerator/HttpTestGen.TUnitGenerator.csproj
```

### Running Tests
```bash
# Note: Requires .NET 9 SDK for tests
dotnet test HttpTestGen.Core.Tests/HttpTestGen.Core.Tests.csproj
```

### Creating NuGet Packages
```bash
dotnet pack HttpTestGen.XunitGenerator/HttpTestGen.XunitGenerator.csproj
dotnet pack HttpTestGen.TUnitGenerator/HttpTestGen.TUnitGenerator.csproj
```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

### Development Setup
1. Fork the repository
2. Clone your fork
3. Create a feature branch
4. Make your changes
5. Add tests for new functionality
6. Submit a pull request

### Code Style
- Follow existing naming conventions
- Add XML documentation for public APIs
- Include unit tests for new features
- Update README for user-facing changes

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Inspired by the REST Client extension for Visual Studio Code
- Built on top of .NET Source Generators
- Thanks to the xUnit and TUnit communities for excellent testing frameworks
- Feature parity with [HTTP File Runner](https://github.com/christianhelle/httprunner) for .http file format support

## 🆘 Support and Troubleshooting

### Common Issues

**Q: Tests are not being generated**
A: Ensure your `.http` files are included in the project and the NuGet package is properly referenced with `PrivateAssets="all"`.

**Q: Generated tests don't compile**
A: Check that your HTTP syntax is valid and all required NuGet packages are installed.

**Q: Tests fail with connection errors**
A: Verify that the target APIs are accessible from your test environment.
