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

namespace IpfsUploader
{
    public record class IpfsConfig(
        string ServerUrl,
        ushort Port,
        FileInfo InputFile,
        FileInfo? Outputfile,
        uint TimeoutMultiplier
    )
    {
        // ---------------- Functions ----------------

        public Uri GetUri( string endPoint )
        {
            string url = $"{ServerUrl}:{Port}{endPoint}";
            if( Uri.IsWellFormedUriString( url, UriKind.Absolute ) == false )
            {
                throw new IpfsException(
                    "Given server URL is not a valid URI: " + url
                );
            }

            return new Uri( url, UriKind.Absolute );
        }
    }
}
