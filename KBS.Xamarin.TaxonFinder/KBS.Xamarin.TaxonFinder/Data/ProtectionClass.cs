using System.Collections.Generic;

namespace KBS.App.TaxonFinder.Data
{
    public class ProtectionClass
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public static List<ProtectionClass> FromDatabase()
        {
            var result = new List<ProtectionClass>();

            result.Add(new ProtectionClass() { ClassId = 11, ClassName = "Rote Liste Deutschland" });
            result.Add(new ProtectionClass() { ClassId = 2, ClassName = "Gesetzlicher Schutz (<a href=\"http://www.gesetze-im-internet.de/bartschv_2005/index.html\" target =\"_blank\">BArtSchV</a>,<a href=\"http://www.gesetze-im-internet.de/bnatschg_2009/index.html\" target=\"_blank\"> BNatSchG</a>)" });
            result.Add(new ProtectionClass() { ClassId = 5, ClassName = "<a href=\"https://www.bfn.de/themen/artenschutz/regelungen/ffh-richtlinie.html\" target=\"_blank\">FFH-Richtlinie</a>" });
            result.Add(new ProtectionClass() { ClassId = 9, ClassName = "Rote Liste Sachsen" });

            return result;
        }

    }
}
