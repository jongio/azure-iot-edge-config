# Azure IoT Edge Configuration

This repo provides sample configuration files and scripts for Azure IoT Edge Runtime and Azure IoT Edge Modules.

## Usage

### Runtime Config

Open `config/agentconfig.json` and setup Azure IoT Edge Runtime with that file.

```
iotedgectl setup --config-file agentconfig.json
```

### Module Config

Open `config/moduleconfig.json`, modify it to suit your needs, and use the `edgeconf` script found in `scripts/csharp/edgeconf` to apply that configuration to your device.


### Module Config Apply Script

Run the following from `scripts/csharp/edgeconf`

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