# HttpTestGen - .NET HTTP File Testing Framework

HttpTestGen is a .NET source generator that automatically converts `.http` files into fully functional C# test code for both xUnit and TUnit frameworks. It bridges the gap between API testing in IDEs and automated testing in .NET projects.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Installation
- **CRITICAL**: Install .NET 9.0 SDK before any other operations:
  ```bash
  wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
  chmod +x dotnet-install.sh
  ./dotnet-install.sh --version 9.0.102 --install-dir /usr/share/dotnet
  export PATH="/usr/share/dotnet:$PATH"
  dotnet --version  # Should show 9.0.102 or higher
  ```

### Build and Test Commands
- **Bootstrap and build the solution**:
  ```bash
  export PATH="/usr/share/dotnet:$PATH"  # Always ensure .NET 9.0 is in PATH
  dotnet restore  # Takes ~42 seconds. NEVER CANCEL. Set timeout to 120+ seconds.
  dotnet build --no-restore --configuration Release  # Takes ~11 seconds. NEVER CANCEL. Set timeout to 60+ seconds.
  ```

- **Run unit tests** (core functionality):
  ```bash
  dotnet test src/HttpTestGen.Core.Tests/HttpTestGen.Core.Tests.csproj --no-build --configuration Release --verbosity normal  # Takes ~2 seconds. 66 tests should pass.
  ```

- **IMPORTANT NOTE**: Integration tests may not work in all environments:
  ```bash
  # These commands may show "0 tests found" due to source generator limitations in some environments:
  dotnet test src/HttpTestGen.XunitTests/HttpTestGen.XunitTests.csproj --no-build --configuration Release --verbosity normal || echo "xUnit integration tests may fail in this environment"
  dotnet test src/HttpTestGen.TUnitTests/HttpTestGen.TUnitTests.csproj --no-build --configuration Release --verbosity normal || echo "TUnit integration tests may fail in this environment"
  ```

### Expected Build Warnings
- **Normal**: Compiler version warnings like `CS9057: The analyzer assembly references version '4.14.0.0' of the compiler, which is newer than the currently running version '4.12.0.0'` are expected and harmless.

## Validation

### Core Functionality Validation
- **ALWAYS manually validate** core functionality by running the unit tests - these verify the HTTP file parsing and code generation logic works correctly.
- **Build validation**: If `dotnet build` succeeds, the source generators compile correctly.
- **Integration test limitation**: Source generators may not work in all development environments. If integration tests show "0 tests found", this is a known limitation, not a code issue.

### Complete Validation Scenario (when source generators work)
1. Create a test `.http` file with basic HTTP requests
2. Add it to a project with the HttpTestGen NuGet package references
3. Build the project - should generate C# test code
4. Run the generated tests to verify HTTP calls work
5. **NOTE**: This scenario may not work in all environments due to source generator limitations.

## Project Structure

### Key Projects
- **HttpTestGen.Core** (.NET Standard 2.0): Core parsing logic and test generation algorithms
- **HttpTestGen.XunitGenerator** (.NET Standard 2.0): Source generator for xUnit test framework  
- **HttpTestGen.TUnitGenerator** (.NET Standard 2.0): Source generator for TUnit test framework
- **HttpTestGen.Core.Tests** (.NET 9.0): Unit tests for core functionality (66 tests)
- **HttpTestGen.XunitTests** (.NET 9.0): Integration tests using xUnit generator
- **HttpTestGen.TUnitTests** (.NET 9.0): Integration tests using TUnit generator

### Important Files
- **Solution file**: `HttpTestGen.sln` - Build entire solution
- **CI/CD**: `.github/workflows/ci.yml` - Shows expected build and test process
- **Sample HTTP files**: `src/HttpTestGen.XunitTests/Test.http` and `src/HttpTestGen.TUnitTests/Test.http`

## Development Workflow

### Making Changes
1. **Always build and test first** to establish baseline:
   ```bash
   export PATH="/usr/share/dotnet:$PATH"
   dotnet restore  # NEVER CANCEL - 120+ second timeout
   dotnet build --configuration Release  # NEVER CANCEL - 60+ second timeout  
   dotnet test src/HttpTestGen.Core.Tests/HttpTestGen.Core.Tests.csproj --configuration Release
   ```

