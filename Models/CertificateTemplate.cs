// Copyright © Uwe Gradenegger <uwe@gradenegger.eu>

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;
using TameMyCerts.WSTEP.Enums;

namespace TameMyCerts.WSTEP.Models
{
    /// <summary>
    ///     Information about a certificate template.
    /// </summary>
    public class CertificateTemplate
    {
        private CertificateTemplate(string templateName, RegistryKey regKey)
        {
            const string enrollPermission = "0E10C968-78FB-11D2-90D4-00C04F79DC55";

            Name = templateName;
            DisplayName = (string)regKey.GetValue("DisplayName");
            ObjectIdentifier = ((string[])regKey.GetValue("msPKI-Cert-Template-OID"))[0];

            var flags = Convert.ToInt32(regKey.GetValue("Flags"));
            UserScope = !((GeneralFlag)flags).HasFlag(GeneralFlag.CT_FLAG_MACHINE_TYPE);

            EnrolleeSuppliesSubject =
                (CertCa.CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT &
                 Convert.ToInt32(regKey.GetValue("msPKI-Certificate-Name-Flag"))) ==
                CertCa.CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT;

            var rawSecurityDescriptor = new RawSecurityDescriptor((byte[])regKey.GetValue("Security"), 0);

            foreach (var genericAce in rawSecurityDescriptor.DiscretionaryAcl)
            {
                if (!(genericAce is ObjectAce objectAce))
                {
                    continue;
                }

                if (objectAce.ObjectAceType != new Guid(enrollPermission))
                {
                    continue;
                }

                switch (objectAce.AceType)
                {
                    case AceType.AccessAllowedObject:
                        AllowedPrincipals.Add(objectAce.SecurityIdentifier);
                        break;
                    case AceType.AccessDeniedObject:
                        DisallowedPrincipals.Add(objectAce.SecurityIdentifier);
                        break;
                }
            }
        }

        /// <summary>
        ///     The common name of the certificate template. Use this when submitting certificate requests.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The display name of the certificate template.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        ///     The object identifier of the certificate template.
        /// </summary>
        public string ObjectIdentifier { get; }

        /// <summary>
        ///     Specifies if the enrollee may provide subject information with the certificate request.
        /// </summary>
        public bool EnrolleeSuppliesSubject { get; }

        public bool UserScope { get; }

        private List<SecurityIdentifier> AllowedPrincipals { get; } = new List<SecurityIdentifier>();
        private List<SecurityIdentifier> DisallowedPrincipals { get; } = new List<SecurityIdentifier>();

        /// <summary>
        ///     Builds a new CertificateTemplate object.
        /// </summary>
        /// <param name="templateOid">The nobject identifier ame of the certificate template from which the object is built.</param>
        public static CertificateTemplate Create(string templateOid)
        {
            var machineBaseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var templateBaseKey =
                machineBaseKey.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography\\CertificateTemplateCache");

            if (templateBaseKey == null) return null;

            foreach (var templateName in templateBaseKey.GetSubKeyNames())
            {
                if (!(templateBaseKey?.OpenSubKey(templateName) is RegistryKey templateSubKey))
                {
                    continue;
                }

                if (((string[])templateSubKey.GetValue("msPKI-Cert-Template-OID"))[0].Equals(templateOid))
                {
                    return new CertificateTemplate(templateName, templateSubKey);
                }
            }

            return null;
        }

        /// <summary>
        ///     Determines whether a given WindowsIdentity may enroll for this certificate template.
        /// </summary>
        /// <param name="identity">The Windows identity to check for permissions.</param>
        /// <param name="explicitlyPermitted">Return true only if the identity is explicitly mentioned in the acl.</param>
        /// <returns></returns>
        public bool AllowsForEnrollment(WindowsIdentity identity, bool explicitlyPermitted = false)
        {
            var isAllowed = false;
            var isDenied = false;

            if (!explicitlyPermitted)
            {
                for (var index = 0; index < identity.Groups?.Count; index++)
                {
                    var group = (SecurityIdentifier)identity.Groups[index];
                    isAllowed = AllowedPrincipals.Contains(group) || isAllowed;
                    isDenied = DisallowedPrincipals.Contains(group) || isDenied;
                }
            }

            isAllowed = AllowedPrincipals.Contains(identity.User) || isAllowed;
            isDenied = DisallowedPrincipals.Contains(identity.User) || isDenied;

            return isAllowed && !isDenied;
        }
    }
}