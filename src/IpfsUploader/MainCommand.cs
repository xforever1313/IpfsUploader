//
// IpfsUploader - A way to upload files to an existing IPFS node.
// Copyright (C) 2024 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System.CommandLine;
using System.Text;

namespace IpfsUploader
{
    internal sealed class MainCommand
    {
        // ---------------- Fields ----------------

        private readonly TextWriter consoleOut;

        private readonly Command rootCommand;

        // ---------------- Constructor ----------------

        public MainCommand( TextWriter consoleOut )
        {
            this.consoleOut = consoleOut;

            this.rootCommand = new RootCommand( "Adds file(s) to an existing IPFS node." );

            var serverUrl = new Option<string>(
                "--server_url",
                "The URL to the server running IPFS."
            )
            {
                IsRequired = true
            };
            this.rootCommand.Add( serverUrl );

            var port = new Option<ushort>(
                "--port",
                () => 5001,
                "The port of the IPFS node's API server.  Remember when setting up a node, this port should NEVER be exposed to the internet."
            )
            {
                IsRequired = true
            };
            this.rootCommand.Add( port );

            var outputFile = new Option<FileInfo>(
                "--output_hash_file",
                "Where to output an file that contains the created hashes in XML format"
            )
            {
                IsRequired = false
            };
            this.rootCommand.Add( outputFile );

            var filePath = new Option<FileInfo>(
                "--file",
                "The file to upload to IPFS.  Globs are allowed.  Directories are not."
            )
            {
                IsRequired = true
            };
            this.rootCommand.Add( filePath );

            this.rootCommand.SetHandler(
                this.Handler,
                serverUrl,
                port,
                outputFile,
                filePath
             );
        }

        // ---------------- Functions ----------------

        public int Invoke( string[] args )
        {
            return this.rootCommand.Invoke( args );
        }

        private void Handler(
            string serverUrl,
            ushort port,
            FileInfo? outputFile,
            FileInfo file
        )
        {
            var config = new IpfsConfig(
                serverUrl,
                port,
                file,
                outputFile
            );

            using( var runner = new IpfsRunner( this.consoleOut ) )
            {
                IpfsResult result = runner.Run( config );

                if( result.Success == false )
                {
                    throw new AggregateException( result.Errors );
                }
            }
        }
    }
}
