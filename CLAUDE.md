# Claude Instructions for Human-Readable-Calculation-Steps-Dotnet

## Project Configuration

### .NET Version
**IMPORTANT**: Keep all project files targeting **.NET 8.0**. Do not upgrade to .NET 9.0 or any other version unless explicitly requested by the user.

- Target Framework: `net8.0`
- This applies to both:
  - `/HumanReadableCalculationSteps/HumanReadableCalculationSteps.csproj`
  - `/HumanReadableCalculationSteps.Tests/HumanReadableCalculationSteps.Tests.csproj`

### Reason
The project is configured for .NET 8.0 for compatibility and stability reasons. Changing the target framework may cause issues with dependencies, CI/CD pipelines, or deployment environments.

## Project Conventions

1. **Testing**: All new features should have comprehensive unit tests
2. **Naming**: Follow existing naming conventions in the codebase
3. **Operators**: When implementing operators, maintain consistency with existing patterns
4. **Documentation**: Keep code self-documenting with clear method and class names

## Development Guidelines

- Run tests before committing any changes
- Maintain backward compatibility when adding new features
- Follow existing code style and formatting