# Azure IoT Edge Configuration

This repo provides sample configuration files and scripts for Azure IoT Edge Runtime and Azure IoT Edge Modules.

## Usage

### Runtime Config

Open `config/agentconfig.json` and setup Azure IoT Edge Runtime with that file.

```
iotedgectl setup --config-file agentconfig.json
```

### Module Config

Open `config/moduleconfig.json` and use the `edgeconf` script found in `scripts/csharp/edgeconf` to apply that configuration to your device.

Run the following from `scripts/csharp/edgeconf`

```
dotnet restore
dotnet build
```

Usage:
```
edgeconf iothubowner-connection-string device-id config-file-path
```

Example:
```
edgeconf "connectionstring" device1 moduleconfig.json
```