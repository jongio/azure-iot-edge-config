# Azure IoT Edge Configuration

This repository provides sample configuration files for the Azure IoT Edge Runtime and Modules. We'll start by explaining the types of configuration files and then show you how to use them in your Edge setup.

**Docs**: You can find the Azure IoT Documentation [here](https://docs.microsoft.com/en-us/azure/iot-edge/).

**Scripts**: The scripts used to apply module configuration [here](https://github.com/jonbgallant/azure-iot-rest). 

## Configuration Details

There are two main configuration files that you need to consider when developing for Azure IoT Edge: __Runtime Configuration__ and __Module Configuration__.

### Runtime Configuration

The Azure IoT Edge Runtime is a Python script that runs on your edge device. It is responsible for pulling the module config from the cloud, pulling images, setting up routes, and running containers. When you setup the runtime, you have the option to pass in a config file, which is a json file that contains all the metadata needed to run your runtime, such as Azure Container Registry credentials, Docker settings and a custom runtime image location. 

#### Usage

1. Create a custom Edge runtime Configuration file that contains your custom settings. Use [this file](https://github.com/jonbgallant/azure-iot-edge-config/blob/master/config/runtimeconfig.json) as a template.

2. Start the Edge runtime with that custom configuration file.

    Here's how you start the runtime with a custom config file.

    ```
    iotedgectl setup --config-file config.json
    ```

    When you run this command, the runtime will copy the config file to c:\ProgramData\azure-iot-edge. It will overwrite any file in that location. You can then start the runtime with the start command:

    ```
    iotedgectl start
    ```

    When you start the runtime it will retrieve the config.json file you passed to the `setup` command and launch the runtime with those settings.

**References**
 - [Sample Runtime Config](https://github.com/jonbgallant/azure-iot-edge-config/blob/master/config/runtimeconfig.json)
 - [Runtime Docs (iotedgectl)](https://pypi.python.org/pypi/azure-iot-edge-runtime-ctl)      

### Module Configuration

The Edge Runtime supports a module configuration that is applied via the cloud and retrieved by the runtime when it launches and whenever the config file changes. The configuration file includes all the metadata for each module on your edge device as well as the routes. You can apply it via the UI today, but sometimes you want to override everything in it via a script.

#### Usage
1. Create a custom Edge Module configuration file. Use [this file](https://github.com/jonbgallant/azure-iot-edge-config/blob/master/config/moduleconfig.json) as a template.

2. Apply custom Edge Module configuration file

    #### device-conf Script or REST API

    The device-conf script in the [azure-iot-rest](https://github.com/jonbgallant/azure-iot-rest) repo will apply your custom module config for you. You can find details on how to use the script [here](https://github.com/jonbgallant/azure-iot-rest).

    ```
    python device-conf.py --name [iothubname] --key [iothubkey] --device-id [deviceid] --config-file [path to module config]
    ```

    #### SDK

    You will find a `RegistryManager.ApplyConfigurationContentOnDeviceAsync` method in the the [`modules-preview`](https://github.com/Azure/azure-iot-sdk-csharp/tree/modules-preview) branch of the C# SDK. Each of the SDKs should have this functionality in the `modules-preview` branch, for example here's the link for [Python](https://github.com/Azure/azure-iot-sdk-python/tree/modules-preview). [This file](https://github.com/jonbgallant/azure-iot-edge-config/blob/master/scripts/csharp/edgeconf/Program.cs) will assist you in creating your own script using a different SDK.  The ConfigurationContent, TwinContent and TwinCollection mapping gets a little tricky, so let me know if you run in to any issues.

    If you use the SDK, then you need to apply each module content individually, here's a snippet on what that looks like:

    ```
    dynamic config = JsonConvert.DeserializeObject(File.ReadAllText(filePath));

    var cc = new ConfigurationContent() { ModuleContent = new Dictionary<string, TwinContent>() };
    foreach (var module in config.moduleContent)
    {
        var twinContent = new TwinContent();
        twinContent.TargetContent = new TwinCollection(module.Value["properties.desired"].ToString());
        cc.ModuleContent[module.Name.ToString()] = twinContent;
    }

    var rm = RegistryManager.CreateFromConnectionString(connectionString);
    Task.Run(async () => { await rm.ApplyConfigurationContentOnDeviceAsync(deviceId, cc); }).GetAwaiter().GetResult();

    ```
    #### CLI

    The Azure IoT Edge CLI is in progress and will contain a function to apply configuration files.

**References**
 - [Sample Module Container Config](https://github.com/jonbgallant/azure-iot-edge-config/blob/master/config/moduleconfig.json)
 
