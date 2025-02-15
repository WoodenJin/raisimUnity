/*
 * Author: Dongho Kang (kangd@ethz.ch)
 */

using System.Collections.Generic;
using System.IO;
using Collada141;
using UnityEngine;

namespace raisimUnity
{
    public class ResourceLoader
    {
        private List<string> _resourceDirs;

        private const string _keyNoRscDir = "NoRscDir";
        private const string _keyRscDir = "RscDir";

        public ResourceLoader()
        {  
            _resourceDirs = new List<string>();
        }

        public void AddResourceDirectory(string path)
        {
            _resourceDirs.Add(path);
        }

        public void LoadFromPref()
        {
            int numberResourceDirs = 0;
            if (PlayerPrefs.HasKey(_keyNoRscDir))
                numberResourceDirs= PlayerPrefs.GetInt(_keyNoRscDir);
            
            for (int i = 0; i < numberResourceDirs; i++)
            {
                string path = PlayerPrefs.GetString(_keyRscDir+i);
                _resourceDirs.Add(path);
            }
        }

        public void SaveToPref()
        {
            PlayerPrefs.SetInt(_keyNoRscDir, _resourceDirs.Count);
            for(int i = 0; i < _resourceDirs.Count; i++)
            {
                PlayerPrefs.SetString(_keyRscDir+i, _resourceDirs[i]);
            }
        }

        public void RemoveResourceDirectory()
        {
            _resourceDirs.RemoveAt(_resourceDirs.Count - 1);
        }

        public string RetrieveMeshPath(string meshDirPathInServer, string meshName)
        {
            // find a full mesh path from client side.
            // return null if it cannot find the file.
            
            var parent = Directory.GetParent(meshDirPathInServer).FullName;    // .../rsc/robot/alma
            var grandParent = Directory.GetParent(parent).FullName;            // .../rsc/robot

            var curr = Path.GetFileName(meshDirPathInServer);                 // urdf
            parent = Path.GetFileName(parent);                                // alma    
            grandParent = Path.GetFileName(grandParent);                      // robot

            // 1. check from grand parent
            //
            // e.g.
            // meshDirPathInServer: .../rsc/robot/alma/urdf/
            // meshName: .../meshes/anymal_base.dae
            //
            // curr = urdf    parent = alma    grandParent = robot
            foreach (var dir in _resourceDirs)
            {
                var meshPath = Path.Combine(dir, grandParent, parent, curr, meshName);
                if (File.Exists(meshPath))
                {
                    return meshPath;
                }
            }
            
            // 2. check from parent 
            //
            // e.g.
            // meshDirPathInServer: .../rsc/alma/urdf/
            // meshName: .../meshes/anymal_base.dae
            //
            // curr = urdf    parent = alma    
            foreach (var dir in _resourceDirs)
            {
                var meshPath = Path.Combine(dir, parent, curr, meshName);
                if (File.Exists(meshPath))
                {
                    return meshPath;
                }
            }
            
            // 3. check from curr 
            //
            // e.g.
            // meshDirPathInServer: .../rsc/anymal/
            // meshName: anymal_base.dae
            //
            // curr = anymal
            foreach (var dir in _resourceDirs)
            {
                var meshPath = Path.Combine(dir, curr, meshName);
                if (File.Exists(meshPath))
                {
                    return meshPath;
                }
            }
            
            // couldn't find mesh from resource directories
            return null;
        }

        public List<string> ResourceDirs
        {
            get { return _resourceDirs; }
        }
    }
}