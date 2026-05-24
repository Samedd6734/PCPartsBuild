using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PCPartsAPI.Services.Interfaces;

namespace PCPartsAPI.Services
{
    public class CompatibilityEngine : ICompatibilityEngine
    {
        public Task<CompatibilityResult> CheckCompatibilityAsync(
            string selectedComponentsJson,
            string currentStepName,
            object component)
        {
            var result = new CompatibilityResult();

            JsonDocument? selected;
            try
            {
                selected = JsonDocument.Parse(selectedComponentsJson);
            }
            catch
            {
                return Task.FromResult(result);
            }

            var root = selected.RootElement;
            var stepUpper = currentStepName.ToUpperInvariant();

            string GetSpec(string stepKey, string propName)
            {
                if (root.TryGetProperty(stepKey, out var stepObj) &&
                    stepObj.TryGetProperty(propName, out var val))
                {
                    if (val.ValueKind == JsonValueKind.String) return val.GetString() ?? "";
                    if (val.ValueKind == JsonValueKind.Number) return val.GetRawText();
                }
                return "";
            }

            int GetSpecInt(string stepKey, string propName)
            {
                if (root.TryGetProperty(stepKey, out var stepObj) &&
                    stepObj.TryGetProperty(propName, out var val))
                {
                    if (val.ValueKind == JsonValueKind.Number && val.TryGetInt32(out int n)) return n;
                    if (val.ValueKind == JsonValueKind.String && int.TryParse(val.GetString(), out int m)) return m;
                }
                return 0;
            }

            string GetProp(object obj, string name)
            {
                var prop = obj.GetType().GetProperty(name);
                return prop != null ? (prop.GetValue(obj)?.ToString() ?? "") : "";
            }

            int GetPropInt(object obj, string name)
            {
                var prop = obj.GetType().GetProperty(name);
                if (prop != null && int.TryParse(prop.GetValue(obj)?.ToString(), out int n))
                    return n;
                return 0;
            }

            string Normalize(string v) => v?.Trim().ToLowerInvariant() ?? "";

            bool IsMoboCompatibleWithCase(string moboSizeStr, string caseSupportedStr)
            {
                if (string.IsNullOrEmpty(moboSizeStr) || string.IsNullOrEmpty(caseSupportedStr))
                    return true;

                int moboSize = 0;
                string mLower = moboSizeStr.ToLowerInvariant();
                if (mLower.Contains("e-atx") || mLower.Contains("eatx") || mLower.Contains("extended")) moboSize = 4;
                else if (mLower.Contains("atx")) moboSize = 3;
                else if (mLower.Contains("micro") || mLower.Contains("matx") || mLower.Contains("m-atx")) moboSize = 2;
                else if (mLower.Contains("mini") || mLower.Contains("mitx") || mLower.Contains("m-itx") || mLower.Contains("itx")) moboSize = 1;

                int caseMax = 0;
                string cLower = caseSupportedStr.ToLowerInvariant();
                if (cLower.Contains("e-atx") || cLower.Contains("eatx") || cLower.Contains("extended")) caseMax = 4;
                else if (cLower.Contains("atx")) caseMax = 3;
                else if (cLower.Contains("micro") || cLower.Contains("matx") || cLower.Contains("m-atx")) caseMax = 2;
                else if (cLower.Contains("mini") || cLower.Contains("mitx") || cLower.Contains("m-itx") || cLower.Contains("itx")) caseMax = 1;

                if (moboSize > 0 && caseMax > 0) return moboSize <= caseMax;
                return cLower.Contains(mLower);
            }

            // CPU 
            if (stepUpper == "CPU")
            {
                var moboSocket = GetSpec("motherboard", "socketType");
                if (string.IsNullOrEmpty(moboSocket)) moboSocket = GetSpec("Motherboard", "socket"); // fallback
                if (!string.IsNullOrEmpty(moboSocket))
                {
                    var cpuSocket = GetProp(component, "SocketType");
                    if (Normalize(cpuSocket) != Normalize(moboSocket))
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Anakartın soketi ({moboSocket}) ile işlemci soketi ({cpuSocket}) uyumsuz.";
                        result.ExistingComponentName = $"Anakart ({moboSocket})";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({cpuSocket})";
                        return Task.FromResult(result);
                    }
                }
            }

