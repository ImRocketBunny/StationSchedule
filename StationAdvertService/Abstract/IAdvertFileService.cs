using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationAdvertService.Abstract
{
    public interface IAdvertFileService
    {
        Task ProcessFileManaging();
        List<string> GetCurrentPlaylist();
    }
}
