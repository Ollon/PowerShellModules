// -----------------------------------------------------------------------
// <copyright file="DirectoryContexts.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.DirectoryServices.ActiveDirectory;

namespace PowerShell.Infrastructure.Utilities
{
    public static class DirectoryContexts
    {
        private const string AccountName = "OLLON\\Administrator";
        private const string Password = "Magicc12;";
        public static DirectoryContext Domain = new DirectoryContext(DirectoryContextType.Domain, AccountName, Password);
        public static DirectoryContext Forest = new DirectoryContext(DirectoryContextType.Forest, AccountName, Password);
        public static DirectoryContext DirectoryServer = new DirectoryContext(DirectoryContextType.DirectoryServer, AccountName, Password);
        public static DirectoryContext ConfigurationSet =
            new DirectoryContext(DirectoryContextType.ConfigurationSet, AccountName, Password);
        public static DirectoryContext ApplicationPartition =
            new DirectoryContext(DirectoryContextType.ApplicationPartition, AccountName, Password);
    }
}
