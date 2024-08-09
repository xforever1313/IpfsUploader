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

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Xml.Linq;
using Microsoft.Extensions.FileSystemGlobbing;

namespace IpfsUploader
{
    public sealed class IpfsRunner : IDisposable
    {
        // ---------------- Fields ----------------

        private const string userAgent = "Ipfs_Uploader";

        private const string addEndpoint = "/api/v0/add";

        private readonly HttpClient httpClient;

        private readonly TextWriter log;

        // ---------------- Constructor ----------------

        public IpfsRunner( TextWriter log )
        {
            Version? version = GetType().Assembly.GetName().Version;
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue( userAgent, version?.ToString( 3 ) )
            );

            this.log = log;
        }

        // ---------------- Functions ----------------

        public IpfsResult Run( IpfsConfig config )
        {
            Uri url = config.GetUri( addEndpoint );

            DirectoryInfo? directory = config.InputFile.Directory;
            if( directory is null )
            {
                throw new IpfsException(
                    "Input file does not appear to live in a directory."
                );
            }

            var globber = new Matcher();
            globber.AddInclude( config.InputFile.Name );

            IEnumerable<string> matchingFiles = globber.GetResultsInFullPath(
                directory.FullName
            );

            if( matchingFiles.Any() == false )
            {
                throw new IpfsException(
                    $"No files found at {config.InputFile.FullName}."
                );
            }

            var errors = new List<Exception>();

            XDocument? doc = null;
            XElement? root = null;
            if( config.Outputfile is not null )
            {
                var dec = new XDeclaration( "1.0", "utf-8", null );
                doc = new XDocument( dec );
                root = new XElement( "Files" );
                doc.Add( root );
            }

            foreach( string file in matchingFiles.OrderBy( s => s ) )
            {
                try
                {
                    this.log.WriteLine( $"Uploading '{file}'" );
                    IpfsUploadResult result = TryUpload( file, url );
                    this.log.WriteLine( $"\t- {result.FileName} - {result.Hash}" );
                    if( root is not null )
                    {
                        result.AppendToXml( root );
                    }
                }
                catch( Exception e )
                {
                    this.log.WriteLine( "\t- Error: " + e.Message );
                    errors.Add( e );
                }
            }

            if( config.Outputfile is not null )
            {
                doc?.Save( config.Outputfile.FullName, SaveOptions.None );
            }

            return new IpfsResult( errors );
        }

        public void Dispose()
        {
            this.httpClient?.Dispose();
        }

        private IpfsUploadResult TryUpload( string filePath, Uri url )
        {
            using FileStream fs = File.OpenRead( filePath );
            using var fileVaue = new StreamContent( fs );
            var dataContent = new MultipartFormDataContent
            {
                { fileVaue, "path", Path.GetFileName( filePath ) }
            };

            HttpResponseMessage response = this.httpClient.PostAsync(
                url,
                dataContent
            ).Result;

            if( response.IsSuccessStatusCode == false )
            {
                string body = response.Content?.ReadAsStringAsync().Result ?? "";
                throw new HttpRequestException(
                    $"HTTP Error {response.StatusCode} when uploading: {body}"
                );
            }
            else if( response.Content is null )
            {
                throw new HttpRequestException(
                    "HTTP Content was null!"
                );
            }

            IpfsUploadResult? jsonResponse = response.Content.ReadFromJsonAsync<IpfsUploadResult>().Result;
            if( jsonResponse is null )
            {
                throw new HttpRequestException(
                    "JSON Response was null!"
                );
            }

            return jsonResponse;
        }
    }
}
