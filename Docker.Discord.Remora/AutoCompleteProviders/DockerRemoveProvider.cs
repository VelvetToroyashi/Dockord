using Docker.DotNet;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Autocomplete;

namespace Docker.Discord.Remora.AutoCompleteProviders;

public class DockerRemoveProvider : IAutocompleteProvider
{
    public string Identity => "docker::remove";

    private readonly IDockerClient _docker;
    
    public DockerRemoveProvider(IDockerClient docker) => _docker = docker;

    public async ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>
        GetSuggestionsAsync(IReadOnlyList<IApplicationCommandInteractionDataOption> options, string userInput, CancellationToken ct = default)
    {
        var containers = await _docker.Containers.ListContainersAsync(new() { All = true });
        
        return containers.Select(c => new ApplicationCommandOptionChoice($"{c.ID[..12]} - {c.Image}", c.ID)).ToArray();
    }
    
}