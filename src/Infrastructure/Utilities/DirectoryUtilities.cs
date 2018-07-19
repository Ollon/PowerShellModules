// -----------------------------------------------------------------------
// <copyright file="DirectoryContext.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace PowerShell.Infrastructure.Utilities
{
    public static class DirectoryUtilities
    {
        public static DirectoryEntry LocalMachine = new DirectoryEntry("WinNT://" + Environment.MachineName + ",Computer");
        public static DirectoryEntry LocalAdministratorsGroup = LocalMachine.Children.Find("administrators", "group");

        public static IEnumerable<DirectoryEntry> LocalAdministrators
        {
            get
            {
                object members = LocalAdministratorsGroup.Invoke("members", null);
                foreach (object groupMember in Cast<IEnumerable>(members))
                {
                    yield return new DirectoryEntry(groupMember);
                }
            }
        }

        public static string FriendlyDomainToLdapDomain(string friendlyDomainName)
        {
            string ldapPath = null;
            try
            {
                DirectoryContext objContext = new DirectoryContext(
                    DirectoryContextType.Domain,
                    friendlyDomainName);
                Domain objDomain = Domain.GetDomain(objContext);
                ldapPath = objDomain.Name;
            }
            catch (Exception e)
            {
                ldapPath = e.Message;
            }
            return ldapPath;
        }

        public static ArrayList EnumerateDomains()
        {
            ArrayList alDomains = new ArrayList();
            Forest currentForest = Forest.GetCurrentForest();
            foreach (Domain objDomain in currentForest.Domains)
            {
                alDomains.Add(objDomain.Name);
            }
            return alDomains;
        }

        public static ArrayList EnumerateGlobalCatalogs()
        {
            ArrayList alGCs = new ArrayList();
            Forest currentForest = Forest.GetCurrentForest();
            foreach (GlobalCatalog gc in currentForest.GlobalCatalogs)
            {
                alGCs.Add(gc.Name);
            }
            return alGCs;
        }

        public static ArrayList EnumerateDomainControllers()
        {
            ArrayList alDcs = new ArrayList();
            Domain domain = Domain.GetCurrentDomain();
            foreach (DomainController dc in domain.DomainControllers)
            {
                alDcs.Add(dc.Name);
            }
            return alDcs;
        }

        public static void CreateTrust(string sourceForestName, string targetForestName)
        {
            Forest sourceForest = Forest.GetForest(new DirectoryContext(
                DirectoryContextType.Forest,
                sourceForestName));
            Forest targetForest = Forest.GetForest(new DirectoryContext(
                DirectoryContextType.Forest,
                targetForestName));

            // create an inbound forest trust
            sourceForest.CreateTrustRelationship(targetForest,
                TrustDirection.Outbound);
        }

        public static void DeleteTrust(string sourceForestName, string targetForestName)
        {
            Forest sourceForest = Forest.GetForest(new DirectoryContext(
                DirectoryContextType.Forest,
                sourceForestName));
            Forest targetForest = Forest.GetForest(new DirectoryContext(
                DirectoryContextType.Forest,
                targetForestName));

            // delete forest trust
            sourceForest.DeleteTrustRelationship(targetForest);
        }

        public static ArrayList EnumerateOU(string OuDn)
        {
            ArrayList alObjects = new ArrayList();
            try
            {
                DirectoryEntry directoryObject = new DirectoryEntry("LDAP://" + OuDn);
                foreach (DirectoryEntry child in directoryObject.Children)
                {
                    string childPath = child.Path;
                    alObjects.Add(childPath.Remove(0, 7));

                    //remove the LDAP prefix from the path
                    child.Close();
                    child.Dispose();
                }
                directoryObject.Close();
                directoryObject.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("An Error Occurred: " + e.Message);
            }
            return alObjects;
        }

        public static bool Exists(string objectPath)
        {
            return DirectoryEntry.Exists("LDAP://" + objectPath);
        }

        public static void Move(string objectLocation, string newLocation)
        {
            //For brevity, removed existence checks
            DirectoryEntry eLocation = new DirectoryEntry("LDAP://" + objectLocation);
            DirectoryEntry nLocation = new DirectoryEntry("LDAP://" + newLocation);
            string newName = eLocation.Name;
            eLocation.MoveTo(nLocation, newName);
            nLocation.Close();
            eLocation.Close();
        }

        public static ArrayList AttributeValuesMultiString(string attributeName,
            string objectDn,
            ArrayList valuesCollection,
            bool recursive)
        {
            DirectoryEntry ent = new DirectoryEntry(objectDn);
            PropertyValueCollection ValueCollection = ent.Properties[attributeName];
            IEnumerator en = ValueCollection.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current != null)
                {
                    if (!valuesCollection.Contains(en.Current.ToString()))
                    {
                        valuesCollection.Add(en.Current.ToString());
                        if (recursive)
                        {
                            AttributeValuesMultiString(attributeName,
                                "LDAP://" +
                                en.Current,
                                valuesCollection,
                                true);
                        }
                    }
                }
            }
            ent.Close();
            ent.Dispose();
            return valuesCollection;
        }

        public static string AttributeValuesSingleString(string attributeName, string objectDn)
        {
            DirectoryEntry ent = new DirectoryEntry(objectDn);
            string strValue = ent.Properties[attributeName].Value.ToString();
            ent.Close();
            ent.Dispose();
            return strValue;
        }

        public static ArrayList GetUsedAttributes(string objectDn)
        {
            DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://" + objectDn);
            ArrayList props = new ArrayList();
            foreach (string strAttrName in objRootDSE.Properties.PropertyNames)
            {
                props.Add(strAttrName);
            }
            return props;
        }

        public static string GetComputerObjectDistinguishedName(string objectName, string LdapDomain)
        {
            string distinguishedName = string.Empty;
            string connectionPrefix = "LDAP://" + LdapDomain;
            DirectoryEntry entry = new DirectoryEntry(connectionPrefix);
            DirectorySearcher mySearcher = new DirectorySearcher(entry)
            {
                Filter = "(&(objectClass=computer)(|(cn = " + objectName + ")(dn = " + objectName + ")))"
            };
            SearchResult result = mySearcher.FindOne();
            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                 objectName +
                 " in the " +
                 LdapDomain +
                 " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();
            distinguishedName = "LDAP://" +
                                directoryObject.Properties
                                    ["distinguishedName"].Value;

            //if (returnValue.Equals(returnType.ObjectGUID))
            //{
            //    distinguishedName = directoryObject.ProjectGuid.ToString();
            //}
            entry.Close();
            entry.Dispose();
            mySearcher.Dispose();
            return distinguishedName;
        }

        public static string GetComputerObjectGuid(string objectName, string LdapDomain)
        {
            string distinguishedName = string.Empty;
            string connectionPrefix = "LDAP://" + LdapDomain;
            DirectoryEntry entry = new DirectoryEntry(connectionPrefix);
            DirectorySearcher mySearcher = new DirectorySearcher(entry)
            {
                Filter = "(&(objectClass=computer)(|(cn = " + objectName + ")(dn = " + objectName + ")))"
            };
            SearchResult result = mySearcher.FindOne();
            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                 objectName +
                 " in the " +
                 LdapDomain +
                 " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();

            //distinguishedName = "LDAP://" + directoryObject.Properties
            //    ["distinguishedName"].Value;
            distinguishedName = directoryObject.Guid.ToString();
            entry.Close();
            entry.Dispose();
            mySearcher.Dispose();
            return distinguishedName;
        }

        public static string GetUserObjectDistinguishedName(string objectName, string LdapDomain)
        {
            string distinguishedName = string.Empty;
            string connectionPrefix = "LDAP://" + LdapDomain;
            DirectoryEntry entry = new DirectoryEntry(connectionPrefix);
            DirectorySearcher mySearcher =
                new DirectorySearcher(entry) { Filter = "(&(objectClass=user)(|(cn = " + objectName + ")(dn = " + objectName + ")))" };
            SearchResult result = mySearcher.FindOne();
            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                 objectName +
                 " in the " +
                 LdapDomain +
                 " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();
            distinguishedName = "LDAP://" +
                                directoryObject.Properties
                                    ["distinguishedName"].Value;

            //if (returnValue.Equals(returnType.ObjectGUID))
            //{
            //    distinguishedName = directoryObject.ProjectGuid.ToString();
            //}
            entry.Close();
            entry.Dispose();
            mySearcher.Dispose();
            return distinguishedName;
        }

        public static string GetUserObjectGuid(string objectName, string LdapDomain)
        {
            string distinguishedName = string.Empty;
            string connectionPrefix = "LDAP://" + LdapDomain;
            DirectoryEntry entry = new DirectoryEntry(connectionPrefix);
            DirectorySearcher mySearcher =
                new DirectorySearcher(entry) { Filter = "(&(objectClass=user)(|(cn = " + objectName + ")(dn = " + objectName + ")))" };
            SearchResult result = mySearcher.FindOne();
            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                 objectName +
                 " in the " +
                 LdapDomain +
                 " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();

            //distinguishedName = "LDAP://" + directoryObject.Properties
            //    ["distinguishedName"].Value;
            distinguishedName = directoryObject.Guid.ToString();
            entry.Close();
            entry.Dispose();
            mySearcher.Dispose();
            return distinguishedName;
        }

        public static string GetGroupObjectDistinguishedName(string objectName, string LdapDomain)
        {
            string distinguishedName = string.Empty;
            string connectionPrefix = "LDAP://" + LdapDomain;
            DirectoryEntry entry = new DirectoryEntry(connectionPrefix);
            DirectorySearcher mySearcher = new DirectorySearcher(entry)
            {
                Filter = "(&(objectClass=group)(|(cn = " + objectName + ")(dn = " + objectName + ")))"
            };
            SearchResult result = mySearcher.FindOne();
            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                 objectName +
                 " in the " +
                 LdapDomain +
                 " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();
            distinguishedName = "LDAP://" +
                                directoryObject.Properties
                                    ["distinguishedName"].Value;

            //if (returnValue.Equals(returnType.ObjectGUID))
            //{
            //    distinguishedName = directoryObject.ProjectGuid.ToString();
            //}
            entry.Close();
            entry.Dispose();
            mySearcher.Dispose();
            return distinguishedName;
        }

        public static string GetGroupObjectGuid(string objectName, string LdapDomain)
        {
            string distinguishedName = string.Empty;
            string connectionPrefix = "LDAP://" + LdapDomain;
            DirectoryEntry entry = new DirectoryEntry(connectionPrefix);
            DirectorySearcher mySearcher = new DirectorySearcher(entry)
            {
                Filter = "(&(objectClass=group)(|(cn = " + objectName + ")(dn = " + objectName + ")))"
            };
            SearchResult result = mySearcher.FindOne();
            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                 objectName +
                 " in the " +
                 LdapDomain +
                 " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();

            //distinguishedName = "LDAP://" + directoryObject.Properties
            //    ["distinguishedName"].Value;
            distinguishedName = directoryObject.Guid.ToString();
            entry.Close();
            entry.Dispose();
            mySearcher.Dispose();
            return distinguishedName;
        }

        public static string ConvertDNtoGUID(string objectDN)
        {
            //Removed logic to check existence first
            DirectoryEntry directoryObject = new DirectoryEntry(objectDN);
            return directoryObject.Guid.ToString();
        }

        public static string ConvertGuidToOctectString(string objectGuid)
        {
            Guid guid = new Guid(objectGuid);
            byte[] byteGuid = guid.ToByteArray();
            string queryGuid = "";
            foreach (byte b in byteGuid)
            {
                queryGuid += @"\" + b.ToString("x2");
            }
            return queryGuid;
        }

        public static string ConvertGuidToDn(string GUID)
        {
            DirectoryEntry ent = new DirectoryEntry();
            string ADGuid = ent.NativeGuid;
            DirectoryEntry x = new DirectoryEntry("LDAP://{GUID=" + ADGuid + ">");

            //change the { to <>
            return x.Path.Remove(0, 7); //remove the LDAP prefix from the path
        }

        public static void CreateShareEntry(string ldapPath, string shareName, string shareUncPath, string shareDescription)
        {
            string oGUID = string.Empty;
            string connectionPrefix = "LDAP://" + ldapPath;
            DirectoryEntry directoryObject = new DirectoryEntry(connectionPrefix);
            DirectoryEntry networkShare = directoryObject.Children.Add("CN=" +
                                                                       shareName,
                "volume");
            networkShare.Properties["uNCName"].Value = shareUncPath;
            networkShare.Properties["Description"].Value = shareDescription;
            networkShare.CommitChanges();
            directoryObject.Close();
            networkShare.Close();
        }

        public static void CreateSecurityGroup(string ouPath, string name)
        {
            if (!DirectoryEntry.Exists("LDAP://CN=" + name + "," + ouPath))
            {
                try
                {
                    DirectoryEntry entry = new DirectoryEntry("LDAP://" + ouPath);
                    DirectoryEntry group = entry.Children.Add("CN=" + name, "group");
                    group.Properties["sAmAccountName"].Value = name;
                    group.CommitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else { Console.WriteLine(ouPath + " already exists"); }
        }

        public static bool Authenticate(string userName, string password, string domain)
        {
            bool authentic = false;
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + domain,
                    userName,
                    password);
                object nativeObject = entry.NativeObject;
                authentic = true;
            }
            catch (DirectoryServicesCOMException) { }
            return authentic;
        }

        public static void AddToGroup(string userDn, string groupDn)
        {
            try
            {
                DirectoryEntry dirEntry = new DirectoryEntry("LDAP://" + groupDn);
                dirEntry.Properties["member"].Add(userDn);
                dirEntry.CommitChanges();
                dirEntry.Close();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        public static void RemoveUserFromGroup(string userDn, string groupDn)
        {
            try
            {
                DirectoryEntry dirEntry = new DirectoryEntry("LDAP://" + groupDn);
                dirEntry.Properties["member"].Remove(userDn);
                dirEntry.CommitChanges();
                dirEntry.Close();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        public static ArrayList Groups(string userDn, bool recursive)
        {
            ArrayList groupMemberships = new ArrayList();
            return AttributeValuesMultiString("memberOf",
                userDn,
                groupMemberships,
                recursive);
        }

        public static string CreateUserAccount(string ldapPath, string userName, string userPassword)
        {
            try
            {
                string oGUID = string.Empty;
                string connectionPrefix = "LDAP://" + ldapPath;
                DirectoryEntry dirEntry = new DirectoryEntry(connectionPrefix);
                DirectoryEntry newUser = dirEntry.Children.Add("CN=" + userName, "user");
                newUser.Properties["samAccountName"].Value = userName;
                newUser.CommitChanges();
                oGUID = newUser.Guid.ToString();
                newUser.Invoke("SetPassword", userPassword);
                newUser.CommitChanges();
                dirEntry.Close();
                newUser.Close();
                return oGUID;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                return string.Empty;
            }
        }

        public static void EnableUserAccount(string userDn)
        {
            try
            {
                DirectoryEntry user = new DirectoryEntry(userDn);
                int val = (int)user.Properties["userAccountControl"].Value;
                user.Properties["userAccountControl"].Value = val & ~0x2;

                //ADS_UF_NORMAL_ACCOUNT;
                user.CommitChanges();
                user.Close();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        public static void DisableUserAccount(string userDn)
        {
            try
            {
                DirectoryEntry user = new DirectoryEntry(userDn);
                int val = (int)user.Properties["userAccountControl"].Value;
                user.Properties["userAccountControl"].Value = val | 0x2;

                //ADS_UF_ACCOUNTDISABLE;
                user.CommitChanges();
                user.Close();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        public static void UnlockUserAccount(string userDn)
        {
            try
            {
                DirectoryEntry uEntry = new DirectoryEntry(userDn);
                uEntry.Properties["LockOutTime"].Value = 0; //unlock account
                uEntry.CommitChanges(); //may not be needed but adding it anyways
                uEntry.Close();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        public static void Rename(string objectDn, string newName)
        {
            DirectoryEntry child = new DirectoryEntry("LDAP://" + objectDn);
            child.Rename("CN=" + newName);
        }

        public static void ResetPassword(string userDn, string password)
        {
            DirectoryEntry uEntry = new DirectoryEntry(userDn);
            uEntry.Invoke("SetPassword", password);
            uEntry.Properties["LockOutTime"].Value = 0; //unlock account
            uEntry.Close();
        }

        private static T Cast<T>(object o) => (T)o;
    }
}
