using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smitty.MaterialImporter
{
    interface IMaterialImporter
    {
        void setJsonFile(TextAsset jsonFile);
        TextAsset getJsonFile();
        void ScheduleParsing();

    }
}
