// -----------------------------------------------------------------------
// <copyright file="AbstractAddOnToolHostObjectControl.cs" company="Ollon, LLC">
//     Copyright (c) 2018 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Microsoft.PowerShell.Host.ISE;

namespace PowerShell.EditorFeatures.Core.Host
{
    public abstract class AbstractAddOnToolHostObjectControl : UserControl, IAddOnToolHostObject
    {
        public abstract ObjectModelRoot HostObject { get; set; }
    }
}
