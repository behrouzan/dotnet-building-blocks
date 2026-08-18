# Behrouzan .NET Building Blocks

Reusable building blocks and libraries for .NET applications.

This repository contains independent NuGet packages designed to solve common application concerns without repeatedly implementing the same infrastructure across projects.

Each package is independently installable and focuses on a specific responsibility.

## Packages

### Behrouzan.Results

A lightweight, framework-independent Result pattern library for representing successful operations and expected application failures.

Key features include:

- `Result`
- `Result<T>`
- Structured errors
- Multiple errors
- Validation errors
- Property paths
- Error codes and types
- Metadata
- `Match`
- `Map`
- `Bind`
- `Combine`

Package documentation:

[`src/Behrouzan.Results/README.md`](src/Behrouzan.Results/README.md)

---

### Behrouzan.Results.AspNetCore

ASP.NET Core integration for `Behrouzan.Results`.

Key features include:

- `Result` to `IResult` conversion
- `Result<T>` to `IResult` conversion
- Problem Details responses
- Automatic HTTP status-code mapping
- Configurable status-code mappings
- Configurable problem type identifiers
- Structured HTTP errors
- Request trace identifiers
- Dependency injection integration

Package documentation:

[`src/Behrouzan.Results.AspNetCore/README.md`](src/Behrouzan.Results.AspNetCore/README.md)

## Repository Structure

```text
src/
  Behrouzan.Results/
  Behrouzan.Results.AspNetCore/

tests/
  Behrouzan.Results.Tests/
  Behrouzan.Results.AspNetCore.Tests/

samples/
  Sample.Api/

docs/
```

Additional building blocks may be added as independent packages while remaining part of this repository.

## Development

Build the solution:

```bash
dotnet build
```

Run all tests:

```bash
dotnet test
```

Create NuGet packages:

```bash
dotnet pack -c Release
```

## Documentation

Additional documentation is available in the [`docs`](docs) directory.

Current documentation includes:

- [`Error`](docs/error-model.md)
- [`Result`](docs/result.md)
- [`Result<T>`](docs/result-of-t.md)

## Status

This repository is under active development.

Individual packages may have different versions and release cycles.

Public APIs may change before their respective `1.0.0` releases.

## License

Licensed under the MIT License.