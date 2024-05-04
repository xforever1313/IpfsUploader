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

using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace IpfsUploader
{
    public class IpfsUploadResult
    {
        // ---------------- Properties ----------------

        [JsonPropertyName( "Name" )]
        public string? FileName { get; set; }

        [JsonPropertyName( "Hash" )]
        public string? Hash { get; set; }

        [JsonPropertyName( "Size" )]
        public long? FileSize { get; set; }

        // ---------------- Functions ----------------

        internal void AppendToXml( XElement parent )
        {
            var fileElement = new XElement( "File" );

            if( this.FileName is not null )
            {
                var attr = new XAttribute( "name", this.FileName );
                fileElement.Add( attr );
            }

            if( this.Hash is not null )
            {
                var element = new XElement( "IpfsHash", this.Hash );
                fileElement.Add( element );
            }

            parent.Add( fileElement );
        }
    }
}
