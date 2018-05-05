// -----------------------------------------------------------------------
// <copyright file="ProductKeyFactory.cs" company="Ollon, LLC">
//     Copyright (c) 2018 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace PowerShell.Infrastructure.Utilities
{
 public static class ProductKeyFactory
    {
        public static string GetProductKey(ProductKey key)
        {
            switch (key)
            {
                case ProductKey.Windows10Professional:
                    return WindowsProductKeys.Windows10Professional;
                case ProductKey.Windows10ProfessionalN:
                    return WindowsProductKeys.Windows10ProfessionalN;
                case ProductKey.Windows10Enterprise:
                    return WindowsProductKeys.Windows10Enterprise;
                case ProductKey.Windows10EnterpriseN:
                    return WindowsProductKeys.Windows10EnterpriseN;
                case ProductKey.Windows10Education:
                    return WindowsProductKeys.Windows10Education;
                case ProductKey.Windows10EducationN:
                    return WindowsProductKeys.Windows10EducationN;
                case ProductKey.Windows10Enterprise2015LTSB:
                    return WindowsProductKeys.Windows10Enterprise2015LTSB;
                case ProductKey.Windows10Enterprise2015LTSBN:
                    return WindowsProductKeys.Windows10Enterprise2015LTSBN;
                case ProductKey.Windows81Professional:
                    return WindowsProductKeys.Windows81Professional;
                case ProductKey.Windows81ProfessionalN:
                    return WindowsProductKeys.Windows81ProfessionalN;
                case ProductKey.Windows81Enterprise:
                    return WindowsProductKeys.Windows81Enterprise;
                case ProductKey.Windows81EnterpriseN:
                    return WindowsProductKeys.Windows81EnterpriseN;
                case ProductKey.WindowsServer2016Datacenter:
                    return WindowsProductKeys.WindowsServer2016Datacenter;
                case ProductKey.WindowsServer2016Standard:
                    return WindowsProductKeys.WindowsServer2016Standard;
                case ProductKey.WindowsServer2016Essentials:
                    return WindowsProductKeys.WindowsServer2016Essentials;
                case ProductKey.WindowsServer2012R2ServerStandard:
                    return WindowsProductKeys.WindowsServer2012R2ServerStandard;
                case ProductKey.WindowsServer2012R2Datacenter:
                    return WindowsProductKeys.WindowsServer2012R2Datacenter;
                case ProductKey.WindowsServer2012R2Essentials:
                    return WindowsProductKeys.WindowsServer2012R2Essentials;
                case ProductKey.Windows8Professional:
                    return WindowsProductKeys.Windows8Professional;
                case ProductKey.Windows8ProfessionalN:
                    return WindowsProductKeys.Windows8ProfessionalN;
                case ProductKey.Windows8Enterprise:
                    return WindowsProductKeys.Windows8Enterprise;
                case ProductKey.Windows8EnterpriseN:
                    return WindowsProductKeys.Windows8EnterpriseN;
                case ProductKey.WindowsServer2012:
                    return WindowsProductKeys.WindowsServer2012;
                case ProductKey.WindowsServer2012N:
                    return WindowsProductKeys.WindowsServer2012N;
                case ProductKey.WindowsServer2012SingleLanguage:
                    return WindowsProductKeys.WindowsServer2012SingleLanguage;
                case ProductKey.WindowsServer2012CountrySpecific:
                    return WindowsProductKeys.WindowsServer2012CountrySpecific;
                case ProductKey.WindowsServer2012ServerStandard:
                    return WindowsProductKeys.WindowsServer2012ServerStandard;
                case ProductKey.WindowsServer2012MultiPointStandard:
                    return WindowsProductKeys.WindowsServer2012MultiPointStandard;
                case ProductKey.WindowsServer2012MultiPointPremium:
                    return WindowsProductKeys.WindowsServer2012MultiPointPremium;
                case ProductKey.WindowsServer2012Datacenter:
                    return WindowsProductKeys.WindowsServer2012Datacenter;
                case ProductKey.Windows7Professional:
                    return WindowsProductKeys.Windows7Professional;
                case ProductKey.Windows7ProfessionalN:
                    return WindowsProductKeys.Windows7ProfessionalN;
                case ProductKey.Windows7ProfessionalE:
                    return WindowsProductKeys.Windows7ProfessionalE;
                case ProductKey.Windows7Enterprise:
                    return WindowsProductKeys.Windows7Enterprise;
                case ProductKey.Windows7EnterpriseN:
                    return WindowsProductKeys.Windows7EnterpriseN;
                case ProductKey.Windows7EnterpriseE:
                    return WindowsProductKeys.Windows7EnterpriseE;
                case ProductKey.WindowsServer2008R2Web:
                    return WindowsProductKeys.WindowsServer2008R2Web;
                case ProductKey.WindowsServer2008R2HPCedition:
                    return WindowsProductKeys.WindowsServer2008R2HPCedition;
                case ProductKey.WindowsServer2008R2Standard:
                    return WindowsProductKeys.WindowsServer2008R2Standard;
                case ProductKey.WindowsServer2008R2Enterprise:
                    return WindowsProductKeys.WindowsServer2008R2Enterprise;
                case ProductKey.WindowsServer2008R2Datacenter:
                    return WindowsProductKeys.WindowsServer2008R2Datacenter;
                case ProductKey.WindowsServer2008R2forItaniumbasedSystems:
                    return WindowsProductKeys.WindowsServer2008R2forItaniumbasedSystems;
                case ProductKey.WindowsVistaBusiness:
                    return WindowsProductKeys.WindowsVistaBusiness;
                case ProductKey.WindowsVistaBusinessN:
                    return WindowsProductKeys.WindowsVistaBusinessN;
                case ProductKey.WindowsVistaEnterprise:
                    return WindowsProductKeys.WindowsVistaEnterprise;
                case ProductKey.WindowsVistaEnterpriseN:
                    return WindowsProductKeys.WindowsVistaEnterpriseN;
                case ProductKey.WindowsWebServer2008:
                    return WindowsProductKeys.WindowsWebServer2008;
                case ProductKey.WindowsServer2008Standard:
                    return WindowsProductKeys.WindowsServer2008Standard;
                case ProductKey.WindowsServer2008StandardwithoutHyperV:
                    return WindowsProductKeys.WindowsServer2008StandardwithoutHyperV;
                case ProductKey.WindowsServer2008Enterprise:
                    return WindowsProductKeys.WindowsServer2008Enterprise;
                case ProductKey.WindowsServer2008EnterprisewithoutHyperV:
                    return WindowsProductKeys.WindowsServer2008EnterprisewithoutHyperV;
                case ProductKey.WindowsServer2008HPC:
                    return WindowsProductKeys.WindowsServer2008HPC;
                case ProductKey.WindowsServer2008Datacenter:
                    return WindowsProductKeys.WindowsServer2008Datacenter;
                case ProductKey.WindowsServer2008DatacenterwithoutHyperV:
                    return WindowsProductKeys.WindowsServer2008DatacenterwithoutHyperV;
                case ProductKey.WindowsServer2008forItaniumBasedSystems:
                    return WindowsProductKeys.WindowsServer2008forItaniumBasedSystems;
                case ProductKey.Empty:
                    return WindowsProductKeys.Empty;
                case ProductKey.SystemCenter2012R2VolumeLicense:
                    return WindowsProductKeys.SystemCenter2012R2VolumeLicense;
                case ProductKey.SQLServer2008EnterpriseCAL:
                    return WindowsProductKeys.SQLServer2008EnterpriseCAL;
                case ProductKey.SQLServer2012EnterpriseCAL:
                    return WindowsProductKeys.SQLServer2012EnterpriseCAL;
                case ProductKey.SQLServer2014EnterpriseCAL:
                    return WindowsProductKeys.SQLServer2014EnterpriseCAL;
                case ProductKey.VisualStudio2010Ultimate:
                    return WindowsProductKeys.VisualStudio2010Ultimate;
                case ProductKey.VisualStudio2012Ultimate:
                    return WindowsProductKeys.VisualStudio2012Ultimate;
                case ProductKey.VisualStudio2013Ultimate:
                    return WindowsProductKeys.VisualStudio2013Ultimate;
                case ProductKey.VisualStudio2015Enterprise:
                    return WindowsProductKeys.VisualStudio2015Enterprise;
                case ProductKey.TeamFoundationServer2012:
                    return WindowsProductKeys.TeamFoundationServer2012;
                case ProductKey.TeamFoundationServer2013:
                    return WindowsProductKeys.TeamFoundationServer2013;
                case ProductKey.TeamFoundationServer2015:
                    return WindowsProductKeys.TeamFoundationServer2015;
                case ProductKey.SQLServer2016EnterpriseCAL:
                    return WindowsProductKeys.SQLServer2016EnterpriseCAL;
                case ProductKey.VisualStudio2017Enterprise:
                    return WindowsProductKeys.VisualStudio2017Enterprise;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }

        public static Dictionary<ProductKey, string> All()
        {
            return new Dictionary<ProductKey, string>
            {
                {ProductKey.SystemCenter2012R2VolumeLicense, "BXH69-M62YX-QQD6R-3GPWX-8WMFY"},
                {ProductKey.SQLServer2008EnterpriseCAL, "To be determined"},
                {ProductKey.SQLServer2012EnterpriseCAL, "748RB-X4T6B-MRM7V-RTVFF-CHC8H"},
                {ProductKey.SQLServer2014EnterpriseCAL, "TJYBJ-8YGH6-QK2JJ-M9DFB-D7M9D"},
                {ProductKey.SQLServer2016EnterpriseCAL, WindowsProductKeys.SQLServer2016EnterpriseCAL},
                {ProductKey.VisualStudio2010Ultimate, ""},
                {ProductKey.VisualStudio2012Ultimate, "RBCXF-CVBGR-382MK-DFHJ4-C69G8"},
                {ProductKey.VisualStudio2013Ultimate, "BWG7X-J98B3-W34RT-33B3R-JVYW9"},
                {ProductKey.VisualStudio2015Enterprise, "HM6NR-QXX7C-DFW2Y-8B82K-WTYJV"},
                {ProductKey.TeamFoundationServer2012, ""},
                {ProductKey.TeamFoundationServer2013, "MHG9J-HHHX9-WWPQP-D8T7H-7KCQG"},
                {ProductKey.TeamFoundationServer2015, "PTBNK-HVGCM-HB2GW-MXWMH-T3BJQ"},
                {ProductKey.Windows10Professional, "W269N-WFGWX-YVC9B-4J6C9-T83GX"},
                {ProductKey.Windows10ProfessionalN, "MH37W-N47XK-V7XM9-C7227-GCQG9"},
                {ProductKey.Windows10Enterprise, "NPPR9-FWDCX-D2C8J-H872K-2YT43"},
                {ProductKey.Windows10EnterpriseN, "DPH2V-TTNVB-4X9Q3-TJR4H-KHJW4"},
                {ProductKey.Windows10Education, "NW6C2-QMPVW-D7KKK-3GKT6-VCFB2"},
                {ProductKey.Windows10EducationN, "2WH4N-8QGBV-H22JP-CT43Q-MDWWJ"},
                {ProductKey.Windows10Enterprise2015LTSB, "WNMTR-4C88C-JK8YV-HQ7T2-76DF9"},
                {ProductKey.Windows10Enterprise2015LTSBN, "2F77B-TNFGY-69QQF-B8YKP-D69TJ"},
                {ProductKey.Windows81Professional, "GCRJD-8NW9H-F2CDX-CCM8D-9D6T9"},
                {ProductKey.Windows81ProfessionalN, "HMCNV-VVBFX-7HMBH-CTY9B-B4FXY"},
                {ProductKey.Windows81Enterprise, "MHF9N-XY6XB-WVXMC-BTDCT-MKKG7"},
                {ProductKey.Windows81EnterpriseN, "TT4HM-HN7YT-62K67-RGRQJ-JFFXW"},
                {ProductKey.WindowsServer2012R2ServerStandard, "D2N9P-3P6X9-2R39C-7RTCD-MDVJX"},
                {ProductKey.WindowsServer2012R2Datacenter, "W3GGN-FT8W3-Y4M27-J84CP-Q3VJ9"},
                {ProductKey.WindowsServer2012R2Essentials, "KNC87-3J2TX-XB4WP-VCPJV-M4FWM"},
                {ProductKey.Windows8Professional, "NG4HW-VH26C-733KW-K6F98-J8CK4"},
                {ProductKey.Windows8ProfessionalN, "XCVCF-2NXM9-723PB-MHCB7-2RYQQ"},
                {ProductKey.Windows8Enterprise, "32JNW-9KQ84-P47T8-D8GGY-CWCK7"},
                {ProductKey.Windows8EnterpriseN, "JMNMF-RHW7P-DMY6X-RF3DR-X2BQT"},
                {ProductKey.WindowsServer2016Datacenter, WindowsProductKeys.WindowsServer2016Datacenter},
                {ProductKey.WindowsServer2016Standard, WindowsProductKeys.WindowsServer2016Standard},
                {ProductKey.WindowsServer2016Essentials, WindowsProductKeys.WindowsServer2016Essentials},
                {ProductKey.WindowsServer2012, "BN3D2-R7TKB-3YPBD-8DRP2-27GG4"},
                {ProductKey.WindowsServer2012N, "8N2M2-HWPGY-7PGT9-HGDD8-GVGGY"},
                {ProductKey.WindowsServer2012SingleLanguage, "2WN2H-YGCQR-KFX6K-CD6TF-84YXQ"},
                {ProductKey.WindowsServer2012CountrySpecific, "4K36P-JN4VD-GDC6V-KDT89-DYFKP"},
                {ProductKey.WindowsServer2012ServerStandard, "XC9B7-NBPP2-83J2H-RHMBY-92BT4"},
                {ProductKey.WindowsServer2012MultiPointStandard, "HM7DN-YVMH3-46JC3-XYTG7-CYQJJ"},
                {ProductKey.WindowsServer2012MultiPointPremium, "XNH6W-2V9GX-RGJ4K-Y8X6F-QGJ2G"},
                {ProductKey.WindowsServer2012Datacenter, "48HP8-DN98B-MYWDG-T2DCC-8W83P"},
                {ProductKey.Windows7Professional, "FJ82H-XT6CR-J8D7P-XQJJ2-GPDD4"},
                {ProductKey.Windows7ProfessionalN, "MRPKT-YTG23-K7D7T-X2JMM-QY7MG"},
                {ProductKey.Windows7ProfessionalE, "W82YF-2Q76Y-63HXB-FGJG9-GF7QX"},
                {ProductKey.Windows7Enterprise, "33PXH-7Y6KF-2VJC9-XBBR8-HVTHH"},
                {ProductKey.Windows7EnterpriseN, "YDRBP-3D83W-TY26F-D46B2-XCKRJ"},
                {ProductKey.Windows7EnterpriseE, "C29WB-22CC8-VJ326-GHFJW-H9DH4"},
                {ProductKey.WindowsServer2008R2Web, "6TPJF-RBVHG-WBW2R-86QPH-6RTM4"},
                {ProductKey.WindowsServer2008R2HPCedition, "TT8MH-CG224-D3D7Q-498W2-9QCTX"},
                {ProductKey.WindowsServer2008R2Standard, "YC6KT-GKW9T-YTKYR-T4X34-R7VHC"},
                {ProductKey.WindowsServer2008R2Enterprise, "489J6-VHDMP-X63PK-3K798-CPX3Y"},
                {ProductKey.WindowsServer2008R2Datacenter, "74YFP-3QFB3-KQT8W-PMXWJ-7M648"},
                {ProductKey.WindowsServer2008R2forItaniumbasedSystems, "GT63C-RJFQ3-4GMB6-BRFB9-CB83V"},
                {ProductKey.WindowsVistaBusiness, "YFKBB-PQJJV-G996G-VWGXY-2V3X8"},
                {ProductKey.WindowsVistaBusinessN, "HMBQG-8H2RH-C77VX-27R82-VMQBT"},
                {ProductKey.WindowsVistaEnterprise, "VKK3X-68KWM-X2YGT-QR4M6-4BWMV"},
                {ProductKey.WindowsVistaEnterpriseN, "VTC42-BM838-43QHV-84HX6-XJXKV"},
                {ProductKey.WindowsWebServer2008, "WYR28-R7TFJ-3X2YQ-YCY4H-M249D"},
                {ProductKey.WindowsServer2008Standard, "TM24T-X9RMF-VWXK6-X8JC9-BFGM2"},
                {ProductKey.WindowsServer2008StandardwithoutHyperV, "W7VD6-7JFBR-RX26B-YKQ3Y-6FFFJ"},
                {ProductKey.WindowsServer2008Enterprise, "YQGMW-MPWTJ-34KDK-48M3W-X4Q6V"},
                {ProductKey.WindowsServer2008EnterprisewithoutHyperV, "39BXF-X8Q23-P2WWT-38T2F-G3FPG"},
                {ProductKey.WindowsServer2008HPC, "RCTX3-KWVHP-BR6TB-RB6DM-6X7HP"},
                {ProductKey.WindowsServer2008Datacenter, "7M67G-PC374-GR742-YH8V4-TCBY3"},
                {ProductKey.WindowsServer2008DatacenterwithoutHyperV, "22XQ2-VRXRG-P8D42-K34TD-G3QQC"},
                {ProductKey.WindowsServer2008forItaniumBasedSystems, "4DWFP-JF3DJ-B7DTH-78FJB-PDRHK"}
            };
        }
    }

    internal static class WindowsProductKeys
    {
        public const string Empty = "00000-00000-00000-00000-00000";
        public const string SystemCenter2012R2VolumeLicense = "BXH69-M62YX-QQD6R-3GPWX-8WMFY";
        public const string SQLServer2008EnterpriseCAL = "To be determined";
        public const string SQLServer2012EnterpriseCAL = "748RB-X4T6B-MRM7V-RTVFF-CHC8H";
        public const string SQLServer2014EnterpriseCAL = "TJYBJ-8YGH6-QK2JJ-M9DFB-D7M9D";
        public const string SQLServer2016EnterpriseCAL = "MDCJV-3YX8N-WG89M-KV443-G8249";
        public const string VisualStudio2008Ultimate = "";
        public const string VisualStudio2010Ultimate = "";
        public const string VisualStudio2012Ultimate = "RBCXF-CVBGR-382MK-DFHJ4-C69G8";
        public const string VisualStudio2013Ultimate = "BWG7X-J98B3-W34RT-33B3R-JVYW9";
        public const string VisualStudio2015Enterprise = "HM6NR-QXX7C-DFW2Y-8B82K-WTYJV";
        public const string VisualStudio2017Enterprise = "NJVYC-BMHX2-G77MM-4XJMR-6Q8QF";
        public const string TeamFoundationServer2012 = "";
        public const string TeamFoundationServer2013 = "MHG9J-HHHX9-WWPQP-D8T7H-7KCQG";
        public const string TeamFoundationServer2015 = "PTBNK-HVGCM-HB2GW-MXWMH-T3BJQ";
        public const string Windows10Professional = "W269N-WFGWX-YVC9B-4J6C9-T83GX";
        public const string Windows10ProfessionalN = "MH37W-N47XK-V7XM9-C7227-GCQG9";
        public const string Windows10Enterprise = "NPPR9-FWDCX-D2C8J-H872K-2YT43";
        public const string Windows10EnterpriseN = "DPH2V-TTNVB-4X9Q3-TJR4H-KHJW4";
        public const string Windows10Education = "NW6C2-QMPVW-D7KKK-3GKT6-VCFB2";
        public const string Windows10EducationN = "2WH4N-8QGBV-H22JP-CT43Q-MDWWJ";
        public const string Windows10Enterprise2015LTSB = "WNMTR-4C88C-JK8YV-HQ7T2-76DF9";
        public const string Windows10Enterprise2015LTSBN = "2F77B-TNFGY-69QQF-B8YKP-D69TJ";
        public const string Windows81Professional = "GCRJD-8NW9H-F2CDX-CCM8D-9D6T9";
        public const string Windows81ProfessionalN = "HMCNV-VVBFX-7HMBH-CTY9B-B4FXY";
        public const string Windows81Enterprise = "MHF9N-XY6XB-WVXMC-BTDCT-MKKG7";
        public const string Windows81EnterpriseN = "TT4HM-HN7YT-62K67-RGRQJ-JFFXW";
        public const string WindowsServer2016Datacenter = "CB7KF-BWN84-R7R2Y-793K2-8XDDG";
        public const string WindowsServer2016Standard = "WC2BQ-8NRM3-FDDYY-2BFGV-KHKQY";
        public const string WindowsServer2016Essentials = "JCKRF-N37P4-C2D82-9YXRT-4M63B";
        public const string WindowsServer2012R2ServerStandard = "D2N9P-3P6X9-2R39C-7RTCD-MDVJX";
        public const string WindowsServer2012R2Datacenter = "W3GGN-FT8W3-Y4M27-J84CP-Q3VJ9";
        public const string WindowsServer2012R2Essentials = "KNC87-3J2TX-XB4WP-VCPJV-M4FWM";
        public const string Windows8Professional = "NG4HW-VH26C-733KW-K6F98-J8CK4";
        public const string Windows8ProfessionalN = "XCVCF-2NXM9-723PB-MHCB7-2RYQQ";
        public const string Windows8Enterprise = "32JNW-9KQ84-P47T8-D8GGY-CWCK7";
        public const string Windows8EnterpriseN = "JMNMF-RHW7P-DMY6X-RF3DR-X2BQT";
        public const string WindowsServer2012 = "BN3D2-R7TKB-3YPBD-8DRP2-27GG4";
        public const string WindowsServer2012N = "8N2M2-HWPGY-7PGT9-HGDD8-GVGGY";
        public const string WindowsServer2012SingleLanguage = "2WN2H-YGCQR-KFX6K-CD6TF-84YXQ";
        public const string WindowsServer2012CountrySpecific = "4K36P-JN4VD-GDC6V-KDT89-DYFKP";
        public const string WindowsServer2012ServerStandard = "XC9B7-NBPP2-83J2H-RHMBY-92BT4";
        public const string WindowsServer2012MultiPointStandard = "HM7DN-YVMH3-46JC3-XYTG7-CYQJJ";
        public const string WindowsServer2012MultiPointPremium = "XNH6W-2V9GX-RGJ4K-Y8X6F-QGJ2G";
        public const string WindowsServer2012Datacenter = "48HP8-DN98B-MYWDG-T2DCC-8W83P";
        public const string Windows7Professional = "FJ82H-XT6CR-J8D7P-XQJJ2-GPDD4";
        public const string Windows7ProfessionalN = "MRPKT-YTG23-K7D7T-X2JMM-QY7MG";
        public const string Windows7ProfessionalE = "W82YF-2Q76Y-63HXB-FGJG9-GF7QX";
        public const string Windows7Enterprise = "33PXH-7Y6KF-2VJC9-XBBR8-HVTHH";
        public const string Windows7EnterpriseN = "YDRBP-3D83W-TY26F-D46B2-XCKRJ";
        public const string Windows7EnterpriseE = "C29WB-22CC8-VJ326-GHFJW-H9DH4";
        public const string WindowsServer2008R2Web = "6TPJF-RBVHG-WBW2R-86QPH-6RTM4";
        public const string WindowsServer2008R2HPCedition = "TT8MH-CG224-D3D7Q-498W2-9QCTX";
        public const string WindowsServer2008R2Standard = "YC6KT-GKW9T-YTKYR-T4X34-R7VHC";
        public const string WindowsServer2008R2Enterprise = "489J6-VHDMP-X63PK-3K798-CPX3Y";
        public const string WindowsServer2008R2Datacenter = "74YFP-3QFB3-KQT8W-PMXWJ-7M648";
        public const string WindowsServer2008R2forItaniumbasedSystems = "GT63C-RJFQ3-4GMB6-BRFB9-CB83V";
        public const string WindowsVistaBusiness = "YFKBB-PQJJV-G996G-VWGXY-2V3X8";
        public const string WindowsVistaBusinessN = "HMBQG-8H2RH-C77VX-27R82-VMQBT";
        public const string WindowsVistaEnterprise = "VKK3X-68KWM-X2YGT-QR4M6-4BWMV";
        public const string WindowsVistaEnterpriseN = "VTC42-BM838-43QHV-84HX6-XJXKV";
        public const string WindowsWebServer2008 = "WYR28-R7TFJ-3X2YQ-YCY4H-M249D";
        public const string WindowsServer2008Standard = "TM24T-X9RMF-VWXK6-X8JC9-BFGM2";
        public const string WindowsServer2008StandardwithoutHyperV = "W7VD6-7JFBR-RX26B-YKQ3Y-6FFFJ";
        public const string WindowsServer2008Enterprise = "YQGMW-MPWTJ-34KDK-48M3W-X4Q6V";
        public const string WindowsServer2008EnterprisewithoutHyperV = "39BXF-X8Q23-P2WWT-38T2F-G3FPG";
        public const string WindowsServer2008HPC = "RCTX3-KWVHP-BR6TB-RB6DM-6X7HP";
        public const string WindowsServer2008Datacenter = "7M67G-PC374-GR742-YH8V4-TCBY3";
        public const string WindowsServer2008DatacenterwithoutHyperV = "22XQ2-VRXRG-P8D42-K34TD-G3QQC";
        public const string WindowsServer2008forItaniumBasedSystems = "4DWFP-JF3DJ-B7DTH-78FJB-PDRHK";
    }


}
