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
        TextAsset uiYaml = (TextAsset) asset;
        string uiYamlText = uiYaml.text;
        
        // Parse the YAML
        using StringReader reader = new StringReader(uiYamlText);
        YamlStream yaml = new YamlStream();
        yaml.Load(reader);

        // Get the root node
        YamlNode rootNode = yaml.Documents[0].RootNode;
        
        AddCatalystBranding(rootNode);
        AddCatalystCredits(rootNode);
        
        // Write the YAML back to a string
        using StringWriter writer = new StringWriter();
        yaml.Save(writer);
        return new TextAsset(writer.ToString());
    }

    private void AddCatalystCredits(YamlNode root)
    {
        // Add catalyst credits branch
        YamlMappingNode creditsBranch = new YamlMappingNode();
        creditsBranch.Add("name", "credits_catalyst");

        YamlMappingNode settings = new YamlMappingNode();
        settings.Add("type", "menu");
        settings.Add("clear_screen", "\"true\"");
        creditsBranch.Add("settings", settings);
        
        YamlSequenceNode elements = new YamlSequenceNode();
        elements.Add(new YamlScalarNode(""));
        elements.Add(new YamlScalarNode("<size=150%><b>Credits</b> : Catalyst"));
        elements.Add(new YamlScalarNode("[[alignment:center]]{{bar}}"));
        elements.Add(new YamlScalarNode(""));
        elements.Add(new YamlScalarNode("This project was made possible by you, the {{col:#F05355:Project Arrhythmia}} community."));
        elements.Add(new YamlScalarNode("Thank you for your support! <3"));
        elements.Add(new YamlScalarNode(""));
        elements.Add(new YamlScalarNode("[[alignment:right|font-style:bold]]- Reimnop"));
        elements.Add(new YamlScalarNode(""));
        elements.Add(new YamlScalarNode("<b>Special thanks</b>"));
        elements.Add(new YamlScalarNode(" enchart"));
        elements.Add(new YamlScalarNode(" Crimson Crips"));
        elements.Add(new YamlScalarNode("[[loop:4]]"));
        
        YamlMappingNode buttons = new YamlMappingNode();
        YamlSequenceNode buttonsSettings = new YamlSequenceNode();
        buttonsSettings.Add(new YamlScalarNode("width:0.2"));
        buttonsSettings.Add(new YamlScalarNode("orientation:horizontal"));
        buttonsSettings.Add(new YamlScalarNode("alignment:center"));
        buttons.Add("settings", buttonsSettings);
        buttons.Add("buttons", "RETURN:credits_menu");
        elements.Add(buttons);
        
        elements.Add("[[alignment:center]]{{bar}}");
        elements.Add("[[alignment:right]]{{col:#F05355:Project Arrhythmia}} Unified Operating System | Version {{versionNumber}}");
        elements.Add(new YamlScalarNode("[[alignment:right]]Powered by {{col:#F05355:Catalyst}} | Version " + CatalystBase.Version));
        
        creditsBranch.Add("elements", elements);
        
        YamlSequenceNode branches = (YamlSequenceNode) root["branches"];
        branches.Add(creditsBranch);
        
        // Add catalyst credits button
        YamlMappingNode creditsMenu = (YamlMappingNode) branches.First(node => (string) node["name"] == "credits_menu");
        YamlSequenceNode creditsMenuElements = (YamlSequenceNode) creditsMenu["elements"];
        YamlMappingNode creditsButtons = (YamlMappingNode) creditsMenuElements[3];
        
        YamlScalarNode descriptionNode = (YamlScalarNode) creditsButtons["settings"][1];
        descriptionNode.Value += "::<alpha=#AA>The people behind Catalyst mod";
        
        YamlScalarNode creditsButtonsNode = (YamlScalarNode) creditsButtons["buttons"];
        creditsButtonsNode.Value += "&& CATALYST:credits_catalyst";
        
        YamlScalarNode loopNode = (YamlScalarNode) creditsMenuElements[5];
        loopNode.Value = "[[loop:6]]";
    }

    private void AddCatalystBranding(YamlNode root)
    {
        HashSet<string> whatToPatch = new HashSet<string>()
        {
            "main_menu",
            "arcade_menu",
            "story_mode_selection",
            "confirm_new_game",
            "credits_thanks",
            "credits_music",
            "credits_dev",
            "settings_menu",
            "audio_menu",
            "gameplay_menu",
            "video_menu",
            "credits_menu",
            "credits_comingsoon",
            "play_adofai", // this exists in the game files but is never used..?
            "adofaisettings" // this too ..?
        };
        
        YamlSequenceNode branches = (YamlSequenceNode) root["branches"];
        foreach (YamlNode node in branches)
        {
            string name = (string) node["name"];
            if (whatToPatch.Contains(name))
            {
                CatalystBase.LogInfo($"Patching main menu UI element [{name}]");
                
                YamlSequenceNode elements = (YamlSequenceNode) node["elements"];
                elements.Add(new YamlScalarNode("[[alignment:right]]Powered by {{col:#F05355:Catalyst}} | Version " + CatalystBase.Version));
            }
        }
    }
}