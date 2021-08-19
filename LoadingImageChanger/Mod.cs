using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HarmonyLib;
using ModHelper;
using UnityEngine.UI;
using UnityEngine;

namespace LoadingImageChanger
{
    public class Mod : IMod
    {
        public string Name => "Loading Image Changer";

        public string Description => "Change Loading Image";

        public string Author => "BustR75";

        public string HomePage => "OwO";
        static float ModdedChance = 1.0f;
        static List<Sprite> Images = new List<Sprite>();

        public void DoPatching()
        {
            Directory.CreateDirectory("LoadingImages");
            var h = new Harmony(Guid.NewGuid().ToString());
            h.Patch(typeof(ImgIllus).GetMethod("OnEnable",BindingFlags.Instance|BindingFlags.NonPublic),
                new HarmonyMethod(typeof(Mod).GetMethod(nameof(LoadPatch),BindingFlags.Static|BindingFlags.NonPublic)));
            if (File.Exists("LoadingImages\\Config.cfg"))
            {
                foreach(string s in File.ReadAllLines("LoadingImages\\Config.cfg"))
                {
                    if (s.ToLower().StartsWith("moddedimagechance="))
                        ModdedChance =  Convert.ToSingle(s.Substring(18));
                }
            }
            else
            {
                File.WriteAllText("LoadingImages\\Config.cfg", "#Set The Chance That A Modded Image Is Used\nModdedImageChance=1.0");
            }
            var fi = new DirectoryInfo("LoadingImages").GetFiles();
            if (fi.Length == 1)
            {
                ModLogger.Debug("No Files Found In LoadingImages");
            }
            foreach (FileInfo f in fi)
            {
                try
                {
                    if (f.Extension != ".cfg")
                    {
                        Texture2D texture = new Texture2D(1, 1);
                        texture.LoadImage(File.ReadAllBytes(f.FullName));
                        texture.filterMode = FilterMode.Bilinear;
                        texture.Apply();
                        Images.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
                        ModLogger.Debug("Added " + f.Name);
                    }
                }
                catch
                {
                    ModLogger.Debug("Unknown file " + f.Name);
                }
            }
        }
        static bool LoadPatch(ImgIllus __instance,Image ___m_IllusImg)
        {
            ModLogger.Debug("Trying to load");
            if (Images.Count > 0 && UnityEngine.Random.Range(0, 1) <= ModdedChance)
            {
                ___m_IllusImg.sprite = Images[UnityEngine.Random.Range(0,Images.Count-1)];
                
                return false;
            }
            return true;
        }
    }
}
