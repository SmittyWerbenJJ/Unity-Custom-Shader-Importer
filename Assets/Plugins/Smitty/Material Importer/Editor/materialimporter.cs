using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Smitty.MaterialImporter
{
    public enum ParsingStates
    {
        noFile,
        waitForUser,
        parsingSuccessful,
        parsingFailed
    }

    public enum textureTypes
    {
        diffuse,
        alpha,
        specular,
        roughness,
        normal,
        metal,
        subsurface
    }
    public struct ParsingInformation
    {
        public List<string> Objects;
        public Dictionary<string, UnityEngine.Shader> Shaders;
        public Dictionary<string, UnityEngine.Texture> Textures;
        public Dictionary<string, UnityEngine.Material> Materials;
    }


    public struct internalMaterial
    {
        public UnityEngine.Material material;
        public Shader shader;
        public Dictionary<textureTypes, Texture> textures;
        public Dictionary<textureTypes, int> Values;
    }




    public struct materialDescription
    {
        public string name;
        public Shader shader;
        public Dictionary<textureTypes, Texture> textures;
        public List<(textureTypes, int)> Values;

        public materialDescription(string name = null, Shader shader = null, Dictionary<textureTypes, Texture> strTexture = null, List<(textureTypes, int)> values = null)
        {
            this.name = name;
            this.shader = shader;
            this.textures = strTexture;
            this.Values = values;
        }
    }


    [Serializable]
    public class Materialimporter
    {
        #region variables

        public List<internalMaterial> allInternalMaterials;

        public ParsingInformation parsingInformation;

        public bool scheduleParsing;
        public TextAsset jsonFile;
        public TextAsset jsonFileOLD;
        public DecompiledJson parsedJson;
        public ParsingStates parsingState;
        public Shader nullShader;
        public List<UnityEngine.Shader> shaders;
        public Dictionary<string, UnityEngine.Shader> ShaderDict;

        public DefaultAsset textureFolder;
        internal DefaultAsset materialFolder;
        public bool isParsed;

        #endregion

        public Materialimporter()
        {
            parsingInformation = new ParsingInformation();
            parsingInformation.Materials = new Dictionary<string, UnityEngine.Material>();
            parsingInformation.Shaders = new Dictionary<string, Shader>();
            parsingInformation.Textures = new Dictionary<string, Texture>();
            parsingInformation.Objects = new List<string>();
        }
        // public List<customMaterial> customMaterials = new List<customMaterial>();

        /// <summary>
        /// Reads the provided Text asset as Json File.
        /// </summary>
        /// <param name="jsonAsset"></param>
        /// <param name="objects"></param>
        /// <param name="materials"></param>
        /// <returns>The success status of the reading </returns>
        public void parseJson(TextAsset jsonAsset)
        {
            //Reset the Json File and parse 
            parsedJson = null;

            //Skip on empty Field
            if (jsonAsset == null)
            {
                parsingState = ParsingStates.parsingFailed;
                isParsed = false;
                return;
            }

            //Parse the Json

            try
            {
                parsedJson = JsonUtility.FromJson<DecompiledJson>(jsonAsset.text);
                parsingState = ParsingStates.parsingSuccessful;
                isParsed = true;

            }
            catch (System.Exception)
            {
                Debug.LogWarning("Json Parsing Failed!");
                parsingState = ParsingStates.parsingFailed;
                isParsed = false;
                return;
            }



            //Set up the Shader Dict, using parsingstate as condition
            if (parsingState == ParsingStates.parsingSuccessful)
            {
                ShaderDict = new Dictionary<string, Shader>();
                foreach (var shaderName in getShaderNames())
                {
                    ShaderDict[shaderName] = null;
                }

            }

            //Set up the parsingInformation Dictionaries --- for UI display

            //Fill the Parsing INformation
            var mats = new Dictionary<string, UnityEngine.Material>();
            var shaders = new Dictionary<string, UnityEngine.Shader>();
            var textures = new Dictionary<string, UnityEngine.Texture>();

            //List of all textures - used for many materials
            var allTextures = new List<string>();

            //Fill the dictonrays with <keys,null> -> each entry should be unique by default

            //*==> Material /  shader Dictionaries
            foreach (var myMaterial in parsedJson.materials)
            {
                //pushes unique material Names into the dictonray
                string _currentMaterial = myMaterial.name;
                if (!mats.ContainsKey(_currentMaterial))
                    mats.Add(_currentMaterial, null);

                //pushes unique Shader  Names into the dictonray
                string _currentShader = myMaterial.type;
                if (!shaders.ContainsKey(_currentShader))
                    shaders.Add(_currentShader, null);

                //pushes unique Texture Names into the dictonray
                allTextures.AddRange(myMaterial.maps.getNames());
            }

            //*==> Texture  Dictionaries
            foreach (var _currentTexture in allTextures)
            {
                if (!textures.ContainsKey(_currentTexture))
                    textures.Add(_currentTexture, null);
            }

            //assign Dictionray to parsingINfo
            parsingInformation.Objects = parsedJson.objects;
            parsingInformation.Materials = mats;
            parsingInformation.Shaders = shaders;
            parsingInformation.Textures = textures;
        }


        /// <summary>
        /// Creates a complete internal material from the parsed Json file, which can be converted to a actual Unity material
        /// </summary>
        /// <param name="materialName">The material Name to search from the parsed Json File</param>
        internalMaterial createInternalMaterial(string materialName)
        {
            var x = new internalMaterial();

            x.material = findMaterial(materialName);
            x.shader = findShaderfromMaterial(materialName);
            x.textures = findTexturesfromMaterial(materialName);
            x.Values = findValuesFromMaterial(materialName);

            return x;

        }
        private Shader getDefaultShader()
        {
            return AssetDatabase.GetBuiltinExtraResource<UnityEngine.Shader>("Lit.shader");
        }

        private UnityEngine.Material findMaterial(string materialName)
        {
            var Names = new List<string>();

            foreach (var item in parsedJson.materials)
                Names.Add(item.name);

            if (Names.Contains(materialName))
                return new UnityEngine.Material(getDefaultShader());
            else return null;
        }



        public void exportMaterials()
        {
            //collecting the data  - physical asset that will be loaded from disk
            var internalMaterialList = parsedJson.materials;
            var textureNames = getTextureNames();
            var textureAssets = getTextureAssets();
            var shadernames = getShaderNames();
            var shaderAssets = getshaderAssets();

            /// create MaterialDescriptions and put them in a internal List
            foreach (var material in internalMaterialList)
            {
                var name = material.name;
                var shader = shaderAssets[material.type];
                var textures = findTexturesfromMaterial(material.name);
                var x = new materialDescription(name, shader, textures);
            }

            throw new NotImplementedException();
        }


        //Returns the mapped shader from the dict
        private Shader findShaderfromMaterial(string ShaderName)
        {
            return ShaderDict[ShaderName];
        }

        /// <summary>
        /// Returns a Dictionray with the Textures for the provided Material
        /// </summary>
        /// <param name="materialName"></param>
        /// <returns></returns>
        private Dictionary<textureTypes, Texture> findTexturesfromMaterial(string materialName)
        {
            //makes Dictionry will all textures of <textureType,texture Name> for the material
            var textureMapsInternal = new Dictionary<textureTypes, string>();
            var textureMapsIntermediate = new Dictionary<string, Texture>();
            var textureMapsAssets = new Dictionary<textureTypes, Texture>();

            //fill internal dict with maps from the material     <textureTypes, string>        
            textureMapsInternal = getInternalDict(materialName);

            //fill intermediate dict with textures from disk     <string, Texture>
            var txtFolderpath = AssetDatabase.GetAssetPath(textureFolder);
            textureMapsIntermediate = getIntermediateDict(txtFolderpath);
            //fill assets dict with textures with types      <textureTypes, Texture>
            textureMapsAssets = getAssetsDict(textureMapsInternal, textureMapsIntermediate);

            //Get all textures from the provided Asset path and filter out the requested textures
            //- Get all the textures
            //convert assets to textures
            //First add the textureTypes to the final dict
            //Then add the texture assets to the texture types in the Dict
            // merge the 2 dictionaries "<texturetypes,string> --- <texturetypes,texture>"
            /** get the Texture from Assets */
            //get all textures in the texture folder

            Dictionary<textureTypes, string> getInternalDict(string materialName)
            {
                var newDict = new Dictionary<textureTypes, string>();
                // foreach (var mat in parsedJson.materials)
                //     if (mat.name == materialName)
                //     {
                //         textureMapsInternal = mat.maps.getAllMaps();
                //         break;
                //     }

                return newDict;
            }

            Dictionary<string, Texture> getIntermediateDict(string txtFolderpath)
            {
                var newDict = new Dictionary<string, Texture>();
                // var assetObjects = new List<UnityEngine.Object>(AssetDatabase.LoadAllAssetsAtPath(txtFolderpath));
                // var allTextureAssets = new List<UnityEngine.Texture>();

                // foreach (var obj in assetObjects)
                // {
                //     if (obj.GetType() == typeof(UnityEngine.Texture)) //only use texture assets
                //     {
                //         allTextureAssets.Add((UnityEngine.Texture)obj);
                //     }
                // }
                return newDict;
            }

            Dictionary<textureTypes, Texture> getAssetsDict(Dictionary<textureTypes, string> internalDict, Dictionary<string, Texture> intermediateDict)
            {
                var newDict = new Dictionary<textureTypes, Texture>();
                // foreach (var obj in textureMapsInternal.Keys)
                // {
                //     textureMapsAssets[obj] = textureMapsInternal[];

                // }



                // //add them to a physical Dict of <textureType,physical textures asset>
                // foreach (var item in new List<textureTypes, Texture>(textureMapsInternal))
                // {


                //     var newTexture = FindTexture(/* all texture assets */ );
                //     var newElement = new Dictionary<textureTypes, Texture>();

                // }

                return newDict;
            }
            return textureMapsAssets;
            //return the physicalDictionary with their types 
        }


        private Dictionary<textureTypes, int> findValuesFromMaterial(string materialName)
        {
            throw new NotImplementedException();
        }

        public List<materialDescription> createAllMaterialDescriptions()
        {
            var descriptions = new List<materialDescription>();
            // foreach (var item in parsedJson.materials)
            // {
            //     var _name = item.name;
            //     var _shader = getShaderNames().Contains(item.type) ? item.type : null;
            //     var _textures = getTextureNames();
            //     var newDescription = new materialDescription(_name, _shader, _textures, _values);
            //     descriptions.Add(newDescription);
            // }

            return descriptions;
        }


        /// <summary>
        /// Find all available Shaders types from the Decompiled File. 
        /// </summary>
        /// <returns>Returns a unique List of Shader names  </returns>
        public List<string> getShaderNames()
        {
            List<string> ShaderNames = new List<string>();

            if (parsedJson == null)
                return ShaderNames;

            foreach (var item in parsedJson.materials)
            {
                ShaderNames.Add(item.type);
            }
            ShaderNames = ShaderNames.MakeUnique();

            return ShaderNames;
        }


        Dictionary<string, Shader> getshaderAssets()
        {
            return parsingInformation.Shaders;
        }


        //Returns all the Texture assets found in the class-provided Asset Folder
        List<Texture> getTextureAssets()
        {
            List<Texture> textures = new List<Texture>();

            var tmp = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(textureFolder));
            foreach (var item in tmp)
                if (item.GetType() == typeof(Texture))
                    textures.Add((Texture)item);

            return textures;
        }

        /// <summary>
        /// Find all available Textures from the Decompiled File. 
        /// </summary>
        /// <returns>Returns a unique List of Texture names  </returns>
        public List<string> getTextureNames()
        {
            List<string> TextureNames = new List<string>();

            if (parsedJson == null) return TextureNames;


            foreach (var item in parsedJson.materials)
            {

                TextureNames.Add(item.maps.map_alpha.texture);
                TextureNames.Add(item.maps.map_diffuse.texture);
                TextureNames.Add(item.maps.map_metal.texture);
                TextureNames.Add(item.maps.map_normal.texture);
                TextureNames.Add(item.maps.map_roughness.texture);
                TextureNames.Add(item.maps.map_specular.texture);
                TextureNames.Add(item.maps.map_subsurface.texture);
            }
            return TextureNames.MakeUnique();
        }

        public UnityEngine.Material createMaterial(materialDescription materialDescription)
        {



            UnityEngine.Material newMaterial = null;

            // //Return if no valid Shader found
            // if (materialDescription.shader == null)
            // {
            //     Debug.LogWarning("No Valid Shader for Materila creation found!");
            //     return null;
            // }

            // //Create The a Material with the provided Shader
            // newMaterial = new UnityEngine.Material(materialDescription.shader);
            // newMaterial.name = materialDescription.name;


            // //TODO assign Textures
            // var maps = materialDescription.textures;

            // var textureMap = FindSingleTexture(maps.map_diffuse.texture);
            // newMaterial.SetTexture("base map", textureMap);

            return newMaterial;
        }

        // public bool createMaterials_unity(string outputFolderPath, List<Shader> shaders)
        // {
        //     UnityEngine.Material newMaterial;

        //     foreach (var customMaterial in customMaterials)
        //     {
        //         //match the shader 
        //         foreach (var shader in shaders)
        //         {
        //             if (customMaterial.shader == shader.name)
        //             {
        //                 newMaterial = new UnityEngine.Material(shader);
        //                 break;
        //             }
        //         }

        //     }
        //     return true;
        // }
        // public bool createMaterials_internal(DefaultAsset textureFolder)
        // {
        //     /*
        //     materials
        //     types / shaders        
        //     -> List customMaterial
        //     */
        //     bool success = true;
        //     foreach (var material in parsedJson.materials)
        //     {
        //         customMaterial customMaterial = new customMaterial();
        //         string materialName = material.name;

        //         //Assignment of The Texture maps in the New Material
        //         var textures = FindTextures(material, textureFolder);

        //         //Assignment of The Values maps in the New Material
        //         var values = FindValues(material);

        //         //assignmet of the shader type in the new material
        //         var shaderType = FindShaderType(material);

        //         //assignment
        //         customMaterial.name = material.name;
        //         customMaterial.texturemaps = textures;
        //         customMaterial.values = values;
        //         customMaterial.shader = shaderType;

        //         customMaterials.Add(customMaterial);
        //     }
        //     return success;
        // }

        // public object MakeUnityMaterialFromCustom()
        // {
        //     foreach (var newMaterial in customMaterials)
        //     {

        //     }
        //     throw new NotImplementedException();
        // }

        // public Dictionary<materialProperty, double> FindValues(Material material)
        // {
        //     Dictionary<materialProperty, double> newValues = new Dictionary<materialProperty, double>();

        //     newValues.Add(materialProperty.diffuse, material.values.val_diffuse);
        //     newValues.Add(materialProperty.roughness, material.values.val_roughness);
        //     newValues.Add(materialProperty.specular, material.values.val_specular);
        //     newValues.Add(materialProperty.subsurface, material.values.val_subsurface);
        //     newValues.Add(materialProperty.metal, material.values.val_metal);
        //     newValues.Add(materialProperty.normal, material.values.val_normal);
        //     newValues.Add(materialProperty.alpha, material.values.val_alpha);


        //     return newValues;
        // }

        // public string FindShaderType(Material material)
        // {
        //     return material.type;
        // }

        // /// <summary>
        // /// Finds all texture maps for the provided material
        // /// </summary>
        // /// <param name="material"></param>
        // /// <returns></returns>
        // public Dictionary<materialProperty, Texture2D> FindTextures(Material material, DefaultAsset folder)
        // {
        //     Dictionary<materialProperty, Texture2D> Texturemaps = new Dictionary<materialProperty, Texture2D>();

        //     //Find All Textures
        //     Texture2D texture_diffuse = FindSingleTexture(material.maps.map_diffuse.texture, folder);
        //     Texture2D texture_alpha = FindSingleTexture(material.maps.map_alpha.texture, folder);
        //     Texture2D texture_roughness = FindSingleTexture(material.maps.map_roughness.texture, folder);
        //     Texture2D texture_metal = FindSingleTexture(material.maps.map_metal.texture, folder);
        //     Texture2D texture_normal = FindSingleTexture(material.maps.map_normal.texture, folder);
        //     Texture2D texture_subsurface = FindSingleTexture(material.maps.map_subsurface.texture, folder);
        //     Texture2D texture_specular = FindSingleTexture(material.maps.map_specular.texture, folder);

        //     //Get the maps used in the Material
        //     Texturemaps.Add(materialProperty.diffuse, texture_diffuse);
        //     Texturemaps.Add(materialProperty.specular, texture_specular);
        //     Texturemaps.Add(materialProperty.alpha, texture_alpha);
        //     Texturemaps.Add(materialProperty.metal, texture_metal);
        //     Texturemaps.Add(materialProperty.roughness, texture_roughness);
        //     Texturemaps.Add(materialProperty.normal, texture_normal);
        //     Texturemaps.Add(materialProperty.subsurface, texture_subsurface);

        //     return Texturemaps;
        // }

        public Texture2D FindSingleTexture(string textureToFind)
        {
            Texture2D newTexture = null;
            string texturesFolderPath = AssetDatabase.GetAssetPath(textureFolder);


            // var assets = AssetDatabase.FindAssets($"name:{textureToFind}, new String[] { texturesFolderPath });
            string assetpath = $"{texturesFolderPath}/{textureToFind}";
            newTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetpath);
            //Assign texture from path & GUID
            // foreach (var textureGUID in assets)
            // {
            //     var assetpath = AssetDatabase.GUIDToAssetPath(textureGUID);
            //     bool pathValid = assetpath.Contains(texturesFolderPath);
            //     bool textureValid = assetpath.Contains(textureToFind);
            //     bool textureEmpty = (textureToFind == string.Empty);

            //     if ((pathValid) && (textureValid) && (!textureEmpty))
            //     {
            //         newTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetpath);
            //         break;
            //     }
            // }
            return newTexture;
        }





    }
}