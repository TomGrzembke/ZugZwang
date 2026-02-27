using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using MyBox;
using UnityEditor;
using UnityEngine;

/// <summary> Generated Yaml reading for getting a default value in the subgraph. </summary>
public class SubshaderReader : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private Object subShader;
    [SerializeField] private string valueToSearch = "WorldBendStrength";
    [SerializeField] private string value;
    [SerializeField] private HeightBendParalax heightBendParalax;
    private string subShaderFilePath;

    [ButtonMethod]
    public void ApplyValue()
    {
        if (heightBendParalax == null)
        {
            Debugger.Log("HeightParalax is null");
            return;
        }
        

        if (subShader == null)
        {
            subShaderFilePath = "No Subshader selected";
            value = "";
            return;
        }

        subShaderFilePath = AssetDatabase.GetAssetPath(subShader);

        var mValue = GetSubgraphValue(subShaderFilePath);

        if (mValue.HasValue)
        {
            //Debug.Log($"Found {valueToSearch} m_Value: " + mValue.Value);
            value = mValue.Value.ToString();
        }
        else
        {
            Debug.LogWarning($"m_Value for {valueToSearch} not found.");
            value = "Not Found";
        }

        heightBendParalax.SetWorldBendStrength(float.Parse(value));
        EditorUtility.SetDirty(heightBendParalax.gameObject);
    }

    private float? GetSubgraphValue(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found: " + path);
            return null;
        }

        var content = File.ReadAllText(path);

        var pattern =
            $@"""m_Name"":\s*""{Regex.Escape(valueToSearch)}""[\s\S]*?""m_Value"":\s*(-?\d+\.?\d*(?:[eE][-+]?\d+)?(?:f)?)";


        var match = Regex.Match(content, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (match.Success)
        {
            var valueString = match.Groups[1].Value;

            if (float.TryParse(valueString, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var result))
                return result;

            Debug.LogWarning($"Could not parse '{valueString}' as a float for {valueToSearch}.");
            return null;
        }

        Debug.LogWarning($"Pattern '{pattern}' not found for {valueToSearch} in file: {path}");
        return null;
    }
#endif
}