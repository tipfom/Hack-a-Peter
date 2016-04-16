using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace hack_a_peter {
    public static class AssetsManager {
        #region FontManagers
        public static SpriteFont GameFont12 { get; private set; }
        public static SpriteFont GameFont14 { get; private set; }
        public static SpriteFont GameFont32 { get; private set; }
        public static SpriteFont GameFont48 { get; private set; }
        public static SpriteFont GameFont72 { get; private set; }

        public static void LoadGameFont (ContentManager content) {
            string directory = "fonts";
            if (GameFont12 == null) {
                GameFont12 = content.Load<SpriteFont> (Path.Combine (directory, "12px"));
                GameFont14 = content.Load<SpriteFont> (Path.Combine (directory, "14px"));
                GameFont32 = content.Load<SpriteFont> (Path.Combine (directory, "32px"));
                GameFont48 = content.Load<SpriteFont> (Path.Combine (directory, "48px"));
                GameFont72 = content.Load<SpriteFont> (Path.Combine (directory, "72px"));
                Console.WriteLine ("loaded gamefonts from " + directory);
            } else {
                Console.WriteLine ("tried to load gamefont, while gamefont has allready been loaded");
            }
        }
        #endregion  

        #region TextureManager
        private static Dictionary<string, Texture2D> loadedImages = new Dictionary<string, Texture2D> ();

        public static void Load (string assname, string filepath, ContentManager content) {
            // all images are in image folder
            filepath = Path.Combine ("images", filepath);
            if (!File.Exists (Path.Combine ("Assets", filepath))) {
                Console.WriteLine ("couldn't load texture " + assname + ", because the file at " + filepath + " did not exist");
                return;
            }

            if (!loadedImages.ContainsKey (assname)) {
                loadedImages.Add (assname, content.Load<Texture2D> (filepath));
                Console.WriteLine ("loaded texture " + assname + " from " + filepath);
            } else {
                Console.WriteLine ("texture " + assname + " has allready been loaded.");
            }
        }

        public static Texture2D Get (string assname) {
            if (loadedImages.ContainsKey (assname)) {
                return loadedImages[assname];
            } else {
                Console.WriteLine ("texture " + assname + " was requested without being loaded, returning null.");
                return null;
            }
        }

        public static void Delete (string assname) {
            if (loadedImages.ContainsKey (assname)) {
                loadedImages[assname].Dispose ();
                loadedImages.Remove (assname);
                Console.WriteLine ("texture " + assname + " got deleted");
            } else {
                Console.WriteLine ("texture " + assname + " was tried to delete while not being loaded.");
            }
        }

        public static void DeleteAll () {
            Console.WriteLine ("deleting all loaded textures");
            List<string> loadedAssetNames = loadedImages.Keys.ToList<string> ();
            for (int i = 0; i < loadedAssetNames.Count; i++) {
                Delete (loadedAssetNames[i]);
            }
            Console.WriteLine (loadedAssetNames.Count.ToString () + " textures deleted");
        }

        public static void Load (string initfilepath, ContentManager content) {
            // initfile is always in assets
            initfilepath = Path.Combine ("Assets", initfilepath);

            Console.WriteLine ("loading textures from " + initfilepath);
            using (Stream filestream = File.OpenRead (initfilepath)) {
                using (StreamReader filereader = new StreamReader (filestream)) {
                    string line;
                    int lineIndex = 0;
                    while ((line = filereader.ReadLine ()) != null) {
                        string[] lineData = line.Split (new string[] { " \"" }, StringSplitOptions.RemoveEmptyEntries);
                        if (lineData.Length == 2) {
                            string assetsName = lineData[0];
                            string filePath = lineData[1].Replace ("\"", ""); // remove last "
                            Load (assetsName, filePath, content);
                        } else {
                            Console.WriteLine ("invalid initfile-format in file " + initfilepath + " at line " + lineIndex.ToString ());
                        }
                        lineIndex++;
                    }
                }
            }
        }
        #endregion
    }
}