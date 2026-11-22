
using System.Xml.Linq;

namespace BuildingHub.ArchitectureTests;

public class LayerDependencyTests
{
    [Fact]
    public void Domain_ShouldNotDependOnApplicationOrInfrastructure()
    {
        AssertNoForbiddenLayerReferences(
            sourceLayer: "Domain",
            forbiddenLayers: ["Application", "Infrastructure"]);
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructure()
    {
        AssertNoForbiddenLayerReferences(
            sourceLayer: "Application",
            forbiddenLayers: ["Infrastructure"]);
    }

    private static void AssertNoForbiddenLayerReferences(
        string sourceLayer,
        string[] forbiddenLayers)
    {
        var root = FindSolutionRoot();

        var modulesRoot = Path.Combine(
            root, "src", "Modules");

        var projects = Directory.GetFiles(
            modulesRoot,
            "*.csproj",
            SearchOption.AllDirectories);

        var violations = new List<string>();

        foreach (var project in projects)
        {
            var sourceName = Path.GetFileNameWithoutExtension(project);

            if (!sourceName.EndsWith(
                    "." + sourceLayer,
                    StringComparison.OrdinalIgnoreCase))
                continue;

            var document = XDocument.Load(project);

            foreach (var reference in document.Descendants("ProjectReference"))
            {
                var include = reference.Attribute("Include")?.Value;

                if (string.IsNullOrWhiteSpace(include))
                    continue;

                var targetPath = Path.GetFullPath(
                    Path.Combine(
                        Path.GetDirectoryName(project)!,
                        include));

                var targetName =
                    Path.GetFileNameWithoutExtension(targetPath);

                foreach (var forbiddenLayer in forbiddenLayers)
                {
                    if (targetName.EndsWith(
                            "." + forbiddenLayer,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        violations.Add(
                            $"{sourceName} -> {targetName}");
                    }
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Forbidden layer dependencies detected:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(
            Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            if (File.Exists(
                Path.Combine(
                    directory.FullName,
                    "BuildingHub.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "BuildingHub.sln was not found.");
    }
}