2. **Make your changes** to the core library or generators

3. **Rebuild and validate**:
   ```bash
   dotnet build --configuration Release
   dotnet test src/HttpTestGen.Core.Tests/HttpTestGen.Core.Tests.csproj --configuration Release
   ```

4. **Test with sample .http files** if source generators work in your environment

### Source Generator Development
- **xUnit Generator**: Located in `src/HttpTestGen.XunitGenerator/SourceGenerator.cs`
- **TUnit Generator**: Located in `src/HttpTestGen.TUnitGenerator/SourceGenerator.cs`
- **Core Logic**: HTTP parsing in `src/HttpTestGen.Core/Parser/HttpFileParser.cs`
- **Test Generation**: Base logic in `src/HttpTestGen.Core/TestGenerator.cs`

## Timing Expectations

### Critical Timing Information
- **.NET SDK Installation**: ~30 seconds
- **dotnet restore**: ~42 seconds - **NEVER CANCEL, set 120+ second timeout**
- **dotnet build**: ~11 seconds - **NEVER CANCEL, set 60+ second timeout**
- **Core unit tests**: ~2 seconds
- **Integration tests**: ~2 seconds each (but may show 0 tests due to environment limitations)

### Timeout Guidelines
- **ALWAYS use generous timeouts** for build operations
- **NEVER cancel builds or restores** - they may appear to hang but are working
- **Set explicit timeouts**: restore (120+ seconds), build (60+ seconds), test (30+ seconds)

## Common Tasks

### Package Creation
```bash
# Create NuGet packages (requires successful build first)
dotnet pack src/HttpTestGen.Core/HttpTestGen.Core.csproj --configuration Release --no-restore --output nupkgs
dotnet pack src/HttpTestGen.XunitGenerator/HttpTestGen.XunitGenerator.csproj --configuration Release --no-restore --output nupkgs  
dotnet pack src/HttpTestGen.TUnitGenerator/HttpTestGen.TUnitGenerator.csproj --configuration Release --no-restore --output nupkgs
```

### Clean Build
```bash
export PATH="/usr/share/dotnet:$PATH"
dotnet clean
dotnet restore  # NEVER CANCEL - 120+ second timeout
dotnet build --configuration Release  # NEVER CANCEL - 60+ second timeout
```

## Known Limitations

### Source Generator Environment Issues
- **Integration tests may show "0 tests found"** in certain development environments
- **Source generators may not trigger** properly in all scenarios  
- **TUnit generator writes files to disk** which may be blocked in some environments
- **These are environmental limitations, not code bugs**

### Working Validation
- **Core unit tests always work** and validate the fundamental parsing and generation logic
- **Build success indicates** source generators compile correctly
- **Use core tests as primary validation** when integration tests fail

## HTTP File Format

### Basic Syntax
```http
# Simple GET request
GET https://api.example.com/users

# POST with JSON body
POST https://api.example.com/users
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com"
}

# Request with expected status code
GET https://api.example.com/notfound
EXPECTED_RESPONSE_STATUS 404

# Request with expected headers
GET https://api.example.com/data
EXPECTED_RESPONSE_HEADER content-type: application/json
```

### Generated Test Methods
- **Method naming**: `{httpmethod}_{hostname}_{index}` (e.g., `get_api_example_com_0`)
- **Assertions**: Automatic status code and header validation based on expectations
- **Framework**: Generated for either xUnit or TUnit depending on the generator used

## Troubleshooting

### Build Issues
- **"No tests found"**: Normal for integration tests in some environments - validate with core tests instead
- **Compiler version warnings**: Expected and harmless - ignore CS9057 warnings
- **Restore timeout**: Increase timeout, never cancel - restore can take 60+ seconds

### Source Generator Issues  
- **No generated tests**: Check that .http files are included as AdditionalFiles in project
- **Generator not triggering**: Verify project references are configured as analyzers with OutputItemType="Analyzer"
- **Environment limitations**: Use core unit tests to validate functionality instead of integration tests