using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildConfig", menuName = "Build/Build Config", order = 0)]
public class BuildConfig : ScriptableObject
{
    [Separator("Build Settings")] public bool shouldUploadToItch;

    [Separator("Itch Upload")] public string butlerTarget = "jannikklg/testenvironment:html";
}