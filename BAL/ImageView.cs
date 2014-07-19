using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IMPOS.BLL
{
    public static class ImageView
    {
        #region Methods

        public static List<ImageEntity> GetAllImagesData(string folderPath, long id)
        {
            try
            {
                List<ImageEntity> allImages = new List<ImageEntity>();
                if (Directory.Exists(folderPath + "\\" + id))
                {
                    foreach (string file in Directory.EnumerateFiles(folderPath + "\\" + id, "*.jpg"))
                    {
                        ImageEntity tempImage = new ImageEntity();
                        tempImage.ImagePath = file;
                        tempImage.OrginalNameImagePath = file;
                        allImages.Add(tempImage);
                    }
                    //MP4,avi,3gp,asx,asf,wmv,mkv,flv,mpv,mov,mp4,mpg,vob
                    foreach (string file in Directory.EnumerateFiles(folderPath + "\\" + id, "*.MP4")
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.avi"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.3gp"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.asx"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.asf"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.wmv"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.mkv"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.flv"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.mpv"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.mov"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.mp4"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.mpg"))
                        .Union(Directory.EnumerateFiles(folderPath + "\\" + id, "*.vob")))
                    {
                        ImageEntity tempImage = new ImageEntity();
                        tempImage.ImagePath =  folderPath + "\\Movie.png";
                        tempImage.OrginalNameImagePath = file;
                        allImages.Add(tempImage);
                    }
                }
                else
                {
                    Directory.CreateDirectory(folderPath + "\\" + id);
                }
                /*
                // Load Xml Document
                XDocument XDoc = XDocument.Load("ImageData.xml");

                // Query for retriving all Images data from XML
                var Query = from Q in XDoc.Descendants("Image")
                            select new ImageEntity
                            {
                                ImagePath = Q.Element("ImagePath").Value
                            };

                // return images data
                */
                return allImages;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        public static void addImage(string folderName, long ID, string filename)
        {
            var ext = filename.Substring(filename.LastIndexOf('.'));
            int count = Directory.EnumerateFiles(folderName + "\\" + ID, "*" + ext).Count();
            System.IO.File.Copy(filename, folderName + "\\" + ID + "\\" + (count + 1) + ext, false);
        }
    }
}
