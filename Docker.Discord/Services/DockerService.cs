using System;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Docker.Discord.Services;

public class DockerService
{
	private readonly DockerClient _docker;

	public DockerService()
    {
	    var dockerEndpoint = "unix:/var/run/docker.sock";

	    if (OperatingSystem.IsWindows())
		    dockerEndpoint = "npipe://./pipe/docker_engine";
	    
	    _docker = new DockerClientConfiguration(new Uri(dockerEndpoint)).CreateClient();
    }

	public Task RunImageAsync(string image, string tag = "latest")
		=> _docker.Images.CreateImageAsync(new()
		{
			FromImage = image,
            Tag = tag
        }, new(), new Progress<JSONMessage>());



}