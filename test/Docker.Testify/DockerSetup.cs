using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Docker.Testify
{
    public abstract class DockerSetup : IDisposable
    {
        public abstract string ImageName { get; }
        public virtual string ContainerPrefix => "tests";
        public abstract int InternalPort { get; }

        public virtual string ImageTag => "latest";
        public virtual TimeSpan TimeOut => TimeSpan.FromSeconds(60);
        public virtual IList<string> EnvironmentVariables => new List<string>();
        public int ExternalPort { get; }

        public abstract bool TestReady();
        public abstract void PublishConnectionInfo();

        protected readonly DockerClient docker;
    	protected string containerId;

        private static HashSet<int> UsedPorts = new HashSet<int>();

        protected DockerSetup()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    			docker = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
    		else
    			docker = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();

            ExternalPort = GetFreePort();

            Debug.WriteLine($"Selected port {ExternalPort}");

            StartContainer().Wait();
    	}

        public async Task StartContainer()
        {
            var hostCfg = new HostConfig();
            var pb = new PortBinding
            {
                HostIP = "0.0.0.0",
                HostPort = ExternalPort.ToString()
            };

            hostCfg.PortBindings = new Dictionary<string, IList<PortBinding>>();
            hostCfg.PortBindings.Add($"{InternalPort}/tcp", new PortBinding[] { pb });

            await PullImage(ImageName, ImageTag);	        

            var container = await docker.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = $"{ImageName}:{ImageTag}",
                Name = $"{ContainerPrefix}-{Guid.NewGuid()}",
                HostConfig = hostCfg,
                Env = EnvironmentVariables
            });

            Debug.WriteLine("Starting docker container...");
            var started = await docker.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
            if (started)
            {
                containerId = container.ID;
                PublishConnectionInfo();

                Debug.WriteLine("Waiting service to start in the docker container...");
                
                var ready = false;
                var expiryTime = DateTime.Now.Add(TimeOut);

                while ((DateTime.Now < expiryTime) && (!ready))
                {
                    await Task.Delay(1000);
                    ready = TestReady();
                }

                if (ready)
                {
                    Debug.WriteLine($"Docker container started: {container.ID}");
                }
                else
                {
                    Debug.WriteLine("Docker container timeout waiting for service");
                    throw new TimeoutException();
                }
            }
            else
            {
                Debug.WriteLine("Docker container failed");
            }
        }

        public async Task PullImage(string name, string tag)
        {
            var images = docker.Images.ListImagesAsync(new ImagesListParameters()).Result;
            var exists = images
                .Where(x => x.RepoTags != null)
                .Any(x => x.RepoTags.Contains($"{name}:{tag}"));

            if (exists)
                return;

            Debug.WriteLine($"Pulling docker image {name}:{tag}");
            await docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = name, Tag = tag }, null, new Progress<JSONMessage>());
        }

        public void Dispose()
    	{
    	    docker.Containers.KillContainerAsync(containerId, new ContainerKillParameters()).Wait();
    	    docker.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { Force = true }).Wait();
    	}

        private int GetFreePort()
        {
            return 8081;
	    }
    }
}
