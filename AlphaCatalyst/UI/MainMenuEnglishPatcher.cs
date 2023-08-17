using System.Collections.Generic;
using System.IO;
using System.Linq;
using Catalyst.Patch;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace Catalyst.UI;

public class MainMenuEnglishPatcher : ResourcePatcher
{
    public override Object Patch(Object asset)
    {
        TextAsset uiYaml = asset.Cast<TextAsset>();
        string uiYamlText = uiYaml.text;
        
        // Parse the YAML
        StringReader reader = new StringReader(uiYamlText);
        YamlStream yaml = new YamlStream();
        yaml.Load(reader);

        // Get the root node
        YamlNode rootNode = yaml.Documents[0].RootNode;
        
        AddCatalystBranding(rootNode);
        AddCatalystCredits(rootNode);
        
        // Write the YAML back to a string
        StringWriter writer = new StringWriter();
        yaml.Save(writer);
        
        return new TextAsset(writer.ToString());
    }

    private void AddCatalystCredits(YamlNode root)
    {
        YamlSequenceNode branches = (YamlSequenceNode) root["branches"];
        
        // Add catalyst github branch
        YamlMappingNode githubBranch = new YamlMappingNode();
        githubBranch.Add("name", "catalyst_github");
        
        YamlMappingNode githubSettings = new YamlMappingNode();
        githubSettings.Add("type", "normal");
        githubSettings.Add("clear_screen", "\"true\"");
        githubBranch.Add("settings", githubSettings);
        
        YamlSequenceNode githubElements = new YamlSequenceNode();
        
        YamlSequenceNode githubPreSequence = new YamlSequenceNode();
        githubPreSequence.Add(new YamlScalarNode("media::despawnall"));
        githubPreSequence.Add(new YamlScalarNode("openlink::https://github.com/Reimnop/Catalyst"));
        githubElements.Add(githubPreSequence);

        githubElements.Add(new YamlScalarNode(""));
        githubElements.Add(new YamlScalarNode("Check your browser for the Catalyst GitHub repository!"));
        
        YamlSequenceNode githubPostSequence = new YamlSequenceNode();
        githubPostSequence.Add(new YamlScalarNode("wait::2"));
        githubPostSequence.Add(new YamlScalarNode("branch::credits_catalyst"));
        githubElements.Add(githubPostSequence);
        
        githubBranch.Add("elements", githubElements);
        
        branches.Add(githubBranch);

        // Add catalyst credits branch
        YamlMappingNode creditsBranch = new YamlMappingNode();
        creditsBranch.Add("name", "credits_catalyst");

        YamlMappingNode creditsSettings = new YamlMappingNode();
        creditsSettings.Add("type", "menu");
        creditsSettings.Add("clear_screen", "true");
        creditsBranch.Add("settings", creditsSettings);
        
        YamlSequenceNode creditsElements = new YamlSequenceNode();
        creditsElements.Add(new YamlScalarNode(""));
        creditsElements.Add(new YamlScalarNode("<size=150%><b>Credits</b> - Catalyst"));
        creditsElements.Add(new YamlScalarNode("[[alignment:center]]{{bar}}"));
        creditsElements.Add(new YamlScalarNode(""));
        creditsElements.Add(new YamlScalarNode("This project was made possible by you, the {{col:#F05355:Project Arrhythmia}} community."));
        creditsElements.Add(new YamlScalarNode("Thank you for your support! <3"));
        creditsElements.Add(new YamlScalarNode(""));
        creditsElements.Add(new YamlScalarNode("- Reimnop"));
        creditsElements.Add(new YamlScalarNode(""));
        creditsElements.Add(new YamlScalarNode("<mark=#{{ui_theme_border_highlight}}><color=#{{ui_theme_bg}}><b>_Cool People_</b></color></mark>"));
        creditsElements.Add(new YamlScalarNode("Miv2nir | Eldar | JoshyTM123 | skalt771 | Rainstar | Windows 98"));
        creditsElements.Add(new YamlScalarNode("<mark=#{{ui_theme_border_highlight}}><color=#{{ui_theme_bg}}><b>_Special Thanks_</b></color></mark>"));
        creditsElements.Add(new YamlScalarNode("enchart | Crimson Crips | Xenon1345 | GuonuoTW | Pidge (For this amazing game!)"));
        creditsElements.Add(new YamlScalarNode("[[loop:2]]"));
        
        YamlMappingNode buttons = new YamlMappingNode();
        YamlSequenceNode buttonsSettings = new YamlSequenceNode();
        buttonsSettings.Add(new YamlScalarNode("width:0.35"));
        buttons.Add("settings", buttonsSettings);
        buttons.Add("buttons", " RETURN:credits_menu&& GITHUB ↗:catalyst_github");
        creditsElements.Add(buttons);
        
        creditsElements.Add("[[alignment:center]]{{bar}}");
        creditsElements.Add("[[alignment:right]]{{col:#F05355:Project Arrhythmia}} Unified Operating System | Version {{versionNumber}}");
        creditsElements.Add(new YamlScalarNode("[[alignment:right]]Powered by {{col:#F05355:Catalyst}} | Version " + CatalystBase.Version));
        
        creditsBranch.Add("elements", creditsElements);
        
        branches.Add(creditsBranch);
        
        // Add catalyst credits button
        YamlMappingNode creditsMenu = (YamlMappingNode) branches.First(node => (string) node["name"] == "credits_menu");
        YamlSequenceNode creditsMenuElements = (YamlSequenceNode) creditsMenu["elements"];
        YamlMappingNode creditsButtons = (YamlMappingNode) creditsMenuElements[4];
        
        YamlScalarNode descriptionNode = (YamlScalarNode) creditsButtons["settings"][1];
        descriptionNode.Value = descriptionNode.Value.Substring(0, descriptionNode.Value.Length - 33); // Truncate 33 characters
        descriptionNode.Value += "::<alpha=#AA>The people behind Catalyst mod";
        
        YamlScalarNode creditsButtonsNode = (YamlScalarNode) creditsButtons["buttons"];
        creditsButtonsNode.Value += "&& CATALYST:credits_catalyst";
        
        YamlScalarNode loopNode = (YamlScalarNode) creditsMenuElements[6];
        loopNode.Value = "[[loop:4]]";
    }

    private void AddCatalystBranding(YamlNode root)
    {
        HashSet<string> whatToPatch = new HashSet<string>()
        {
            "main_menu",
            "arcade_menu",
            "story_mode_selection",
            "confirm_new_game",
            "settings_menu",
            "audio_menu",
            "gameplay_menu",
            "video_menu",
            "accessibility_menu",
            "credits_menu",
            "credits_thanks",
            "credits_music",
            "credits_art",
            "credits_translators",
            "credits_dev",
            "credits_publisher"
        };
        
        YamlSequenceNode branches = (YamlSequenceNode) root["branches"];
        foreach (YamlNode node in branches)
        {
            string name = (string) node["name"];
            if (whatToPatch.Contains(name))
            {
                CatalystBase.LogInfo($"Patching main menu UI element [{name}]");
                
                YamlSequenceNode elements = (YamlSequenceNode) node["elements"];
                elements.Add(new YamlScalarNode($"[[alignment:right]]Powered by {{col:#F05355:Catalyst}} | Version {CatalystBase.Version}"));
            }
        }
    }
}