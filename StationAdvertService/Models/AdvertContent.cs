using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationAdvertService.Models
{
    class AdvertContent
    {
        public string FormatName { get; set; }
        public string Formatversion { get; set; }
        public string ContentVersion { get; set; }
        public string DefaultPlaylistId { get; set; }
        public List<AdvertPlaylist> Playlists { get; set; }

    }
}
