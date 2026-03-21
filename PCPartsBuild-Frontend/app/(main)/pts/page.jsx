'use client';

import React, { useState, useEffect, useMemo, useCallback } from 'react';
import { api } from '@/lib/api';
import { translations } from '@/lib/translations';
import Swal from 'sweetalert2';

// API endpoints mapper based on legacy pts.html (lines 677-686)
const COMPONENT_ENDPOINTS = {
    cpu: 'processors',
    motherboard: 'motherboards',
    ram: 'rams',
    gpu: 'gpus',
    case: 'cases',
    psu: 'psus',
    storage: 'storages',
    cpuCooler: 'cpuCoolers'
};

const ITEMS_PER_PAGE = 24;

export default function PtsPage() {
    const [lang, setLang] = useState('tr');
    const [currentCategory, setCurrentCategory] = useState('cpu');
    const [allData, setAllData] = useState({
        cpu: [], motherboard: [], ram: [], gpu: [],
        case: [], psu: [], storage: [], cpuCooler: []
    });
    const [currentBuild, setCurrentBuild] = useState({
        cpu: null, motherboard: null, ram: null, gpu: null,
        case: null, psu: null, storage: null, cpuCooler: null
    });
    const [loading, setLoading] = useState(false);
    const [currentPage, setCurrentPage] = useState(1);
    const [searchQuery, setSearchQuery] = useState('');
    const [selectedFilters, setSelectedFilters] = useState({});
    const [expandedFilters, setExpandedFilters] = useState({});
    const [filterShowMore, setFilterShowMore] = useState({});

    const t = translations[lang] || translations['tr'];

    // Initialize state from localStorage
    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);

        const handleStorageChange = () => {
            setLang(localStorage.getItem('lang') || 'tr');
        };
        window.addEventListener('storage', handleStorageChange);
        return () => window.removeEventListener('storage', handleStorageChange);
    }, []);

    // Fetch data for current category
    useEffect(() => {
        const fetchData = async () => {
            // Cache check: Don't refetch if we already have data
            if (allData[currentCategory]?.length > 0) return;

            setLoading(true);
            try {
                const endpoint = COMPONENT_ENDPOINTS[currentCategory];
                if (!endpoint) return;

                const response = await api.get(endpoint);
                if (response.ok) {
                    const data = await response.json();
                    if (Array.isArray(data)) {
                        setAllData(prev => ({ ...prev, [currentCategory]: data }));
                    }
                }
            } catch (error) {
                console.error("Fetch error:", error);
                // Fail silent or minor alert
            } finally {
                setLoading(false);
            }
        };
        fetchData();
        setCurrentPage(1);
        setSearchQuery('');
        setSelectedFilters({});
    }, [currentCategory, allData]);

    // Compatibility Logic (Ported from legacy applyFiltersAndRenderList step 833-1055)
    const compatibilityInfo = useMemo(() => {
        const messages = {};
        const items = allData[currentCategory] || [];
        
        items.forEach(item => {
            let isCompatible = true;
            let failReason = "";

            if (currentCategory === 'cpu') {
                if (currentBuild.motherboard && item.socket !== currentBuild.motherboard.socket) {
                    isCompatible = false;
                    failReason = t["incompatible-socket-cpu"];
                }
                if (currentBuild.cpuCooler && item.socket && !currentBuild.cpuCooler.supportedSockets?.includes(item.socket)) {
                    isCompatible = false;
                    failReason = t["incompatible-socket-cooler"];
                }
            }

            if (currentCategory === 'motherboard') {
                if (currentBuild.cpu && item.socket !== currentBuild.cpu.socket) {
                    isCompatible = false;
                    failReason = t["incompatible-socket-mobo"];
                }
                if (currentBuild.ram && item.memoryType !== currentBuild.ram.memoryType) {
                    isCompatible = false;
                    failReason = t["incompatible-ramType"];
                }
                if (currentBuild.case) {
                    const rawSupport = currentBuild.case.supportedMotherboards || currentBuild.case.SupportedMotherboards || "";
                    const moboForm = item.formFactor ? item.formFactor.trim().toLowerCase() : "";
                    const supportList = Array.isArray(rawSupport) ? rawSupport : rawSupport.split(',').map(s => s.trim().toLowerCase());
                    if (!supportList.includes(moboForm)) {
                        isCompatible = false;
                        failReason = t["incompatible-moboFormFactor"];
                    }
                }
            }

            if (currentCategory === 'ram') {
                if (currentBuild.motherboard && item.memoryType !== currentBuild.motherboard.memoryType) {
                    isCompatible = false;
                    failReason = t["incompatible-ramType"];
                }
            }

            if (currentCategory === 'gpu') {
                if (currentBuild.case) {
                    const maxLen = currentBuild.case.maxGpuLength || currentBuild.case.MaxGpuLength || 999;
                    if ((item.length || 0) > maxLen) {
                        isCompatible = false;
                        failReason = t["incompatible-gpuLength"].replace('{maxGpuLength}', maxLen);
                    }
                }
            }

            if (currentCategory === 'case') {
                const moboForm = currentBuild.motherboard?.formFactor?.trim().toLowerCase();
                const gpuLen = currentBuild.gpu?.length || 0;
                const cooler = currentBuild.cpuCooler;

                if (moboForm) {
                    const rawSupport = item.supportedMotherboards || item.SupportedMotherboards || "";
                    const supportList = Array.isArray(rawSupport) ? rawSupport : rawSupport.split(',').map(s => s.trim().toLowerCase());
                    if (!supportList.includes(moboForm)) {
                        isCompatible = false;
                        failReason = t["incompatible-moboFormFactor"];
                    }
                }

                if (isCompatible && gpuLen > 0) {
                    const maxGpu = item.maxGpuLength || item.MaxGpuLength || 999;
                    if (gpuLen > maxGpu) {
                        isCompatible = false;
                        failReason = t["incompatible-gpuLength"].replace('{maxGpuLength}', maxGpu);
                    }
                }

                if (isCompatible && cooler) {
                    if (cooler.coolerType === 'Liquid') {
                        const radSize = String(cooler.radiatorSize || "");
                        const frontList = (item.radiatorSupportFront || item.RadiatorSupportFront || "").split(',').map(s => s.trim());
                        const topList = (item.radiatorSupportTop || item.RadiatorSupportTop || "").split(',').map(s => s.trim());
                        if (!frontList.includes(radSize) && !topList.includes(radSize)) {
                            isCompatible = false;
                            failReason = lang === 'tr' ? `Seçili ${radSize}mm sıvı soğutma kasanın panellerine uymuyor.` : `Selected ${radSize}mm liquid cooler doesn't fit the case panels.`;
                        }
                    } else {
                        const maxCooler = item.maxCpuCoolerHeight || item.MaxCpuCoolerHeight || 999;
                        if ((cooler.height || 0) > maxCooler) {
                            isCompatible = false;
                            failReason = t["incompatible-coolerHeight"].replace('{maxCpuCoolerHeight}', maxCooler);
                        }
                    }
                }
            }

            if (currentCategory === 'cpuCooler') {
                if (currentBuild.cpu && !item.supportedSockets?.includes(currentBuild.cpu.socket)) {
                    isCompatible = false;
                    failReason = t["incompatible-socket-cooler"];
                }
                if (isCompatible && currentBuild.case) {
                    const c = currentBuild.case;
                    if (item.coolerType === 'Liquid') {
                        const radSize = String(item.radiatorSize || "");
                        const frontList = (c.radiatorSupportFront || c.RadiatorSupportFront || "").split(',').map(s => s.trim());
                        const topList = (c.radiatorSupportTop || c.RadiatorSupportTop || "").split(',').map(s => s.trim());
                        if (!frontList.includes(radSize) && !topList.includes(radSize)) {
                            isCompatible = false;
                            failReason = lang === 'tr' ? `Kasa ${radSize}mm radyatör desteklemiyor.` : `Case does not support ${radSize}mm radiator.`;
                        }
                    } else {
                        const maxCooler = c.maxCpuCoolerHeight || c.MaxCpuCoolerHeight || 999;
                        if ((item.height || 0) > maxCooler) {
                            isCompatible = false;
                            failReason = t["incompatible-coolerHeight"].replace('{maxCpuCoolerHeight}', maxCooler);
                        }
                    }
                }
            }

            if (currentCategory === 'psu') {
                if (currentBuild.case) {
                    const maxPsuLen = currentBuild.case.maxPsuLength || currentBuild.case.MaxPsuLength || 999;
                    if ((item.length || 0) > maxPsuLen && (item.length || 0) !== 0) {
                        isCompatible = false;
                        failReason = lang === 'tr' ? "Kasa için çok uzun." : "Too long for case.";
                    }
                }
            }

            if (!isCompatible) {
                messages[item.id] = failReason;
            }
        });

        return messages;
    }, [allData, currentCategory, currentBuild, t, lang]);

    // Filtering & Searching Logic (useMemo for performance)
    const filteredItems = useMemo(() => {
        let items = [...(allData[currentCategory] || [])];

        // 1. Search Query filter (matches Name or Brand)
        if (searchQuery) {
            const lowQuery = searchQuery.toLowerCase();
            items = items.filter(item => 
                (item.modelName || "").toLowerCase().includes(lowQuery) || 
                (item.brand || "").toLowerCase().includes(lowQuery)
            );
        }

        // 2. Sidebar Filters (intersecting values)
        for (const [type, values] of Object.entries(selectedFilters)) {
            if (values.length > 0) {
                items = items.filter(item => {
                    if (type === 'supportedMotherboards' || type === 'supportedMotherboardFormFactors') {
                        const raw = (item[type] || item.SupportedMotherboards || "");
                        const supported = (Array.isArray(raw) ? raw : raw.split(',')).map(s => s.trim().toLowerCase());
                        return values.some(v => supported.includes(v.toLowerCase()));
                    }
                    if (type === 'isModular') {
                        return values.includes(String(item[type]));
                    }
                    const val = String(item[type]);
                    return values.includes(val);
                });
            }
        }

        // 3. Sorting: Compatible first, then by Brand/Model
        items.sort((a, b) => {
            const isAInc = !!compatibilityInfo[a.id] && !(currentBuild[currentCategory]?.id === a.id);
            const isBInc = !!compatibilityInfo[b.id] && !(currentBuild[currentCategory]?.id === b.id);
            return isAInc - isBInc;
        });

        return items;
    }, [allData, currentCategory, searchQuery, selectedFilters, compatibilityInfo, currentBuild]);

    // Pagination Calculation
    const totalPages = Math.ceil(filteredItems.length / ITEMS_PER_PAGE);
    const paginatedItems = filteredItems.slice((currentPage - 1) * ITEMS_PER_PAGE, currentPage * ITEMS_PER_PAGE);

    // Totals Calculation (Ported from legacy calculateTotals 1456-1509)
    const totals = useMemo(() => {
        const cpuTdp = currentBuild.cpu?.tdp || 0;
        const gpuTdp = currentBuild.gpu?.tdp || 0;
        const totalWattage = cpuTdp + gpuTdp;

        let recommendedPsu = 0;
        if (currentBuild.gpu && (currentBuild.gpu.RecommendedPsu || currentBuild.gpu.recommendedPsu)) {
            recommendedPsu = currentBuild.gpu.RecommendedPsu || currentBuild.gpu.recommendedPsu;
        } else if (totalWattage > 0) {
            recommendedPsu = Math.ceil(((totalWattage + 100) * 1.25) / 50) * 50;
            if (recommendedPsu < 300) recommendedPsu = 300;
        }

        // Calculate Price based on available price fields in data
        const totalPrice = Object.values(currentBuild).reduce((sum, item) => sum + (item?.price || 0), 0);

        return {
            wattage: totalWattage,
            psu: recommendedPsu,
            price: totalPrice
        };
    }, [currentBuild]);

    // User Selection Handlers
    const handleCategoryChange = (cat) => {
        setCurrentCategory(cat);
    };

    const addToBuild = (item) => {
        setCurrentBuild(prev => ({ ...prev, [currentCategory]: item }));
    };

    const removeFromBuild = (type) => {
        setCurrentBuild(prev => ({ ...prev, [type]: null }));
    };

    const handleFilterChange = (type, value) => {
        setSelectedFilters(prev => {
            const current = prev[type] || [];
            if (current.includes(value)) {
                return { ...prev, [type]: current.filter(v => v !== value) };
            } else {
                return { ...prev, [type]: [...current, value] };
            }
        });
        setCurrentPage(1); // Reset page on filter change
    };

    const saveBuildAction = async () => {
        const userId = localStorage.getItem('userId');
        if (!userId) {
            Swal.fire({
                icon: 'warning',
                title: t['auth-login'],
                text: lang === 'tr' ? 'Sistemi kaydetmek için lütfen giriş yapın.' : 'Please log in to save your build.',
                showCancelButton: true,
                confirmButtonText: t['auth-login'],
                confirmButtonColor: '#16a3b2'
            }).then(r => { if (r.isConfirmed) window.location.href = '/giris'; });
            return;
        }

        const hasParts = Object.values(currentBuild).some(p => p !== null);
        if (!hasParts) {
            Swal.fire('Uyarı', t['msg-no-parts'], 'info');
            return;
        }

        const { value: buildName } = await Swal.fire({
            title: lang === 'tr' ? 'Sisteme İsim Verin' : 'Name Your Build',
            input: 'text',
            inputPlaceholder: 'Örn: Oyun Bilgisayarım',
            showCancelButton: true,
            confirmButtonText: 'Kaydet',
            confirmButtonColor: '#16a3b2',
            inputValidator: (v) => !v && 'Bir isim yazmalısınız!'
        });

        if (buildName) {
            Swal.showLoading();
            const payload = {
                userId, buildName, totalPrice: totals.price,
                cpuId: currentBuild.cpu?.id,
                motherboardId: currentBuild.motherboard?.id,
                ramId: currentBuild.ram?.id,
                gpuId: currentBuild.gpu?.id,
                storageId: currentBuild.storage?.id,
                caseId: currentBuild.case?.id,
                psuId: currentBuild.psu?.id,
                cpuCoolerId: currentBuild.cpuCooler?.id
            };
            try {
                const res = await api.post('builds/save', payload);
                if (res.ok) {
                    Swal.fire('Başarılı!', lang === 'tr' ? 'Sisteminiz kaydedildi.' : 'Your build has been saved.', 'success');
                } else {
                    throw new Error('Save failed');
                }
            } catch (e) {
                Swal.fire('Hata', lang === 'tr' ? 'Kaydedilirken bir hata oluştu.' : 'An error occurred while saving.', 'error');
            }
        }
    };

    // Calculate filter categories dynamically from the current data
    const dynamicFilterProps = useMemo(() => {
        const configMap = {
            cpu: ['socket', 'brand', 'coreCount'],
            motherboard: ['socket', 'formFactor', 'chipset', 'brand', 'memoryType'],
            ram: ['memoryType', 'capacityPerModule', 'moduleCount', 'speed', 'brand'],
            gpu: ['brand', 'vramMemorySize', 'chipset'],
            case: ['caseType', 'supportedMotherboardFormFactors', 'brand'],
            psu: ['wattage', 'isModular', 'rating', 'brand'],
            storage: ['capacity', 'storageType', 'interface', 'brand'],
            cpuCooler: ['coolerType', 'radiatorSize', 'brand']
        };
        const props = configMap[currentCategory] || [];
        const items = allData[currentCategory] || [];
        
        return props.map(prop => {
            let uniqueValues = [];
            if (prop === 'supportedMotherboards' || prop === 'supportedMotherboardFormFactors') {
                uniqueValues = [...new Set(items.flatMap(i => {
                    const raw = (i[prop] || i.SupportedMotherboards || "");
                    return (Array.isArray(raw) ? raw : raw.split(',')).map(s => s.trim());
                }))].filter(Boolean);
            } else {
                uniqueValues = [...new Set(items.map(i => String(i[prop])))].filter(v => v && v !== 'null' && v !== 'undefined' && v !== "");
            }
            // Sort values (number-aware)
            uniqueValues.sort((a,b) => {
                const na = parseFloat(a), nb = parseFloat(b);
                if (!isNaN(na) && !isNaN(nb)) return na - nb;
                return a.localeCompare(b);
            });
            return { prop, values: uniqueValues };
        }).filter(f => f.values.length > 0);
    }, [currentCategory, allData]);


    return (
        <div className="flex flex-col flex-1 bg-background-light dark:bg-background-dark font-sans transition-default">
            
            {/* Category Navigation Bar (Ported from legacy 74-109) */}
            <div className="w-full bg-card-light dark:bg-card-darker border-b border-gray-200 dark:border-gray-800/50 overflow-x-auto shadow-md">
                <div className="container mx-auto flex items-center gap-2 px-4 h-16">
                    {Object.keys(COMPONENT_ENDPOINTS).map(cat => (
                        <button
                            key={cat}
                            onClick={() => handleCategoryChange(cat)}
                            className={`flex-shrink-0 flex items-center gap-2 h-full px-4 border-b-2 transition-colors ${currentCategory === cat ? 'border-primary text-primary' : 'border-transparent text-gray-500 dark:text-gray-400 hover:text-primary'}`}
                        >
                            <span className="material-symbols-outlined text-[20px]">{getIconForCategory(cat)}</span>
                            <span className="text-sm font-semibold whitespace-nowrap">{t[`tab-${cat === 'motherboard' ? 'mobo' : cat === 'cpuCooler' ? 'cooler' : cat}`]}</span>
                        </button>
                    ))}
                </div>
            </div>

            <main className="flex-grow container mx-auto px-4 py-8">
                <div className="flex flex-col lg:flex-row lg:gap-8">
                    
                    {/* Filter Sidebar Component */}
                    <aside className="w-full lg:w-1/4 mb-6 lg:mb-0">
                        <div className="sticky top-10">
                            <div className="bg-card-light dark:bg-card-darker rounded-xl border border-gray-200 dark:border-gray-800/50 shadow-xl flex flex-col max-h-[calc(100vh-12rem)]">
                                <div className="p-5 border-b border-gray-200 dark:border-gray-800/50">
                                    <h3 className="text-lg font-bold mb-4 text-gray-900 dark:text-white uppercase tracking-tight">{t["filter-title"]}</h3>
                                    <div className="relative group">
                                        <input 
                                            type="text" 
                                            value={searchQuery}
                                            onChange={(e) => setSearchQuery(e.target.value)}
                                            placeholder={t["search-placeholder"]} 
                                            className="w-full bg-gray-50 dark:bg-gray-800/30 border border-gray-200 dark:border-gray-700 text-sm rounded-lg focus:ring-2 focus:ring-primary/50 focus:border-primary p-3 transition-all duration-200"
                                        />
                                        <span className="absolute right-3 top-3 text-gray-400 group-focus-within:text-primary material-symbols-outlined text-[20px]">search</span>
                                    </div>
                                </div>
                                <div className="p-4 overflow-y-auto flex-grow space-y-3 custom-scrollbar">
                                    {dynamicFilterProps.map(({ prop, values }) => (
                                        <div key={prop} className="border-b border-gray-100 dark:border-gray-800/50 last:border-0 py-2">
                                            <button 
                                                className="w-full flex justify-between items-center py-2 text-left focus:outline-none transition-colors group"
                                                onClick={() => setExpandedFilters(prev => ({...prev, [prop]: !prev[prop]}))}
                                            >
                                                <span className="text-xs font-black text-gray-500 dark:text-gray-400 group-hover:text-primary uppercase tracking-widest">{t[`filter-${prop}`] || prop}</span>
                                                <span className={`material-symbols-outlined text-gray-400 text-[18px] transition-transform ${expandedFilters[prop] ? 'rotate-180' : ''}`}>expand_more</span>
                                            </button>
                                            {expandedFilters[prop] && (
                                                <div className="pl-1 pb-3 pt-1">
                                                    {(filterShowMore[prop] ? values : values.slice(0, 5)).map(val => (
                                                        <label key={val} className="flex items-center py-1.5 cursor-pointer group hover:bg-gray-50 dark:hover:bg-white/5 rounded px-1 transition-colors">
                                                            <input 
                                                                type="checkbox" 
                                                                checked={(selectedFilters[prop] || []).includes(val)}
                                                                onChange={() => handleFilterChange(prop, val)}
                                                                className="h-4 w-4 rounded border-gray-300 dark:border-gray-700 text-primary bg-white dark:bg-gray-800 focus:ring-offset-0 focus:ring-primary/30"
                                                            />
                                                            <span className="ml-3 text-sm text-gray-700 dark:text-gray-300 group-hover:text-primary font-medium">{formatFilterValue(prop, val, t)}</span>
                                                        </label>
                                                    ))}
                                                    {values.length > 5 && (
                                                        <button 
                                                            className="text-[11px] font-black text-primary hover:text-primary/70 mt-2 px-1 uppercase tracking-tighter"
                                                            onClick={() => setFilterShowMore(prev => ({...prev, [prop]: !prev[prop]}))}
                                                        >
                                                            {filterShowMore[prop] ? t["action-show-less"] : t["action-show-all"]}
                                                        </button>
                                                    )}
                                                </div>
                                            )}
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>
                    </aside>

                    {/* Middle Section: Component Cards Grid */}
                    <div className="w-full lg:w-2/4 flex flex-col">
                        <div className="flex justify-between items-center mb-6">
                            <h2 className="text-2xl font-black text-gray-900 dark:text-white tracking-tight">
                                {t[`step-title-${currentCategory === 'motherboard' ? 'motherboard' : currentCategory}`]}
                            </h2>
                            <span className="text-sm font-semibold text-gray-500">{filteredItems.length} {lang === 'tr' ? 'Sonuç' : 'Results'}</span>
                        </div>
                        <div className="grid grid-cols-1 gap-5 min-h-[600px] content-start">
                            {loading ? (
                                <div className="text-center py-20 flex flex-col items-center gap-4">
                                    <div className="w-12 h-12 border-4 border-primary/20 border-t-primary rounded-full animate-spin"></div>
                                    <p className="text-lg font-bold text-gray-500">{lang === 'tr' ? 'Parçalar yükleniyor...' : 'Loading components...'}</p>
                                </div>
                            ) : paginatedItems.length === 0 ? (
                                <div className="bg-yellow-50 dark:bg-yellow-900/10 border border-yellow-200 dark:border-yellow-900/30 p-8 rounded-xl text-center">
                                    <span className="material-symbols-outlined text-4xl text-yellow-500 mb-2">sensor_off</span>
                                    <p className="text-yellow-600 dark:text-yellow-500 font-bold">{lang === 'tr' ? 'Bu filtrelere uygun parça bulunamadı.' : 'No components found matching your criteria.'}</p>
                                </div>
                            ) : (
                                paginatedItems.map(item => {
                                    const isSelected = currentBuild[currentCategory]?.id === item.id;
                                    const incReason = compatibilityInfo[item.id] && !isSelected;
                                    return (
                                        <div key={item.id} className={`bg-white dark:bg-card-dark p-4 rounded-xl border border-gray-200 dark:border-gray-700 flex flex-col sm:flex-row shadow-xl hover:shadow-2xl transition-all duration-300 gap-5 relative overflow-hidden group ${isSelected ? 'border-primary ring-2 ring-primary/20 bg-primary/5' : incReason ? 'opacity-60 grayscale-[0.8]' : ''}`}>
                                            <div className="w-full sm:w-28 h-28 bg-gray-50 dark:bg-gray-800/50 rounded-lg flex items-center justify-center p-2 flex-shrink-0 group-hover:scale-105 transition-transform">
                                                <img src={item.imageUrl || `https://placehold.co/150x150?text=${encodeURIComponent(item.brand)}`} className="max-h-full max-w-full object-contain" alt={item.modelName} />
                                            </div>
                                            <div className="flex-grow min-w-0">
                                                <div className="flex justify-between items-start">
                                                    <h4 className="font-bold text-lg text-gray-900 dark:text-white leading-tight truncate pr-2">{item.brand} {item.modelName}</h4>
                                                </div>
                                                <ComponentDetails item={item} category={currentCategory} t={t} lang={lang} />
                                                {incReason && (
                                                    <div className="mt-3 py-1 px-3 bg-red-500/10 border border-red-500/20 rounded-md inline-flex items-center gap-2">
                                                        <span className="material-symbols-outlined text-[16px] text-red-500">warning</span>
                                                        <p className="text-[11px] text-red-500 font-bold uppercase tracking-tight">{compatibilityInfo[item.id]}</p>
                                                    </div>
                                                )}
                                            </div>
                                            <div className="flex flex-col items-end min-w-[150px] justify-between border-t sm:border-t-0 sm:border-l border-gray-100 dark:border-gray-800/50 pt-4 sm:pt-0 sm:pl-5">
                                                <div className="text-right">
                                                    <p className="text-xs font-bold text-gray-400 dark:text-gray-500 uppercase tracking-widest">{t["summary-total"]}</p>
                                                    <p className="text-2xl font-black text-gray-900 dark:text-white">{(item.price || 0).toLocaleString()} TL</p>
                                                </div>
                                                <div className="flex items-center gap-2 w-full mt-4 sm:mt-0">
                                                    <button 
                                                        onClick={() => addToBuild(item)}
                                                        disabled={isSelected}
                                                        className={`flex-grow h-11 rounded-lg font-black text-sm uppercase tracking-wider transition-all shadow-lg ${isSelected ? 'bg-green-600 text-white cursor-default' : 'bg-primary text-white hover:bg-primary/90 shadow-primary/20'}`}
                                                    >
                                                        {isSelected ? t["action-selected"] : t["action-add"]}
                                                    </button>
                                                    <button className="fav-btn group-hover:scale-110 transition-transform">
                                                        <span className="material-symbols-outlined">favorite</span>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    );
                                })
                            )}
                        </div>

                        {/* Pagination Component */}
                        {totalPages > 1 && (
                            <div className="flex justify-center items-center gap-2 mt-12 mb-8 py-6 border-t border-gray-200 dark:border-gray-800/50">
                                <button onClick={() => setCurrentPage(p => Math.max(1, p-1))} disabled={currentPage === 1} className="w-10 h-10 flex items-center justify-center rounded-lg border dark:border-gray-700 disabled:opacity-30 hover:border-primary transition-colors"><span className="material-symbols-outlined">chevron_left</span></button>
                                {[...Array(totalPages)].map((_, i) => {
                                    const page = i + 1;
                                    if (page === 1 || page === totalPages || (page >= currentPage - 2 && page <= currentPage + 2)) {
                                        return <button key={page} onClick={() => setCurrentPage(page)} className={`w-10 h-10 rounded-lg border font-bold text-sm transition-all ${currentPage === page ? 'bg-primary border-primary text-white shadow-xl shadow-primary/30' : 'bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-300 hover:border-primary'}`}>{page}</button>;
                                    }
                                    if (page === currentPage - 3 || page === currentPage + 3) return <span key={page} className="text-gray-400">...</span>;
                                    return null;
                                })}
                                <button onClick={() => setCurrentPage(p => Math.min(totalPages, p+1))} disabled={currentPage === totalPages} className="w-10 h-10 flex items-center justify-center rounded-lg border dark:border-gray-700 disabled:opacity-30 hover:border-primary transition-colors"><span className="material-symbols-outlined">chevron_right</span></button>
                            </div>
                        )}
                    </div>

                    {/* Right Section: System Summary Sidebar */}
                    <aside className="w-full lg:w-1/4">
                        <div className="sticky top-10">
                            <div className="bg-card-light dark:bg-card-darker rounded-xl border border-gray-200 dark:border-gray-800/50 shadow-2xl flex flex-col overflow-hidden">
                                <div className="p-5 border-b border-gray-200 dark:border-gray-800/50 bg-gray-50/50 dark:bg-white/5">
                                    <h3 className="text-lg font-black dark:text-white uppercase tracking-tight flex items-center gap-2">
                                        <span className="material-symbols-outlined text-primary text-[22px]">inventory_2</span>
                                        {t["System-summary"]}
                                    </h3>
                                </div>
                                <div className="p-5 space-y-4 max-h-[50vh] overflow-y-auto custom-scrollbar">
                                    {Object.entries(currentBuild).filter(([_,it]) => it).length === 0 ? (
                                        <div className="text-center py-10 opacity-40">
                                            <span className="material-symbols-outlined text-5xl mb-2">construction</span>
                                            <p className="text-xs font-bold uppercase tracking-widest">{t["msg-no-parts"]}</p>
                                        </div>
                                    ) : (
                                        Object.entries(currentBuild).map(([type, item]) => item && (
                                            <div key={type} className="group flex items-center gap-3 p-2 rounded-xl border border-transparent hover:bg-white dark:hover:bg-white/5 hover:border-gray-100 dark:hover:border-gray-700 transition-all duration-200">
                                                <div className="h-12 w-12 flex-shrink-0 bg-white dark:bg-gray-900 border dark:border-gray-800 rounded-lg overflow-hidden flex items-center justify-center p-1.5 shadow-sm">
                                                    <img src={item.imageUrl || `https://via.placeholder.com/100?text=${type.substring(0,3).toUpperCase()}`} alt="" className="object-contain h-full w-full" />
                                                </div>
                                                <div className="flex-grow min-w-0">
                                                    <span className="text-[10px] uppercase font-black text-primary block leading-none mb-1 tracking-tighter">{t[`tab-${type === 'motherboard' ? 'mobo' : type === 'cpuCooler' ? 'cooler' : type}`]}</span>
                                                    <p className="text-xs font-bold text-gray-900 dark:text-gray-100 truncate pr-1">{item.modelName}</p>
                                                </div>
                                                <button onClick={() => removeFromBuild(type)} className="w-8 h-8 flex items-center justify-center text-gray-400 hover:text-white hover:bg-red-500 rounded-lg transition-all"><span className="material-symbols-outlined text-[18px]">close</span></button>
                                            </div>
                                        ))
                                    )}
                                </div>
                                <div className="p-6 border-t border-gray-200 dark:border-gray-800/50 bg-gray-50/50 dark:bg-white/5 space-y-4">
                                    <div className="space-y-2">
                                        <div className="flex justify-between text-[11px] font-black uppercase tracking-widest text-gray-500 dark:text-gray-400"><span>{t["summary-estimated"]}</span><span className="text-gray-900 dark:text-white">{totals.wattage} W</span></div>
                                        <div className="flex justify-between text-[11px] font-black uppercase tracking-widest text-gray-500 dark:text-gray-400"><span>{t["summary-recommended"]}</span><span className="text-green-600 dark:text-green-400">{totals.psu} W</span></div>
                                    </div>
                                    <div className="pt-4 border-t border-gray-200 dark:border-gray-800/50">
                                        <div className="flex justify-between items-baseline mb-6">
                                            <span className="text-sm font-black uppercase tracking-tighter dark:text-gray-300">{t["summary-total"]}</span>
                                            <span className="text-3xl font-black text-primary tracking-tighter">{(totals.price || 0).toLocaleString()} TL</span>
                                        </div>
                                        <button onClick={saveBuildAction} className="w-full bg-primary hover:bg-primary/90 text-white font-black py-4 px-4 rounded-xl flex items-center justify-center gap-3 transition-all shadow-xl shadow-primary/20 active:scale-95 uppercase tracking-widest text-xs">
                                            <span className="material-symbols-outlined text-[20px]">bookmark</span>
                                            {t["action-save-system"]}
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </aside>
                </div>
            </main>
        </div>
    );
}

/**
 * Sub-component for rendering the specs list based on component category
 */
function ComponentDetails({ item, category, t, lang }) {
    return (
        <ul className="mt-2 grid grid-cols-2 gap-x-2 gap-y-1 text-[11px] font-bold uppercase tracking-tight text-gray-500 dark:text-gray-400">
            {category === 'cpu' && (
                <><li><span className="opacity-60">{lang === 'tr' ? 'Soket' : 'Socket'}:</span> <span className="text-gray-700 dark:text-gray-300">{item.socket}</span></li><li><span className="opacity-60">{lang === 'tr' ? 'Çekirdek' : 'Cores'}:</span> <span className="text-gray-700 dark:text-gray-300">{item.coreCount}</span></li><li><span className="opacity-60">TDP:</span> <span className="text-gray-700 dark:text-gray-300">{item.tdp}W</span></li></>
            )}
            {category === 'motherboard' && (
                <><li><span className="opacity-60">{lang === 'tr' ? 'Soket' : 'Socket'}:</span> <span className="text-gray-700 dark:text-gray-300">{item.socket}</span></li><li><span className="opacity-60">RAM:</span> <span className="text-gray-700 dark:text-gray-300">{item.memoryType}</span></li><li><span className="opacity-60">FORM:</span> <span className="text-gray-700 dark:text-gray-300">{item.formFactor}</span></li></>
            )}
            {category === 'ram' && (
                <><li><span className="opacity-60">TIP:</span> <span className="text-gray-700 dark:text-gray-300">{item.memoryType}</span></li><li><span className="opacity-60">HIZ:</span> <span className="text-gray-700 dark:text-gray-300">{item.speed} MHz</span></li><li><span className="opacity-60">KIT:</span> <span className="text-gray-700 dark:text-gray-300">{item.capacityPerModule}Gx{item.moduleCount}</span></li></>
            )}
            {category === 'gpu' && (
                <><li><span className="opacity-60">VRAM:</span> <span className="text-gray-700 dark:text-gray-300">{item.vramMemorySize}GB</span></li><li><span className="opacity-60">{lang === 'tr' ? 'UZUNLUK' : 'LENGTH'}:</span> <span className="text-gray-700 dark:text-gray-300">{item.length}mm</span></li><li><span className="opacity-60">TDP:</span> <span className="text-gray-700 dark:text-gray-300">{item.tdp}W</span></li></>
            )}
            {category === 'case' && (
                <><li><span className="opacity-60">TIP:</span> <span className="text-gray-700 dark:text-gray-300">{item.caseType}</span></li><li><span className="opacity-60">GPU MAX:</span> <span className="text-gray-700 dark:text-gray-300">{item.maxGpuLength}mm</span></li></>
            )}
            {category === 'psu' && (
                 <><li><span className="opacity-60">{lang === 'tr' ? 'GÜÇ' : 'POWER'}:</span> <span className="text-gray-700 dark:text-gray-300">{item.wattage}W</span></li><li><span className="opacity-60">SERT.:</span> <span className="text-gray-700 dark:text-gray-300">{item.rating}</span></li></>
            )}
            {category === 'storage' && (
                <><li><span className="opacity-60">KAP.:</span> <span className="text-gray-700 dark:text-gray-300">{item.capacity}GB</span></li><li><span className="opacity-60">TIP:</span> <span className="text-gray-700 dark:text-gray-300">{item.storageType}</span></li></>
            )}
            {category === 'cpuCooler' && (
                <><li><span className="opacity-60">TIP:</span> <span className="text-gray-700 dark:text-gray-300">{t[`filter-${item.coolerType}`] || item.coolerType}</span></li>{item.radiatorSize && <li><span className="opacity-60">RAD:</span> <span className="text-gray-700 dark:text-gray-300">{item.radiatorSize}mm</span></li>}</>
            )}
        </ul>
    );
}


function getIconForCategory(cat) {
    switch(cat) {
        case 'cpu': return 'memory';
        case 'motherboard': return 'developer_board';
        case 'ram': return 'straighten';
        case 'gpu': return 'videogame_asset';
        case 'storage': return 'sd_storage';
        case 'case': return 'computer';
        case 'psu': return 'power';
        case 'cpuCooler': return 'mode_fan';
        default: return 'build';
    }
}

function formatFilterValue(prop, val, t) {
    if (val === 'true') return t['filter-true'] || 'Yes';
    if (val === 'false') return t['filter-false'] || 'No';
    return t[`filter-${val}`] || val;
}