            // MOTHERBOARD
            if (stepUpper == "MOTHERBOARD")
            {
                var cpuSocket = GetSpec("cpu", "socketType");
                if (string.IsNullOrEmpty(cpuSocket)) cpuSocket = GetSpec("CPU", "socket");
                if (!string.IsNullOrEmpty(cpuSocket))
                {
                    var moboSocket = GetProp(component, "SocketType");
                    if (Normalize(moboSocket) != Normalize(cpuSocket))
                    {
                        result.IsCompatible = false;
                        result.Reason = $"İşlemci soketi ({cpuSocket}) ile anakart soketi ({moboSocket}) uyumsuz.";
                        result.ExistingComponentName = $"CPU ({cpuSocket})";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({moboSocket})";
                        return Task.FromResult(result);
                    }
                }

                var ramMem = GetSpec("ram", "memoryType");
                if (string.IsNullOrEmpty(ramMem)) ramMem = GetSpec("RAM", "memoryType");
                if (!string.IsNullOrEmpty(ramMem))
                {
                    var moboMem = GetProp(component, "MemoryType");
                    if (Normalize(moboMem) != Normalize(ramMem))
                    {
                        result.IsCompatible = false;
                        result.Reason = $"RAM bellek tipi ({ramMem}) ile anakartın desteklediği bellek tipi ({moboMem}) uyumsuz.";
                        result.ExistingComponentName = $"RAM ({ramMem})";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({moboMem})";
                        return Task.FromResult(result);
                    }
                }

                var caseForm = GetSpec("case", "supportedMotherboardFormFactors");
                if (!string.IsNullOrEmpty(caseForm))
                {
                    var moboForm = GetProp(component, "FormFactor");
                    if (!string.IsNullOrEmpty(moboForm) && !IsMoboCompatibleWithCase(moboForm, caseForm))
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Kasa ({caseForm}) bu anakartın boyutunu ({moboForm}) desteklemiyor.";
                        result.ExistingComponentName = $"Kasa ({caseForm})";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({moboForm})";
                        return Task.FromResult(result);
                    }
                }
            }

            // RAM
            if (stepUpper == "RAM")
            {
                var moboMemType = GetSpec("motherboard", "memoryType");
                if (string.IsNullOrEmpty(moboMemType)) moboMemType = GetSpec("Motherboard", "memoryType");
                if (!string.IsNullOrEmpty(moboMemType))
                {
                    var ramMemType = GetProp(component, "MemoryType");
                    if (Normalize(ramMemType) != Normalize(moboMemType))
                    {
                        result.IsCompatible = false;
                        result.Reason = $"RAM tipi ({ramMemType}) ile anakartın desteklediği bellek tipi ({moboMemType}) uyumsuz.";
                        result.ExistingComponentName = $"Anakart ({moboMemType})";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({ramMemType})";
                        return Task.FromResult(result);
                    }
                }

                var moboSlots = GetSpecInt("motherboard", "memorySlotCount");
                if (moboSlots == 0) moboSlots = GetSpecInt("Motherboard", "memorySlots");
                if (moboSlots > 0)
                {
                    var moduleConfig = GetProp(component, "ModuleConfig");
                    int ramModuleCount = 0;
                    if (!string.IsNullOrEmpty(moduleConfig))
                    {
                        var xIndex = moduleConfig.IndexOf('x', StringComparison.OrdinalIgnoreCase);
                        if (xIndex > 0 && int.TryParse(moduleConfig.Substring(0, xIndex).Trim(), out int mc))
                            ramModuleCount = mc;
                    }

                    if (ramModuleCount > moboSlots)
                    {
                        result.IsCompatible = false;
                        result.Reason = $"RAM kit'i {ramModuleCount} modül içeriyor ama anakartında sadece {moboSlots} RAM slotu var.";
                        result.ExistingComponentName = $"Anakart ({moboSlots} slot)";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({ramModuleCount} modül)";
                        return Task.FromResult(result);
                    }
                }
            }

            // GPU
            if (stepUpper == "GPU")
            {
                var maxGpuLength = GetSpecInt("case", "maxGPULengthMm");
                if (maxGpuLength > 0)
                {
                    var gpuLength = GetPropInt(component, "LengthMm");
                    if (gpuLength > 0 && gpuLength > maxGpuLength)
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Ekran kartın ({gpuLength}mm) kasaya sığmıyor (Maks: {maxGpuLength}mm).";
                        result.ExistingComponentName = $"Kasa (Maks {maxGpuLength}mm)";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({gpuLength}mm)";
                        return Task.FromResult(result);
                    }
                }

