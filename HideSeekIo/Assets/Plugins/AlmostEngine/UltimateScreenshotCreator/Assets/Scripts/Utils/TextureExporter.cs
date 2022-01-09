using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AlmostEngine.Screenshot
{
    public class TextureExporter
    {

        #region Saving

        public enum ImageFileFormat
        {
            PNG,
            JPG
        }
        ;

        /// <summary>
        /// Exports to file.
        /// </summary>
        /// <returns><c>true</c>, if to file was exported, <c>false</c> otherwise.</returns>
        /// <param name="texture">Texture.</param> The texture to export.
        /// <param name="fullpath">Filename.</param> The filename must be a valid full path. Use the ScreenshotNameParser to get a valid path.
        /// <param name="imageFormat">Image format.</param>
        /// <param name="JPGQuality">JPG quality.</param>
        public static bool ExportToFile(Texture2D texture, string fullpath, ImageFileFormat imageFormat, int JPGQuality = 70, bool addToGallery = true)
        {

            if (texture == null)
            {
                Debug.LogError("Can not export the texture to file " + fullpath + ", texture is empty.");
                return false;
            }

#if UNITY_WEBPLAYER

            Debug.Log("WebPlayer is not supported.");
            return false;

#else

            // Convert texture to bytes
            byte[] bytes = null;
            if (imageFormat == ImageFileFormat.JPG)
            {
                bytes = texture.EncodeToJPG(JPGQuality);
            }
            else
            {
                bytes = texture.EncodeToPNG();
            }

#endif


#if !UNITY_EDITOR && UNITY_WEBGL

            // Create a downloadable image for the web browser
            try {
                string shortFileName = fullpath;
                int index = fullpath.LastIndexOf('/');
                if (index >= 0) {
                    shortFileName = fullpath.Substring(index+1);
                }
                string format = (imageFormat == ImageFileFormat.JPG) ? "jpeg" : "png";
                WebGLUtils.ExportImage(bytes, shortFileName, format);
            } catch {
                Debug.LogError ("Failed to create downloadable image.");
                return false;
            }


#elif !UNITY_WEBPLAYER

            // Create the directory
            if (!PathUtils.CreateExportDirectory(fullpath))
                return false;

            // Export the image
            try
            {
                System.IO.File.WriteAllBytes(fullpath, bytes);
            }
            catch
            {
                Debug.LogError("Failed to create the file : " + fullpath);
                return false;
            }

#endif



            if (addToGallery)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                // Update android gallery
                try {
                    AndroidUtils.AddImageToGallery(fullpath);
                } catch {
                    Debug.LogError ("Failed to add image to Android Gallery");
                    return false;
                }

#elif UNITY_IOS && !UNITY_EDITOR
                // Update ios gallery
                try {
                    iOsUtils.AddImageToGallery(fullpath);
                } catch {
                    Debug.LogError ("Failed to add image to iOS Gallery");
                    return false;
                }
#endif
            }



#if !UNITY_WEBPLAYER
            return true;
#endif


        }

        #endregion

        #region Loading

        public static Texture2D LoadFromFile(string fullname)
        {
            if (!System.IO.File.Exists(fullname))
            {
                Debug.LogError("Can not load texture from file " + fullname + ", file does not exists.");
                return null;
            }

            byte[] bytes = System.IO.File.ReadAllBytes(fullname);
            Texture2D texture = new Texture2D(2, 2);
            if (!texture.LoadImage(bytes))
            {
                Debug.LogError("Failed to load the texture " + fullname + ".");
            }

            return texture;

        }

        [System.Serializable]
        public class ImageFile
        {
            public Texture2D m_Texture;
            public string m_Name;
            public string m_Fullname;
            public System.DateTime m_CreationDate;
        }

        public static List<ImageFile> LoadFromPath(string path)
        {

            List<ImageFile> images = new List<ImageFile>();

            if (!System.IO.Directory.Exists(path))
            {
                Debug.LogError("Can not load images from directory " + path + ", directory does not exists.");
                return images;
            }

            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension == ".jpg" || file.Extension == ".png")
                {

                    ImageFile item = new ImageFile();
                    item.m_Name = file.Name;
                    item.m_Fullname = file.FullName;
                    item.m_CreationDate = file.CreationTime;
                    item.m_Texture = TextureExporter.LoadFromFile(file.FullName);

                    images.Add(item);
                }
            }

            return images;
        }

        #endregion
    }
}
