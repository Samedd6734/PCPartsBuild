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

            // JSON'dan yardımcı spec okuma fonksiyonu
            string GetSpec(string stepKey, string propName)
            {
                if (root.TryGetProperty(stepKey, out var stepObj) &&
                    stepObj.TryGetProperty(propName, out var val))
                    return val.GetString() ?? "";
                return "";
            }

            int GetSpecInt(string stepKey, string propName)
            {
                if (root.TryGetProperty(stepKey, out var stepObj) &&
                    stepObj.TryGetProperty(propName, out var val) &&
                    val.TryGetInt32(out int n))
                    return n;
                return 0;
            }

            // Dynamic property okuma
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

            string Normalize(string v) => v.Trim().ToLowerInvariant();

            switch (stepUpper)
            {
                // MOTHERBOARD seçilirken CPU kontrolü
                case "MOTHERBOARD":
                {
                    var cpuSocket = GetSpec("CPU", "socket");
                    if (!string.IsNullOrEmpty(cpuSocket))
                    {
                        var moboSocket = GetProp(component, "Socket");
                        if (Normalize(moboSocket) != Normalize(cpuSocket))
                        {
                            result.IsCompatible = false;
                            result.Reason = $"İşlemci soketi ({cpuSocket}) ile anakart soketi ({moboSocket}) uyumsuz. Sol menüden \"{cpuSocket}\" soketini filtrelemelisin.";
                            result.ExistingComponentName = $"CPU ({cpuSocket})";
                            result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")}";
                        }
                    }
                    break;
                }

                // RAM seçilirken Motherboard kontrolü
                case "RAM":
                {
                    var moboMemType = GetSpec("Motherboard", "memoryType");
                    if (!string.IsNullOrEmpty(moboMemType))
                    {
                        var ramMemType = GetProp(component, "MemoryType");
                        if (Normalize(ramMemType) != Normalize(moboMemType))
                        {
                            result.IsCompatible = false;
                            result.Reason = $"RAM tipi ({ramMemType}) ile anakartın desteklediği bellek tipi ({moboMemType}) uyumsuz. Filtrelerden \"{moboMemType}\" seçmelisin.";
                            result.ExistingComponentName = $"Anakart ({moboMemType})";
                            result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")}";
                        }
                    }

                    // RAM slot sayısı kontrolü
                    var moboSlots = GetSpecInt("Motherboard", "memorySlots");
                    if (moboSlots > 0)
                    {
                        var ramModuleCount = GetPropInt(component, "ModuleCount");
                        if (ramModuleCount > moboSlots)
                        {
                            result.IsCompatible = false;
                            result.Reason = $"RAM kit'i {ramModuleCount} modül içeriyor ama anakartında sadece {moboSlots} RAM slotu var.";
                            result.ExistingComponentName = $"Anakart ({moboSlots} slot)";
                            result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")}";
                        }
                    }
                    break;
                }

                // GPU seçilirken — şu an zorunlu fiziksel kontrol yok (kasa henüz seçilmedi)
                // Case seçiminde GPU uyumu kontrol edilecek
                case "GPU":
                    break;

                // STORAGE seçimi — anakart M.2 slot kontrolü
                case "STORAGE":
                {
                    var formFactor = GetProp(component, "FormFactor");
                    if (Normalize(formFactor) == "m.2")
                    {
                        var moboM2Slots = GetSpecInt("Motherboard", "m2SlotCount");
                        if (moboM2Slots <= 0)
                        {
                            result.IsCompatible = false;
                            result.Reason = "Seçtiğin SSD M.2 formunda ama anakartında M.2 slotu yok. 2.5 inç SSD veya HDD filtrelemelisin.";
                            result.ExistingComponentName = "Anakart (M.2 slot yok)";
                            result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")}";
                        }
                    }
                    break;
                }

                // CASE seçilirken Motherboard formfactor + GPU uzunluk kontrolü
                case "CASE":
                {
                    // Anakart form factor
                    var moboFormFactor = GetSpec("Motherboard", "formFactor");
                    if (!string.IsNullOrEmpty(moboFormFactor))
                    {
                        var supported = GetProp(component, "SupportedMotherboards");
                        if (!string.IsNullOrEmpty(supported) &&
                            !supported.ToLowerInvariant().Contains(Normalize(moboFormFactor)))
                        {
                            result.IsCompatible = false;
                            result.Reason = $"Kasa \"{moboFormFactor}\" anakart form faktörünü desteklemiyor. Desteklenen: {supported}. Filtreleri kontrol et.";
                            result.ExistingComponentName = $"Anakart ({moboFormFactor})";
                            result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")}";
                            break;
                        }
                    }

                    // GPU uzunluk kontrolü
                    var gpuLength = GetSpecInt("GPU", "length");
                    if (gpuLength > 0)
                    {
                        var maxGpu = GetPropInt(component, "MaxGpuLength");
                        if (maxGpu > 0 && gpuLength > maxGpu)
                        {
                            result.IsCompatible = false;
                            result.Reason = $"Ekran kartın ({gpuLength}mm) bu kasaya sığmıyor (Maks: {maxGpu}mm). Daha geniş bir kasa seçmelisin.";
                            result.ExistingComponentName = $"GPU ({gpuLength}mm)";
                            result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")}";
                        }
                    }
                    break;
                }

                // PSU seçilirken toplam TDP + GPU güç konnektör kontrolü
                case "PSU":
                {
                    var cpuTdp = GetSpecInt("CPU", "tdp");
                    var gpuTdp = GetSpecInt("GPU", "tdp");
                    var totalTdp = cpuTdp + gpuTdp;
                    var psuWattage = GetPropInt(component, "Wattage");

                    // %25 güvenlik payı ile hesapla
                    var requiredWattage = (int)Math.Ceiling(totalTdp * 1.25);
                    if (psuWattage > 0 && psuWattage < requiredWattage)
                    {
                        result.IsCompatible = false;
                        result.Reason = $"Sistemin toplam TDP'si {totalTdp}W. Güvenli çalışma için en az {requiredWattage}W PSU lazım ama seçtiğin PSU {psuWattage}W. Daha güçlü bir PSU filtrele.";
                        result.ExistingComponentName = $"Sistem TDP ({totalTdp}W)";
                        result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")} ({psuWattage}W)";
                    }

                    // GPU güç konnektörü kontrolü
                    if (result.IsCompatible)
                    {
                        var gpuPowerConnectors = GetSpec("GPU", "powerConnectors");
                        if (!string.IsNullOrEmpty(gpuPowerConnectors) && gpuPowerConnectors.Contains("16pin"))
                        {
                            var has12v = GetProp(component, "Has12VHPWR");
                            if (Normalize(has12v) == "false")
                            {
                                result.IsCompatible = false;
                                result.Reason = "Ekran kartın 12VHPWR (16 pin) konnektör gerektiriyor ama seçtiğin PSU'da bu kablo yok. 12VHPWR destekli PSU filtrele.";
                                result.ExistingComponentName = "GPU (12VHPWR gerekli)";
                                result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")}";
                            }
                        }
                    }
                    break;
                }

                // COOLER seçilirken CPU soket + TDP + Kasa yükseklik kontrolü
                case "COOLER":
                {
                    // Soket uyumu
                    var cpuSocket = GetSpec("CPU", "socket");
                    if (!string.IsNullOrEmpty(cpuSocket))
                    {
                        var supportedSockets = GetProp(component, "SupportedSockets");
                        if (!string.IsNullOrEmpty(supportedSockets) &&
                            !supportedSockets.ToLowerInvariant().Contains(Normalize(cpuSocket)))
                        {
                            result.IsCompatible = false;
                            result.Reason = $"Soğutucu \"{cpuSocket}\" soketini desteklemiyor. Desteklenen soketler: {supportedSockets}. Filtrelerden soket uyumuna bak.";
                            result.ExistingComponentName = $"CPU ({cpuSocket})";
                            result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")}";
                            break;
                        }
                    }

                    // TDP kapasitesi
                    var cpuTdpForCooler = GetSpecInt("CPU", "tdp");
                    if (cpuTdpForCooler > 0)
                    {
                        var coolerTdp = GetPropInt(component, "TdpRating");
                        if (coolerTdp > 0 && coolerTdp < cpuTdpForCooler)
                        {
                            result.IsCompatible = false;
                            result.Reason = $"İşlemcinin TDP'si {cpuTdpForCooler}W ama soğutucunun kapasitesi sadece {coolerTdp}W. Daha güçlü soğutucu seçmelisin.";
                            result.ExistingComponentName = $"CPU ({cpuTdpForCooler}W TDP)";
                            result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")} ({coolerTdp}W)";
                            break;
                        }
                    }

                    // Kasa yükseklik kontrolü (Hava soğutucu)
                    var coolerType = GetProp(component, "CoolerType");
                    if (Normalize(coolerType) == "air")
                    {
                        var maxCoolerHeight = GetSpecInt("Case", "maxCpuCoolerHeight");
                        var coolerHeight = GetPropInt(component, "Height");
                        if (maxCoolerHeight > 0 && coolerHeight > 0 && coolerHeight > maxCoolerHeight)
                        {
                            result.IsCompatible = false;
                            result.Reason = $"Soğutucu yüksekliği ({coolerHeight}mm) kasanın izin verdiği maksimum ({maxCoolerHeight}mm) değeri aşıyor. Daha kısa bir soğutucu veya sıvı soğutma seçebilirsin.";
                            result.ExistingComponentName = $"Kasa (Maks {maxCoolerHeight}mm)";
                            result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")} ({coolerHeight}mm)";
                        }
                    }

                    // Kasa radyatör kontrolü (Sıvı soğutucu)
                    if (Normalize(coolerType) == "liquid")
                    {
                        var radSize = GetPropInt(component, "RadiatorSize");
                        if (radSize > 0)
                        {
                            var caseFrontRad = GetSpec("Case", "radiatorSupportFront");
                            var caseTopRad = GetSpec("Case", "radiatorSupportTop");
                            var allRad = $"{caseFrontRad}, {caseTopRad}";

                            if (!string.IsNullOrEmpty(caseFrontRad) || !string.IsNullOrEmpty(caseTopRad))
                            {
                                if (!allRad.Contains(radSize.ToString()))
                                {
                                    result.IsCompatible = false;
                                    result.Reason = $"Kasanın radyatör desteği ({allRad.Trim(' ', ',')}) {radSize}mm radyatörü desteklemiyor.";
                                    result.ExistingComponentName = $"Kasa radyatör desteği";
                                    result.AttemptedComponentName = $"{GetProp(component, "Brand")} {GetProp(component, "ModelName")} ({radSize}mm)";
                                }
                            }
                        }
                    }
                    break;
                }
            }

            selected.Dispose();
            return Task.FromResult(result);
        }
    }
}
