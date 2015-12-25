﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using PowerPointLabs.ImagesLab.Model;
using PowerPointLabs.Properties;
using PowerPointLabs.Views;

namespace PowerPointLabs.ImagesLab.Util
{
    class StoragePath
    {
        private const string ImagesLabImagesList = "ImagesLabImagesList";

        public static string AggregatedFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + "pptlabs_imagesLab" + @"\";

        public static readonly string LoadingImgPath = AggregatedFolder + "loading";

        public static bool InitPersistentFolder(ICollection<string> filesInUse)
        {
            try
            {
                Empty(new DirectoryInfo(AggregatedFolder), filesInUse);
            }
            catch (Exception e)
            {
                ErrorDialogWrapper.ShowDialog("Failed to remove unused images.", e.Message, e);
            }

            if (!Directory.Exists(AggregatedFolder))
            {
                try
                {
                    Directory.CreateDirectory(AggregatedFolder);
                }
                catch
                {
                    return false;
                }
            }

            InitResources();
            return true;
        }

        private static void Empty(DirectoryInfo directory, ICollection<string> filesInUse)
        {
            if (!directory.Exists) return;

            try
            {
                filesInUse.Add(AggregatedFolder + ImagesLabImagesList);
                filesInUse.Add(LoadingImgPath);
                foreach (var file in directory.GetFiles())
                {
                    if (!filesInUse.Contains(file.FullName))
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch
                        {
                            // may be still in use, which is fine
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignore ex, if cannot delete trash
            }
        }

        private static void InitResources()
        {
            try
            {
                Resources.Loading.Save(LoadingImgPath);
            }
            catch
            {
                // may fail to save it, which is fine
            }
        }

        public static string GetPath(string name)
        {
            return AggregatedFolder + name;
        }

        /// <summary>
        /// Save images list
        /// </summary>
        /// <param name="list"></param>
        public static void Save(Collection<ImageItem> list)
        {
            try
            {
                using (var writer = new StreamWriter(GetPath(ImagesLabImagesList)))
                {
                    var serializer = new XmlSerializer(list.GetType());
                    serializer.Serialize(writer, list);
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                PowerPointLabsGlobals.Log("Failed to save Images Lab settings: " + e.StackTrace, "Error");
            }
        }

        /// <summary>
        /// Load images list
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<ImageItem> Load()
        {
            try
            {
                using (var stream = File.OpenRead(GetPath(ImagesLabImagesList)))
                {
                    var serializer = new XmlSerializer(typeof(ObservableCollection<ImageItem>));
                    var list = serializer.Deserialize(stream) as ObservableCollection<ImageItem> 
                        ?? new ObservableCollection<ImageItem>();
                    return list;
                }
            }
            catch (Exception e)
            {
                PowerPointLabsGlobals.Log("Failed to load Images Lab settings: " + e.StackTrace, "Error");
                return new ObservableCollection<ImageItem>();
            }
        }
    }
}
