
using System.Xml.Linq;

namespace BuildingHub.ArchitectureTests;

public class ModuleDependencyTests
{
    [Fact]
    public void BusinessModules_ShouldNotReferenceOtherBusinessModules()
    {
        var solutionRoot = FindSolutionRoot();

        var modulesRoot = Path.Combine(
            solutionRoot,
            "src",
            "Modules");

        Assert.True(
            Directory.Exists(modulesRoot),
            $"Modules directory not found: {modulesRoot}");

        var projectFiles = Directory.GetFiles(
            modulesRoot,
            "*.csproj",
            SearchOption.AllDirectories);

        Assert.NotEmpty(projectFiles);

        var violations = new List<string>();

        foreach (var projectFile in projectFiles)
        {
            var sourceModule = GetModuleName(
                projectFile,
                modulesRoot);

            var document = XDocument.Load(projectFile);

            var references = document
                .Descendants("ProjectReference");

            foreach (var reference in references)
            {
                var include = reference.Attribute("Include")?.Value;

                if (string.IsNullOrWhiteSpace(include))
                    continue;

                var targetPath = Path.GetFullPath(
                    Path.Combine(
                        Path.GetDirectoryName(projectFile)!,
                        include));

                if (!targetPath.StartsWith(
                        modulesRoot + Path.DirectorySeparatorChar,
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                var targetModule = GetModuleName(
                    targetPath,
                    modulesRoot);

                if (!string.Equals(
                        sourceModule,
                        targetModule,
                        StringComparison.OrdinalIgnoreCase))
                {
                    violations.Add(
                        $"{sourceModule} -> {targetModule}: " +
                        $"{Path.GetFileName(projectFile)} references " +
                        $"{Path.GetFileName(targetPath)}");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Cross-module ProjectReference detected:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void AccountingInfrastructure_ShouldNotReferenceCommunity()
    {
        var solutionRoot = FindSolutionRoot();

        var projectFile = Path.Combine(
            solutionRoot,
            "src",
            "Modules",
            "Accounting",
            "BuildingHub.Modules.Accounting.Infrastructure",
            "BuildingHub.Modules.Accounting.Infrastructure.csproj");

        Assert.True(
            File.Exists(projectFile),
            $"Accounting Infrastructure project not found: {projectFile}");

        var document = XDocument.Load(projectFile);

        var references = document
            .Descendants("ProjectReference")
            .Select(x => x.Attribute("Include")?.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        var communityReferences = references
            .Where(x => x!.Contains(
                "Community",
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.Empty(communityReferences);
    }

    private static string GetModuleName(
        string projectPath,
        string modulesRoot)
    {
        var relativePath = Path.GetRelativePath(
            modulesRoot,
            projectPath);

        return relativePath.Split(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar)[0];
    }

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(
            Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            if (File.Exists(
                Path.Combine(directory.FullName, "BuildingHub.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "BuildingHub.sln was not found.");
    }
}
