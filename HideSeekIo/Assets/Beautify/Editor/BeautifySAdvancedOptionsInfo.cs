using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;


namespace BeautifyEffect {
    public class BeautifySAdvancedOptionsInfo {

        public bool pendingChanges;
        public ShaderAdvancedOption[] options;

        public void ReadOptions() {
            pendingChanges = false;
            // Populate known options
            options = new ShaderAdvancedOption[] {
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_ORTHO", name = "Orthographic Mode", description = "Enables support for orthographic camera projection."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_ENABLE_CORE_EFFECT",
                    name = "Use Sharpen",
                    description = "Enables sharpen effect."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_DEPTH_BASED_SHARPEN",
                    name = "Depth Based Effects",
                    description = "Enables effects and options that depends on scene depth (depth-based sharpen, bloom layer mask, depth of field...) Disable to improve performance."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_ENABLE_DITHER",
                    name = "Use Dithering",
                    description = "Disabling dithering can improve performance on old mobile devices."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_DITHER_FINAL",
                    name = "Dither at the end",
                    description = "Applies dithering at the end of the stack (recommended). Disable to apply dithering at the start."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_ENABLE_COLOR_TWEAKS",
                    name = "Color Tweaks",
                    description = "Enables brightness, contrast and vibrance effects."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_EYE_ADAPTATION_DYNAMIC_RANGE",
                    name = "Use Dynamic Range Eye Adaptation",
                    description = "Disable to use legacy eye adaptation method (less accurate, provided for compatibility and always used in Best Performance mode)."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_SUN_FLARES_OCCLUSION_DEPTH",
                    name = "Use Scene Depth at Sun position to detect occlusion",
                    description = "Uses camera depth buffer to compute Sun occlusion instead of relying on raycasting."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_SUN_FLARES_OCCLUSION_CHROMA",
                    name = "Use Scene Color at Sun position to detect occlusion",
                    description = "Reduces Sun Flares intensity based on sky brightness."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_ACES_FITTED",
                    name = "Use Alternate ACES Tonemapping Operator",
                    description = "Uses an alternate algorithm that poduces less saturation on bright colors. ACES is only available in best quality mode."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_OUTLINE_SOBEL",
                    name = "Use Sobel Outline method",
                    description = "Uses a color-based edge detection algorithm instead of a depth based method. Useful for 2D projects."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_USE_PROCEDURAL_SEPIA",
                    name = "Use Procedural Sepia",
                    description = "Uses a formula based instead of Look-Up texture to produce Sepia effect."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_BETTER_FASTER_LUT",
                    name = "Better Fast LUT",
                    description = "Improves LUT quality in Best Performance Mode."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_EDGE_AA",
                    name = "AntiAliasing",
                    description = "Enables edge blur in final pass."
                },
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_HARD_LIGHT",
                    name = "Hard Light",
                    description = "Enables hard light effect."
                }
            };


            Shader shader = Shader.Find("Beautify/Beautify");
            if (shader != null) {
                string path = AssetDatabase.GetAssetPath(shader);
                string file = Path.GetDirectoryName(path) + "/BeautifyAdvancedParams.cginc";
                string[] lines = File.ReadAllLines(file, Encoding.UTF8);
                for (int k = 0; k < lines.Length; k++) {
                    for (int o = 0; o < options.Length; o++) {
                        if (lines[k].Contains(options[o].id)) {
                            options[o].enabled = !lines[k].StartsWith("//");
                        }
                    }
                }
            }
        }


        public bool GetAdvancedOptionState(string optionId) {
            if (options == null)
                return false;
            for (int k = 0; k < options.Length; k++) {
                if (options[k].id.Equals(optionId)) {
                    return options[k].enabled;
                }
            }
            return false;
        }

        public void UpdateAdvancedOptionsFile() {
            // Reloads the file and updates it accordingly
            Shader shader = Shader.Find("Beautify/Beautify");
            if (shader == null) return;

            string path = AssetDatabase.GetAssetPath(shader);
            string file = Path.GetDirectoryName(path) + "/BeautifyAdvancedParams.cginc";
            string[] lines = File.ReadAllLines(file, Encoding.UTF8);
            for (int k = 0; k < lines.Length; k++) {
                for (int o = 0; o < options.Length; o++) {
                    if (lines[k].Contains(options[o].id)) {
                        if (options[o].enabled) {
                            lines[k] = "#define " + options[o].id;
                        } else {
                            lines[k] = "//#define " + options[o].id;
                        }
                    }
                }
            }
            File.WriteAllLines(file, lines, Encoding.UTF8);


            SetShaderOptionValue("Beautify.cs", "USE_CAMERA_DEPTH_TEXTURE", GetAdvancedOptionState("BEAUTIFY_DEPTH_BASED_SHARPEN"));

            pendingChanges = false;
            AssetDatabase.Refresh();
        }


        void SetShaderOptionValue(string file, string option, bool state) {
            string[] res = Directory.GetFiles(Application.dataPath, file, SearchOption.AllDirectories);
            string path = null;
            for (int k = 0; k < res.Length; k++) {
                if (res[k].Contains("Beautify")) {
                    path = res[k];
                    break;
                }
            }
            if (path == null) {
                Debug.LogError(file + " could not be found!");
                return;
            }

            string[] code = File.ReadAllLines(path, System.Text.Encoding.UTF8);
            string searchToken = "#define " + option;
            for (int k = 0; k < code.Length; k++) {
                if (code[k].Contains(searchToken)) {
                    bool changed = false;
                    if (state) {
                        if (code[k].IndexOf("//#define") >= 0) {
                            code[k] = "#define " + option;
                            changed = true;
                        }
                    } else {
                        if (code[k].IndexOf("//#define") < 0) {
                            code[k] = "//#define " + option;
                            changed = true;
                        }
                    }
                    if (changed) {
                        File.WriteAllLines(path, code, Encoding.UTF8);
                    }
                    break;
                }
            }
        }


    }

    public struct ShaderAdvancedOption {
        public string id;
        public string name;
        public string description;
        public bool enabled;
        public bool unavailable;
    }


}