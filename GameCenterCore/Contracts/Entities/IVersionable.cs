﻿using GameCenterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts
{
    public interface IVersionable
    {
        VersionInfo Version { get; }
    }
}
