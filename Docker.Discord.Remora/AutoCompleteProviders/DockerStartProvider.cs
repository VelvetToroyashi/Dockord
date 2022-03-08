using Docker.DotNet;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Autocomplete;

namespace Docker.Discord.Remora.AutoCompleteProviders;

public class DockerStartProvider : IAutocompleteProvider
{
    public string Identity => "docker::start";
    
    private readonly IDockerClient _docker;
    
    public DockerStartProvider(IDockerClient dockerClient) => _docker = dockerClient;
    
    public async ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>
        GetSuggestionsAsync(IReadOnlyList<IApplicationCommandInteractionDataOption> options, string userInput, CancellationToken ct)
    {
        var containers = await _docker.Containers.ListContainersAsync(new() { All = true });
        
        var stoppedContainers = containers.Where(c => !c.Status.Contains("Up"));

        return stoppedContainers.Select(c => new ApplicationCommandOptionChoice($"{c.ID[..12]} - {c.Image}", c.ID)).ToArray();
    }
    
}