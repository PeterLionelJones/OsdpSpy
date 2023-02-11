# OSDPSPY: An Analysis Tool for OSDP

## Introduction

OSDPSPY is a verbose logging tool that dissects OSDP traffic with the following features:

- Listens to the RS485 bus and decodes the frames received
- Logs to console by default
- Can log to an osdpcap file
- Can log to Elasticsearch
- Can log to Seq
- Can capture a secure channel key exchange and decrypt secure channel traffic

OSDPSPY can be run on any machine that supports .NET 6: Windows, MacOS and Linux. 

## Installing OSDPSPY

1) Install .NET 6 by following the instructions at: [Download .NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2) Type the following command: **dotnet tool install -g osdpspy**
3) To verify the version, type: **osdpspy -v**
4) To see the list of available commands, type: **osdpspy -h**

