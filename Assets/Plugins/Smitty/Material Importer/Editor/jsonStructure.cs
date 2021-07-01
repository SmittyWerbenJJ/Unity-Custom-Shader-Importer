using System.Collections.Generic;

namespace Smitty.MaterialImporter
{


    [System.Serializable]
    public class DecompiledJson
    {
        public List<string> objects;
        public List<Material> materials;
        public List<Link> links;
    }


    [System.Serializable]
    public class MapDiffuse
    {
        public string texture;
        public bool isAlpha;
    }
    [System.Serializable]
    public class MapAlpha
    {
        public string texture;
        public bool isAlpha;
    }
    [System.Serializable]
    public class MapRoughness
    {
        public string texture;
        public bool isAlpha;
    }
    [System.Serializable]
    public class MapSpecular
    {
        public string texture;
        public bool isAlpha;
    }
    [System.Serializable]
    public class MapMetal
    {
        public string texture;
        public bool isAlpha;
    }
    [System.Serializable]
    public class MapNormal
    {
        public string texture;
        public bool isAlpha;
    }
    [System.Serializable]
    public class MapSubsurface
    {
        public string texture;
        public bool isAlpha;
    }

    [System.Serializable]
    public class Maps
    {
        public MapDiffuse map_diffuse;
        public MapAlpha map_alpha;
        public MapRoughness map_roughness;
        public MapSpecular map_specular;
        public MapMetal map_metal;
        public MapNormal map_normal;
        public MapSubsurface map_subsurface;

        public List<string> getNames()
        {
            List<string> tmpList = new List<string>();
            tmpList.Add(this.map_diffuse.texture);
            tmpList.Add(this.map_alpha.texture);
            tmpList.Add(this.map_metal.texture);
            tmpList.Add(this.map_normal.texture);
            tmpList.Add(this.map_roughness.texture);
            tmpList.Add(this.map_specular.texture);
            tmpList.Add(this.map_subsurface.texture);

            var tmplistReduced = tmpList;
            foreach (var item in tmpList)
                if (item == null)
                    tmplistReduced.Remove(item);

            return tmplistReduced;
        }

        public Dictionary<textureTypes, string> getAllMaps()
        {
            var dict = new Dictionary<textureTypes, string>();
            dict.Add(textureTypes.diffuse, map_diffuse.texture);
            dict.Add(textureTypes.alpha, map_alpha.texture);
            dict.Add(textureTypes.specular, map_specular.texture);
            dict.Add(textureTypes.subsurface, map_subsurface.texture);
            dict.Add(textureTypes.roughness, map_roughness.texture);
            dict.Add(textureTypes.normal, map_normal.texture);
            dict.Add(textureTypes.metal, map_metal.texture);

            return dict;
        }
    }

    [System.Serializable]
    public class Values
    {
        public int val_diffuse;
        public double val_alpha;
        public double val_roughness;
        public int val_specular;
        public double val_metal;
        public int val_normal;
        public double val_subsurface;
    }

    [System.Serializable]
    public class Material
    {
        public string name;
        public string type;
        public Maps maps;
        public Values values;
    }

    [System.Serializable]
    public class Link
    {
        public string Object;
        public int materialSlot;
        public string material;
    }
}