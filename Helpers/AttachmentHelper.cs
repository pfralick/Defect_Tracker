using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.Helpers
{
    public class AttachmentHelper
    {
        public static string ShowIcon(string filePath)
        {
            string iconPath;
            switch (Path.GetExtension(filePath)) //.jpg
            {
                case ".pdf":
                    iconPath = "/Icons/pdf.png";
                    break;
                case ".txt":
                    iconPath = "/Icons/txtIcon.png";
                    break;
                default:
                    iconPath = "/Icons/unknownIcon.png";
                    break;
            }

            return iconPath;
        }

    }
}