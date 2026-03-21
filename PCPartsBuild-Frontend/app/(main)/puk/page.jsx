'use client';

import React, { useState, useEffect, useMemo, useRef, useCallback } from 'react';
import { api } from '@/lib/api';
import { translations } from '@/lib/translations';
import Swal from 'sweetalert2';

// 1. Kategoriler ve API uç noktaları
const categories = [
    { id: 'cpu', key: 'cpu', icon: 'memory', endpoint: 'processors' },
    { id: 'motherboard', key: 'motherboard', icon: 'developer_board', endpoint: 'motherboards' },
    { id: 'ram', key: 'ram', icon: 'memory_alt', endpoint: 'rams' },
    { id: 'gpu', key: 'gpu', icon: 'videogame_asset', endpoint: 'gpus' },
    { id: 'storage', key: 'storage', icon: 'storage', endpoint: 'storages' },
    { id: 'case', key: 'case', icon: 'computer', endpoint: 'cases' },
    { id: 'psu', key: 'psu', icon: 'electrical_services', endpoint: 'psus' },
    { id: 'cpuCooler', key: 'cpuCooler', icon: 'ac_unit', endpoint: 'cpucoolers' }
];

export default function PukPage() {
    const [lang, setLang] = useState('tr');
    const [selectedParts, setSelectedParts] = useState({
        cpu: null,
        motherboard: null,
        ram: null,
        gpu: null,
        storage: null,
        case: null,
        psu: null,
        cpuCooler: null
    });
    
    const [allData, setAllData] = useState({});
    const [loadingStates, setLoadingStates] = useState({});
    const [searchTerms, setSearchTerms] = useState({});
    const [openDropdown, setOpenDropdown] = useState(null);
    const [compatibilityResults, setCompatibilityResults] = useState({
        status: 'idle', // 'idle', 'checking', 'success', 'fail'
        issues: []
    });
    const [aiExpertResult, setAiExpertResult] = useState({
        loading: false,
        text: ''
    });

    // Refs for outside click and scrolling
    const dropdownRef = useRef(null);
    const resultsRef = useRef(null);

    // Initial Lang & LocalStorage
    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);
        
        const handleStorageChange = () => {
            setLang(localStorage.getItem('lang') || 'tr');
        };
        window.addEventListener('storage', handleStorageChange);
        return () => window.removeEventListener('storage', handleStorageChange);
    }, []);

    const t = translations[lang];

    // 2. Veri Yükleme (Lazy Load)
    const loadCategoryData = useCallback(async (catId, endpoint) => {
        if (allData[catId]) return; // Zaten yüklüyse tekrar çekme
        
        setLoadingStates(prev => ({ ...prev, [catId]: true }));
        try {
            const response = await api.get(endpoint);
            if (response.ok) {
                const data = await response.json();
                setAllData(prev => ({ ...prev, [catId]: data }));
            }
        } catch (error) {
            console.error(`Failed to load ${catId}:`, error);
        } finally {
            setLoadingStates(prev => ({ ...prev, [catId]: false }));
        }
    }, [allData]);

    // 3. Dropdown Logic
    const toggleDropdown = (catId, endpoint) => {
        if (openDropdown === catId) {
            setOpenDropdown(null);
        } else {
            setOpenDropdown(catId);
            loadCategoryData(catId, endpoint);
        }
    };

    const handleSelect = (catId, item) => {
        setSelectedParts(prev => ({ ...prev, [catId]: item }));
        setOpenDropdown(null);
        setSearchTerms(prev => ({ ...prev, [catId]: '' })); // Clear search after selection
        // Reset results on any selection change
        setCompatibilityResults({ status: 'idle', issues: [] });
    };

    const clearSelection = (e, catId) => {
        e.stopPropagation();
        setSelectedParts(prev => ({ ...prev, [catId]: null }));
        setCompatibilityResults({ status: 'idle', issues: [] });
    };

    // Close dropdowns on outside click
    useEffect(() => {
        const handleClickOutside = (e) => {
            if (openDropdown && !e.target.closest('.custom-select-wrapper')) {
                setOpenDropdown(null);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, [openDropdown]);

    // 4. Compatibility Engine (1:1 Port)
    const checkCompatibility = () => {
        const activeCount = Object.values(selectedParts).filter(p => p !== null).length;
        if (activeCount < 2) {
            Swal.fire({
                title: lang === 'tr' ? 'Yetersiz Parça' : 'Insufficient Components',
                text: t['error-selection'],
                icon: 'warning',
                background: '#1f2937', color: '#fff', confirmButtonColor: '#16a3b2'
            });
            return;
        }

        const issues = [];
        const { cpu, motherboard: mobo, ram, gpu, psu, case: pcCase, cpuCooler: cooler, storage } = selectedParts;

        // Utilities
        const isFilled = obj => obj && typeof obj === 'object';
        const toNumber = (v, fallback = 0) => {
            if (typeof v === 'number') return v;
            if (!v) return fallback;
            const n = Number(String(v).replace(/[^\d.-]/g, ''));
            return isNaN(n) ? fallback : n;
        };
        const toInt = (v, fallback = 0) => Math.round(toNumber(v, fallback));
        const toBoolean = v => {
            if (typeof v === 'boolean') return v;
            if (typeof v === 'string') return ['true', '1', 'yes'].includes(v.toLowerCase());
            return false;
        };
        const normalizeString = v => v ? String(v).trim().toLowerCase() : "";
        const normalizeNumberArray = v => {
            if (!v) return [];
            if (Array.isArray(v)) return v.map(x => toInt(x)).filter(n => !isNaN(n));
            if (typeof v === 'string') return v.split(',').map(s => toInt(s)).filter(n => !isNaN(n));
            return [];
        };

        // 1) CPU <-> Motherboard
        if (isFilled(cpu) && isFilled(mobo)) {
            const cpuSocket = normalizeString(cpu.socket);
            const moboSocket = normalizeString(mobo.socket);
            if (cpuSocket && moboSocket && cpuSocket !== moboSocket) {
                issues.push({ level: 'critical', msg: `${lang === 'tr' ? 'İşlemci' : 'CPU'} soketi (${cpu.socket.toUpperCase()}) ile ${lang === 'tr' ? 'Anakart' : 'Motherboard'} soketi (${mobo.socket.toUpperCase()}) uyumsuz.` });
            }
            if (!gpu && !toBoolean(cpu.integratedGraphics)) {
                issues.push({ level: 'critical', msg: `${lang === 'tr' ? 'İşlemci' : 'CPU'} dahili grafik birimine sahip değil. Görüntü alabilmek için harici bir ekran kartı gereklidir.` });
            }
        }

        // 2) RAM <-> Motherboard
        if (isFilled(ram) && isFilled(mobo)) {
            const ramType = normalizeString(ram.memoryType);
            const moboMemType = normalizeString(mobo.memoryType);
            if (ramType && moboMemType && ramType !== moboMemType) {
                issues.push({ level: 'critical', msg: `RAM tipi (${ram.memoryType}) ile Anakart (${mobo.memoryType}) uyumsuz.` });
            }
            const ramModules = toInt(ram.moduleCount || 1);
            const moboSlots = toInt(mobo.memorySlots || 0);
            if (moboSlots > 0 && ramModules > moboSlots) {
                issues.push({ level: 'critical', msg: `Seçilen RAM kiti (${ramModules} modül), anakartın slot sayısını (${moboSlots}) aşıyor.` });
            }
            const totalRam = toInt(ram.capacityPerModule || 0) * ramModules;
            const maxMoboRam = toInt(mobo.maxMemory || 0);
            if (maxMoboRam > 0 && totalRam > maxMoboRam) {
                issues.push({ level: 'critical', msg: `Toplam RAM (${totalRam}GB), anakartın maksimum desteğini (${maxMoboRam}GB) aşıyor.` });
            }
            const ramSpeed = toInt(ram.speed || 0);
            const supportedSpeeds = normalizeNumberArray(mobo.supportedMemoryFrequencies);
            if (ramSpeed > 0 && supportedSpeeds.length > 0) {
                const maxSpeed = Math.max(...supportedSpeeds);
                if (ramSpeed > maxSpeed) {
                    issues.push({ level: 'warning', msg: `RAM hızı (${ramSpeed}MHz), anakartın desteklediği maksimum hızdan (${maxSpeed}MHz) yüksek. RAM çalışır ancak downclock olur.` });
                }
            }
        }

        // 3) GPU <-> Case / PSU
        if (isFilled(gpu)) {
            if (isFilled(pcCase)) {
                const gpuLen = toInt(gpu.length);
                const maxGpu = toInt(pcCase.maxGpuLength);
                if (gpuLen > 0 && maxGpu > 0 && gpuLen > maxGpu) {
                    issues.push({ level: 'critical', msg: `Ekran kartı uzunluğu (${gpuLen}mm), kasanın sınırını (${maxGpu}mm) aşıyor.` });
                }
            }
            if (isFilled(psu)) {
                const gpuPower = toInt(gpu.tdp || 0);
                const cpuPower = toInt(cpu?.tdp || 0);
                const totalLoad = gpuPower + cpuPower + 100;
                const psuWatt = toInt(psu.wattage);
                if (psuWatt < totalLoad) {
                    issues.push({ level: 'critical', msg: `PSU gücü (${psuWatt}W) yetersiz. Tahmini tüketim: ${totalLoad}W.` });
                } else if (psuWatt < Math.ceil(totalLoad * 1.20)) {
                    issues.push({ level: 'warning', msg: `PSU kapasitesi sınırda. Önerilen güvenli değer: ${Math.ceil(totalLoad * 1.20)}W ve üzeri.` });
                }
                const recPsu = toInt(gpu.recommendedPsu);
                if (recPsu > 0 && psuWatt < recPsu) {
                    issues.push({ level: 'warning', msg: `Üretici bu ekran kartı için en az ${recPsu}W önermektedir.` });
                }
                const gpuConStr = normalizeString(gpu.powerConnectors);
                if ((gpuConStr.includes('16pin') || gpuConStr.includes('12vhpwr')) && !toBoolean(psu.has12VHPWR)) {
                    issues.push({ level: 'warning', msg: `Ekran kartı 12VHPWR girişi istiyor, PSU'nuzda bu port yok. Dönüştürücü gerekebilir.` });
                }
            }
        }

        // 4) CPU Cooler <-> Case
        if (isFilled(cooler) && isFilled(pcCase)) {
            const type = normalizeString(cooler.coolerType);
            if (type === 'air') {
                const h = toInt(cooler.height);
                const maxH = toInt(pcCase.maxCpuCoolerHeight);
                if (h > maxH && maxH > 0) {
                    issues.push({ level: 'critical', msg: `Soğutucu yüksekliği (${h}mm), kasanın sınırını (${maxH}mm) aşıyor.` });
                }
            } else if (type === 'liquid') {
                const radSize = toInt(cooler.radiatorSize);
                const frontSup = normalizeNumberArray(pcCase.radiatorSupportFront);
                const topSup = normalizeNumberArray(pcCase.radiatorSupportTop);
                if (!frontSup.includes(radSize) && !topSup.includes(radSize)) {
                    issues.push({ level: 'critical', msg: `Seçilen ${radSize}mm radyatör kasanın ne ön ne de üst paneline uyuyor.` });
                }
            }
        }

        // 5) Case <-> Motherboard
        if (isFilled(pcCase) && isFilled(mobo)) {
            const supported = normalizeString(pcCase.supportedMotherboards);
            const moboForm = normalizeString(mobo.formFactor);
            if (supported && moboForm && !supported.includes(moboForm)) {
                issues.push({ level: 'critical', msg: `Kasa, ${mobo.formFactor} boyutundaki anakartı desteklemiyor.` });
            }
        }

        // Results update
        setCompatibilityResults({
            status: issues.length > 0 ? 'fail' : 'success',
            issues: issues
        });

        // Scroll to results
        setTimeout(() => {
            resultsRef.current?.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }, 100);
    };

    // 5. AI Expert Opinion (Gemini Integration)
    const getExpertOpinion = async () => {
        const activeParts = Object.entries(selectedParts).filter(([_, p]) => p !== null);
        if (activeParts.length < 2) return;

        setAiExpertResult({ loading: true, text: '' });
        
        try {
            const partsList = activeParts.map(([key, item]) => {
                let name = `${item.brand || ''} ${item.modelName || ''}`.trim();
                return `- ${key.toUpperCase()}: ${name}`;
            });

            const promptTitle = t['analysis-prompt-title'];
            const strictInstructions = lang === 'tr' ? `
YUKARIDAKİ PARÇALAR İÇİN ANALİZ TALİMATLARI:
1. Yanıtı çok KISA, ÖZ ve NET tut. Uzun paragraflardan kaçın kullanıcıyı kötüleyen kelimele kullanma bu sistem rezlaet en kçtü sistem vs gibi.
2. Teknik terim kullanabilirsin ancak yabancı terimler kullanma amatör bir kullanıcının anlayacağı dilde olsun.
3. Yanıtı sadece şu 3 kısa madde başlığı altında topla:
   - 🟢 Genel Uyum: (Uyumlu olup olmayan bileşenleri adını tek tek belirt ama uzatma uyumlu olanları ayrı uyumsuz olanları ayrı belirt özellikle ekran kartı ve işlemci açısından mantıksız sistemleri denetle bu ilemci ekran kartıyla fiziksek olarak uyumlu olsa bile arasındaki devasa farktan dolayı bu işlemci erkan kartını besleyemez vs gibi)
   - ⚠️ Darboğaz/Risk: (Varsa darboğazı ve tahmini yüzdesini veya varsa PSU yetersizliğini belirt, yoksa 'Sorun yok' de)
   - 🚀 Performans Tahmini: (Hangi seviyede bir performansla oyun oynatacağını 1-2 cümleyle özetle düşük bir sistem ise hangi seviyede oyunları oynatamayayacağını belirt)
4. Toplam yanıt 200 kelimeyi geçmesin.` : `
ANALYSIS INSTRUCTIONS:
1. Keep the response VERY SHORT, CONCISE, and CLEAR. Avoid long paragraphs.
2. Use technical terms but keep it understandable for average users.
3. Structure the response strictly under these 3 bullet points:
   - 🟢 **Overall Compatibility:** (State compatibility in one sentence)
   - ⚠️ **Bottlenecks/Risks:** (Mention bottlenecks or PSU issues if any, otherwise say 'No issues')
   - 🚀 **Performance Estimate:** (Summarize gaming performance/resolution in 1-2 sentences)
4. Total response must not exceed 150 words.`;

            const prompt = `${promptTitle}\n${strictInstructions}\n\nSİSTEM PARÇALARI:\n${partsList.join('\n')}`;
            
            const GEMINI_API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
            const API_KEY = process.env.NEXT_PUBLIC_GEMINI_API_KEY; // This should be configured

            if (!API_KEY) {
                setAiExpertResult({ loading: false, text: t['error-api'] });
                return;
            }

            const response = await fetch(`${GEMINI_API_URL}?key=${API_KEY}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    contents: [{ parts: [{ text: prompt }] }]
                })
            });

            const result = await response.json();
            const text = result.candidates?.[0]?.content?.parts?.[0]?.text || t['error-api'];
            
            // Format markdown-like text (simple version)
            const formatted = text
                .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
                .replace(/\n/g, '<br>');

            setAiExpertResult({ loading: false, text: formatted });
            
            // Show in modal as per legacy
            Swal.fire({
                title: t['action-expert'],
                html: `<div class="text-left text-sm leading-relaxed overflow-y-auto max-h-[60vh]">${formatted}</div>`,
                background: '#1f2937',
                color: '#fff',
                width: '800px',
                confirmButtonColor: '#16a3b2'
            });

        } catch (error) {
            console.error("AI Analysis failed:", error);
            setAiExpertResult({ loading: false, text: t['error-api'] });
        }
    };

    return (
        <main className="flex-grow container mx-auto px-4 py-8 md:py-12">
            {/* Header Content */}
            <div className="text-center max-w-4xl mx-auto mb-10">
                <h1 className="text-3xl md:text-5xl font-bold text-gray-900 dark:text-white tracking-tight">
                    {t['puk-title']}
                </h1>
                <p className="mt-3 text-lg text-gray-600 dark:text-gray-400">
                    {t['puk-subtitle']}
                </p>
            </div>

            {/* Selection Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
                {categories.map((cat) => (
                    <div 
                        key={cat.id}
                        className={`component-card group relative bg-white dark:bg-card-dark p-4 rounded-xl border-2 transition-all duration-300 ${
                            selectedParts[cat.id] 
                                ? 'border-primary shadow-lg bg-blue-50/20 dark:bg-blue-900/10' 
                                : 'border-dashed border-gray-300 dark:border-gray-700 hover:border-primary/50'
                        }`}
                        style={{ zIndex: openDropdown === cat.id ? 50 : 'auto' }}
                    >
                        <div className="flex items-center gap-3 mb-4">
                            <div className={`p-2 rounded-lg ${selectedParts[cat.id] ? 'bg-primary text-white' : 'bg-gray-100 dark:bg-gray-800 text-gray-500'}`}>
                                <span className="material-symbols-outlined">{cat.icon}</span>
                            </div>
                            <h3 className="font-semibold text-gray-900 dark:text-white uppercase text-sm tracking-wide">
                                {t[`label-${cat.id}`]}
                            </h3>
                        </div>

                        {/* Custom Select Wrapper */}
                        <div className={`custom-select-wrapper ${openDropdown === cat.id ? 'active' : ''} ${selectedParts[cat.id] ? 'has-selection' : ''}`}>
                            <div 
                                className="custom-select-trigger"
                                onClick={() => toggleDropdown(cat.id, cat.endpoint)}
                            >
                                <span className={`selection-text truncate ${selectedParts[cat.id] ? 'text-gray-900 dark:text-white font-medium' : 'text-gray-500 dark:text-gray-400'}`}>
                                    {selectedParts[cat.id] ? `${selectedParts[cat.id].brand} ${selectedParts[cat.id].modelName}` : t['select-default']}
                                </span>
                                <div className="flex items-center">
                                    <button 
                                        className="clear-btn" 
                                        onClick={(e) => clearSelection(e, cat.id)}
                                    >
                                        <span className="material-symbols-outlined text-[18px]">close</span>
                                    </button>
                                    <span className="material-symbols-outlined arrow-icon text-gray-400">expand_more</span>
                                </div>
                            </div>

                            {/* Options Dropdown */}
                            {openDropdown === cat.id && (
                                <div className="custom-select-options">
                                    <div className="custom-search-box">
                                        <input 
                                            type="text" 
                                            placeholder={t['search-placeholder']}
                                            value={searchTerms[cat.id] || ''}
                                            onChange={(e) => setSearchTerms(prev => ({ ...prev, [cat.id]: e.target.value }))}
                                            autoFocus
                                        />
                                    </div>
                                    <div className="custom-options-list custom-scrollbar">
                                        {loadingStates[cat.id] ? (
                                            <div className="p-4 text-center text-gray-500 text-sm">
                                                <div className="animate-spin inline-block w-4 h-4 border-2 border-primary border-t-transparent rounded-full mr-2"></div>
                                                {t['loading-data']}
                                            </div>
                                        ) : (
                                            allData[cat.id]?.filter(item => {
                                                const search = (searchTerms[cat.id] || '').toLowerCase();
                                                const fullName = `${item.brand} ${item.modelName}`.toLowerCase();
                                                return fullName.includes(search);
                                            }).map(item => (
                                                <div 
                                                    key={item.id}
                                                    className="custom-option"
                                                    onClick={() => handleSelect(cat.id, item)}
                                                >
                                                    {item.brand} {item.modelName} 
                                                    {cat.id === 'storage' && ` (${item.capacity} GB)`}
                                                    {cat.id === 'psu' && ` (${item.wattage}W)`}
                                                </div>
                                            ))
                                        )}
                                        {allData[cat.id]?.length === 0 && (
                                            <div className="p-4 text-center text-gray-500 text-sm">{lang === 'tr' ? 'Parça bulunamadı.' : 'No items found.'}</div>
                                        )}
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>
                ))}
            </div>

            {/* Results & Actions Area */}
            <div className="max-w-4xl mx-auto space-y-8" ref={resultsRef}>
                {/* Status Sections */}
                {compatibilityResults.status === 'idle' && (
                    <div className="flex flex-col items-center justify-center p-12 bg-white dark:bg-card-dark rounded-2xl border-2 border-dashed border-gray-200 dark:border-gray-800 transition-all duration-300">
                        <div className="w-16 h-16 bg-gray-50 dark:bg-gray-800/50 rounded-full flex items-center justify-center mb-6">
                            <span className="material-symbols-outlined text-4xl text-gray-300">fact_check</span>
                        </div>
                        <h2 className="text-2xl font-bold text-gray-800 dark:text-white mb-2">{t['result-info-title']}</h2>
                        <p className="text-gray-500 dark:text-gray-400 text-center max-w-md leading-relaxed">{t['result-info-text']}</p>
                    </div>
                )}

                {compatibilityResults.status === 'success' && (
                    <div className="flex flex-col md:flex-row items-center gap-6 p-8 bg-green-50 dark:bg-green-950/20 rounded-2xl border border-green-200 dark:border-green-900 animate-fade-in-down">
                        <div className="w-20 h-20 bg-green-500 rounded-2xl flex items-center justify-center shrink-0 shadow-lg shadow-green-500/20">
                            <span className="material-symbols-outlined text-white text-5xl">check_circle</span>
                        </div>
                        <div className="text-center md:text-left">
                            <h2 className="text-2xl font-bold text-green-900 dark:text-green-100 mb-2">{t['result-success-title']}</h2>
                            <p className="text-green-700 dark:text-green-400/80 leading-relaxed">
                                {lang === 'tr' 
                                    ? `Seçilen parçalar (${Object.values(selectedParts).filter(p => p !== null).length} adet) birbiriyle tam uyumlu görünüyor. Herhangi bir teknik çakışma tespit edilmedi. İyi oyunlar! 🎮`
                                    : `The selected ${Object.values(selectedParts).filter(p => p !== null).length} components appear to be fully compatible. No technical conflicts detected. Happy gaming! 🎮`}
                            </p>
                        </div>
                    </div>
                )}

                {compatibilityResults.status === 'fail' && (
                    <div className="flex flex-col gap-6 p-8 bg-red-50 dark:bg-red-950/20 rounded-2xl border border-red-200 dark:border-red-900 animate-fade-in-down">
                        <div className="flex items-center gap-4">
                            <div className="w-14 h-14 bg-red-500 rounded-xl flex items-center justify-center shrink-0">
                                <span className="material-symbols-outlined text-white text-3xl">error</span>
                            </div>
                            <h2 className="text-2xl font-bold text-red-900 dark:text-red-100">{t['result-fail-title']}</h2>
                        </div>
                        <ul className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            {compatibilityResults.issues.map((issue, idx) => (
                                <li 
                                    key={idx} 
                                    className={`flex items-start gap-3 bg-white dark:bg-gray-900/40 p-4 rounded-xl border-l-4 shadow-sm border ${
                                        issue.level === 'critical' ? 'border-l-red-500 border-gray-100 dark:border-red-900/50' : 'border-l-orange-500 border-gray-100 dark:border-orange-900/50'
                                    }`}
                                >
                                    <span className={`material-symbols-outlined shrink-0 mt-0.5 ${issue.level === 'critical' ? 'text-red-500' : 'text-orange-500'}`}>
                                        {issue.level === 'critical' ? 'report' : 'warning'}
                                    </span>
                                    <span className="text-gray-800 dark:text-gray-200 text-sm font-medium leading-normal">{issue.msg}</span>
                                </li>
                            ))}
                        </ul>
                    </div>
                )}

                {/* Actions Grid */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <button 
                        onClick={checkCompatibility}
                        className="group flex items-center justify-center gap-3 bg-primary text-white py-5 rounded-2xl font-bold text-lg hover:shadow-xl hover:shadow-primary/20 active:scale-[0.98] transition-all"
                    >
                        <span className="material-symbols-outlined group-hover:rotate-12 transition-transform">bolt</span>
                        {t['action-check']}
                    </button>
                    <button 
                        onClick={getExpertOpinion}
                        disabled={Object.values(selectedParts).filter(p => p !== null).length < 2 || aiExpertResult.loading}
                        className="group flex items-center justify-center gap-3 bg-gray-900 dark:bg-white text-white dark:text-gray-900 py-5 rounded-2xl font-bold text-lg hover:shadow-xl hover:shadow-black/10 dark:hover:shadow-white/10 active:scale-[0.98] transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        <span className={`material-symbols-outlined ${aiExpertResult.loading ? 'animate-spin' : 'group-hover:translate-x-1 transition-transform'}`}>
                            {aiExpertResult.loading ? 'sync' : 'smart_toy'}
                        </span>
                        {aiExpertResult.loading ? t['ai-analiz'] : t['action-expert']}
                    </button>
                </div>
            </div>
        </main>
    );
}
