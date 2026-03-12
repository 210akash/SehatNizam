//-----------------------------------------------------------------------
// <copyright file="FileUtilities.cs" company="CRM">
//     CRM copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Utilities
{
    using System.Collections.Generic;

    /// <summary>
    /// File Utilities
    /// </summary>
    public class FileUtilities
    {
        /// <summary>The dictionary</summary>
        private readonly Dictionary<string, string> dict;

        /// <summary>Initializes a new instance of the <see cref="FileUtilities" /> class.</summary>
        public FileUtilities()
        {
            dict = new Dictionary<string, string>();
            dict.Add(".3gp", "video/3gpp");
            dict.Add(".3g2", "video/3gpp2");
            dict.Add(".adp", "audio/adpcm");
            dict.Add(".avi", "video/avi");
            dict.Add(".bmp", "image/bmp");
            dict.Add(".uva", "audio/vnd.dece.audio");
            dict.Add(".uvi", "image/vnd.dece.graphic");
            dict.Add(".uvh", "video/vnd.dece.hd");
            dict.Add(".uvm", "video/vnd.dece.mobile");
            dict.Add(".uvu", "video/vnd.uvvu.mp4");
            dict.Add(".uvp", "video/vnd.dece.pd");
            dict.Add(".uvs", "video/vnd.dece.sd");
            dict.Add(".uvv", "video/vnd.dece.video");
            dict.Add(".fst", "image/vnd.fst");
            dict.Add(".fvt", "video/vnd.fvt");
            dict.Add(".fbs", "image/vnd.fastbidsheet");
            dict.Add(".f4v", "video/x-f4v");
            dict.Add(".flv", "video/x-flv");
            dict.Add(".fpx", "image/vnd.fpx");
            dict.Add(".npx", "image/vnd.net-fpx");
            dict.Add(".fli", "video/x-fli");
            dict.Add(".fh", "image/x-freehand");
            dict.Add(".g3", "image/g3fax");
            dict.Add(".gif", "image/gif");
            dict.Add(".h261", "video/h261");
            dict.Add(".h263", "video/h263");
            dict.Add(".h264", "video/h264");
            dict.Add(".rip", "audio/vnd.rip");
            dict.Add(".ief", "image/ief");
            dict.Add(".jpm", "video/jpm");
            dict.Add(".jpeg", "image/jpeg");
            dict.Add(".jpg", "image/jpeg");
            dict.Add(".jpgv", "video/jpeg");
            dict.Add(".lvp", "audio/vnd.lucent.voice");
            dict.Add(".m3u", "audio/x-mpegurl");
            dict.Add(".m4v", "video/x-m4v");
            dict.Add(".asf", "video/x-ms-asf");
            dict.Add(".pya", "audio/vnd.ms-playready.media.pya");
            dict.Add(".pyv", "video/vnd.ms-playready.media.pyv");
            dict.Add(".wm", "video/x-ms-wm");
            dict.Add(".wma", "audio/x-ms-wma");
            dict.Add(".wax", "audio/x-ms-wax");
            dict.Add(".wmx", "video/x-ms-wmx");
            dict.Add(".wmv", "video/wmv");
            dict.Add(".wvx", "video/x-ms-wvx");
            dict.Add(".mid", "audio/midi");
            dict.Add(".mj2", "video/mj2");
            dict.Add(".mpga", "audio/mpeg");
            dict.Add(".mxu", "video/vnd.mpegurl");
            dict.Add(".mpeg", "video/mpeg");
            dict.Add(".mp4a", "audio/mp4");
            dict.Add(".mp4", "video/mp4");
            dict.Add(".ecelp4800", "audio/vnd.nuera.ecelp4800");
            dict.Add(".ecelp7470", "audio/vnd.nuera.ecelp7470");
            dict.Add(".ecelp9600", "audio/vnd.nuera.ecelp9600");
            dict.Add(".oga", "audio/ogg");
            dict.Add(".ogv", "video/ogg");
            dict.Add(".weba", "audio/webm");
            dict.Add(".webm", "video/webm");
            dict.Add(".ktx", "image/ktx");
            dict.Add(".pcx", "image/x-pcx");
            dict.Add(".psd", "image/vnd.adobe.photoshop");
            dict.Add(".pic", "image/x-pict");
            dict.Add(".pnm", "image/x-portable-anymap");
            dict.Add(".pbm", "image/x-portable-bitmap");
            dict.Add(".pgm", "image/x-portable-graymap");
            dict.Add(".png", "image/png");
            dict.Add(".ppm", "image/x-portable-pixmap");
            dict.Add(".qt", "video/quicktime");
            dict.Add(".ram", "audio/x-pn-realaudio");
            dict.Add(".rmp", "audio/x-pn-realaudio-plugin");
            dict.Add(".svg", "image/svg+xml");
            dict.Add(".movie", "video/x-sgi-movie");
            dict.Add(".rgb", "image/x-rgb");
            dict.Add(".au", "audio/basic");
            dict.Add(".tiff", "image/tiff");
            dict.Add(".viv", "video/vnd.vivo");
            dict.Add(".wbmp", "image/vnd.wap.wbmp");
            dict.Add(".wav", "audio/x-wav");
            dict.Add(".webp", "image/webp");
            dict.Add(".xbm", "image/x-xbitmap");
            dict.Add(".xpm", "image/x-xpixmap");
            dict.Add(".xwd", "image/x-xwindowdump");
        }

        /// <summary>
        /// Generate File Name
        /// </summary>
        /// <returns>Dictionary dictionary</returns>
        public Dictionary<string, string> GetMimeType()
        {
            return dict;
        }
    }
}
