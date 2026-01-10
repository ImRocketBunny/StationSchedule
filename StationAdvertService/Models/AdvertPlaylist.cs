using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationAdvertService.Models
{
    class AdvertPlaylist
    {
        public int ItemNo { get; set; }
        public string PlaylistId { get; set; }
        public List<Advert> Contents { get; set; }
    }
}
