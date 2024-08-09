# IPFS Uploader

This is a command line tool that helps upload files to an [IPFS](https://ipfs.tech/) Node.

## Installing

This is deployed as a [Dotnet Tool](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-use).  Install with the following command.

```sh
dotnet tool install --global ipfs_upload
```

## Usage

Run ```ipfs_upload``` and pass in the following arguments.

```txt
Options:
  --server_url <server_url> (REQUIRED)  The URL to the server running IPFS.
  --port <port> (REQUIRED)              The port of the IPFS node's API server.  Remember when setting up a node, this
                                        port should NEVER be exposed to the internet. [default: 5001]
  --output_xml_file <output_xml_file>   Where to output an file that contains the created hashes in XML format
  --file <file> (REQUIRED)              The file to upload to IPFS.  Globs are allowed.  Directories are not.
  --print_license                       Prints this program's license to stdout, and exits.  This takes priority over
                                        all other print options. [default: False]
  --print_readme                        Prints the readme file to stdout, and exits. This takes priority over the
                                        --print_credits option. [default: False]
  --print_credits                       Prints the third-party licenses to stdout, and exits. [default: False]
  --version                             Show version information
  -?, -h, --help                        Show help and usage information
```

Remember, the "port" on your IPFS node should **NEVER** be exposed to the public internet, as it controls the IPFS node.

The output_xml_file's format looks like the following:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Files>
  <File name="BD240401.jpg">
    <IpfsHash>QmV4nrqxNE5FZdbJRCBJimtjsJYwPThg8iAD9icQ2ARsgt</IpfsHash>
  </File>
  <File name="BD240402.jpg">
    <IpfsHash>QmYCMvUaUwvoPDBKAqHyxcFz72y9kALjECBC3JpnWLaBsQ</IpfsHash>
  </File>
</Files>
```
