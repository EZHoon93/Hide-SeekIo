using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace AlmostEngine.Screenshot
{
    public class ScreenshotNameParser
    {

        public enum DestinationFolder
        {
            CUSTOM_FOLDER,
            DATA_PATH,
            PERSISTENT_DATA_PATH,
            PICTURES_FOLDER
        }
        ;

        public static string ParseSymbols(string name, ScreenshotResolution resolution, System.DateTime time)
        {
            // Add a 0 before numbers if < 10
            if (time.Month < 10)
            {
                name = name.Replace("{month}", "0{month}");
            }
            if (time.Day < 10)
            {
                name = name.Replace("{day}", "0{day}");
            }
            if (time.Hour < 10)
            {
                name = name.Replace("{hour}", "0{hour}");
            }
            if (time.Minute < 10)
            {
                name = name.Replace("{minute}", "0{minute}");
            }
            if (time.Second < 10)
            {
                name = name.Replace("{second}", "0{second}");
            }

            // Date
            name = name.Replace("{year}", time.Year.ToString());
            name = name.Replace("{month}", time.Month.ToString());
            name = name.Replace("{day}", time.Day.ToString());
            name = name.Replace("{hour}", time.Hour.ToString());
            name = name.Replace("{minute}", time.Minute.ToString());
            name = name.Replace("{second}", time.Second.ToString());

            // Dimensions
            name = name.Replace("{width}", resolution.m_Width.ToString());
            name = name.Replace("{height}", resolution.m_Height.ToString());
            name = name.Replace("{scale}", resolution.m_Scale.ToString());
            name = name.Replace("{ratio}", resolution.m_Ratio).Replace(":", "_");

            // Resolution
            name = name.Replace("{orientation}", resolution.m_Orientation.ToString());
            name = name.Replace("{name}", resolution.m_ResolutionName);
            name = name.Replace("{ppi}", resolution.m_PPI.ToString());
            name = name.Replace("{category}", resolution.m_Category);
            //			name = name.Replace ("{percent}", resolution.m_Stats.ToString ());

            return name;
        }

        public static string ParseExtension(TextureExporter.ImageFileFormat format)
        {
            if (format == TextureExporter.ImageFileFormat.PNG)
            {
                return ".png";
            }
            else
            {
                return ".jpg";
            }
        }

        public static string ParsePath(DestinationFolder destinationFolder, string customPath)
        {
            string path = "";

			#if !UNITY_EDITOR && UNITY_ANDROID
                if (destinationFolder == DestinationFolder.PICTURES_FOLDER) {
				    path = AndroidUtils.GetExternalPictureDirectory() + "/" +customPath;
                } else {
				    path = AndroidUtils.GetFirstAvailableMediaStorage() + "/" +customPath;
                }
			#elif !UNITY_EDITOR && UNITY_IOS
				path = Application.persistentDataPath + "/" +customPath;
			#else
				if (destinationFolder == DestinationFolder.CUSTOM_FOLDER)
				{
					path = customPath;
				}
				else if (destinationFolder == DestinationFolder.PERSISTENT_DATA_PATH)
				{
					// #if UNITY_EDITOR || UNITY_STANDALONE
					// path = Application.persistentDataPath + "/" + customPath;
					// #elif UNITY_ANDROID
					// 				path = AndroidUtils.GetFirstAvailableMediaStorage()  + "/" + customPath;
					// #else 
					path = Application.persistentDataPath + "/" + customPath;
					// #endif
				}
				else if (destinationFolder == DestinationFolder.DATA_PATH)
				{
					path = Application.dataPath + "/" + customPath;
				}
				else if (destinationFolder == DestinationFolder.PICTURES_FOLDER)
				{
					#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WSA || UNITY_WSA_10_0
						path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "/" + customPath;
						// #elif UNITY_ANDROID
						// 				path = AndroidUtils.GetExternalPictureDirectory()  + "/" + customPath;
						// #elif UNITY_IOS
						// 				path = Application.persistentDataPath + "/" +customPath;
					#else
						path = Application.persistentDataPath + "/" +customPath;
					#endif
				}
			#endif

            // Add a slash if not already at the end of the folder name
            if (path.Length > 0)
            {
                path = path.Replace("//", "/");
                if (path[path.Length - 1] != '/' && path[path.Length - 1] != '\\')
                {
                    path += "/";
                }
            }

            return path;
        }


        /// <summary>
        /// Returns the parsed screenshot filename using the symbols, extensions and special folders.
        /// </summary>
        public static string ParseFileName(string screenshotName, ScreenshotResolution resolution, DestinationFolder destination, string customPath, TextureExporter.ImageFileFormat format, bool overwriteFiles, System.DateTime time)
        {
            string filename = "";

#if UNITY_EDITOR || !UNITY_WEBGL
            // Destination Folder can not be parsed in webgl
            filename += ParsePath(destination, customPath);
#endif

            // File name
            filename += ParseSymbols(screenshotName, resolution, time);

            // Get the file extension
            filename += ParseExtension(format);


            // Increment the file number if a file already exist
            if (!overwriteFiles)
            {
                return PathUtils.PreventOverwrite(filename);
            }

            return filename;
        }

    }
}