                var psuWatt = GetSpecInt("psu", "wattageW");
                if (psuWatt > 0)
                {
                    var cpuTdp = GetSpecInt("cpu", "tdp");
                    var gpuTdp = GetPropInt(component, "TDPWatt");
                    var requiredWattage = (int)Math.Ceiling((cpuTdp + gpuTdp) * 1.25);
                    if (psuWatt < requiredWattage)
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Sistemin tahmini güç tüketimi için seçtiğin PSU ({psuWatt}W) yetersiz. En az {requiredWattage}W gerekiyor.";
                        result.ExistingComponentName = $"PSU ({psuWatt}W)";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({gpuTdp}W TDP)";
                        return Task.FromResult(result);
                    }
                }
            }

            // STORAGE
            if (stepUpper == "STORAGE")
            {
                var formFactor = GetProp(component, "FormFactor");
                if (Normalize(formFactor).Contains("m.2"))
                {
                    var moboM2Slots = GetSpecInt("motherboard", "m2SlotCount");
                    if (moboM2Slots == 0) moboM2Slots = GetSpecInt("Motherboard", "m2SlotCount"); // legacy fallback
                    if (moboM2Slots <= 0 && GetSpec("motherboard", "id") != "") // ID exists meaning mobo is selected
                    {
                        result.IsCompatible = false;
                        result.Reason = "Seçtiğin SSD M.2 formunda ama anakartında M.2 slotu bulunmuyor.";
                        result.ExistingComponentName = "Anakart (M.2 slot yok)";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")}";
                        return Task.FromResult(result);
                    }
                }
            }

            // CASE
            if (stepUpper == "CASE")
            {
                var moboFormFactor = GetSpec("motherboard", "formFactor");
                if (string.IsNullOrEmpty(moboFormFactor)) moboFormFactor = GetSpec("Motherboard", "formFactor");
                if (!string.IsNullOrEmpty(moboFormFactor))
                {
                    var supported = GetProp(component, "SupportedMotherboardFormFactors");
                    if (!string.IsNullOrEmpty(supported) && !IsMoboCompatibleWithCase(moboFormFactor, supported))
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Kasa \"{moboFormFactor}\" anakart form faktörünü desteklemiyor.";
                        result.ExistingComponentName = $"Anakart ({moboFormFactor})";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")}";
                        return Task.FromResult(result);
                    }
                }

                var gpuLength = GetSpecInt("gpu", "lengthMm");
                if (gpuLength == 0) gpuLength = GetSpecInt("GPU", "length");
                if (gpuLength > 0)
                {
                    var maxGpu = GetPropInt(component, "MaxGPULengthMm");
                    if (maxGpu > 0 && gpuLength > maxGpu)
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Ekran kartın ({gpuLength}mm) bu kasaya sığmıyor (Maks: {maxGpu}mm).";
                        result.ExistingComponentName = $"GPU ({gpuLength}mm)";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")}";
                        return Task.FromResult(result);
                    }
                }

                var coolerHeight = GetSpecInt("cpuCooler", "heightMm");
                if (coolerHeight > 0)
                {
                    var maxCoolerHeight = GetPropInt(component, "MaxCPUCoolerHeightMm");
                    if (maxCoolerHeight > 0 && coolerHeight > maxCoolerHeight)
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Soğutucu yüksekliği ({coolerHeight}mm) kasanın izin verdiği sınırı aşıyor (Maks: {maxCoolerHeight}mm).";
                        result.ExistingComponentName = $"Soğutucu ({coolerHeight}mm)";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")}";
                        return Task.FromResult(result);
                    }
                }

                var coolerRad = GetSpecInt("cpuCooler", "radiatorSizeMm");
                if (coolerRad > 0)
                {
                    var frontRad = GetPropInt(component, "FrontRadiatorSupportMm");
                    var topRad = GetPropInt(component, "TopRadiatorSupportMm");
                    if (frontRad > 0 || topRad > 0)
                    {
                        if (coolerRad > frontRad && coolerRad > topRad)
                        {
                            result.IsCompatible = false;
                            result.Reason = $"Kasanın radyatör desteği (Ön: {frontRad}mm, Üst: {topRad}mm) soğutucunun {coolerRad}mm radyatörünü desteklemiyor.";
                            result.ExistingComponentName = $"Soğutucu ({coolerRad}mm)";
                            result.AttemptedComponentName = $"{GetProp(component, "ProductName")}";
                            return Task.FromResult(result);
                        }
                    }
                }
            }

            // PSU
            if (stepUpper == "PSU")
            {
                var cpuTdp = GetSpecInt("cpu", "tdp");
                if (cpuTdp == 0) cpuTdp = GetSpecInt("CPU", "tdp");
                
                var gpuTdp = GetSpecInt("gpu", "tdpWatt");
                if (gpuTdp == 0) gpuTdp = GetSpecInt("GPU", "tdp");
                
                var gpuRecPsu = GetSpecInt("gpu", "recommendedPSUW");
                if (gpuRecPsu == 0) gpuRecPsu = GetSpecInt("GPU", "RecommendedPSUW");
                
                var totalTdp = cpuTdp + gpuTdp;
                var psuWattage = GetPropInt(component, "WattageW");

                var requiredWattage = gpuRecPsu > 0 ? gpuRecPsu : (int)Math.Ceiling(totalTdp * 1.25);
                if (psuWattage > 0 && psuWattage < requiredWattage)
                {
                    result.IsCompatible = false;
                    result.Reason = $"Sistemin güç tüketimi için güvenli payla birlikte en az {requiredWattage}W gerekiyor ama seçtiğin PSU {psuWattage}W.";
                    result.ExistingComponentName = $"Sistem Tahmini ({requiredWattage}W Gerekli)";
                    result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({psuWattage}W)";
                    return Task.FromResult(result);
                }
            }

            // CPUCOOLER
            if (stepUpper == "CPUCOOLER")
            {
                var cpuSocket = GetSpec("cpu", "socketType");
                if (string.IsNullOrEmpty(cpuSocket)) cpuSocket = GetSpec("CPU", "socket");
                if (!string.IsNullOrEmpty(cpuSocket))
                {
                    var supportedSockets = GetProp(component, "SupportedSockets");
                    if (!string.IsNullOrEmpty(supportedSockets) && !supportedSockets.ToLowerInvariant().Contains(Normalize(cpuSocket)))
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Soğutucu \"{cpuSocket}\" soketini desteklemiyor.";
                        result.ExistingComponentName = $"CPU ({cpuSocket})";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")}";
                        return Task.FromResult(result);
                    }
                }

                var cpuTdp = GetSpecInt("cpu", "tdp");
                if (cpuTdp == 0) cpuTdp = GetSpecInt("CPU", "tdp");
                if (cpuTdp > 0)
                {
                    var coolerTdp = GetPropInt(component, "TDPCapacityW");
                    if (coolerTdp > 0 && coolerTdp < cpuTdp)
                    {
                        result.IsCompatible = false;
                        result.Reason = $"İşlemcinin TDP'si {cpuTdp}W ama soğutucunun kapasitesi sadece {coolerTdp}W.";
                        result.ExistingComponentName = $"CPU ({cpuTdp}W TDP)";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({coolerTdp}W)";
                        return Task.FromResult(result);
                    }
                }

                var coolerType = GetProp(component, "CoolerType");
                var maxCoolerHeight = GetSpecInt("case", "maxCPUCoolerHeightMm");
                if (maxCoolerHeight == 0) maxCoolerHeight = GetSpecInt("Case", "maxCpuCoolerHeight");
                
                if (Normalize(coolerType) == "hava soğutucu" || Normalize(coolerType) == "air")
                {
                    var coolerHeight = GetPropInt(component, "HeightMm");
                    if (maxCoolerHeight > 0 && coolerHeight > 0 && coolerHeight > maxCoolerHeight)
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Soğutucu yüksekliği ({coolerHeight}mm) kasanın izin verdiği sınırı aşıyor (Maks: {maxCoolerHeight}mm).";
                        result.ExistingComponentName = $"Kasa (Maks {maxCoolerHeight}mm)";
                        result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({coolerHeight}mm)";
                        return Task.FromResult(result);
                    }
                }
                
                if (Normalize(coolerType) == "sıvı soğutucu" || Normalize(coolerType) == "liquid")
                {
                    var radSize = GetPropInt(component, "RadiatorSizeMm");
                    if (radSize > 0)
                    {
                        var caseFrontRad = GetSpecInt("case", "frontRadiatorSupportMm");
                        if (caseFrontRad == 0) caseFrontRad = GetSpecInt("Case", "frontRadiatorSupport");
                        
                        var caseTopRad = GetSpecInt("case", "topRadiatorSupportMm");
                        if (caseTopRad == 0) caseTopRad = GetSpecInt("Case", "topRadiatorSupport");

                        if (caseFrontRad > 0 || caseTopRad > 0)
                        {
                            if (radSize > caseFrontRad && radSize > caseTopRad)
                            {
                                result.IsCompatible = false;
                                result.Reason = $"Kasanın radyatör desteği (Ön: {caseFrontRad}mm, Üst: {caseTopRad}mm) {radSize}mm radyatörü desteklemiyor.";
                                result.ExistingComponentName = $"Kasa radyatör desteği";
                                result.AttemptedComponentName = $"{GetProp(component, "ProductName")} ({radSize}mm)";
                                return Task.FromResult(result);
                            }
                        }
                    }
                }
            }

            selected.Dispose();
            return Task.FromResult(result);
        }
    }
}
