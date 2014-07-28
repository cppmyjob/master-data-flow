﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{
    public interface ICommandResult
    {
        NextCommandResult FindNextCommand(ICommandWorkflow workflow);
    }
}
