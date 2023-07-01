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


## The Origin of OSDPSPY

OSDPSPY was originally developed to support my MSc Cybersecurity dissertation and the practical uses
of the tool can be found in this document: [Securing an OT Device](../Documentation/Securing%20an%20OT%20Device.pdf)

The document also describes the structure of this software as it was originally implemented.