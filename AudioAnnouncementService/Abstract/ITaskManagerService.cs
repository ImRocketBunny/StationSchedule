﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Abstract
{
    public interface ITaskManagerService
    {
        public Task Execute();
    }
}
