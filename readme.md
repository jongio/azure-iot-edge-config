# Azure IoT Edge Configuration

This repository provides sample configuration files and scripts for the Azure IoT Edge Runtime and Modules. We'll start by explaining the types of configuration files and then show you how to use them in your Edge setup.

You can find the Azure IoT Documentation [here](https://docs.microsoft.com/en-us/azure/iot-edge/).

## Configuration Details

There are two main configuration files that you need to consider when developing for Azure IoT Edge, __Runtime Configuration__ and __Module Configuration__.

### Runtime Configuration

The Azure IoT Edge Runtime is a Python script that runs on your edge device. It is responsible for pulling the module config from the cloud, pulling images, setting up routes, and running containers. When you setup the runtime, you have the option to pass in a config file, which is a json file that contains all the metadata needed to run your runtime, such as Azure Container Registry credentials, Docker settings and a custom runtime image location. 

**References**
 - [Sample Runtime Config](https://github.com/jonbgallant/azure-iot-edge-config/blob/master/config/runtimeconfig.json)
 - [Runtime Docs (iotedgectl)](https://pypi.python.org/pypi/azure-iot-edge-runtime-ctl)      

### Module Configuration

The Edge Runtime supports a module configuration that is applied via the cloud and retrieved by the runtime when it launches and whenever the config file changes. The configuration file includes all the metadata for each module on your edge device as well as the routes. You can apply it via the UI today, but sometimes you want to override everything in it via a script.

**References**
 - [Sample Module Container Config](https://github.com/jonbgallant/azure-iot-edge-config/blob/master/config/moduleconfig.json)
 
## Configuration Management

Here's what you have to do to use these custom configuration files.

### Runtime Configuration
1. Create a custom Edge runtime Configuration file that contains your custom settings, such as custom runtime location or ACR credentials.

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

### Module Configuration
1. Create a custom Edge Module configuration file

2. Apply that custom Edge Module configuration file

    There are 3 ways to apply the file to the cloud:

    #### edgeconf Script

    > Make sure you have [.NET Core Installed](https://www.microsoft.com/net/learn/get-started)

    The edgeconf script will apply your custom module config for you. See the [edgeConf Usage](#edgeConf-usage) section below for details.

    ```
    edgeconf "connection-string" device-id config-file-path
    ```

    #### SDK

    You will find a `RegistryManager.ApplyConfigurationContentOnDeviceAsync` method in the the [`modules-preview`](https://github.com/Azure/azure-iot-sdk-csharp/tree/modules-preview) branch of the C# SDK. Each of the SDKs should have this functionality in the `modules-preview` branch, for example here's the link for [Python](https://github.com/Azure/azure-iot-sdk-python/tree/modules-preview). [This file](https://github.com/jonbgallant/azure-iot-edge-config/blob/master/scripts/csharp/edgeconf/Program.cs) will assist you in creating your own script using a different SDK.  The ConfigurationContent, TwinContent and TwinCollection mapping gets a little tricky, so let me know if you run in to any issues.

    #### REST API

    You can also apply the config change via the Azure IoT REST API, the URI is: 

    ```
    POST /devices/{{deviceId}}/applyConfigurationContent?api-version=2017-11-08-preview HTTP/1.1
    Host: {{iot-hub-name}}.azure-devices.net
    Authorization: {{sas-token}}
    Content-Type: application/json
    ```

    Payload will be the modified version of the moduleconfig.json file found [here](https://github.com/jonbgallant/azure-iot-edge-config/blob/master/config/moduleconfig.json).

    See [this gist](https://gist.github.com/jonbgallant/76e577162e85f0110abc2743458d4771) for help generating the SAS Token.

## edgeconf Usage

### Runtime Config

Open `config/runtimeconfig.json`, modify it to suit your needs, and setup Azure IoT Edge Runtime with that file.

```
iotedgectl setup --config-file runtimeconfig.json
```

### Module Configs

Open `config/moduleconfig.json`, modify it to suit your needs, and use the `edgeconf` script found in `scripts/csharp/edgeconf` to apply that configuration to your device.


### Module Config Apply Script

Run the following from `scripts/csharp/edgeconf`

> Make sure you have [.NET Core Installed](https://www.microsoft.com/net/learn/get-started)


```
dotnet restore
dotnet build -r win10-x64
```

#### Script Usage
```
edgeconf iothubowner-connection-string device-id config-file-path
```

Example:
```
edgeconf "connection-string" device1 moduleconfig.json
```

You can now run docker logs and see the edgeAgent observe the config change and apply it without having to restart the Edge.

```
docker logs edgeAgent -f
```