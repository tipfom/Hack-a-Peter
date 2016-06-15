using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace hack_a_peter
{
    public class Assets
    {
        #region static instances
        public static AssetsManager<Texture2D> Textures = new AssetsManager<Texture2D>("textures");
        public static AssetsManager<SpriteFont> Fonts = new AssetsManager<SpriteFont>("fonts");
        public static AssetsManager<Video> Videos = new AssetsManager<Video>("videos");
        // it seems, that there is a difference between Song and SoundEffect.
        public static AssetsManager<Song> Songs = new AssetsManager<Song>("songs");
        public static AssetsManager<SoundEffect> SoundEffects = new AssetsManager<SoundEffect>("sounds");
        #endregion

        #region static methods
        public static void Load(string initfilepath, ContentManager content)
        {
            // initfile is always in assets
            initfilepath = Path.Combine("Assets", initfilepath);

            Console.WriteLine("loading assets from " + initfilepath);
            using (Stream filestream = File.OpenRead(initfilepath))
            {
                using (StreamReader filereader = new StreamReader(filestream))
                {
                    string line;
                    int lineIndex = 0;
                    while ((line = filereader.ReadLine()) != null)
                    {
                        if (line.Length == 0)
                            continue;

                        char identifier = line.ToCharArray()[0];
                        string[] lineData = line.Remove(0, 2).Split(new string[] { " \"" }, StringSplitOptions.RemoveEmptyEntries);
                        if (lineData.Length == 2)
                        {
                            string assetsName = lineData[0];
                            string filePath = lineData[1].Remove(lineData[1].Length - 1); // remove last "

                            switch (line.ToCharArray()[0])
                            {
                                case 'A':
                                case 'a':
                                    SoundEffects.Load(assetsName, filePath, content);
                                    break;
                                case 'F':
                                case 'f':
                                    Fonts.Load(assetsName, filePath, content);
                                    break;
                                case 'S':
                                case 's':
                                    Songs.Load(assetsName, filePath, content);
                                    break;
                                case 'T':
                                case 't':
                                    Textures.Load(assetsName, filePath, content);
                                    break;
                                case 'V':
                                case 'v':
                                    Videos.Load(assetsName, filePath, content);
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("invalid initfile-format in file " + initfilepath + " at line " + lineIndex.ToString());
                        }

                        lineIndex++;
                    }
                }
            }
        }
        #endregion

        public class AssetsManager<T>
        {
            private Dictionary<string, T> loadedAssets = new Dictionary<string, T>();
            private string homePath;

            public AssetsManager(string homepath)
            {
                homePath = homepath;
            }

            public void Load(string assname, string filepath, ContentManager content)
            {
                // all assets of one type have their specific folder
                filepath = Path.Combine(homePath, filepath);

                // check if file doesnt exist and no *.xnb copy of the file exists (... fonts)
                if (!File.Exists(Path.Combine("Assets", filepath)) && !File.Exists(Path.Combine("Assets", Path.ChangeExtension(filepath, "xnb"))))
                {
                    Console.WriteLine("couldn't load asset " + assname + " of type " + typeof(T).Name + ", because the file at " + filepath + " did not exist");
                    return;
                }

                if (!loadedAssets.ContainsKey(assname))
                {
                    loadedAssets.Add(assname, content.Load<T>(filepath));
                    Console.WriteLine("loaded asset " + assname + " of type " + typeof(T).Name + " from " + filepath);
                }
                else
                {
                    Console.WriteLine("asset " + assname + " of type " + typeof(T).Name + " has allready been loaded.");
                }
            }

            public T Get(string assname)
            {
                if (loadedAssets.ContainsKey(assname))
                {
                    return loadedAssets[assname];
                }
                else
                {
                    Console.WriteLine("asset " + assname + " of type " + typeof(T).Name + " was requested without being loaded, returning default.");
                    return default(T);
                }
            }

            public void Delete(string assname)
            {
                if (loadedAssets.ContainsKey(assname))
                {
                    // hohoho garbage collector
                    loadedAssets.Remove(assname);
                    Console.WriteLine("asset " + assname + " of type " + typeof(T).Name + " got deleted");
                }
                else
                {
                    Console.WriteLine("asset " + assname + " of type " + typeof(T).Name + " was tried to delete while not being loaded.");
                }
            }
        }
    }
}