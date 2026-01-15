# NuGet Trusted Publishing Setup

This repository uses [Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing) to securely publish NuGet packages without API keys.

## What is Trusted Publishing?

Trusted Publishing uses OpenID Connect (OIDC) to establish trust between GitHub Actions and NuGet.org. This eliminates the need for API keys and reduces security risks.

## Configuration Required on NuGet.org

For each NuGet package published by this repository, configure the following Trusted Publisher settings:

### Packages:
- `HttpTestGen.Core`
- `HttpTestGen.XunitGenerator`
- `HttpTestGen.TUnitGenerator`

### Trusted Publisher Configuration:
1. Log in to [NuGet.org](https://www.nuget.org)
2. Navigate to each package's management page
3. Go to **Trusted Publishers** section
4. Add a new GitHub Actions publisher with these settings:
   - **Owner**: `christianhelle`
   - **Repository**: `httptestgen`
   - **Workflow**: `release.yml`
   - **Subject identifier pattern**: `repo:christianhelle/httptestgen:ref:refs/tags/v*`

## How It Works

1. When a new tag matching `v*` is pushed, the release workflow is triggered
2. GitHub generates an OIDC token containing claims about the workflow
3. The `dotnet nuget push` command uses this token to authenticate with NuGet.org
4. NuGet.org validates the token against the configured Trusted Publisher settings
5. If validation succeeds, the package is published

## Workflow Configuration

The release workflow includes:
- `permissions.id-token: write` - Required to generate OIDC tokens
- No `--api-key` parameter in the `dotnet nuget push` command

## Benefits

✅ **Enhanced Security**: No API keys to manage or potentially leak  
✅ **Automated**: No manual key rotation required  
✅ **Auditable**: All publishes are tied to specific workflow runs  
✅ **Scoped**: Each package can have different publisher configurations

## Troubleshooting

### Error: "Unable to authenticate"
- Verify that Trusted Publisher is configured for all three packages on NuGet.org
- Check that the subject identifier pattern matches: `repo:christianhelle/httptestgen:ref:refs/tags/v*`
- Ensure the workflow has `id-token: write` permission

### Error: "Package already exists"
- This is expected with the `--skip-duplicate` flag and is not an error
- The workflow uses `continue-on-error: true` to handle this gracefully

## References

- [Microsoft: Trusted Publishing Documentation](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
- [GitHub Actions: OIDC with NuGet](https://docs.github.com/en/actions/deployment/security-hardening-your-deployments/about-security-hardening-with-openid-connect)
